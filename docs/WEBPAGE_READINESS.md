# WebPage Readiness Checklist

This is the active readiness ledger for the browser host. It distinguishes verified repository/CI evidence from work that remains incomplete. Do not treat a successful build as proof that every architectural migration is complete.

## Current baseline

- Branch: `development` (integration branch).
- Latest completed successful .NET CI validation: [run 39179278861](https://github.com/TrentBest/WebPage/actions/runs/39179278861) — **success**, commit `879a62a0191a53a608b678c41ad4c8dab723bf10`. Browser-host restore/build, browser-journey smoke test, solution build, SingularityHub tests, LivingGui Experience tests, Pong Experience tests, Codecov upload, and Workshop repository-source integration all completed successfully. The checklist-only follow-up commit `131d9abe4d1e15a20db8e66d1e40d104b07d7de9` is currently validating in [run 39179783300](https://github.com/TrentBest/WebPage/actions/runs/39179783300): host restore/build passed, browser-journey smoke is in progress, and repository-source integration passed. Earlier successful baseline: [run 39178605389](https://github.com/TrentBest/WebPage/actions/runs/39178605389).
- Azure Static Web Apps workflow [run 39179189929](https://github.com/TrentBest/WebPage/actions/runs/39179189929) — **skipped**. This is not evidence of a deployment.
- No package publication or deployment is authorized by this checklist.

## Release completion estimate

**Estimated release readiness: 60%** — an engineering judgment, not a test pass rate or an automated metric. This is a baseline estimate as of 2026-10-19 and should move only when evidence changes.

Why 60%:
- **Validated foundation:** manifest loading and composition, dependency-closure tests, the existing FSM_API-driven page/Living GUI behavior, browser smoke coverage, and cross-repository source integration are present.
- **Major release gate still open:** the browser's manifest-selected `RuntimeAssembly` is not yet proven to own or activate the primary Experience lifecycle. The current 33 ms WebPage heartbeat still advances the compatibility `PageFSM → LivingGuiFsm` path.
- **Remaining release proof:** implement and test the host-side execution/observation bridge without losing behavior; verify the full visitor journey and responsive layout; reconcile public docs/visuals; confirm final CI and release safeguards.
- **Confidence:** medium-low for the percentage itself because the host bridge is a substantial architectural gate whose true scope will become clearer during implementation. The listed source/test evidence is higher-confidence than the numeric estimate.

Progress scale: 0% = no release evidence; 100% = all release gates evidenced, CI green for the final code, and explicit human approval obtained for any publication/deployment. CI success alone cannot move the estimate past the execution-ownership gate.

## Readiness gates

### Host contract and composition

- [x] Host manifest is loaded and parsed by `WorkshopExperienceService`.
- [x] Manifest selects startup and primary-running Experience entries.
- [x] Primary root 2102 declares the canonical Moniker dependency 2110.
- [x] FSM_COS composition proof asserts dependency closure for 2102 → 2110.
- [x] Composition catalog creates isolated bundle instances for separate compositions.
- [ ] Add/confirm an end-to-end test proving the actual browser's startup-to-primary transition is driven by the manifest-selected `RuntimeAssembly`, not only by the transitional page runtime. The latest browser smoke test passes, but that is not yet proof that the primary lifecycle is owned by `RuntimeAssembly`.

Evidence: `SingularityHub.Tests/WebPageHostManifestTests.cs`, `TheSingularityWorkshop/Services/WorkshopExperienceService.cs`, and `TheSingularityWorkshop/Workshop/Composition/WorkshopCompositionCatalog.cs`.

### Execution ownership and behavior preservation

- [x] Migration debt is explicitly documented; transitional execution is not falsely described as migrated.
- [x] Preservation contract covers the 100-slot population, independent organism FSMs, deterministic targets, travel, planting, growth, reproduction, reduction/recovery, gravity, and browser coordinate space.
- [ ] Define and implement the WebPage host-side Experience execution bridge, or verify an existing reusable Experience runtime contract that WebPage can consume. FSM_COS explicitly leaves Experience startup, FSM_API scheduling, and host lifecycle to the host; do not treat their absence from `RuntimeAssembly` as a defect in the composition kernel.
- [ ] Render the browser view from that owner/runtime state instead of maintaining a parallel WebPage-owned execution path.
- [ ] Keep the existing behavior tests green during the migration; remove old paths only after replacement and proof.
- [ ] Resolve the neutral resource/compute declaration contract with the owning MicroBundleDomain/FSM_COS packages before implementing it in WebPage.

Evidence: `docs/FUNCTIONALITY_PRESERVATION.md` and `docs/CURRENT_VERTICAL_SLICE.md`.

### Visitor experience and living documentation

- [ ] Inspect the actual browser journey at desktop, tablet, and narrow/mobile widths: gateway → Moniker → primary Experience.
- [ ] Confirm the experience fits the client area and does not introduce unintended page scrolling or clipped controls.
- [ ] Ensure every diagram, image, and status claim distinguishes a runnable demonstration from an architecture illustration or future direction.
- [ ] Reconcile the public README, current vertical slice, and visible behavior after each architecture change.

### CI and release safeguards

- [x] Latest observed .NET CI run is successful (link above), including browser journey smoke and cross-repository source integration.
- [ ] After the next code change, verify the new commit's .NET tests and relevant browser smoke test.
- [ ] Review all workflow outcomes; a skipped deploy workflow must remain described as skipped, not deployed.
- [ ] Confirm `master` remains untouched and only `master` / `development` branches exist.
- [ ] Explicit human approval is required before publishing packages, creating a release, or deploying.

## Immediate next engineering task

Close the gap between **composition** and **execution**. The manifest and FSM_COS can produce a `RuntimeAssembly`, but the preservation ledger currently records the browser as still using `FSMManagerService → PageFSM → LivingGuiFsm → PageStateContext`. The `WebPageCosRuntime` search hit is from an indexed historical commit, not a file currently retrievable on `development`; it is not evidence of a usable primary-Experience runtime contract. The current `IdleExperienceHost.razor` also still injects `FSMManagerService`, so it does not establish the missing ownership boundary. First define the host-side execution/observation contract that consumes `RuntimeAssembly` and connects the manifest-selected Experience bundles to the existing behavior. FSM_COS documents the intended boundary as **composition ends at `RuntimeAssembly`; the host starts Experiences, schedules FSM_API process groups, and manifests the result**. MicroBundleDomain's `IMicroBundle` contract is intentionally limited to `Load` and `Arbitrate`; it does not promise an Experience scheduler or state-observation API. Therefore the immediate gap is a proven WebPage host bridge (unless a separate reusable Experience runtime is verified), not adding host execution responsibilities to FSM_COS or MicroBundleDomain. Add executable proof for the browser handoff. Do not delete the transitional path or simulate a successful migration with documentation alone.

### Verified implementation blocker (current `development`)

- `TheSingularityWorkshop/Workshop/MicroBundles/LivingGuiExperienceMicroBundle.cs` declares itself a composition capability. Its `Load` only validates the context, and `Arbitrate` returns `false`; it exposes no simulation state, lifecycle transitions, or tick/observation API. Its XML summary explicitly says the living organism is scheduled by host FSM_API process groups.
- `TheSingularityWorkshop/Services/FSMManagerService.cs` owns a 33 ms `PeriodicTimer` heartbeat and calls `Page.Update()`; `PageFSM.Tick` advances `LivingGuiFsm.Update()` while populating and separately updates the gravity process group. This is direct source evidence that the current execution owner is still the WebPage compatibility runtime, even though the manifest-selected bundle has been composed.
- `WorkshopExperienceService.RuntimeAssembly` proves composition and dependency closure, not a runnable Living GUI Experience. A WebPage-only rewrite cannot safely transfer execution to an API that the composed bundle does not expose. The next implementation gate is an explicit WebPage host bridge for Experience startup, scheduling, and state observation, unless a separately owned reusable Experience runtime is found and verified. FSM_COS's own `RUNTIME_ASSEMBLY.md` and `RUNTIME_BOUNDARY.md` explicitly assign these host responsibilities outside the composition kernel. Keep the current behavior path until the bridge and browser handoff regression test prove parity.

