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

De eerste keer duurt het even, want de database wordt dan aangemaakt en gevuld met de dataset van de opdracht.

Stoppen doe je met `Ctrl+C`.

## Inloggen

Elke patiënt logt in met zijn e-mailadres. Het wachtwoord is `Welkom123!`, en dat is voor alle patiënten hetzelfde.

Een goede patiënt om mee te beginnen is `nout.b@example.com`, die heeft lopende oefeningen en geplande afspraken.

## Lokaal zonder Docker

Kopieer `Server/appsettings.Development.example.json` naar `Server/appsettings.Development.json` en vul je eigen JWT-sleutel in. Daarna draai je `dotnet run` in `Server/` en `npm run dev` in `Client/`.
