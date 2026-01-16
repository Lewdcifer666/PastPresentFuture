\# Past / Present / Future — Project Start Here



\## What this repo is

A 3-player co-op puzzle game (local + online) where each player is assigned a role:

\- \*\*Past\*\*: can interact/move objects.

\- \*\*Present\*\*: can time-freeze objects.

\- \*\*Future\*\*: can only observe the target/end state and obtain clue overlays (no direct interaction).



Players share the same physical space (a room), but:

\- They do \*\*not\*\* see each other.

\- They see \*\*different versions\*\* of the same room via role-based perception filters and a timeline offset system.

\- They must communicate using voice chat.



---



\## Publishing Target (Steam)

This project is intended for release on \*\*Steam\*\* (initial target: \*\*Windows\*\*).

Steamworks integration is \*\*not\*\* required for the prototype milestone and will be introduced later,

after the vertical slice is proven fun and stable.



---



\## Single Source of Truth (SSOT)

This `docs/` folder is the SSOT for:

\- Architecture

\- Folder structure

\- Naming rules

\- Task breakdown

\- Milestones

\- Room design

\- Definitions of done

\- How to resume work in new chat sessions



\*\*Rule:\*\* If something conflicts with `docs/`, `docs/` wins.



---



\## How to Resume Work (New Chat Protocol)

In any new chat session, you can say:



> "We are at TASK XYZ-###. Continue."



The assistant should:

1\. Open `docs/04\_ROADMAP\_TASKS.md`

2\. Find the requested task ID (example: `CORE-010`)

3\. Continue with a detailed step-by-step guide:

&nbsp;  - exact files to create/edit

&nbsp;  - exact folder paths

&nbsp;  - acceptance criteria

&nbsp;  - a final checklist

4\. Update guidance to match current task requirements.



---



\## Documents Index

\- `01\_PROJECT\_BLUEPRINT.md` — overall architecture + product decisions

\- `02\_TECHNICAL\_DESIGN.md` — timeline model + networking model + systems

\- `03\_GAME\_DESIGN.md` — rules, roles, puzzle philosophy, UX

\- `04\_ROADMAP\_TASKS.md` — tasks with IDs, subtasks, acceptance criteria

\- `05\_NAMING\_CONVENTIONS.md` — strict naming rules

\- `06\_REPO\_STRUCTURE.md` — exact folder and solution layout

\- `07\_ROOM\_001\_SPEC.md` — complete spec for the first room

\- `08\_DEFINITION\_OF\_DONE.md` — DoD per system and per milestone



---



\## Target Tools

\- \*\*Unity\*\*: Unity 6.3 LTS (6000.3.4f1)

\- \*\*IDE\*\*: Visual Studio 2022

\- \*\*Language\*\*: C#

\- \*\*Networking\*\*: Netcode for GameObjects (NGO) (recommended)

\- \*\*Version Control\*\*: GitHub

\- \*\*Release Target\*\*: Steam (later milestone)



---



\## Guardrails (Non-Negotiables)

\- Role rules must be enforced by code (not just UI).

\- Networking must be host-authoritative (anti-desync).

\- The Past player’s "delay view" is a render/visualization layer; physics stays authoritative.

\- Future player is never idle: they always have overlays/clues to interpret.

\- Rooms are authored via data/config, not hard-coded logic.



