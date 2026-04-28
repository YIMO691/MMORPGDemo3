# Codex Task: Phase 1 Task 2 Unity Skeleton Planning

## Current Phase

Phase 1: Login / Role / City.

## Current Task

Prepare Unity client skeleton planning. Do not create a real Unity project unless Unity is installed and the user explicitly confirms.

## Allowed To Modify

- `client/`
- `docs/`
- `ai/prompts/phase1/`

## Forbidden To Modify

- `.env`
- `.venv/`
- `.tools/`
- `ai/providers/`
- `server/`
- Generated Unity folders such as `Library/`, `Temp/`, `Obj/`, `Logs/`, and build output directories.

## Tech Stack Constraints

- Client uses Unity, C#, Lua, Protobuf, and uGUI.
- Do not introduce new frameworks without approval.
- Do not import art or large assets.
- Qwen is the primary AI provider when AI assistance is needed.

## Architecture Constraints

- Client is responsible for rendering, UI, input, presentation, resource loading, and network message sending.
- Client sends intent only.
- C# owns low-level, high-frequency, network, entity, and resource logic.
- Lua owns UI flow, quests, activities, tutorial, and lightweight hotfix logic.

## Required Output

- Add or update `client/README.md` with Unity project creation instructions.
- Add docs describing Bootstrap, Login, RoleSelect, and City directory design.
- Clearly define GameLauncher, LoginView, RoleSelectView, and CityView responsibilities.
- Update `docs/11_Changelog.md`.

## Acceptance Criteria

- No real Unity project files are created without explicit user confirmation.
- Client directory plan is clear.
- `git status` does not show `.env`.
- No secret-like values are introduced.

## Output Report

Report:
1. Created files.
2. Updated files.
3. Validation result.
4. Whether Unity installation is still required.
5. Next recommended task.

