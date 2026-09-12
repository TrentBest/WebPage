# Current Vertical Slice — Pong + Living Workshop Landing

Read this before changing the active experience.

## Working branch

`feature/pong-microbundle-vertical-slice`

Do not modify `master`.

## Current objective

Publish a deterministic, FSM_API-driven landing experience that demonstrates the technology through the experience itself.

## Presentation sequence

The gateway is the only first-contact surface. The Workshop moniker is absent until critical mass.

```text
PAGE_INITIALIZING
  -> GATEWAY
  -> user clicks Enter Workshop
  -> GATEWAY_EXIT
  -> LIVING_GUI_IGNITION
  -> LIVING_GUI_POPULATING
       one seed at center
       each mature GUI doubles in size
       mature GUI spawns a child
       child animates from parent to a chaotic destination
       process group stops at exactly 100
  -> MONIKER_REVEAL
       moniker is behind the frozen GUI
  -> GRAVITY
       preallocated Gravity FSM process group starts
       GUI nodes fall away
       moniker remains untouched and visible
  -> LIVING_GUI_DISSIPATING
       begins only after every GUI node has fallen away
       91 FSM heartbeats at 33ms ~= 3 seconds
  -> NAVIGATION_ARRIVAL
       browser chrome and page structure ease into existence
       content panel contracts with the chrome
  -> RUNNING
```

### Non-negotiable presentation rules

1. The moniker does not exist before exactly 100 living GUI nodes.
2. Exactly 100 freezes the living GUI process group.
3. Moniker reveal is independent of Gravity timing.
4. Gravity is a separate, already allocated FSM_API process group.
5. The moniker remains behind the falling GUI; do not redesign its kelp-like oscillation without an explicit visual request.
6. The three-second delay begins only after the last GUI node has fallen away.
7. Navigation/page chrome arrival is eased; it must not hard-snap the panel to its final size.
8. UI components render FSM state. They do not own state transitions or authoritative timers.

## Important files

| File | Responsibility |
|---|---|
| `WORKSHOP_PRESENTATION_CONTRACT.md` | Canonical behavior contract |
| `CURRENT_VERTICAL_SLICE.md` | Active handoff map for the next agent |
| `TheSingularityWorkshop/Services/PageStateContext.cs` | Living population, exact critical mass, gravity physics |
| `TheSingularityWorkshop/Services/PageFSM.cs` | Page FSM plus isolated LivingGui and Gravity scheduler groups |
| `TheSingularityWorkshop/Services/FSMManagerService.cs` | Application heartbeat; UI does not own stepping |
| `TheSingularityWorkshop/Pages/Home.razor` | Gateway presentation |
| `TheSingularityWorkshop/Pages/LivingGui.razor` | Living GUI manifestation and moniker layer |
| `TheSingularityWorkshop/Layout/MainLayout.razor` | Final chrome arrival gate |
| `TheSingularityWorkshop/Layout/MainLayout.razor.css` | Ease-in/out panel contraction when chrome arrives |
| `TheSingularityWorkshop/Workshop/MicroBundles/MicroBundle.cs` | Runtime MicroBundle lifecycle shell |
| `TheSingularityWorkshop/Workshop/MicroBundles/PongMicroBundle.cs` | First concrete MicroBundle |
| `SingularityHub.Abstractions/HubContracts.cs` | Nine-layer ontology and Hub contracts |
| `SingularityHub.Abstractions/MicroBundleAddress.cs` | Ontology + integer variant address |
| `SingularityHub/MicroBundleRegistry.cs` | One-occupant-per-address registry rule |
| `SingularityHub.Tests/IdleExperienceArchitectureTests.cs` | Executable presentation sequence |
| `SingularityHub.Tests/MicroBundleAddressTests.cs` | Address capacity and uniqueness proof |
| `SingularityHub.Tests/IncrementalVersionTests.cs` | Visible incremental proof marker |
| `AGENT_INCREMENTAL_RULE.md` | Mandatory agent proof rule |

## MicroBundle addressing direction

The existing Hub already has a nine-layer `OntologySignature` and a `StructuralId`. The next layer is deliberately separate:

```text
OntologySignature (9 integer layers)
        +
VariantId (0 .. int.MaxValue - 1)
        =
MicroBundleAddress
```

For each exact address there is one occupant. Different VariantIds may coexist under the same ontology. This gives each ontology coordinate `int.MaxValue` addressable variant slots without turning the ontology itself into a unique-instance identifier.

`MicroBundleRegistry` is currently the runtime uniqueness boundary. The next hosting step is to back the same contract with a durable/public catalog so a published MicroBundle becomes addressable to every Workshop host, not just the current process.

Do not invent a URL scheme before the address contract and persistence semantics are settled.

## Known trap: CSS precedence

The landing has both component-local styles and `wwwroot/css/workshop-landing.css`. The global stylesheet previously contained competing `!important` gateway rules. If a component appears to ignore its own geometry, inspect the global landing stylesheet before changing markup repeatedly.

## Mandatory agent behavior

Every meaningful repository change must:

1. make the actual requested change;
2. add/update an incremental visible unit-test/version proof;
3. advance `IncrementalVersionTests.CurrentVersion`;
4. keep the assertion meaningful;
5. report the commit SHA and version;
6. verify the current CI/test result before claiming success.

Do not change `master`.
