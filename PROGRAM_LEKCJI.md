# LowOnLegs — pełny program lekcji

Wersja programu: 29.10.2026. To trwała mapa całej uzgodnionej ścieżki, a nie tylko zapis postępu. Szczegółowa diagnoza kodu, eksperymenty, materiały i uzasadnienia: [SCIEZKA_BACKEND_LOWONLEGS.md](SCIEZKA_BACKEND_LOWONLEGS.md). Skróty ukończonych lekcji: [notatki](notatki/README.md).

## Jak korzystamy z programu

- Lekcja to temat i kryterium opanowania; może zająć kilka sesji po 60–120 minut. Nie obiecujemy realizacji całego programu w miesiąc.
- Kolejność poniżej jest domyślna. Zmianę celu, zakresu lub kolejności zapisujemy tutaj z uzasadnieniem, zamiast po cichu zastępować program.
- Tematy i rezultaty są zaplanowane teraz. Dokładne polecenia i kolejne małe zadania dobieramy na podstawie faktycznego kodu i pracy Adama. To nie są gotowe implementacje ćwiczeń.
- Wyjaśnienie mechanizmu przed użyciem → przewidywanie → samodzielny eksperyment → dowód → review → krótka notatka. Jedno zadanie naraz, pełne rozwiązanie dopiero na prośbę.
- Preferencja CI/CD: Azure DevOps Pipelines w YAML. Adam korzystał wcześniej z gotowych buildów i release'ów; tutaj sam pisze konfigurację i diagnozuje błędy.
- Orientacyjnie 20 h tygodniowo, dodatkowe 0–10 h na utrwalenie. Szacunki lekcji nie obejmują dowolnie długiej diagnozy środowiska.

## I. Zrozumienie aplikacji i kontenerów

| Nr | Lekcja | Praktyka i kryterium ukończenia | Czas |
|---|---|---|---|
| 01 | Async, propagacja błędów i aktualność DTO | Prześledź finish; await przez całą ścieżkę; ProblemDetails i handler; wymuszona awaria → 500 i log; po sukcesie aktualne Elo w odpowiedzi; brak gracza bez zmiany Elo. Ukończona na podstawie eksperymentów Adama. | Zrealizowana |
| 02 | Docker: kod → obraz → proces | Przeczytaj backendowy Dockerfile: kontekst, FROM/etapy, COPY, WORKDIR, RUN, ARG, USER, EXPOSE, ENTRYPOINT. Porównaj cache po zmianie kodu i pliku projektu. Własnymi słowami odtwórz drogę do finalnego obrazu bez SDK. | 1–1,5 h |
| 03 | Compose, sieć i frontend w kontenerze | Wyjaśnij każdą sekcję Compose, Node/Nginx oraz routing HTTP/SignalR. Zmień testowo port hosta; przewidź, które adresy się zmienią. Narysuj przeglądarka → Nginx → API. | 1–1,5 h |
| 04 | Docker: dane, logi i diagnoza | Na izolowanych danych porównaj restart, rebuild i recreate; historia SQLite, stan RAM, uploady. Znajdź log dla błędu procesu i proxy. Samodzielnie odtwórz minimalny Compose w pliku ćwiczeniowym. | 1–1,5 h |

Lekcje 02–04 odpowiadają szczegółowo rozpisanej lekcji D0 w planie etapowym. Nie wymagamy całej administracji Dockerem przed Azure.

## II. Minimum poprawności i pierwsze Azure

