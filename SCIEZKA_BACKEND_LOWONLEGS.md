# LowOnLegs — diagnoza, plan nauki i pierwsza lekcja

Stan analizy: 27 października 2026. Dokument roboczy mentora i Adama.

Pełna lista wszystkich zaplanowanych lekcji, ich kolejność, cele i kryteria: [PROGRAM_LEKCJI.md](PROGRAM_LEKCJI.md), rozpisana 29.10.2026. Ten dokument zachowuje diagnozę, uzasadnienia i szczegóły eksperymentów. W sprawie kolejności lekcji pierwszeństwo ma program; termin sprzątania Azure ma pierwszeństwo przed numeracją.

## 1. Ustalenia i sposób współpracy

Cel: rozwijać kompetencje backendowe .NET mid/senior przez samodzielne eksperymenty na LowOnLegs. Około 7 lat doświadczenia jest kontekstem, a nie dowodem znajomości każdego zagadnienia. Stan hobby-projektu nie jest oceną poziomu zawodowego autora.

- Priorytety: praktyczne Azure, systemy rozproszone, granice mikroserwisów, backend i dane. Frontend tylko w zakresie koniecznym do ćwiczeń.
- Podstawa: około 20 godzin tygodniowo. Dodatkowe 0–10 godzin na utrwalenie, eksperymenty i samodzielne diagnozowanie, nie na obowiązkowe nowe technologie.
- Przechodzimy dalej po wykazaniu umiejętności, nie po upływie tygodnia. Nie zakładamy osiągnięcia seniority w miesiąc.
- Mentor daje jedno zadanie naraz. Najpierw przewidywanie wyniku, potem eksperyment, dowód i wyjaśnienie. Najpierw wskazówka, później konkretniejsza podpowiedź; pełna implementacja dopiero na prośbę.
- Przed poleceniem nowej metody, konfiguracji lub mechanizmu mentor zawsze wyjaśnia: co robi, dlaczego jest tutaj potrzebny, jak współpracuje z pozostałym kodem i czego nie zapewnia. Przy konfiguracji ASP.NET Core rozróżnia rejestrację usług od dodania middleware oraz uzasadnia kolejność. Nie podaje samych wywołań do przepisania bez zrozumienia. Zasada doprecyzowana przez Adama 28.10.2026.
- Po pracy Adama: review poprawności, testów i kompromisów. Powrót do tematu po 2 i 7 dniach z innym przypadkiem, bez kopiowania rozwiązania AI.
- Każda sesja kończy się krótką notatką: przewidywanie → obserwacja → wniosek → następne pytanie. ADR: problem, dwie alternatywy, decyzja, konsekwencje, warunek ponownego rozważenia.
- Po każdej zrealizowanej lekcji mentor aktualizuje folder `notatki/`: krótkie podsumowanie, obserwacje, pytania do powtórki i tylko istotne komendy. Wspólna ściąga jest w `notatki/SCIAGA.md`; nie oznaczamy niezrealizowanych tematów jako ukończonych.
- Nie instalowano zależności, nie uruchamiano aplikacji, migracji ani testów; nie tworzono zasobów Azure. Zmiana w tej sesji dotyczy wyłącznie tego dokumentu, zgodnie z bieżącą prośbą o zapis planu.
- Git na początku wskazywał usunięte `CLAUDE.md`, `PLAN_NAUKI_LOWONLEGS.md`, `LowOnLegs_Microservices_Plan_Etap_1.md`. Usunięcia pozostają nietknięte. Historyczny `CLAUDE.md` odczytano z HEAD; opisuje frontend i jego build/lint. Nie znaleziono lokalnego `AGENTS.md` w repozytorium.

### Azure — potwierdzone przez Adama

Pozostały kredyt: **175,99 EUR**. Wygaśnięcie: **21 listopada 2026**. Nie znamy dokładnej godziny, typu subskrypcji, regionu, limitów poszczególnych usług ani zgody na wydatki z własnych środków. Kwota kredytu nie jest automatycznie budżetem do wydania.

Plan: pierwsze wdrożenie w tygodniu 1; ćwiczenia chmurowe zakończyć do 17 listopada; eksport, usuwanie zasobów i kontrola kosztów 18–20 listopada. Pozostaje około 25 dni kalendarzowych, nie pełne cztery tygodnie chmury. Przed provisioningiem ustalić limit własnych wydatków i sprawdzić rzeczywisty koszt konfiguracji w danej subskrypcji. Dziś nie jest to potrzebne do nauki lokalnej.

## 2. Diagnoza repozytorium

Ścieżki w tej części są względem katalogu głównego repo. Nazwa metody wskazuje miejsce dowodu. „Ryzyko” oznacza wniosek ze statycznego kodu; nie oznacza zaobserwowanego incydentu.

