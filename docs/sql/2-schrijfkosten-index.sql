-- Schrijfkosten van een index (zie docs/database-analyse.md)
-- Meet 3x de tijd van 2000 INSERTs in PijnIndicaties, met en zonder IX_PijnIndicaties_PatientId_DatumTijdRegistratie.
-- Alles gebeurt in transacties die worden teruggedraaid: de index en de data blijven ongewijzigd.

SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
-- Triggers (audit/validatie) overslaan, zodat we alleen het effect van de index meten
EXEC sp_set_session_context N'Seeding', 1;
DECLARE @i int, @t0 datetime2, @metIndex int, @zonderIndex int, @ronde int = 1;
DECLARE @resultaten TABLE (Ronde int, MetIndexMs int, ZonderIndexMs int);

WHILE @ronde <= 3
BEGIN
    BEGIN TRANSACTION;
    SET @i = 0; SET @t0 = SYSDATETIME();
    WHILE @i < 2000 BEGIN
        INSERT INTO PijnIndicaties (PatientId, DatumTijdRegistratie, PijnScore, Toelichting)
        VALUES (1 + @i % 250, DATEADD(SECOND, -@i, SYSDATETIME()), @i % 11, 'schrijftest');
        SET @i += 1;
    END
    SET @metIndex = DATEDIFF(MILLISECOND, @t0, SYSDATETIME());
    ROLLBACK;

    BEGIN TRANSACTION;
    DROP INDEX IX_PijnIndicaties_PatientId_DatumTijdRegistratie ON PijnIndicaties;
    SET @i = 0; SET @t0 = SYSDATETIME();
    WHILE @i < 2000 BEGIN
        INSERT INTO PijnIndicaties (PatientId, DatumTijdRegistratie, PijnScore, Toelichting)
        VALUES (1 + @i % 250, DATEADD(SECOND, -@i, SYSDATETIME()), @i % 11, 'schrijftest');
        SET @i += 1;
    END
    SET @zonderIndex = DATEDIFF(MILLISECOND, @t0, SYSDATETIME());
    ROLLBACK; -- index komt terug, testrijen verdwijnen

    INSERT INTO @resultaten VALUES (@ronde, @metIndex, @zonderIndex);
    SET @ronde += 1;
END

SELECT Ronde, MetIndexMs, ZonderIndexMs FROM @resultaten
UNION ALL SELECT NULL, AVG(MetIndexMs), AVG(ZonderIndexMs) FROM @resultaten;
SELECT CONCAT('Index nog aanwezig: ', COUNT(*)) FROM sys.indexes WHERE name = 'IX_PijnIndicaties_PatientId_DatumTijdRegistratie';
SELECT CONCAT('Testrijen over: ', COUNT(*)) FROM PijnIndicaties WHERE Toelichting = 'schrijftest';
