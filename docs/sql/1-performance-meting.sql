-- Performance-meting (zie docs/database-analyse.md)
-- Draai in SSMS als beheerder (sa) op de database RevaliNow. Toont per query de logical reads
-- (SET STATISTICS IO) en het execution plan (SET STATISTICS PROFILE). Wijzigt niets.
-- Tip: zet in SSMS ook 'Include Actual Execution Plan' (Ctrl+M) aan voor het grafische plan.

SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
DECLARE @patient int = 229, @vandaag datetime2 = CAST(CAST(SYSDATETIME() AS date) AS datetime2), @morgen datetime2 = DATEADD(day, 1, CAST(CAST(SYSDATETIME() AS date) AS datetime2));
PRINT '=== Q1 afvinkingen vandaag per oefenplan (zoals OefenplanService) ===';
SET STATISTICS IO ON; SET STATISTICS PROFILE ON;
SELECT u.OefenplanId, SUM(CASE WHEN u.IsAfgevinkt = 1 THEN 1 ELSE -1 END) AS Netto
FROM UitgevoerdeOefeningen u
WHERE u.OefenplanId IN (SELECT Id FROM Oefenplannen WHERE PatientId = @patient)
  AND u.DatumTijdAfgevinkt >= @vandaag AND u.DatumTijdAfgevinkt < @morgen
GROUP BY u.OefenplanId;
SET STATISTICS PROFILE OFF; SET STATISTICS IO OFF;
PRINT '=== Q2 pijnindicaties van patient, nieuwste eerst (dashboard) ===';
SET STATISTICS IO ON; SET STATISTICS PROFILE ON;
SELECT Id, DatumTijdRegistratie, PijnScore, Toelichting FROM PijnIndicaties WHERE PatientId = @patient ORDER BY DatumTijdRegistratie DESC;
SET STATISTICS PROFILE OFF; SET STATISTICS IO OFF;
PRINT '=== Q3 eerstvolgende afspraak ===';
SET STATISTICS IO ON; SET STATISTICS PROFILE ON;
SELECT TOP 1 a.Id, a.DatumTijdAfspraak, g.Voornaam, g.Achternaam FROM Afspraken a JOIN Gebruikers g ON g.Id = a.RevalidatieartsId
WHERE a.PatientId = @patient AND a.DatumTijdAfspraak >= SYSDATETIME() ORDER BY a.DatumTijdAfspraak;
SET STATISTICS PROFILE OFF; SET STATISTICS IO OFF;
