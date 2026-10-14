# LowOnLegs — praktyczna nauka mikroserwisów i Azure

> Plan nauki przez budowanie, bez sztywnego harmonogramu. Punktem wyjścia jest istniejący projekt LowOnLegs. Najpierw lokalne mikroserwisy, później Azure.

## Cel

Zbudować i umieć wytłumaczyć system z niezależnymi usługami .NET, komunikacją asynchroniczną, osobnymi danymi, konteneryzacją, obserwowalnością i wdrożeniem do Azure. Cel zawodowy: solidny przykład projektowy do repozytorium i rozmów .NET/backend, **bez przedstawiania go jako komercyjnego doświadczenia w Azure**.

## Kontekst

- Doświadczenie: .NET / REST API / RabbitMQ / Docker / Azure DevOps / Application Insights / Microsoft Orleans.
- Preferowana nauka: samodzielna implementacja i eksperymenty, dokumentacja zamiast długich filmów.
- Nie chcemy teraz poświęcać czasu na naukę Angulara czy Reacta.
- Nie tworzymy z góry przesadnie rozbudowanej architektury; wykorzystujemy istniejący kod tam, gdzie ma to sens.
- Zero sztywnego kalendarza. Następny etap po spełnieniu kryteriów obecnego.

## Ścieżka po pierwszym etapie — orientacyjnie

1. **Podział domeny i API:** Matches + Ranking, separacja danych, Docker Compose.
2. **Messaging:** RabbitMQ, trwałe kolejki, acknowledgements, ponowne dostarczenie, idempotencja, retry/DLQ i transactional outbox.
3. **Odporność/observability:** OpenTelemetry, Aspire Dashboard, health checks, timeouty, YARP jeśli potrzebny.
4. **Azure:** Resource Groups, Azure Container Apps, Managed Identity, konfiguracja i logowanie, pierwszy deployment.
5. **Azure + CI/CD:** Azure DevOps pipeline, Azure Service Bus (porównanie z RabbitMQ), opcjonalnie Key Vault.
6. **Portfolio:** testy integracyjne, Testcontainers, README, diagram, opis decyzji i ograniczeń.

Nie trzeba od początku implementować Kubernetes/AKS, event sourcingu, rozbudowanego CQRS czy pełnego Clean Architecture.

---

# Etap 1 — Matches.Api i Ranking.Api

## Oczekiwany efekt

Dwa niezależnie uruchamiane API w .NET 10. `Matches.Api` zapisuje mecz i po jego zakończeniu emituje `MatchFinishedV1`; `Ranking.Api` odbiera to zdarzenie i aktualizuje Elo we **własnej** bazie. Uruchomienie lokalne przez Docker Compose.

### Przepływ

```text
HTTP Client
     |
     v
Matches.Api ------> Matches DB
     |
     | MatchFinishedV1 (RabbitMQ)
     v
  RabbitMQ --------> Ranking.Api ------> Ranking DB
                                          |
                                          v
                                   GET /rankings
```

## Reguły architektoniczne

- Dwa API mogą żyć w **jednym repozytorium**.
- Mogą współdzielić mały kod **techniczny**: np. konfigurację telemetryczną czy health checks. Unikać wspólnej biblioteki, która wprowadza zależności biznesowe między usługami.
- Mogą współdzielić **wersjonowane kontrakty komunikacyjne** (na potrzeby nauki np. `Contracts`), ale trzeba myśleć o zgodności między wersjami. Później można rozdzielić kontrakty lub traktować je jako niezależny pakiet.
- **Nie współdzielą** `DbContext`, repozytoriów ani bazowych tabel, logiki domenowej typu `RankingService`, ani bibliotek implementacyjnych drugiej usługi.
- `Matches.Api` nie referencjonuje `Ranking.Application` / `Ranking.Infrastructure` i nie wywołuje ich klas bezpośrednio.
- Każda usługa jest właścicielem swoich danych. Na start mogą działać na jednej instancji PostgreSQL, lecz w dwóch bazach z odrębnymi użytkownikami.
- Każdą usługę można niezależnie uruchomić, testować, zmienić i docelowo wdrożyć.
- **Nie wyodrębniaj usług mechanicznie**: granice odpowiedzialności najpierw sprawdź w istniejącym kodzie i modelu domenowym.

