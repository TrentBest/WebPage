# WebPage Readiness Checklist

This is the active readiness ledger for the browser host. It distinguishes verified repository/CI evidence from work that remains incomplete. Do not treat a successful build as proof that every architectural migration is complete.

## Current baseline

- Branch: `development` (integration branch).
- Latest observed .NET CI run: [run 39148410137](https://github.com/TrentBest/WebPage/actions/runs/39148410137) — **success**, updated 2026-10-18 17:15 UTC, commit `ae302eef5b36f8412b71227e513b4d021ca77e01`.
- Latest observed Azure Static Web Apps workflow: [run 39148410141](https://github.com/TrentBest/WebPage/actions/runs/39148410141) — **skipped**. This is not evidence of a deployment.
- No package publication or deployment is authorized by this checklist.

## Readiness gates

### Host contract and composition

- [x] Host manifest is loaded and parsed by `WorkshopExperienceService`.
- [x] Manifest selects startup and primary-running Experience entries.
- [x] Primary root 2102 declares the canonical Moniker dependency 2110.
- [x] FSM_COS composition proof asserts dependency closure for 2102 → 2110.
- [x] Composition catalog creates isolated bundle instances for separate compositions.
- [ ] Add/confirm an end-to-end test proving the actual browser's startup-to-primary transition is driven by the manifest-selected `RuntimeAssembly`, not only by the transitional page runtime.

Evidence: `SingularityHub.Tests/WebPageHostManifestTests.cs`, `TheSingularityWorkshop/Services/WorkshopExperienceService.cs`, and `TheSingularityWorkshop/Workshop/Composition/WorkshopCompositionCatalog.cs`.

### Execution ownership and behavior preservation

- [x] Migration debt is explicitly documented; transitional execution is not falsely described as migrated.
- [x] Preservation contract covers the 100-slot population, independent organism FSMs, deterministic targets, travel, planting, growth, reproduction, reduction/recovery, gravity, and browser coordinate space.
- [ ] Move Living GUI lifecycle/simulation authority to its canonical reusable Experience/runtime owner.
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

- [x] Latest observed .NET CI run is successful (link above).
- [ ] After the next code change, verify the new commit's .NET tests and relevant browser smoke test.
- [ ] Review all workflow outcomes; a skipped deploy workflow must remain described as skipped, not deployed.
- [ ] Confirm `master` remains untouched and only `master` / `development` branches exist.
- [ ] Explicit human approval is required before publishing packages, creating a release, or deploying.

## Immediate next engineering task

Close the gap between **composition** and **execution**. The manifest and FSM_COS can produce a `RuntimeAssembly`, but the preservation ledger currently records the browser as still using `FSMManagerService → PageFSM → LivingGuiFsm → PageStateContext`. First identify a canonical runtime owner and its state-observation contract, then add executable proof for the browser handoff. Do not delete the transitional path or simulate a successful migration with documentation alone.
