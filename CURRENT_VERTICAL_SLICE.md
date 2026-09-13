# Current Vertical Slice — Manifest → Hub → Idler → Flex

Read this before changing the active landing experience.

## Working branch

`development`

`master` remains the protected promotion target. Do not modify it during active development.

## Current objective

Build a deterministic, FSM_API-driven Workshop landing experience in which the **host is generic and the content is discovered**.

The page launches the Hub with a WebPage host manifest. The Hub uses that manifest to locate the MicroBundle/Experience registry, discovers eligible Experiences, selects an Idler, and later selects a Flex when the visitor enters the Workshop.

The first concrete implementations are Pong and Living GUI. They are examples of the protocol, not hardcoded protocol names.

## Intended presentation sequence

```text
WEBPAGE BOOT
  -> load host manifest
  -> start Hub
  -> connect MicroBundle / Experience registry
  -> discover available Idlers and Flexes
  -> randomly select an available Idler
  -> RUN IDLER (Pong is the first concrete one)
  -> user clicks Enter Workshop
  -> randomly select an available Flex
  -> RUN FLEX (Living GUI is the first concrete one)
  -> present Workshop moniker for 3 seconds
  -> reveal navigation / page chrome / primary panel
  -> moniker continues in the primary panel
  -> user selects a tab
  -> clear moniker
  -> present selected GUI / Experience
```

The exact visual Living GUI lifecycle remains an important demonstration inside that larger Experience lifecycle. Its internal sequence is:

```text
LIVING_GUI_IGNITION
  -> LIVING_GUI_POPULATING
       root grows
       children root
       children grow
       reproduction continues
  -> exactly 100 nodes
  -> freeze Living GUI group
  -> reveal moniker behind GUI
  -> start preallocated Gravity group
  -> all GUI nodes fall away
  -> begin three-second dissipating phase only after the last node falls
  -> ease navigation/page chrome into existence
```

## Non-negotiable architectural rules

1. The WebPage manifest describes the host, not the concrete Experience inventory.
2. The Hub discovers MicroBundles/Experiences through the registry boundary.
3. Idler and Flex are capabilities/categories, not special hardcoded classes.
4. The first implementations are Pong as Idler and Living GUI as Flex; replacing either must not require rewriting the bootstrap contract.
5. The Hub owns runtime lifecycle and the application heartbeat.
6. Experiences expose process groups; they do not independently step the global runtime.
7. Blazor components render authoritative Hub/FSM state; they do not become the state machine.
8. FSM_API remains the execution mechanism for meaningful lifecycle/state behavior.
9. Do not replace a working capability with a simpler implementation until the behavior has been located and preserved elsewhere.
10. AI/Grammar/Protocol work remains retained future architecture, but the runtime must operate without it.

## Current transitional implementation

The branch still contains static `IdleExperienceCatalog`, `FlexExperienceCatalog`, and `WorkshopDemoBundle` scaffolding. These are not yet the final registry-driven content boundary.

Do not delete them merely because they are hardcoded. First move their functionality into the Experience/MicroBundle/registry model, prove the replacement, and only then remove the obsolete implementation.

## Important files

| File | Responsibility |
|---|---|
| `WORKSHOP_RUNTIME_ARCHITECTURE.md` | Canonical host/manifest/registry/Hub workflow |
| `EXPERIENCE_THEORY.md` | Experience definition and runtime contract |
| `TheSingularityWorkshop/Workshop/MicroBundles/README.md` | MicroBundle lifecycle/addressing/provider theory |
| `TheSingularityWorkshop/Workshop/Experiences/IExperience.cs` | Experience contract |
| `TheSingularityWorkshop/Services/IdleExperienceCatalog.cs` | Transitional Idler catalog; replace through registry |
| `TheSingularityWorkshop/Services/FlexExperienceCatalog.cs` | Transitional Flex catalog; replace through registry |
| `TheSingularityWorkshop/Infrastructure/Hub/HubRuntime.cs` | Web host composition adapter for Hub |
| `TheSingularityWorkshop/Infrastructure/Hub/WorkshopDemoBundle.cs` | Transitional Hub composition proof |
| `TheSingularityWorkshop/Services/PageStateContext.cs` | Living population, critical mass, gravity state |
| `TheSingularityWorkshop/Services/PageFSM.cs` | Transitional page/landing orchestration |
| `TheSingularityWorkshop/Services/FSMManagerService.cs` | Application heartbeat |
| `TheSingularityWorkshop/Pages/Home.razor` | Landing presentation adapter |
| `TheSingularityWorkshop/Pages/LivingGui.razor` | Living GUI manifestation |
| `SingularityHub.Abstractions/MicroBundleAddress.cs` | Ontology + integer variant address |
| `SingularityHub/MicroBundleRegistry.cs` | Runtime uniqueness boundary |
| `SingularityHub.Tests/IncrementalVersionTests.cs` | Visible incremental proof marker |
| `AGENT_INCREMENTAL_RULE.md` | Mandatory change/proof rule |

## Registry direction

The desired runtime boundary is:

```text
WebPage host manifest
        |
        v
      Hub
        |
        v
MicroBundle / Experience registry
        |
        +--> discover
        +--> filter by capability
        +--> select
        +--> resolve version/dependencies
        +--> load
        +--> arbitrate
        v
Experience runtime
        |
        v
FSM_API execution
```

The registry may initially be local/test-backed. The architectural contract should not assume that it is. The eventual public catalog can live behind the same address and manifest boundary.

## AI direction

AI is deliberately held for a later development phase. Keep the AI/Grammar/Protocol work and its documentation; do not make it a prerequisite for the landing runtime.

The future pipeline is expected to feed deterministic commands/selections into the Experience/MicroBundle boundary rather than replacing Hub ownership of runtime semantics.

## Mandatory agent behavior

Every meaningful repository change must:

1. make the actual requested change;
2. add/update an incremental visible unit-test/version proof;
3. advance `IncrementalVersionTests.CurrentVersion`;
4. keep the assertion meaningful;
5. report the commit SHA and version;
6. verify the current CI/test result before claiming success.

Do not change `master`.
