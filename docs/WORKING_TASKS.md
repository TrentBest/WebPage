# Working Task List — Self-Priming Handoff

> **Last updated:** 2026-10-09  
> **Active workstream:** FSM_COS pause gate → WebPage public launch  
> **Primary branch:** `development`  
> **No-publish rule:** Never publish a NuGet package without explicit user approval. Keep NuGet workflow publish gates at `&& false` by default.

This is the durable handoff for continuing work after a conversation reset. Read this file first, inspect the current GitHub branch/CI state, and continue the highest-priority unfinished task. Do not assume this document's commit SHAs or CI status are still current; verify them.

## Self-prime instructions

When resuming, do the following without asking the user to repeat context:

1. Read this task list and the linked FSM_COS pause gate.
2. Inspect the current `development` heads and recent commits in both repositories.
3. Check actual workflow runs for the current commits. Never infer CI success from an older green run.
4. Continue the first unfinished P0/P1 item below. Make concrete, scoped commits on `development`; report changed files, commit links, test/CI evidence, and remaining blockers.
5. Do not merge to `master`, deploy the public site, change package versions, or publish NuGet packages unless the appropriate explicit approval has been given. WebPage public launch is urgent, but a deploy must be intentional and verified.
6. Keep this list updated as work completes, priorities change, or a meaningful blocker is discovered.

## Current decision

FSM_COS is being brought to a **bounded pause point**, not declared feature-complete or release-ready. Its kernel should remain minimal and compose capability MicroBundles without taking hard dependencies on every package. WebPage is the next proving ground: the site must demonstrate that the manifest and FSM_COS-produced `RuntimeAssembly` meaningfully drive the Experience, rather than merely compose successfully before a parallel host-owned runtime takes over.

Reference: [FSM_COS Pause Point](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/blob/development/docs/PAUSE_POINT.md)

## Prioritized tasks

### P0 — Finish the FSM_COS pause gate, then stop broadening the kernel

- [x] FSM_COS regression suite passed in [run 37979475866](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/actions/runs/37979475866) for source head `e721e0b52fecbbd0aaafc84a8c8e5e1e2bd4f440`, including the root deduplication and conflicting-version tests.
- [x] CI was triggered by the development push and completed successfully; `build-and-test` passed.
- [x] Same root ID/version deduplication is covered by the passing FSM_COS regression suite.
- [x] Conflicting versions for the same root ID are rejected before loading, covered by the passing FSM_COS regression suite.
- [x] Verified the workflow retains `inputs.publish == true && false`; the `publish_nuget` job was skipped.
- [x] Recorded the passing gate in [FSM_COS PAUSE_POINT.md](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/blob/development/docs/PAUSE_POINT.md). FSM_COS is now paused for broad feature work; only correctness blockers or narrow WebPage-unblocking fixes should reopen it.

### P0 — Prepare WebPage for a truthful public launch