| Nr | Lekcja | Praktyka i kryterium ukończenia | Czas |
|---|---|---|---|
| 05 | Reguły meczu i pierwsze testy | Samodzielnie przygotuj testy końca meczu, remisu, przewagi i tego samego gracza; ustal zachowanie API. Dodaj test ujawniający błąd Elo debla przed jego poprawieniem. Wyjaśnij, co testować bez bazy. | 2–4 h |
| 06 | Transakcja wyniku i Elo | Wymuś awarię pomiędzy zapisami. Zaprojektuj granicę transakcji i potwierdź brak częściowych zmian. Rozróżnij oczekiwanie na Task, commit i reset stanu w RAM. | 2–4 h |
| 07 | SQL Server zamiast SQLite | Przygotuj izolowany SQL Server, provider i osobne migracje dla nowej bazy. Test integracyjny wykonuje prawdziwy zapis i rollback. Zachowaj wcześniejsze dane/migracje SQLite; pokaż, jak oglądać tabele. | 3–5 h |
| 08 | Azure: minimum pojęć, koszt i wybór hostingu | Na używanych zasobach poznaj subskrypcję, RG, region, RBAC i PaaS. Porównaj App Service i Container Apps, zapisz ADR. Sprawdź ofertę i koszt; ustal limit własnych wydatków. Alert budżetowy nie jest odcięciem kosztów. | 1–2 h |
| 09 | Minimum bezpieczeństwa i konfiguracji | Prześledź konfigurację .NET i zmienne środowiskowe; odróżnij CORS, uwierzytelnianie i autoryzację. Ogranicz zapis/upload w demonstratorze, usuń sekrety z kodu, przygotuj wspieraną wersję .NET i obrazu. Potwierdź testem brak dostępu bez uprawnień. | 2–4 h |
| 10 | Pierwszy deploy API i bazy | Wdróż mały demonstrator na Container Apps + Azure SQL, jeśli ADR i oferta potwierdzą wybór. Konfiguracja, tożsamość i połączenie bazy; minimalny istniejący frontend. Smoke test oraz restart z rozdzieleniem trwałych danych i RAM. Jedna replika, deploy pomiędzy meczami. | 3–5 h |

Lekcję 08 wykonujemy wcześniej, równolegle do przygotowań lokalnych, jeśli potrzebna jest weryfikacja czasu/kosztów. Pierwszy deploy nie czeka na broker, moduły ani mikroserwisy. Minimum każdej lekcji to jeden przepływ singla; rozszerzenia nie blokują wdrożenia.

## III. Samodzielne CI/CD w Azure DevOps

| Nr | Lekcja | Praktyka i kryterium ukończenia | Czas |
|---|---|---|---|
| 11 | CI od pustego pliku YAML | Zrozum trigger, agenta, stage/job/step, zmienne i artefakt. Napisz restore/build/test; celowo zepsuj test i znajdź przyczynę w logach. Nie publikuj obrazu, gdy testy nie przechodzą. | 2–4 h |
| 12 | Budowanie i publikowanie obrazu | CI buduje obraz, oznacza go identyfikatorem wersji/commita i publikuje do jednego rejestru. Wyjaśnij service connection i uprawnienia. Potrafisz wskazać obraz odpowiadający konkretnemu commitowi. | 2–3 h |
| 13 | CD: wdrożenie i kontrola wyniku | Napisz etap deployu istniejącego obrazu, oddziel konfigurację środowiska, wykonaj smoke test. Wyjaśnij różnicę między udanym krokiem wdrożenia a działającą aplikacją; wdrażaj bez ponownego budowania artefaktu. | 2–4 h |
| 14 | Migracje, rollback i restore | Oddziel kontrolowane migracje od startu każdej instancji, przygotuj kompatybilną zmianę schematu. Cofnij kod do poprzedniego obrazu; sprawdź zgodność schematu. Udowodnij odtworzenie kopii danych. | 2–4 h |

Minimum CI/CD przed końcem oferty: jeden pipeline build/test/obraz i kontrolowany deploy na jedno środowisko. Zaawansowane strategie rolloutów oraz kolejne środowiska są rozszerzeniem.

## IV. Współbieżność, idempotencja i wydajność danych