| Obszar | Potwierdzone w kodzie | Znaczenie dla nauki |
|---|---|---|
| Architektura | `LowOnLegs/LowOnLegs.sln`: API, Core, Services, Data, Infrastructure; referencje w `.csproj` | Monolit warstwowy, nie mikroserwisy ani jeszcze szczelne moduły domenowe. Services zna Data, a Core zawiera DTO i mapowania. Infrastructure ma głównie deklaracje folderów; `BleService` jest pusty. |
| Wersje | Wszystkie backendowe `.csproj`: `net9.0`; EF Core/SQLite 9.0.2; OpenAPI 9.0.2; stare referencje SignalR 1.2.0 w API/Infrastructure | Przegląd zależności i aktualizacja do wspieranej linii, bez automatycznej wymiany pakietów podczas analizy. |
| Frontend | `LowOnLegs.React/package.json`: React ^19.2.4, Vite ^8.0.4, TS ~6.0.2, SignalR ^10.0.0, TanStack Query, Axios | To deklaracje zakresów wersji, nie pomiar zainstalowanego środowiska. |
| Baza | `LowOnLegs/LowOnLegs/Program.cs`: `UseSqlite`; Data referencjonuje provider SQLite; Compose montuje plik bazy w named volume | **Aktualnie SQLite, nie SQL Server.** SQL Server będzie świadomą zmianą edukacyjną i przygotowaniem do Azure SQL. |
| Funkcje | `MatchService`, `DoubleMatchService`, `PlayerService`, `StatsService`, kontrolery i `EloService` | Są single, debel, wybór zawodników, punkty, korekty, historia, Elo i H2H. SignalR oraz Docker faktycznie istnieją. |
| Statystyki | `StatsService.GetHeadToHead` i rankingi; `LowOnLegs.React/src/pages/Stats.tsx`: `computeEloWithK`, `computePlayerStats` | Porównanie K, średnie, win rate i serie zwycięstw częściowo liczone w przeglądarce. Nie przypisujemy wszystkich tych funkcji backendowi. |
| Stan meczu | `Program.cs`: singletony `MatchStateManager`, `DoubleMatchStateManager`; stan w polach procesu | Jeden bieżący singiel i debel na proces. Restart traci bieżący stan; druga instancja tworzy osobny stan. Brak trwałego identyfikatora operacji zakończenia. |
| Równoległość | `MatchService.AddPoint`: Get → modyfikacja DTO → Set; manager blokuje Get i Set osobno | Blokady nie obejmują całej operacji. Możliwa utrata aktualizacji; trzeba ją odtworzyć kontrolowaną synchronizacją, nie liczyć na przypadek. |
| Async | `MatchService.FinishMatch`, `UpdateSinglesElo`, odpowiedniki debla ignorują zwracane `Task`; repozytoria używają `SaveChangesAsync` | Brakuje oczekiwania na zakończenie operacji. Brak propagacji `CancellationToken`. SQLite może maskować problem przez synchroniczne wykonanie części operacji. |
| Błędy i transakcje | `MatchRepository.Add`, `DoubleMatchRepository.Add`: `catch` zwraca false; caller nie sprawdza rezultatu. `PlayerRepository.UpdateElo` zapisuje osobno | Brak jawnej wspólnej transakcji dla wyniku i obu rankingów. Nie oznacza to braku transakcji pojedynczego SaveChanges. Błąd zapisu może nie zostać ujawniony klientowi. |
| Model | `ApplicationDbContext`: Players, Matches, DoubleMatches, FK z Restrict; EloSingles/EloDoubles w Players | Własność wyniku i rankingu jest obecnie połączona. Modele nie deklarują tokenów współbieżności ani kluczy idempotencji. |
| Migracje i indeksy | `Data/Migrations/20250308114536_Initial.cs`: typy INTEGER/TEXT i Sqlite:Autoincrement; snapshot ma indeksy FK | Migracje są dla SQLite. Nie przenosimy ich bezrefleksyjnie na SQL Server. Brak indeksu historii po filtrze/sortowaniu w snapshot; jego sens ustalimy pomiarem. |
| Zapytania | `MatchRepository.GetAll`: Include zawodników/zwycięzcy, Where, OrderByDescending, ToList bez paginacji. H2H skanuje listę meczów dla par graczy | Materializacja całej historii i praca około O(P² × M) w H2H. Nie ma dowodu N+1 w tym przepływie; należy policzyć SQL, a nie nazwać tak każdą wolną operację. |
| Reguły | `MatchDto.SaveMatchToDatabase` sprawdza tylko obecność graczy; `ToMatchDto` ustawia IsFinished; `FinishMatch` nie wymaga wygranego wyniku | Można próbować zakończyć remis/niedokończony mecz. Singles Elo traktuje prawą stronę jako zwycięzcę przy remisie, mimo braku WinnerId w DTO. Brak jawnego zakazu tego samego gracza po obu stronach. |
| Debel | `DoubleMatchService.UpdateDoublesElo`: dodatnia `CalculateDoubleDelta` dodawana również przegranym | Dla równych rankingów statycznie wynika wzrost obu drużyn. Kandydat do testu reguły, zanim użyjemy danych jako portfolio. |
| Czas | `MatchStateDto.ToMatchDto`: `DateTime.Now`; większość innych zapisów UTC | Ujednolicić semantykę czasu i kolejność meczów przed odtwarzaniem rankingu. |
| Bezpieczeństwo | `Program.cs` ma UseAuthorization, ale brak konfiguracji uwierzytelniania i brak `[Authorize]` w kontrolerach/hubie | Sam middleware nie chroni operacji. CORS też nie jest autoryzacją. Publiczne wdrożenie wymaga ograniczenia dostępu do zapisu. |
| Walidacja/upload | `PlayersController.AddPlayer`: rozszerzenie pobierane z nazwy, zapis w uploads; brak własnej listy typów/rozmiaru i walidacji zawartości | Zbadać limity frameworka osobno; dodać reguły aplikacyjne przed wystawieniem uploadu. Nie twierdzimy, że nie istnieje żaden limit HTTP. |
| SignalR | `MatchesController.AddPoint/SubtractPoint` publikuje `UpdateScore`; finish/reset/start nie publikuje. `ScoreboardHub.SendScoreUpdate` używa pól leftPlayer/rightPlayer zamiast leftScore/rightScore | Niejednolity kontrakt; klient huba może rozgłaszać dane. Hook React oczekuje leftScore/rightScore. Reconnect nie zapewnia odtworzenia stanu. |
| Utrzymanie | `Program.cs` wykonuje `Database.Migrate()` przy starcie; config zawiera Logging, Cors, connection string; brak znalezionych testów, workflow CI, własnych metryk/tracingu, BackgroundService | Warto ćwiczyć kontrolowane migracje, błędy i korelację. Brak własnego ILogger nie oznacza braku logów frameworka. Nie odczytywano magazynu sekretów ani nie wypisywano wartości sekretów. |
| Kontenery | Dockerfile wieloetapowy, użytkownik non-root; Compose API + frontend/Nginx; wolumen tylko `/app/data`; upload do `/app/uploads` | Zdjęcia nie mają trwałego wolumenu. Komentarz w Program o `/app/data/uploads` nie odpowiada obliczanej ścieżce. Compose nie zawiera SQL Server ani brokera. |

### Jeden pełny przepływ: zakończenie singla

