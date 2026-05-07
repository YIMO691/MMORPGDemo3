# File Organization Convention

This document describes where new files should live in `AI_MMORPG`.

## Root Directory

The repository root should stay small and contain only cross-tool entry points and top-level project folders.

Allowed root files:

- `README.md`
- `AGENTS.md`
- `CLAUDE.md`
- `.gitignore`
- `.gitattributes`
- `.editorconfig`
- `.env.example`
- `SECURITY.md`
- `CONTRIBUTING.md`

Allowed root directories:

- `.github/`
- `.claude/`
- `client/`
- `server/`
- `proto/`
- `docs/`
- `configs/`
- `assets_source/`
- `deploy/`
- `tools/`

Allowed local-only ignored directories:

- `.venv/`
- `.tools/`

Root-level Unity folders such as `Assets/`, `Library/`, `Packages/`, `ProjectSettings/`, `Logs/`, and `UserSettings/` are treated as accidental local output and are ignored.

## Project Areas

### `client/MmoDemoClient/`

Unity client project. Open this folder in Unity Hub, not the repository root.

Keep Unity-generated folders out of Git:

- `Library/`
- `Temp/`
- `Obj/`
- `Logs/`
- `Build/`
- `Builds/`
- `UserSettings/`

### `server/`

.NET 9.0 server solution.

```text
server/
  MmoDemo.sln
  src/
  tests/
```

Do not commit `bin/` or `obj/`.

### `proto/`

Protocol design drafts. Proto naming follows `AGENTS.md`.

### `docs/`

Project documentation.

```text
docs/
  README.md
  10_Roadmap.md
  11_Changelog.md
  architecture/
  design/
  phases/
  reference/
  templates/
```

Update `docs/11_Changelog.md` for completed tasks.

### `.github/`

GitHub community files and templates:

```text
.github/
  CODEOWNERS
  PULL_REQUEST_TEMPLATE.md
  ISSUE_TEMPLATE/
```

Do not add CI workflows without first writing a short design note and getting approval, because project rules currently list CI/CD as out of scope.

### `.claude/`

Claude/Codex support files. Project skills under `.claude/skills/` may be tracked. Local private files such as `.claude/settings.local.json` and `.claude/worktrees/` are ignored.

## New File Decision Guide

- Server C# code: `server/src/`
- Server tests: `server/tests/`
- Unity scripts or assets: `client/MmoDemoClient/Assets/`
- Protocol drafts: `proto/`
- Architecture or feature docs: `docs/architecture/`, `docs/design/`, or `docs/phases/`
- Workflow docs: `docs/workflows/`
- Reusable templates: `docs/templates/`
- GitHub templates: `.github/`
- Local automation scripts: `tools/`
- Local secrets or machine config: `.env` or `*.local.json`, never committed
