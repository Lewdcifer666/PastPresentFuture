\# 04 — Roadmap \& Tasks



\## Progress Tracker

Update this section manually as you complete tasks.



\- Current Milestone: M1 — Prototype Core

\- Current Task ID: ROOM-010

\- Current Branch: main

\- Notes: Initial repo skeleton + docs



---



\## Task ID Rules

\- Each task has a stable ID like `CORE-010`.

\- Subtasks are `CORE-010.A`, `CORE-010.B`, etc.

\- Acceptance criteria is mandatory.



In a new chat, say:

> "Continue from CORE-001"



---



\# Milestone M0 — Foundation



\## CORE-001 — Create repo skeleton

\### Steps

\- Create folders: `/docs`, `/unity`

\- Add markdown docs from `/docs` plan

\- Add `.gitignore` for Unity

\### Acceptance Criteria

\- Repo has docs folder with all required files

\- Unity folder exists

\- Docs are committed and pushed

\- Unity `.gitignore` is present



---



\## CORE-002 — Create Unity project (Unity 6.3 LTS)

\### Steps

\- Create Unity project under `/unity/PastPresentFuture.Unity`

\- Ensure project opens in Unity Hub

\### Acceptance Criteria

\- Project loads without errors

\- Commit `Assets`, `Packages`, `ProjectSettings`



---



\## CORE-003 — Create scenes: Boot and Lobby

\### Steps

\- Create `Assets/\_Project/Scenes/Boot.unity`

\- Create `Assets/\_Project/Scenes/Lobby.unity`

\- Boot auto-loads Lobby

\### Acceptance Criteria

\- Press Play from Boot → Lobby loads automatically



---



\# Milestone M1 — Prototype Core (Prove the Concept)



\## NET-010 — Add NGO and basic host/join UI

\### Subtasks

\- NET-010.A: Install NGO packages

\- NET-010.B: Create NetworkManager prefab

\- NET-010.C: Create simple UI buttons: Host / Join(Local)

\### Acceptance Criteria

\- Two editor instances can connect

\- A 3-player limit is enforced



---



\## ROLE-010 — Role assignment system

\### Subtasks

\- ROLE-010.A: Role enum Past/Present/Future

\- ROLE-010.B: Random or selected assignment

\- ROLE-010.C: Display role in UI

\### Acceptance Criteria

\- Each player has a role

\- Roles are visible in HUD



---



\## TL-010 — Timeline snapshot buffer (host + client record)

\### Subtasks

\- TL-010.A: Trackable objects registry

\- TL-010.B: Ring buffer snapshots

\- TL-010.C: Debug timeline HUD

\### Acceptance Criteria

\- Snapshots recorded for tracked objects

\- Buffer keeps last N seconds



---



\## TL-020 — Past view proxy rendering

\### Subtasks

\- TL-020.A: Create proxy objects for Past client

\- TL-020.B: Apply snapshot transforms at PastViewTime

\- TL-020.C: Hide real objects from Past camera

\### Acceptance Criteria

\- Past player sees delayed motion

\- Present player sees real-time motion



---



\## FX-010 — Freeze system (Present role)

\### Subtasks

\- FX-010.A: Freezeable component

\- FX-010.B: Host-authoritative freeze toggle

\- FX-010.C: Visual feedback (simple)

\### Acceptance Criteria

\- Present can freeze/unfreeze

\- Frozen objects stop moving consistently on all clients



---



\## ROOM-010 — Room framework (objectives + door)

\### Subtasks

\- ROOM-010.A: RoomManager skeleton

\- ROOM-010.B: Objective evaluation interface

\- ROOM-010.C: Door opens on success

\### Acceptance Criteria

\- A room can be marked solved and opens door



---



\## ROOM-020 — Implement Room\_001 (vertical slice)

See `docs/07\_ROOM\_001\_SPEC.md`

\### Acceptance Criteria

\- Room\_001 is solvable

\- Requires all 3 roles at least once

\- Demonstrates Past delay + Present freeze + Future overlay



---



\# Milestone M2 — Vertical Slice Polish



\## UI-010 — Role-specific HUD

\## UI-020 — Tutorial prompts for Room\_001

\## NET-020 — Online join via Relay/Lobby (optional for prototype)

\## QA-010 — Reset / fail-state loop



---



\# Milestone M3 — Steam Readiness (Later)

(These tasks are intentionally deferred.)



\## STEAM-010 — Steamworks integration spike

\## STEAM-020 — Steam build pipeline + depots

\## STEAM-030 — Steam invites / rich presence (optional)



