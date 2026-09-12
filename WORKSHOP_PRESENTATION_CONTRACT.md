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
      | mature node doubles in size
      | mature node spawns a child
      | child animates parent -> chaotic destination
      | no shared spawn timing boundary is required
      | living GUI process group stops at exactly 100
      | NO MONIKER before 100
      v
100 GUI NODES / CRITICAL MASS
      |
      | freeze population
      | set MonikerReady = true
      | moniker is rendered behind the swarm
      v
MONIKER_REVEAL
      |
      | short sequencing boundary
      | moniker remains visible
      v
GRAVITY
      |
      | preallocated Gravity FSM process group starts
      | every GUI node falls away
      | moniker remains untouched behind it
      v
LIVING_GUI_DISSIPATING
      |
      | starts only after the final GUI node is gone
      | 91 x 33ms heartbeats ~= 3 seconds
      v
NAVIGATION_ARRIVAL
      |
      | browser chrome arrives
      | main panel contracts with ease-in/out motion
      v
RUNNING
```

## Non-negotiable rules

1. **100 nodes is the only reveal threshold.** The moniker must not be visually rendered before the living GUI reaches exactly `PageStateContext.CriticalMass` (currently 100).
2. **Critical mass opens the gate in the same state mutation.** Do not wait for Gravity to set `MonikerReady`.
3. **Critical mass stops population work.** The living GUI process group is no longer stepped after `LivingGuiFrozen` becomes true.
4. **Gravity is separately allocated.** The Gravity FSM handle/process group exists before the reveal and remains dormant until the page enters `GRAVITY`.
5. **The moniker is behind the swarm.** GUI nodes must visually occlude it while they are falling. Do not change the existing kelp-like moniker motion without an explicit visual request.
6. **The moniker persists after reveal.** It must not disappear during `MONIKER_REVEAL`, `GRAVITY`, `LIVING_GUI_DISSIPATING`, `NAVIGATION_ARRIVAL`, or `RUNNING`.
7. **The three-second delay begins after the fall completes.** It is not an arbitrary page-load animation timer.
8. **Navigation arrival is an eased layout transition.** The browser chrome may remain mounted, but it must not steal layout space until the arrival state; when it arrives, the panel contracts smoothly.
9. **The UI is not the authority for counting or timing.** Razor renders FSM/context state. It does not own the presentation sequence.
10. **The gateway does not visually present the moniker.** The button says `Enter Workshop`; the avatar is present, but the branding reveal is earned by critical mass.
11. **The gateway control is oversized.** On desktop it is 50% of viewport width and 50% of viewport height. The avatar must be clipped as a true circle using equal dimensions and `border-radius: 50%`; no square image inside an oval.
12. **Do not weaken tests to excuse a wrong sequence.** Fix the state/data flow.

## MicroBundle direction

Animations are candidates for MicroBundles rather than anonymous CSS timers. The current landing uses FSM_API process groups as the authoritative runtime mechanism; the next abstraction is to package reusable presentation behaviors as MicroBundles with providers for the host surface.

The Hub already models a nine-layer `OntologySignature`. A concrete MicroBundle address adds an integer variant slot:

```text
Nine-layer ontology
        +
VariantId [0, int.MaxValue - 1]
        = one globally addressable MicroBundle slot
```

One exact address has one occupant. Different variants can coexist under the same ontology. `MicroBundleRegistry` currently proves this uniqueness in-process; durable/public hosting is the next infrastructure layer.

## Source of truth

- `TheSingularityWorkshop/Services/PageStateContext.cs` owns population, critical mass, and gravity data.
- `TheSingularityWorkshop/Services/PageFSM.cs` owns state progression and isolated FSM_API work groups.
- `TheSingularityWorkshop/Services/FSMManagerService.cs` owns the application heartbeat; UI components do not tick the FSM.
- `TheSingularityWorkshop/Pages/LivingGui.razor` manifests the context and must not invent timing.
- `TheSingularityWorkshop/Layout/MainLayout.razor` gates final browser chrome arrival from FSM state.
- `TheSingularityWorkshop/Layout/MainLayout.razor.css` animates the final panel contraction.
- `SingularityHub.Tests/IdleExperienceArchitectureTests.cs` is the executable sequence contract.
- `SingularityHub.Tests/MicroBundleAddressTests.cs` is the address/uniqueness contract.
- `SingularityHub.Tests/IncrementalVersionTests.cs` is the required visible proof-of-work marker.

## Browser metadata

Do not casually leak `The Singularity Workshop` into initial first-contact metadata if the product decision is that the moniker should not appear before critical mass. The browser title and other immediately visible metadata are part of the presentation surface.

## Agent proof rule

Every meaningful change must advance the visible incremental version/test marker according to `AGENT_INCREMENTAL_RULE.md`. Report the commit SHA and marker version after repository work.
