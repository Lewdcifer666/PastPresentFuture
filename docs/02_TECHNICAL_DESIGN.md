\# 02 — Technical Design (TDD)



\## Core Technical Problem

Simulate one authoritative reality while letting each role see a different time slice or overlay — without physics rewinds or paradoxes.



---



\## Architectural Layers

1\. \*\*Core Logic (Pure C#)\*\*

&nbsp;  - timeline buffer

&nbsp;  - role permissions

&nbsp;  - puzzle objective evaluation

2\. \*\*Runtime Simulation (Unity scene objects)\*\*

&nbsp;  - interactables and physics objects

3\. \*\*Networking (NGO)\*\*

&nbsp;  - authoritative host

&nbsp;  - client intents

&nbsp;  - replication of snapshots/events

4\. \*\*Presentation\*\*

&nbsp;  - per-role cameras, overlays, UI

&nbsp;  - Past delay rendering

&nbsp;  - Future target overlays



---



\## Timeline Model (Authoritative)

\### Key Rule

There is only one "true" simulation time: `T\_now` on the host.



We record snapshots for all tracked objects:

\- Transform (position/rotation)

\- Motion (velocity if needed)

\- State flags (frozen, activated, etc.)

\- Puzzle-specific state (switch states, lock states, etc.)



Snapshots are stored in a ring buffer.



\### Terminology

\- \*\*Snapshot\*\*: state of an object at a timestamp

\- \*\*Timeline Buffer\*\*: circular buffer storing snapshots over recent seconds

\- \*\*View Time\*\*: what a player renders

\- \*\*Authority Time\*\*: what the host simulates



\### View Times

\- Present view time: `T\_now`

\- Past view time: `T\_now - PastDelaySeconds`

\- Future view: does not render `T\_now + ...` by default; instead renders:

&nbsp; - goal overlay state (data-driven)

&nbsp; - clue overlays



---



\## Past Delay System (Implementation Approach)

\### Goal

Past sees object transforms from older snapshots, producing the illusion of being behind.



\### Hard Constraint

We do NOT run physics in the past.

Physics stays authoritative on host.



\### Implementation Approach

\- Past client camera renders a "past view" of objects by applying snapshot transforms.

\- Past client interactions are still sent to host in real-time.

\- Host applies interactions to real objects.

\- Past player sees consequences with delay because their renderer uses older snapshots.



\### Practical Strategy

Maintain two representations:

1\. \*\*Real Object\*\* (authoritative transform)

2\. \*\*Past View Proxy\*\* (visual-only)



The Past player sees proxies; Present sees real objects.



Optionally, Past view can use a shader/material effect instead of proxy duplication,

but proxies are usually simpler and more stable for early prototypes.



---



\## Freeze System (Present Ability)

\### Definition

Freezing an object:

\- disables motion (physics velocity = 0)

\- disables being moved by other forces

\- optionally disables interaction by Past (design choice per object)



\### Freeze Rules (prototype)

\- Present can freeze any object in a `Freezable` layer.

\- Frozen objects are replicated by host.

\- Freeze duration can be infinite or limited per puzzle.



\### Technical Implementation

\- On freeze: host sets state `IsFrozen = true`

\- Host applies physics constraints (e.g., Rigidbody constraints) OR toggles kinematic state

\- Host records freeze events into timeline snapshots



---



\## Future Role: Goal Overlay + Clues

Future does not simulate actual future.

Future sees:

\- outlines for where objects must end up

\- symbols indicating correct states

\- clue fragments



These are authored per room via `ScriptableObject` configs.



---



\## Networking Model (NGO)

\### Authoritative Host

\- Host owns all interactables and room state.

\- Clients send "intent" RPCs:

&nbsp; - AttemptInteract(objectId, actionType)

&nbsp; - AttemptMove(objectId, target)

&nbsp; - AttemptFreeze(objectId, freezeOn)



\### Replication

\- Use NetworkVariables for simple state flags (frozen, activated).

\- Use NetworkTransform (or custom transform replication) for object motion.

\- Timeline snapshots are not fully replicated as history; instead:

&nbsp; - host sends current state

&nbsp; - each client records locally for rendering past-proxies



---



\## Key Services (Runtime Systems)

\- `SessionManager` — players, roles, join/leave

\- `RoleService` — permission checks for role actions

\- `TimelineService` — records snapshots, provides transforms for view time

\- `FreezeSystem` — freeze/unfreeze with replication

\- `RoomManager` — room state machine, objective evaluation

\- `InteractableSystem` — interaction interface layer



---



\## Debugging \& Tools

\- Timeline HUD (debug only): show `T\_now`, `PastViewTime`, buffer length.

\- Gizmos for Future overlays.

\- Console commands:

&nbsp; - set past delay

&nbsp; - force open door

&nbsp; - reload room



---



\## Performance Constraints

\- Snapshot buffer should be bounded (ring buffer).

\- Record only required objects (tracked set).

\- Snapshot frequency configurable (e.g., 10–20 per second).



---



\## Security / Anti-Cheat (Prototype Level)

\- Host validates all intents.

\- Client never applies authoritative changes.



