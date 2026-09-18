-- CREATE
CREATE DATABASE RevaliDB;
GO

USE RevaliDB;
GO

PRINT('DROP+CREATE ALL TABLES
');

DROP TABLE IF EXISTS dbo.zorgverzekeraars;
CREATE TABLE dbo.zorgverzekeraars (
    id INT PRIMARY KEY,
    naam VARCHAR(100) NOT NULL
);
PRINT('''zorgverzekeraars'' table created

');
GO

DROP TABLE IF EXISTS dbo.gebruikers;
CREATE TABLE dbo.gebruikers (
    id INT PRIMARY KEY,
    gebruikersnaam VARCHAR(50) NOT NULL,
    wachtwoord_hash VARCHAR(255) NOT NULL,
    rol VARCHAR(50) NOT NULL,
    voornaam VARCHAR(50) NOT NULL,
    achternaam VARCHAR(50) NOT NULL,
    organisatie_id INT NULL,
	FOREIGN KEY (organisatie_id) REFERENCES zorgverzekeraars(id),
);
PRINT('''gebruikers'' table created

');
GO

DROP TABLE IF EXISTS dbo.patienten;
CREATE TABLE dbo.patienten (
    id INT PRIMARY KEY,
    voornaam VARCHAR(50),
    achternaam VARCHAR(50),
    geboortedatum DATE,
    adres VARCHAR(100),
    telefoonnummer VARCHAR(15),
    email VARCHAR(100),
    huisarts_id INT,
	FOREIGN KEY (huisarts_id) REFERENCES gebruikers(id)
);
PRINT('''patienten'' table created

');
GO

DROP TABLE IF EXISTS dbo.accessoires;
CREATE TABLE dbo.accessoires (
    id INT PRIMARY KEY,
    patient_id INT NOT NULL,
    huisarts_id INT NOT NULL,
    naam VARCHAR(100) NOT NULL,
    advies_datum DATE NOT NULL,
    verwachte_gebruiksperiode VARCHAR(50) NOT NULL,
    status VARCHAR(20) NOT NULL,
	FOREIGN KEY (patient_id) REFERENCES patienten(id),
	FOREIGN KEY (huisarts_id) REFERENCES gebruikers(id)
);
PRINT('''accessoires'' table created

');
GO

DROP TABLE IF EXISTS dbo.activiteiten_logboek;
CREATE TABLE dbo.activiteiten_logboek (
    id INT PRIMARY KEY,
    patient_id INT NOT NULL,
    datum_tijd_activiteit DATETIME NOT NULL,
    beschrijving VARCHAR(255) NOT NULL,
	FOREIGN KEY (patient_id) REFERENCES patienten(id)
);
PRINT('''activiteiten_logboek'' table created

');
GO

DROP TABLE IF EXISTS dbo.afspraken;
CREATE TABLE dbo.afspraken (
	id INT PRIMARY KEY,
    patient_id INT NOT NULL,
    revalidatiearts_id INT NOT NULL,
    datum_tijd_afspraak DATETIME NOT NULL,
    duur_minuten INT NOT NULL,
    type_afspraak VARCHAR(50) NOT NULL,
    status VARCHAR(20) NOT NULL,
	FOREIGN KEY (patient_id) REFERENCES patienten(id),
	FOREIGN KEY (revalidatiearts_id) REFERENCES gebruikers(id)
);
PRINT('''afspraken'' table created

');
GO

DROP TABLE IF EXISTS dbo.declaraties;
CREATE TABLE dbo.declaraties (
    id INT PRIMARY KEY,
    patient_id INT NOT NULL,
    type_declaratie VARCHAR(50) NOT NULL,
    gedeclareerd_door_gebruiker_id INT NOT NULL,
    datum_handeling DATE NOT NULL,
    omschrijving VARCHAR(255) NOT NULL,
    gedeclareerd_bedrag DECIMAL(10,2) NOT NULL,
    status VARCHAR(20) NOT NULL,
	FOREIGN KEY (patient_id) REFERENCES patienten(id),
	FOREIGN KEY (gedeclareerd_door_gebruiker_id) REFERENCES gebruikers(id)
);
PRINT('''declaraties'' table created

');
GO

DROP TABLE IF EXISTS dbo.declaratie_verwerkingen;
CREATE TABLE dbo.declaratie_verwerkingen (
    id INT PRIMARY KEY,
    declaratie_id INT,
    zorgverzekeraar_medewerker_id INT,
    datum_verwerking DATETIME,
    status_verwerking VARCHAR(50),
    vergoed_bedrag INT,
    afgekeurd_bedrag INT,
    toelichting VARCHAR(255),
	FOREIGN KEY (declaratie_id) REFERENCES declaraties(id),
	FOREIGN KEY (zorgverzekeraar_medewerker_id) REFERENCES gebruikers(id)
);
PRINT('''declaratie_verwerkingen'' table created

');
GO


DROP TABLE IF EXISTS dbo.intakes;
CREATE TABLE dbo.intakes (
    id INT PRIMARY KEY,
    patient_id INT,
    revalidatiearts_id INT,
    diagnose VARCHAR(255),
    ernst_blessure VARCHAR(50),
    behandeldoelen TEXT,
    datum_intake DATE,
	FOREIGN KEY (patient_id) REFERENCES patienten(id),
	FOREIGN KEY (revalidatiearts_id) REFERENCES gebruikers(id)
);
PRINT('''intakes'' table created

');
GO

DROP TABLE IF EXISTS dbo.medicatie;
CREATE TABLE dbo.medicatie (
    patient_id INT NOT NULL,
    huisarts_id INT NOT NULL,
    naam VARCHAR(100) NOT NULL,
    dosering VARCHAR(50) NOT NULL,
    frequentie VARCHAR(50) NOT NULL,
    start_datum DATE NOT NULL,
    eind_datum DATE NOT NULL,
    status VARCHAR(50) NOT NULL,
	FOREIGN KEY (patient_id) REFERENCES patienten(id),
	FOREIGN KEY (huisarts_id) REFERENCES gebruikers(id)
);
PRINT('''medicatie'' table created

');
GO

DROP TABLE IF EXISTS dbo.notities;
CREATE TABLE dbo.notities (
    id INT PRIMARY KEY,
    patient_id INT,
    auteur_gebruiker_id INT,
    datum_tijd_notitie DATETIME,
    inhoud TEXT,
	FOREIGN KEY (patient_id) REFERENCES patienten(id),
	FOREIGN KEY (auteur_gebruiker_id) REFERENCES gebruikers(id)
);
PRINT('''notities'' table created

');
GO

DROP TABLE IF EXISTS dbo.oefeningen;
CREATE TABLE dbo.oefeningen (
    id INT PRIMARY KEY,
    naam VARCHAR(100),
    beschrijving TEXT,
    instructie_video_url VARCHAR(255)
);
PRINT('''oefeningen'' table created

');
GO

DROP TABLE IF EXISTS dbo.oefenplannen;
CREATE TABLE dbo.oefenplannen (
    id INT PRIMARY KEY,
    patient_id INT,
    oefening_id INT,
    revalidatie_arts_id INT,
    start_datum DATE,
    eind_datum DATE,
    herhalingen INT,
    sets INT,
    frequentie_per_dag INT,
    opmerkingen TEXT,
	FOREIGN KEY (patient_id) REFERENCES patienten(id),
	FOREIGN KEY (oefening_id) REFERENCES oefeningen(id),
	FOREIGN KEY (revalidatie_arts_id) REFERENCES gebruikers(id)
);
PRINT('''oefenplannen'' table created

');
GO


DROP TABLE IF EXISTS dbo.vergoedingsregels;
CREATE TABLE dbo.vergoedingsregels (
    id INT PRIMARY KEY,
    naam VARCHAR(100) NOT NULL,
    max_vergoeding VARCHAR(50),
    max_vergoed_bedrag DECIMAL(10,2),
    max_vergoed_percentage DECIMAL(5,2),
    voorwaarden TEXT,
    geldig_vanaf DATE,
    geldig_tot DATE,
    actief BIT
);
PRINT('''vergoedingsregels'' table created

');
GO

DROP TABLE IF EXISTS dbo.uitgevoerde_oefeningen;
CREATE TABLE dbo.uitgevoerde_oefeningen (
    id INT PRIMARY KEY,
    patient_oefenplan_id INT NOT NULL,
    datum_tijd_afgevinkt DATETIME,
    is_afgevinkt BIT,
	FOREIGN KEY (patient_oefenplan_id) REFERENCES oefenplannen(id)
);
PRINT('''uitgevoerde_oefeningen'' table created

');
GO

DROP TABLE IF EXISTS dbo.revalidatieartsen;
CREATE TABLE dbo.revalidatieartsen (
    id INT PRIMARY KEY,
    patient_id INT NOT NULL,
    revalidatiearts_id INT NOT NULL,
	FOREIGN KEY (patient_id) REFERENCES patienten(id),
	FOREIGN KEY (revalidatiearts_id) REFERENCES gebruikers(id)
);
PRINT('''revalidatieartsen'' table created

');
GO

DROP TABLE IF EXISTS dbo.pijnindicaties;
CREATE TABLE dbo.pijnindicaties (
    id INT PRIMARY KEY,
    patient_id INT NOT NULL,
    datum_tijd_registratie DATETIME NOT NULL,
    pijn_score INT NOT NULL,
    toelichting VARCHAR(255),
	FOREIGN KEY (patient_id) REFERENCES patienten(id)
);
PRINT('''pijnindicaties'' table created

');
GO

-- INSERTS
PRINT('TABLE INSERTS');


INSERT INTO zorgverzekeraars (id, naam) VALUES
(1, 'ZorgDirect'),
(2, 'GezondVooruit');

PRINT('inserted into zorgverzekeraars
');

INSERT INTO gebruikers 
(id, gebruikersnaam, wachtwoord_hash, rol, voornaam, achternaam, organisatie_id) VALUES
(1, 'zvm_jansen', 'password123', 'Zorgverzekeraar', 'Sophie', 'Jansen', 1),
(2, 'zvm_devries', 'password123', 'Zorgverzekeraar', 'Mark', 'de Vries', 2),
(3, 'ha_bakker', 'password123', 'Huisarts', 'Lisa', 'Bakker', NULL),
(4, 'ha_janssen', 'password123', 'Huisarts', 'Thomas', 'Janssen', NULL),
(5, 'ra_smit', 'password123', 'Revalidatiearts', 'Emma', 'Smit', NULL),
(6, 'ra_groen', 'password123', 'Revalidatiearts', 'David', 'Groen', NULL),
(7, 'ra_visser', 'password123', 'Revalidatiearts', 'Linda', 'Visser', NULL);

PRINT('inserted into gebruikers
');

INSERT INTO patienten (id, voornaam, achternaam, geboortedatum, adres, telefoonnummer, email, huisarts_id) VALUES
(1, 'Freddy', 'Voetballer', '1995-03-15', 'Kruisbandstraat 10', '612345678', 'freddy.v@example.com', 3),
(2, 'Anna', 'Kamer', '1988-11-22', 'Therapiestraat 5', '623456789', 'anna.k@example.com', 4),
(3, 'Bas', 'Verkade', '1976-07-01', 'Oefenlaan 12', '634567890', 'bas.v@example.com', 3),
(4, 'Carla', 'Dirksen', '2001-01-30', 'Herstelweg 3', '645678901', 'carla.d@example.com', 4),
(5, 'Dirk', 'Meijer', '1965-09-10', 'Revalidatieplein 7', '656789012', 'dirk.m@example.com', 3),
(6, 'Eva', 'Peters', '1999-04-05', 'Fysioweg 22', '667890123', 'eva.p@example.com', 4),
(7, 'Frank', 'de Jong', '1970-02-18', 'Gipsstraat 1', '678901234', 'frank.dj@example.com', 3),
(8, 'Greetje', 'Willems', '1985-12-03', 'Rolstoelpad 8', '689012345', 'greetje.w@example.com', 4),
(9, 'Hans', 'Kramer', '1993-06-25', 'Sportlaan 15', '690123456', 'hans.k@example.com', 3),
(10, 'Irene', 'Van Dam', '1972-10-14', 'Zorgcentrum 4', '601234567', 'irene.vd@example.com', 4),
(11, 'Jeroen', 'Faber', '1980-08-08', 'Gezondheidsstraat 2', '611223344', 'jeroen.f@example.com', 3),
(12, 'Kim', 'Schouten', '1997-01-20', 'Pijnbestrijding 1', '622334455', 'kim.s@example.com', 4),
(13, 'Lars', 'Veenstra', '1960-05-12', 'Welzijnshof 9', '633445566', 'lars.v@example.com', 3),
(14, 'Mieke', 'Bos', '1990-03-03', 'Bewegingstraat 6', '644556677', 'mieke.b@example.com', 4),
(15, 'Niels', 'Dekker', '1978-07-28', 'Vitaliteitslaan 11', '655667788', 'niels.d@example.com', 3),
(16, 'Olga', 'Postma', '1983-09-09', 'Herstelstraat 14', '666778899', 'olga.p@example.com', 4),
(17, 'Paul', 'Kuipers', '1992-11-11', 'Zorgpad 1', '677889900', 'paul.k@example.com', 3),
(18, 'Quinn', 'Vries', '1975-04-04', 'Medicinelaan 3', '688990011', 'quinn.v@example.com', 4),
(19, 'Ruben', 'Smeets', '1987-02-02', 'Oefenplein 5', '699001122', 'ruben.s@example.com', 3),
(20, 'Sarah', 'Koks', '1994-08-18', 'Genezingsweg 10', '600112233', 'sarah.k@example.com', 4),
(21, 'Tim', 'Visser', '1968-10-25', 'Sportlaan 2', '610203040', 'tim.v@example.com', 3),
(22, 'Ursula', 'Bouwman', '1981-06-07', 'Kwaliteitsstraat 7', '620304050', 'ursula.b@example.com', 4),
(23, 'Vincent', 'Meijerink', '1996-03-29', 'Zorglaan 9', '630405060', 'vincent.m@example.com', 3),
(24, 'Wendy', 'Van der Velde', '1979-01-13', 'Therapieplein 12', '640506070', 'wendy.vdv@example.com', 4),
(25, 'Xavier', 'De Ruiter', '1986-05-05', 'Revalidatiepad 15', '650607080', 'xavier.dr@example.com', 3);

PRINT('inserted into patienten
');

INSERT INTO dbo.accessoires (id, patient_id, huisarts_id, naam, advies_datum, verwachte_gebruiksperiode, status) VALUES
(1, 1, 3, 'Krukken', '2025-01-09', '6 weken', 'Actief'),
(2, 1, 3, 'Kniebrace', '2025-01-15', '12 weken', 'Actief'),
(3, 2, 4, 'Rugbrace', '2025-02-01', '8 weken', 'Actief'),
(4, 3, 3, 'Schouderbrace', '2025-01-16', '4 weken', 'Afgerond'),
(5, 4, 4, 'Enkelbrace', '2025-02-06', '6 weken', 'Actief'),
(6, 5, 3, 'Nekbrace', '2025-03-03', '8 weken', 'Actief'),
(7, 6, 4, 'Fysiotherapie banden', '2025-02-12', 'Onbepaald', 'Actief'),
(8, 7, 3, 'Meniscusbrace', '2025-03-07', '10 weken', 'Actief'),
(9, 8, 4, 'TENS apparaat', '2025-04-03', 'Onbepaald', 'Actief'),
(10, 9, 3, 'Polsbrace', '2025-03-12', '4 weken', 'Afgerond'),
(11, 10, 4, 'Kniebrace', '2025-04-07', '8 weken', 'Actief'),
(12, 11, 3, 'Rugkussen', '2025-04-11', 'Onbepaald', 'Actief'),
(13, 12, 4, 'Enkelgewichtjes', '2025-04-16', '6 weken', 'Actief'),
(14, 13, 3, 'Looprek', '2025-04-21', '12 weken', 'Actief'),
(15, 14, 4, 'Schouderkatrol', '2025-04-26', '8 weken', 'Actief'),
(16, 15, 3, 'Knieband', '2025-05-02', '6 weken', 'Actief'),
(17, 16, 4, 'Enkelfles', '2025-05-06', 'Onbepaald', 'Actief'),
(18, 17, 3, 'Krukken', '2025-05-11', '4 weken', 'Actief'),
(19, 18, 4, 'Rugbrace', '2025-05-16', '8 weken', 'Actief'),
(20, 19, 3, 'Schouderriem', '2025-05-21', '6 weken', 'Actief'),
(21, 20, 4, 'Enkelondersteuning', '2025-05-26', '4 weken', 'Actief'),
(22, 21, 3, 'Kniebrace', '2025-05-29', '10 weken', 'Actief'),
(23, 22, 4, 'Hielspoorzooltjes', '2025-05-31', 'Onbepaald', 'Actief'),
(24, 23, 3, 'Rolstoel', '2025-06-02', '12 weken', 'Actief'),
(25, 24, 4, 'Handtrainer', '2025-06-06', 'Onbepaald', 'Actief');

PRINT('inserted into accessoires
');

INSERT INTO activiteiten_logboek (id, patient_id, datum_tijd_activiteit, beschrijving) VALUES
(1, 1, '2025-01-10 12:00', 'Rustdag na operatie.'),
(2, 1, '2025-01-11 15:00', 'Korte wandeling met krukken rondom huis.'),
(3, 1, '2025-01-12 10:00', 'Huishoudelijke taken (afwas).'),
(4, 1, '2025-01-12 16:00', 'Boodschappen gedaan met hulp.'),
(5, 1, '2025-01-13 11:00', '30 minuten fysiotherapie oefeningen gedaan.'),
(6, 1, '2025-01-13 17:00', 'Korte fietstocht op hometrainer (15 min).'),
(7, 1, '2025-01-14 09:00', 'Oefeningen volgens schema uitgevoerd.'),
(8, 1, '2025-01-14 14:00', 'Koken en voorbereiden maaltijden.'),
(9, 1, '2025-01-15 10:00', 'Oefeningen gedaan'),
(10, 1, '2025-01-15 16:00', 'Wandeling buiten'),
(11, 2, '2025-02-01 14:00', 'Eerste lichte oefeningen voor rug.'),
(12, 2, '2025-02-02 09:00', 'Half uur gefietst op hometrainer.'),
(13, 2, '2025-02-02 18:00', 'Korte wandeling buiten'),
(14, 2, '2025-02-03 10:00', 'Rug oefeningen gedaan'),
(15, 2, '2025-02-03 15:00', 'Wandeling met hond.'),
(16, 3, '2025-01-16 09:00', 'Oefeningen voor schouder mobiliteit.'),
(17, 3, '2025-01-17 11:00', 'Lichte huishoudelijke taken.'),
(18, 4, '2025-02-06 10:00', 'Enkel oefeningen volgens schema.'),
(19, 4, '2025-02-07 14:00', 'Korte wandeling binnenshuis.'),
(20, 5, '2025-03-03 10:00', 'Nekoefeningen met elastiek.'),
(21, 5, '2025-03-04 15:00', 'Rustig gewandeld.');

PRINT('inserted into activiteiten_logboek
');

INSERT INTO afspraken (id, patient_id, revalidatiearts_id, datum_tijd_afspraak, duur_minuten, type_afspraak, status) VALUES
(1, 1, 5, '2025-01-10 14:00', 60, 'intake', 'afgerond'),
(2, 1, 5, '2025-01-31 10:00', 45, 'controle', 'afgerond'),
(3, 1, 5, '2025-02-21 11:00', 45, 'controle', 'afgerond'),
(4, 1, 5, '2025-03-14 13:00', 30, 'controle', 'afgerond'),
(5, 1, 5, '2025-04-04 09:00', 30, 'controle', 'gepland'),
(6, 1, 5, '2025-04-25 10:00', 30, 'controle', 'gepland'),
(7, 2, 6, '2025-02-01 09:00', 60, 'intake', 'afgerond'),
(8, 2, 6, '2025-02-22 14:00', 45, 'controle', 'afgerond'),
(9, 2, 6, '2025-03-15 10:00', 45, 'controle', 'gepland'),
(10, 2, 6, '2025-04-05 11:00', 30, 'controle', 'gepland'),
(11, 3, 5, '2025-01-15 16:00', 60, 'intake', 'afgerond'),
(12, 3, 5, '2025-02-05 15:00', 45, 'controle', 'afgerond'),
(13, 3, 5, '2025-02-26 14:00', 30, 'controle', 'gepland'),
(14, 4, 7, '2025-02-05 10:00', 60, 'intake', 'afgerond'),
(15, 4, 7, '2025-02-26 11:00', 45, 'controle', 'afgerond'),
(16, 4, 7, '2025-03-19 13:00', 30, 'controle', 'gepland'),
(17, 5, 6, '2025-03-01 09:00', 60, 'intake', 'afgerond'),
(18, 5, 6, '2025-03-22 10:00', 45, 'controle', 'gepland'),
(19, 6, 5, '2025-02-10 14:00', 60, 'intake', 'afgerond'),
(20, 6, 5, '2025-03-03 15:00', 45, 'controle', 'gepland');

PRINT('inserted into afspraken
');

INSERT INTO declaraties (id, patient_id, type_declaratie, gedeclareerd_door_gebruiker_id, datum_handeling, omschrijving, gedeclareerd_bedrag, status) VALUES
(1, 1, 'Huisarts', 3, '2025-01-08', 'Consult wondverzorging arm', 3500, 'Nieuw'),
(2, 1, 'Huisarts', 3, '2025-01-15', 'Controle wondinfectie', 2500, 'Nieuw'),
(3, 2, 'Huisarts', 4, '2025-02-01', 'Telefonisch consult pijnmedicatie', 1500, 'Nieuw'),
(4, 3, 'Huisarts', 3, '2025-01-16', 'Consult schouderklachten', 3500, 'Nieuw'),
(5, 4, 'Huisarts', 4, '2025-02-06', 'Consult enkelblessure', 3500, 'Nieuw'),
(6, 5, 'Huisarts', 3, '2025-03-02', 'Consult nekhernia', 3500, 'Nieuw'),
(7, 6, 'Huisarts', 4, '2025-02-11', 'Consult chronische rugpijn', 3500, 'Nieuw'),
(8, 7, 'Huisarts', 3, '2025-03-06', 'Consult meniscus scheur', 3500, 'Nieuw'),
(9, 8, 'Huisarts', 4, '2025-04-02', 'Consult fibromyalgie', 3500, 'Nieuw'),
(10, 9, 'Huisarts', 3, '2025-03-11', 'Consult carpaal tunnelsyndroom', 3500, 'Nieuw'),
(11, 10, 'Huisarts', 4, '2025-04-06', 'Consult knieartrose', 3500, 'Nieuw'),
(12, 11, 'Huisarts', 3, '2025-04-10', 'Consult algemene klachten', 3500, 'Nieuw'),
(13, 12, 'Huisarts', 4, '2025-04-15', 'Consult pijnklachten', 3500, 'Nieuw'),
(14, 13, 'Huisarts', 3, '2025-04-20', 'Consult revalidatie bespreking', 3500, 'Nieuw'),
(15, 14, 'Huisarts', 4, '2025-04-25', 'Consult controle', 3500, 'Nieuw'),
(16, 15, 'Huisarts', 3, '2025-05-01', 'Consult nieuwe klachten', 3500, 'Nieuw'),
(17, 16, 'Huisarts', 4, '2025-05-05', 'Consult algemene controle', 3500, 'Nieuw'),
(18, 17, 'Huisarts', 3, '2025-05-10', 'Consult pijnbeheer', 3500, 'Nieuw'),
(19, 18, 'Huisarts', 4, '2025-05-15', 'Consult algemene check', 3500, 'Nieuw'),
(20, 19, 'Huisarts', 3, '2025-05-20', 'Consult nieuwe blessure', 3500, 'Nieuw'),
(21, 20, 'Huisarts', 4, '2025-05-25', 'Consult klachtenbespreking', 3500, 'Nieuw'),
(22, 21, 'Huisarts', 3, '2025-05-28', 'Consult voortgang', 3500, 'Nieuw'),
(23, 22, 'Huisarts', 4, '2025-05-30', 'Consult advies', 3500, 'Nieuw'),
(24, 23, 'Huisarts', 3, '2025-06-01', 'Consult nazorg', 3500, 'Nieuw'),
(25, 24, 'Huisarts', 4, '2025-06-05', 'Consult algemeen', 3500, 'Nieuw'),
(26, 25, 'Huisarts', 3, '2025-06-10', 'Consult opvolging', 3500, 'Nieuw'),
(27, 1, 'Revalidatiearts', 5, '2025-01-10', 'Intake revalidatie knie', 8000, 'Nieuw'),
(28, 1, 'Revalidatiearts', 5, '2025-01-31', 'Controle consult 1', 6000, 'Nieuw'),
(29, 1, 'Revalidatiearts', 5, '2025-02-21', 'Controle consult 2', 6000, 'Nieuw'),
(30, 1, 'Revalidatiearts', 5, '2025-03-14', 'Controle consult 3', 6000, 'Nieuw'),
(31, 1, 'Revalidatiearts', 5, '2025-04-04', 'Controle consult 4', 6000, 'Nieuw'),
(32, 2, 'Revalidatiearts', 6, '2025-02-01', 'Intake revalidatie rug', 8000, 'Nieuw'),
(33, 2, 'Revalidatiearts', 6, '2025-02-22', 'Controle consult 1', 6000, 'Nieuw'),
(34, 2, 'Revalidatiearts', 6, '2025-03-15', 'Controle consult 2', 6000, 'Nieuw'),
(35, 3, 'Revalidatiearts', 5, '2025-01-15', 'Intake revalidatie schouder', 8000, 'Nieuw'),
(36, 3, 'Revalidatiearts', 5, '2025-02-05', 'Controle consult 1', 6000, 'Nieuw'),
(37, 4, 'Revalidatiearts', 7, '2025-02-05', 'Intake revalidatie enkel', 8000, 'Nieuw'),
(38, 4, 'Revalidatiearts', 7, '2025-02-26', 'Controle consult 1', 6000, 'Nieuw'),
(39, 5, 'Revalidatiearts', 6, '2025-03-01', 'Intake revalidatie nek', 8000, 'Nieuw'),
(40, 5, 'Revalidatiearts', 6, '2025-03-22', 'Controle consult 1', 6000, 'Nieuw'),
(41, 6, 'Revalidatiearts', 5, '2025-02-10', 'Intake chronische rugpijn', 8000, 'Nieuw'),
(42, 6, 'Revalidatiearts', 5, '2025-03-03', 'Controle consult 1', 6000, 'Nieuw'),
(43, 7, 'Revalidatiearts', 7, '2025-03-05', 'Intake meniscus', 8000, 'Nieuw'),
(44, 7, 'Revalidatiearts', 7, '2025-03-26', 'Controle consult 1', 6000, 'Nieuw'),
(45, 8, 'Revalidatiearts', 6, '2025-04-01', 'Intake fibromyalgie', 8000, 'Nieuw'),
(46, 8, 'Revalidatiearts', 6, '2025-04-22', 'Controle consult 1', 6000, 'Nieuw'),
(47, 9, 'Revalidatiearts', 5, '2025-03-10', 'Intake CTS', 8000, 'Nieuw'),
(48, 9, 'Revalidatiearts', 5, '2025-03-31', 'Controle consult 1', 6000, 'Nieuw'),
(49, 10, 'Revalidatiearts', 7, '2025-04-05', 'Intake knie artrose', 8000, 'Nieuw'),
(50, 10, 'Revalidatiearts', 7, '2025-04-26', 'Controle consult 1', 6000, 'Nieuw'),
(51, 11, 'Revalidatiearts', 6, '2025-04-10', 'Intake algemeen', 8000, 'Nieuw'),
(52, 11, 'Revalidatiearts', 6, '2025-05-01', 'Controle consult 1', 6000, 'Nieuw'),
(53, 12, 'Revalidatiearts', 5, '2025-04-15', 'Intake revalidatie', 8000, 'Nieuw'),
(54, 12, 'Revalidatiearts', 5, '2025-05-06', 'Controle consult 1', 6000, 'Nieuw'),
(55, 13, 'Revalidatiearts', 7, '2025-04-20', 'Intake oudere patient', 8000, 'Nieuw'),
(56, 13, 'Revalidatiearts', 7, '2025-05-11', 'Controle consult 1', 6000, 'Nieuw'),
(57, 14, 'Revalidatiearts', 6, '2025-04-25', 'Intake diverse klachten', 8000, 'Nieuw'),
(58, 14, 'Revalidatiearts', 6, '2025-05-16', 'Controle consult 1', 6000, 'Nieuw'),
(59, 15, 'Revalidatiearts', 5, '2025-05-01', 'Intake nieuwe patient', 8000, 'Nieuw'),
(60, 15, 'Revalidatiearts', 5, '2025-05-22', 'Controle consult 1', 6000, 'Nieuw'),
(61, 16, 'Revalidatiearts', 7, '2025-05-05', 'Intake chronische pijn', 8000, 'Nieuw'),
(62, 16, 'Revalidatiearts', 7, '2025-05-26', 'Controle consult 1', 6000, 'Nieuw'),
(63, 17, 'Revalidatiearts', 6, '2025-05-10', 'Intake post-op', 8000, 'Nieuw'),
(64, 17, 'Revalidatiearts', 6, '2025-05-31', 'Controle consult 1', 6000, 'Nieuw'),
(65, 18, 'Revalidatiearts', 5, '2025-05-15', 'Intake complexe case', 8000, 'Nieuw'),
(66, 18, 'Revalidatiearts', 5, '2025-06-05', 'Controle consult 1', 6000, 'Nieuw'),
(67, 19, 'Revalidatiearts', 7, '2025-05-20', 'Intake sportblessure', 8000, 'Nieuw'),
(68, 19, 'Revalidatiearts', 7, '2025-06-10', 'Controle consult 1', 6000, 'Nieuw'),
(69, 20, 'Revalidatiearts', 6, '2025-05-25', 'Intake revalidatieplan', 8000, 'Nieuw'),
(70, 20, 'Revalidatiearts', 6, '2025-06-15', 'Controle consult 1', 6000, 'Nieuw'),
(71, 21, 'Revalidatiearts', 5, '2025-05-28', 'Intake algemeen herstel', 8000, 'Nieuw'),
(72, 21, 'Revalidatiearts', 5, '2025-06-18', 'Controle consult 1', 6000, 'Nieuw'),
(73, 22, 'Revalidatiearts', 7, '2025-05-30', 'Intake chronische aandoening', 8000, 'Nieuw'),
(74, 22, 'Revalidatiearts', 7, '2025-06-20', 'Controle consult 1', 6000, 'Nieuw'),
(75, 23, 'Revalidatiearts', 6, '2025-06-01', 'Intake nieuwe doorverwijzing', 8000, 'Nieuw'),
(76, 23, 'Revalidatiearts', 6, '2025-06-22', 'Controle consult 1', 6000, 'Nieuw'),
(77, 24, 'Revalidatiearts', 5, '2025-06-05', 'Intake breukherstel', 8000, 'Nieuw'),
(78, 24, 'Revalidatiearts', 5, '2025-06-26', 'Controle consult 1', 6000, 'Nieuw'),
(79, 25, 'Revalidatiearts', 7, '2025-06-10', 'Intake algemeen traject', 8000, 'Nieuw'),
(80, 25, 'Revalidatiearts', 7, '2025-07-01', 'Controle consult 1', 6000, 'Nieuw');

PRINT('inserted into declaraties
');

INSERT INTO declaratie_verwerkingen 
(id, declaratie_id, zorgverzekeraar_medewerker_id, datum_verwerking, status_verwerking, vergoed_bedrag, afgekeurd_bedrag, toelichting) VALUES
(1, 1, 1, '2025-05-01 10:00:00', 'Goedgekeurd', 3500, 0, 'Standaard huisartsenconsult'),
(2, 2, 1, '2025-05-01 10:15:00', 'Goedgekeurd', 2500, 0, 'Controle consult conform polis'),
(3, 3, 2, '2025-05-02 11:00:00', 'Goedgekeurd', 1500, 0, 'Telefonisch consult conform voorwaarden'),
(4, 4, 1, '2025-05-03 09:30:00', 'Goedgekeurd', 3500, 0, 'Consult schouder'),
(5, 5, 2, '2025-05-04 14:00:00', 'Goedgekeurd', 3500, 0, 'Consult enkel'),
(6, 6, 1, '2025-05-05 10:00:00', 'Goedgekeurd', 3500, 0, 'Consult nekhernia'),
(7, 7, 2, '2025-05-06 11:00:00', 'Goedgekeurd', 3500, 0, 'Consult rugpijn'),
(8, 8, 1, '2025-05-07 10:30:00', 'Goedgekeurd', 3500, 0, 'Consult meniscus'),
(9, 27, 1, '2025-05-08 13:00:00', 'Gedeeltelijk goedgekeurd', 7000, 1000, 'Intakebedrag max 70 euro'),
(10, 28, 1, '2025-05-08 13:30:00', 'Goedgekeurd', 6000, 0, 'Conform behandelplan'),
(11, 29, 2, '2025-05-09 14:00:00', 'Goedgekeurd', 6000, 0, 'Conform behandelplan'),
(12, 32, 1, '2025-05-10 09:00:00', 'Gedeeltelijk goedgekeurd', 7000, 1000, 'Intakebedrag max 70 euro'),
(13, 33, 1, '2025-05-10 09:30:00', 'Goedgekeurd', 6000, 0, 'Conform behandelplan'),
(14, 35, 2, '2025-05-11 10:00:00', 'Gedeeltelijk goedgekeurd', 7000, 1000, 'Intakebedrag max 70 euro'),
(15, 36, 2, '2025-05-11 10:30:00', 'Goedgekeurd', 6000, 0, 'Conform behandelplan');

PRINT('inserted into declaratie_verwerkingen
');

INSERT INTO intakes
(id, patient_id, revalidatiearts_id, diagnose, ernst_blessure, behandeldoelen, datum_intake) VALUES
(1, 1, 5, 'Gescheurde kruisbanden (ACL)', 'Ernstig', 'Volledig herstel kniefunctie; wondgenezing; pijnmanagement', '2025-01-10'),
(2, 2, 6, 'Hernia L5-S1', 'Matig', 'Pijnreductie; mobiliteit; terugkeer naar werk', '2025-02-01'),
(3, 3, 5, 'Schouder impingement', 'Licht tot matig', 'Pijnvrij bewegen; krachtopbouw', '2025-01-15'),
(4, 4, 7, 'Enkelverstuiking graad 2', 'Matig', 'Stabiliteit; belasting opbouw', '2025-02-05'),
(5, 5, 6, 'Nekhernia C5-C6', 'Ernstig', 'Nekmobiliteit; zenuwherstel; pijncontrole', '2025-03-01'),
(6, 6, 5, 'Chronische rugpijn', 'Matig', 'Activatie; coping mechanismen; functiebehoud', '2025-02-10'),
(7, 7, 7, 'Gescheurde meniscus', 'Ernstig', 'Volledig herstel kniefunctie; pijnvrij sporten', '2025-03-05'),
(8, 8, 6, 'Fibromyalgie', 'Chronisch', 'Pijnmanagement; energieniveau; levenskwaliteit', '2025-04-01'),
(9, 9, 5, 'Carpale tunnelsyndroom', 'Licht', 'Pijnreductie; grijpkracht herstel', '2025-03-10'),
(10, 10, 7, 'Knieartrose', 'Matig', 'Mobiliteit; pijnvermindering; spierversterking', '2025-04-05');

PRINT('inserted into intakes
');

INSERT INTO medicatie 
(patient_id, huisarts_id, naam, dosering, frequentie, start_datum, eind_datum, status) VALUES
(1, 3, 'Amoxicilline', '500mg', '3x daags', '2025-01-08', '2025-01-18', 'Afgerond'),
(1, 3, 'Paracetamol', '500mg', '4x daags max', '2025-01-08', '2025-02-01', 'Afgerond'),
(2, 4, 'Diclofenac', '50mg', '2x daags', '2025-02-01', '2025-03-01', 'Afgerond'),
(3, 3, 'Ibuprofen', '400mg', '3x daags', '2025-01-15', '2025-02-15', 'Afgerond'),
(4, 4, 'Naproxen', '250mg', '2x daags', '2025-02-05', '2025-03-05', 'Afgerond'),
(5, 3, 'Tramadol', '50mg', '1x daags', '2025-03-02', '2025-04-01', 'Actief'),
(6, 4, 'Fentanyl pleister', '12mcg/u', 'Elke 72 uur', '2025-02-10', '2025-05-01', 'Actief'),
(7, 3, 'Pijnstiller X', '10mg', '3x daags', '2025-03-06', '2025-04-06', 'Afgerond'),
(8, 4, 'Lyrica', '75mg', '2x daags', '2025-04-02', '2025-06-02', 'Actief'),
(9, 3, 'Corticostero�den', '10mg', '1x daags', '2025-03-11', '2025-03-25', 'Afgerond'),
(10, 4, 'Diclofenac gel', 'Topisch', '2x daags', '2025-04-06', '2025-05-06', 'Actief'),
(11, 3, 'Spierverslapper Y', '5mg', '1x daags', '2025-04-11', '2025-05-11', 'Actief'),
(12, 4, 'Pijnstiller Z', '20mg', '2x daags', '2025-04-16', '2025-05-16', 'Actief'),
(13, 3, 'Tramadol', '50mg', '1x daags', '2025-04-21', '2025-05-21', 'Actief'),
(14, 4, 'Paracetamol', '500mg', '4x daags', '2025-04-26', '2025-05-26', 'Actief'),
(15, 3, 'Ibuprofen', '400mg', '3x daags', '2025-05-02', '2025-06-02', 'Actief'),
(16, 4, 'Naproxen', '250mg', '2x daags', '2025-05-06', '2025-06-06', 'Actief'),
(17, 3, 'Code�ne', '30mg', '2x daags', '2025-05-11', '2025-06-11', 'Actief'),
(18, 4, 'Paracetamol', '500mg', '4x daags', '2025-05-16', '2025-06-16', 'Actief'),
(19, 3, 'Diclofenac', '50mg', '2x daags', '2025-05-21', '2025-06-21', 'Actief'),
(20, 4, 'Ibuprofen', '400mg', '3x daags', '2025-05-26', '2025-06-26', 'Actief'),
(21, 3, 'Paracetamol', '500mg', '4x daags', '2025-05-29', '2025-06-29', 'Actief'),
(22, 4, 'Naproxen', '250mg', '2x daags', '2025-05-31', '2025-07-01', 'Actief'),
(23, 3, 'Tramadol', '50mg', '1x daags', '2025-06-02', '2025-07-02', 'Actief'),
(24, 4, 'Pijnstiller A', '10mg', '3x daags', '2025-06-06', '2025-07-06', 'Actief');

PRINT('inserted into medicatie
');

INSERT INTO notities (id, patient_id, auteur_gebruiker_id, datum_tijd_notitie, inhoud) VALUES
(1, 1, 3, '2025-01-09 16:00:00', 'Wondinfectie arm lijkt te verbeteren na start antibiotica.'),
(2, 1, 5, '2025-01-10 15:00:00', 'Pati�nt is gemotiveerd'),
(3, 1, 3, '2025-01-20 10:00:00', 'Wondgenezing goed'),
(4, 1, 5, '2025-01-31 10:45:00', 'Voortgang positief'),
(5, 2, 4, '2025-02-05 09:30:00', 'Pati�nt klaagt over zenuwpijn'),
(6, 2, 6, '2025-02-06 11:00:00', 'Zenuwpijn besproken'),
(7, 3, 3, '2025-01-17 14:30:00', 'Schouderklachten lijken te stabiliseren.'),
(8, 3, 5, '2025-01-17 17:00:00', 'Oefenplan opgesteld voor schouder mobiliteit.'),
(9, 4, 4, '2025-02-07 10:30:00', 'Enkel nog steeds gezwollen'),
(10, 4, 7, '2025-02-07 11:30:00', 'Start oefenplan voor enkelstabiliteit.'),
(11, 5, 3, '2025-03-03 10:00:00', 'Nekpijn nog ernstig'),
(12, 5, 6, '2025-03-03 10:45:00', 'Intake afgerond'),
(13, 6, 4, '2025-02-12 14:00:00', 'Chronische rugpijn besproken'),
(14, 6, 5, '2025-02-12 15:00:00', 'Activatieprogramma gestart voor rugpijn.'),
(15, 7, 3, '2025-03-07 10:00:00', 'Meniscus scheur'),
(16, 7, 7, '2025-03-07 11:00:00', 'Focus op herstel kniefunctie voor sportterugkeer.'),
(17, 8, 4, '2025-04-03 10:00:00', 'Fibromyalgie pati�nt'),
(18, 8, 6, '2025-04-03 11:00:00', 'Behandelplan gericht op energiebeheer en pijnreductie.'),
(19, 9, 3, '2025-03-12 10:00:00', 'CT-scan uitslag besproken'),
(20, 9, 5, '2025-03-12 11:00:00', 'Start fysiotherapie voor pols en hand.');

PRINT('inserted into notities
');

INSERT INTO oefeningen (id, naam, beschrijving, instructie_video_url) VALUES
(1, 'Kniebuiging', 'Langzaam knieën buigen tot 90 graden rug recht houden.', 'https://www.youtube.com/watch?v=00g8DINXtIY'),
(2, 'Hamstring Stretch', 'Zittend één been gestrekt', 'https://www.youtube.com/shorts/ywgDXzYD6v8'),
(3, 'Calf Raises', 'Op de tenen staan en langzaam zakken.', 'https://www.youtube.com/watch?v=eMTy3qylqnE'),
(4, 'Core Stability Plank', 'Plankpositie rug recht', 'https://www.youtube.com/watch?v=pvIjsG5Svck'),
(5, 'Schouder Rotaties', 'Cirkelbewegingen met de armen.', 'https://www.youtube.com/watch?v=oLwTC-lAJws&list=PLurruTwy8-10c7tAfYnJRkFVy_U1O3IZ1');

PRINT('inserted into oefeningen
');

INSERT INTO oefenplannen 
(id, patient_id, oefening_id, revalidatie_arts_id, start_datum, eind_datum, herhalingen, sets, frequentie_per_dag, opmerkingen) VALUES
(1, 1, 1, 5, '2025-01-12', '2025-03-01', 10, 3, 2, 'Focus op stabiliteit'),
(2, 1, 2, 5, '2025-01-12', '2025-03-01', 15, 3, 2, 'Hamstring flexibiliteit'),
(3, 1, 3, 5, '2025-01-12', '2025-03-01', 12, 3, 2, 'Kuitspierversterking'),
(4, 1, 4, 5, '2025-01-20', '2025-04-01', 1, 3, 1, 'Houd 30 sec vast'),
(5, 1, 1, 5, '2025-03-02', '2025-05-01', 12, 4, 2, 'Verhoging intensiteit'),
(6, 1, 2, 5, '2025-03-02', '2025-05-01', 20, 3, 2, 'Verdieping stretch'),
(7, 1, 5, 5, '2025-03-10', '2025-05-15', 10, 3, 1, 'Lichte schouderoefeningen voor algehele conditie'),
(8, 1, 1, 5, '2025-05-02', '2025-07-01', 15, 4, 2, 'Laatste fase knieherstel'),
(9, 1, 4, 5, '2025-04-05', '2025-06-01', 1, 4, 1, 'Core versterking gevorderd'),
(10, 1, 3, 5, '2025-04-10', '2025-06-15', 15, 3, 2, 'Meer explosieve kuitbewegingen'),
(11, 2, 1, 6, '2025-02-03', '2025-04-01', 8, 3, 2, 'Voorzichtige beweging'),
(12, 2, 4, 6, '2025-02-03', '2025-04-01', 1, 2, 1, 'Houd 20 sec vast'),
(13, 2, 2, 6, '2025-03-01', '2025-05-01', 10, 3, 1, 'Verbeteren flexibiliteit onderrug'),
(14, 2, 1, 6, '2025-04-02', '2025-06-01', 10, 3, 2, 'Verhoging belasting'),
(15, 2, 3, 6, '2025-04-15', '2025-06-30', 10, 2, 1, 'Enkelstabiliteit'),
(16, 3, 5, 5, '2025-01-17', '2025-03-01', 15, 3, 2, 'Focus op rotatie'),
(17, 3, 1, 5, '2025-02-01', '2025-04-01', 10, 2, 1, 'Algemene conditie'),
(18, 3, 2, 5, '2025-03-05', '2025-05-01', 12, 3, 2, 'Schouderflexibiliteit'),
(19, 3, 4, 5, '2025-04-01', '2025-06-01', 1, 3, 1, 'Core stabiliteit'),
(20, 4, 3, 7, '2025-02-07', '2025-04-01', 12, 3, 2, 'Enkelversterking'),
(21, 4, 1, 7, '2025-02-07', '2025-04-01', 8, 2, 1, 'Lichte knieoefeningen ter ondersteuning'),
(22, 4, 2, 7, '2025-04-02', '2025-06-01', 10, 3, 2, 'Hamstring stretch voor balans'),
(23, 4, 5, 7, '2025-04-10', '2025-06-30', 10, 3, 1, 'Schouder mobiliteit'),
(24, 5, 1, 6, '2025-03-03', '2025-05-01', 8, 3, 2, 'Nek- en rugoefeningen'),
(25, 5, 4, 6, '2025-03-03', '2025-05-01', 1, 2, 1, 'Nekstabiliteit'),
(26, 5, 2, 6, '2025-04-05', '2025-06-01', 10, 3, 1, 'Arm- en schouderstretch'),
(27, 6, 1, 5, '2025-02-12', '2025-04-01', 10, 3, 2, 'Rugversterking'),
(28, 6, 2, 5, '2025-02-12', '2025-04-01', 10, 3, 1, 'Onderrug stretch'),
(29, 6, 4, 5, '2025-03-01', '2025-05-01', 1, 3, 1, 'Core voor rug'),
(30, 7, 1, 7, '2025-03-07', '2025-05-01', 10, 3, 2, 'Kniebuiging lichte belasting'),
(31, 7, 2, 7, '2025-03-07', '2025-05-01', 15, 3, 1, 'Hamstring flexibiliteit knie'),
(32, 7, 3, 7, '2025-03-20', '2025-06-01', 12, 3, 2, 'Kuitspieren en balans'),
(33, 8, 4, 6, '2025-04-03', '2025-06-01', 1, 2, 1, 'Algemene core stability'),
(34, 8, 5, 6, '2025-04-03', '2025-06-01', 10, 3, 1, 'Lichte mobiliteitsoefeningen'),
(35, 9, 1, 5, '2025-03-12', '2025-05-01', 10, 3, 2, 'Algemene krachtopbouw'),
(36, 9, 3, 5, '2025-03-12', '2025-05-01', 12, 3, 1, 'Kuit- en enkelstabiliteit'),
(37, 10, 1, 7, '2025-04-07', '2025-06-01', 8, 3, 2, 'Lichte kniebuigingen'),
(38, 10, 2, 7, '2025-04-07', '2025-06-01', 10, 3, 1, 'Stretch voor mobiliteit'),
(39, 11, 4, 6, '2025-04-10', '2025-06-01', 1, 3, 1, 'Core stability oefeningen'),
(40, 11, 5, 6, '2025-04-10', '2025-06-01', 10, 2, 1, 'Schouder en nek mobiliteit'),
(41, 12, 1, 5, '2025-04-15', '2025-06-01', 10, 3, 2, 'Lichte belasting oefeningen'),
(42, 12, 2, 5, '2025-04-15', '2025-06-01', 15, 3, 1, 'Flexibiliteit'),
(43, 13, 1, 7, '2025-04-20', '2025-06-01', 8, 3, 2, 'Knieversterking'),
(44, 13, 3, 7, '2025-04-20', '2025-06-01', 12, 3, 1, 'Kuitspier activatie'),
(45, 14, 2, 6, '2025-04-25', '2025-06-01', 10, 3, 2, 'Algemene stretch'),
(46, 14, 4, 6, '2025-04-25', '2025-06-01', 1, 3, 1, 'Core stability algemeen'),
(47, 15, 1, 5, '2025-05-01', '2025-07-01', 10, 3, 2, 'Start herstel'),
(48, 15, 2, 5, '2025-05-01', '2025-07-01', 15, 3, 1, 'Flexibiliteit'),
(49, 16, 3, 7, '2025-05-05', '2025-07-01', 12, 3, 2, 'Kuit en enkel'),
(50, 16, 4, 7, '2025-05-05', '2025-07-01', 1, 3, 1, 'Core training'),
(51, 17, 1, 6, '2025-05-10', '2025-07-01', 10, 3, 2, 'Lichte kracht'),
(52, 17, 5, 6, '2025-05-10', '2025-07-01', 10, 3, 1, 'Schouder mobiliteit'),
(53, 18, 2, 5, '2025-05-15', '2025-07-01', 15, 3, 2, 'Algemene stretch'),
(54, 18, 3, 5, '2025-05-15', '2025-07-01', 12, 3, 1, 'Kuit en balans'),
(55, 19, 4, 7, '2025-05-20', '2025-07-01', 1, 3, 1, 'Core stability'),
(56, 19, 1, 7, '2025-05-20', '2025-07-01', 10, 3, 2, 'Lichte knie'),
(57, 20, 5, 6, '2025-05-25', '2025-07-01', 10, 3, 1, 'Schouder en nek'),
(58, 20, 2, 6, '2025-05-25', '2025-07-01', 15, 3, 2, 'Hamstring'),
(59, 21, 3, 5, '2025-05-28', '2025-07-01', 12, 3, 1, 'Kuitversterking'),
(60, 21, 1, 5, '2025-05-28', '2025-07-01', 10, 3, 2, 'Kniebuigingen'),
(61, 22, 4, 7, '2025-05-30', '2025-07-01', 1, 3, 1, 'Core oefeningen'),
(62, 22, 2, 7, '2025-05-30', '2025-07-01', 10, 3, 2, 'Flexibiliteit'),
(63, 23, 5, 6, '2025-06-01', '2025-08-01', 10, 3, 1, 'Schouder'),
(64, 23, 1, 6, '2025-06-01', '2025-08-01', 10, 3, 2, 'Knie'),
(65, 24, 2, 5, '2025-06-05', '2025-08-01', 15, 3, 2, 'Hamstring'),
(66, 24, 3, 5, '2025-06-05', '2025-08-01', 12, 3, 1, 'Kuit'),
(67, 25, 4, 7, '2025-06-10', '2025-08-01', 1, 3, 1, 'Core'),
(68, 25, 5, 7, '2025-06-10', '2025-08-01', 10, 3, 2, 'Schouder');

PRINT('inserted into oefenplannen
');

INSERT INTO vergoedingsregels (id, naam, max_vergoed_bedrag, max_vergoed_percentage, voorwaarden, geldig_vanaf, geldig_tot, actief) VALUES
(1, 'Huisarts consult', 3500, 100, 'Geen eigen risico', '2025-1-1', '', 1),
(2, 'Fysiotherapie sessie', 4500, 100, 'Max 9 sessies per jaar;daarna aanvullend', '2025-1-1', '', 1),
(3, 'Revalidatiearts intake', 7000, 100, 'Eenmalig per traject', '2025-1-1', '', 1),
(4, 'Krukken', 5000, 100, 'Medische noodzaak', '2025-1-1', '', 1),
(5, 'Rugbrace', 12000, 80, 'Met voorschrift specialist', '2025-1-1', '', 1),
(6, 'MedicatieA-Antibiotica', 100, 100, 'Basisverzekering', '2025-1-1', '', 1),
(7, 'Revalidatiearts controle consult', 6000, 100, 'Conform behandelplan', '2025-1-1', '', 1),
(8, 'Enkelbrace', 7500, 90, 'Met voorschrift', '2025-1-1', '', 1),
(9, 'Nekbrace', 9000, 85, 'Met voorschrift specialist', '2025-1-1', '', 1);

PRINT('inserted into vergoedingsregels
');

INSERT INTO uitgevoerde_oefeningen (id, patient_oefenplan_id, datum_tijd_afgevinkt, is_afgevinkt) VALUES
(1, 1, '2025-01-13 09:00', 1),
(2, 1, '2025-01-13 18:00', 1),
(3, 1, '2025-01-14 09:00', 1),
(4, 1, '2025-01-14 18:00', 1),
(5, 1, '2025-01-15 09:00', 1),
(6, 1, '2025-01-15 18:00', 1),
(7, 1, '2025-01-16 09:00', 1),
(8, 1, '2025-01-16 18:00', 1),
(9, 1, '2025-01-17 09:00', 1),
(10, 1, '2025-01-17 18:00', 1),
(11, 2, '2025-01-13 09:15', 1),
(12, 2, '2025-01-13 18:15', 1),
(13, 2, '2025-01-14 09:15', 1),
(14, 2, '2025-01-14 18:15', 1),
(15, 2, '2025-01-15 09:15', 1),
(16, 2, '2025-01-13 09:30', 1),
(17, 3, '2025-01-13 18:30', 1),
(18, 3, '2025-01-14 09:30', 1),
(19, 4, '2025-01-21 10:00', 1),
(20, 4, '2025-01-22 10:00', 1),
(21, 4, '2025-01-23 10:00', 1),
(22, 5, '2025-03-03 09:00', 1),
(23, 5, '2025-03-03 18:00', 1),
(24, 6, '2025-03-03 09:15', 1),
(25, 7, '2025-03-11 09:00', 1),
(26, 8, '2025-05-03 09:00', 1),
(27, 9, '2025-04-06 09:00', 1),
(28, 10, '2025-04-11 09:00', 1),
(29, 11, '2025-02-04 09:00', 1),
(30, 12, '2025-02-04 10:00', 1),
(31, 13, '2025-03-02 09:00', 1),
(32, 14, '2025-04-03 09:00', 1),
(33, 15, '2025-04-16 09:00', 1),
(34, 16, '2025-01-18 09:00', 1),
(35, 17, '2025-02-02 09:00', 1),
(36, 18, '2025-03-06 09:00', 1),
(37, 19, '2025-04-02 09:00', 1),
(38, 20, '2025-02-08 09:00', 1),
(39, 21, '2025-02-08 10:00', 1),
(40, 22, '2025-04-03 09:00', 1),
(41, 23, '2025-04-11 09:00', 1),
(42, 24, '2025-03-04 09:00', 1),
(43, 25, '2025-03-04 10:00', 1),
(44, 26, '2025-04-06 09:00', 1),
(45, 27, '2025-02-13 09:00', 1),
(46, 28, '2025-02-13 10:00', 1),
(47, 29, '2025-03-02 09:00', 1),
(48, 30, '2025-03-08 09:00', 1),
(49, 31, '2025-03-08 10:00', 1),
(50, 32, '2025-03-21 09:00', 1);

PRINT('inserted into uitgevoerde_oefeningen
');

INSERT INTO revalidatieartsen (id, patient_id, revalidatiearts_id) VALUES
(1, 1, 5),
(2, 2, 6),
(3, 3, 5),
(4, 4, 7),
(5, 5, 6),
(6, 6, 5),
(7, 7, 7),
(8, 8, 6),
(9, 9, 5),
(10, 10, 7),
(11, 11, 6),
(12, 12, 5),
(13, 13, 7),
(14, 14, 6),
(15, 15, 5),
(16, 16, 7),
(17, 17, 6),
(18, 18, 5),
(19, 19, 7),
(20, 20, 6),
(21, 21, 5),
(22, 22, 7),
(23, 23, 6),
(24, 24, 5),
(25, 25, 7);

PRINT('inserted into revalidatieartsen
');

INSERT INTO pijnindicaties (id, patient_id, datum_tijd_registratie, pijn_score, toelichting) VALUES
(1, 1, '2025-01-10 10:00', 7, 'Na de operatie'),
(2, 1, '2025-01-11 09:00', 6, 'Iets minder pijn'),
(3, 1, '2025-01-12 09:30', 5, 'Pijn bij bewegen'),
(4, 1, '2025-01-13 09:00', 5, 'Tijdens oefeningen wat meer pijn'),
(5, 1, '2025-01-14 09:00', 4, 'Begin van de dag goed'),
(6, 1, '2025-01-15 09:00', 4, 'Pijn nam licht toe na intensievere oefening'),
(7, 1, '2025-01-16 09:00', 3, 'Gaat steeds beter'),
(8, 1, '2025-01-17 09:00', 3, 'Alleen nog lichte pijn bij langdurig staan'),
(9, 1, '2025-01-18 09:00', 2, 'Bijna pijnvrij in rust'),
(10, 1, '2025-01-19 09:00', 2, 'Soms een steek bij onverwachte beweging'),
(11, 2, '2025-02-01 11:00', 8, 'Erge uitstralende pijn in been'),
(12, 2, '2025-02-02 10:00', 7, 'Medicatie helpt iets'),
(13, 2, '2025-02-03 09:00', 6, 'Na fysiotherapie beter'),
(14, 2, '2025-02-04 09:00', 6, 'Pijn blijft hoog na lange zit'),
(15, 2, '2025-02-05 09:00', 5, 'Iets beter na meer rust'),
(16, 3, '2025-01-15 14:00', 6, 'Pijn bij bovenhandse bewegingen'),
(17, 3, '2025-01-16 10:00', 5, 'Bijna geen pijn'),
(18, 4, '2025-02-05 13:00', 7, 'Flink gezwollen enkel'),
(19, 4, '2025-02-06 09:00', 6, 'Minder zwelling'),
(20, 5, '2025-03-01 16:00', 9, 'Ondraaglijke nekpijn en tintelingen'),
(21, 5, '2025-03-02 09:00', 9, 'Medicatie helpt');

PRINT('inserted into pijnindicaties');