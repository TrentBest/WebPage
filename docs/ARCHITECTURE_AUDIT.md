# Architecture Audit — October 2026

![Living GUI documentation lens: question, model, implementation, visible manifestation, and proof](https://raw.githubusercontent.com/TrentBest/WebPage/development/docs/assets/living-gui-documentation-lens.svg)

> **Evidence class:** architecture illustration, not live runtime telemetry. Follow the subject-specific source, view, fixture, or test below; if none exists, record that gap rather than implying it is implemented.


## Reference standard

FSM_API is the reference implementation for deterministic lifecycle behavior.

WebPage consumes FSM_API and the other Workshop packages through their published/package boundaries. It must not grow competing runtime kernels.

## Current architectural position

The repository is being rehabilitated from its original website-wrapper purpose into a browser proving ground and public learning resource.

The authoritative direction is:

~~~text
host manifest
    ↓
startup presentation Experience(s)
    ↓
primary running Experience
    ↓
FSM_COS RuntimeManifest
    ↓
dependency closure / arbitration / RuntimeAssembly
    ↓
FSM_API execution
    ↓
WebPage presentation
~~~

## Confirmed aligned boundaries

- FSM_API owns meaningful state-machine execution.
- FSM_COS owns composition, dependency closure, arbitration, convergence, and RuntimeAssembly creation.
- MicroBundleDomain owns the MicroBundle domain contract.
- WebPage owns browser presentation, manifest loading, host adapters, and proving-ground integration.
- The active branch now has a **manifest-bound host execution adapter**, but has **not** completed package-owned execution migration: `PageFsmExecutionRuntime` still delegates to the preserved `PageFSM`/`LivingGuiFsm` implementation.
- Reusable capabilities are consumed through Workshop NuGet packages when a package already owns them.
- WebPage explicitly references `FSM_API`, `FSM_COS`, and `MicroBundleDomain`; package versions must remain on published versions unless a newer package is explicitly released and approved.
- The Living GUI primary MicroBundle declares the canonical Moniker dependency.

## Current composition proof

The current manifest requests primary root 2102.

~~~text
2102 LivingGuiExperienceMicroBundle
        ↓
declared dependency
        ↓
2110 MonikerMicroBundle
~~~

WebPage must not request 2110 separately just because it is presented before the primary Experience.

## Source-backed living evidence map

These links are the current implementation/evidence trail, not a claim that the composition-to-execution migration is finished.

| Claim / boundary | Source of truth | Evidence available now | Boundary still to prove |
|---|---|---|---|
| Host manifest declares startup, primary Experience, and capabilities | [Host manifest](../TheSingularityWorkshop/wwwroot/Workshop/Forge/StreamingAssets/Experiences/webpage-host.manifest.json) | Versioned JSON input; startup Moniker is bundle 2101 and primary Living GUI is bundle 2102 | Manifest selection and composition do not alone prove the selected primary Experience owns all browser execution |
| WebPage requests composition and binds the selected Experience | [WorkshopExperienceService](../TheSingularityWorkshop/Services/WorkshopExperienceService.cs) and [ExperienceExecutionRuntime](../TheSingularityWorkshop/Services/ExperienceExecutionRuntime.cs) | Primary bundle IDs resolve to one registered `IExperience`; the adapter validates required MicroBundle closure against `RuntimeAssembly` and exposes binding status | The adapter still delegates to PageFSM; RuntimeAssembly itself is not an executing lifecycle |
| Browser surface is a presentation composed by the host | [Home.razor](../TheSingularityWorkshop/Pages/Home.razor) and [LivingGui.razor](../TheSingularityWorkshop/Pages/LivingGui.razor) | Home and Living GUI read lifecycle/context through `FSM.ExecutionRuntime`; manager compatibility properties delegate through the same adapter | The adapter still uses PageFSM as its implementation; package-owned lifecycle and behavior parity remain unproven |
| Page progression is driven through the active execution adapter | [FSMManagerService](../TheSingularityWorkshop/Services/FSMManagerService.cs), [ExperienceExecutionRuntime](../TheSingularityWorkshop/Services/ExperienceExecutionRuntime.cs), and [PageFSM](../TheSingularityWorkshop/Services/PageFSM.cs) | The 33 ms heartbeat calls `ExecutionRuntime.Tick()`; the current adapter forwards to `PageFSM.Update()` and its observation snapshot | Replace the compatibility delegate only after a package-owned runtime passes behavior-parity tests |
| Living GUI population is independently stateful | [LivingGuiFsm](../TheSingularityWorkshop/Services/LivingGuiFsm.cs) | Organism FSM pool and lifecycle are visible in source | Runtime visualization must bind to current context/state, not to a hand-authored depiction |
| Lifecycle and reproduction have deterministic test evidence | [LivingGuiRuntimeTests](../SingularityHub.Tests/LivingGuiRuntimeTests.cs), [LivingGuiReproductionTests](../SingularityHub.Tests/LivingGuiReproductionTests.cs) | Tests assert root/child creation, independent lifecycle phases, growth/reproduction and position inheritance | These tests establish code-level behavior; they do not by themselves prove the browser's rendered state matches every runtime observation |

**How to read this map:** code links are implementation evidence; test links are executable behavioral evidence; the shared SVG above is only an architecture illustration. The on-demand [RuntimeObservationPanel](../TheSingularityWorkshop/Components/RuntimeObservationPanel.razor) is implemented and browser-tested; it shows manifest/Experience identity, composition, adapter binding, page-FSM state, Living GUI population/phase, and source/test links. It reports the remaining execution-ownership boundary honestly.


## Transitional areas

The repository still contains older page-FSM, first-contact, catalog, demo, and exploration implementations.

These are migration candidates, not automatically deletion candidates.

For each:

1. locate behavior;
2. identify canonical owner;
3. preserve with executable proof;
4. replace the host implementation;
5. verify package/runtime behavior;
6. remove duplicate code.

## Function-preservation gate

The existing Living GUI runtime is not disposable scaffolding. Its tests define behavior that must survive extraction. See [`FUNCTIONALITY_PRESERVATION.md`](FUNCTIONALITY_PRESERVATION.md).

The audit must not be marked complete merely because the old classes compile or because they have been deleted. Completion requires the same observable behavior under the canonical package/runtime path.

## Known cleanup targets

1. Old gateway/first-contact lifecycle documentation.
2. Static Idler/Flex selection scaffolding.
3. WorkshopDemoBundle bootstrap scaffolding.
4. Duplicate host lifecycle state where FSM_API already owns the behavior.
5. `Workshop/MicroBundles/MicroBundle.cs` and its local `IMicroBundle`/context/manifestation scaffolding: historical lifecycle machinery that must not become a second MicroBundle runtime. Migrate concrete demos to the published FSM_COS + MicroBundleDomain contracts as their capabilities become current.
6. Local catalogs/providers that duplicate package-owned MicroBundle discovery or composition responsibilities.
7. Transitional Living GUI runtime once its canonical package replacement is proven.
8. Stale platform-specific documentation.
9. Root-level documentation duplication.

## Documentation rule

A document is current only when its claims match code and tests.

Historical ideas may remain, but they must be labeled historical or future and must not be presented as the active runtime contract.

## Completion standard

The audit is satisfied when:

- package ownership is explicit;
- manifest startup and primary Experience semantics are unambiguous;
- FSM_COS is the only composition engine;
- FSM_API is the lifecycle authority;
- browser components are manifestations, not hidden runtime owners;
- current documentation agrees with tests and running behavior.
