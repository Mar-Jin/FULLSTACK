# RevaliNow

Dit is mijn fullstack-project voor school: een app waarin revalidatiepatiënten hun pijn, oefeningen, logboek en afspraken kunnen bijhouden.
Ik heb het gebouwd met Vue (frontend), ASP.NET Core (backend) en SQL Server (database).

## Runnen

Je hebt alleen [Docker Desktop](https://www.docker.com/products/docker-desktop/) nodig.

1. Start Docker Desktop.
2. Open een terminal in deze map en voer uit:
   ```bash
   docker compose -f docker-compose.dev.yml up --build
   ```
3. Ga naar <http://localhost>.

De eerste keer duurt het even, want de database wordt dan aangemaakt en gevuld met testdata.

Stoppen doe je met `Ctrl+C`.

## Inloggen

Log in met bijvoorbeeld `ra_smit`. Het wachtwoord is `password123`, en dat is voor alle testaccounts hetzelfde.
