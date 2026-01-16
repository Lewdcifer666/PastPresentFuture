\# 05 — Naming Conventions



\## C# Naming

\- Classes: `PascalCase`

\- Methods: `PascalCase`

\- Public fields: `PascalCase` (prefer properties)

\- Private fields: `\_camelCase`

\- Interfaces: `IThing`

\- Events: `OnSomethingHappened`



\## Unity Assets

\- Scenes: `Boot`, `Lobby`, `Room\_001`

\- Prefabs: `PF\_Player`, `PF\_Door`, `PF\_Crate`

\- ScriptableObjects: `SO\_Role\_Past`, `SO\_Room\_001`

\- Materials: `M\_Whatever`

\- Textures: `T\_Whatever`

\- Audio: `A\_Whatever`



\## Folders

\- Runtime scripts: `Assets/\_Project/Runtime/...`

\- Editor scripts: `Assets/\_Project/Editor/...`

\- Never put scripts directly in `Assets/` root.



\## IDs

\- Tasks: `CORE-010`, `NET-010` etc.

\- Objects referenced by config should have stable IDs in the config.



