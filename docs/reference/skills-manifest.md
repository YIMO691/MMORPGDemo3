# Skills Manifest

This repository has a project-specific local `github-repo-standards` skill adapted for `AI_MMORPG`.

## Local Skills

### `github-repo-standards`

Location:

```text
.claude/skills/github-repo-standards/SKILL.md
```

Purpose:

- Check GitHub community health files.
- Report missing or stale repository standard files.
- Keep root Unity-generated folders out of Git and out of the workspace root.
- Use templates from `docs/templates/github/`.
- Help maintain `README.md`, `.gitignore`, `.gitattributes`, `.editorconfig`, `SECURITY.md`, `CONTRIBUTING.md`, `.github/CODEOWNERS`, PR templates, issue templates, `docs/README.md`, and `docs/reference/file-organization-convention.md`.

Trigger examples:

```text
检查仓库规范
使用 github-repo-standards 检查并补齐这个仓库的 GitHub 标准文件
```

## Template Files

```text
docs/templates/github/SECURITY.md
docs/templates/github/CODEOWNERS
docs/templates/github/CONTRIBUTING.md
```

These templates intentionally keep placeholder fields so they can be reused for other repositories.

## Project Rule

Do not add or change `.github/workflows/*.yml` without explicit approval. CI/CD changes require a short design note first.
