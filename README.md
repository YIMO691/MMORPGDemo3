# AI MMORPG Demo

[中文版](README_CN.md)

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com)
[![Unity](https://img.shields.io/badge/Unity-2022.3-FFFFFF?logo=unity)](https://unity.com)
[![Tests](https://img.shields.io/badge/tests-32%2F32-brightgreen)](.)
[![Phase](https://img.shields.io/badge/phase-1--9%20complete-blue)](.)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)

Unity + C# + Lua + .NET 9.0 server-authoritative MMORPG vertical slice. Resume portfolio project demonstrating full-stack game development.

## Game Features

| Phase | Feature | Description |
|:-----:|---------|-------------|
| 1 | Login & Roles | Guest login, role creation (3 classes), role selection |
| 2 | Real-time Sync | WebSocket connection, WASD movement, entity snapshots |
| 3 | Combat | 3 skills (Slash/Power Strike/Fireball), monster AI, drops, inventory |
| 4 | Quest & Chat | 3 kill quests with rewards, scene-wide chat broadcast |
| 5 | Lua Hotfix | MoonSharp-powered config reloading, live quest/monster tuning |
| 6 | Resource Update | Remote file manifest + download + SHA256 cache validation |
| 7 | World Map | 2 scenes (City + Wilderness), portal travel, camera follow |
| 8 | Docker Deploy | PostgreSQL persistence, Redis sessions, Docker Compose |
| 9 | Android | Touch joystick + skill buttons, APK build pipeline |

## Quick Start

### Prerequisites

- [.NET SDK 9.0](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Unity 2022.3 LTS](https://unity.com/releases/editor/archive) (Android Build Support optional)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (optional, for Phase 8)

### Server

```powershell
# Clone & run
git clone https://github.com/YIMO691/MMORPGDemo3.git
cd MMORPGDemo3
dotnet run --project server/src/MmoDemo.Gateway/MmoDemo.Gateway.csproj
```

Server listens on `http://localhost:5000` and `ws://localhost:5000/ws`.

### Client

Open `client/MmoDemoClient/` in Unity Hub. Open `Assets/_Scenes/Bootstrap.unity`, press Play.

### Docker (Phase 8)

```powershell
docker compose -f deploy/docker-compose.yml up --build
```

Sets `USE_POSTGRES=true` automatically — player data persists across restarts.

### Tests

```powershell
dotnet test server/MmoDemo.sln
# 32 tests: HTTP API · WebSocket auth · Combat flow · Quest progress
#           Chat broadcast · Lua config · Resource download · Scene switch
```

## Architecture

```
┌─ Unity Client ─────────────────────┐    ┌─ .NET Server ─────────────────────┐
│  GameLauncher → UIManager           │    │  ASP.NET Core Minimal API          │
│  GameManager → WebSocketClient      │←──→│  ├─ /health, /api/auth/* (HTTP)    │
│  ChatPanel · QuestTracker           │ WS │  ├─ /api/resources/*              │
│  ResourceManager · LuaManager       │    │  ├─ /api/admin/reload-config      │
│  MobileInput · CameraFollow         │    │  └─ /ws (WebSocket)               │
└─────────────────────────────────────┘    │  MessageRouter → Services          │
                                           │  SceneManager → In-memory stores   │
                                           │  AppDbContext → PostgreSQL (opt)    │
                                           └────────────────────────────────────┘
```

- Client sends **intent** (`c2s.*`), server is **authoritative**
- Envelope: `{"t": "<type>", "ts": <unix_ms>, "p": <payload>}`
- C# handles high-frequency systems · Lua handles hotfix configs & UI flow

## Project Structure

```
├── client/MmoDemoClient/   Unity project
│   └── Assets/
│       ├── _Scripts/        C# game code (14 scripts)
│       ├── _Scenes/         Bootstrap.unity
│       ├── Resources/       Prefabs + Lua scripts
│       └── Plugins/         MoonSharp.Interpreter.dll
├── server/                  .NET 9.0 solution
│   ├── src/
│   │   ├── MmoDemo.Gateway/       Entry point + endpoints
│   │   ├── MmoDemo.Application/   Interfaces/ + Services/ + routing
│   │   ├── MmoDemo.Contracts/     HTTP models + WS payloads
│   │   ├── MmoDemo.Domain/        Entity models
│   │   └── MmoDemo.Infrastructure/ Stores (memory + PostgreSQL)
│   └── tests/                     32 integration tests
├── proto/                   Protocol design drafts
├── configs/                 Lua configs (quests.lua, monsters.lua)
├── resources/               Remote-updateable files
├── deploy/                  Dockerfile + docker-compose.yml
└── docs/                    Architecture, phases, changelog, build guides
```

## API Reference

### HTTP Endpoints

| Method | Path | Purpose |
|--------|------|---------|
| GET | `/health` | Health check |
| POST | `/api/auth/guest-login` | Guest login → playerId + token |
| POST | `/api/roles/list` | List roles for player |
| POST | `/api/roles/create` | Create role (name 1-12 chars, classId 1-3) |
| POST | `/api/roles/select` | Select active role |
| POST | `/api/scene/enter-city` | Enter city scene |
| GET | `/api/resources/manifest` | Resource file manifest |
| GET | `/api/resources/{path}` | Download resource file |
| POST | `/api/admin/reload-config` | Hot-reload Lua configs |

### WebSocket Messages

| Type | Direction | Purpose |
|------|-----------|---------|
| `c2s.auth` / `s2c.auth_result` | ⇄ | Authenticate |
| `c2s.enter_scene` / `s2c.enter_scene_result` | ⇄ | Enter/switch scene |
| `c2s.move` / `s2c.entity_snapshot` | ⇄ | Movement sync |
| `c2s.cast_skill` / `s2c.combat_event` | ⇄ | Combat |
| `c2s.pickup_item` / `s2c.drop_spawned` | ⇄ | Item drops |
| `c2s.accept_quest` / `s2c.quest_updated` | ⇄ | Quest progress |
| `c2s.chat` / `s2c.chat_broadcast` | ⇄ | Chat messages |

## Documentation

| Document | Description |
|----------|-------------|
| [Roadmap](docs/10_Roadmap.md) | Phase plan & progress |
| [Changelog](docs/11_Changelog.md) | Full change history |
| [Architecture](docs/architecture/) | Design decisions & constraints |
| [Phase Plans](docs/phases/) | Detailed spec per phase |
| [Design Docs](docs/design/) | UI & flow designs |
| [Android Build](docs/build-android.md) | Mobile build guide |

## Constraints

- Server-authoritative: client sends intent, never final result
- No new frameworks without approval
- Never commit secrets (`.env`, `*.local.json` are gitignored)
- Never commit build outputs (`bin/`, `obj/`, `Library/`, `Build/`, `Builds/`)
- Update [changelog](docs/11_Changelog.md) for all completed work
