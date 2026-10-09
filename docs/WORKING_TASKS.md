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

- [ ] Check CI for the latest `TrentBest/TheSingularityWorkshop.FSM_COS` `development` commit, including the duplicate-root and conflicting-version regression tests.
- [ ] If CI has not run, trigger/enable the appropriate verification path or run the available checks; do not mark the gate complete until current code is verified.
- [ ] Confirm repeated requests for the same root ID/version load once.
- [ ] Confirm conflicting versions for the same root ID fail before any bundle loads.
- [ ] Confirm NuGet publishing remains explicitly disabled by the `&& false` gate.
- [ ] Record the verified pause-gate result in the FSM_COS pause document. Then only fix correctness blockers; defer unrelated features.

### P0 — Prepare WebPage for a truthful public launch

- [ ] Inspect current manifest, startup sequence, hub configuration, deployment workflow, repository visibility/settings, and latest CI state.
- [ ] Confirm the first-run flow and primary manifest Experience agree with the intended public story: moniker first, then the hub; keep the initial public surface deliberately small (one useful, honest primary entry point rather than a gallery of unimplemented tabs).
- [ ] Resolve any mismatch between the actual active manifest and public-tab documentation. Current docs have described Rendering as the only active tab, while the host service composes a Living GUI primary Experience plus Moniker; verify the actual manifest before changing either.
- [ ] Build and test the current WebPage branch; fix launch-blocking errors, responsive layout defects, broken assets/manifest paths, and misleading or nonfunctional controls.
- [ ] Check the public experience at desktop and narrow/mobile viewport sizes where runtime tooling permits. Keep the moniker readable and the hub usable.
- [ ] Verify the Azure Static Web Apps deployment configuration and required secret *presence/status only*; never print or expose secret values.
- [ ] Confirm what is currently live, if anything, and whether a deployment from `master` is needed. Do not claim a launch or deployment until the deployment run and public URL are verified.
- [ ] Produce a short public launch checklist and a truthful Brave1 submission summary grounded in the working site and actual package architecture.

### P1 — Make RuntimeAssembly the real Experience authority

- [ ] Trace the complete path from manifest → FSM_COS → `RuntimeAssembly` → Experience behavior → browser presentation.
- [ ] Current documented gap: `Home.razor` / `LivingGui.razor` still rely on `FSMManagerService → PageFSM → LivingGuiFsm → PageStateContext` after composition. Do not claim the migration is complete.
- [ ] Identify the smallest behavior that can be driven directly by the composed Experience/MicroBundle now, without breaking the existing proven Living GUI lifecycle.
- [ ] Add executable tests before migrating ownership; preserve independent FSM instances, deterministic Squirrel-based distribution, growth/reproduction, gravity, moniker timing, and responsive panel-relative positioning.
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

## Session-end update template

At the end of each work session, update this file with:
- what changed and commit SHA(s);
- current branch head(s);
- actual test/CI status and the run/commit it applies to;
- current blocker(s);
- the exact next action.

**Do not mark work complete because code was written.** Mark it complete only when its stated verification condition has been met.
