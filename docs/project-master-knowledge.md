# AI MMORPG Demo — 项目完全知识手册

> 面试准备用。涵盖技术栈、设计思路、完整实现细节、数据流向。

---

## 一、技术栈总览

| 层 | 技术 | 版本 | 用途 |
|----|------|------|------|
| 客户端引擎 | Unity | 2022.3 LTS | 渲染、输入、UI、场景管理 |
| 客户端语言 | C# | .NET Standard 2.1 | 游戏逻辑、网络通信、实体管理 |
| 客户端脚本 | Lua | 5.2 (MoonSharp) | 热更新配置、UI 流程控制 |
| 服务端框架 | ASP.NET Core | 9.0 Minimal API | HTTP 端点 + WebSocket |
| 服务端语言 | C# | .NET 9.0 | 业务逻辑、消息路由、权威计算 |
| 数据库 | PostgreSQL | 16 (EF Core + Npgsql 9.0) | 玩家/角色/物品持久化 |
| 缓存 | Redis | 7 (StackExchange.Redis 2.12) | 会话 Token 管理 |
| 容器化 | Docker + Compose | - | 三服务编排部署 |
| Lua 解释器 | MoonSharp | 2.0 (NuGet) | 服务端 C# 内嵌 Lua |
| 协议 | JSON over WebSocket | - | 实时双向通信 |
| 资源更新 | UnityWebRequest | - | HTTP 下载 + SHA256 校验 |
| 测试 | xUnit + WebApplicationFactory | - | 32 个集成测试 |
| 移动端 | Unity Android Build | - | 触屏输入 + APK 打包 |

---

## 二、设计思路 (Why)

### 2.1 为什么是服务端权威？

> 客户端只发送 **意图 (Intent)**，服务端计算 **结果** 并广播。

**三个原因：**
1. **防作弊** — Unity 客户端可以被反编译/修改，服务端不可信
2. **状态一致** — 所有玩家看到的游戏世界由服务端统一维护
3. **简历亮点** — 展示对 MMORPG 安全模型的理解

**具体体现：**
- 移动：客户端发送方向+位置 → 服务端校验速度/边界 → 广播校正后的位置
- 战斗：客户端发送 `{targetId, skillId}` → 服务端检查距离 → 计算伤害 → 广播结果
- 掉落：服务端决定掉什么、掉多少 → 客户端只负责渲染
- 任务：服务端跟踪进度、计算奖励 → 客户端只显示

### 2.2 为什么用 JSON 而不是 Protobuf？

- Demo 阶段优先**可读性和调试效率**
- JSON 在浏览器/Postman/curl 中可直接查看
- 序列化性能差异在 Demo 规模下可忽略
- Proto 文件保留在 `proto/` 作为设计草案

### 2.3 为什么用 MoonSharp 而不是 xLua？

- xLua 需要从 GitHub 手动安装 + native 插件，平台兼容性复杂
- MoonSharp 是纯 C# 实现，`dotnet add package` 一行安装
- 功能足够：执行 Lua 脚本、暴露 C# 对象、运行时重载
- 跨平台无痛（Windows/Linux/macOS 全支持）

### 2.4 为什么分层架构？

```
Gateway (路由层)
  ↓ 依赖
Application (业务层: Interfaces/ + Services/ + 消息路由)
  ↓ 依赖
Contracts (协议层: Models + Payloads)  +  Domain (实体层: Entity/Monster/Quest)
  ↓ 依赖
Infrastructure (存储层: 内存 / PostgreSQL)
```

- **接口与实现分离** — 可无缝切换内存存储 ↔ PostgreSQL
- **单向依赖** — Gateway → Application → Contracts/Domain → Infrastructure
- **测试友好** — `WebApplicationFactory<Program>` 一行启动完整服务端

### 2.5 为什么单体而不是微服务？

- 9 个 Phase 的 Demo 规模，单体足够
- 清晰的分层架构已展示工程能力
- 面试时可表达：**理解微服务利弊，知道什么时候该用/不该用**

---

## 三、完整实现清单

### Phase 1 — 登录 / 角色 / 主城

**做了：**
- 6 个 HTTP REST API 端点（health / guest-login / role list / create / select / enter-city）
- Guest Login：基于 deviceId 生成 playerId + token
- 角色系统：最多 4 角色，名字 1-12 字符，3 职业（Warrior/Mage/Archer）
- Unity 客户端：GameLauncher → NetworkManager → LoginView → RoleSelectView → CityView

