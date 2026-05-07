---
name: github-repo-standards
description: 检查并整理 AI_MMORPG 仓库的 GitHub 社区标准文件、根目录结构、Unity/.NET 忽略规则和文档索引；禁止在未获批准时添加 CI/CD workflow。
trigger: manual
---

# AI_MMORPG GitHub 仓库规范检查器

## 适用项目

本 skill 只针对 `F:\AI_MMORPG`：

- Unity 客户端项目位于 `client/MmoDemoClient/`
- .NET 9.0 服务端位于 `server/`
- 协议草案位于 `proto/`
- 项目文档位于 `docs/`
- 当前没有数据库、Redis、Docker 部署或 CI/CD

## 触发语句

用户说以下类似请求时使用：

- `检查仓库规范`
- `规范仓库`
- `仓库健康检查`
- `补齐社区标准`
- `整理 GitHub 仓库`
- `使用 github-repo-standards`

## 必查文件

检查这些文件或目录是否存在、非空、且内容适配 AI_MMORPG：

| 路径 | 用途 |
| --- | --- |
| `README.md` | GitHub 首页和快速开始 |
| `AGENTS.md` | Codex/AI 代理项目规则 |
| `CLAUDE.md` | Claude Code 项目规则 |
| `.gitignore` | 忽略 .NET、Unity、密钥和本地工具输出 |
| `.gitattributes` | 换行与二进制资源规则 |
| `.editorconfig` | 跨编辑器格式规则 |
| `SECURITY.md` | 安全报告流程 |
| `CONTRIBUTING.md` | 贡献和 PR 前检查 |
| `.github/CODEOWNERS` | 仓库所有权 |
| `.github/PULL_REQUEST_TEMPLATE.md` | PR 模板 |
| `.github/ISSUE_TEMPLATE/` | Issue 模板 |
| `docs/README.md` | 文档索引 |
| `docs/reference/file-organization-convention.md` | 文件组织规则 |
| `docs/reference/skills-manifest.md` | 本地 skill 清单 |

## 根目录整理规则

根目录应只保留入口文件、顶层源码目录和平台配置。

允许的项目目录：

- `.claude/`
- `.github/`
- `assets_source/`
- `client/`
- `configs/`
- `deploy/`
- `docs/`
- `proto/`
- `server/`
- `tools/`

允许存在但必须被 Git 忽略的本地目录：

- `.venv/`
- `.tools/`

如果根目录出现这些 Unity 生成目录，先确认未被 Git 跟踪且被 `.gitignore` 命中，再清理：

- `Assets/`
- `Library/`
- `Logs/`
- `Packages/`
- `ProjectSettings/`
- `UserSettings/`
- `Temp/`
- `Obj/`
- `Build/`
- `Builds/`

Unity 正确打开目录是：

```text
client/MmoDemoClient
```

不要把 `F:\AI_MMORPG` 根目录当 Unity 项目打开。

## 补齐模板

模板目录：

```text
docs/templates/github/
```

当前模板：

| 模板 | 目标文件 |
| --- | --- |
| `SECURITY.md` | `SECURITY.md` |
| `CONTRIBUTING.md` | `CONTRIBUTING.md` |
| `CODEOWNERS` | `.github/CODEOWNERS` |

应用模板时必须替换占位符，例如：

- `{GITHUB_USERNAME}` -> `YIMO691`
- `{CONTACT_EMAIL_PLACEHOLDER}` -> 当前项目维护邮箱
- `{PROJECT_DESCRIPTION_PLACEHOLDER}` -> Unity + C# + Lua + .NET 9.0 MMORPG Demo

## CI/CD 边界

不要自动创建或复制 `.github/workflows/*.yml`。

项目规则明确要求：修改 CI/CD、Docker、部署形态前，必须先写简短设计说明并等待用户确认。

## 执行流程

1. 扫描必查文件、根目录、`.github/`、`docs/`、`.gitignore` 命中情况。
2. 报告缺失、空文件、旧项目残留文案、错误 Unity 根目录输出。
3. 补齐或修正普通社区标准文件。
4. 清理未跟踪且已忽略的根目录 Unity 生成物。
5. 更新 `docs/11_Changelog.md`。
6. 如改动了 server 命令或可执行示例，运行 `dotnet test server/MmoDemo.sln`；纯文档整理可不跑，但建议作为回归确认。
7. 不要 commit，除非用户明确要求。

## 验证命令

```powershell
git status --short --untracked-files=all
git diff --check
git check-ignore -v Assets Library Logs Packages ProjectSettings UserSettings
dotnet test server/MmoDemo.sln
```
