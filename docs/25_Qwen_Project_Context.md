# Qwen Project Context: Unity MMORPG Demo

This document is the stable project context that must be provided to Qwen before generating new phase documents or code.

## Project Positioning

This project is a small Unity MMORPG vertical slice for a resume portfolio. It is not a full commercial MMORPG.

The long-term demo path is:

```text
Login
-> Role selection
-> Empty city screen
-> World map
-> Movement sync
-> Combat
-> Loot
-> Inventory
-> Quest
-> Chat
-> Lua hotfix
-> Remote resource update
-> Android device run
-> Server Docker Compose deployment
```

The current phase is Phase 1 only.

Phase 1 scope:

```text
Login
-> Role selection
-> Empty city screen
```

Phase 1 does not include:

- Combat.
- Inventory.
- Quest.
- Chat.
- Movement sync.
- Entity sync.
- WebSocket.
- Database.
- Redis.
- Docker deployment.
- Real Unity UI implementation.
- Resource hot update.
- Lua hotfix.

## Current Progress

Completed:

- Phase 0: foundation skeleton.
- Phase 1 preparation documents.
- Phase 1 Task 1: server skeleton with `/health`.
- Phase 1 Task 2: Unity client skeleton planning.
- Phase 1 Task 3: login protocol and interface design.
- Phase 1 Task 4: role selection flow design, reviewed and fixed.

Next:

- Phase 1 Task 5: empty city screen design.

Task 5 is still design-only. It must not implement real server business code or real Unity UI.

## Tech Stack

Client:

- Unity.
- C#.
- xLua.
- YooAsset or Addressables, later phase.
- Luban, later phase.
- Protobuf.
- uGUI.
- Input System.

Server:

- .NET.
- ASP.NET Core Minimal API.
- WebSocket reserved for later phases only.
- PostgreSQL later.
- Redis later.
- Docker Compose later.

Protocol:

- Protobuf.
- Current phase designs `.proto` files only.
- No generated protocol code yet.

AI role split:

- Qwen-Coder: primary model for code, protocol, and document drafts.
- Codex: repository execution and review agent when used.
- ChatGPT: architecture advice, prompt writing, and human review support.
- MiMo, Kimi, and DeepSeek: optional later, not current dependencies.

## Global Engineering Rules

1. Client owns rendering, input, UI, local presentation, resource loading, and network request sending.
2. Server remains authoritative.
3. Client only sends intent. It must not decide damage, loot, rewards, quest progress, or role state.
4. C# owns low-level, high-frequency, network, entity, resource, and platform logic.
5. Lua owns UI flow, quest flow, activity flow, tutorial, and lightweight hotfix logic.
6. Resource hot update and Lua hotfix are separate.
7. Every task must stay small and scoped.
8. Every task must use an explicit file whitelist.
9. Every meaningful change must update `docs/11_Changelog.md`.
10. Do not expand the current phase scope.

## Proto Rules

All current project proto files must use:

```proto
syntax = "proto3";

package mmorpg;

option csharp_namespace = "AIMMO.Proto";
```

Do not introduce package variants such as:

```proto
package mmo.role;
package mmo.scene;
package auth;
```

Field names use `snake_case`.

Message names use:

- `C2S_xxxReq`.
- `S2C_xxxRes`.
- `S2C_xxxNtf`.

Current Phase 1 proto files:

- `proto/auth.proto`.
- `proto/role.proto`.
- `proto/scene.proto`.

Use `int32 scene_id`.
Use `string player_id` and `string role_id` for Phase 1 identity strings.

## Interface Design Rules

Phase 1 uses HTTP interface design only. Do not design WebSocket flows in Phase 1.

Login:

```text
POST /api/auth/guest-login
```

Role selection:

```text
POST /api/roles/list
POST /api/roles/create
POST /api/roles/select
```

Empty city:

```text
POST /api/scene/enter-city
```

Do not design GET requests with a request body.

For Phase 1 design, `token` may stay in the request body. A later phase may move it to an `Authorization` header and derive `playerId` from token/session state.

Do not design complex JWT/auth flows in Phase 1.

## Document Rules

All Markdown code fences must use triple backticks.

Documents must clearly state:

- The current task is design-only when it is design-only.
- Server business code is not implemented.
- Real Unity UI is not implemented.
- Database is not connected.
- Redis is not connected.

Do not claim that login, role selection, or city interfaces are implemented if they are design-only.

## Forbidden Current-Phase Content

Before and during Phase 1 Task 5, do not introduce:

- WebSocket.
- Entity sync.
- Movement sync.
- Combat.
- Inventory.
- Quest.
- Chat.
- NPCs.
- Monsters.
- Loot.
- Database.
- Redis.
- Docker.
- Complex authentication.
- JWT.
- CDN.
- Audit systems.
- Rate limiting.
- Store/payment systems.

If one of these must be mentioned, it can only appear in an "out of scope" or "later phase" section, not as a current flow step.

## Phase 1 Task 5 Goal

Task 5 is empty city screen design.

Allowed:

- Review and refine `proto/scene.proto`.
- Create empty city interface design documentation.
- Create empty city entry sequence documentation.
- Add City module responsibility notes to the client module boundary.
- Add planned city interface notes to `server/README.md`.
- Update changelog.

Not allowed:

- Do not implement server code.
- Do not create Unity UI.
- Do not write real scene loading.
- Do not write WebSocket flow.
- Do not write entity sync.
- Do not write movement sync.

The empty city screen displays only:

- Role name.
- Level.
- Gold.

The flow can go only as far as:

```text
Role selection succeeds
-> Prepare to enter City flow
-> Request empty city screen data
-> Client prepares to display empty City screen
```

Do not write "load real Unity city scene".
Do not write "initialize entity sync".
Do not write "connect WebSocket".

