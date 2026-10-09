# WebPage as the FSM_COS Proving Ground

> **Status:** Direction — this document defines the migration contract. It is not a claim that every legacy implementation has already been removed.

## 01 — Why this boundary exists

WebPage is the browser host and public proving ground for The Singularity Workshop. It must demonstrate the Workshop's reusable behavior rather than quietly becoming a second implementation of it.

The distinction matters because a behavior implemented only inside WebPage cannot be reliably reused by AnyApp, WebApp, a service, a simulation, or a future distributed host. The owning package must define the behavior; WebPage should demonstrate it through a real browser.

## 02 — Responsibility map

```text
FSM_API
  owns state-machine semantics and processing behavior
        |
        v
MicroBundleDomain
  owns the MicroBundle contract
        |
        v
MicroBundle repository / catalog
  locates and supplies MicroBundle artifacts
        |
        v
FSM_COS
  resolves requested dependencies, arbitrates, composes
  and returns RuntimeAssembly
        |
        v
WebPage host
  loads/validates host manifest, supplies catalog,
  invokes composition, handles browser-specific concerns
        |
        v
GUI / browser manifestation
  presents the assembled Experience and transports user input
```

This diagram describes ownership, not a requirement that every box be a direct project reference. Each dependency must follow the canonical package's actual public contract.

**The direction is downward for reusable capability and outward for presentation.** FSM_API and reusable Workshop packages must not depend on FSM_COS or WebPage. FSM_COS must not know about DOM, Blazor components, viewport sizes, browser storage, or browser navigation.

## 03 — Startup and Experience selection

The host manifest is the startup authority.

1. WebPage loads and validates the manifest.
2. WebPage identifies the requested primary Experience from that manifest.
3. WebPage supplies the relevant MicroBundle catalog and composition request to FSM_COS.
4. FSM_COS resolves the dependency closure and returns a RuntimeAssembly.
5. WebPage presents the assembled Experience and adapts browser input/environment events.
6. UI observes the resulting state; it must not become a second state machine or runtime heartbeat.

The current documented primary root is `LivingGuiExperienceMicroBundle` (2102), with its canonical Moniker dependency (2110). Request the root once; let FSM_COS resolve its dependency closure. Do not manually compose the Moniker as a second root.

## 04 — Extraction rule

A proving-ground implementation is useful evidence, not automatic permanent ownership.

```text
observe existing WebPage behavior
        ↓
name the behavior and its contract
        ↓
identify the canonical owning package
        ↓
add/adjust package implementation and tests
        ↓
replace WebPage duplicate with package consumption
        ↓
prove browser integration and preserve visible behavior
        ↓
remove obsolete implementation
```

Do not delete transitional code before its replacement and proof exist. Do not preserve duplicate implementations merely because they are already convenient.

### Migration targets already identified

The repository's agent instructions identify these as transitional and specifically prohibit blind deletion:

- `Services/PageFSM.cs`
- `Services/LivingGuiFsm.cs`
- `Services/PageStateContext.cs`
- `Services/FSMManagerService.cs`
- `Services/BlazorFSMIntegration.cs`
- `Workshop/MicroBundles/MicroBundle.cs` and companion local lifecycle contracts
- Razor components that directly advance local runtimes instead of observing a manifest-selected RuntimeAssembly

For each target, record: current behavior, intended owner, replacement API, preserved proof/test, browser-only remainder, and removal status. If a reusable capability has no suitable owning package yet, document the gap and establish the contract before inventing another WebPage-only foundation.

## 05 — What WebPage owns

WebPage may own browser-specific adaptation and host responsibilities:

- manifest loading and validation;
- selection of manifest-declared startup Experiences;
- invoking FSM_COS and supplying a catalog/provider;
- viewport measurement and responsive presentation;
- DOM and browser lifecycle integration;
- pointer, keyboard, and touch input transport;
- navigation, authentication/session handoff, and browser persistence adapters;
- presentation of actual package/runtime state;
- deep dives that inspect real runtime/package behavior.

WebPage must not own reusable domain behavior, autonomous entity behavior, MicroBundle lifecycle semantics, composition/arbitration semantics, rendering-engine semantics, or a competing application/game heartbeat.

## 06 — Documentation contract

Every authoritative document distinguishes:

- **Current** — implemented and observable now.
- **Direction** — actively being established or migrated.
- **Future** — deliberately not promised by today's implementation.

Explain in this order:

1. Why the boundary exists.
2. What the code does today.
3. What contract crosses the boundary.
4. How another developer uses or extends it.
5. What proves the claim (tests, CI, runtime evidence, or responsive screenshots).
6. What does not belong here and which package owns it.

Show architecture before lengthy explanation when a diagram materially clarifies the relationship. Visuals must describe real behavior, not aspirations disguised as completed work. WebPage explains how it consumes FSM_COS; FSM_COS remains authoritative for its own domain and theory.

## 07 — Two-branch working agreement

The desired repository shape is exactly:

- `master` — stable promotion target.
- `development` — active integration and engineering branch.

Normal work happens on `development`. Keep master untouched during ordinary development. Do not create permanent feature, docs, release-preparation, or experiment branches. When a temporary branch is genuinely necessary, consolidate it into development, verify CI, and delete it.

Before removing an existing branch, compare it with development/master and preserve any unique valuable commits. A branch cleanup must not silently discard implementation, documentation, or architectural decisions. Branch count is a housekeeping goal, not permission to lose work.

No NuGet publication or release is implied by a green build or merge. Explicit approval is required.

## 08 — Completion checklist

For each extraction slice:

- [ ] State the current behavior and the owner that should replace it.
- [ ] Confirm the intended package contract exists (or create it in the owning repository).
- [ ] Keep dependencies pointed from host to packages, never upward.
- [ ] Add or update tests proving behavior and lifecycle ownership.
- [ ] Replace the WebPage duplicate with package consumption.
- [ ] Keep browser-only adapters in the host.
- [ ] Update README/docs and diagrams to match implementation.
- [ ] Build and run relevant tests; inspect warnings and failures.
- [ ] Inspect browser behavior at multiple viewport sizes.
- [ ] Verify GitHub Actions and dependency versions.
- [ ] Confirm only master/development remain after safe branch consolidation.
- [ ] Do not publish without explicit approval.

## Current / Direction / Future

**Current:** WebPage's own instructions already define FSM_API as state-machine authority, FSM_COS as composition boundary, and the manifest as startup authority. Several local FSM and MicroBundle implementations are explicitly marked as migration debt.

**Direction:** Make the manifest → FSM_COS → RuntimeAssembly → browser presentation path the actual path for the primary Experience. Extract behavior to its owning Workshop package, preserve proof, and carry the FSM_COS documentation standard into WebPage's public and engineering docs.

**Future:** A WebPage host where the visible Experience can be changed by manifest/catalog selection without re-implementing its behavior in the browser host, and where deep dives explain the real packages and runtime that produced what the visitor sees.

---

*The Singularity Workshop — show the behavior, preserve the semantics, explain the machinery.*
