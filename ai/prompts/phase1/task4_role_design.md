# Codex Task: Phase 1 Task 4 Role Selection Design

## Current Phase

Phase 1: Login / Role / City.

## Current Task

Design role list, create-role, and select-role flows. This is a design-only task.

## Allowed To Modify

- `docs/`
- `proto/role.proto`
- `proto/common.proto` only if the shared role summary needs a minimal Phase 1 adjustment.

## Forbidden To Modify

- `.env`
- `.venv/`
- `.tools/`
- `ai/providers/`
- `client/` implementation files.
- `server/` implementation files.
- Combat, inventory, quest, chat, or movement files.

## Tech Stack Constraints

- Protocol uses Protocol Buffers.
- Server will be .NET.
- Client will be Unity C# with Lua-driven UI flow.
- Qwen is the primary AI provider when AI assistance is needed.

## Architecture Constraints

- Client requests role list, creation, and selection as intent.
- Server owns final role data.
- Minimum role fields are `roleId`, `name`, `level`, `classId`, and `sceneId`.
- Do not implement database or full role persistence in this task.

## Required Output

- Review `proto/role.proto`.
- Document role list, create-role, and select-role sequences.
- Document client UI states.
- Document server response data shape.
- Update `docs/11_Changelog.md`.

## Acceptance Criteria

- Docs include role selection sequence notes.
- `proto/role.proto` satisfies Phase 1 role selection needs.
- Minimum role fields are clear.
- `git status` does not show `.env`.

## Output Report

Report:
1. Created files.
2. Updated files.
3. Proto changes, if any.
4. Validation result.
5. Next recommended task.

