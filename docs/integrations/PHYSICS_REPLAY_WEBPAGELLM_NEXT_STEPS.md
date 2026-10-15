# WebPageLLM Addendum — Physics Replay Lab: Current State and Immediate Intent

**Audience:** WebPageLLM implementing the Workshop browser experience  
**Applies to:** `TrentBest/WebPage`, branch `development`  
**Companion brief:** [Physics Replay / Shareable Moments — WebPageLLM Integration Brief](./PHYSICS_REPLAY_WEBPAGELLM_HANDOFF.md)  
**Status:** engineering handoff; no publish or deployment authorization

## Why this addendum exists

The companion handoff describes the product direction and the initial model-level replay seam. Since it was written, the Physics repository has gained a fork-aware route executor and a focused integration test using the real `ConstantAccelerationSolver`. This addendum records that newer evidence and gives WebPageLLM a concrete, bounded implementation target without replacing the companion brief (which may be edited concurrently).

## Current Physics evidence

Repository: [TheSingularityWorkshop.Physics — development](https://github.com/TrentBest/TheSingularityWorkshop.Physics/tree/development)

- [`TimelineReplayExecutor`](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/Physics/Universe/TimelineReplayExecutor.cs) reconstructs a target tick in one validated segment using caller-supplied checkpoint restoration, event application, and model advancement.
- [`TimelineReplayRouteExecutor`](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/Physics/Universe/TimelineReplayRouteExecutor.cs) coordinates validated segments along a fork-aware route, checking plan order, anchor consistency, and state continuity at fork boundaries.
- [The route executor tests](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/tests/TheSingularityWorkshop.Physics.Tests/TimelineReplayRouteExecutorTests.cs) cover a successful fork route, mismatched fork state, and incomplete route rejection.
- [The solver-backed replay integration test](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/tests/TheSingularityWorkshop.Physics.Tests/ConstantAccelerationTimelineReplayTests.cs) calls the repository's actual `ConstantAccelerationSolver.Advance`, restores a small kinematic checkpoint, applies a recorded velocity-delta event, and checks the reconstructed position, velocity, and elapsed time at a target tick.
- [Physics CI for the documented solver-backed test](https://github.com/TrentBest/TheSingularityWorkshop.Physics/actions/runs/38674140600) passed.

This proves a working model-level reconstruction contract and a test of one solver adapter. It does **not** prove a browser-ready world adapter, general scene serialization, cross-platform bitwise determinism, asset transport, or an end-to-end interactive physics scene. Do not present the browser fixture as a replay produced by Physics.

## My intent for the WebPage experience

Build a visible **Replay Lab / Moments** experience that demonstrates the user journey and gives Trent something real to inspect in a browser while the integration boundary matures. It should fit the current Workshop hub and navigation patterns, not introduce a competing app shell.

### Implement now

1. Inspect the current WebPage hub, routing, styling, and lazy-loading conventions before choosing files or navigation placement.
2. Add a discoverable **Replay Lab** destination using those existing conventions.
3. Create a small, visually compelling timeline scene with start, selected moment, and end; visible event markers; a playhead; and an accessible scrub control. If no browser-consumable Physics adapter exists, use a tiny deterministic illustrative fixture.
4. Label the scene plainly as **illustrative demo data — not yet connected to Physics world replay**. Make the label persist near the scene, not only in documentation.
5. Make scrubbing and play/pause genuinely change the illustrative scene and displayed tick. Keep behavior deterministic and local; no fabricated backend or pretend network calls.
6. Show capability states honestly:
   - **Implemented in Physics:** model-level single-segment reconstruction and fork-aware route coordination, with tests.
   - **Not yet integrated in browser:** actual Physics world/solver adapter and browser-consumable scene data.
   - **Planned:** portable moment export, share links, hosted playback, and end-to-end remix.
7. Disable or clearly mark unavailable any share, remix, capture, save, or publish action that does not work end to end.
8. Keep the feature lazy-loaded and bounded. Preserve the existing landing experience, Living GUI, other tabs, and manifest-driven behavior. Do not add a new mandatory dependency or parse opaque Physics checkpoint bytes in browser code.

### Architecture boundary

Put the visualization behind a small replaceable adapter/data contract. The browser-facing demo may expose presentation data such as ticks, event labels, and scene values, but must not pretend that this is the Physics checkpoint format. Do not couple WebPage directly to Physics internals or invent a public API that does not exist.

Keep responsibilities separated:
- Physics owns checkpoint/event compatibility, replay semantics, route/fork validation, and solver advancement.
- WebPage owns the visual experience, accessible controls, annotations, and unsupported-state messaging.
- A future transport/package layer can define versioned portable moments after real payload and compatibility requirements are known.
- Sharing/hosting must later handle consent, attribution, access, retention, and deletion separately from the solver.

## Acceptance checklist for the first browser pass

- [ ] Entry is discoverable from the existing hub and does not break current navigation.
- [ ] Opening Replay Lab lazy-loads its scene.
- [ ] Scrub/play controls actually update the illustrative scene and tick label.
- [ ] Start, selected moment, event markers, and end are legible.
- [ ] The illustrative/not-Physics-connected status is prominent and accessible.
- [ ] Share/remix/publish controls do not claim success when unavailable.
- [ ] Keyboard navigation and reduced-motion preferences are respected where the existing UI supports them.
- [ ] Existing build/test workflow passes.
- [ ] Report exact changed files and local run instructions, then identify precisely what Trent should click and evaluate.

## Stop conditions

Do not deploy, publish, release, change secrets, or trigger hosting. Do not overwrite the companion handoff merely to fold this addendum into it; it may be under concurrent edit. If WebPage's actual architecture makes the proposed entry incompatible, report the specific constraint and implement the smallest safe empty state instead of building a parallel navigation system.

**Desired outcome:** a real, inspectable browser prototype now, with honest labels and a clean replacement path to future Physics-backed playback—not a mock presented as finished physics.


## Implementation now present on `development`

A first browser prototype has now been added:

- [`ReplayLab.razor`](../../TheSingularityWorkshop/Pages/ReplayLab.razor) provides the `/replay-lab` route, a scene that responds to the selected tick, play/pause, tick stepping, an accessible range scrubber, event jumps, and an implementation-status panel.
- [The existing demonstrations gallery](../../TheSingularityWorkshop/Pages/Home.razor) now includes a **Replay Lab // Shareable Moments** card that opens the route. This reuses the existing gallery rather than creating a parallel navigation shell.
- The prototype uses local deterministic presentation data only. It prominently states that it is not connected to Physics world replay; share/remix/publish are not offered as working actions.
- The browser-host .NET test workflow was triggered by the commits. Confirm the latest run is green before treating this as build-verified. The Azure Static Web Apps workflow was skipped; no deployment was performed.

### Manual evaluation target

Open the Workshop, enter the existing hub, select **Demonstrations**, and choose **OPEN REPLAY LAB**. On `/replay-lab`, try the slider, each event row, **PLAY / PAUSE**, **± 1 TICK**, and **RESET**. Confirm the scene coordinates and selected event change as the tick changes. Review the persistent demo disclaimer and the three capability-status cards. Treat visual polish and browser behavior as awaiting Trent's inspection; the illustrative scene is not evidence of a physical simulation.
