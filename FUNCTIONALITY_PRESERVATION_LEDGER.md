# Functionality Preservation Ledger

**Branch:** `development`  
**Purpose:** archaeology and preservation before housekeeping removes or relocates implementation.

This ledger is the migration gate for the Experience/MicroBundle rebuild. **No implementation is deleted merely because it looks superseded.** First locate the behavior, identify its architectural boundary, preserve it with tests, move/adapt it, update documentation, and only then remove obsolete duplication.

## Canonical target

```text
WebPage host manifest
        |
        v
      Hub
        |
        +--> MicroBundle / Experience registry
        |
        +--> select available Idler
        |       |
        |       +--> Pong (first concrete Idler)
        |
        +--> user: Enter Workshop
                |
                +--> select available Flex
                        |
                        +--> Living GUI (first concrete Flex)
```

The bootstrap protocol must select by capability/inventory, not by hardcoded experience names. AI/Grammar/Protocol remains a future production pipeline and is not a prerequisite for the deterministic runtime.

## Preservation matrix

| Area | Current implementation | Intended boundary | Coverage / evidence | Status |
|---|---|---|---|---|
| Experience contract | `TheSingularityWorkshop/Workshop/Experiences/IExperience.cs` | Experience model | `Experiences/README.md`, Experience contract tests | **Retain; evolve** |
| Experience theory | `EXPERIENCE_THEORY.md` | Architectural contract | Experience tests + incremental heartbeat | **Retain; reconcile** |
| MicroBundle contract | `Workshop/MicroBundles/IMicroBundle.cs` | MicroBundle capability unit | MicroBundle tests/docs | **Retain** |
| MicroBundle provider boundary | `Workshop/MicroBundles/IMicroBundleProvider.cs`, `WebMicroBundleProvider.cs` | Provider/registry boundary | Provider/MicroBundle tests | **Retain; inspect** |
| MicroBundle composition | `MicroBundle.cs`, `MicroBundleContext.cs`, `MicroBundleManifestation.cs` | Runtime MicroBundle model | MicroBundle tests/docs | **Retain; inspect** |
| Pong MicroBundle | `PongMicroBundle.cs` | Concrete Idler capability | `PongExperience.Tests` + MicroBundle tests | **Retain; migrate into registry selection** |
| Hub registry boundary | `SingularityHub/MicroBundleRegistry.cs` + `SingularityHub.Abstractions/MicroBundleAddress.cs` | Discovery/inventory | registry/address tests | **Retain; make runtime path canonical** |
| Idle catalog | `Services/IdleExperienceCatalog.cs` | Registry capability query/selection | existing catalog/Experience tests | **Transitional; preserve behavior, replace static inventory** |
| Flex catalog | `Services/FlexExperienceCatalog.cs` | Registry capability query/selection | existing catalog/Experience tests | **Transitional; preserve behavior, replace static inventory** |
| Workshop demo bundle | `Infrastructure/Hub/WorkshopDemoBundle.cs` | Host manifest/registry bootstrap proof | Hub composition tests | **Transitional; do not delete until replaced** |
| Hub host runtime | `Infrastructure/Hub/HubRuntime.cs` | Host composition -> Hub lifecycle | Hub composition tests | **Retain; remove hardcoded bundle only after registry path exists** |
| FSM manager | `Infrastructure/FSM/FSMManagerService.cs` | Host adapter to Hub-owned scheduler | `LivingGuiRuntimeTests`, incremental heartbeat | **Retain; reduce page ownership** |
| Page FSM | `Infrastructure/FSM/PageFSM.cs` | Presentation/lifecycle adapter | landing + Living GUI tests | **Transitional; preserve lifecycle semantics while moving orchestration to Hub** |
| Living GUI FSM | `Infrastructure/FSM/LivingGuiFsm.cs` | Concrete Flex runtime/process groups | `LivingGuiRuntimeTests` | **Retain; attach to Experience/Flex boundary** |
| Living GUI state | `Infrastructure/FSM/PageStateContext.cs` | Runtime state for concrete Flex | Living GUI tests | **Retain; decouple from page semantics where practical** |
| Living GUI presentation | `Components/LivingGui.razor` | Blazor presentation adapter | runtime rendering/heartbeat tests | **Retain; presentation only** |
| Landing host | `Home.razor` and related landing components | Blazor adapter consuming Hub state | landing tests | **Retain; remove hardcoded Experience selection** |
| Idle host | `Components/IdleExperienceHost.razor` | Presentation of selected Idler | Pong/landing tests | **Retain; bind to Hub selection** |
| Flex host | `Components/FlexExperienceHost.razor` | Presentation of selected Flex | landing/Experience tests | **Retain; bind to Hub selection** |
| Workshop feed | `Components/WorkshopExperienceFeed.razor` | Experience inventory/presentation | component tests if present | **Inspect; avoid duplicate registry ownership** |
| Moniker lifecycle | landing/PageFSM implementation | Presentation state after Flex selection | incremental heartbeats for arrival/3-second behavior | **Preserve; move source of truth toward Hub** |
| AI pipeline | historical AI/Grammar/Protocol work | Future deterministic producer/manifest pipeline | historical branches/docs | **Retain; deferred, not runtime prerequisite** |

