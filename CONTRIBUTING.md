# Contributing

This repository is a Unity + C# + Lua + .NET MMORPG demo. Keep changes small, scoped, and aligned with `AGENTS.md` and `CLAUDE.md`.

## Getting Started

1. Read `AGENTS.md` and `CLAUDE.md` for project rules.
2. Review `docs/10_Roadmap.md` and `docs/11_Changelog.md`.
3. For server work, use `dotnet test server/MmoDemo.sln` when feasible.

## Branch Naming

- AI-agent work: `codex/<short-task-name>`
- Manual feature work: `feature/<short-task-name>`
- Fixes: `fix/<short-task-name>`

## Commit Style

- Use English for commit messages.
- Describe the change clearly and avoid mixing unrelated work.
- Do not commit generated folders or build outputs.

## Before Submitting a PR

- [ ] The change is scoped to one task or feature.
- [ ] Relevant docs or `docs/11_Changelog.md` are updated.
- [ ] Server tests pass when server code changed.
- [ ] No API key, token, password, `.env`, or `*.local.json` is included.
- [ ] Unity generated folders are not staged.

## Code Conventions

- C# private fields use `_camelCase`; types and methods use PascalCase.
- Lua files use `lowercase_snake_case`.
- Server namespaces use `MmoDemo.*`.
- Proto files use `package mmorpg`.
