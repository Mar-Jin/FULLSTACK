# Database-analyse RevaliNow

Dit document beschrijft hoe de database bedrijfsregels, beveiliging, traceerbaarheid en performance regelt, met de metingen die de keuzes onderbouwen. Alles wordt aangemaakt via EF Core-migraties in `Server/Migrations/`, dus bij `docker compose up` staat het er vanzelf in.

De testscripts staan in [`docs/sql/`](sql/) en kun je in SSMS draaien als beheerder (`sa`) op de database `RevaliNow`.

| Migratie | Inhoud |
|---|---|
| `InitialCreate` | Tabellen, foreign keys, CHECK-constraints, unieke indexes |
| `AuditTrail` | Tabel `AuditLogs` en audit-triggers |
| `TemporalEnIndexes` | Temporal tables en samengestelde indexes |
| `ValidatieTriggers` | Triggers voor bedrijfsregels |
| `ViewsEnProcedures` | Views en stored procedures |
| `RollenEnRowLevelSecurity` | Databaserollen en row-level security |

---

## 1. Data-integriteit

### Constraints
De database bewaakt de regels zelf, ook als iemand buiten de API om data wijzigt (bijvoorbeeld via SSMS).

- **CHECK-constraints** (24), bijvoorbeeld `PijnScore BETWEEN 0 AND 10`, `EindDatum >= StartDatum`, geldige statussen en rollen.
- **Rol en koppeling van een account:** `CK_Gebruikers_PatientId` zorgt dat alleen accounts met rol `Patient` aan een patiënt gekoppeld zijn, en `CK_Gebruikers_OrganisatieId` doet hetzelfde voor medewerkers van een zorgverzekeraar.
- **Unieke indexes**, bijvoorbeeld één account per patiënt (gefilterd op `PatientId IS NOT NULL`), en één verwerking per declaratie.
- **Foreign keys** met `ON DELETE RESTRICT`: medische gegevens worden nooit automatisch mee verwijderd.

### Triggers
Triggers zijn er voor regels die een CHECK-constraint niet kan afdwingen, omdat ze over meerdere rijen of tabellen gaan. Een overtreding breekt de transactie af met `THROW`. De API vertaalt foutnummers 50000–50999 naar een 400 met de melding uit de trigger.

| Trigger | Regel | Fout |
|---|---|---|
| `TR_UitgevoerdeOefeningen_Validatie` | Afvinken alleen binnen de looptijd van het oefenplan | 50001 |
| | Niet in de toekomst afvinken | 50002 |
| | Per dag niet vaker afvinken dan `FrequentiePerDag`, niet vaker ongedaan maken dan afgevinkt | 50003 |
| `TR_PijnIndicaties_Validatie` | Pijn niet in de toekomst registreren | 50010 |
| `TR_Patienten_Rolcontrole` | `HuisartsId` moet een gebruiker met rol Huisarts zijn | 50020 |
| `TR_Afspraken_Rolcontrole` | `RevalidatieartsId` moet een Revalidatiearts zijn | 50021 |
| `TR_Oefenplannen_Rolcontrole` | idem voor oefenplannen | 50022 |

**Waarom een trigger en niet alleen validatie in de API?** API-validatie beschermt alleen verzoeken die via de API binnenkomen. Een trigger beschermt ook een directe query, bijvoorbeeld van een collega via SSMS of van een andere applicatie in het landschap. De API controleert dezelfde regels ook, zodat de gebruiker een duidelijke melding krijgt. Dat is defense in depth.

**De rolcontroles** zijn nodig omdat een foreign key alleen controleert dát een gebruiker bestaat, niet welke rol hij heeft.

**De dataset** bevat historische afwijkingen: 212 afvinkingen buiten de looptijd en pijnregistraties met een datum in de toekomst. De validatie-triggers slaan het inlezen daarom over (`SESSION_CONTEXT('Seeding') = 1`). De rolcontroles draaien wél tijdens het seeden, zodat de dataset zelf ook gecontroleerd wordt.

**Getest** met [`sql/3-triggers-en-temporal.sql`](sql/3-triggers-en-temporal.sql). Elke ongeldige wijziging wordt geweigerd:
```
Geweigerd (50001): Afvinken kan alleen binnen de looptijd van het oefenplan.
Geweigerd (50002): Een oefening kan niet in de toekomst worden afgevinkt.
Eerste keer: toegestaan
Tweede keer geweigerd (50003): Deze oefening is vandaag al vaak genoeg gedaan, ...
Geweigerd (50010): Een pijnindicatie kan niet in de toekomst worden geregistreerd.
Geweigerd (50021): Een afspraak moet bij een gebruiker met de rol Revalidatiearts zijn.
Geweigerd (547):   The INSERT statement conflicted with the CHECK constraint "CK_PijnIndicaties_PijnScore".
```

---

## 2. Beveiliging: rollen en row-level security

### Twee verbindingen
- **Beheer (`sa`):** alleen bij het opstarten, voor migraties, seeden en het aanmaken van de API-login.
- **API (`revalinow_api`):** alles wat de app daarna doet. Deze login is lid van `rol_api` en heeft verder geen rechten.

