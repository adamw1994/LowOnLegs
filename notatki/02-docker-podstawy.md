# Lekcja 02 — Dockerfile, obrazy i cache

Ukończona w zakresie podstaw. Eksperyment cache wykonany przez Adama.

## Obraz i Dockerfile

- Obraz zawiera aplikację i środowisko; kontener jest jego uruchomioną instancją. .NET na hoście nie jest potrzebny, jeśli wymagany runtime jest w obrazie.
- Dockerfile nie ma rozszerzenia; ma własną składnię instrukcji. Compose jest zapisany w YAML.
- `FROM` rozpoczyna etap od obrazu lub wcześniejszego etapu; `AS` nadaje nazwę.
- SDK buduje/publikuje aplikację; finalny obraz korzysta z runtime ASP.NET Core i skopiowanych plików publish.
- `WORKDIR` ustawia katalog wewnątrz obrazu. Źródło zwykłego `COPY` jest względem kontekstu budowania, cel względny — względem WORKDIR. `COPY --from=publish` czyta z etapu publish.
- `RUN` wykonuje polecenie podczas budowania. `ENTRYPOINT` określa program startujący wraz z kontenerem.
- `ARG` to parametr budowania. `UseAppHost=false` pomija osobny program uruchamiający; API startuje przez `dotnet LowOnLegs.API.dll`. Apphost i dołączenie runtime (SelfContained) to osobne ustawienia.
- `USER $APP_UID` uruchamia proces bez root; wcześniej mkdir/chown przygotowuje katalogi do zapisu.

## Cache — obserwacja Adama

- Przed zmianami kroki były CACHED dzięki wcześniejszym buildom.
- Po zmianie komentarza w `.cs`: `RUN dotnet restore` pozostało CACHED, `COPY . .` wykonało się ponownie.
- Dlatego najpierw kopiujemy csproj i wykonujemy restore, potem resztę kodu. Zmiana zawartości csproj unieważni ten wcześniejszy krok — omówione, nie potwierdzone osobnym eksperymentem.


## Powtórka

- Dlaczego finalny obraz nie potrzebuje SDK?
- Czym różnią się RUN i ENTRYPOINT?
- Dlaczego kopiujemy csproj przed resztą kodu?
- Zmiana C#: `docker compose up -d --build api`. Sam restart nie buduje nowego obrazu.
- Komendy: [ściąga](SCIAGA.md).
