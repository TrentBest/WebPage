# WebPageLLM — Runtime Host Refactor

**Branch:** `development`  
**Status:** host-manifest switching/reset is implemented as an initial slice. The explicit entry scene and illustrative Replay Lab are present on `development`; the entry/browser-smoke correction is committed, but no passing workflow result for the corrected sequence is verified yet. Repository-backed Experience artifact loading and Experience-manifest switching remain unimplemented.

## Mission

Make WebPage an intentionally small WebApp host. It should load the selected Experience manifest and versioned MicroBundle artifacts at runtime, ask FSM_COS to compose them, and present the resulting capabilities. The showcase's domain content and behavior belong in the Experiences repository and owning MicroBundles—not compiled WebPage pages.

## Cross-repository source of truth

The first repository-owned showcase artifacts are now in [TheSingularityWorkshop.Experiences](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/tree/development/experiences/WorkshopShowcase):

- [manifest.json](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/blob/development/experiences/WorkshopShowcase/manifest.json) — Experience identity/version and versioned startup/running/capability roots.
- [demonstrations.json](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/blob/development/experiences/WorkshopShowcase/demonstrations.json) — complete current six-entry demonstration catalog, copied from the legacy host manifest.
- [journeys.json](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/blob/development/experiences/WorkshopShowcase/journeys.json) — current progressive-discovery destinations and reveal stages, moved as data; WebPage still implements the logic in compiled navigation code.
- [Experience README](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/blob/development/experiences/WorkshopShowcase/README.md) — ownership and migration status.
- [Experience-side handoff](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/blob/development/experiences/WorkshopShowcase/WebPageLLM_HANDOFF.md) — host contract and coordination.

A second authored Experience now exists separately at [Experiences/CastleSiege](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/tree/development/experiences/CastleSiege). Its stable battlespace IDs, Y-up/metre content convention, schema/example, and preforge handoff are the source for Castle Siege scene content. This is **not** the `workshop-showcase` catalog and is not yet loaded by WebPage. The WebPage consumer handoff is [CASTLE_SIEGE_PHYSICS_HANDOFF.md](integrations/CASTLE_SIEGE_PHYSICS_HANDOFF.md); it requires explicit coordinate conversion to Physics's real frame and forbids treating authored bounds or fixture trajectories as validated physical outcomes.

These are the beginning of the move, not yet the runtime source consumed by WebPage. Keep the current in-host manifest/content until the replacement has been exercised and validated; avoid a premature delete-and-break migration.

## Repository integration boundary verified

The Experience repository's source key is `workshop-showcase`; its numeric Experience ID is intentionally pending reservation. The current MicroBundleRepository core already defines `ExperienceArtifactAddress`, `ExperienceArtifact`, and `IExperienceCatalog`, but its REST host currently exposes health/list/get/put routes for MicroBundle artifacts only—not an Experience manifest/publication endpoint. WebPage cannot truthfully claim to load the Experience-owned manifest through MicroBundleRepository until that delivery contract is exposed and connected. The required REST integration is tracked in [MicroBundleRepository issue #22](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleRepository/issues/22); the host should consume that published contract rather than inventing an endpoint locally. The Experience identity/content publication prerequisite is tracked in [Experiences issue #19](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/issues/19); the source manifest currently has no reserved numeric Experience ID.

Do not work around this by treating an arbitrary GitHub URL as trusted executable authority. The host needs a configured Experience source/publication address, immutable version/hash validation, and the existing repository boundary for executable MicroBundle artifacts.

## Rendering capability boundary — source-verified

**Current:** `WorkshopCompositionCatalog` is an `IMicroBundleCatalog` for the canonical `TheSingularityWorkshop.MicroBundleDomain.IMicroBundle` contract. `WorkshopExperienceService.SwitchManifestAsync` converts declared roots to `RuntimeManifest`, calls `FsmCos.Execute`, verifies selected bundles in `RuntimeAssembly`, and activates only after candidate composition succeeds. This confirms the primary WebPage path goes through FSM_COS.

The WebPage-owned `Workshop.Rendering.RenderingMicroBundle` (ID 4100, version 0.1.0) currently declares ProtocolAi and GrammarAi dependencies and produces semantic rendering-intent/perception data. It is **not** proof that the Renderer or Visualize packages are loaded or that GPU rendering occurs.

