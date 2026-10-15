# Gestures Movement Course — WebPageLLM Handoff

**Audience:** WebPageLLM and anyone integrating a Workshop capability into the browser experience  
**Status:** early integration handoff; course geometry and deterministic outcome evaluation are implemented on the Gestures working branch, with CI green and public API review still open in PR #10  
**Source repository:** [TheSingularityWorkshop.Gestures](https://github.com/TrentBest/TheSingularityWorkshop.Gestures)  
**Working source branch:** `work/planar-ik-reference`  
**Do not merge, publish a package, or deploy from this brief.**

## Why bring this into the WebPage now?

The Workshop should make its capabilities inspectable, not merely describe them. The movement-course work is a good first vertical slice: a person can author a challenge, see its geometry, inspect the order of its elements, and later compare an agent's attempt with outcomes reported by the authoritative simulation.

This is **not yet a parkour agent or a physics-backed movement demo**. The first browser slice should make the course contract visible while keeping that boundary honest.

## What Gestures has in the working PR

PR: [#10 — Add a constrained planar IK CPU reference](https://github.com/TrentBest/TheSingularityWorkshop.Gestures/pull/10)

The branch currently contains:

- Engine-neutral, immutable `MovementCourse` and `MovementCourseElement` types.
- Stable course-local element IDs and explicit authored traversal order.
- Axis-aligned bounds represented by center plus full X/Y/Z extents, with consistent caller-defined units.
- A vocabulary that includes approach and acceleration zones, hurdles, clearance and landing zones, continuation, gaps, vaults, balance sections, climbs, checkpoints, and finish markers.
- A data-only `MovementCourseObservation` contract and deterministic `MovementCourseEvaluator` for externally supplied outcomes.
- Outcomes for cleared, missed, collision, unsafe landing, blocked, and indeterminate; aggregate evaluation also distinguishes completed, failed, incomplete, and indeterminate.

Relevant source on the working branch:

- [MovementCourse.cs](https://github.com/TrentBest/TheSingularityWorkshop.Gestures/blob/work/planar-ik-reference/src/TheSingularityWorkshop.Gestures/MovementCourse.cs)
- [MovementCourseEvaluator.cs](https://github.com/TrentBest/TheSingularityWorkshop.Gestures/blob/work/planar-ik-reference/src/TheSingularityWorkshop.Gestures/MovementCourseEvaluator.cs)
- [MovementCourseTests.cs](https://github.com/TrentBest/TheSingularityWorkshop.Gestures/blob/work/planar-ik-reference/tests/TheSingularityWorkshop.Gestures.Tests/MovementCourseTests.cs)
- [MovementCourseEvaluatorTests.cs](https://github.com/TrentBest/TheSingularityWorkshop.Gestures/blob/work/planar-ik-reference/tests/TheSingularityWorkshop.Gestures.Tests/MovementCourseEvaluatorTests.cs)

The branch is not merged or published. The WebPage must not add a package dependency on an unpublished version. The source contracts may still change during review.

## What to build first in WebPage

Add a small, lazy-loaded **Movement Lab** experience using the existing Workshop hub/experience conventions. Do not create a competing navigation system or disturb the landing sequence, Pong, Living GUI, or other experiences.

### 1. Show a tiny authored hurdle course

Use the same three-element geometry as the current Gestures evaluator tests so the browser view and source-level fixture stay aligned:

| Element ID | Kind | Center (X, Y, Z) | Full size (X, Y, Z) |
|---|---|---|---|
| `approach` | ApproachZone | `(0, 0, 0)` | `(2, 1, 2)` |
| `hurdle-01` | Hurdle | `(3, 0.5, 0)` | `(0.2, 1, 2)` |
| `landing` | LandingZone | `(5, 0, 0)` | `(2, 0.2, 2)` |

All values use the fixture's `meters` label; bounds are axis-aligned and size means full extent, not half extent. This is deliberately a small geometry/evaluation fixture, not a full 110-metre track.

Show a simple top-down and/or side-on diagram, ordered element list, stable IDs, center, full extents, and units. Distinguish the hurdle from traversal regions visually. Let a visitor select an element and inspect its geometry.

This is a **WebPage display fixture**, not yet a serialized Gestures course artifact. Keep it in a replaceable adapter/model so it can be replaced with the actual package contract after the PR is accepted and a consumable package/version exists. Do not invent a JSON schema and claim it is canonical.

### 2. Make the evidence boundary visible

Display a compact status legend:

- **Course geometry** — available in the current working branch.
- **Outcome contract** — implemented in the working branch, pending CI/review.
- **Physics-confirmed attempt** — not connected yet.
- **Agent traversing the course** — not implemented by this slice.

Do not infer a successful hurdle from a reachable IK target, a rendered pose, or proximity to the landing zone. Only the authoritative simulation can provide the physical evidence used to judge collision, clearance, support, momentum, and landing safety.

If adding clickable sample outcomes to test the presentation, label them **synthetic test observations — not physics output**. Never present them as a real agent run or verified simulation.

### 3. Keep it cheap to open

The existing Workshop has expensive living visualizations. Do not load a physics simulation, large scene, continuous animation loop, or background agent just to show this static course.

- Load the Movement Lab only when the visitor opens it.
- Start with static geometry and explicit user interaction.
- Measure cold-open latency and interaction latency.
- Dispose/unsubscribe any runtime work when the experience closes.
- Keep the initial view useful on desktop and narrow screens, and make selection keyboard-accessible.

## What this slice should prove

- A visitor can discover the Movement Lab from the existing Workshop hub.
- The course is visible as an ordered, agent-independent challenge.
- The selected element's ID, type, center, size, and units are inspectable.
- The UI distinguishes geometry, synthetic test data, and real simulation evidence.
- Opening or closing the experience does not regress the existing landing sequence or trigger costly background work.
- The adapter boundary is small enough to replace when the real course package integration is ready.

## Next integration step after the Gestures PR is validated

1. Recheck PR #10 CI and review the public API before depending on it.
2. Agree on a versioned interchange shape only when serialization is deliberately designed; the current geometry contract does not yet define serialization.
3. Add a small WebPage adapter to translate the accepted course model into the browser view.
4. Connect outcomes only after the physics owner provides a tested observation source. Until then, keep evaluator examples explicitly synthetic.
5. Add a real agent attempt only when the movement controller, physics stepping, and evidence path are connected end to end.

## WebPageLLM handoff request

Inspect the current navigation and experience host first. Implement the smallest truthful, lazy-loaded geometry preview that fits those conventions, or report the exact blocker if the current host cannot expose it without architectural churn.

Report:

- exact files changed and tests run;
- how to launch the local WebPage and open the Movement Lab;
- what Trent should click or inspect;
- cold-open/interaction measurements;
- which data is fixture-only versus connected to real code.

This brief authorizes implementation of the local preview only. It does **not** authorize package publication, PR merge, Azure deployment, or any claim that physical course traversal already works.
