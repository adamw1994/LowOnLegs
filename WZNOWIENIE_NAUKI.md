# Wznowienie nauki LowOnLegs

Aktualizacja: 30.10.2026. Przeczytaj ten plik po zmianie komputera lub rozpoczęciu nowego czatu.

## Materiały źródłowe

- `PROGRAM_LEKCJI.md` — pełna kolejność i kryteria lekcji.
- `SCIEZKA_BACKEND_LOWONLEGS.md` — diagnoza repo, eksperymenty i zasady mentoringu.
- `notatki/README.md` — ukończone lekcje 01–04 i ściąga komend.

Mentor daje jedno małe zadanie naraz i wyjaśnia każdy nowy mechanizm przed jego użyciem. Adam wykonuje implementację. Nie pisać rozwiązania za niego ani nie przechodzić dalej bez sprawdzenia rozumienia. Podsumowania numerować zgodnie z programem.

## Bieżąca lekcja 05 — reguły meczu i pierwsze testy

Adam ustalił: wynik pozwalający zakończyć mecz wymaga co najmniej 11 punktów i przewagi 2. IsGameOver istnieje, ale FinishMatch nie wykorzystuje go do ochrony zapisu. Odrzucenie 11:10 nie powinno zapisać meczu, zmienić Elo ani rozpocząć nowego meczu. Docelowy sygnał w tym pierwszym teście: InvalidOperationException z serwisu; mapowanie błędu domenowego na HTTP będzie osobną decyzją, nie zakładać, że 500 jest docelowo poprawne.

Projekt testowy utworzony, dodany do solution i referencjonuje Services. Testy na net10.0, backend na net9.0, ponieważ lokalny szablon xUnit obsługiwał net10.0. NSubstitute dodany; ręczne RecordingMatchRepository nie jest już zadaniem. Wyjaśniono Arrange/Act/Assert oraz stub/spy/mock/fake.

W `LowOnLegs/LowOnLegs.Services.Tests/MatchServiceTests.cs` jest metoda `FinishMatch_WhenScoreIs11To10_ShouldRejectWithoutSideEffects`. Arrange gotowe: dwóch graczy, rzeczywisty MatchStateManager, wynik 11:10 zapisany przez SetMatchState, zastępniki obu repozytoriów i skonstruowany MatchService. Act i Assert jeszcze puste według odczytu z 30.10.2026. Nie potwierdzono uruchomienia testu.

## Dokładny następny krok

Adam ma dodać do testu:

```csharp
await Assert.ThrowsAsync<InvalidOperationException>(
    () => service.FinishMatch());
```

Wyjaśniono już, że lambda przekazuje operację do xUnit, a ThrowsAsync ją uruchamia i oczekuje określonego wyjątku. Następnie Adam uruchamia:

```powershell
dotnet test LowOnLegs/LowOnLegs.Services.Tests/LowOnLegs.Services.Tests.csproj
```

Spodziewamy się czerwonego testu z powodu braku odrzucenia wyniku. Nie poprawiać jeszcze serwisu. Sprawdzić rzeczywisty komunikat: niepowodzenie przez inny wyjątek lub błąd konfiguracji nie jest dowodem wykrycia brakującej reguły. Po prawidłowym czerwonym teście kolejne zadania to asercje braku zapisów i zachowania stanu oraz minimalna poprawka — po jednym kroku. Pełnego rozwiązania nie podano.

## Przeniesienie środowiska

Pliki i kod wymagają commita/push na dotychczasowym komputerze, a następnie clone/pull na nowym. Ten plik nie jest kopią całej rozmowy. Nie zakładać synchronizacji lokalnych rozmów Codex, instalacji SDK, obrazów Dockera ani wolumenów przez Git. Do obecnego testu jednostkowego potrzebne jest SDK .NET 10 i odtworzenie pakietów; Docker i historyczna baza nie są potrzebne.