1. `SingleScoreboard.tsx` wywołuje `finishMatch`; odpowiedź ustawia lokalny stan komponentu.
2. `MatchesController.FinishMatch` obsługuje `POST /api/matches/finish`, synchronicznie wywołuje serwis i opakowuje wynik w `Ok`.
3. `MatchService.FinishMatch` pobiera DTO singletona. `ToMatchDto` oznacza mecz jako zakończony. Kryterium zapisu wymaga dwóch graczy, nie poprawnego końcowego wyniku.
4. `_matchRepository.Add(...)` zwraca `Task<bool>`; serwis go nie oczekuje. Repozytorium dodaje encję i wykonuje SaveChangesAsync; wyjątek zamienia w false.
5. Serwis uruchamia `UpdateSinglesElo`: odczytuje zawodników, liczy Elo, dwukrotnie wywołuje `UpdateElo`, także bez await. Repozytoria w żądaniu używają tego samego scoped DbContext.
6. Serwis zaczyna kolejny mecz z tymi samymi graczami, kontroler zwraca stan. Ścieżka nie potwierdza poprawnego zakończenia wszystkich zapisów.
7. Nie ma publikacji zdarzenia zakończenia ani broadcastu SignalR w tym endpointcie. Aktualizacja lokalnego UI odpowiedzią HTTP nie jest aktualizacją wszystkich klientów.

**Hipotezy do eksperymentu:** częściowy zapis wyniku/Elo, przedwczesny sukces HTTP, nakładanie operacji DbContext przy rzeczywistym zawieszeniu async, nieobserwowane wyjątki, podwójny zapis przy dwóch finish. Nie przesądzamy, który symptom wystąpi na obecnym providerze. Kolejny finish po resecie może oznaczać nowy mecz 0:0, a nie identyczny rekord — konieczny jest stabilny identyfikator meczu/komendy, zanim zdefiniujemy idempotencję.

**Do uruchomienia przez ucznia w późniejszych zadaniach:** build, ostrzeżenia kompilatora, migracje na pustej izolowanej bazie, smoke test, dwa klienty SignalR, restart kontenera, test błędu zapisu i wyścigu. Dziś nie potwierdzono działania aplikacji ani przechodzenia testów.

## 3. Kierunek architektury i priorytety

| Wariant | Korzyść | Koszt | Ocena |
|---|---|---|---|
| Naprawiony monolit | Najprostsze wdrożenie, wspólna transakcja, łatwy debug | Odpowiedzialności mogą się mieszać | Wybór produkcyjny przy obecnej skali i braku innych wymagań |
| Monolit z wyraźnymi modułami | Rozdziela Mecze/Zawodników od Rankingu bez sieci | Kontrakty i dyscyplina własności danych | Zalecany krok edukacyjny po pierwszym wdrożeniu |
| API + jeden serwis rankingu | Uczy awarii, opóźnień, retry, idempotencji i osobnych danych | Broker, eventual consistency, tracing, trudniejszy deploy i replay | Celowy koszt edukacyjny; brak dowodu potrzeby produkcyjnej |

Rekomendacja mentora: naprawić monolit → wdrożyć minimum → wydzielić odpowiedzialność rankingu w procesie → dopiero osobny proces. Ranking jest dobrym kandydatem, bo powstaje po zakończeniu meczu i może być chwilowo opóźniony. Obecny `StatsService` nie jest gotowym niezależnym serwisem: czyta wspólne repozytoria, a Elo mieszka w Players.

Docelowo dla ćwiczenia: backend posiada zawodników, mecze i outbox; ranking posiada własną bazę projekcji oraz inbox. Brak wspólnych tabel, FK między bazami i bezpośredniego odczytu tabel API przez konsumenta. Kontrakt przenosi ID zawodników, wynik, ID meczu, EventId, wersję kontraktu, czas i ustaloną kolejność; nie encje EF. Przejście kontrolowane: najpierw projekcja porównawcza, potem jeden właściciel zapisu Elo i przełączenie odczytu. Przez okres przejściowy nie utrzymujemy dwóch równorzędnych źródeł prawdy.

Na początek tylko ranking singlowy i jeden konsument. Elo zależy od kolejności wyników także między różnymi meczami tych samych graczy: deduplikacja EventId i wersja pojedynczego meczu nie rozwiązują całej kolejności. Dla małego ćwiczenia ustalić globalną sekwencję zakończeń i deterministyczny replay; partycjonowanie zostawić na później.

**Teraz:** diagnoza async, reguły końca meczu, spójność zapisu, testy, SQL Server, minimum zabezpieczeń i pierwsze Azure. Daje to konkretne historie rekrutacyjne: fałszywy sukces API, granica transakcji, wyścig, pomiar SQL, decyzja hostingowa.

**Później:** idempotencja żądań, optimistic concurrency, outbox/inbox, DLQ, retry i timeouty, projekcja rankingu, odtwarzalny deploy. W rozmowie: rozróżniać atomowość, izolację, dostarczenie i efekt biznesowy; pokazać własny eksperyment i jego ograniczenia.

**Opcjonalnie:** debel w projekcji, dwa API, backplane/Azure SignalR, bardziej rozbudowane Bicep, test obciążeniowy. Kubernetes, service mesh, event sourcing, pełny CQRS i rozbudowane DDD poza podstawą. Nie dokładamy drugiego brokera ani chmury.

### Hosting i narzędzia — propozycja, nie wdrożona decyzja

