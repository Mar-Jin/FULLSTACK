-- Validatie-triggers en temporal table demonstreren
-- Elke test probeert een ongeldige wijziging en toont de foutmelding van de trigger.
-- De temporal-test maakt een eigen logboekregel aan, bewerkt en verwijdert die, en toont daarna de historie.
-- Tijden in de historie staan in UTC (zo werkt SQL Server altijd bij temporal tables).

SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
DECLARE @plan int = (SELECT TOP 1 Id FROM Oefenplannen WHERE PatientId = 229 AND FrequentiePerDag = 1 AND SYSDATETIME() BETWEEN StartDatum AND EindDatum);
DECLARE @oudPlan int = (SELECT TOP 1 Id FROM Oefenplannen WHERE EindDatum < '2026-01-01');

PRINT '--- Trigger 1: afvinken buiten de looptijd';
BEGIN TRY INSERT INTO UitgevoerdeOefeningen (OefenplanId, DatumTijdAfgevinkt, IsAfgevinkt) VALUES (@oudPlan, SYSDATETIME(), 1); PRINT 'FOUT: toegestaan'; END TRY
BEGIN CATCH PRINT CONCAT('Geweigerd (', ERROR_NUMBER(), '): ', ERROR_MESSAGE()); END CATCH

PRINT '--- Trigger 2: vooruit afvinken';
BEGIN TRY INSERT INTO UitgevoerdeOefeningen (OefenplanId, DatumTijdAfgevinkt, IsAfgevinkt) VALUES (@plan, DATEADD(DAY, 1, SYSDATETIME()), 1); PRINT 'FOUT: toegestaan'; END TRY
BEGIN CATCH PRINT CONCAT('Geweigerd (', ERROR_NUMBER(), '): ', ERROR_MESSAGE()); END CATCH

PRINT '--- Trigger 3: vaker afvinken dan de frequentie (1x per dag)';
BEGIN TRANSACTION;
INSERT INTO UitgevoerdeOefeningen (OefenplanId, DatumTijdAfgevinkt, IsAfgevinkt) VALUES (@plan, SYSDATETIME(), 1);
PRINT 'Eerste keer: toegestaan';
BEGIN TRY INSERT INTO UitgevoerdeOefeningen (OefenplanId, DatumTijdAfgevinkt, IsAfgevinkt) VALUES (@plan, SYSDATETIME(), 1); PRINT 'FOUT: tweede keer toegestaan'; END TRY
BEGIN CATCH PRINT CONCAT('Tweede keer geweigerd (', ERROR_NUMBER(), '): ', ERROR_MESSAGE()); END CATCH
IF @@TRANCOUNT > 0 ROLLBACK;

PRINT '--- Trigger 4: pijn in de toekomst';
BEGIN TRY INSERT INTO PijnIndicaties (PatientId, DatumTijdRegistratie, PijnScore) VALUES (229, DATEADD(DAY, 2, SYSDATETIME()), 4); PRINT 'FOUT: toegestaan'; END TRY
BEGIN CATCH PRINT CONCAT('Geweigerd (', ERROR_NUMBER(), '): ', ERROR_MESSAGE()); END CATCH

PRINT '--- Trigger 5: afspraak bij een huisarts in plaats van een revalidatiearts';
BEGIN TRY UPDATE Afspraken SET RevalidatieartsId = 1 WHERE Id = 1; PRINT 'FOUT: toegestaan'; END TRY
BEGIN CATCH PRINT CONCAT('Geweigerd (', ERROR_NUMBER(), '): ', ERROR_MESSAGE()); END CATCH

PRINT '--- Ter vergelijking: CHECK-constraint op pijnscore 11';
BEGIN TRY INSERT INTO PijnIndicaties (PatientId, DatumTijdRegistratie, PijnScore) VALUES (229, SYSDATETIME(), 11); PRINT 'FOUT: toegestaan'; END TRY
BEGIN CATCH PRINT CONCAT('Geweigerd (', ERROR_NUMBER(), '): ', LEFT(ERROR_MESSAGE(), 110)); END CATCH

PRINT '--- Temporal table: een logboekregel bewerken en daarna verwijderen';
INSERT INTO Logboek (PatientId, DatumTijdActiviteit, Beschrijving) VALUES (229, SYSDATETIME(), 'temporal-test: origineel');
DECLARE @log int = SCOPE_IDENTITY();
WAITFOR DELAY '00:00:01';
UPDATE Logboek SET Beschrijving = 'temporal-test: bewerkt' WHERE Id = @log;
WAITFOR DELAY '00:00:01';
DELETE FROM Logboek WHERE Id = @log;
SELECT Id, Beschrijving, CONVERT(varchar(19), GeldigVanaf, 120) AS GeldigVanaf,
       CASE WHEN GeldigTot > '9999-01-01' THEN 'nu' ELSE CONVERT(varchar(19), GeldigTot, 120) END AS GeldigTot
FROM Logboek FOR SYSTEM_TIME ALL WHERE Id = @log ORDER BY GeldigVanaf;
SELECT CONCAT('Nog in Logboek: ', COUNT(*)) FROM Logboek WHERE Id = @log;
SELECT CONCAT('Audit trail: ', STRING_AGG(Actie, ', ')) FROM AuditLogs WHERE Tabelnaam = 'Logboek' AND RecordId = @log;