## Current findings

### 1. The Experience model already exists

`IExperience` already carries identity, name, version, ontology, MicroBundle IDs, capabilities, sensory-system IDs, and processing-group IDs. It therefore provides a natural contract for the next registry-driven step rather than being replaced by another page-specific abstraction.

### 2. The static catalogs are the most obvious migration seam

`IdleExperienceCatalog` currently contains a single hardcoded `pong` definition. `FlexExperienceCatalog` currently contains a single hardcoded `living-gui` definition. These files are useful evidence of the behavior we need to preserve, but they are not the desired source of truth. Their selection semantics should migrate behind the Hub's Experience/MicroBundle inventory boundary.

### 3. `WorkshopDemoBundle` is transitional host composition

`HubRuntime` currently loads `WorkshopDemoBundle` directly. That proves Hub composition, but it does not yet implement the desired manifest -> registry -> discovery flow. It must remain until the replacement path can load the same required behavior and tests demonstrate parity.

### 4. The concrete runtime is already test-backed

The Living GUI has dedicated runtime tests covering the root/nested lifecycle and Hub/FSM composition. Pong and Living GUI also have dedicated Experience test projects. These tests are the preservation anchors for migration.

### 5. `EXPERIENCE_THEORY.md` needs reconciliation

The document still contains historical incremental-heartbeat references (`0.0.17` / `0.00.018`) that no longer describe the repository's current `0.0.48` heartbeat. This is documentation drift, not evidence that the Experience model should be discarded. Reconcile it in a dedicated documentation change.

## Migration rules

1. **No deletion before mapping.**
2. **No hardcoded Experience names in bootstrap.**
3. **The registry is inventory/discovery; the manifest describes where/how the host connects.**
4. **The Hub owns runtime lifecycle and FSM/API stepping.**
5. **Blazor renders Hub state; it does not become a second runtime.**
6. **Pong remains the first concrete Idler.**
7. **Living GUI remains the first concrete Flex.**
8. **AI/Grammar/Protocol remains preserved as a later production pipeline.**
9. **Every meaningful migration advances the incremental version heartbeat and adds executable proof.**
10. **`master` remains untouched.**

## Next migration gates

- [ ] Reconcile `EXPERIENCE_THEORY.md` with the current architecture.
- [ ] Define the host manifest contract and prove it can identify the registry endpoint without naming a concrete Experience.
- [ ] Add Hub-side Experience/Capability discovery and deterministic/random selection tests.
- [ ] Move Pong selection from `IdleExperienceCatalog` to the registry boundary while preserving behavior.
- [ ] Move Living GUI selection from `FlexExperienceCatalog` to the registry boundary while preserving behavior.
- [ ] Replace direct `WorkshopDemoBundle` loading with manifest/registry composition.
- [ ] Move moniker/page transition ownership out of page-specific bootstrap logic as the Hub lifecycle becomes authoritative.
- [ ] Only after all gates pass, remove obsolete static catalogs/demo composition if no tests or runtime paths depend on them.