App Service dobrze pasuje do pojedynczego API i ogranicza zakres nauki hostingu. Container Apps pasuje do istniejących kontenerów i późniejszego workera, ale wymaga zrozumienia rewizji, ingressu, zasobów i skalowania. **Wybór edukacyjny: Container Apps + Azure SQL, jedna instancja API na początek**, ze względu na kontenery i późniejszy konsument. Nie uruchamiać skalowania bieżącego singletona; nawet jedna replika nie zapewnia trwałości przy restarcie ani podczas zmiany rewizji. Deploy na początku wyłącznie pomiędzy meczami, z jawnym ograniczeniem demonstratora. [Porównanie usług Microsoft](https://learn.microsoft.com/en-us/azure/container-apps/compare-options).

Wybrany broker do nauki: Azure Service Bus; lokalnie jego emulator, nie dodatkowo RabbitMQ. Emulator ma ograniczenia i nie zachowuje danych po restarcie, dlatego test „konsument nie działa” zatrzymuje konsumenta, nie broker. Własności trwałości chmury sprawdzamy osobno. [Dokumentacja emulatora](https://learn.microsoft.com/en-us/azure/service-bus-messaging/overview-emulator).

.NET 9 zbliża się do końca wsparcia 10 listopada 2026; w etapie przygotowania wdrożenia zaplanować .NET 10 LTS oraz kompatybilne pakiety/obrazy, z testem regresji. To mała aktualizacja techniczna, nie przebudowa architektury. [Polityka wsparcia](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core), sprawdzona 27.10.2026.

Koszt sprawdzamy przed stworzeniem zasobów: compute API/workera, baza, broker, rejestr obrazów, logi/retencja, transfer i ewentualny storage zdjęć. Nie zakładamy, że kredyt pokrywa dowolną usługę/SKU. Istnieje oferta bezpłatnego Azure SQL, ale dostępność w subskrypcji i zachowanie po limicie należy zweryfikować. [Azure SQL free tier](https://learn.microsoft.com/en-us/azure/azure-sql/database/free-tier?view=azuresql). **Alert budżetowy nie odcina wydatków**, a informacje kosztowe są opóźnione. [Budżety Azure](https://learn.microsoft.com/en-us/azure/cost-management-billing/costs/tutorial-acm-create-budgets).

## 4. Etapy i kryteria przejścia

Każda pozycja „sesje” to zadanie na 60–120 minut, w razie potrzeby powtarzane. Przed eksperymentem zapisujesz przewidywanie. Prace na osobnej bazie ćwiczeniowej, z syntetycznymi graczami. Po każdej zmianie wracamy do smoke testu: zawodnicy → start → punkty → finish → historia → ranking. Nie łączymy niedokończonych zmian do używanej aplikacji.

### A. Punkt startowy i wiarygodne zakończenie meczu — 10–14 h, lokalnie

Cel: zrozumieć istniejący przepływ i naprawić fałszywe założenia o Task oraz wyniku operacji. Zależności: brak. Minimum: singiel; debel zachowuje dotychczasową funkcjonalność, a jego znaleziony błąd otrzymuje test przed publikacją jako poprawny ranking.

Sesje: (1) lekcja poniżej; (2) uruchomienie na osobnych danych i zapis baseline; (3) testy reguł końca meczu/remisu/identycznych graczy; (4) samodzielne przeprowadzenie await przez przepływ i jawne błędy API; (5) ustalenie atomowej operacji wynik + Elo; (6) smoke test oraz test regresji debla.

Eksperyment A — błąd zakończenia:
1. Pojęcie: sukces HTTP a trwały efekt, async i granica transakcji.
2. Samodzielna zmiana: kontrolowany punkt awarii w testowej zależności repozytorium; następnie test z prawdziwą bazą.
3. Wywołanie: izolowany test jednego finish; wymuś asynchroniczne opóźnienie oraz błąd zapisu. Nie używaj awarii produkcyjnej bazy.
4. Przewidź kolejność odpowiedzi, resetu stanu i zmian Elo; oddziel opóźnienie od błędu.
5. Zbierz status HTTP, log kolejności, liczbę rekordów i oba Elo przed/po.
6. Poprawność: po błędzie brak częściowego wyniku i resetu udającego sukces; po sukcesie dokładnie oczekiwane wartości. Sama poprawka await nie jest dowodem atomowości.
7. Wyjaśnij koszt jednej transakcji, czas życia DbContext i zachowanie anulowanego żądania po commit.

Bramka: potrafisz wskazać właściciela transakcji, pokazać test nieprzechodzący przed poprawką i przechodzący po niej oraz odtworzyć rozumowanie bez AI. Portfolio: test awarii + diagram wywołań + krótki ADR.

Materiały: [DbContext lifetime — unikanie równoległych operacji](https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/), [transakcje EF — pojedynczy SaveChanges i wiele operacji](https://learn.microsoft.com/en-us/ef/core/saving/transactions). Zastosowanie: zaznacz na swoim diagramie, co każdy mechanizm gwarantuje, a czego nie.

### B. Minimum wdrożenia Azure — 12–18 h, lokalnie i chmura

Zależność: bramka A dla zapisu; inwentaryzację subskrypcji i kosztów robimy już równolegle czasowo z A. Minimum: API + baza + istniejący klient w minimalnej konfiguracji, jedna replika, syntetyczne dane, dostęp ograniczony. Nie czekamy na modularność, broker ani pełne testy wydajnościowe.

Sesje: (1) region/subskrypcja/RG/RBAC/PaaS i budżet; (2) .NET 10 i przegląd starych referencji; (3–4) provider SQL Server i osobny zestaw migracji na nowej bazie, weryfikacja danych oraz test integracyjny; (5) obraz i kontrolowany deploy Azure SQL + API; (6) konfiguracja klienta/SignalR, tożsamość aplikacji i dostęp do bazy; (7) health, logi i ograniczenie operacji zapisu; (8) restart i runbook usuwania zasobów. Nie kasuj historii migracji SQLite ani starych danych. Upload na minimum wyłączony lub ograniczony do administratora; trwałe zdjęcia są rozszerzeniem.

Eksperyment B — restart API:
1. Pojęcie: trwałość bazy a pamięć procesu.
2. Samodzielnie dodaj identyfikator instancji do diagnostyki i prostą kontrolę zdrowia.
3. Zakończ mecz testowy, rozpocznij kolejny, zrestartuj tylko testowe API.
4. Przewidź osobno historię, Elo, stan trwającego meczu i połączenie SignalR.
5. Zbierz odczyt bazy, log startu i wynik odświeżenia dwóch klientów.
6. Kryterium minimum: historia i Elo pozostają, reconnect sprawdzony; utrata trwającego meczu udokumentowana jako ograniczenie demonstratora. Jeśli aplikacja ma być używana do prawdziwych meczów, trwały stan jest dodatkową bramką przed takim użyciem.
7. Wyjaśnij różnicę między wysoką dostępnością, trwałością i liczbą replik.

Bramka: smoke test w Azure, brak sekretów w Git/logach, kontrola dostępu do mutacji, działające połączenie SQL, zanotowany koszt i sposób sprzątania. Portfolio: diagram wdrożenia, ADR hostingu, dowód restartu. Rozszerzenia: trwałe uploady i trwały stan meczu.

Materiały: [zakres AZ-900 — architecture/services oraz management/governance](https://learn.microsoft.com/en-us/credentials/certifications/resources/study-guides/az-900), [porównanie hostingu](https://learn.microsoft.com/en-us/azure/container-apps/compare-options). Zastosowanie: przypisz każdy używany zasób do odpowiedzialności, tożsamości i pozycji kosztowej; nie przerabiaj całego kursu przed deployem.

### C. Współbieżność, idempotencja i SQL — 14–20 h, lokalnie

Zależność: A, a testy SQL Server po B lokalnym. Minimum: jeden typ meczu i jedno zmierzone zapytanie. Sesje: (1) wyścig punktów; (2) atomowa aktualizacja stanu; (3) stabilny identyfikator meczu/komendy i duplikat finish; (4) optimistic concurrency na danych SQL; (5) izolacja i test dwóch żądań; (6–7) paginacja/projekcja historii, pomiar indeksu; (8) powtórka bez AI.

Eksperyment C1 — dwa żądania i duplikat:
1. Pojęcie: lost update, concurrency token, idempotencja i izolacja to różne mechanizmy.
2. Samodzielnie przygotuj test z barierą po odczycie stanu; później dodaj wybraną ochronę i stabilny klucz operacji.
3. Dwie niezależne instancje klienta/testowe scope'y odczytują ten sam stan, następnie równocześnie dodają punkt. Osobno wyślij dwa finish dla tego samego ID, także po utracie odpowiedzi.
4. Przewidź punkty, liczbę meczów, Elo i statusy odpowiedzi. Ustal zachowanie tego samego klucza z innym payloadem.
5. Zbierz harmonogram żądań, wersje danych i asercje SQL.
6. Poprawność: brak cichej utraty punktu; duplikat nie tworzy drugiego efektu; konflikt jest jawny albo bezpiecznie rozstrzygnięty. Rowversion w bazie nie chroni automatycznie singletona w RAM.
7. Wyjaśnij lock w procesie vs transakcję SQL, zakres idempotencji i koszt retry konfliktu.

Eksperyment C2 — indeks:
1. Pojęcie: pomiar planu, IO i czasu zamiast intuicji.
2. Wygeneruj np. 100 tys. syntetycznych meczów; wybierz jedno zapytanie historii i jeden kandydat indeksu.
3. Na tej samej bazie porównaj zapytanie przed/po, przy tych samych parametrach; rozdziel zimny i ciepły cache.
4. Przewidź wpływ na sortowanie, odczyty i koszt INSERT.
5. Zbierz actual plan, logical reads, czas, liczbę wierszy oraz SQL generowany przez EF.
6. Wyniki muszą być identyczne; poprawa udokumentowana albo indeks odrzucony na podstawie pomiaru.
7. Wyjaśnij selektywność, koszt utrzymania indeksu i dlaczego Include nie dowodzi N+1.

Bramka: deterministyczny test wyścigu, test powtórzenia i pomiar SQL. Portfolio: raport „przed/po” i ADR spójności. Materiały: [optimistic concurrency EF](https://learn.microsoft.com/en-us/ef/core/saving/concurrency), [actual execution plan SQL Server](https://learn.microsoft.com/en-us/sql/relational-databases/performance/display-an-actual-execution-plan). Zastosowanie: przewidź plan i wynik konfliktu przed uruchomieniem.

### D. Asynchroniczny ranking i granice danych — 16–24 h, głównie lokalnie

Zależność: atomowe zakończenie A i identyfikacja/idempotencja C. Minimum w miesiącu: jeden kontrakt i konsument projekcji porównawczej; pełny outbox i przełączenie własności mogą wyjść poza miesiąc.

Sesje: (1) kontrakt MatchFinished i granica modułu; (2) pojedyncza kolejka Service Bus/emulator; (3) konsument z własną bazą; (4) outbox zapisany razem z wynikiem; (5) dispatcher i retry z ograniczeniem, backoff/jitter; (6) inbox + efekt projekcji w jednej transakcji; (7) DLQ i ręczna naprawa; (8) kolejność/replay; (9) kontrakt v2 zgodny ze starym konsumentem. Debel dopiero po singlu.

Eksperyment D1 — konsument offline i duplikaty:
1. Pojęcie: at-least-once, eventual consistency, jednokrotność efektu.
2. Napisz konsumenta z transakcyjną deduplikacją i korelacją EventId/MatchId; ACK dopiero po commit.
3. Zatrzymaj konsumenta na 3 minuty, zakończ kilka meczów; wznow go, ponów ten sam komunikat; zabij konsumenta po commit, przed ACK.
4. Przewidź zaległość, opóźnione Elo, powtórne dostarczenie i liczbę efektów.
5. Zbierz logi EventId, liczbę dostarczeń, inbox, ranking i czas nadrobienia.
6. Każdy unikalny wynik ma jeden efekt biznesowy; powtórzenie jest bezpieczne; zaległość spada po uruchomieniu.
7. Wyjaśnij, dlaczego „exactly once” nie wynika magicznie z brokera i ile przechowywać inbox.

Eksperyment D2 — luka SQL/publikacja, kolejność i trucizna:
1. Pojęcie: dual write, outbox, sekwencja i DLQ.
2. Dodaj przełączniki awarii: po commit wyniku, przed publish; po publish, przed oznaczeniem outbox; uszkodzony kontrakt testowy.
3. Uruchom lokalnie każdy punkt oddzielnie. Dostarcz wyniki dwóch meczów wspólnego gracza w odwrotnej kolejności.
4. Przewidź utratę zdarzenia bez outbox, duplikat z outbox i różnicę Elo przy złej kolejności.
5. Zbierz SQL outbox/inbox, retry count, DLQ reason i ranking porównany z sekwencyjnym replay.
6. Brak utraty zatwierdzonego wyniku; duplikat bez drugiego efektu; zła kolejność wykryta/buforowana albo projekcja odbudowana zgodnie z ustaloną polityką; błędny komunikat nie blokuje kolejki bez końca.
7. Wyjaśnij koszt opóźnienia, przechowywania outbox i globalnej kolejności; retry błędu walidacji nie naprawi danych.

Bramka: odtworzenie wszystkich awarii i zgodność projekcji z referencją; zanim przełączymy odczyt, aplikacja dalej używa dotychczasowego rankingu. Portfolio: sekwencja awarii i ADR własności danych. Materiały: [System Design Primer — Asynchronism, Consistency patterns](https://github.com/donnemartin/system-design-primer), [Service Bus DLQ](https://learn.microsoft.com/en-us/azure/service-bus-messaging/service-bus-dead-letter-queues). Zastosowanie: własny diagram pokazujący każdy commit i ACK; analiza brakujących gwarancji.

### E. Eksploatacja, dowody i zamknięcie chmury — 10–16 h

Zależności: wdrożenie B; korelacja między procesami po D. Część chmurową i sprzątanie wykonujemy przed końcem oferty niezależnie od postępu D.

Sesje: (1) CI build/test i obraz z oznaczoną wersją; (2) kontrolowana migracja jako krok deployu, kopia/restore; (3) rollback kodu przy kompatybilnym schemacie; (4) logi, metryki latencji/błędów/zaległości oraz trace HTTP → komunikat → konsument; (5) timeout i spowolnienie zależności; (6) minimalne Bicep do odtworzenia wybranych zasobów, jeśli czas pozwala; (7) eksport, restore lokalny, sprzątanie; (8) prezentacja 5 minut bez AI.

Eksperyment E — timeout i rollback:
1. Pojęcie: częściowa awaria, budżet czasu, retry amplification i operacyjny rollback.
2. Dodaj kontrolowane opóźnienie testowej zależności i konfigurację timeout/retry oraz identyfikację wersji aplikacji.
3. W środowisku testowym opóźnij zależność ponad timeout; potem wdroż wersję z błędem i wróć do poprzedniego obrazu, bez destrukcyjnej migracji wstecz.
4. Przewidź status HTTP, liczbę prób, czas odpowiedzi, efekt operacji mimo timeoutu oraz zgodność starego kodu ze schematem.
5. Zbierz korelowany trace, latencję, retry count, błędy i smoke test po rollbacku.
6. Retry ograniczone, brak drugiego efektu, odtworzenie usługi udowodnione. Backup musi przejść próbę restore, nie tylko zostać utworzony.
7. Wyjaśnij kiedy retry szkodzi, czym różni się rollback kodu od cofania danych i co monitorować zamiast samych logów.

Bramka: runbook deploy/restore/cleanup wykonany; portfolio z dowodami, nie samymi deklaracjami. Materiały: [budżety Azure](https://learn.microsoft.com/en-us/azure/cost-management-billing/costs/tutorial-acm-create-budgets), [SignalR scaling](https://learn.microsoft.com/en-us/aspnet/core/signalr/scale). Zastosowanie: wyjaśnij, dlaczego druga replika nie synchronizuje ani singletona, ani wszystkich klientów. Eksperyment dwóch API jest rozszerzeniem lokalnym; nie wymaga kupowania Azure SignalR teraz.

## 5. Pierwsze cztery tygodnie — orientacyjnie, bez gwarancji tempa

Łączny pełny zakres A–E: około 62–92 h, a z poprawkami i powtórkami może przekroczyć miesiąc. Bazowe cztery tygodnie to 80 h, więc pełne wydzielenie serwisu nie jest obietnicą.

| Okres | Podstawa około 20 h | Rozszerzenie 0–10 h |
|---|---|---|
| 27.10–02.11 | A 10 h + B 8 h + 2 h powtórki; cel: pierwsze małe Azure, jeśli przechodzą bramki | Reszta A/B, diagnoza zależności; nie kolejna technologia |
| 03.11–09.11 | B 6 h + C 10 h + E 2 h (CI) + 2 h powtórki | Indeksy, testy SQL i drugi wariant konfliktu |
| 10.11–16.11 | C 6 h + D 10 h + E 2 h (telemetria) + 2 h powtórki | Outbox/inbox i awarie; krótki test rzeczywistego Service Bus w Azure |
| 17.11–23.11 | D 8 h lokalnie + E 8 h (w tym sprzątanie do 20.11) + 4 h powtórki/portfolio | Replay/kolejność lub Bicep; od 21.11 praca lokalna |

**Wariant minimum:** poprawny zapis singla, test awarii, SQL Server i jedno wdrożenie Azure z logami/kosztami, test duplikatu, pomiar zapytania, krótki eksperyment kolejki, eksport/restore i usunięcie zasobów. Nie wycinamy zabezpieczeń i kontroli kosztów dla dotrzymania daty. Jeśli B blokuje się na migracji, zmniejszamy funkcje demonstratora i rezerwujemy osobną sesję diagnozy; nie przepisujemy całej aplikacji.

Po miesiącu w razie braku czasu: pełne przełączenie rankingu na własną bazę, kontrakty v2, uporządkowany replay, debel, rozbudowane Bicep i dwie instancje SignalR. Wszystko możliwe lokalnie z SQL Server i emulatorem. Testy specyficzne dla IAM, kosztów i usług Azure robimy wcześniej; lokalny emulator nie zastępuje ich dowodu.

18–20 listopada: sprawdź eksport danych i restore lokalny; usuń wyłącznie grupę zasobów ćwiczenia po sprawdzeniu jej zawartości; sprawdź zasoby poza nią, obrazy/storage i dalsze naliczanie kosztów. Samo zatrzymanie API nie usuwa kosztu innych zasobów. Nie czekamy na automatyczne wygaśnięcie kredytu.

## 6. Diagnoza umiejętności — najwyżej pięć pytań

Odpowiedzi nie blokują analizy. Pierwsze pytanie jest dzisiejszym zadaniem; pozostałe wracają w odpowiednich sesjach.

1. Wskaż w ścieżce FinishMatch, co musi się zakończyć przed odpowiedzią HTTP, a na co obecny kod nie czeka. Co zmienia sytuacja, gdy Task kończy się synchronicznie?
2. Czy oddzielne lock w Get i Set gwarantują dwa punkty z dwóch równoległych żądań? Rozpisz konkretny przeplot.
3. Gdzie postawisz granicę transakcji dla wyniku i dwóch Elo oraz jak odróżnisz atomowość od ochrony przed lost update?
4. Konsument zatwierdził zmianę Elo, ale nie wysłał ACK. Jak obsłużysz ponowne dostarczenie bez utraty wyniku? Co z kolejnością dwóch meczów wspólnego zawodnika?
5. Jak odróżnisz wolny SQL od opóźnienia zależności na podstawie dowodów oraz jak sprawdzisz, czy proponowany indeks pomaga?

Nie wiemy jeszcze, jak samodzielnie diagnozujesz te przypadki, projektujesz testy, interpretujesz plan wykonania i konfigurujesz Azure. CV wskazuje doświadczenie z RabbitMQ/Orleans/CI, więc nie zaczynamy od definicji klas i HTTP; poziom trudności skorygujemy po Twojej pracy.

## 7. Pierwsza lekcja — dziś, 27 października, 60–90 minut

Temat: **Kiedy API naprawdę może powiedzieć „mecz zapisany”?** Nie implementujemy dziś docelowej architektury. Plan sesji to mapa dla mentora; Adam wykonuje teraz tylko krok 1, następne zadanie dostaje po krótkim review.

### Krok 1 — Twoje pierwsze zadanie, 15–20 minut

Otwórz kolejno:

- `LowOnLegs/LowOnLegs/Controllers/MatchesController.cs` — FinishMatch.
- `LowOnLegs/LowOnLegs.Services/MatchService.cs` — FinishMatch i UpdateSinglesElo.
- `LowOnLegs/LowOnLegs.Data/Repositories/MatchRepository.cs` — Add.
- `LowOnLegs/LowOnLegs.Data/Repositories/PlayerRepository.cs` — UpdateElo.

Bez zmieniania kodu rozpisz kolejność od POST finish do odpowiedzi. Przy każdej operacji zaznacz: zwracany typ, czy caller czeka, jaki efekt zmienia i gdzie trafia błąd. Załóż dwóch różnych graczy i wynik 11:8.

Następnie przewidź: **co zobaczy klient, jaki będzie stan nowego meczu oraz co może znaleźć się w bazie, jeśli zapis meczu się nie powiedzie?** Rozdziel to, co wynika z kodu zawsze, od tego, co zależy od momentu ukończenia Task. Nie wystarczy odpowiedź „brakuje await”. Przyślij tabelę lub 5–10 zdań. Nie potrzebujesz dziś działającego Dockera, żeby zrobić ten krok.

### Kolejne kroki sesji — mentor podaje po review

- 20–35 min: omówienie przewidywania; narysowanie dwóch osi czasu: operacja kończy się od razu vs zostaje zawieszona. Wskazówka 1 w razie potrzeby: porównaj sygnatury metod. Wskazówka 2: prześledź dokładnie konsumenta `Task<bool>`.
- 35–50 min: krótka lektura sekcji DbContext lifetime z materiału A; własnymi słowami powiąż ją z czasem życia żądania. Bez czytania całego kursu async.
- 50–75 min: zaprojektuj jeden test, który potrafiłby obalić Twoją hipotezę: przygotowanie, kontrolowana awaria, obserwacje HTTP i danych, warunek zaliczenia. Jeśli środowisko jest już gotowe, implementacja testu może być kolejnym zadaniem po uzgodnieniu zakresu; nie uruchamiaj na używanej bazie.
- 75–90 min: trzy zdania wniosków i pytanie „czy dodanie await rozwiązuje również częściowy zapis i duplikaty?”. Zostaw to jako most do lekcji 2.

Wersja 60-minutowa: skróć lekturę i zaprojektuj test na papierze. Rezultat: mapa przepływu, przewidywanie i specyfikacja testu. Kryterium: umiesz wskazać różnicę między rozpoczęciem operacji, zakończeniem Task, zatwierdzeniem transakcji i odpowiedzią HTTP. Nie oceniamy szybkości ani znajomości składni na pamięć.

## Lekcja dodatkowa D0 — Docker w LowOnLegs od podstaw

Uzgodnienie z Adamem, 28.10.2026: Docker poznajemy w osobnej lekcji po zakończeniu obecnego eksperymentu z błędem HTTP, przed pierwszym wdrożeniem Azure. Teraz wystarczą komendy uruchomienia, przebudowania, logów i zatrzymania. Zadanie z analizą COPY odkładamy do tej lekcji. Nie zakładamy znajomości konfiguracji napisanej wcześniej przez AI.

Cel: samodzielnie wyjaśnić, uruchomić, zmodyfikować i odtworzyć konfigurację kontenerów tego projektu. Zakres obejmuje używane tu mechanizmy, nie całą platformę Docker. Czas: 3 sesje po 60–90 minut, łącznie 3–4,5 h, w ramach przygotowania do etapu B; zastępują część ogólnego czasu na kontenery. Dodatkowo opcjonalna powtórka 30–60 minut. Nie blokujemy Azure nauką zaawansowanej administracji Dockerem.

Mentor przed każdym poleceniem wyjaśnia jego cel, argumenty i skutek. Adam przewiduje wynik przed eksperymentem, wykonuje zmianę sam i opisuje obserwację. Jedno zadanie naraz; bez gotowej konfiguracji do bezrefleksyjnego przepisania.

### Sesja 1 — od kodu do obrazu backendu

- Model: obraz, kontener, rejestr, warstwy i cache; plik na hoście a plik wewnątrz obrazu.
- Pliki: `docker-compose.yml`, backendowy `Dockerfile` oraz pliki `.dockerignore`, jeśli istnieją — najpierw inwentaryzacja ich faktycznej zawartości.
- Przejście po backendowym Dockerfile: FROM i nazwy etapów base/build/publish/final; SDK a runtime; WORKDIR; COPY i kontekst build; RUN; ARG; USER i uprawnienia katalogów; EXPOSE; ENTRYPOINT.
- Wyjaśnić restore/build/publish, osobne kopiowanie csproj dla cache, COPY --from oraz to, które pliki trafiają do finalnego obrazu. Oddzielić konfigurację czasu budowania od zmiennych czasu uruchomienia.
- Eksperyment: przewidzieć użycie cache przy ponownym buildzie, potem po zmianie pliku .cs i osobno pliku projektu; zebrać oznaczenia etapów z logów budowania. Nie zmieniać zależności wyłącznie dla eksperymentu; wystarczy kontrolowana, odwracalna zmiana pliku.
- Kryterium: Adam wskazuje pełne ścieżki źródła i celu wybranego COPY, tłumaczy potrzebę SDK podczas budowania oraz brak potrzeby SDK w finalnym kontenerze.
- Rezultat: własna mapa „kod → etapy budowania → obraz → proces API”.

### Sesja 2 — aplikacja jako zestaw usług

- Przejść przez każdą aktualną sekcję Compose: services, build/context/dockerfile, environment, volumes, restart, ports, depends_on i definicję named volume. Wyjaśnić minimalną składnię YAML: wcięcia, mapy i listy.
- Sieć: nazwa usługi api, port hosta i kontenera, znaczenie localhost z perspektywy przeglądarki i kontenera. EXPOSE nie publikuje portu na hoście. depends_on nie jest dowodem gotowości API do obsługi żądań.
- Frontendowy Dockerfile: Node buduje pliki statyczne, finalny Nginx je serwuje. `nginx.conf`: routing SPA, proxy /api, /uploads i WebSocket dla SignalR. Prześledzić drogę przeglądarka → port hosta → Nginx → api:8080.
- Konfiguracja .NET: ASPNETCORE_ENVIRONMENT, ASPNETCORE_URLS, ConnectionStrings__DefaultConnection i znaczenie podwójnego podkreślenia.
- Eksperyment: zmienić wyłącznie port hosta frontendu w izolowanej konfiguracji ćwiczeniowej, przewidzieć nowy URL i to, czy trzeba zmieniać adres api w Nginx. Sprawdzić zwykłe HTTP i SignalR, potem cofnąć zmianę.
- Kryterium: Adam wyjaśnia, dlaczego frontend trafia do API bez publikowania portu backendu oraz dlaczego edycja C# nie aktualizuje automatycznie istniejącego kontenera.
- Rezultat: diagram sieci i własny opis każdej sekcji Compose.

### Sesja 3 — dane, diagnostyka i samodzielne odtworzenie

- Named volume, bind mount i zapisywalna warstwa kontenera; ścieżki SQLite i zdjęć w obecnym projekcie. Rozróżnić stop/start, restart, recreate, rebuild i usunięcie wolumenu. Omówić automatyczne migracje przy starcie API i ich konsekwencje.
- Narzędzia: compose config (może wyświetlać rozwinięte wartości konfiguracji — nie publikować sekretów), ps, logs, build, up, stop/start, down; inspect i exec tylko do konkretnego pytania diagnostycznego. Rozróżnić Ctrl+C przy podglądzie logów od zatrzymania usług.
- Eksperyment trwałości: wyłącznie osobny projekt Compose z własnym pustym wolumenem i syntetycznymi danymi. Przewidzieć stan historii, bieżącego meczu w RAM i uploadu po odtworzeniu API; zebrać wyniki przed/po. Nie usuwać istniejącego wolumenu projektu i nie wykonywać prune. Usuwanie danych omówić, bez potrzeby wykonywania go na używanej bazie.
- Diagnostyka: rozróżnić błąd builda, zakończenie procesu API i błąd proxy. Zidentyfikować właściwy log dla każdego przypadku.
- Zadanie końcowe: samodzielnie napisać w pliku ćwiczeniowym minimalny Compose API + frontend + trwała baza, wyjaśnić każdą linię i porównać z istniejącym plikiem. Mentor nie zastępuje używanej konfiguracji za ucznia.
- Kryterium: samodzielny start, rebuild po zmianie, odczyt logu błędu, wyjaśnienie trwałości danych i odtworzenie minimalnej konfiguracji bez kopiowania od AI.
- Rezultat do portfolio: krótka instrukcja uruchomienia i diagnozy, diagram oraz tabela przewidywań i obserwacji.

Powiązanie z dalszą nauką: w CI budujemy i publikujemy ten sam obraz, a w CD uruchamiamy jego określoną wersję z konfiguracją środowiska. Dopiero tam dokładamy szczegóły rejestru, tagowania i dostępu z Azure DevOps. Nie rozszerzamy D0 o Kubernetes ani administrację klastrem.

## 8. Dziennik postępu

| Data | Zadanie | Przewidywanie / dowód / wniosek | Status |
|---|---|---|---|
| 2026-10-27 | Analiza repo i plan mentora | Analiza statyczna, bez uruchomienia; kredyt Azure potwierdzony przez Adama | Przygotowane |
| 2026-10-28 | Lekcja 1, krok 1: FinishMatch | Adam prześledził kontroler, zapis, Elo i reset; rozpoznał ignorowany rezultat oraz brak await. Do doprecyzowania: Add zwraca Task<bool>, a powrót null-guard to normalne wyjście z void. | Omówione; następnie przewidywanie awarii |
| 2026-10-28 | Lekcja 1 — async, błędy HTTP i odświeżenie DTO | Adam potwierdził HTTP 500 i wyjątek w logach, poprawny ranking z bazy, odświeżone Elo w odpowiedzi finish oraz zachowanie bez jednego gracza. Podsumowanie w notatki/01-async-bledy-dto.md. Transakcja pozostaje do zrobienia. | Ukończona na podstawie eksperymentów Adama |
| 2026-10-28 | Docker D0, sesja 1 — kod i obraz | Początek: FROM, obrazy SDK/runtime i etapy budowania. | W toku |

### Ustalenie po pierwszej analizie Adama — 28.10.2026

Nie odkładamy błędów poprawności zapisu do wydzielania mikroserwisu. Przed Azure robimy ograniczoną poprawkę: oczekiwanie na operacje w całej ścieżce, jawna obsługa nieudanego zapisu i brak resetu udającego sukces. Atomowość wyniku i Elo sprawdzamy osobno — samo await jej nie zapewnia. Estetykę, reorganizację klas i granice modułów zostawiamy na później. Najbliższe zadanie: przewidzieć HTTP, Elo i stan meczu, gdy Add zwróci ukończony Task z false, a kolejne operacje zakończą się poprawnie. Bez implementacji przez mentora.

Do ustalenia przed provisioningiem: limit własnych wydatków, typ subskrypcji, region i faktyczne limity/SKU. Do ustalenia podczas uruchomienia lokalnego: SDK, Docker i izolowana baza ćwiczeniowa. Nie przesyłaj connection stringów, tokenów ani kluczy do czatu.
