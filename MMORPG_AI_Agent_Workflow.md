# MMORPG Demo：AI 分工、模型路由与 Codex 执行手册

> 适用项目：`F:\AI_MMORPG`  
> 目标：使用 Codex + Qwen/API + 可选国产/海外大模型，全流程辅助开发一个适合写进简历的小型 MMORPG Demo。  
> 当前状态：Qwen API 已跑通，OpenAI/Kimi/DeepSeek/MiMo 暂未配置也不影响继续 Phase 0。

---

## 0. 当前项目状态总结

你已经完成了 AI API 调用与模型路由环境搭建：

- 已创建 AI 调用目录：`ai/`
- 已创建 provider 配置：`ai/providers/`
- 已创建 AI 脚本：
  - `ai/scripts/check_env.py`
  - `ai/scripts/smoke_test.py`
  - `ai/scripts/ai_chat.py`
  - `ai/scripts/model_matrix.py`
  - `ai/scripts/common.py`
- 已创建 Prompt 模板：`ai/prompts/`
- 已创建 AI 使用文档：
  - `docs/08_AI_Task_Rules.md`
  - `docs/12_AI_Model_Routing.md`
  - `docs/13_AI_Setup_Guide.md`
- 已创建 `AGENTS.md`
- 已配置 `.env.example`
- 已创建本地 `.env`
- 已配置本地便携 Python 和虚拟环境：
  - `.tools/python-3.12.10`
  - `.venv`
- 已安装 `requirements-ai.txt`
- 已成功运行：
  - `.\.venv\Scripts\python.exe ai\scripts\check_env.py`
  - `.\.venv\Scripts\python.exe ai\scripts\smoke_test.py`

当前 smoke test 结果：

```text
OpenAI: skipped
Qwen: OK
Kimi: skipped
DeepSeek: skipped
MiMo: skipped / disabled
```

结论：

> Qwen 已经可用，可以继续 Phase 0。  
> 不需要等待 OpenAI/Kimi/DeepSeek/MiMo。  
> 下一步是 Git 初始化 + Phase 0 MMORPG 工程骨架。

---

## 1. 项目目标

本项目不是要做完整商业 MMORPG，而是做一个适合写进简历、能跑通完整工程闭环的小型 MMORPG 竖切 Demo。

第一版范围：

- 登录
- 选角
- 主城空界面
- 野外地图
- 玩家移动同步
- 2-3 个技能
- 怪物 AI
- 掉落
- 背包
- 任务
- 聊天
- Lua 热更新
- 资源远程更新
- Android 真机运行
- 服务端 Docker Compose 部署

第一版不做：

- 公会
- 拍卖行
- 交易行
- 大世界无缝加载
- 万人同屏
- 大规模副本
- 复杂 PVP
- 商城支付
- 真正商用级反作弊

核心原则：

> 先做完整闭环，再做功能扩展。  
> 先做工程骨架，再做玩法系统。  
> 先保证可运行、可维护、可讲清楚，再追求复杂度。

---

## 2. 推荐 AI 分工

### 2.1 总体分工

| 工具 / 模型 | 推荐角色 | 主要职责 |
|---|---|---|
| Codex | 执行器 / 工程修改代理 | 读取仓库、改文件、补代码、运行测试、生成提交说明 |
| Qwen-Coder | 主力代码模型 | Unity C#、Lua、.NET、Protobuf、Docker、CI、工具脚本 |
| ChatGPT Plus | 架构顾问 / 人工协助 | 帮你分析流程、写提示词、审查方案、总结问题 |
| OpenAI API | 可选兜底模型 | Qwen 不稳定时用于复杂代码和综合任务 |
| Kimi | 可选长上下文模型 | 长文档阅读、设计文档审查、README、总结 |
| DeepSeek | 可选推理模型 | Bug 分析、服务端逻辑、战斗公式、复杂问题定位 |
| MiMo | 可选架构/Agent 模型 | 长上下文、任务拆分、架构一致性检查 |
| 人类你自己 | Owner / 决策者 | 购买 API、登录、确认范围、审查关键变更、最终合并 |

### 2.2 具体任务分配

| 任务类型 | 首选 | 备用 |
|---|---|---|
| Unity C# 脚本 | Qwen-Coder | OpenAI / Kimi |
| xLua 桥接代码 | Qwen-Coder | Kimi |
| Lua UI / 任务模块 | Qwen-Coder | Kimi |
| .NET 服务端接口 | Qwen-Coder | DeepSeek |
| Protobuf 协议 | Qwen-Coder | OpenAI |
| Docker Compose | Qwen-Coder | DeepSeek |
| GitHub Actions / CI | Qwen-Coder | OpenAI |
| Bug 分析 | DeepSeek | Kimi / ChatGPT |
| 架构审查 | MiMo / Kimi | ChatGPT |
| 长文档总结 | Kimi / MiMo | ChatGPT |
| README / 简历包装 | Kimi / ChatGPT | Qwen |
| Codex 执行任务 | Qwen-Coder | OpenAI API |

