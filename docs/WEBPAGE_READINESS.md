# WebPage Readiness Checklist

![Living GUI documentation lens: question, model, implementation, visible manifestation, and proof](https://raw.githubusercontent.com/TrentBest/WebPage/development/docs/assets/living-gui-documentation-lens.svg)

> **Living GUI lens.** Trace each claim to source, visible manifestation, and executable evidence. The shared animated loop is an orientation illustration—not runtime telemetry. Label implemented, illustrated, proposed, and unverified claims honestly.

This is the active release ledger for the browser host. A green build is necessary, not sufficient: the browser must demonstrate the promised behavior and architecture boundaries.

## Current baseline

- **Integration branch:** `development`.
- **Latest audited runtime candidate:** [`9d65f67`](https://github.com/TrentBest/WebPage/commit/9d65f67c9c59355becc907869642778cbcc1b219); later commits in this ledger are documentation-only.
- **Current `development` HEAD:** [`e2b40c7`](https://github.com/TrentBest/WebPage/commit/e2b40c74aff88ce42fd4b3854896b22087e17967).
- **Latest successful .NET CI on current HEAD:** [run 4105 / 39311139425](https://github.com/TrentBest/WebPage/actions/runs/39311139425) — **success** on `e2b40c7`. Both the test job and Workshop repository-source integration job completed successfully.
- **Azure Static Web Apps:** run [1133 / 39309999012](https://github.com/TrentBest/WebPage/actions/runs/39309999012) was **skipped**. Both deployment-related jobs were skipped; no deployment is claimed. Run 1132 / 39286432512 is the prior skipped run.
- **Documentation rollout:** the shared lens is embedded in all 88 indexed Markdown documents under `docs/`; this is not the same as completing each subject-specific visual/evidence audit. See [coverage ledger](LIVING_GUI_COVERAGE.md).
- **Branch hygiene:** `development`, `master`, and an extra `fix/restore-founder-entry-sequence` branch currently exist. The extra branch is a release-hygiene item; it has not been deleted here.
- No package publication, release creation, or deployment has been performed.

## Release readiness

**Estimated release readiness: 72%.** This is an engineering estimate, not a test pass rate. The on-demand `RuntimeObservationPanel` displays an immutable snapshot through the active execution adapter alongside manifest identity/version, selected startup and primary Experiences, composition result, execution-owner disclosure, lifecycle-binding status, and direct source/test links. The host presentation now reads through `FSM.ExecutionRuntime`; the adapter rejects assemblies missing required Experience bundles, and regression tests verify adapter tick/observation routing. The full Playwright journey and both CI jobs passed on runtime candidate [`9d65f67`](https://github.com/TrentBest/WebPage/commit/9d65f67c9c59355becc907869642778cbcc1b219) in [run 4100 / 39278781274](https://github.com/TrentBest/WebPage/actions/runs/39278781274); subsequent documentation-only HEAD [`e2b40c7`](https://github.com/TrentBest/WebPage/commit/e2b40c74aff88ce42fd4b3854896b22087e17967) passed both .NET workflow jobs in [run 4105 / 39311139425](https://github.com/TrentBest/WebPage/actions/runs/39311139425). This improves the host seam and evidence, but does not transfer lifecycle execution ownership to an Experience-owned runtime.

Why it is **not** higher yet:
- The manifest-selected `RuntimeAssembly` proves composition and dependency closure, but does **not** own or activate the primary Experience lifecycle.
- The current 33 ms WebPage heartbeat still advances the compatibility `PageFSM → LivingGuiFsm → PageStateContext` path.
- No end-to-end test yet proves that the manifest-selected primary Experience owns the visitor-visible lifecycle and that the browser renders observations from that owner.
- Actual desktop/tablet/mobile visitor review and final release safeguards remain outstanding.

**Confidence:** high for the cited CI outcome and source facts; medium-low for the percentage because the execution bridge remains a substantial engineering gate. The number must rise through implementation and verification, not by relabeling open gates.

Progress scale: 0% = no release evidence; 100% = technical release gates verified, final-code CI green, and visitor experience reviewed. Readiness does not itself authorize publication or deployment; explicit human approval remains a separate required release action.

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
- [x] Define a host-side `IExperienceExecutionRuntime` seam and manifest-bound compatibility adapter; validate that every declared Experience MicroBundle exists in the composed assembly before activation.
- [x] Route host ticks, state reads, and immutable observations through the active adapter; preserve the same `PageFSM` during the compatibility phase.
- [ ] Replace the compatibility adapter's delegated `PageFSM` lifecycle with an Experience-owned execution contract, then render from that authoritative owner without a parallel execution path.
- [ ] Keep existing behavior tests green during migration; remove old paths only after replacement and parity proof.
- [ ] Resolve the neutral resource/compute declaration contract with the owning MicroBundleDomain/FSM_COS packages before implementing it in WebPage.

Evidence: [functionality preservation ledger](FUNCTIONALITY_PRESERVATION.md), [current vertical slice](CURRENT_VERTICAL_SLICE.md), `WorkshopExperienceService.cs`, `FSMManagerService.cs`, `PageFSM.cs`, and `LivingGuiExperienceMicroBundle.cs`.

The current `LivingGuiExperienceMicroBundle` is a composition capability: `Load` validates context and `Arbitrate` returns false; it does not expose simulation state, lifecycle transitions, or a tick/observation API. `WorkshopExperienceService.SwitchManifestAsync` composes and activates `RuntimeAssembly`, records the manifest-selected startup/primary entries, and sets its presentation state to `Moniker`; that is composition activation, not primary lifecycle execution. `FSMManagerService` still owns a 33 ms `PeriodicTimer` and calls `ExecutionRuntime.Tick()`; the current `PageFsmExecutionRuntime` delegates that tick to `Page.Update()`. Therefore composition and adapter binding are real, but lifecycle ownership has not yet moved to the selected Experience. FSM_COS assigns Experience startup, scheduling, and host lifecycle to the host; do not add these responsibilities to the composition kernel or pretend a docs-only change closes the gap.

### Visitor experience and living documentation

- [ ] Inspect the actual browser journey at desktop, tablet, and narrow/mobile widths: gateway → Moniker → primary Experience.
- [ ] Confirm the experience fits the client area without unintended page scrolling or clipped controls.
- [x] Shared lens embedded across all 88 indexed Markdown documents under `docs/`.
- [ ] Complete subject-specific visual/evidence audit: each document must link its actual view, source, fixture, browser journey, or test—not merely repeat the shared illustration.
- [x] Add an on-demand runtime-observation component backed by one immutable snapshot, with lifecycle flags, tick counters, population, and an explicit execution-ownership caveat.
- [x] Verify panel open, refresh, and close interactions in the Playwright browser journey.
- [x] Expose manifest/version, selected startup/primary Experiences, composition result, current execution owner, manifest-lifecycle binding status, and source/test links; verify those fields in Playwright.
- [ ] Replace the compatibility execution path with a manifest-selected runtime contract and prove behavior parity.
- [ ] Reconcile public README, current vertical slice, and visible behavior after architecture changes.
- [ ] Add validation so new or edited documents cannot silently omit their living-view entry point and evidence classification.

Tracking: [living GUI coverage ledger](LIVING_GUI_COVERAGE.md), [architecture audit](ARCHITECTURE_AUDIT.md), and [theory](THEORY.md). The coverage ledger intentionally distinguishes shared-lens coverage from subject-specific completion.

### CI and release safeguards

- [x] Runtime candidate `9d65f67` passed both .NET CI jobs, including the browser journey, execution-adapter binding/tick tests, Living GUI tests, Experience tests, Renderer source smoke, and city-rendering proof screenshot: [run 4100 / 39278781274](https://github.com/TrentBest/WebPage/actions/runs/39278781274).
- [x] Current documentation-only HEAD `e2b40c7` passed both .NET workflow jobs: [run 4105 / 39311139425](https://github.com/TrentBest/WebPage/actions/runs/39311139425). Earlier documentation-only HEADs `a171c2d` and `b89cc8a` also passed both jobs in [run 4104 / 39309999095](https://github.com/TrentBest/WebPage/actions/runs/39309999095) and [run 4103 / 39286432177](https://github.com/TrentBest/WebPage/actions/runs/39286432177), respectively.
- [x] Earlier successful code-candidate run 39195465334 remains historical evidence for `8ec877b`; it is not used as the current baseline.
- [x] Previously inspected Azure workflow outcome was skipped, not deployed; no deployment is claimed.
- [x] Verify the immutable runtime-observation API on commit `4f56ba5`: [CI run 39185455518](https://github.com/TrentBest/WebPage/actions/runs/39185455518) passed both jobs.
- [x] Verify the initial visitor-visible runtime-observation component on commit `4f3fd79`: [CI run 39192108299](https://github.com/TrentBest/WebPage/actions/runs/39192108299) passed both jobs.
- [x] Verify manifest/composition/execution diagnostics and their browser assertions on commit `6fa578e`: [CI run 39192970280](https://github.com/TrentBest/WebPage/actions/runs/39192970280) passed both jobs.
- [ ] Reconcile branch hygiene with the repository's master/development-only policy; an extra fix branch currently exists.
- [ ] Confirm release versioning, package/workflow gates, and public-facing documentation on the final candidate.
- [ ] Explicit human approval before publishing packages, creating a release, or deploying.

## Immediate engineering sequence

1. [x] **Define the host seam.** `IExperienceExecutionRuntime` now exposes lifecycle actions, ticking, owner identity, and immutable observation; the current implementation remains a clearly named compatibility adapter.
2. [x] **Bind the selected Experience.** The manifest-selected assembly is validated for required MicroBundle IDs and the adapter is activated with the manifest; failed candidate composition/binding must leave the prior active runtime intact.
3. [x] **Expose evidence in the living GUI.** The on-demand observation panel reports manifest identity, composition, selected Experience, adapter ownership/binding, lifecycle state, population/phase, and source/test links. Browser smoke verifies opening, refresh, closing, and mobile fit.
4. **Transfer lifecycle ownership and prove parity.** Replace the compatibility adapter's delegated `PageFSM` tick with an Experience-owned runtime contract and regression-test startup → Moniker → Living GUI → population/reproduction/gravity/navigation before removing the transitional path.
5. **Finish visitor and release review.** Complete desktop/tablet/mobile visual review, subject-specific living-documentation evidence audits, branch hygiene, and release-workflow safeguards. Run final CI on the final code candidate.

Do not remove the transitional runtime before its replacement passes parity tests. Do not claim the shared SVG is live telemetry. Do not publish or deploy without explicit authorization.
