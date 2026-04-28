# Client Directory Rules

These rules apply after the real Unity project is created under `client/UnityProject/`.

## Folder Naming

- Use PascalCase for Unity C# feature and framework folders.
- Use lowercase for Lua script folders.
- Keep feature names aligned with docs and proto names: `Login`, `RoleSelect`, `City`, `World`, `Entity`, `Combat`, `Inventory`, `Quest`, `Chat`.
- Do not create random temporary folders under `Assets/Game/`.
- Do not commit generated Unity folders: `Library/`, `Temp/`, `Obj/`, `Logs/`, or build outputs.

## C# Naming

- Types use PascalCase, for example `GameLauncher`, `LoginController`, `NetworkClient`.
- Methods and properties use PascalCase.
- Private fields use `_camelCase`.
- Namespaces should start with `MmoDemo.Client`.
- C# owns low-level, high-frequency, network, entity, resource, and UI infrastructure code.

## Lua Naming

- Lua files use lowercase snake_case, for example `login_flow.lua`.
- Lua modules should follow folder responsibility, such as `ui/login_flow.lua`.
- Lua handles UI flow, quests, activities, tutorial, and lightweight hotfix logic.
- Lua must not decide final combat damage, rewards, inventory changes, or quest completion.

## Resource Naming

- UI prefabs: `UI_<Feature>_<Name>`.
- Characters: `Char_<RoleOrNpcName>`.
- Monsters: `Monster_<Name>`.
- Maps: `Map_<SceneName>`.
- Audio: `Audio_<Category>_<Name>`.
- VFX: `VFX_<Feature>_<Name>`.

## Directory Control

New top-level folders under `Assets/Game/` require documentation in this file or `ClientArchitecture.md`. Feature folders must not be expanded beyond the current phase without a scoped task.

