# WebPageLLM — Runtime Host Refactor

**Branch:** `development`  
**Status:** host-manifest switching/reset is implemented as an initial slice; repository-backed Experience artifact loading and Experience-manifest switching are not yet implemented.

## Mission

Make WebPage an intentionally small WebApp host. It should load the selected Experience manifest and versioned MicroBundle artifacts at runtime, ask FSM_COS to compose them, and present the resulting capabilities. The showcase's domain content and behavior belong in the Experiences repository and owning MicroBundles—not compiled WebPage pages.

## Cross-repository source of truth

The first repository-owned showcase artifacts are now in [TheSingularityWorkshop.Experiences](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/tree/development/experiences/WorkshopShowcase):

- [manifest.json](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/blob/development/experiences/WorkshopShowcase/manifest.json) — Experience identity/version and versioned startup/running/capability roots.
- [demonstrations.json](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/blob/development/experiences/WorkshopShowcase/demonstrations.json) — complete current six-entry demonstration catalog, copied from the legacy host manifest.
- [journeys.json](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/blob/development/experiences/WorkshopShowcase/journeys.json) — current progressive-discovery destinations and reveal stages, moved as data; WebPage still implements the logic in compiled navigation code.
- [Experience README](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/blob/development/experiences/WorkshopShowcase/README.md) — ownership and migration status.
- [Experience-side handoff](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/blob/development/experiences/WorkshopShowcase/WebPageLLM_HANDOFF.md) — host contract and coordination.

These are the beginning of the move, not yet the runtime source consumed by WebPage. Keep the current in-host manifest/content until the replacement has been exercised and validated; avoid a premature delete-and-break migration.

## Repository integration boundary verified

The Experience repository's source key is `workshop-showcase`; its numeric Experience ID is intentionally pending reservation. The current MicroBundleRepository core already defines `ExperienceArtifactAddress`, `ExperienceArtifact`, and `IExperienceCatalog`, but its REST host currently exposes health/list/get/put routes for MicroBundle artifacts only—not an Experience manifest/publication endpoint. WebPage cannot truthfully claim to load the Experience-owned manifest through MicroBundleRepository until that delivery contract is exposed and connected.

Do not work around this by treating an arbitrary GitHub URL as trusted executable authority. The host needs a configured Experience source/publication address, immutable version/hash validation, and the existing repository boundary for executable MicroBundle artifacts.

## Current WebPage debt confirmed from source

- `WorkshopExperienceService.InitializeAsync` loads `wwwroot/Workshop/Forge/StreamingAssets/Experiences/webpage-host.manifest.json` by default.
- `WorkshopExperienceService.SwitchManifestAsync(path)` now loads a same-origin relative host-manifest asset, composes a candidate primary/hub/capability runtime, and activates its manifest-owned bundle set in the stable session-scoped Hub only after composition succeeds, preserving the Hub instance held by PageFSM. `RestartFromManifestAsync` now reloads the active host-manifest path.
- `WorkshopCompositionCatalog` now creates fresh bundle instances instead of returning long-lived shared instances. Hub and HubRuntime DI lifetimes are scoped per browser session.
- The Hub lab now exposes the active manifest identity/version/source, a same-origin manifest-path selector, switch action, and reset/reload action.
- Tests cover switching between two host manifests, reloading/recomposing the active manifest, preserving the active composition when a candidate manifest is invalid, and rejecting external/traversal paths. The host build and repository-source integration have passed in an earlier workflow run; the full browser journey and test suite are still running on the latest commits.
- This is an initial host-manifest switching slice, not the final repository-backed Experience loader. Hub destinations still have empty `microBundles` arrays, route behavior is partly compiled, and the new Experience-owned `workshop-showcase` manifest/content are not yet consumed by WebPage.
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

- 2026-10-17: Added the first Experience-owned Workshop Showcase manifest and moved the current six-entry demonstration catalog into the Experiences repository.
- 2026-10-17: Added candidate-based host-manifest switching/recomposition, active-manifest reset, fresh bundle factories, stable Hub manifest-state reset/activation, and Hub-lab controls; added tests for switch, reset, rollback, and safe paths.
- 2026-10-17: Updated the WebPage orchestration contract to link to the Experience-owned artifacts.
- Remaining: wire the host to resolve the Experience-owned manifest and content through the canonical repository/cache boundary; add user-facing selection; move the remaining demonstration/journey catalog; replace compiled route/presentation behavior only after the replacement is proven.