| Nr | Lekcja | Praktyka i kryterium ukończenia | Czas |
|---|---|---|---|
| 15 | Dwa żądania zmieniają ten sam stan | Odtwórz utratę punktu z kontrolowanym przeplotem Get/Set. Zaproponuj atomową aktualizację. Wyjaśnij, dlaczego lock procesu nie chroni wielu instancji. Test jest powtarzalny. | 2–3 h |
| 16 | Izolacja i optimistic concurrency | Dwa niezależne DbContext zmieniają ten sam rekord. Poznaj token wersji, konflikt i świadomą politykę retry. Potrafisz odróżnić to od wspólnej transakcji oraz ochrony singletona. | 2–4 h |
| 17 | Powtórzony finish i idempotencja API | Ustal stabilną tożsamość meczu/komendy. Wyślij duplikat, równoległy duplikat i ponowienie po utracie odpowiedzi; sprawdź pojedynczy efekt. Ustal ten sam klucz z innym payloadem i czas przechowywania wyniku. | 2–4 h |
| 18 | SQL: plan, indeksy i koszt zapytania | Syntetyczna historia, SQL generowany przez EF, liczba zapytań, paginacja/projekcja i actual plan. Porównaj IO/czas przed i po indeksie przy identycznych wynikach; odróżnij N+1 od przetwarzania pełnej historii w RAM. | 3–5 h |

## V. Jeden wydzielony ranking i system rozproszony

| Nr | Lekcja | Praktyka i kryterium ukończenia | Czas |
|---|---|---|---|
| 19 | Granice modułów i własność danych | Porównaj monolit, modularność i osobny ranking. Zapisz ADR edukacyjnego wydzielenia; ustal API/zdarzenie i właścicieli danych. Brak odczytów wspólnych tabel przez nowy komponent. | 1–2 h |
| 20 | Pierwszy kontrakt i kolejka | Zaprojektuj MatchFinished v1. Użyj jednego brokera: Service Bus, lokalnie emulator. Publikuj i odbieraj zdarzenie; poznaj ACK, lock i dostarczenie co najmniej raz. Wyjaśnij ograniczenia emulatora. | 2–4 h |
| 21 | Osobna projekcja rankingu | Konsument singli zapisuje we własnej bazie i działa początkowo porównawczo. Zatrzymaj go na kilka minut, wznow i porównaj projekcję z referencją. Wyjaśnij eventual consistency. | 2–4 h |
| 22 | Transactional outbox | Wymuś awarię po commit wyniku/przed publish oraz po publish/przed oznaczeniem wysyłki. Zapisz wynik i outbox w jednej transakcji. Dispatcher nie gubi zatwierdzonych zdarzeń i może publikować duplikaty. | 3–5 h |
| 23 | Inbox i jednokrotność efektu | Zapisz deduplikację i efekt konsumenta w jednej transakcji. Zabij konsumenta po commit/przed ACK; ponowne dostarczenie nie zmienia drugi raz Elo. Wyjaśnij dostarczenie vs efekt biznesowy. | 2–4 h |
| 24 | Timeout, retry i DLQ | Awaria zależności, backoff/jitter, limit prób i błędny komunikat. Poznaj kiedy retry szkodzi. Zdiagnozuj DLQ, popraw przyczynę i bezpiecznie ponów wiadomość. | 2–4 h |
| 25 | Kolejność, replay i wersjonowanie | Odwróć wyniki dwóch meczów wspólnego gracza. Ustal sekwencję i deterministyczną odbudowę Elo. Sprawdź kontrakt v2 ze starym konsumentem; określ politykę korekt wyniku. | 3–5 h |
| 26 | Kontrolowane przełączenie rankingu | Porównaj nową projekcję ze starym odczytem; przełącz właściciela zapisu/odczytu i zapewnij ścieżkę odwrotu. API i ranking nie zapisują równorzędnie tych samych danych. Istniejący klient działa przy minimalnych zmianach. | 2–4 h |

Lekcje 20–26 wymagają stabilnej identyfikacji wyniku i idempotencji z 17. Nie wydzielamy serwisu na każdą encję. To koszt edukacyjny, a nie deklaracja potrzeby mikroserwisów w tej aplikacji. Pełny zakres tej części może być realizowany już po wygaśnięciu Azure, lokalnie.

## VI. Utrzymanie i domknięcie programu