## Proponowana minimalna struktura na start

```text
LowOnLegs.Microservices/
├── src/
│   ├── Matches.Api/
│   ├── Ranking.Api/
│   └── Contracts/                # opcjonalne, małe i wersjonowane
├── tests/                       # dodawaj wraz z testowanym zachowaniem
├── docker-compose.yml
└── LowOnLegs.Microservices.slnx
```

**To propozycja, nie nakaz.** Jeżeli aktualny projekt ma już rozsądne moduły / projekty, dopasuj podział do istniejącej struktury. Nie zakładaj automatycznie po cztery projekty na każdą usługę.

## Zadanie 1 — przygotowanie rozwiązania

- [ ] Przejrzyj obecną strukturę LowOnLegs i zidentyfikuj kod meczów, zawodników i rankingu.
- [ ] Zaproponuj granice `Matches` oraz `Ranking` i opisz, które dane należą do której usługi.
- [ ] Utwórz dwa projekty ASP.NET Core Web API (.NET 10) albo dostosuj istniejące projekty.
- [ ] Zdecyduj o bazie (PostgreSQL lub SQL Server) i konfiguracji lokalnej.
- [ ] Dodaj prosty health check dla każdej usługi.

## Zadanie 2 — Matches.Api

Endpointy:

| Metoda | Trasa | Odpowiedzialność |
|---|---|---|
| `POST` | `/matches` | Utworzenie meczu |
| `POST` | `/matches/{id}/finish` | Zakończenie meczu |
| `GET` | `/matches/{id}` | Pobranie meczu |

Przykładowe żądanie zakończenia meczu (dopasuj identyfikatory i wynik do realnego modelu):

```json
{
  "winnerId": "player-1",
  "loserId": "player-2",
  "winnerScore": 3,
  "loserScore": 1
}
```

- [ ] Dane meczu są zapisywane w bazie należącej do Matches.
- [ ] Walidacja: zawodnicy, poprawny wynik, poprawne przejście statusu.
- [ ] Ten sam mecz nie może zostać zakończony dwukrotnie.
- [ ] Uwaga na równoległe żądania: sam `if` w kodzie nie gwarantuje braku wyścigu; zabezpiecz operację transakcją / optimistic concurrency lub równoważnym mechanizmem, jeśli testujesz współbieżność.

## Zadanie 3 — Ranking.Api

Endpointy:

| Metoda | Trasa | Odpowiedzialność |
|---|---|---|
| `GET` | `/rankings` | Lista rankingowa |
| `GET` | `/rankings/{playerId}` | Ranking zawodnika |

- [ ] Własna baza i migracje EF Core.
- [ ] Początkowy Elo: przykładowo 1000 dla nowego zawodnika.
- [ ] Logika aktualizacji Elo na podstawie zdarzenia o zakończeniu meczu.
- [ ] Ranking nie odczytuje tabel ani encji z Matches.
- [ ] Ustal, skąd Ranking bierze podstawowe informacje o zawodnikach (np. identyfikatory w zdarzeniu, inicjalizacja/provisioning); nie twórz ukrytego dostępu do cudzej bazy.

**Pierwszy kamień milowy:** ukończ zadania 1–3, zanim dodasz RabbitMQ.

## Zadanie 4 — komunikacja RabbitMQ

Przykładowy kontrakt:

```csharp
public record MatchFinishedV1(
    Guid EventId,
    Guid MatchId,
    Guid WinnerId,
    Guid LoserId,
    DateTime FinishedAtUtc
);
```

Możesz rozszerzyć o wynik meczu, wersję i inne niezbędne dane. Nazwy i typy dopasuj do istniejącego projektu.

