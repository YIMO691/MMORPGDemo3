# Codex Task: Phase 1 Task 5 Empty City Design

## Current Phase

Phase 1: Login / Role / City.

## Current Task

Design the empty city entry flow and UI fields. This is a design-only task.

## Allowed To Modify

- `docs/`
- `proto/scene.proto`
- `proto/common.proto` only if a minimal shared field is required and documented.

## Forbidden To Modify

- `.env`
- `.venv/`
- `.tools/`
- `ai/providers/`
- `client/` implementation files.
- `server/` implementation files.
- Combat, inventory, quest, chat, or movement implementation.

## Tech Stack Constraints

- Protocol uses Protocol Buffers.
- Server will be .NET.
- Client will be Unity C# with Lua-driven UI flow.
- Qwen is the primary AI provider when AI assistance is needed.

## Architecture Constraints

- Client sends enter-scene intent only.
- Server returns authoritative scene entry data.
- City UI is limited to role name, level, and gold for Phase 1.
- Do not implement movement, combat, NPCs, quests, or loot.

## Required Output

- Review `proto/scene.proto`.
- Document city entry sequence.
- Document empty city UI display fields.
- Confirm protocol support for entering the starting city.
- Update `docs/11_Changelog.md`.

## Acceptance Criteria

- Docs include city entry flow notes.
- `proto/scene.proto` satisfies empty city entry needs.
- City UI scope is limited to role name, level, and gold.
- `git status` does not show `.env`.

## Output Report

Report:
1. Created files.
2. Updated files.
3. Proto changes, if any.
4. Validation result.
5. Next recommended task.