---

## 3. 为什么现在只用 Qwen 也够

你已经验证：

```text
Qwen: OK
```

所以现在可以继续开发。OpenAI Plus 会员不能直接当 OpenAI API Key 用。Plus 适合在 ChatGPT 网页里问问题、整理文档、审查提示词；但 Codex/API 脚本需要 OpenAI API Platform 单独开通 API Key 和计费。

当前最小可用组合：

```text
Qwen-Coder + Codex + ChatGPT Plus
```

这已经足够进入：

- Git 初始化
- Phase 0 工程骨架
- 协议草案
- 配置表模板
- 工程文档
- Phase 1 登录/选角/主城设计

暂时不必强行开通 OpenAI API、Kimi、DeepSeek、MiMo。

---

## 4. Qwen API 的使用方式

### 4.1 当前 `.env` 关键配置

`F:\AI_MMORPG\.env` 中应包含：

```bash
DASHSCOPE_API_KEY=你的Qwen_API_Key
QWEN_BASE_URL=https://dashscope-intl.aliyuncs.com/compatible-mode/v1
QWEN_CODER_MODEL=qwen3-coder-plus

AI_DEFAULT_PROVIDER=qwen
AI_FALLBACK_PROVIDER=openai
```

如果你使用中国北京区域：

```bash
QWEN_BASE_URL=https://dashscope.aliyuncs.com/compatible-mode/v1
```

如果你使用中国香港区域：

```bash
QWEN_BASE_URL=https://cn-hongkong.dashscope.aliyuncs.com/compatible-mode/v1
```

### 4.2 检查 Key

```powershell
cd F:\AI_MMORPG
.\.venv\Scripts\python.exe ai\scripts\check_env.py
```

期望看到：

```text
[OK] Qwen key found: sk-****xxxx | model=qwen3-coder-plus
```

### 4.3 Smoke Test

```powershell
.\.venv\Scripts\python.exe ai\scripts\smoke_test.py
```

期望看到：

```text
[OK] Qwen: OK
```

### 4.4 直接用 Qwen 生成内容

```powershell
.\.venv\Scripts\python.exe ai\scripts\ai_chat.py --provider qwen --prompt "请生成一个 Unity C# 单例管理器模板"
```

使用系统 Prompt：

```powershell
.\.venv\Scripts\python.exe ai\scripts\ai_chat.py --provider qwen --system ai\prompts\system\unity_client_engineer.md --prompt "请为 MMORPG Demo 生成 GameLauncher.cs 的设计草案"
```

查看脚本帮助：

```powershell
.\.venv\Scripts\python.exe ai\scripts\ai_chat.py --help
```

---

## 5. 安全规则

必须遵守：

1. 不要把 `.env` 发给任何 AI。
2. 不要把 API Key 粘贴进聊天窗口。
3. 不要提交 `.env`。
4. 不要提交 `.venv/`。
5. 不要提交 `.tools/`。
6. 不要提交 `providers.local.json`。
7. 所有密钥只放在本地环境变量或 `.env`。
8. Codex 不能读取、输出、重写 `.env`。
9. 如果 Git status 里出现 `.env`，立即停止提交。
10. 任何 AI 任务都必须小范围执行。

`.gitignore` 至少应包含：

```gitignore
.env
.env.local
*.local.json
.venv/
.tools/
ai/outputs/*
!ai/outputs/.gitkeep
```

---

## 6. 现在立刻执行的步骤

### 6.1 初始化 Git

在 `F:\AI_MMORPG` 执行：

```powershell
git init
git status
```

重点检查：

```text
.env 不能出现在待提交列表里
```

如果 `.env` 没出现，继续：

```powershell
git add .
git status
git commit -m "chore: initialize AI model routing environment"
```

如果 `.env` 出现，立刻停止，并检查 `.gitignore`。

---

## 7. Phase 0：交给 Codex 的任务

Git 提交完成后，把下面整段交给 Codex。

