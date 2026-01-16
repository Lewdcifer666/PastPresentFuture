\# 03 — Game Design Document (GDD)



\## Core Game Loop

Observe → communicate → act → stabilize → verify → progress.



---



\## Role Rules (Player Fantasy)

\### Past

\- Touches the world.

\- Is responsible for moving/placing objects.

\- Often sees consequences late.



\### Present

\- Sees the real current world state.

\- Can freeze objects to prevent disasters or allow precision placement.



\### Future

\- Sees what the room "should look like" in the solved state.

\- Receives clue overlays.

\- Never sees intermediate steps clearly.



---



\## Design Goals per Role

\### Past should feel:

\- active

\- under pressure

\- reliant on guidance



\### Present should feel:

\- powerful but limited

\- constantly managing chaos



\### Future should feel:

\- essential

\- like decoding a weird prophecy

\- never bored



---



\## Puzzle Design Rules

1\. Every room must require all three roles at least once.

2\. Avoid puzzles where one role becomes idle for > 60 seconds.

3\. Teach one new concept per room.

4\. Use time offset to create disagreement but never unfairness.

5\. Provide readable feedback: players must understand why something failed.



---



\## Failure States (Prototype)

\- Object breaks / becomes unusable

\- Door locks temporarily

\- Alarm timer forces reset



Failure must be reversible via:

\- room reset

\- puzzle state rollback (data driven)



---



\## UX Differences per Role

\### Past UI

\- slight distortion / "echo" effect (optional)

\- delayed object movements via proxies



\### Present UI

\- freeze reticle

\- freeze charge meter (optional)



\### Future UI

\- goal outlines

\- clue icons

\- “prophecy fragments” (text or symbols)



---



\## Tone

\- funny, chaotic, cooperative

\- blame becomes comedy, not toxicity

\- optional end-of-room stats for laughs



