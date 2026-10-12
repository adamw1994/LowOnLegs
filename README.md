# 🏓 LowOnLegs — Table Tennis Scoreboard

**Track every point. Keep the rivalry going.**

![.NET](https://img.shields.io/badge/.NET-9-512BD4?logo=dotnet&logoColor=white)
![React](https://img.shields.io/badge/React-19-149ECA?logo=react&logoColor=white)
![TypeScript](https://img.shields.io/badge/TypeScript-3178C6?logo=typescript&logoColor=white)
![SignalR](https://img.shields.io/badge/Real--time-SignalR-512BD4)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?logo=docker&logoColor=white)

LowOnLegs is a full-stack table tennis app that brings live scoreboards, match history, and player rankings into one place. Run singles or doubles matches, follow scores as they change, and see how each game shapes the standings.

## Features

- **Live scoreboards** — real-time score updates powered by SignalR, without refreshing the page.
- **Singles & doubles** — dedicated scoreboards for one-on-one matches and team games, with serving indicators and score corrections.
- **Player profiles** — add players with nicknames and custom photos.
- **Match history** — revisit previous singles and doubles results.
- **Elo rankings** — separate singles and doubles standings, plus a comparison of different Elo K-factors.
- **Head-to-head statistics** — compare opponents, win rates, scoring averages, and winning streaks.
- **Light & dark themes** — switch themes with your preference saved between visits.

## Tech Stack

| Layer | Technologies |
| --- | --- |
| Backend | C#, .NET 9, ASP.NET Core Web API |
| Real-time communication | ASP.NET Core SignalR |
| Frontend | React 19, TypeScript, Vite |
| Styling | Tailwind CSS |
| Data fetching & routing | TanStack Query, Axios, React Router |
| Infrastructure | Docker, Docker Compose, Nginx |

## How It Works

The React frontend communicates with the ASP.NET Core backend through REST endpoints for players, matches, and statistics. SignalR pushes score changes to connected clients so the scoreboard stays up to date during play.

Nginx serves the frontend and forwards API and SignalR traffic to the backend. Docker Compose builds and runs both services together.

## Quick Start

### Prerequisites

- Docker with Docker Compose installed and running.
- On Windows: Docker Desktop configured to use Linux containers.
- Port `80` available on your machine.

### Run locally

From the repository root:

```bash
docker compose up -d --build
```

Open **[http://localhost](http://localhost)** in your browser.

The first launch downloads the required images and builds both services, so it may take a few minutes. You do not need to install Node.js or the .NET SDK separately when using Docker.

### Follow the logs

```bash
docker compose logs -f
```

### Stop the application

```bash
docker compose down
```

Match and player records are stored in a named Docker volume and retained when the containers are stopped or recreated. Uploaded player photos currently live inside the API container and are not retained when that container is recreated.

## Project Structure

```text
LowOnLegs_PingPong/
├── LowOnLegs/
│   ├── LowOnLegs/                 # API controllers, SignalR hub, application setup
│   ├── LowOnLegs.Core/            # Models, DTOs, and interfaces
│   ├── LowOnLegs.Services/        # Match logic and statistics
│   ├── LowOnLegs.Data/            # Data access
│   └── LowOnLegs.Infrastructure/  # Infrastructure components
├── LowOnLegs.React/               # React frontend and Nginx configuration
└── docker-compose.yml            # Local container setup
```
