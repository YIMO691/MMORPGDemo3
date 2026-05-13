# 面试准备：AI MMORPG Demo 项目复习指南

## 1. 项目一句话介绍

> 从零构建的服务端权威 MMORPG 竖切 Demo，Unity C# 客户端 + .NET 9.0 服务端，涵盖登录、战斗、任务、聊天、Lua 热更新、资源热更新、多场景、Docker 部署、Android 打包 9 个阶段，32 个集成测试全覆盖。

---

## 2. 架构核心（面试官必问）

### 2.1 为什么选「服务端权威」架构？

**问题：** 为什么不让客户端计算伤害、掉落、任务奖励？

**回答：**
- 防作弊：客户端可被修改，服务端不可信
- 客户端只发送**意图**（`c2s.cast_skill`），服务端验证并计算**结果**（`s2c.combat_event`）
- 举例：客户端请求释放技能 → 服务端检查距离、计算伤害、判断暴击 → 广播结果给场景所有人

### 2.2 消息协议设计

**问题：** WebSocket 消息格式是什么样的？

**回答：**
```json
{"t": "c2s.cast_skill", "ts": 1715123456789, "p": {"targetId": "monster_1", "skillId": 1}}
```

- `t`：消息类型，统一 `c2s.*` / `s2c.*` 命名
- `ts`：Unix 毫秒时间戳
- `p`：强类型 payload，服务端用 `System.Text.Json` 反序列化

**为什么不用 Protobuf？** Demo 阶段优先可读性和调试效率，JSON 足够。Proto 文件在 `proto/` 作为设计草案。

### 2.3 服务端分层架构

**问题：** 服务端怎么组织的？

**回答：**
```
Gateway (Program.cs)     ← 入口，DI 注册，路由
  ↓
Application              ← 业务逻辑
  ├─ Interfaces/         12 个接口
  ├─ Services/           11 个服务实现
  ├─ MessageRouter.cs    消息分发（switch by type）
  ├─ WebSocketHandler.cs 连接生命周期
  └─ SceneManager.cs     实体/场景/广播管理
  ↓
Contracts                ← HTTP Models + WS Payloads
Domain                   ← Entity, PlayerEntity, Monster, Quest...
Infrastructure           ← 存储层（内存 + PostgreSQL）
```

**关键设计决策：**
- 接口与实现分离（`Interfaces/` + `Services/`），方便替换存储后端
- `MessageRouter` 用 `switch` 分发消息类型，简单高效
- `SceneManager` 管理所有连接、实体、场景成员关系

---

## 3. 各 Phase 技术亮点

### Phase 1-2：登录与实时通信

**亮点：** 从零搭建 .NET Minimal API + WebSocket

**面试可能会问：**
- WebSocket 连接管理怎么做？→ `WebSocketHandler` 维护接收循环，`SceneManager` 用 `ConcurrentDictionary` 管理连接映射
- 怎么处理断线？→ `finally` 块中广播 `entity_left` 并清理资源
- 移动同步怎么做？→ 客户端每帧发送方向+位置，服务端校验边界并广播 `entity_snapshot`

### Phase 3：战斗系统

**亮点：** 服务端权威的回合式即时战斗

**面试可能会问：**
- 伤害公式？→ `(caster.Level × 5 × skill.mult) - (target.Def × 0.5)`，最小 1 点
- 暴击怎么算？→ 15% 概率，1.5x 伤害
- 怪物 AI？→ 状态机：Idle → Patrol → Chase → Attack → Return
- 掉落怎么生成？→ 查表（`DropTables`）+ 随机数判定

### Phase 4：任务与聊天

**亮点：** 任务进度钩入战斗流程

**面试可能会问：**
- 任务怎么跟踪进度？→ `OnMonsterKilled` 钩子，比对任务目标怪物类型
- 聊天怎么广播？→ `SceneManager.Broadcast(sceneId, excludeCid, msg)`

### Phase 5：Lua 热更新

**亮点：** 不改 C# 代码也能调整游戏配置

**面试可能会问：**
- 为什么选 MoonSharp 而不是 xLua？→ MoonSharp 纯 C# 实现，NuGet 一键安装，无 native 依赖
- 热更新流程？→ 改 `quests.lua` → `POST /api/admin/reload-config` → 新值立即生效
- Lua 不能做什么？→ 不能决定战斗伤害/掉落/任务奖励（服务端权威原则）

### Phase 6：资源热更新

**亮点：** 客户端启动时自动检查并下载更新资源

**面试可能会问：**
- 怎么判断文件需要更新？→ SHA256 hash 比对，manifest 中有文件名+hash+大小
- 怎么防止目录遍历攻击？→ `Path.GetFullPath` 校验，拒绝 `../` 路径

### Phase 7：多场景

**亮点：** 场景切换 + 实体生命周期管理

**面试可能会问：**
- 场景切换流程？→ 离开旧场景（广播 entity_left）→ 进入新场景（设置出生点）→ 加载新实体
- 客户端怎么处理？→ 销毁所有旧 GameObject → 从 enter_scene_result 重建

