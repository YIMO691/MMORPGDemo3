# AI MMORPG Demo

[English](README.md)

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com)
[![Unity](https://img.shields.io/badge/Unity-2022.3-FFFFFF?logo=unity)](https://unity.com)
[![Tests](https://img.shields.io/badge/tests-32%2F32-brightgreen)](.)
[![Phase](https://img.shields.io/badge/phase-1--9%20完成-blue)](.)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)

基于 Unity + C# + Lua + .NET 9.0 的服务端权威 MMORPG 竖切 Demo，适合写入简历的作品集项目。

## 游戏功能

| Phase | 功能 | 说明 |
|:-----:|------|------|
| 1 | 登录与角色 | 游客登录、角色创建（3 职业）、角色选择 |
| 2 | 实时同步 | WebSocket 连接、WASD 移动、实体快照同步 |
| 3 | 战斗系统 | 3 个技能、怪物 AI、掉落拾取、背包装备 |
| 4 | 任务与聊天 | 3 个击杀任务、场景广播聊天 |
| 5 | Lua 热更新 | MoonSharp 配置驱动，运行时可重载任务/怪物数据 |
| 6 | 资源热更新 | 远程文件清单 + 下载 + SHA256 缓存校验 |
| 7 | 世界地图 | 主城 + 野外双场景，传送门切换，摄像机跟随 |
| 8 | Docker 部署 | PostgreSQL 持久化、Redis 会话、Docker Compose 编排 |
| 9 | Android 打包 | 虚拟摇杆 + 技能按钮、APK 一键构建 |

全部游戏状态默认为内存存储（无需数据库）。服务端对战斗、掉落、背包、任务保持权威。客户端只发送意图。

## 快速开始

### 环境要求

- [.NET SDK 9.0](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Unity 2022.3 LTS](https://unity.com/releases/editor/archive)（可选 Android Build Support）
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)（Phase 8 可选）

### 启动服务端

```powershell
git clone https://github.com/YIMO691/MMORPGDemo3.git
cd MMORPGDemo3
dotnet run --project server/src/MmoDemo.Gateway/MmoDemo.Gateway.csproj
```

服务端监听 `http://localhost:5000` 和 `ws://localhost:5000/ws`。

### 启动客户端

用 Unity Hub 打开 `client/MmoDemoClient/`，打开 `Assets/_Scenes/Bootstrap.unity`，点击 Play。

### Docker 部署（Phase 8）

```powershell
docker compose -f deploy/docker-compose.yml up --build
```

自动设置 `USE_POSTGRES=true`，玩家数据重启后保留。

### 运行测试

```powershell
dotnet test server/MmoDemo.sln
# 32 个测试：HTTP API · WebSocket 认证 · 战斗流程 · 任务进度
#            聊天广播 · Lua 配置 · 资源下载 · 场景切换
```

## 架构

```
┌─ Unity 客户端 ─────────────────────┐    ┌─ .NET 服务端 ─────────────────────┐
│  GameLauncher → UIManager           │    │  ASP.NET Core Minimal API          │
│  GameManager → WebSocketClient      │←──→│  ├─ /health, /api/auth/* (HTTP)    │
│  ChatPanel · QuestTracker           │ WS │  ├─ /api/resources/*              │
│  ResourceManager · LuaManager       │    │  ├─ /api/admin/reload-config      │
│  MobileInput · CameraFollow         │    │  └─ /ws (WebSocket)               │
└─────────────────────────────────────┘    │  MessageRouter → 业务服务          │
                                           │  SceneManager → 内存实体管理       │
                                           │  AppDbContext → PostgreSQL (可选)   │
                                           └────────────────────────────────────┘
```

- 客户端发送 **意图**（`c2s.*`），服务端保持 **权威**
- 消息格式：`{"t": "<type>", "ts": <unix毫秒>, "p": <payload>}`
- C# 负责高频系统 · Lua 负责热更新配置和 UI 流程

## 项目结构

```
├── client/MmoDemoClient/   Unity 项目
│   └── Assets/
│       ├── _Scripts/        C# 游戏代码（14 个脚本）
│       ├── _Scenes/         Bootstrap.unity
│       ├── Resources/       Prefabs + Lua 脚本
│       └── Plugins/         MoonSharp.Interpreter.dll
├── server/                  .NET 9.0 解决方案
│   ├── src/
│   │   ├── MmoDemo.Gateway/       入口 + 路由注册
│   │   ├── MmoDemo.Application/   Interfaces/ + Services/ + 消息分发
│   │   ├── MmoDemo.Contracts/     HTTP 模型 + WebSocket 载荷
│   │   ├── MmoDemo.Domain/        实体模型
│   │   └── MmoDemo.Infrastructure/ 存储层（内存 + PostgreSQL）
│   └── tests/                     32 个集成测试
├── proto/                   协议设计草案
├── configs/                 Lua 配置（quests.lua, monsters.lua）
├── resources/               远程可更新资源文件
├── deploy/                  Dockerfile + docker-compose.yml
└── docs/                    架构、阶段、变更日志、构建指南
```

## API 参考

### HTTP 端点

| 方法 | 路径 | 用途 |
|------|------|------|
| GET | `/health` | 健康检查 |
| POST | `/api/auth/guest-login` | 游客登录 → playerId + token |
| POST | `/api/roles/list` | 获取角色列表 |
| POST | `/api/roles/create` | 创建角色（1-12 字符名，classId 1-3） |
| POST | `/api/roles/select` | 选择当前角色 |
| POST | `/api/scene/enter-city` | 进入主城 |
| GET | `/api/resources/manifest` | 资源文件清单 |
| GET | `/api/resources/{path}` | 下载资源文件 |
| POST | `/api/admin/reload-config` | 热重载 Lua 配置 |

### WebSocket 消息

| 类型 | 方向 | 用途 |
|------|------|------|
| `c2s.auth` / `s2c.auth_result` | ⇄ | 认证 |
| `c2s.enter_scene` / `s2c.enter_scene_result` | ⇄ | 进入/切换场景 |
| `c2s.move` / `s2c.entity_snapshot` | ⇄ | 移动同步 |
| `c2s.cast_skill` / `s2c.combat_event` | ⇄ | 战斗 |
| `c2s.pickup_item` / `s2c.drop_spawned` | ⇄ | 物品掉落 |
| `c2s.accept_quest` / `s2c.quest_updated` | ⇄ | 任务进度 |
| `c2s.chat` / `s2c.chat_broadcast` | ⇄ | 聊天消息 |

## 文档

| 文档 | 说明 |
|------|------|
| [路线图](docs/10_Roadmap.md) | 各阶段规划与进度 |
| [变更日志](docs/11_Changelog.md) | 完整变更记录 |
| [架构文档](docs/architecture/) | 设计决策与约束 |
| [阶段文档](docs/phases/) | 各阶段详细说明 |
| [设计文档](docs/design/) | UI 与流程设计 |
| [Android 构建](docs/build-android.md) | 移动端打包指南 |

## 开发约束

- 服务端权威：客户端只发送意图，不提交最终结果
- 未经审批不引入新框架
- 绝不提交密钥（`.env`、`*.local.json` 已 gitignore）
- 绝不提交构建产物（`bin/`、`obj/`、`Library/`、`Build/`、`Builds/`）
- 每次完成功能更新 [变更日志](docs/11_Changelog.md)