**数据流：**
```
Unity LoginView → POST /api/auth/guest-login → Server AuthService
  → 返回 {playerId, token} → 存 NetworkManager
  → POST /api/roles/create → Server RoleService
  → 返回角色列表 → 用户点击角色
  → POST /api/scene/enter-city → CityView 显示角色信息
```

### Phase 2 — WebSocket + 移动同步

**做了：**
- WebSocket 端点 `/ws`，JSON 信封格式 `{t, ts, p}`
- MessageRouter：用 switch 根据 `t` 字段分发消息到处理函数
- WebSocketHandler：接收循环、连接生命周期、断线清理
- SceneManager：管理连接/实体/场景映射，场景内广播
- MovementService：服务端校验位置，边界限制
- Entity 系统：Entity 基类 → PlayerEntity / Monster / Npc

**数据流：**
```
Unity WebSocketClient.Connect() → ws://localhost:5000/ws
  → 发送 c2s.auth → 服务端验证 → c2s.enter_scene
  → 每帧发送 c2s.move {dirX, dirZ, posX, posZ}
  → 服务端广播 s2c.entity_snapshot 给场景所有人
```

### Phase 3 — 战斗 / 怪物 / 掉落 / 背包

**做了：**
- 3 个技能：Slash (1.0x, 2f), Power Strike (1.5x, 2f), Fireball (2.0x, 6f)
- 伤害公式：`(Level × 5 × skill.mult) - (Def × 0.5)`，最小 1，方差 0.9~1.1
- 暴击：15% 概率，1.5x 伤害
- 怪物 AI 状态机：Idle → Patrol（巡逻）→ Chase（追击玩家）→ Attack（攻击）→ Return（返回）
- 3 种怪物：Slime (HP30), Goblin (HP50), Wolf (HP40)
- 掉落：战利品表 + 概率判定，8 种物品模板
- 背包：列表、使用消耗品（回血）、装备/卸下
- 客户端：绿色怪物 Cube、黄色掉落 Sphere、白色伤害文字

### Phase 4 — 任务 / 聊天

**做了：**
- 3 个击杀任务：Kill 3 Slimes / 2 Goblins / 1 Wolf
- 任务进度钩子：`HandleCastSkill` → 怪物死亡 → `QuestService.OnMonsterKilled`
- 自动完成：进度达标 → 奖励 EXP + Gold → 升级
- 聊天：发送到场景 → `SceneManager.Broadcast` → 所有同场景玩家收到
- 客户端 ChatPanel（左下角）+ QuestTracker（右上角）UI 叠加层

### Phase 5 — Lua 热更新

**做了：**
- MoonSharp 2.0 集成（服务端 + 客户端）
- `configs/quests.lua` 和 `configs/monsters.lua` 替代硬编码 C# 字典
- `POST /api/admin/reload-config` 运行时重载
- 客户端 LuaManager：DoString / DoFile / Call / Reload
- 热更新演示：改 Lua 文件 → POST reload → 新值立即生效，C# 代码不动

### Phase 6 — 资源热更新

**做了：**
- `GET /api/resources/manifest` — 返回文件名 + SHA256 + 大小的 JSON 清单
- `GET /api/resources/{path}` — 文件下载（目录遍历防护）
- 客户端 ResourceManager：比对 hash → 下载变更文件 → 缓存到 persistentDataPath
- 启动时自动检查更新（在登录前）

### Phase 7 — 多场景 / 世界地图

**做了：**
- 两个场景：`city_001`（主城，范围 50）+ `field_001`（野外，范围 80）
- 场景切换：传送门标记 → 离开旧场景（广播 entity_left）→ 进入新场景
- 不同场景不同怪物配置（主城有 Slime，野外只有 Wolf/Goblin）
- 客户端：清空旧实体 → 重建新实体 → 摄像机跟随
- CameraFollow 脚本（第三人称跟随）

### Phase 8 — Docker + PostgreSQL + Redis

**做了：**
- `deploy/Dockerfile` — 多阶段 .NET 9 构建
- `deploy/docker-compose.yml` — postgres + redis + gateway 三服务
- `deploy/init.sql` — 自动建表（players, sessions, roles, inventory_items）
- EF Core + Npgsql：AppDbContext, PostgresPlayerStore, PostgresRoleStore
- StackExchange.Redis：会话 Token 缓存
- 环境变量 `USE_POSTGRES=true` 切换持久化/内存模式
- 无 Docker 时自动回退到内存存储

