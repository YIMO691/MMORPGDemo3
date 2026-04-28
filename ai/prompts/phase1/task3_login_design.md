# Codex Task: Phase 1 Task 3 Login Design

## Current Phase

Phase 1: Login / Role / City.

## Current Task

Design the login protocol and interface flow. This is a design-only task.

## Allowed To Modify

- `docs/`
- `proto/auth.proto`
- `proto/common.proto` only if a minimal shared field is required and documented.

## Forbidden To Modify

- `.env`
- `.venv/`
- `.tools/`
- `ai/providers/`
- `client/` implementation files.
- `server/` implementation files.
- Combat, inventory, quest, chat, or movement protocol files unless documenting no change is needed.

## Tech Stack Constraints

- Protocol uses Protocol Buffers.
- Server will be .NET.
- Client will be Unity C# with Lua-driven UI flow.
- Qwen is the primary AI provider when AI assistance is needed.

## Architecture Constraints

- Client sends login intent only.
- Server issues `playerId` and `token`.
- Do not implement database, Redis, full auth, or security hardening in this task.
- Do not write API keys or secrets.

## Required Output

- Review `proto/auth.proto` for Phase 1 login needs.
- Document login request and response fields.
- Document server login interface shape.
- Document client call sequence.
- Update `docs/11_Changelog.md`.

## Acceptance Criteria

- Docs include login sequence notes.
- `proto/auth.proto` satisfies Phase 1 login needs.
- No database or complex auth implementation is added.
- `git status` does not show `.env`.

## Output Report

Report:
1. Created files.
2. Updated files.
3. Proto changes, if any.
4. Validation result.
5. Next recommended task.