- [ ] Po zakończeniu meczu opublikuj `MatchFinishedV1`.
- [ ] Ranking odbiera zdarzenie i aktualizuje obu zawodników.
- [ ] Konfiguracja kolejki trwałej, durable messages i manual acknowledgements.
- [ ] Przetestuj zatrzymanie konsumenta i jego ponowne uruchomienie.
- [ ] Zapewnij idempotencję: dwukrotnie odebrane zdarzenie z tym samym `EventId` nie może ponownie zmienić Elo.
- [ ] Zauważ lukę między zatwierdzeniem transakcji w Matches DB a opublikowaniem zdarzenia — to motywacja do **Transactional Outbox** w następnym etapie.

## Zadanie 5 — Docker Compose

- [ ] `matches-api`
- [ ] `ranking-api`
- [ ] `rabbitmq`
- [ ] `matches-db`
- [ ] `ranking-db`
- [ ] Poprawne connection stringi przez nazwy serwisów Docker, nie `localhost` wewnątrz kontenerów.
- [ ] Migracje / inicjalizacja w kontrolowany sposób.
- [ ] Uruchomienie przez `docker compose up --build`.

## Kryteria ukończenia etapu

- [ ] Oba API uruchamiają się niezależnie.
- [ ] Każde API czyta i zapisuje tylko własne dane.
- [ ] Mogę utworzyć i zakończyć mecz przez HTTP.
- [ ] Event po zakończeniu meczu trafia do RabbitMQ.
- [ ] Ranking odbiera event i aktualizuje Elo.
- [ ] `GET /rankings` odzwierciedla zmianę.
- [ ] Ponowne zakończenie meczu jest odrzucane lub bezpiecznie obsługiwane bez duplikowania eventu.
- [ ] Powtórne dostarczenie tego samego eventu nie zmienia ponownie Elo.
- [ ] Wyłączenie Ranking.Api nie blokuje zakończenia meczu, o ile broker jest dostępny.
- [ ] Po ponownym uruchomieniu konsument przetwarza oczekujące komunikaty.
- [ ] `docker compose up --build` uruchamia cały system.
- [ ] README opisuje decyzje architektoniczne, sposób uruchomienia i ograniczenia.

## Darmowe środowisko / koszty

Lokalnie: Docker Compose, RabbitMQ, PostgreSQL, OpenTelemetry, Aspire Dashboard — bez konieczności opłacania infrastruktury Azure. Późniejszy eksperyment w Azure można wykonać na kwalifikującym się koncie darmowym lub w limitach wybranych bezpłatnych usług. **Nie zakładaj, że cała architektura na Azure będzie bezkosztowa**: usługi towarzyszące (baza, rejestr kontenerów, monitoring, sieć) mogą generować opłaty. Weryfikuj bieżący cennik przed wdrożeniem, ustaw budżet/alerty i kasuj zasoby po eksperymentach.

## Dokumentacja

- Microsoft microservices architecture: https://learn.microsoft.com/en-us/azure/architecture/microservices/
- Docker Compose: https://docs.docker.com/compose/
- RabbitMQ tutorials: https://www.rabbitmq.com/tutorials
- Azure Container Apps quickstart: https://learn.microsoft.com/en-us/azure/container-apps/get-started
- Aspire Dashboard: https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/dashboard/overview

---

# Prompt do Codexa — analiza istniejącego repozytorium i ponowna propozycja architektury

Skopiuj poniższy prompt do Codexa pracującego w katalogu rzeczywistego projektu. **Codex ma najpierw przeczytać repozytorium, nie zakładać, że podział proponowany wyżej jest optymalny.**