### Phase 9 — Android 打包

**做了：**
- MobileInput：虚拟摇杆（左下角）+ 技能按钮 1/2/3（右下角）
- `Application.isMobilePlatform` 自动切换输入方式
- 服务器地址通过 PlayerPrefs 配置（局域网 IP）
- AndroidBuilder：一键切换平台 + APK 打包（Editor 菜单）
- `docs/build-android.md` 构建文档

---

## 四、网络协议全景

### HTTP 端点（8个）

| 方法 | 路径 | 请求 | 响应 |
|------|------|------|------|
| GET | `/health` | - | `{status, service, phase}` |
| POST | `/api/auth/guest-login` | `{deviceId, platform, appVersion}` | `{code, playerId, token}` |
| POST | `/api/roles/list` | `{playerId, token}` | `{code, roles[]}` |
| POST | `/api/roles/create` | `{playerId, token, name, classId}` | `{code, role}` |
| POST | `/api/roles/select` | `{playerId, token, roleId}` | `{code, role}` |
| POST | `/api/scene/enter-city` | `{playerId, token, roleId}` | `{code, role}` |
| GET | `/api/resources/manifest` | - | `{files:[{name,hash,size}]}` |
| GET | `/api/resources/{path}` | - | 文件二进制 |
| POST | `/api/admin/reload-config` | - | `{message}` |

### WebSocket 消息（14种）

**客户端 → 服务端 (c2s):**
| 类型 | Payload | 说明 |
|------|---------|------|
| `c2s.auth` | `{playerId, token, roleId}` | 认证 |
| `c2s.enter_scene` | `{sceneId}` | 进入场景 |
| `c2s.move` | `{dirX, dirY, dirZ, posX, posY, posZ}` | 移动意图 |
| `c2s.cast_skill` | `{targetId, skillId}` | 施放技能 |
| `c2s.pickup_item` | `{dropId}` | 拾取物品 |
| `c2s.get_inventory` | `{}` | 获取背包 |
| `c2s.use_item` | `{templateId}` | 使用物品 |
| `c2s.equip_item` | `{templateId}` | 装备物品 |
| `c2s.accept_quest` | `{questId}` | 接受任务 |
| `c2s.chat` | `{text}` | 发送聊天 |

**服务端 → 客户端 (s2c):**
| 类型 | Payload | 说明 |
|------|---------|------|
| `s2c.auth_result` | `{ok, message}` | 认证结果 |
| `s2c.enter_scene_result` | `{ok, sceneId, spawnX/Y/Z, entities[]}` | 场景进入 |
| `s2c.entity_snapshot` | `{entities:[{entityId,type,posX/Z,hp,...}]}` | 实体位置广播 |
| `s2c.entity_joined` | `{entity:{...}}` | 新实体加入 |
| `s2c.entity_left` | `{entityId}` | 实体离开 |
| `s2c.combat_event` | `{casterId, targetId, damage, crit, targetDied}` | 战斗事件 |
| `s2c.monster_death` | `{entityId, killerId, expReward, goldReward}` | 怪物死亡 |
| `s2c.drop_spawned` | `{dropId, itemTemplateId, itemName, posX, posZ}` | 掉落生成 |
| `s2c.drop_picked_up` | `{dropId, pickedBy}` | 掉落被拾取 |
| `s2c.inventory_data` | `{items:[{templateId,name,type,quantity}], playerHp}` | 背包数据 |
| `s2c.quest_updated` | `{questId, name, description, progress, targetCount}` | 任务进度 |
| `s2c.quest_completed` | `{questId, name, expReward, goldReward}` | 任务完成 |
| `s2c.chat_broadcast` | `{senderName, text, timestamp}` | 聊天广播 |

---

## 五、领域模型

```
Entity (基类)
├─ EntityId, Type (Player/Monster/Npc), SceneId
├─ PosX/Y/Z, RotY, Hp, MaxHp, MoveSpeed, CreatedAt
│
├─ PlayerEntity : Entity
│   └─ PlayerId, RoleId, RoleName, Level, Gold
│
└─ Monster : Entity
    ├─ TemplateId, DisplayName, AiState (Idle→Patrol→Chase→Attack→Return)
    ├─ Attack, Defense, ChaseRange, AttackRange
    ├─ ExpReward, GoldReward, DropTableIds[]
    └─ PatrolCenter, PatrolRadius, RespawnSeconds

Player (账户)
└─ PlayerId, Sessions[], RoleIds[], SelectedRoleId

Role (角色)
└─ RoleId, PlayerId, Name, Level, ClassId, SceneId, Gold

QuestDefinition (任务定义)
└─ QuestId, Name, Description, TargetMonsterType, TargetCount, ExpReward, GoldReward

PlayerQuestState (玩家任务状态)
└─ QuestId, Progress, Completed

Item / Inventory (物品/背包)
└─ ItemType: Consumable / Equipment / Material
```

