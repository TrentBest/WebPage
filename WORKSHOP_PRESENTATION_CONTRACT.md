# Workshop Landing Presentation Contract

This document is the executable-vision companion to `PageFSM` and the landing-page tests.
It exists to prevent a future agent from turning the front door into a conventional hero page or revealing the Workshop moniker before the experience has earned it.

## The first-contact contract

The landing is a stateful experience. The visual surface is a manifestation of the page FSM; it is not the owner of the sequence.

```text
PAGE_INITIALIZING
      |
      v
GATEWAY
      |
      | visitor clicks Enter Workshop
      v
GATEWAY_EXIT
      |
      | one heartbeat
      v
LIVING_GUI_IGNITION
      |
      | one heartbeat
      | create exactly one centered seed
      v
LIVING_GUI_POPULATING
      |
      | reproduce / fly / grow
      | no moniker
      v
100 GUI NODES / CRITICAL MASS
      |
      | freeze
      v
MONIKER_REVEAL
      |
      | sequencing boundary only
      | still no visible moniker frame
      v
GRAVITY
      |
      | instantiate and visibly present
      | THE SINGULARITY WORKSHOP
      | 91 FSM heartbeats ~= 3 seconds
      v
NAVIGATION_ARRIVAL
      |
      v
RUNNING
```

## Non-negotiable presentation rules

1. **The initial gateway does not visually present the Workshop moniker.**
   The visitor sees the large `Enter Workshop` control and its avatar, not a branding wall.
2. **Entering the Workshop does not reveal the moniker.**
   `GATEWAY_EXIT`, `LIVING_GUI_IGNITION`, and `LIVING_GUI_POPULATING` are part of the experience.
3. **The living GUI must reach exactly 100 nodes before `MONIKER_REVEAL`.**
   Critical mass is the narrative trigger.
4. **`MONIKER_REVEAL` is a state boundary, not the visual presentation frame.**
   Entering this state does not set `PageStateContext.MonikerReady`. The state exists so the FSM exposes a deliberate boundary between critical mass and visible branding. The next heartbeat enters `GRAVITY`.
5. **Gravity owns the visible moniker window.**
   `PageStateContext.MonikerReady` becomes true when `GRAVITY` is entered. The frozen GUI swarm falls away beneath the moniker for 91 FSM heartbeats, approximately three seconds at the 33 ms page heartbeat.
6. **Navigation chrome arrives later.**
   The landing remains immersive until `NAVIGATION_ARRIVAL` / `RUNNING`.
7. **Do not solve a failing sequence test by weakening the sequence.**
   The sequence test is the contract. Fix the machine/lifecycle that violates it.

## Architecture rule: one PageFSM, one owned handle

`FSM_API` stores definitions and instances in process-global processing groups. `PageFSM` therefore treats its `FSMHandle` as the owner of its update operation.

`PageFSM.Update()` must advance **that handle**, not the entire `PageFSM` processing group. Group-wide ticking is an integration concern; it must not make one page wrapper accidentally advance another page instance.

There is one important lifecycle detail: `FSMHandle.Update()` drives the definition's `Step()` directly, while the normal FSM_API scheduler is also responsible for entering a state when `HasEnteredCurrentState` is false. Therefore `PageFSM.Update()` performs that single `OnEnter` step for its owned handle before calling `FSMHandle.Update()`. This preserves the scheduler's state-entry semantics without reintroducing process-global group ticking.

`PageFSM.Dispose()` must unregister its handle.

This matters because FSM definitions contain delegates. Rebuilding a process-global definition with instance-bound delegates can otherwise make an unrelated handle execute another `PageFSM` instance's behavior.

## What the UI may and may not decide

The UI may decide how a state looks.

The UI may not decide when the state changes.

In particular:

- no JavaScript timer should become the authority for the moniker reveal;
- no Razor-only counter should become the authority for critical mass;
- no CSS animation should be used as a substitute for the FSM transition;
- no branding text should be added to the initial gateway merely because the page title contains the Workshop name.

## Tests are breadcrumbs

`Incremental Unit Test 04` is the canonical presentation-sequence test.

`Incremental Unit Test 07` proves PageFSM disposal removes its runtime handle.

`Incremental Unit Test 08` proves calling one `PageFSM.Update()` does not advance another PageFSM instance.

`Incremental Unit Test 09` independently proves the moniker remains hidden through `MONIKER_REVEAL` and becomes ready only in `GRAVITY`.

The incremental version heartbeat in `SingularityHub.Tests/IncrementalVersionTests.cs` is the visible proof marker for coherent repository changes. The current marker is `0.0.12` for the scheduler-lifecycle and moniker-gate correction.

## Current branch safety

The active vertical slice is:

`feature/pong-microbundle-vertical-slice`

`master` is not the working branch. Do not modify `master` while developing this experiment.
