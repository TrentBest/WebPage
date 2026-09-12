# Workshop Landing Presentation Contract

This is the canonical behavior contract for the active landing vertical slice.
When code, tests, screenshots, or older notes disagree, this document and the current
user-approved behavior take precedence.

## Branch safety

Active branch:

`feature/pong-microbundle-vertical-slice`

**Do not modify `master` for this work.**

## Exact presentation sequence

```text
PAGE_INITIALIZING
      |
      v
GATEWAY
      |
      | oversized Enter Workshop control
      | exactly 50% viewport width x 50% viewport height
      | avatar is an inscribed circle
      | no visible Workshop moniker
      v
GATEWAY_EXIT
      |
      v
LIVING_GUI_IGNITION
      |
      | create one centered seed
      v
LIVING_GUI_POPULATING
      |
      | reproduce / fly / grow
      | NO MONIKER
      v
100 GUI NODES / CRITICAL MASS
      |
      | freeze all population updates
      | set MonikerReady = true
      | moniker becomes visible now
      v
MONIKER_REVEAL
      |
      | sequencing boundary
      | moniker remains visible
      v
GRAVITY
      |
      | frozen swarm falls away
      | moniker remains visible
      v
NAVIGATION_ARRIVAL
      |
      v
RUNNING
```

## Non-negotiable rules

1. **100 nodes is the only reveal threshold.** The moniker must not be visually rendered before the living GUI reaches exactly `PageStateContext.CriticalMass` (currently 100).
2. **Critical mass opens the gate in the same state mutation.** Do not wait for Gravity to set `MonikerReady`.
3. **The moniker persists after reveal.** It must not disappear during `MONIKER_REVEAL`, `GRAVITY`, `NAVIGATION_ARRIVAL`, or `RUNNING`. It disappears only when the relevant page/component/context is discarded, such as changing away from the experience.
4. **The UI is not the authority for counting.** Razor renders `PageStateContext.MonikerReady`; `PageStateContext` owns the 100-node threshold.
5. **The gateway does not visually present the moniker.** The button says `Enter Workshop`; the avatar is present, but the branding reveal is earned by critical mass.
6. **The gateway control is oversized.** On desktop it is 50% of viewport width and 50% of viewport height. The avatar must be clipped as a true circle using equal dimensions and `border-radius: 50%`; no square image inside an oval.
7. **Do not weaken the tests to excuse a wrong sequence.** Fix the state/data flow.

## Source of truth

- `TheSingularityWorkshop/Services/PageStateContext.cs` owns population and critical mass.
- `TheSingularityWorkshop/Services/PageFSM.cs` owns state progression.
- `TheSingularityWorkshop/Pages/LivingGui.razor` manifests the context and must not invent timing.
- `SingularityHub.Tests/IdleExperienceArchitectureTests.cs` is the executable sequence contract.
- `SingularityHub.Tests/IncrementalVersionTests.cs` is the required visible proof-of-work marker.

## FSM ownership

Each `PageFSM` owns a private FSM_API processing group. `PageFSM.Update()` ticks only that group so one page instance cannot advance another. FSM_API owns the `OnEnter`/`HasEnteredCurrentState` lifecycle.

## Browser metadata

Do not casually leak `The Singularity Workshop` into initial first-contact metadata if the product decision is that the moniker should not appear before critical mass. The browser title and other immediately visible metadata are part of the presentation surface.

## Agent proof rule

Every meaningful change must advance the visible incremental version/test marker according to `AGENT_INCREMENTAL_RULE.md`. Report the commit SHA and marker version after repository work.