```md
Qwen API smoke test 已通过，现在进入 MMORPG Demo Phase 0。

当前状态：
- Qwen provider 可用
- OpenAI/Kimi/DeepSeek/MiMo 暂未配置，不要依赖它们
- 不要读取、输出或提交 .env
- 不要伪造其他 provider 测试成功

请创建 MMORPG Demo 第一阶段工程骨架，只创建目录、协议草案、配置模板和文档，不写复杂业务代码。

必须遵守：
1. 不提交 .env
2. 不写 API Key
3. 不直接开始战斗、背包、任务、聊天业务代码
4. 只做 Phase 0 Foundation
5. 所有内容必须符合 AGENTS.md
6. 每次完成任务必须更新 docs/11_Changelog.md

请完成：

## 目录

创建或检查：

- client/
- server/
- proto/
- configs/
- tools/
- deploy/
- docs/
- assets_source/

## 文档

创建或更新：

- docs/00_Project_Brief.md
- docs/01_Tech_Stack.md
- docs/02_Architecture.md
- docs/03_Module_Boundary.md
- docs/04_Directory_Rules.md
- docs/05_Protocol_Rules.md
- docs/06_Config_Table_Rules.md
- docs/07_Lua_Hotfix_Rules.md
- docs/10_Roadmap.md
- docs/11_Changelog.md
- docs/14_Phase_0_Foundation.md
- docs/15_Phase_1_Login_Role_City.md

文档必须说明：
- 这是小型 MMORPG 竖切 Demo
- 客户端只上报意图
- 服务端保持权威
- C# 负责底层、高频、网络、实体、资源
- Lua 负责 UI、任务、活动、轻玩法逻辑
- 资源热更新和 Lua 热更新分离
- 第一版不做公会、拍卖行、大世界无缝、万人同屏

## 协议草案

创建或更新：

- proto/common.proto
- proto/auth.proto
- proto/role.proto
- proto/scene.proto
- proto/movement.proto
- proto/combat.proto
- proto/inventory.proto
- proto/quest.proto
- proto/chat.proto

协议命名规则：

- C2S_LoginReq
- S2C_LoginRes
- C2S_CreateRoleReq
- S2C_CreateRoleRes
- C2S_EnterSceneReq
- S2C_EnterSceneRes
- C2S_MoveReq
- S2C_EntitySnapshotNtf
- C2S_CastSkillReq
- S2C_CombatEventNtf
- C2S_UseItemReq
- S2C_InventoryChangedNtf
- C2S_AcceptQuestReq
- S2C_QuestChangedNtf
- C2S_ChatReq
- S2C_ChatNtf

只写最小协议结构，不写复杂逻辑。

## 配置模板

创建或更新：

- configs/Role.template.csv
- configs/Scene.template.csv
- configs/Monster.template.csv
- configs/Skill.template.csv
- configs/Item.template.csv
- configs/Drop.template.csv
- configs/Quest.template.csv

配置模板只需要字段和 1-2 行示例数据，不需要大量内容。

## Phase 文档

请写清楚：

### Phase 0：Foundation

目标：
- AI 调用环境可用
- Git 仓库可用
- 工程目录稳定
- 协议草案完成
- 配置表草案完成
- Unity 工程准备创建
- .NET 服务端准备创建

验收标准：
- Qwen smoke test 通过
- Git status 干净
- 文档完整
- proto 文件存在
- config 模板存在
- AGENTS.md 存在
- .env 未提交

### Phase 1：Login / Role / City

目标：
- Unity 客户端能打开登录界面
- 服务端能提供登录接口
- 客户端能拿到 playerId/token
- 客户端能进入选角界面
- 客户端能进入主城空界面
- 服务端能返回玩家基础数据

Phase 1 暂不实现：
- 战斗
- 背包
- 任务
- 聊天
- 移动同步

## 输出报告

完成后请输出：
1. 创建了哪些文件
2. 更新了哪些文件
3. Phase 0 是否完成
4. 是否有任何文件可能泄漏密钥
5. 下一步进入 Phase 1 前我需要安装哪些软件
6. 下一条 Codex 任务建议
```

---

## 8. Phase 0 完成后的提交

Codex 完成 Phase 0 后，你执行：

```powershell
git status
```

确认没有 `.env`。

然后：

```powershell
git add .
git commit -m "docs: initialize MMORPG demo foundation"
```

---

## 9. Phase 1 前需要安装的软件

Phase 0 完成后，再安装或确认：

| 软件 | 用途 | 优先级 |
|---|---|---|
| Unity Hub | 管理 Unity 编辑器 | P0 |
| Unity Editor | 客户端开发 | P0 |
| Visual Studio 2022 / Rider | C# IDE | P0 |
| .NET SDK | 服务端开发 | P0 |
| Docker Desktop | PostgreSQL/Redis/服务端部署 | P0 |
| Git | 版本管理 | P0 |
| Android Studio / Android SDK | Android 真机打包 | P1 |
| Protobuf Compiler | 协议生成 | P1 |
| Node.js | 可选工具链 | P2 |

建议安装顺序：

```text
Git
↓
Unity Hub + Unity Editor
↓
Visual Studio / Rider
↓
.NET SDK
↓
Docker Desktop
↓
Android Studio
↓
Protobuf Compiler
```

