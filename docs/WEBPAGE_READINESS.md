# WebPage Readiness Checklist

![Living GUI documentation lens: question, model, implementation, visible manifestation, and proof](https://raw.githubusercontent.com/TrentBest/WebPage/development/docs/assets/living-gui-documentation-lens.svg)

> **Living GUI lens.** Trace each claim to source, visible manifestation, and executable evidence. The shared animated loop is an orientation illustration—not runtime telemetry. Label implemented, illustrated, proposed, and unverified claims honestly.

This is the active release ledger for the browser host. A green build is necessary, not sufficient: the browser must demonstrate the promised behavior and architecture boundaries.

## Current baseline

- **Integration branch:** `development`.
- **Current audited commit:** [`41add99`](https://github.com/TrentBest/WebPage/commit/41add99fae4f0b2f4476a09ff6c8a2c21e7f60f5).
- **Latest .NET CI:** [run 39183749004](https://github.com/TrentBest/WebPage/actions/runs/39183749004) — **success** on that exact commit. The Workshop repository-source integration job completed successfully, including dependency checkouts, .NET setup, WebPage restore/build, Renderer source smoke test, and city-rendering proof screenshot.
- **Azure Static Web Apps:** [run 39183749042](https://github.com/TrentBest/WebPage/actions/runs/39183749042) — **skipped**. No deployment is claimed.
- **Documentation rollout:** the shared lens is embedded in all 88 indexed Markdown documents under `docs/`; this is not the same as completing each subject-specific visual/evidence audit. See [coverage ledger](LIVING_GUI_COVERAGE.md).
- **Branch hygiene:** `development`, `master`, and an extra `fix/restore-founder-entry-sequence` branch currently exist. The extra branch is a release-hygiene item; it has not been deleted here.
- No package publication, release creation, or deployment has been performed.

## Release readiness

**Estimated release readiness: 65%.** This is an engineering estimate, not a test pass rate. It has moved from 60% because the latest audited commit now has a completed green .NET CI run, the source-integration/Renderer smoke evidence is confirmed, and the architecture/theory pages have a source-backed evidence map. Documentation coverage is also no longer waiting for the shared lens rollout.

Why it is **not** higher yet:
- The manifest-selected `RuntimeAssembly` proves composition and dependency closure, but does **not** own or activate the primary Experience lifecycle.
- The current 33 ms WebPage heartbeat still advances the compatibility `PageFSM → LivingGuiFsm → PageStateContext` path.
- No end-to-end test yet proves that the manifest-selected primary Experience owns the visitor-visible lifecycle and that the browser renders observations from that owner.
- Actual desktop/tablet/mobile visitor review and final release safeguards remain outstanding.

**Confidence:** high for the cited CI outcome and source facts; medium-low for the percentage because the execution bridge remains a substantial engineering gate. The number must rise through implementation and verification, not by relabeling open gates.

Progress scale: 0% = no release evidence; 100% = release gates verified, final-code CI green, visitor experience reviewed, and explicit human authorization obtained for publication/deployment. Readiness does not itself authorize publication.

## Readiness gates

### Host contract and composition

- [x] Host manifest is loaded and parsed by `WorkshopExperienceService`.
- [x] Manifest selects startup and primary-running Experience entries.
- [x] Primary root 2102 declares the canonical Moniker dependency 2110.
- [x] FSM_COS composition proof asserts dependency closure for 2102 → 2110.
- [x] Composition catalog creates isolated bundle instances for separate compositions.
- [ ] Add an end-to-end test proving startup-to-primary transition is driven by the manifest-selected `RuntimeAssembly`, not only the transitional page runtime.

Evidence: `SingularityHub.Tests/WebPageHostManifestTests.cs`, `TheSingularityWorkshop/Services/WorkshopExperienceService.cs`, and `TheSingularityWorkshop/Workshop/Composition/WorkshopCompositionCatalog.cs`.

### Execution ownership and behavior preservation — principal release blocker

- [x] Migration debt is documented honestly; transitional execution is not falsely described as migrated.
- [x] Preservation contract covers the 100-slot population, independent organism FSMs, deterministic targets, travel, planting, growth, reproduction, reduction/recovery, gravity, and browser coordinate space.
- [ ] Define and implement the WebPage host-side Experience execution bridge, or verify an existing reusable Experience runtime contract.
- [ ] Render the browser view from that owner/runtime state instead of maintaining a parallel WebPage-owned execution path.
- [ ] Keep existing behavior tests green during migration; remove old paths only after replacement and parity proof.
- [ ] Resolve the neutral resource/compute declaration contract with the owning MicroBundleDomain/FSM_COS packages before implementing it in WebPage.

Evidence: [functionality preservation ledger](FUNCTIONALITY_PRESERVATION.md), [current vertical slice](CURRENT_VERTICAL_SLICE.md), `WorkshopExperienceService.cs`, `FSMManagerService.cs`, `PageFSM.cs`, and `LivingGuiExperienceMicroBundle.cs`.

The current `LivingGuiExperienceMicroBundle` is a composition capability: `Load` validates context and `Arbitrate` returns false; it does not expose simulation state, lifecycle transitions, or a tick/observation API. `FSMManagerService` owns a 33 ms `PeriodicTimer` and calls `Page.Update()`. Therefore composition is real, but execution ownership has not yet moved. FSM_COS assigns Experience startup, scheduling, and host lifecycle to the host; do not add these responsibilities to the composition kernel or pretend a docs-only change closes the gap.

### Visitor experience and living documentation

- [ ] Inspect the actual browser journey at desktop, tablet, and narrow/mobile widths: gateway → Moniker → primary Experience.
- [ ] Confirm the experience fits the client area without unintended page scrolling or clipped controls.
- [x] Shared lens embedded across all 88 indexed Markdown documents under `docs/`.
- [ ] Complete subject-specific visual/evidence audit: each document must link its actual view, source, fixture, browser journey, or test—not merely repeat the shared illustration.
- [ ] Reconcile public README, current vertical slice, and visible behavior after architecture changes.
- [ ] Add validation so new or edited documents cannot silently omit their living-view entry point and evidence classification.

Tracking: [living GUI coverage ledger](LIVING_GUI_COVERAGE.md), [architecture audit](ARCHITECTURE_AUDIT.md), and [theory](THEORY.md). The coverage ledger intentionally distinguishes shared-lens coverage from subject-specific completion.

### CI and release safeguards

- [x] Latest observed .NET CI run is successful on audited commit `41add99`.
- [x] Workshop repository-source integration and Renderer source smoke test completed successfully in that run.
- [x] Azure workflow outcome reviewed; it was skipped, not deployed.
- [ ] After the next code change, verify the new commit's .NET tests and relevant browser smoke test.
- [ ] Reconcile branch hygiene with the repository's master/development-only policy; an extra fix branch currently exists.
- [ ] Confirm release versioning, package/workflow gates, and public-facing documentation on the final candidate.
- [ ] Explicit human approval before publishing packages, creating a release, or deploying.

## Immediate engineering sequence

1. **Define the host contract** for startup, FSM_API scheduling, lifecycle ownership, and observable state. Check for an existing reusable runtime in the consuming packages before designing a WebPage-only abstraction.
2. **Connect the manifest-selected assembly to the contract** with explicit failure behavior; composition success must not silently masquerade as execution success.
3. **Bridge the browser presentation to authoritative state** and expose a small runtime-backed evidence view with manifest/version, selected Experience, composition/execution status, active lifecycle state, population/phase, and source/test links.
4. **Add regression proof** for startup → Moniker → Living GUI → population/reproduction/gravity/navigation, retaining the current implementation until behavioral parity is demonstrated.
5. **Run the full CI/browser checks**, review responsive layout, then reconcile docs and release workflow safeguards.

Do not remove the transitional runtime before its replacement passes parity tests. Do not claim the shared SVG is live telemetry. Do not publish or deploy without explicit authorization.
