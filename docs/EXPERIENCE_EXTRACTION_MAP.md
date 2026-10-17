# WebPage → Experiences Extraction Map

**Branch:** `development`  
**Status:** source-backed migration map; no legacy implementation is removed by this document.

## Objective

Make WebPage a deliberately thin manifestation host. Experience semantics, visitor journeys, demonstration metadata, and reusable behavior belong in TheSingularityWorkshop.Experiences or their owning MicroBundles. WebPage should retain browser lifecycle, route/URL adaptation, input, capability detection, GUI presentation, diagnostics, and explicit user-controlled Experience selection.

This map exists to prevent two opposite mistakes: leaving a second copy of Experience behavior in WebPage, or deleting a working path before its Experience-owned replacement is actually loaded and tested.

## Source ownership and current duplication

| Concern | Current WebPage source | Canonical owner/source | Extraction rule |
|---|---|---|---|
| Showcase identity, startup/running roots, capabilities | `TheSingularityWorkshop/wwwroot/Workshop/Forge/StreamingAssets/Experiences/webpage-host.manifest.json` | [Experiences WorkshopShowcase manifest](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/blob/development/experiences/WorkshopShowcase/manifest.json) | Keep host-specific boot/URL/capability configuration in the host only where it is truly host-specific. The Experience manifest owns semantic Experience identity and required roots. Do not treat the two schemas as interchangeable. |
| Demonstration catalog and evidence claims | Host manifest `demonstrations` data, parsed by `Services/WebPageHostManifest.cs` and `Services/DemonstrationRegistry.cs`; rendered by `Pages/Home.razor` | [Experiences demonstrations.json](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/blob/development/experiences/WorkshopShowcase/demonstrations.json) | Load catalog from the selected Experience artifact/content reference. Keep truthful host-capability filtering and browser rendering as host adapters; don't keep a duplicate catalog in the host manifest. |
| Progressive-discovery journey | `Components/Layout/NavMenu.razor` and route/tab handling in `Pages/Home.razor` | [Experiences journeys.json](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/blob/development/experiences/WorkshopShowcase/journeys.json) | Drive destination identity, reveal stage, and progression from journey data. Keep URL translation and accessible browser controls in WebPage. Do not silently drop current destinations or the `about-us` host utility route. |
| Destination-specific experience behavior | `Pages/Home.razor` `BuildManifestHub` branches, including Welcome, Demonstrations, Economy, Digitens, and other selected destinations | Experience-owned providers / MicroBundles, using the canonical experience definitions and the actual owning implementations | Replace each branch with a host-neutral destination/provider contract. A manifest route or label alone is not a replacement for the behavior. Keep a thin browser adapter for iframe, browser API, and navigation where necessary. |
| Composition and runtime lifecycle | `Services/WorkshopExperienceService.cs` | FSM_COS + FSM_API + canonical MicroBundle/Experience contracts | The service may orchestrate load, validation, candidate composition, activation, reset, and cleanup. It must not become a second catalog of executable MicroBundles or a second behavior engine. |
| Known compiled bundle implementations | `Workshop/Composition/WorkshopCompositionCatalog.cs` | Owning MicroBundle packages plus trusted artifact/provider registration | The catalog is a current bootstrap for known implementations, not arbitrary runtime code loading. Do not claim unknown executable MicroBundles can be loaded until the trusted artifact/provider path exists. |
| SVG and visual evidence | Host-local SVG samples and components | GUI.SVG / GUI.Core and the owning visual model; Visualize/Renderer when appropriate | Generate or bind visuals to the real model. A diagram can explain a runtime; it cannot be presented as proof that the runtime executed or that a GPU backend was used. |

## Cross-repository blockers

1. **Experience identity and immutable publication:** [Experiences issue #19](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/issues/19). The Showcase manifest currently has `experienceId: null` and `identityStatus: pending-numeric-reservation`. Do not invent an ID or resolve executable content from a mutable branch URL.
2. **Repository artifact delivery:** [MicroBundleRepository issue #22](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleRepository/issues/22). The Experience artifact contract needs a trusted REST retrieval path, including immutable identity/hash verification.
3. **Authored Experience → browser runtime:** [WebPage issue #100](https://github.com/TrentBest/WebPage/issues/100) tracks the neighboring authoring-to-runtime proof. Reuse existing Experience/MicroBundle contracts; do not add a parallel schema.

Until the first two contracts are available, WebPage can validate and compose known host-manifest roots, but cannot truthfully claim that it loads the canonical Experience package through the repository at runtime.

## Safe migration order

1. Reserve and publish the Experience identity/content package; expose repository retrieval with integrity verification.
2. Add a host-side resolver for the canonical Experience artifact and its referenced demonstration/journey content. Validate the complete candidate before changing active state.
3. Feed declared MicroBundle roots through FSM_COS. Keep the active Experience alive if resolution, validation, or composition fails.
4. Make destination selection resolve an Experience-owned destination/provider, while retaining host-owned URL and browser interaction adapters.
5. Add a same-host-build test with two different Experience artifacts and assert changes in selected roots, journey destinations, and actual presentation—not just a title.
6. Run existing direct-route, first-contact, Living GUI, demonstrations, Economy, Digitens, and about-us browser journeys against the data-driven implementation.
7. Remove duplicated WebPage catalog/branch logic only after each replacement is exercised by automated tests and browser smoke checks.
8. Keep reset, manifest switching, and reload explicit. Reset means recreating the selected Experience's runtime state from its selected immutable source; switching means resolving and composing the newly selected source before activation.

## Acceptance gates

- The active Experience identity, version, content hash, and source are observable.
- Invalid or unavailable artifacts leave the active runtime intact and show a useful diagnostic.
- The host can switch between two valid Experiences without recompilation.
- A reset starts the selected Experience from its defined initial state and does not leave stale bundles registered.
- Journey destinations and demonstration evidence are loaded from the Experience package, not duplicated in WebPage.
- Existing visitor journeys remain intact through migration.
- GUI.SVG / rendering examples disclose exactly what is generated, connected, executed, or measured.
- All work stays on `development`; no package publication or deployment is implied by this migration map.
