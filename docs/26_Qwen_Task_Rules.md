# Qwen Task Rules

These rules define how local Qwen scripts should generate project changes.

## Output Mode

Qwen should handle one small task at a time.

Prefer split mode:

- Generate one file per Qwen call when practical.
- Validate each file before writing.
- Write files only after validation passes.

Avoid asking Qwen to generate many files and immediately write all of them without checks.

## Recommended JSON Format

Prefer `content_lines`:

```json
{
  "path": "docs/example.md",
  "content_lines": [
    "# Title",
    "",
    "content"
  ]
}
```

Avoid raw multi-line `content` when possible because escaping Markdown and JSON can be fragile.

Avoid `content_base64`; it has caused padding and UTF-8 handling problems.

## Safety Rules

Qwen must not output or modify:

- `.env`.
- `.env.example`.
- `.venv/`.
- `.tools/`.
- `ai/providers/`.
- `providers.local.json`.
- `server/src/`, unless the task explicitly enters implementation.
- `server/tests/`, unless the task explicitly requests tests.
- `client/UnityProject/`, unless Unity project work is explicitly allowed.

Qwen must not output:

- API keys.
- Real keys beginning with common secret prefixes.
- Password values.
- Secret values.
- Real token values.

Docs may mention a `token` field name, but not real secrets.

## Validation Rules

After every Qwen generation, run:

```powershell
git status
git diff --name-only
```

Confirm that only whitelisted files changed.

Every review must check:

- Did Qwen implement business code outside the task?
- Did Qwen modify `server/src/`?
- Did Qwen modify `client/UnityProject/`?
- Did Qwen add database, Redis, or WebSocket as a current flow?
- Did Qwen break proto package or namespace rules?
- Did Qwen break Markdown code fences?
- Did Qwen introduce mojibake or encoding damage?
- Did Qwen introduce secret-like text?

## Proto Checks

Every modified proto file must satisfy:

```powershell
Select-String -Path proto/xxx.proto -Pattern "package mmorpg"
Select-String -Path proto/xxx.proto -Pattern "option csharp_namespace"
```

Do not introduce multiple proto package styles.

## Changelog Rules

Every meaningful task must update:

```text
docs/11_Changelog.md
```

Text must display correctly as UTF-8. Do not commit mojibake.

## Task 5 Special Rules

Phase 1 Task 5 is empty city screen design.

Allowed files:

- `proto/scene.proto`.
- `docs/23_City_Interface_Design.md`.
- `docs/24_City_Sequence.md`.
- `client/docs/ClientModuleBoundary.md`.
- `server/README.md`.
- `docs/11_Changelog.md`.

Forbidden files and directories:

- `server/src/`.
- `server/tests/`.
- `client/UnityProject/`.
- `configs/`.
- `deploy/`.
- `ai/providers/`.
- `.env`.

Task 5 may design:

- Empty city interface.
- Empty city entry sequence.
- City module responsibilities.
- Empty city display fields.

Task 5 must not design current-phase:

- WebSocket.
- Scene entity sync.
- Movement sync.
- Combat.
- Inventory.
- Quest.
- Chat.

