# AI MMORPG Demo

Unity + C# + Lua + .NET 9.0 MMORPG vertical-slice demo for a resume portfolio.

The current implementation includes guest login, role selection, city entry, WebSocket movement, monsters, combat, drops, inventory, quest progress, and scene chat. Data is stored in memory for demo purposes.

## Repository Layout

```text
client/MmoDemoClient/  Unity client project
server/                .NET 9.0 ASP.NET Core gateway and tests
proto/                 Protocol design drafts
docs/                  Roadmap, architecture, design notes, changelog
configs/               Configuration examples
assets_source/         Source art or asset references
tools/                 Local project tooling
```

## Requirements

- Unity Hub with a Unity editor version compatible with `client/MmoDemoClient`
- .NET SDK 9.0 or newer SDK capable of targeting `net9.0`
- PowerShell on Windows

## Server

From the repository root:

```powershell
dotnet build server/MmoDemo.sln
dotnet test server/MmoDemo.sln
dotnet run --project server/src/MmoDemo.Gateway/MmoDemo.Gateway.csproj
```

The gateway listens on:

```text
http://localhost:5000
ws://localhost:5000/ws
```

## Unity Client

Open this folder in Unity Hub:

```text
F:\AI_MMORPG\client\MmoDemoClient
```

Do not open the repository root as a Unity project. The root-level `Assets/`, `Library/`, `Packages/`, `ProjectSettings/`, and `UserSettings/` folders are ignored as accidental Unity-generated output.

## Configuration

Copy `.env.example` to `.env` for local AI provider settings. Never commit `.env`, API keys, tokens, or `*.local.json` files.

## Documentation

- [Project roadmap](docs/10_Roadmap.md)
- [Changelog](docs/11_Changelog.md)
- [Phase docs](docs/phases/)
- [Design docs](docs/design/)
- [Architecture docs](docs/architecture/)

## Current Non-Goals

- Database persistence
- Redis
- Docker deployment
- CI/CD workflows
- Production authentication