```text
Jesteś seniorem .NET / architektem systemów rozproszonych i mentorem. Masz dostęp do bieżącego repozytorium mojego projektu LowOnLegs. Chcę uczyć się mikroserwisów, RabbitMQ, Dockera i później Microsoft Azure przez praktykę, rozbudowując istniejący system. Mam ponad 7 lat doświadczenia komercyjnego z .NET, REST API, RabbitMQ, bazami danych, Dockerem i Azure DevOps; nie mam jeszcze praktycznego doświadczenia we wdrażaniu aplikacji na Azure. Nie chcę teraz uczyć się frontendu ani budować ogromnej architektury na wyrost.

Najpierw PRZEANALIZUJ REPOZYTORIUM. Nie zmieniaj żadnych plików na tym etapie.

1. Przeczytaj README, pliki .sln/.slnx, .csproj, konfigurację, Dockerfile/compose, backendowe endpointy, moduły meczów, graczy i Elo/rankingu, integracje i sposób przechowywania danych. Jeśli frontend ma znaczenie dla kontraktów API, tylko wskaż istotne zależności — bez refaktoryzacji frontendu.
2. Opisz faktycznie istniejącą architekturę z odwołaniami do KONKRETNYCH plików/klas. Oddziel pewne ustalenia od założeń i brakujących informacji. Wypisz istniejące granice domenowe oraz miejsca silnego sprzężenia.
3. Oceń, czy sensowny jest podział na Matches.Api i Ranking.Api. Rozważ też alternatywy: pozostawienie modularnego monolitu z asynchroniczną integracją lub inny podział bounded contexts. Uzasadnij wybór z punktu widzenia rzeczywistego kodu, a nie abstrakcyjnego przykładu. Wprost wskaż, jeśli wydzielenie mikroserwisów jest sztuczne — celem jest nauka, ale architektura powinna pozostać uczciwa.
4. Zaproponuj MINIMALNĄ architekturę pierwszego etapu: granice usług, własność danych, API, przepływ zakończenia meczu i aktualizacji Elo, komunikację synchroniczną/asynchroniczną, sposób uruchomienia lokalnie oraz ewentualny kontrakt MatchFinishedV1. Wyjaśnij, które istniejące komponenty warto ponownie wykorzystać i które przenieść.
5. Rozstrzygnij zasady współdzielenia kodu: co może być we wspólnej bibliotece technicznej, co może być wersjonowanym kontraktem zdarzenia, a co musi pozostać prywatną logiką biznesową usług. Nie proponuj wspólnego DbContext ani wywoływania klas Application/Infrastructure z innej usługi.
6. Uwzględnij ryzyka: transactional outbox (luka DB -> broker), idempotencja konsumenta, duplikaty zdarzeń, kolejność, spójność ostateczna, równoległe zakończenie tego samego meczu, awaria konsumenta, brak dostępności brokera i migracje danych. Nie implementuj wszystkiego od razu; ustal rozsądne etapy.
7. Proponuj stack bliski istniejącemu projektowi. Preferowane: .NET 10, ASP.NET Core, EF Core (jeśli obecny), Docker Compose, RabbitMQ i oddzielna własność danych. Nie dokładaj Kubernetes, event sourcingu, CQRS, YARP ani osobnych projektów Domain/Application/Infrastructure tylko po to, żeby wyglądało profesjonalnie.
8. Przygotuj konkretny plan wdrożenia w małych krokach z listą zmian w plikach, testami akceptacyjnymi i pierwszym zadaniem do samodzielnego wykonania. Pierwszy kamień milowy powinien obejmować dwa API i separację danych bez messagingu; RabbitMQ w kolejnym kroku. Zaproponuj też ewentualne bezpieczniejsze przejście, jeśli obecny projekt nie pozwala na taki podział bez dużej przebudowy.
9. Uwzględnij docelową drogę do Azure Container Apps i CI/CD w Azure DevOps, ale NIE projektuj jeszcze szczegółowego deploymentu i NIE wykonuj żadnych operacji w Azure.

Odpowiedz po polsku. Oczekuję:
A) mapy aktualnej architektury popartej ścieżkami do plików,
B) 2–3 realnych wariantów podziału z kompromisami,
C) rekomendowanej architektury (diagram Mermaid),
D) listy projektów, referencji i odpowiedzialności,
E) planu etapowego z testami akceptacyjnymi,
F) potencjalnych pułapek i pytań do mnie,
G) jednego małego zadania na start.

Na razie TYLKO analiza i propozycja; NIE edytuj kodu, nie twórz plików i nie wykonuj destrukcyjnych poleceń. Jeżeli czegoś nie wiesz, zapytaj zamiast zgadywać.
```
