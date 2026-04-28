# Client Architecture

The Unity client is responsible for rendering, UI, input, local presentation, resource loading, and sending network intent messages. The server remains authoritative for movement validation, combat result, inventory, loot, quest progress, and rewards.

## Layers

### Boot

Entry layer for the Unity client. It initializes the runtime, creates persistent roots, and enters the first UI flow.

Planned directory:

```text
Assets/Game/Boot/
```

### Core

Low-level client runtime objects that must stay stable across scenes, such as game state, app lifecycle, and shared runtime context.

Planned directory:

```text
Assets/Game/Core/
```

### Framework

Reusable systems used by features.

Planned directories:

```text
Assets/Game/Framework/Asset/
Assets/Game/Framework/Config/
Assets/Game/Framework/Event/
Assets/Game/Framework/Lua/
Assets/Game/Framework/Network/
Assets/Game/Framework/Pool/
Assets/Game/Framework/UI/
```

### Feature

User-facing modules. Phase 1 only prepares Login, RoleSelect, and City. Later folders are reserved for later phases and must not be implemented early.

Planned directories:

```text
Assets/Game/Feature/Login/
Assets/Game/Feature/RoleSelect/
Assets/Game/Feature/City/
Assets/Game/Feature/World/
Assets/Game/Feature/Entity/
Assets/Game/Feature/Combat/
Assets/Game/Feature/Inventory/
Assets/Game/Feature/Quest/
Assets/Game/Feature/Chat/
```

### Lua

Hotfixable script layer for UI flow, quests, activities, tutorial, and lightweight feature glue. Lua does not own high-frequency systems or authoritative results.

Planned directories:

```text
Assets/Game/Lua/bootstrap/
Assets/Game/Lua/ui/
Assets/Game/Lua/quest/
Assets/Game/Lua/combat/
Assets/Game/Lua/inventory/
Assets/Game/Lua/chat/
```

### Res

Client presentation resources. Resource hot update is separate from Lua hotfix and is not implemented in Phase 1 Task 2.

Planned directories:

```text
Assets/Game/Res/UI/
Assets/Game/Res/Character/
Assets/Game/Res/Monster/
Assets/Game/Res/Map/
Assets/Game/Res/Audio/
Assets/Game/Res/VFX/
```

### Editor

Unity editor-only tools. Runtime code must not depend on editor assemblies.

Planned directory:

```text
Assets/Game/Editor/
```