Separately, `Workshop.MicroBundles.MicroBundle` is a Hub-level lifecycle shell implementing `TheSingularityWorkshop.SingularityHub.IMicroBundle`; it is not the type consumed by the FSM_COS catalog. `MonikerMicroBundle` explicitly implements both the domain and Hub contracts. There is no general automatic Hub-to-domain bridge: dual-contract behavior is implemented per bundle where needed.

**Direction:** a real Renderer-owned capability should implement the domain contract and be selected as an optional manifest root. The shared core, WebPage host without that root, and non-rendering Experiences must remain free of Renderer/Visualize/GUI.SVG/graphics-backend dependencies.

**Current integration proof (2026-10-17):** Renderer issue #15 now has passing CI evidence in [run 39069183165](https://github.com/TrentBest/TheSingularityWorkshop.Renderer/actions/runs/39069183165). Its `FsmCosRendering` sample proves that a manifest can select the exact Renderer-owned `RendererMicroBundle` through FSM_COS and observe its load context; `FsmCosWithoutRendering` proves a separate FSM_COS consumer builds and runs without Renderer/Visualize/GUI.SVG references. This is a meaningful composition/dependency-boundary milestone, **not** proof that Visualize is loaded, a graphics backend activates, pixels are rendered, or GPU performance exists. The Renderer bundle remains a lifecycle/identity shell; WebPage still needs the next consumer-visible adapter contract and a real end-to-end Renderer → Visualize frame proof.

## Singularity City / Universe presentation contract

**Current:** WebPage's 2D floor plan, hub, labs, and terminals are the semi-diegetic entry layer: the visitor can see the Workshop's conceptual spaces while using browser-native controls. The manifest declares destinations and demonstrations; those entries should be the single source for discoverability, not duplicated hardcoded feature cards. The Physics Replay Lab is now declared in the host manifest's demonstration catalog and embeds its existing route as an explicitly illustrative fixture.

**Direction:** present each repository's real capability as an inspectable station/Experience in the shared Singularity City/Universe. Show the source/evidence boundary at the station: what runs now, what data it uses, and what remains unavailable. Keep navigation in the Workshop world rather than turning the showcase into a generic SaaS feature grid.

**Future:** 3D diegetic tooling can appear as in-world instruments, terminals, workbenches, and observatories. Those are alternate manifestations of the same Experience identity and capability graph—not new owners of semantics. They must consume the same manifest/artifact contracts, FSM_COS composition results, and FSM_API lifecycle behavior; the 3D scene must not become a second runtime.

**Limit:** the current hub still has compiled route/presentation adapters and is not a fully simulated 3D universe. Do not claim that a destination is MicroBundle-backed merely because it appears as a station.

## Current WebPage debt confirmed from source

- `WorkshopExperienceService.InitializeAsync` loads `wwwroot/Workshop/Forge/StreamingAssets/Experiences/webpage-host.manifest.json` by default.
- `WorkshopExperienceService.SwitchManifestAsync(path)` now loads a same-origin relative host-manifest asset, composes a candidate primary/hub/capability runtime, and activates its manifest-owned bundle set in the stable session-scoped Hub only after composition succeeds, preserving the Hub instance held by PageFSM. `RestartFromManifestAsync` now reloads the active host-manifest path.
- `WorkshopCompositionCatalog` now creates fresh bundle instances instead of returning long-lived shared instances. Hub and HubRuntime DI lifetimes are scoped per browser session.
- The Hub lab now exposes the active manifest identity/version/source, a same-origin manifest-path selector, switch action, and reset/reload action.
- Tests cover switching between two host manifests, reloading/recomposing the active manifest, preserving the active composition when a candidate manifest is invalid, and rejecting external/traversal paths. The browser smoke test initially failed because it expected first-contact labels before the restored entry scene; the entry action was separated from the PageFSM gateway signal and `tests/browser-smoke.mjs` was updated. The corrected commits are on `development`; a fresh passing workflow/browser run is not yet verified.
- This is an initial host-manifest switching slice, not the final repository-backed Experience loader. Hub destinations still have empty `microBundles` arrays, route behavior is partly compiled, and the new Experience-owned `workshop-showcase` manifest/content are not yet consumed by WebPage.
- The manifest declares a `Replay Lab` destination and a `physics-replay-lab` demonstration entry; the previous hardcoded gallery card was removed so the manifest is the single catalog source. `/replay-lab` remains a deterministic illustrative fixture with scrub/play controls and explicit unsupported states. It is not connected to Physics world/checkpoint reconstruction; the integration handoff records the next verified contract needed.
- The current hub runtime has no demonstrated transactional reset/replacement lifecycle. Do not dispose/reuse runtime objects casually or mutate active state before candidate composition succeeds.