- [x] Inspect current manifest, startup sequence, hub configuration, deployment workflow, repository visibility, and current CI state. The manifest is `TheSingularityWorkshop/wwwroot/Workshop/Forge/StreamingAssets/Experiences/webpage-host.manifest.json`; Azure deploys only on `master`; WebPage and FSM_COS CI runs for the task-list commits were still in progress at the last check.
- [x] Confirm the host service loads the manifest, composes the Living GUI primary root (2102) with its Moniker dependency through FSM_COS, composes hub MicroBundles separately, and transitions from Moniker to Hub. The current hub item is `Rendering` (4100), so the active destination still needs to be reconciled with the intended initial AnyApp-first public plan.
- [x] Resolved the initial-hub mismatch: manifest and `PUBLIC_WORKSHOP_TABS.md` now agree on a single AnyApp hub destination. `/anyapp` is now a responsive, honest architecture demo; it explicitly says no public installer is currently offered and links to the source and Rendering research.
- [x] Build and test the current WebPage code. The latest source head [`f1c586f`](https://github.com/TrentBest/WebPage/commit/f1c586fd2d7ee6ce179511fab0dbe02aa3750c65) passed [`.NET Tests` run 37980195235](https://github.com/TrentBest/WebPage/actions/runs/37980195235); the run includes a zero-warning/error build and the three configured test suites.
- [ ] Visually inspect the public experience at desktop and narrow/mobile viewport sizes. The layout has responsive rules and the source build passed before the supporting-capability follow-up, but a live browser viewport check has not been completed in this session.
- [x] Verified the development deployment workflow is configured to publish a prebuilt WebPage and uses the existing Azure token secret reference without exposing its value. Development deployment is gated by an explicit `[deploy-public]` commit marker; ordinary development pushes do not deploy.
- [x] Confirmed the last successful public deployment was from `master` SHA `725536d375eb2fe5f9bf5a4088b32b57bd082fe5` on 2026-10-04. Azure reported the site at https://lemon-ground-09f542010.1.azurestaticapps.net. The current development code has not yet been deployed.
- [x] Created [BRAVE1_SUBMISSION.md](BRAVE1_SUBMISSION.md), a truthful technical submission draft that distinguishes implemented behavior from ongoing work and avoids unsupported performance/mission claims. The draft still needs adaptation to the actual Brave1 form before submission.

### P1 — Make RuntimeAssembly the real Experience authority

- [x] Traced manifest → FSM_COS → `RuntimeAssembly` and the transitional browser presentation path. The moniker is now rendered from `RuntimeAssembly`'s composed `GuiNode` via `BlazorGuiRenderer`; the broader Living GUI execution path remains transitional.
- [x] Confirmed and documented the gap: `LivingGui.razor` still relies on `FSMManagerService → PageFSM → LivingGuiFsm → PageStateContext` after composition. Do not claim the migration is complete.
- [ ] Identify the smallest behavior that can be driven directly by the composed Experience/MicroBundle now, without breaking the existing proven Living GUI lifecycle.
- [ ] Add/extend executable parity tests before migrating Living GUI ownership; preserve independent FSM instances, deterministic Squirrel-based distribution, growth/reproduction, gravity, moniker timing, and responsive panel-relative positioning.
- [ ] Keep browser-only rendering/input in WebPage and reusable domain behavior in the canonical Experience/MicroBundle owner. Do not delete transitional code until parity is proven.

### P1 — Keep the MicroBundle/package boundary disciplined

- [ ] For each candidate package, ask whether it is required by the kernel, an optional runtime capability, or a development/test-only dependency.
- [ ] Prefer an optional MicroBundle when it materially improves manifest discovery, configuration, or composition; do not wrap every NuGet package mechanically.
- [ ] Keep FSM_COS dependent only on foundational contracts it truly needs (currently FSM_API and MicroBundleDomain, verify against the project file).
- [ ] Do not integrate every Workshop package before launch. Choose the smallest credible runtime assembly that proves the architecture.
- [ ] Keep package version changes and NuGet releases out of this launch task unless separately approved.

### P2 — Post-launch improvements (do not block the initial public release unless one becomes a launch blocker)

- [ ] Add a dashboard-style deep dive that exposes only parameters genuinely wired to presentation/runtime behavior.
- [ ] Expand Explore/Create and other Workshop tabs from manifest-defined capabilities when they have real functionality.
- [ ] Continue moving legacy host-owned lifecycle/runtime logic into reusable package-owned Experiences with tests.
- [ ] Improve visual polish and documentation diagrams without displacing launch-blocking verification.

## Architectural guardrails

- FSM_API → FSM_Layer → FSM_COS → consuming applications/hosts; lower packages must not depend upward on FSM_COS.
- FSM_COS is the composition boundary (“assembly crane”), not a GUI framework, rendering engine, application, or execution loop.
- A package existing on NuGet does not mean the runtime should consume it. Minimize required dependencies; represent optional capabilities as MicroBundles when that adds real value.
- Preserve working behavior during refactors; cleanup is not success if it deletes functionality.
- Keep the creator story honest: future discoverability/distribution/monetization are aspirations unless implemented.
- Use `master` and `development`; avoid introducing `main`.
- User's `@GitHub` means continue independently and make concrete progress. Ask only when a real decision/approval is required.

## Latest verified snapshot (2026-10-09)

- WebPage source head [`f1c586f`](https://github.com/TrentBest/WebPage/commit/f1c586fd2d7ee6ce179511fab0dbe02aa3750c65) passed [.NET Tests run 37980195235](https://github.com/TrentBest/WebPage/actions/runs/37980195235). The suite initially caught a brittle test that assumed JSON arrays were single-line; the assertion was corrected to ignore whitespace, and the updated suite passed. The supporting-capability change passed the current-head [.NET Tests run 37980715414](https://github.com/TrentBest/WebPage/actions/runs/37980715414) on commit `5fc44be76d45cca42cda36bc8904885ea5df5c05`.
- FSM_COS source/tests at [`e721e0b`](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/commit/e721e0b52fecbbd0aaafc84a8c8e5e1e2bd4f440) passed [run 37979475866](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/actions/runs/37979475866): build-and-test succeeded; publish job was skipped. The pause-gate documentation update is [`a575f18`](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/commit/a575f18987da399e1f1a0ea184c63d01cc48ede7). No NuGet publish is authorized.
- WebPage repo is public. Latest successful deployment is [Azure run 37980337029](https://github.com/TrentBest/WebPage/actions/runs/37980337029), which deployed `development` SHA [`271e93e`](https://github.com/TrentBest/WebPage/commit/271e93ee9a791bccbaa3d3aefe968d168f50a972) to https://lemon-ground-09f542010.1.azurestaticapps.net on 2026-10-09. The newer supporting-capability commits are not live yet. Current `development` and `master` are substantially diverged (`development` was 1,692 commits ahead and 56 behind, with 300 changed files at comparison time); **do not merge the whole branch or open a giant PR**. Public rollout is being done from `development` via the explicitly gated `[deploy-public]` marker instead.
- `Home.razor` renders the moniker from the composed MicroBundle GUI tree, then the manifest hub. The only hub destination is now AnyApp. `/anyapp` is a responsive architecture/status demo. A follow-up now declares Rendering as a supporting capability separate from hub navigation so `/rendering` can retain its interactive distance/perception probe without becoming a second hub tab; that follow-up still needs current-head CI and redeployment.
- Architecture gap remains: FSM_COS produces a `RuntimeAssembly` and the moniker now uses its composed GUI tree, but active Living GUI browser behavior still goes through the transitional WebPage-local FSM path. This is documented migration debt, not a reason to block all public launch work.

## Immediate next action

The user explicitly authorized getting WebPage public today. The current public site deployment succeeded from `development` SHA `30a396130afd3e0ded75e8a8a5c878d323d85407` in [Azure run 37980896257](https://github.com/TrentBest/WebPage/actions/runs/37980896257), at https://lemon-ground-09f542010.1.azurestaticapps.net. That deployed version includes the single AnyApp hub, moniker rendering from the composed MicroBundle GUI tree, route fallback, and a separately declared Rendering supporting capability. The deployed source passed [.NET Tests run 37980715414](https://github.com/TrentBest/WebPage/actions/runs/37980715414); a second test run [37980896380](https://github.com/TrentBest/WebPage/actions/runs/37980896380) also passed on deployment SHA `30a396`. A documentation-only Brave1 draft and repository-link correction were added afterward at commits `6659a2c` and `5cf600a`. No application source changed after the successful deployment. Do not merge the diverged development branch into master and do not publish NuGet.

## Session-end update template

At the end of each work session, update this file with:
- what changed and commit SHA(s);
- current branch head(s);
- actual test/CI status and the run/commit it applies to;
- current blocker(s);
- the exact next action.

**Do not mark work complete because code was written.** Mark it complete only when its stated verification condition has been met.