| Nr | Lekcja | Praktyka i kryterium ukończenia | Czas |
|---|---|---|---|
| 27 | Logi, metryki i tracing | Powiąż HTTP → outbox → broker → konsument; zmierz latencję, błędy i zaległość. Spowolnij zależność i wskaż przyczynę na podstawie dowodów. Podstawowe logi/health robimy już przy deployu; tu śledzimy wiele procesów. | 2–4 h |
| 28 | Powtarzalna infrastruktura | Opisz minimalne używane zasoby w Bicep; rozdziel konfigurację od sekretów. Odtwórz testowe środowisko i porównaj z opisem. Zakres redukujemy, jeśli zagrozi terminowi zamknięcia chmury. | 2–4 h |
| 29 | Eksport danych i sprzątanie Azure | Wykonaj restore lokalny, sprawdź wszystkie zasoby ćwiczenia, usuń właściwe zasoby i zweryfikuj dalsze koszty. Ta lekcja ma termin kalendarzowy i nie czeka na 19–28. | 1–2 h |
| 30 | Powtórka i prezentacja decyzji | Bez AI odtwórz wybrany fragment konfiguracji/testu. Opowiedz o awarii, alternatywach i kompromisach; przedstaw test, pomiar, diagram i krótki ADR. Zidentyfikuj luki do kolejnego cyklu. | 2–3 h |

## Ograniczenie Azure i wariant minimum

Kredyt zgłoszony przez Adama: 175,99 EUR według stanu z 28.10; oferta wygasa 21.11.2026. To nie aktualny odczyt salda. Przed zasobami sprawdzamy bieżące limity, region/SKU i zgodę na własne wydatki.

- Pierwsze wdrożenie planujemy wcześnie: po niezbędnym minimum 02–10. Gdy testy ujawnią problem, skracamy rozszerzenia i zakres demonstratora zamiast pomijać poprawność lub ochronę dostępu.
- Do 17.11: priorytet dla hostingu, tożsamości, kosztów, CI/CD i krótkiego eksperymentu rzeczywistego Service Bus, jeśli zależności pozwolą. Nie trzeba ukończyć lokalnego serwisu rankingu, żeby sprawdzić oddzielny techniczny przykład nadania/odbioru w chmurze.
- 18–20.11: obowiązkowo lekcja 29, nawet jeśli numeracyjnie nie doszliśmy jeszcze do niej. Nie zakładamy dokładnej godziny wygaśnięcia 21.11.
- Po ofercie: SQL Server i emulator lokalnie; dalsze tematy rozproszone bez utrzymywania zasobów Azure.

Całość to orientacyjnie **57–105 godzin po lekcji 01** (górna granica zaokrąglona), plus powtórki i nieprzewidziana diagnostyka. To program na więcej niż sam dostęp do chmury, nie zobowiązanie kalendarzowe. Minimum miesiąca: wiarygodny zapis, kontenery, SQL Server, małe Azure, własne CI/CD, podstawowy eksperyment kolejki oraz restore i cleanup. Zaawansowane wydzielenie realizujemy później, jeżeli zabraknie czasu.

## Rozszerzenia po podstawie — nieobowiązkowe

- R1. Trwały stan bieżącego meczu: restart nie gubi punktów, test odzyskania po awarii.
- R2. Dwie instancje API i SignalR: odtworzenie rozjazdu stanu/powiadomień, dobór mechanizmu synchronizacji i routing połączeń.
- R3. Debel w projekcji: czterech graczy, kolejność i testy aktualizacji zespołów.
- R4. Uploady: walidacja zawartości/rozmiaru, trwały storage i uprawnienia.
- R5. Obciążenie i skalowanie: ustalony scenariusz, pomiar wąskiego gardła i kosztu poprawy.

Kubernetes, service mesh, event sourcing, pełny CQRS i dodatkowy broker/chmura pozostają poza tym programem.

## Wznowienie na innym komputerze lub w nowej rozmowie

Przekaż mentorowi: „Przeczytaj PROGRAM_LEKCJI.md, SCIEZKA_BACKEND_LOWONLEGS.md i notatki/. Kontynuuj w trybie mentora, jedno zadanie naraz, z wyjaśnieniem przed użyciem nowego mechanizmu. Nie implementuj za mnie”.

Ten program oraz notatki są zwykłymi plikami repozytorium. Przeniesienie wymaga ich skopiowania albo zapisania w Git i wysłania do zdalnego repo. Sam zapis pliku na tym komputerze nie tworzy kopii zapasowej. W tej sesji nie wykonywano commita ani push.