### Rollen (least privilege)
| Rol | Rechten |
|---|---|
| `rol_api` | `SELECT` op de tabellen die RevaliNow toont; `INSERT` op pijn en afvinken; `INSERT/UPDATE/DELETE` op het logboek; `SELECT` op de views; `EXECUTE` op de procedures. **Geen** rechten op `AuditLogs` en geen schrijfrechten op `Gebruikers`. |
| `rol_auditor` | Alleen lezen: `AuditLogs`, de historietabellen, `Logboek`, `PijnIndicaties`. |

De audit-triggers schrijven wel in `AuditLogs`, ook als de API de wijziging doet. Dat werkt via *ownership chaining*: de trigger en de tabel hebben dezelfde eigenaar. De API zelf kan de audit trail dus niet lezen, wijzigen of wissen.

### Row-level security
De API zet bij elke databaseverbinding wie er is ingelogd in `SESSION_CONTEXT` (`GebruikerId`, `Rol`, `PatientId`), met `@read_only = 1`. Daarna kan niemand de waarden binnen die verbinding nog aanpassen, ook niet via een SQL-injectie. De security policy `Beveiliging.PatientgegevensBeleid` filtert met die waarden elke query:

| Wie | Ziet |
|---|---|
| Patiënt | Alleen zijn eigen rijen |
| Huisarts | Zijn eigen patiënten (`Patienten.HuisartsId`) |
| Revalidatiearts | Gekoppelde patiënten (`PatientRevalidatieartsen`) |
| Beheer (`db_owner`) / `rol_auditor` | Alles |
| Zonder sessiecontext | Niets |

- **FILTER**-predicaten bepalen welke rijen je ziet bij SELECT, UPDATE en DELETE. Ze staan op alle tabellen met patiëntgegevens, inclusief de historietabellen.
- **BLOCK**-predicaten (`AFTER INSERT`/`AFTER UPDATE`) voorkomen dat je een rij toevoegt of wijzigt die bij een ander hoort.

**Waarom naast de controle in de API?** De API weigert een verkeerd `patientId` al met een 403, vóórdat er een query wordt uitgevoerd (`[AlleenEigenPatient]`). Row-level security is de tweede verdediging: als er ooit een fout in een endpoint zit, of als iemand het API-wachtwoord gebruikt in SSMS, geeft de database toch geen gegevens van een andere cliënt terug.

**Valkuil die we tegenkwamen:** SQL Server past row-level security **niet** toe op tabellen die je binnen een predicaatfunctie opvraagt. Een eerste versie van `fn_OefenplanToegang` controleerde alleen "bestaat het oefenplan", en liet daardoor alle 28.325 uitgevoerde oefeningen door. De functie controleert nu expliciet via `fn_Toegang` of je bij de patiënt van het oefenplan mag. Gevonden met de test hieronder.

**Getest** met [`sql/4-row-level-security.sql`](sql/4-row-level-security.sql):
```
1. API-login zonder sessiecontext:  Patienten 0, PijnIndicaties 0, UitgevoerdeOefeningen 0
2. Als patiënt 229:                 Patienten 1, PijnIndicaties 5, waarvan van een ander 0, Oefenplannen 3
3. Pijn registreren voor patiënt 1: geweigerd (block predicate)
4. Logboek van patiënt 1 wissen:    0 rijen verwijderd (filter predicate)
5. AuditLogs lezen:                 geweigerd (SELECT permission denied)
6. Gebruikers wijzigen:             geweigerd (UPDATE permission denied)
7. Als huisarts 1:                  27 patiënten = precies zijn eigen 27 patiënten
```
Daarnaast getest als patiënt 1: die ziet zijn eigen 471 uitgevoerde oefeningen, en 0 van anderen.

---

## 3. Traceerbaarheid

### Temporal tables
`Logboek` en `PijnIndicaties` zijn *system-versioned temporal tables*. SQL Server bewaart automatisch elke vorige versie van een rij in `LogboekHistorie` en `PijnIndicatiesHistorie`, met de periode waarin die versie geldig was (`GeldigVanaf`/`GeldigTot`, altijd in UTC).

**Waarom deze twee tabellen?** Dit zijn de tabellen die de cliënt zelf bewerkt of verwijdert. Zo is altijd terug te zien hoe een logboekregel eruitzag vóór een bewerking, of welke pijnscore er stond vóór een correctie.

Voorbeeld: een regel aanmaken, bewerken en verwijderen, daarna `FOR SYSTEM_TIME ALL`:
```
Id   Beschrijving              GeldigVanaf          GeldigTot
2339 Lange wandeling gemaakt…  2026-10-28 23:18:24  2026-10-28 23:19:17
2339 temporal-test: bewerkt    2026-10-28 23:19:17  2026-10-28 23:19:18
Nog in Logboek: 0
```

