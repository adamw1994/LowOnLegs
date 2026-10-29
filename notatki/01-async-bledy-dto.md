# Lekcja 01 — async, błędy i DTO

Ukończona 28.10.2026. Eksperymenty manualne wykonane przez Adama, nie testy automatyczne.

## Najważniejsze

- `Task` reprezentuje zakończenie operacji; `await` oczekuje na nie i propaguje wyjątek. Nie oznacza automatycznie nowego wątku.
- Oczekujemy na zapis meczu, oba zapisy Elo kolejno i całą operację w kontrolerze. Nie uruchamiamy równolegle operacji na tym samym DbContext.
- Repozytorium `Add` zwraca teraz `Task`. `catch { return false; }` ukrywał wyjątek; pozwalamy mu przejść wyżej.
- Wyjątek z oczekiwanej operacji przerywa normalną ścieżkę: dalsze instrukcje, np. `StartMatch`, nie wykonają się, jeśli błąd nie zostanie wcześniej obsłużony.
- `AddProblemDetails()` rejestruje usługę tworzenia standardowych odpowiedzi błędów; `UseExceptionHandler()` dodaje ich obsługę do potoku HTTP. Pierwsze przed Build, drugie wcześnie w potoku po Build.
- Klient otrzymuje HTTP 500 i ogólny opis; log backendu zawiera wyjątek i stack trace. Handler nie cofa zapisów w bazie.
- DTO zawiera skopiowane dane. Zmiana Elo w bazie nie odświeża DTO w stanie meczu. Po aktualizacji pobieramy graczy ponownie do następnego stanu.
- Brak gracza pozostawiamy jako `null`, bez zapytania o sztuczne ID 0. Obecnie brak jednego gracza oznacza pominięcie zapisu meczu i Elo.

## Co sprawdziliśmy

- Kontrolowany wyjątek przed zapisem → HTTP 500, ProblemDetails i pełny wyjątek w logach.
- Po usunięciu awarii: zakończenie meczów i aktualny ranking w endpointcie statystyk.
- Po odświeżeniu DTO: aktualne Elo również w odpowiedzi finish — potwierdzone przez Adama.
- Brak jednego gracza → brak błędu, Elo wybranego gracza bez zmiany.

## Nadal do zrobienia

Wspólna transakcja wyniku i Elo. Dziś awaria aktualizacji Elo może pozostawić zapisany wynik i częściowo zmieniony ranking. `await` zapewnia oczekiwanie, nie atomowość.

## Powtórka bez zaglądania do kodu

1. Dlaczego samo dodanie await nie naprawia ignorowanego false?
2. Gdzie przechwytywany jest wyjątek i dlaczego jego treść nie trafia domyślnie do klienta?
3. Dlaczego odpowiedź finish mogła zawierać Elo 1000 mimo poprawnego zapisu?
