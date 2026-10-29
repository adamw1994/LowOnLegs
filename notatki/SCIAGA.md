# Ściąga — lokalne uruchomienie

Docker Desktop uruchomiony, kontenery Linux. PowerShell w katalogu repo:

```powershell
cd C:\Users\Adam\Repo\LowOnLegs
```

| Cel | Komenda | Znaczenie |
|---|---|---|
| Uruchom całość z aktualnego kodu | `docker compose up -d --build` | Buduje obrazy i uruchamia usługi w tle |
| Wgraj zmianę backendu | `docker compose up -d --build api` | Przebudowuje obraz API i aktualizuje jego kontener |
| Zobacz stan i nazwy kontenerów | `docker compose ps -a` | Niczego nie uruchamia |
| Obserwuj logi backendu | `docker compose logs -f api` | Ctrl+C kończy podgląd, aplikacja działa dalej |
| Ostatnie 100 linii logów | `docker compose logs --tail 100 api` | Jednorazowy odczyt |
| Zatrzymaj i usuń kontenery projektu | `docker compose down` | Pozostawia nazwany wolumen bazy |
P
- Strona: **http://localhost**.
- Ranking singlowy z bazy: **http://localhost/api/stats/elo/singles**.
- DevTools: F12 → Network → żądanie finish → status i Response.
- Docker Desktop → Containers: usługi `api` i `frontend`. Images to obrazy, z których tworzy się kontenery.
- Zapisanie C# w IDE nie aktualizuje kontenera — potrzebna przebudowa.
- Backend przy starcie automatycznie wykonuje migracje.

## Gdzie są dane i logi?

- SQLite: plik `/app/data/LowOnLegsDb.db` w kontenerze API; katalog `/app/data` jest podłączony do wolumenu `db_data` (faktyczna nazwa zwykle zawiera prefiks projektu Compose).
- SQLite nie wymaga osobnego serwera. Program do oglądania tabel jest opcjonalny.
- `docker compose down -v` usuwa również wolumeny projektu — nie używaj, jeśli chcesz zachować dane.
- Zdjęcia w `/app/uploads` nie mają obecnie trwałego wolumenu; odtworzenie kontenera może je usunąć. Bieżący mecz w RAM znika przy restarcie procesu.
- ASP.NET Core wypisuje logi do konsoli procesu; Docker przechwytuje stdout/stderr i udostępnia je przez `compose logs`.
