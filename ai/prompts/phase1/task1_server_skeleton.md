# Codex Task: Phase 1 Task 1 Server Skeleton

## Current Phase

Phase 1: Login / Role / City.

## Current Task

Create the server skeleton only.

## Allowed To Modify

- `server/`
- `docs/11_Changelog.md`

## Forbidden To Modify

- `.env`
- `.venv/`
- `.tools/`
- `ai/providers/`
- `client/`
- `proto/` unless a blocking issue is found and explained first.
- Unrelated docs or AI routing files.

## Tech Stack Constraints

- Server uses .NET.
- Do not add PostgreSQL, Redis, Docker Compose, or new frameworks in this task.
- Do not depend on OpenAI, Kimi, DeepSeek, or MiMo.
- Qwen is the primary AI provider when AI assistance is needed.

## Architecture Constraints

- Server remains authoritative.
- Client sends intent only.
- This task must not implement login, role, combat, inventory, quest, chat, or movement sync business logic.
- Do not write API keys or secrets.

## Required Output

- Create a .NET solution under `server/`.
- Create a Gateway/API project.
- Add `GET /health` returning `OK`.
- Add `server/README.md` with run instructions.
- Update `docs/11_Changelog.md`.

## Acceptance Criteria

- `dotnet run` starts the server.
- `GET /health` returns `OK`.
- `git status` does not show `.env`.
- No secret-like values are introduced.

## Output Report

Report:
1. Created files.
2. Updated files.
3. Commands run.
4. Validation result.
5. Any risks or follow-up work.