## Required refactor order

1. **Define the repository boundary.** Select Experience identity/version; resolve its manifest and content using the canonical Experience/MicroBundle repository path. Define trusted immutable identity/digest, cache/bootstrap behavior, size limits, compatibility, and allowed-origin policy. Do not use arbitrary browser-supplied URLs as executable authority.
2. **Keep host configuration small.** WebPage-specific settings remain host-owned (base path, browser capabilities, initial Experience selection, authentication/session adapters). Experience identity, journeys, demo metadata, behavior, and required bundles move to Experiences/MicroBundles.
3. **Separate resolution from activation.** Parse and validate into a candidate object, resolve versioned roots, compose through FSM_COS, and only then atomically activate. A bad manifest leaves the active runtime intact.
4. **Make reset and switching real.** Reset reloads/recomposes the selected manifest. Switch selects another valid manifest. Reload detects changed content only under an explicit version/identity policy. Surface active identity/version, status, and failure reason.
5. **Correct lifecycle ownership.** Audit MicroBundle factories, FSM_COS assembly lifetimes, hub registration, event subscriptions, and disposal before enabling repeated activation. Do not reuse disposed bundle instances or leave old Hub registrations active.
6. **Present selected Experience data.** Replace compiled page-specific selection with manifest/content-driven routing and package-owned providers. Keep browser-specific route/viewport/input integration in WebPage.
7. **Prove redefinition.** With one host build, switch between two valid Experience manifests and verify a meaningful change in composition and visitor behavior. Test reset, invalid manifest, unavailable artifact, incompatible version, and rollback.
8. **Move incrementally.** Migrate demonstration and journey data from the legacy WebPage manifest to the Experience repository. Remove each old copy only after tests prove the replacement.
9. **Living documentation.** Use GUI.SVG or the owning Visualize/Renderer boundary to show real state/model changes. Label what a visual proves and does not prove; do not present an SVG animation as FSM execution or GPU proof.

## Acceptance gates

- [ ] Two-branch Sith policy: work on `development`; leave `master` untouched.
- [ ] Existing visitor journeys remain functional during migration.
- [ ] Candidate failure cannot corrupt the active Experience.
- [ ] Reset, switch, and reload have distinct, tested semantics.
- [ ] Experience data is repository-owned; WebPage contains host adapters, not duplicated showcase domain behavior.
- [ ] Composition is delegated to FSM_COS and state-machine semantics to FSM_API.
- [ ] No arbitrary code execution from JSON or browser input.
- [ ] Tests/build/browser evidence are recorded before claiming the refactor complete.
- [ ] No package publish or deployment without explicit authorization.

## Progress log

- 2026-10-17: Corrected the Forge return smoke test to use its actual `← EXIT FORGE` control instead of browser history. The Forge is an in-place room, so browser Back can leave the Workshop rather than return to the map. The first correction confirmed the navigation assumption but revealed that the button label differed from the other Forge GUI's return label; the test now matches `SpatialForgeGuiBuilder`. A fresh browser run is triggered by commit `1f0e532d750911494b9ca3298b06f378467641d3`.
- 2026-10-17: Confirmed the existing `/replay-lab` route is a truthful local fixture, updated its integration handoff, and kept physical replay/share/remix explicitly unavailable pending Physics contracts.
- 2026-10-17: Verified the WebPage FSM_COS/domain-contract boundary and recorded the distinction between the WebPage-owned RenderingMicroBundle placeholder and a real Renderer-owned capability in Renderer issue #15.
- 2026-10-17: Added the first Experience-owned Workshop Showcase manifest and moved the current six-entry demonstration catalog into the Experiences repository.
- 2026-10-17: Added candidate-based host-manifest switching/recomposition, active-manifest reset, fresh bundle factories, stable Hub manifest-state reset/activation, and Hub-lab controls; added tests for switch, reset, rollback, and safe paths.
- 2026-10-17: Updated the WebPage orchestration contract to link to the Experience-owned artifacts.
- Remaining: wire the host to resolve the Experience-owned manifest and content through the canonical repository/cache boundary; add user-facing selection; move the remaining demonstration/journey catalog; replace compiled route/presentation behavior only after the replacement is proven.
