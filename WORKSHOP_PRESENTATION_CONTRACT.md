# Workshop Landing Presentation Contract

This is the canonical behavior contract for the active landing vertical slice.
When code, tests, screenshots, or older notes disagree, this document and the current
user-approved behavior take precedence.

## Branch safety

Active branch:

`development`

**Do not modify `master` during ordinary development.** The repository is intentionally
being consolidated to `master` plus `development`; this contract follows the active branch.

## Exact presentation sequence

```text
PAGE_INITIALIZING
      |
      v
GATEWAY
      |
      | label 1
      | label 2
      | oversized Enter Workshop control
      | exactly 50% viewport width x 50% viewport height
      | avatar is an inscribed circle
      | no visible Workshop moniker
      v
GATEWAY_EXIT
      |
      | visitor has explicitly granted runtime permission
      v
FSM_COS_COMPOSE_EXPERIENCE
      |
      | resolve Living GUI Experience
      | resolve Moniker MicroBundle dependency first
      | only after successful composition may the runtime start
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
      | reproduction continues until the configured critical-mass threshold of 64 organisms
      v
FIRST REPRODUCTION / MONIKER READY
      |
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

1. **Critical mass is the opening handoff.** The living GUI reaches the configured 64-organism observation threshold before the moniker can be revealed.
2. **The opening handoff does not stop life.** The living GUI continues reproducing while the moniker is presented.
3. **Gravity is the explicit freeze boundary.** The living GUI process group remains active until the page enters `GRAVITY`, where `LivingGuiFrozen` is set before gravity advances.
4. **Gravity is separately allocated.** The Gravity FSM handle/process group exists before the reveal and remains dormant until the page enters `GRAVITY`.
5. **The moniker is behind the swarm.** GUI nodes must visually occlude it while they are falling. Do not change the existing kelp-like moniker motion without an explicit visual request.
6. **The moniker persists after reveal.** It must not disappear during `MONIKER_REVEAL`, `GRAVITY`, `LIVING_GUI_DISSIPATING`, `NAVIGATION_ARRIVAL`, or `RUNNING`.
7. **The three-second delay begins after the fall completes.** It is not an arbitrary page-load animation timer.
8. **Navigation arrival is an eased layout transition.** The browser chrome may remain mounted, but it must not steal layout space until the arrival state; when it arrives, the panel contracts smoothly.
9. **The UI is not the authority for counting or timing.** Razor renders FSM/context state. It does not own the presentation sequence.
10. **The gateway does not visually present the moniker.** The first two labels and the warning card establish first contact; the button says `Enter Workshop`. The visitor's click is explicit permission to compose and run the selected Experience.
11. **FSM_COS owns the Experience handoff.** The Living GUI Experience is composed only after entry permission. Its MicroBundle declares the canonical Moniker dependency, so the Moniker is installed by the composition system before the Living GUI runtime is started.
12. **The gateway control is oversized.** On desktop it is 50% of viewport width and 50% of viewport height. The avatar must be clipped as a true circle using equal dimensions and `border-radius: 50%`; no square image inside an oval.
13. **Do not weaken tests to excuse a wrong sequence.** Fix the state/data flow.

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


## First-contact boundary

The pre-entry presentation is intentionally separate from the Living GUI lifecycle:

```text
FIRST CONTACT FSM
  |
  +-- "THEY SAY A PICTURE IS WORTH A THOUSAND WORDS."
  |
  +-- "HOW MANY WORDS IS A LIVING IMAGE WORTH?"
  |
  +-- ENTER THE WORKSHOP + advisory
  |
  v
LANDING HANDOFF
  |
  v
FSM_COS-COMPOSED EXPERIENCE
  |
  +-- Living GUI runtime starts
  +-- first reproduction makes moniker eligible
  +-- reproduction continues without a cap
  +-- gravity is the explicit freeze boundary
  +-- moniker behind population
  +-- gravity FSM
  +-- nodes fall away
  +-- moniker persists
  +-- navigation / Hub becomes available
```

The first-contact FSM owns only the two-label invitation and gateway. It does not compose the Experience or own the Moniker. After explicit entry permission, FSM_COS composes the selected Experience and resolves its MicroBundle dependencies. The Page FSM then owns the Living GUI lifecycle and persistent Moniker reveal. This prevents two presentation state machines from fighting over the same identity.

The two opening statements are presentation content, not implementation claims. They are followed by the explicit human choice to enter the Workshop.