### Phase 8：Docker 部署

**亮点：** 容器化 + 持久化存储

**面试可能会问：**
- 为什么用 PostgreSQL？→ EF Core + Npgsql，Code First 自动建表
- 怎么做到开发/生产兼容？→ 环境变量 `USE_POSTGRES` 切换，默认内存存储

### Phase 9：Android 打包

**亮点：** 触屏输入适配 + 构建管线

**面试可能会问：**
- 移动端输入怎么处理？→ `MobileInput` 组件，虚拟摇杆 + 技能按钮，`Application.isMobilePlatform` 自动切换

---

## 4. 技术栈全景

| 层 | 技术 | 用途 |
|----|------|------|
| 客户端引擎 | Unity 2022.3 | 渲染、输入、UI |
| 客户端语言 | C# | 游戏逻辑、网络、实体 |
| 客户端脚本 | Lua (MoonSharp) | 热更新配置、UI 流程 |
| 服务端框架 | ASP.NET Core Minimal API | HTTP + WebSocket |
| 服务端语言 | C# (.NET 9.0) | 业务逻辑、消息路由 |
| 数据库 | PostgreSQL + EF Core | 持久化（可选） |
| 缓存 | Redis + StackExchange.Redis | 会话管理（可选） |
| 容器化 | Docker + Docker Compose | 部署编排 |
| 协议 | JSON over WebSocket | 实时通信 |
| 测试 | xUnit + WebApplicationFactory | 集成测试 |

---

## 5. 常见面试追问

### 5.1 "如果同时有 1000 个玩家在线，你的架构有什么问题？"

1. **内存存储不可扩展** → 需要 PostgreSQL 持久化 + Redis 缓存
2. **SceneManager 单实例** → 需要按场景分片，或引入消息队列
3. **广播风暴** → 需要 AOI（Area of Interest），只同步视野内实体
4. **WebSocket 连接数** → 需要网关层（Kestrel 默认有连接限制）

### 5.2 "你怎么保证移动同步不被作弊？"

- 服务端校验速度：`pos = oldPos + dir × speed × dt`，客户端位置仅作参考
- 边界检查：`BoundsMin/Max` 限制移动范围
- 未来可加：速度异常检测、位置回滚

### 5.3 "热更新和资源更新有什么区别？"

- **Lua 热更新**：更新脚本逻辑（改任务奖励、调整 UI 流程），不改 C# 代码
- **资源热更新**：更新 AssetBundle/贴图/配置（换皮肤、加新道具）
- 两套独立管线，版本号分开管理

### 5.4 "为什么不用微服务架构？"

- Demo 项目不需要微服务的复杂度
- 单体应用 + 清晰的分层架构足够演示工程能力
- 面试时表达：理解微服务利弊，知道什么场景该用什么架构

### 5.5 "测试覆盖率多少？怎么做测试？"

- 32 个集成测试，覆盖所有 API 端点和 WebSocket 流程
- xUnit + `WebApplicationFactory<Program>`（ASP.NET Core 内置集成测试框架）
- 测试模式：HTTP 创建账户 → WebSocket 连接 → 模拟游戏流程 → 断言结果
- 不需要 Mock，直接在内存中启动完整服务端

---

## 6. 复习路线（建议 3 天）

### Day 1：架构和服务端
1. 读 `README.md` 和 `README_CN.md`
2. 理解 `Program.cs` 的 DI 注册和路由
3. 跟一遍 `MessageRouter.cs` 的消息分发流程
4. 跑一遍 `dotnet test`，读懂关键测试

### Day 2：客户端和网络
1. 理解 `GameLauncher → UIManager → CityView → GameManager` 启动流程
2. 跟一遍 `WebSocketClient` 的收发循环
3. 理解 `GameManager.OnWsMessage` 的消息处理
4. Unity 中 Play，实测所有功能

### Day 3：高级特性
1. 理解 Lua 热更新流程（改 `quests.lua` → reload）
2. 理解资源更新流程（manifest → download → cache）
3. 理解场景切换流程（portal → leave → enter）
4. 理解 Docker Compose 编排和 PostgreSQL 持久化

---

## 7. 项目数字速查

| 指标 | 数值 |
|------|------|
| 服务端 C# 文件 | ~45 个 |
| 客户端 C# 脚本 | 14 个 |
| 接口数 | 12 个 |
| 服务实现 | 11 个 |
| HTTP 端点 | 8 个 |
| WebSocket 消息类型 | 14 种 |
| 测试 | 32 个（全覆盖） |
| 怪物种类 | 3 种（Slime/Goblin/Wolf） |
| 技能数 | 3 个（Slash/Power Strike/Fireball） |
| 任务数 | 3 个击杀任务 |
| 场景数 | 2 个（主城+野外） |
| 职业数 | 3 个（Warrior/Mage/Archer） |
| NuGet 包 | MoonSharp, Npgsql EF Core, StackExchange.Redis |
