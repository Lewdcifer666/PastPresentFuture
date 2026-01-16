\# 01 — Project Blueprint



\## Vision

A 3-player co-op puzzle game where communication is mandatory because each player perceives and influences time differently.



The game’s uniqueness comes from:

\- \*\*asymmetric perception\*\* (each role sees a different version of the room)

\- \*\*asymmetric authority\*\* (each role has different permitted actions)

\- \*\*timeline offset\*\* (Present sees Past’s consequences earlier than Past sees them)



---



\## Core Pillars

1\. \*\*Asymmetric roles\*\*: each role feels essential.

2\. \*\*Conflicting realities\*\*: players disagree about what is "true" in the room.

3\. \*\*Chaos with structure\*\*: funny communication failures, but puzzles remain solvable.

4\. \*\*Room-based progression\*\*: each room introduces one new rule.



---



\## Target Experience (Player Loop)

1\. Enter room

2\. Observe (each role sees unique information)

3\. Communicate (voice)

4\. Attempt solution (Past acts, Present stabilizes, Future directs)

5\. Verify objective state

6\. Door opens → move to next room



---



\## Player Roles (Summary)

\### Past (Actor)

\- Can pick up / move / activate interactables.

\- Has delayed perception: sees the world slightly behind the true timeline.

\- Interactions occur in real-time on the authoritative host.



\### Present (Controller)

\- Can time-freeze objects.

\- Sees the true current state.

\- Often detects catastrophes before Past does.



\### Future (Oracle)

\- Cannot interact physically.

\- Sees the target state overlay and receives clue layers.

\- Does not see intermediate progress, only end-goal hints and/or “prophecy” fragments.



---



\## Platform \& Modes

\- Local co-op: 3 players on one machine (controllers).

\- Online co-op: 3 players across the network.

\- Roles can be chosen or randomly assigned.



---



\## Publishing Target (Steam)

Initial release target: \*\*Steam (Windows)\*\*.

Steamworks integration is deferred until after the vertical slice milestone.



---



\## Networking Choice

Host-authoritative simulation:

\- Clients send intents.

\- Host validates and applies.

\- Host replicates state/events.



Reason:

\- prevents timeline divergence

\- consistent perception offsets

\- easier debugging



---



\## Non-Goals (Initial Prototype)

\- Dedicated servers (later)

\- Complex matchmaking (later)

\- Steamworks integration (later)

\- Procedural rooms (later)



---



\## Milestone Strategy

\- Build one \*\*vertical slice room\*\* that demonstrates the whole concept.

\- Only then expand to more rooms/content.



Milestones are defined in `docs/04\_ROADMAP\_TASKS.md`.



