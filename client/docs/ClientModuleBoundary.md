# Client Module Boundary

The client is a presentation and intent-sending application. The server owns authoritative gameplay results.

## C# Responsibilities

- Unity bootstrap and runtime lifecycle.
- Network connection and message send/receive.
- Entity presentation model.
- Resource loading.
- UI framework and view binding.
- High-frequency runtime code.
- Lua VM bootstrap and script loading.

## Lua Responsibilities

- UI flow orchestration.
- Tutorial and lightweight activity flow.
- Quest presentation flow in later phases.
- Hotfixable lightweight behavior.

Lua must call C# APIs for networking and resource access. Lua must not bypass server authority.

## UI Responsibilities

- Render screen state.
- Collect player input.
- Send intent through controllers and network client.
- Display server-returned state.

UI must not calculate final rewards, inventory state, combat damage, or quest completion.

## Network Responsibilities

- Serialize client intent messages.
- Send requests to the server.
- Dispatch server responses and notifications.
- Keep protocol naming aligned with `proto/`.

Network must not mutate authoritative gameplay state without server response.

## Login Module Responsibilities

- Collect device/platform information from the system.
- Send login request to the server via HTTP API.
- Handle login response and store player ID and token.
- Transition to Role Selection screen upon successful authentication.
- Do not create WebSocket, Redis, database, JWT, or complex authentication logic in Phase 1 Task 3.

## Feature Communication

- Feature modules communicate through controller APIs and shared events.
- Direct cross-feature object references should be avoided.
- Shared low-level services live in `Framework/`.
- Phase 1 flow order is `Login -> RoleSelect -> City`.
