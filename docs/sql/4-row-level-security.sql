-- Row-level security en databaserollen demonstreren
-- EXECUTE AS USER = 'revalinow_api' doet alsof je de API bent; sp_set_session_context zet wie er 'ingelogd' is,
-- precies zoals SessieContextInterceptor dat in de API doet. REVERT gaat terug naar je eigen login.

SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
PRINT '--- 1. Als API-login ZONDER sessiecontext (bijv. iemand die met het API-wachtwoord in SSMS inlogt)';
EXECUTE AS USER = 'revalinow_api';
SELECT 'Patienten zichtbaar' AS Test, COUNT(*) AS Aantal FROM Patienten
UNION ALL SELECT 'PijnIndicaties zichtbaar', COUNT(*) FROM PijnIndicaties
UNION ALL SELECT 'UitgevoerdeOefeningen zichtbaar', COUNT(*) FROM UitgevoerdeOefeningen;
REVERT;

PRINT '--- 2. Als patient 229 (zoals de API dat zet)';
EXECUTE AS USER = 'revalinow_api';
EXEC sp_set_session_context N'Rol', N'Patient';
EXEC sp_set_session_context N'PatientId', 229;
SELECT 'Patienten zichtbaar' AS Test, COUNT(*) AS Aantal FROM Patienten
UNION ALL SELECT 'PijnIndicaties zichtbaar', COUNT(*) FROM PijnIndicaties
UNION ALL SELECT 'Waarvan van een ander', COUNT(*) FROM PijnIndicaties WHERE PatientId <> 229
UNION ALL SELECT 'Oefenplannen zichtbaar', COUNT(*) FROM Oefenplannen
UNION ALL SELECT 'UitgevoerdeOefeningen zichtbaar', COUNT(*) FROM UitgevoerdeOefeningen;
PRINT '--- 3. Patient 229 probeert pijn te registreren voor patient 1 (block predicate)';
BEGIN TRY
    INSERT INTO PijnIndicaties (PatientId, DatumTijdRegistratie, PijnScore, Toelichting) VALUES (1, SYSDATETIME(), 5, 'rls-test');
    PRINT 'FOUT: insert voor een ander is gelukt';
END TRY
BEGIN CATCH PRINT 'Geweigerd: ' + ERROR_MESSAGE(); END CATCH
PRINT '--- 4. Patient 229 probeert het logboek van patient 1 te verwijderen (filter predicate)';
DELETE FROM Logboek WHERE PatientId = 1;
PRINT 'Rijen verwijderd: ' + CAST(@@ROWCOUNT AS varchar);
PRINT '--- 5. De API-login probeert de audit trail te lezen (geen rechten)';
BEGIN TRY SELECT COUNT(*) FROM AuditLogs; END TRY BEGIN CATCH PRINT 'Geweigerd: ' + ERROR_MESSAGE(); END CATCH
PRINT '--- 6. De API-login probeert een account te wijzigen (geen rechten)';
BEGIN TRY UPDATE Gebruikers SET Rol = 'Huisarts' WHERE Id = 257; END TRY BEGIN CATCH PRINT 'Geweigerd: ' + ERROR_MESSAGE(); END CATCH
REVERT;

PRINT '--- 7. Als huisarts 1 (andere module, zelfde beveiliging)';
EXECUTE AS USER = 'revalinow_api';
EXEC sp_set_session_context N'Rol', N'Huisarts';
EXEC sp_set_session_context N'GebruikerId', 1;
SELECT 'Patienten van huisarts 1' AS Test, COUNT(*) AS Aantal FROM Patienten
UNION ALL SELECT 'Volgens de data', (SELECT COUNT(*) FROM Patienten) ;
REVERT;
SELECT 'Echte aantal patienten van huisarts 1 (als beheerder)' AS Test, COUNT(*) AS Aantal FROM Patienten WHERE HuisartsId = 1;
