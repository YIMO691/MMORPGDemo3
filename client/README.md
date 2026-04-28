# MmoDemo Unity Client

This directory is reserved for the Unity client. Phase 1 Task 2 prepares the client structure and documentation only. It does not create a real Unity project or implement login, role selection, city entry, combat, inventory, quest, chat, movement sync, resource hot update, or Lua hotfix logic.

## Current Stage

- Phase 0 foundation is complete.
- Phase 1 Task 1 server skeleton is complete.
- Phase 1 Task 2 prepares the Unity client skeleton.
- Unity Hub and Unity Editor are installed locally, but this task intentionally does not generate Unity project files.

## Unity Project Creation Recommendation

Use Unity Hub to create the real project later:

- Project location: `F:/AI_MMORPG/client/UnityProject`
- Recommended Unity version: Unity `2022.3 LTS` or newer installed LTS in the same major line.
- Recommended template: `3D (URP)` for an Android-oriented MMORPG demo.
- Keep generated Unity folders such as `Library/`, `Temp/`, `Obj/`, and build output directories out of Git.

Do not create the real Unity project until the next client implementation task explicitly asks for it.

## Planned Directory Structure

```text
client/
  README.md
  UnityProjectPlaceholder.md
  docs/
    ClientArchitecture.md
    ClientDirectoryRules.md
    ClientModuleBoundary.md
  UnityProject/
    Assets/
      Game/
        Boot/
        Core/
        Framework/
          Asset/
          Config/
          Event/
          Lua/
          Network/
          Pool/
          UI/
        Feature/
          Login/
          RoleSelect/
          City/
          World/
          Entity/
          Combat/
          Inventory/
          Quest/
          Chat/
        Lua/
          bootstrap/
          ui/
          quest/
          combat/
          inventory/
          chat/
        Res/
          UI/
          Character/
          Monster/
          Map/
          Audio/
          VFX/
        Editor/
```

## Phase 1 Task 2 Non-Goals

- No real Unity project generation.
- No Unity scene creation.
- No C# gameplay implementation.
- No Lua runtime implementation.
- No server calls.
- No art, audio, VFX, or large resource import.
- No combat, inventory, quest, chat, or movement sync.

## Next Step

Phase 1 Task 3: login protocol and interface design.