---

## 10. Phase 1 之前不要做的事

不要急着做：

- 战斗系统
- 技能系统
- 背包系统
- 任务系统
- 聊天系统
- 移动同步
- 资源热更新
- Lua 热更新
- 大量美术资源导入

Phase 1 只做：

```text
登录
↓
选角
↓
进入主城空界面
```

目标是跑通第一个最小闭环。

---

## 11. Phase 1：建议任务拆分

Phase 1 应拆成 5 个小任务。

### 任务 1：服务端空工程

目标：

- 创建 .NET solution
- 创建 Gateway/API 项目
- 创建 `/health`
- 创建基础目录
- 不接数据库

验收：

```text
dotnet run 可以启动
GET /health 返回 OK
```

### 任务 2：客户端 Unity 空工程结构

目标：

- 创建 Unity 工程
- 创建 Bootstrap 场景
- 创建 Login 空界面
- 创建 GameLauncher.cs

验收：

```text
Unity 可运行
进入 Login 界面
```

### 任务 3：登录协议和接口

目标：

- 使用 `auth.proto`
- 服务端提供登录接口
- 返回 playerId/token
- 客户端能显示 playerId

验收：

```text
点击登录后显示 playerId
服务端有日志
```

### 任务 4：选角

目标：

- 客户端显示角色列表
- 服务端返回角色数据
- 支持创建一个角色

验收：

```text
登录后进入选角界面
可以创建角色
可以选择角色
```

### 任务 5：主城空界面

目标：

- 选角后进入主城
- 显示角色名、等级、金币
- 暂无玩法

验收：

```text
登录 -> 选角 -> 主城 全流程可跑
```

---

## 12. Codex 通用任务模板

以后所有任务都用这个格式交给 Codex。

```md
# Codex 执行任务

## 当前阶段
填写 Phase，例如 Phase 1 Login / Role / City。

## 当前任务
填写具体任务。

## 只能修改
列出允许修改的目录或文件。

## 禁止修改
- .env
- .venv/
- .tools/
- ai/providers/
- 与当前任务无关的模块

## 技术栈
客户端：
- Unity
- C#
- xLua
- YooAsset/Addressables
- Luban
- Protobuf
- uGUI

服务端：
- .NET
- ASP.NET Core WebSocket
- PostgreSQL
- Redis
- Docker Compose

## 架构约束
- 客户端只上报意图
- 服务端保持权威
- C# 负责底层、高频、网络、实体、资源
- Lua 负责 UI、任务、活动、轻玩法逻辑
- 不能引入未确认依赖
- 不能扩大范围

## 需要产出
1. 代码或文档
2. 测试或验证步骤
3. 更新 docs/11_Changelog.md
4. 输出修改文件清单
5. 输出下一步建议

## 验收标准
列出可验证标准。
```

---

## 13. AGENT 规则建议

如果后续要扩展 `AGENTS.md`，可以加入以下内容。

```md
# AI Agent Rules for MMORPG Demo

## Role Split

- Qwen-Coder: primary coding model
- Codex: repository execution agent
- ChatGPT: project advisor and prompt writer
- Kimi: long-context reviewer, optional
- DeepSeek: bug and reasoning specialist, optional
- MiMo: architecture planner, optional

## Safety

- Never read or output .env
- Never commit API keys
- Never modify provider secrets
- Never introduce new frameworks without approval
- Never expand MVP scope without explicit approval

## Engineering Principles

- Client sends intent only
- Server remains authoritative
- C# handles high-frequency systems
- Lua handles hotfixable business logic
- Remote resource update and Lua hotfix are separate
- Small tasks only
- Update docs and changelog every time

## Development Order

1. AI API environment
2. Git initialization
3. Phase 0 foundation docs and skeleton
4. Phase 1 login / role / city
5. Resource and Lua hotfix foundation
6. Scene and entity system
7. Movement sync
8. Combat
9. Inventory and quest
10. Chat
11. Android build and deployment
```

---

## 14. 总结：你现在的正确路线

当前已经完成：

```text
Qwen API OK
```

现在立刻做：

```text
git init
git add .
git commit -m "chore: initialize AI model routing environment"
```

然后交给 Codex：

```text
创建 Phase 0 MMORPG 工程骨架
```

不要跳到战斗、背包、任务。  
你的第一个游戏目标是：

```text
登录
↓
选角
↓
进入主城空界面
```

你的第一个工程目标是：

```text
目录稳定
↓
文档稳定
↓
协议稳定
↓
配置模板稳定
↓
Git 可追踪
```

只要这样做，后面各种 AI 才能各司其职，不会把项目越做越乱。
