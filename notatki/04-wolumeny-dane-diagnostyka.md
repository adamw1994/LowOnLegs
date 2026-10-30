# Lekcja 04 — Wolumeny, trwałość danych i diagnostyka

Ukończona z pomocą mentora: restart API, odtworzenie kontenera z zachowaniem bazy oraz minimalny Compose. Pisanie konfiguracji bez podpowiedzi pozostaje zadaniem do późniejszej powtórki.

## Dane i ograniczenia testu

- SQLite to plik, nie osobny serwer. `db_data:/app/data` podłącza wolumen do katalogu kontenera; nie przechowuje pamięci procesu.
- Adam potwierdził: historia i Elo pozostają po `docker compose restart api`. Ten test nie dowodzi jeszcze trwałości po usunięciu/odtworzeniu kontenera.
- Kolejny eksperyment: projekt `lowonlegs-lab`, plik `docker-compose.lab.yml`, port 8086 i osobny wolumen `lab_db`. Początkowo pusta historia; Adam dodał mecz i potwierdził zachowanie historii, wyniku i Elo po wymuszonym odtworzeniu kontenera API. To osobny dowód trwałości pliku bazy niezależnie od kontenera, nie test backupu ani odporności na usunięcie wolumenu.

```powershell
# Start izolowanego laboratorium z istniejących obrazów
docker compose -p lowonlegs-lab -f docker-compose.lab.yml up -d
# Zastąpienie kontenera API nowym, z zachowaniem wolumenu
docker compose -p lowonlegs-lab -f docker-compose.lab.yml up -d --no-deps --force-recreate api
# Sprzątnięcie kontenerów i sieci laboratorium, z zachowaniem bazy
docker compose -p lowonlegs-lab -f docker-compose.lab.yml down
```

`-p` wybiera projekt, `-f` plik konfiguracji. W komendach laboratorium zawsze podajemy oba. Nie dodajemy `-v` do down, jeśli chcemy zachować wolumen. Powyższe sprzątanie jest instrukcją, nie potwierdzeniem wykonania.
- Odświeżenie scoreboardu wywołuje startMatch i resetuje stan backendu. Dlatego nie można używać odświeżenia jako niezależnego dowodu utraty RAM po restarcie.
- Poza Dockerem backend zobaczy bazę wskazaną w swojej konfiguracji; dotychczasowe dane wymagają bezpiecznego przeniesienia pliku bazy.

## Powtórka

- Ćwiczenie końcowe: `docker-compose.powtorka.yml`, projekt `lowonlegs-powtorka`, istniejący obraz API, port `8090:8080`, konfiguracja SQLite i osobny wolumen `powtorka_db`. Adam potwierdził dane graczy pod `http://localhost:8090/api/players`, bez frontendu i Nginx.
- Nazwa pola YAML musi być dokładna: `environment`, nie `enviroment`. Walidacja: `docker compose -p lowonlegs-powtorka -f docker-compose.powtorka.yml config --quiet` — bez uruchamiania usług.
- Start ćwiczenia: `docker compose -p lowonlegs-powtorka -f docker-compose.powtorka.yml up -d`. Sprzątanie kontenerów i sieci z zachowaniem bazy: ta sama komenda z `down` zamiast `up -d`.

- Zmiana C#: `docker compose up -d --build api`. Sam restart uruchamia poprzedni obraz, bez nowej kompilacji.
- Zmiana mapowania portu: `docker compose up -d frontend`, bez konieczności builda.
- Pełna lista komend: [ściąga](SCIAGA.md).
- Umiej wyjaśnić drogę przeglądarka → port hosta → Nginx → API oraz różnicę między pamięcią, plikiem kontenera i wolumenem.
