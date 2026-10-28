# WebPageLLM — Runtime Host Refactor

![Living GUI documentation lens: question, model, implementation, visible manifestation, and proof](https://raw.githubusercontent.com/TrentBest/WebPage/development/docs/assets/living-gui-documentation-lens.svg)

> **Evidence class:** architecture illustration, not live runtime telemetry. Follow the subject-specific source, view, fixture, or test below; if none exists, record that gap rather than implying it is implemented.


**Branch:** `development`  
**Status:** host-manifest switching/reset is implemented as an initial slice. The explicit entry scene and illustrative Replay Lab are present on `development`; the Forge return path is implemented, and the browser smoke now preserves the existing first-contact/gateway FSM for both the Forge return and a later direct Welcome navigation; the latest browser-smoke correction now advances through the navigation FSM's progressive destination stages instead of assuming later links are exposed after a fresh session load; CI is pending. Repository-backed Experience artifact loading and Experience-manifest switching remain unimplemented.

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

The Experience repository's source key is `workshop-showcase`; its numeric Experience ID is intentionally pending reservation. **Correction from current development source (2026-10-27):** MicroBundleRepository now exposes `GET /api/experiences/{experienceId}/publication` and `GET /api/experiences/{experienceId}/{version}/{contentHash}`. Its `RestExperienceClient` resolves the current publication address through `IExperienceCatalog`, retrieves an immutable artifact through `IExperienceRepository`, checks the returned identity, and reconstructs the artifact so its SHA-256 is verified. [Host-level endpoint tests](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleRepository/blob/development/tests/REST.Host/TheSingularityWorkshop.MicroBundleRepository.Rest.Host.Tests/ExperienceEndpointTests.cs) cover publication lookup, exact artifact bytes, unpublished/missing artifacts, unconfigured storage, invalid IDs/hashes, oversized artifacts, and stored-integrity failures. The current repository development head [`4ed4804`](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleRepository/commit/4ed4804f276c5a7535b3aaa90ee8b58182168a4a) passed [Build and Test](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleRepository/actions/runs/40154223681) and the [package workflow](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleRepository/actions/runs/40154223714). The older statement that no Experience endpoint existed was stale. **This is not yet end-to-end WebPage activation:** the REST host registers the Experience catalog/repository only in Azure mode; Local mode returns `503` for Experience routes. The Showcase manifest still has `experienceId: null` and references `demonstrations.json`/`journeys.json` by relative filenames, so the immutable artifact's content closure and reference policy still need to be settled. [MicroBundleRepository issue #22](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleRepository/issues/22) remains open for pre-buffer size bounds, complete error/cancellation coverage, security/CORS/write-authorization policy, and end-to-end validation. [Experiences issue #19](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/issues/19) still owns numeric identity and publication semantics. The separate dependency-closure decision remains unresolved in [MicroBundleRepository #24](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleRepository/issues/24), [MicroBundleDomain #17](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleDomain/issues/17), and [FSM_COS #11](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/issues/11). Do not invent a WebPage-side resolver or treat bundle inventory as semantic dependency resolution.

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
- Tests cover switching between two host manifests, reloading/recomposing the active manifest, preserving the active composition when a candidate manifest is invalid, and rejecting external/traversal paths. The browser smoke preserves the explicit destination across a fresh host load while still requiring the existing first-contact/gateway entry sequence before asserting the Welcome hub. CI caught that the earlier test incorrectly expected the hub immediately after reload; the app's entry lifecycle is intentionally not bypassed. The corrected gateway test was committed on `development`, then CI exposed a further stale assertion: later navigation links are intentionally revealed only as the visitor advances through destinations. The browser smoke now walks Welcome → Singularity Laboratory → Singularity Architecture → Singularity City before asserting Digitens, matching the live navigation FSM. Latest test commit: `005072d2e5bcb35a7fe4b393845375756f9d009f`; CI is pending.
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

- 2026-10-17: The progressive-navigation browser retry reached the host but found a toggle-state assumption: the Gateway may hand off with navigation already expanded, so looking only for an “Expand” button can fail or collapse an already-open menu. The smoke test now checks whether Welcome is already visible and opens the menu only when needed, while preserving the real staged destination sequence. Test commit: `cbbc4978f7058349f4c9635f5fb04b0d22456527`; CI pending.
- 2026-10-17: The Forge browser journey exposed a real return-path defect after the test was corrected to click its actual `← EXIT FORGE` control. The floorplan launches `/explore?room=forge`, so returning to the Explore spatial map alone is the wrong destination. The route-aware Forge exit performs room cleanup and returns hub-launched visitors to `/?tab=welcome`; a Forge entered from inside Explore instead restores its own spatial map overlay. The browser retry confirmed the host serves the route, but the smoke test was waiting for `.workshop-map` (the Explore-world overlay) after the app had intentionally returned to the Home Welcome hub. The test now asserts the Welcome hub overlay and its heading, then continues checking navigation. App behavior commit: `5f072511a464ebba8ac4a51e1462f943e4d32996`; smoke assertion commits: `3712716d492f57ac30f3a8dae0705fece33f81f2`, `d526be00da049b345ebd2261d22d2687247e5ddd`. The next run revealed a second test-contract issue: a fresh host load correctly restores the `WELCOME` selection but does not skip the first-contact/gateway FSM. The browser test now waits for both first-contact statements and clicks the existing Enter Workshop action before asserting the Welcome hub. Test-only correction commit: `e4dab8b60986003ca86c6c33f4c894736df325d0`. Await CI before declaring the route resolved.
- 2026-10-17: Confirmed the existing `/replay-lab` route is a truthful local fixture, updated its integration handoff, and kept physical replay/share/remix explicitly unavailable pending Physics contracts.
- 2026-10-17: Verified the WebPage FSM_COS/domain-contract boundary and recorded the distinction between the WebPage-owned RenderingMicroBundle placeholder and a real Renderer-owned capability in Renderer issue #15.
- 2026-10-17: Added the first Experience-owned Workshop Showcase manifest and moved the current six-entry demonstration catalog into the Experiences repository.
- 2026-10-17: Added candidate-based host-manifest switching/recomposition, active-manifest reset, fresh bundle factories, stable Hub manifest-state reset/activation, and Hub-lab controls; added tests for switch, reset, rollback, and safe paths.
- 2026-10-17: Updated the WebPage orchestration contract to link to the Experience-owned artifacts.
- Remaining: wire the host to resolve the Experience-owned manifest and content through the canonical repository/cache boundary; add user-facing selection; move the remaining demonstration/journey catalog; replace compiled route/presentation behavior only after the replacement is proven.
- 2026-10-17: The next browser run passed the first-contact handoff and reached the progressive rail, but failed because selecting Singularity City left the rail in a state where the test blindly required the `Expand Workshop navigation` accessible name. The toggle name intentionally changes with expanded/collapsed state; the test now inspects whether the next destination is already visible and only clicks Expand when that button is actually present. Test commit: `c164d921772e3ebfadb682a5b049fce334171306`; waiting on CI. The independent repository-source integration job passed in both runs; the browser journey remains the gate.

- 2026-10-17: The latest browser run reached Singularity City, then timed out looking for Digitens in the Workshop rail. Source inspection confirmed City Lab intentionally declares `@layout BareLayout`, so the rail is absent on that route; the failure was an invalid cross-layout navigation assumption, not evidence that Digitens routing is broken. The smoke journey now verifies the City Lab page and its explicit Digitens return link, then enters Economy through its supported host tab and verifies Digitens through the restored Workshop rail. Test-only correction: `pending CI`. No master, deploy, or package publish.


## Casino gateway regression — coordination note (2026-10-17)

**Current:** The latest diagnostic browser run confirms `/economy/casino-floor.html` is served and contains the expected `SINGULARITY CASINO` heading, but the host renders zero matching iframe elements after the direct `/?tab=economy` first-contact/gateway journey. The Renderer repository-source integration job passed; the browser journey remains failed: [run 39076907908](https://github.com/TrentBest/WebPage/actions/runs/39076907908). Tracking issue: [#121](https://github.com/TrentBest/WebPage/issues/121).

**Source path:** `Home.razor` only mounts the hub overlay in `PageFSM.NavigationArrival` or `PageFSM.Running`; `BuildManifestHub` renders the casino iframe for `ECONOMY`. The gateway callback currently awaits `interop.markWorkshopVisited` before calling `FSM.RequestEnter()`. That ordering is a plausible failure path, not yet a proven root cause.

**Next proof:** instrument the callback and failure diagnostics to record browser page errors, whether the JS interop completed, and the actual PageFSM state after the click. Verify a persistence/interoperability error cannot strand entry before the FSM request, without bypassing the entry FSM. Add a focused regression proof for direct Economy entry, then update the status here only after CI passes. Do not relax the iframe assertion or extend its timeout as a substitute for fixing lifecycle behavior.

**Boundary:** WebPage owns the browser gateway/navigation adapter; Economy owns reusable simulated currency and casino domain behavior. Stay on `development`; no master changes, publish, or deploy.


## 2026-10-23 checkpoint — Physics evidence and integration boundaries reconciled

- Updated [Singularity Laboratory Stations](SINGULARITY_LABORATORY_STATIONS.md) against Physics development through 2026-10-23. Added bounded `SeededPointSet2d` and `MandelbrotEscapeTime2d` sources, tests, and exporter links; linked the checked-in thermal-conduction and acoustic observation/reflection run records, source-derived SVG, and shared run-record catalog. Physics Build and Tests passed on the reviewed sampler/exporter PR heads. None of these artifacts by itself constitutes a wired browser station; acoustic records/SVG remain path/observation metadata, not synthesized audio.
- Updated the [Castle Siege Physics handoff](integrations/CASTLE_SIEGE_PHYSICS_HANDOFF.md). Physics issue [#23](https://github.com/TrentBest/TheSingularityWorkshop.Physics/issues/23) remains open; existing projectile/intersection/impact primitives are not yet a validated Castle Siege run/adapter. The Experience-owned Y-up/metre convention still requires a tested mapping to Physics' declared frame.
- Updated the [Replay / Shareable Moments handoff](integrations/PHYSICS_REPLAY_WEBPAGELLM_HANDOFF.md). The browser-smoke source checks the manifest-driven Replay Lab card and iframe heading, but no exact-head WebPage CI result was confirmed in this review. The fixture remains illustrative; Physics world reconstruction and portable Moments remain unavailable.
- Appended the authorized dated WebPageLLM response to Physics root `PhysicsCastlesLLM.md` only. No other Physics file was changed.

**Current gate:** repository-backed Experience activation still depends on WebPage #100, Experiences #19, MicroBundleDomain #17, MicroBundleRepository #22/#24, and FSM_COS #11. Do not add a parallel manifest/closure resolver or duplicate the Experience-owned Castle Siege scene data. No merge, publication, release, or deployment was performed.

**CI status:** the exact current WebPage development head has not been verified by a confirmed workflow result after these documentation commits. Treat the source-level browser smoke as coverage, not as a passing execution.

## 2026-10-24 — Castle Siege authored destination scaffold

Implemented the minimal WebPage-side spatial destination scaffold on `development`:

- [Spatial destination catalog](https://github.com/TrentBest/WebPage/blob/development/TheSingularityWorkshop/Workshop/Spatial/SpatialPlaceCatalog.cs) now declares `city.castle-siege` / `castle-siege`.
- [Spatial scene](https://github.com/TrentBest/WebPage/blob/development/TheSingularityWorkshop/Workshop/Spatial/SpatialWorkshopScene.cs) makes it reachable from the existing Explore world.
- [Castle Siege presentation builder](https://github.com/TrentBest/WebPage/blob/development/TheSingularityWorkshop/Gui/SpatialCastleSiegeGuiBuilder.cs) shows the commander-shoulder fixture, functional presentation-zone selection and label toggling, plus explicit setup/baseline/variant/measurement status cards.
- [Explore routing](https://github.com/TrentBest/WebPage/blob/development/TheSingularityWorkshop/Pages/Explore.razor) uses the existing scene router; no parallel route system was added.
- [Registration tests](https://github.com/TrentBest/WebPage/blob/development/SingularityHub.Tests/SpatialCastleSiegeRegistrationTests.cs) cover the destination identity and default-scene construction.

The visual castle and siege engine are an illustrative WebPage fixture only. The left/center/right highlight zones are presentation state, not canonical Experience IDs or Physics target geometry. No trajectory, target hit/miss, impact, damage, or authoritative replay is computed. The Experience-owned Castle Siege artifact is not yet loaded because the immutable artifact/dependency-closure contract remains unresolved.

The latest WebPage commits are [experiment status panels](https://github.com/TrentBest/WebPage/commit/f76f1b2d5991172f1beba8d0c0488f5ab7d36443), [Explore route](https://github.com/TrentBest/WebPage/commit/91bf62548d2cae0d0ad2caace39056a5caddc790), and [destination maturity wording](https://github.com/TrentBest/WebPage/commit/f1fa87482fc662bcdaf7daf2c47958d6d10619bc). The .NET Tests workflow has a `development` push trigger, but no confirmed exact-head result was surfaced; CI remains unverified.

## 2026-10-24 — Acoustic evidence label corrected

The Singularity Studios page already presents Physics-generated free-field observations and the source-derived first-order room-reflection run record/SVG. Its former hero label said “FREE-FIELD OBSERVATIONS ONLY TODAY,” which understated the checked-in reflection-path metadata shown later on the same page. It now reads “ANALYTICAL PATH METADATA ONLY.” The browser smoke source also asserts that the first-order reflection heading, Physics-owned serialized room-run link, and source-derived room-plan SVG are present. This still makes no claim of measured audio, synthesized waveforms, phase-aware path combination, or a complete room-acoustics model. The smoke assertions are committed in [659043a](https://github.com/TrentBest/WebPage/commit/659043ab2922d937a7e53a7a01be91c8925c51e1); current-head WebPage CI remains unverified.


## 2026-10-27 — Foundry execution and HTTP/REST ownership audit

**Request sources:** [WebPage #123](https://github.com/TrentBest/WebPage/issues/123), [WebPage #124](https://github.com/TrentBest/WebPage/issues/124), [Foundry #1](https://github.com/TrentBest/TheSingularityWorkshop.Foundry/issues/1), [GUI #53](https://github.com/TrentBest/TheSingularityWorkshop.GUI/issues/53), and [WebPage #122](https://github.com/TrentBest/WebPage/issues/122).

This is a source-backed boundary checkpoint, not a claim that WebPage is already executing Hub workflows through Foundry.

### Execution ownership verified on development

- [`HubContracts.cs`](https://github.com/TrentBest/WebPage/blob/development/SingularityHub.Abstractions/HubContracts.cs) defines `ISingularityHub` through arbitration, warehouse identity liaison, and process-group lifecycle. `WorkDescriptor(ProcessGroupId, WorkId, PathIndex)` and `IExecutionProvider.Execute(WorkDescriptor) -> CompletionToken` are separate provider abstractions; they are not a `WorkflowDefinition` or `WorkflowResult`.
- [`SingularityHub.cs`](https://github.com/TrentBest/WebPage/blob/development/SingularityHub/SingularityHub.cs) implements MicroBundle registration/loading, at-most-ten-round arbitration, process-group update dispatch, routing, and warehouse identity mapping. The Hub is not the Foundry workflow runner.
- [`HubRuntime.cs`](https://github.com/TrentBest/WebPage/blob/development/TheSingularityWorkshop/Infrastructure/Hub/HubRuntime.cs) resets manifest-owned bundle state, loads the selected Hub bundle, runs arbitration, and restores prior bundles if activation fails. This is current host compatibility behavior.
- [`WorkshopExperienceService.cs`](https://github.com/TrentBest/WebPage/blob/development/TheSingularityWorkshop/Services/WorkshopExperienceService.cs) fetches a host manifest, composes an FSM_COS `RuntimeAssembly`, prepares/activates the host execution adapter, and activates the Hub bundle. A `RuntimeAssembly` is composition output, not a workflow definition or proof of executable lifecycle.
- [`ExperienceExecutionRuntime.cs`](https://github.com/TrentBest/WebPage/blob/development/TheSingularityWorkshop/Services/ExperienceExecutionRuntime.cs) calls the current compatibility runtime `PageFsmExecutionRuntime`: `Tick()` forwards to `PageFSM.Update()`, and `Observe()` forwards to `PageFSM.ObserveRuntime()`. That is legacy WebPage lifecycle behavior, not Foundry integration.
- The WebPage project [currently references FSM_COS and GUI.Blazor but not Foundry](https://github.com/TrentBest/WebPage/blob/development/TheSingularityWorkshop/TheSingularityWorkshop.csproj). Foundry's [`WorkflowDefinition`](https://github.com/TrentBest/TheSingularityWorkshop.Foundry/blob/development/src/Foundry.Workflow/WorkflowDefinition.cs) is a version-1 ordered list of named handler steps; [`WorkflowRunner`](https://github.com/TrentBest/TheSingularityWorkshop.Foundry/blob/development/src/Foundry.Workflow/WorkflowRunner.cs) validates and executes those handlers and returns per-step/workflow outcomes. Foundry has now added [`WorkflowExecutionCoordinator`](https://github.com/TrentBest/TheSingularityWorkshop.Foundry/blob/development/src/Foundry.Workflow/WorkflowExecutionCoordinator.cs), which provides accepted/running/terminal snapshots around that existing runner, with submit/start/cancel/dispose behavior and a green [Foundry CI run #92](https://github.com/TrentBest/TheSingularityWorkshop.Foundry/actions/runs/40547511993). This is a meaningful owner-side lifecycle primitive, but it still accepts only `WorkflowDefinition`; no WebPage adapter connects it to `WorkDescriptor`, `RuntimeAssembly`, or the legacy PageFSM lifecycle.

### Ownership decision and next gate

- **FSM_COS:** compose MicroBundles and produce `RuntimeAssembly`.
- **Foundry:** own the host-neutral workflow execution lifecycle and authoritative outcomes, reconciling `WorkflowRunner` with any necessary composed-runtime lifecycle before adding a second runner.
- **GUI:** present Hub/workflow state and emit explicit user intents; presentation remains optional for headless hosts.
- **WebPage:** supply browser-host adapters and preserve the existing PageFSM path until a stable Foundry-facing contract is implemented and tested.
- **Legacy Hub:** keep the current bounded arbitration/process-group behavior accurately documented; do not equate process-group ticks with sequential Foundry workflow steps.

The existing [contract tests](https://github.com/TrentBest/WebPage/blob/development/SingularityHub.Tests/ContractSurfaceTests.cs), [provider tests](https://github.com/TrentBest/WebPage/blob/development/SingularityHub.Tests/ExecutionDomainTests.cs), and [Hub lifecycle tests](https://github.com/TrentBest/WebPage/blob/development/SingularityHub.Tests/SingularityHubTests.cs) cover host-neutral contracts, provider results, and process-group transitions. A regression assertion now guards that the Hub lifecycle contract does not absorb the provider/work-descriptor contract. Foundry issue #1 now has a tested source-level submit/start/snapshot/cancel/dispose coordinator for sequential workflow definitions. The remaining gate is a stable, versioned host-facing contract plus an explicit decision about how (or whether) composed `RuntimeAssembly` lifecycle differs from sequential `WorkflowDefinition` execution and Hub `WorkDescriptor` provider work. The WebPage adapter and cross-host lifecycle/failure tests are not yet implemented. No Foundry or GUI source was edited from WebPage.

### HTTP/REST call-site inventory

| Current WebPage call site | Classification and decision |
| --- | --- |
| [`Program.cs`](https://github.com/TrentBest/WebPage/blob/development/TheSingularityWorkshop/Program.cs) registers a scoped `HttpClient` with the Blazor host base address. | Browser/framework plumbing; keep. `HttpClient` is the .NET transport, not a separate REST framework. |
| [`WorkshopExperienceService.ReadManifestJsonAsync`](https://github.com/TrentBest/WebPage/blob/development/TheSingularityWorkshop/Services/WorkshopExperienceService.cs) reads a configured relative or trusted HTTPS manifest, requires JSON, applies cancellation/timeout, and caps the body at 1 MiB. | Static artifact/configuration retrieval, not a generic REST operation. It remains a compatibility path, not the target for repository-owned Experience content. The repository now exposes a publication-pointer and immutable-artifact REST path, but WebPage must not switch until numeric identity, artifact content/reference packaging, trusted dependency closure, and fail-closed activation are agreed and tested. Do not trust mutable GitHub branch URLs as executable authority. |
| [`StaticWebAppAuthenticationStateProvider`](https://github.com/TrentBest/WebPage/blob/development/TheSingularityWorkshop/Services/StaticWebAppAuthenticationStateProvider.cs) calls `/.auth/me`. | Azure Static Web Apps authentication plumbing; keep platform-specific. |
| [`Weather.razor`](https://github.com/TrentBest/WebPage/blob/development/TheSingularityWorkshop/Pages/Weather.razor) reads `sample-data/weather.json`. | Static sample data; keep. |
| [`NamingPoll.razor`](https://github.com/TrentBest/WebPage/blob/development/TheSingularityWorkshop/Pages/NamingPoll.razor) posts `api/vote` and gets `api/results`; [`VoteFunction.cs`](https://github.com/TrentBest/WebPage/blob/development/PollingAPI/VoteFunction.cs) implements both routes. | Real domain REST calls and the clearest FSM_REST candidate. Keep vote DTOs, storage policy, and user-facing errors local. Do not switch the package dependency until the public package/version and the required transport API are confirmed and covered by deterministic tests. |
| [`WorkshopWorldClient.cs`](https://github.com/TrentBest/WebPage/blob/development/TheSingularityWorkshop/Workshop/World/WorkshopWorldClient.cs) posts `api/world/switch` and polls `api/world/state` every three seconds. | REST-shaped calls, but no matching route implementation was found in the current WebPage tree. Endpoint ownership/availability must be established before migrating or claiming this path works. |
| Direct `HttpRequestMessage` or JavaScript `fetch()` client | No additional call site was identified in the reviewed current-development source paths; recheck if new transport code is introduced. |

FSM_REST's [current source transport](https://github.com/TrentBest/TheSingularityWorkshop.FSM_REST/blob/development/src/FSM_REST/HttpClientRestTransport.cs) accepts a host-owned `HttpClient`; its [consumer guide](https://github.com/TrentBest/TheSingularityWorkshop.FSM_REST/blob/development/docs/CONSUMING.md) documents `RestRequestFactory`, `IRestTransport`, and `RestOperationExecutor`. The source project declares `0.1.0-alpha.5`, but WebPage has no FSM_REST package/project reference and a compatible published NuGet package was not verified in this review. The current source transport also does not expose the requested fluent `HttpClientBuilder`. **Do not add a speculative dependency or a local wrapper:** coordinate that API/package gate with FSM_REST's owner, then integrate only the confirmed REST call sites. Binary/streaming and platform-auth flows must not be forced through a text-only REST response contract.

**Checkpoint (2026-10-27):** source mapping and the Hub/provider separation regression are in WebPage development. MicroBundleRepository's publication/artifact endpoints now exist and the current development build/test and package workflows passed, but WebPage's runtime consumer remains unwired; Experience identity/content closure and repository size/security gates remain open. Foundry's coordinator exists and passed its own CI, but no stable WebPage-facing adapter connects it to `RuntimeAssembly` or the legacy PageFSM lifecycle. FSM_REST adoption remains gated on a confirmed public package version and the builder/transport contract. Recheck the current WebPage browser workflow before claiming the latest route fixes are green. No merge, publication, release, or deployment was performed.


## 2026-10-28 — Cross-host UserIO capability requirements

**Requesting owner:** [Experiences issue #27 — manifest-declared UserIO requirements and host-compatible MicroBundle resolution](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/issues/27). This is an explicit cross-host request for one Experience to run through AnyApp and WebApp without rewriting its domain logic.

### Source-grounded contract status

- The current Experience manifest's `Bundles` / `ExperienceBundleRequest` entries identify concrete bundle roots and can carry immutable artifact identity/configuration references. `Intent` is a single `SemanticIntent` (`Name` plus optional `ProtocolId`), not a capability collection.
- FSM_UserIO currently provides the semantic-intent contract; no concrete UserIO provider MicroBundle or capability resolver was confirmed in the reviewed source.
- MicroBundleDomain's `MicroBundleDescriptor` lists generic `MicroBundleProvider` identifiers and dependencies. It is deliberately ontology-agnostic and does not negotiate host providers.
- MicroBundleRepository's immutable artifact identity is a concrete content address, not a semantic capability address. A publication pointer transports the Experience manifest; it does not resolve its semantic UserIO requirements.
- WebPage's `OntologySignature`, Hub capability IDs, and `ExperienceManifest.RequiredCapabilities` belong to the existing WebPage Hub/ontology boundary. They must not be silently reused as the FSM_UserIO / AnyApp manifest contract.

### Ownership and WebPage decision

- **Experiences** owns reusable Experience semantics and the versioned manifest requirement vocabulary.
- **FSM_UserIO** owns host-neutral semantic request/response contracts; a semantic request is not authorization to execute a domain transition.
- **AnyApp and WebPage/WebApp** own host-side provider resolution and platform adapters. Resolve a semantic requirement to explicit, versioned MicroBundle/artifact identities before composition; do not put WPF, DOM, keyboard, mouse, or touch assumptions into the Experience.
- **MicroBundleDomain** owns generic descriptor/provider/dependency contracts, not a Workshop-specific capability ontology.
- **FSM_COS** composes the resolved bundle request and dependency closure.
- **Foundry** owns runtime workflow execution across AnyApp, WebApp, The, and WebPage. A UserIO adapter must not become a second execution scheduler.

**Current disposition: blocked for consumer implementation; actionable as contract coordination.** The first implementation gate belongs with the Experience publication contract: define stable semantic capability IDs and their versioning, then have each host resolve them to existing immutable artifact identities. Add failure-before-activation tests and preserve the previously active Experience when resolution, verification, or composition fails. Only after the shared semantic request/response and Foundry lifecycle seams are identified should browser/WPF adapters be added.

WebPage will not overload `SemanticIntent`, reinterpret `OntologyAddress`, reuse Hub ontology IDs as UserIO requirements, or invent a second browser-only capability schema. No WebPage UserIO provider or runtime integration is claimed by this checkpoint.