---

## 六、关键代码路径 (跟代码用)

### 6.1 登录完整流程
```
1. Unity GameLauncher.Start()
2. → NetworkManager.CheckHealth()
3. → ResourceManager.CheckForUpdates()
4. → UIManager.ShowLogin()
5. → LoginView.OnLoginClick()
6. → NetworkManager.GuestLogin()
7. → POST /api/auth/guest-login
8. → Server AuthService.GuestLogin()
9. → InMemoryPlayerStore.GetOrCreate()
10. → 返回 {playerId, token}
11. → GameLauncher.OnLoginSuccess()
12. → UIManager.ShowRoleSelect()
```

### 6.2 战斗完整流程
```
1. Unity Input.GetKeyDown(KeyCode.Alpha1)
2. → GameManager.AttackNearest(1)
3. → WebSocketClient.SendAsync("c2s.cast_skill", {targetId, skillId})
4. → Server WebSocketHandler → MessageRouter.HandleCastSkill()
5. → CombatService.CastSkill() → 距离检查 → 伤害公式 → 暴击判定
6. → 广播 s2c.combat_event 给场景所有人
7. → 如果怪物死亡:
8.   → 广播 s2c.monster_death
9.   → MonsterService.MarkDead() → 15秒后重生
10.  → QuestService.OnMonsterKilled() → 更新任务进度
11.  → DropService.GenerateDrops() → 广播 s2c.drop_spawned
12. → Unity GameManager.HandleCombatEvent() → 显示伤害文字
```

### 6.3 场景切换完整流程
```
1. Unity 玩家走到传送门标记附近
2. → GameManager.RequestSceneChange("field_001")
3. → _currentSceneId = "field_001" (预更新防止重复触发)
4. → WebSocketClient.SendAsync("c2s.enter_scene", {sceneId: "field_001"})
5. → Server MessageRouter.HandleEnterScene()
6.   → 离开旧场景: SceneManager.LeaveScene() + 广播 entity_left
7.   → 进入新场景: SceneManager.EnterScene() → 设置出生点
8.   → MonsterService.EnsureSceneMonsters() → 生成该场景怪物
9.   → 返回 s2c.enter_scene_result {sceneId, spawnPos, entities[]}
10. → Unity GameManager.HandleEnterScene()
11.  → 销毁所有旧实体、旧掉落
12.  → 在出生点创建新 localPlayer
13.  → CameraFollow.target = 新玩家
14.  → 加载新场景实体列表
15.  → IsReady = true
```

### 6.4 Lua 热更新流程
```
1. 开发者修改 configs/quests.lua (改 exp: 30 → 999)
2. → curl -X POST http://localhost:5000/api/admin/reload-config
3. → Server QuestService.Reload()
4. → MoonSharp DoFile("configs/quests.lua")
5. → 解析 Lua Table → 更新 _definitions 字典
6. → 下一个玩家接任务时使用新值
7. → C# 代码完全未修改
```

---

## 七、面试高频问答速查

**Q: 项目规模多大？**
A: 服务端 ~45 个 C# 文件，客户端 14 个脚本，5 个服务端项目，32 个集成测试。

**Q: 你负责了什么？**
A: 全部。从目录结构、协议设计、服务端架构、客户端脚本、Docker 部署、Android 打包，到测试和文档。

**Q: 最难的部分是什么？**
A: (选一个) 场景切换的实体生命周期管理 — 要确保离开旧场景时正确广播并清理，进入新场景时避免重复创建。或者战斗系统中的服务端权威计算 — 伤害公式、暴击、距离检查、掉落生成、任务进度全部要在服务端完成。

**Q: 如果重做会怎么改进？**
A: 1) 引入 AOI 减少广播量 2) 用消息队列替代直接 WebSocket 广播 3) 状态同步替代帧同步 4) 加 ECS 架构优化大量实体。

**Q: 学到了什么？**
A: 服务端权威架构的重要性、实时通信的复杂性、热更新双管线设计、分层架构如何提高可测试性、Docker 编排的实践经验。
