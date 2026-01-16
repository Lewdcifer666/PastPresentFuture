\# 07 — Room 001 Spec (Vertical Slice)



\## Purpose

Teach the core fantasy in one room:

\- Past moves objects (but sees delayed results)

\- Present freezes objects to prevent failure

\- Future sees goal state overlays and gives guidance



---



\## Room Theme

"The Pedestal, The Glass Box, The Plate"



---



\## Objects

1\. Door (locked)

2\. Pressure Plate (near door)

3\. Heavy Cube

4\. Glass Box containing Key

5\. Pedestal socket requiring Key

6\. Future-only overlay: outlines where cube must end up, and a symbol indicating freeze timing



---



\## Role Requirements

\- Past must move cube and interact with box/key/pedestal

\- Present must freeze glass box OR cube at least once

\- Future must provide target placement guidance using overlay



---



\## Failure Case

If Past interacts with glass box while it is not frozen, it shatters and the key becomes unreachable (or triggers reset).



Present can prevent this by freezing the box before the interaction moment.



---



\## Success Condition

\- Cube is on pressure plate

\- Key is placed into pedestal

\- Door unlocks and opens



---



\## Implementation Notes

\- Future overlay authored via ScriptableObject:

&nbsp; - cube target outline

&nbsp; - pedestal highlight

&nbsp; - timing icon

\- Past delay should be noticeable (1–2 seconds) but not frustrating

\- Reset button (debug) can reload the scene