### Audit trail
De audit trail (user story 8) is een andere vraag dan de temporal tables. Temporal tables vertellen *hoe de data eruitzag*. De audit trail vertelt *wie wat deed*: actie, tabel, record, cliënt, gebruiker en tijdstip, in `AuditLogs`. De triggers `TR_*_Audit` vullen die tabel voor pijnindicaties, logboek en afvinken. Een wijziging buiten de API om wordt ook gelogd, met `GebruikerId` leeg en de SQL-login in `DatabaseGebruiker`.

---

## 4. Performance

### Methode
Dezelfde drie queries die de API het vaakst uitvoert, gemeten vóór en na de indexes met `SET STATISTICS IO` (logical reads, oftewel het aantal gelezen pagina's van 8 KB) en `SET STATISTICS PROFILE` (het execution plan). Script: [`sql/1-performance-meting.sql`](sql/1-performance-meting.sql). Patiënt 229 op de volledige dataset.

### Resultaat
| Query | Vóór: plan | Vóór: reads | Na: plan | Na: reads |
|---|---|---|---|---|
| Afvinkingen vandaag per oefenplan | Clustered Index **Scan** op 28.325 rijen, Hash Match, Sort | **143** | Index Seek | **6** |
| Pijn van een patiënt, nieuwste eerst | Index Seek + **Key Lookup** per rij + **Sort** | **12** | Index Seek | **2** |
| Eerstvolgende afspraak | Clustered Index **Scan** + Sort | **15** | Index Seek | **2** |

### De indexes en waarom
| Index | Waarom |
|---|---|
| `UitgevoerdeOefeningen (OefenplanId, DatumTijdAfgevinkt) INCLUDE (IsAfgevinkt)` | Grootste tabel. De voortgang van vandaag zoekt per oefenplan binnen één dag. Zonder deze index las SQL Server de hele tabel. Met `IsAfgevinkt` in de index hoeft de tabel zelf niet gelezen te worden. |
| `PijnIndicaties (PatientId, DatumTijdRegistratie DESC) INCLUDE (PijnScore, Toelichting)` | Dashboard en grafiek: pijn van één cliënt, nieuwste eerst. De index staat al in de goede volgorde (geen Sort) en bevat alle kolommen (geen Key Lookup). |
| `Afspraken (PatientId, DatumTijdAfspraak) INCLUDE (…)` | Eerstvolgende afspraak en het afsprakenoverzicht. |
| `Logboek (PatientId, DatumTijdActiviteit DESC) INCLUDE (Beschrijving)` | Logboek van één cliënt, nieuwste eerst. |

Elke index begint met de kolom van de foreign key. EF verwijdert daarom de losse index op die foreign key: er komt per tabel **geen extra** index bij.

### Effect op schrijven
Een index maakt elke INSERT iets duurder, want de index moet worden bijgewerkt. Gemeten met [`sql/2-schrijfkosten-index.sql`](sql/2-schrijfkosten-index.sql): 3 keer 2000 INSERTs in `PijnIndicaties`, met en zonder de nieuwe index, in transacties die daarna worden teruggedraaid.

| | Met index | Zonder index |
|---|---|---|
| Gemiddeld per 2000 inserts | 205 ms | 207 ms |

Het verschil valt binnen de meetruis. Dat past bij de situatie: het is één index per tabel, en pijnregistraties, logboekregels en afvinkingen komen één voor één binnen, niet in bulk. De leeswinst op elke pagina die een cliënt opent, weegt daar ruim tegenop.

### Kosten van row-level security
In het plan na de indexes zie je extra seeks op `Patienten` en `PatientRevalidatieartsen`. Dat zijn de RLS-predicaten. Het zijn seeks op primary keys en unieke indexes, dus een kleine en vaste prijs per query. Die betalen we bewust voor de extra beveiligingslaag.

### Views en stored procedures
Het doel is minder round-trips naar de database, en de logica op één plek.

| Object | Gebruikt door | Vóór | Na |
|---|---|---|---|
| `vw_OefenplanVoortgang` | `GET /oefenplannen` | 2 queries (oefenplannen, daarna de tellingen) en samenvoegen in C# | 1 query |
| `vw_AfspraakOverzicht` | `GET /afspraken` en het dashboard | Join in C# via `Include` | 1 query, herbruikbaar |
| `sp_OefeningAfvinken` | `POST /afvinkingen` | 3 round-trips: plan ophalen, tellen, invoegen | 1 aanroep |
| `sp_DashboardSamenvatting` | `GET /dashboard` | Pijn en afspraken apart ophalen en in C# berekenen | 1 aanroep |
| `sp_PijnverloopPerWeek` | Grafiek "Weekgemiddelde" | Alle metingen naar de browser | Alleen één rij per week |

`sp_OefeningAfvinken` doet nog iets wat losse queries niet kunnen: met `UPDLOCK, HOLDLOCK` worden twee snelle klikken op hetzelfde oefenplan **na elkaar** verwerkt. Bij losse queries konden twee verzoeken tegelijk "nog 1 keer te gaan" zien en daarna allebei afvinken. Een view of procedure gebruikt dezelfde indexes en filtert ook via row-level security.
