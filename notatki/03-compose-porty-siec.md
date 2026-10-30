# Lekcja 03 — Compose, porty, sieć i Nginx

Ukończona w zakresie podstaw. Adam potwierdził działanie aplikacji po zmianie portu hosta.

## Sieć i Compose

- `ports: "8085:80"` znaczy: port 8085 komputera → port 80 kontenera. Nginx nadal nasłuchuje na 80.
- Domyślny port HTTP to 80, HTTPS to 443; jawny port w URL zastępuje domyślny.
- Nginx serwuje pliki Reacta i przekazuje /api do backendu przez `api:8080`. Kontenery odnajdują się po nazwach usług w sieci Compose.
- localhost w przeglądarce oznacza komputer użytkownika; wewnątrz kontenera — ten kontener.
- EXPOSE jest informacją o porcie; nie publikuje portu na hoście. ASPNETCORE_URLS konfiguruje nasłuchiwanie .NET.
- Konfigurację `nginx.conf` edytujemy w repo; Dockerfile kopiuje ją do obrazu frontendu.
- Eksperyment portu zadziałał po zapisaniu pliku Compose. Compose czyta plik z dysku, nie niezapisany bufor IDE.
- `depends_on: - api` ustala kolejność startu, nie gwarantuje gotowości API. Nginx może zwrócić 502, gdy nie może połączyć się z API.
- `restart: unless-stopped` dotyczy zatrzymania procesu, nie odpowiedzi 500. Healthcheck sprawdza zdrowie; sam negatywny wynik nie uruchamia tej polityki restartu.


## Powtórka

- Zmiana mapowania portu: `docker compose up -d frontend`, bez konieczności builda.
- Wyjaśnij drogę przeglądarka → port hosta → Nginx → API.
- Dlaczego localhost w Nginx nie wskazuje kontenera API?
- Co różni uruchomienie kontenera od gotowości aplikacji?
- Komendy: [ściąga](SCIAGA.md).
