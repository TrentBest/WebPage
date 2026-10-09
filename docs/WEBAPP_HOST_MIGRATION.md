# WebPage Host Migration — WebApp-Adjacent Architecture

> **Purpose:** move WebPage from its current Blazor WebAssembly host toward the same ASP.NET Core Razor Components hosting model used by TheSingularityWorkshop.WebApp, while preserving WebPage's public Experience, manifest, and learning surface.

## Decision

WebPage should remain the Workshop's public proving ground, but it should not remain a single Blazor WebAssembly application that owns a parallel version of the Workshop runtime.

The target host is aligned with the existing WebApp shape:

- ASP.NET Core host using `Microsoft.NET.Sdk.Web`;
- Razor Components with an explicit interactive render mode;
- FSM_API for state-machine execution;
- FSM_COS for composition and dependency closure;
- MicroBundleDomain for canonical capability contracts;
- WebPage-owned manifest loading, browser presentation, navigation, authoring, and education;
- independently owned packages for reusable Experience behavior.

This is a host-architecture migration, not permission to replace the current experience with a static mock or to delete existing runtime behavior.

## Current / Direction / Future

### Current — verified from the development branch

- The main WebPage project uses `Microsoft.NET.Sdk.BlazorWebAssembly`.
- It directly references FSM_API 1.0.13, MicroBundleDomain 1.0.1, FSM_COS 0.1.0-alpha.5, GUI.Blazor 0.1.0-alpha.8, ProtocolAi, and GrammarAi.
- Startup manifest composition resolves the Living GUI root (2102) and its declared Moniker dependency (2110) through FSM_COS.
- The composed `RuntimeAssembly` is real, but browser execution still depends on the transitional `FSMManagerService → PageFSM → LivingGuiFsm → PageStateContext) path.
- The WebApp repository already uses `Microsoft.NET.Sdk.Web`, `AddRazorComponents().AddInteractiveServerComponents()`, and `MapRazorComponents<App>().AddInteractiveServerRenderMode()`.

### Direction — next implementation slices

1. **Release dependency checkpoint.** Verify HeadlessAi's current branch, tests, package contents, documentation, and disabled publication gate. Verify the MicroBundleDomain and FSM_COS contracts WebPage intends to consume. Do not infer package readiness from an open PR or an older green run.
2. **Host conversion proof.** Establish a minimal ASP.NET Core Razor Components host on a short-lived branch from `development`, preserving the existing WebPage routes and service boundaries. Confirm static assets, routing, JavaScript interop, browser storage, and interactive lifecycle behavior before replacing the existing host.
3. **Runtime boundary.** Keep the host manifest as startup authority; pass the real FSM_COS `RuntimeAssembly` to the active Experience. Do not present composition as proof that the Experience is already executing through its canonical runtime.
4. **Behavior migration.** Extract reusable Living GUI lifecycle and actor behavior to its canonical Experience owner only when that owner and its regression proof exist. Keep WebPage as browser manifestation, not the duplicate execution authority.
5. **Cross-host proof.** Demonstrate the same intended Experience semantics in WebPage and WebApp/AnyApp without copying a second runtime implementation into either host.
6. **Public release proof.** Update the functionality ledger, architecture docs, screenshots or other visual evidence, and CI results. Prepare the WebPage update for review; do not promote or deploy it without explicit approval.

### Future — not yet claimed

- Every WebPage destination is manifest-backed.
- The browser is fully driven by canonical Experience runtime state rather than transitional WebPage lifecycle classes.
- WebPage and WebApp share all reusable Experience behavior.
- A completed author → compose → preview → publish → share flow.
- A production-ready public Workshop.

## Non-negotiable preservation gates

A host migration must preserve the existing visitor experience and runtime behavior. At minimum, prove:

- the first-contact / Moniker / primary Experience sequence;
- manifest-selected Experience and FSM_COS dependency closure;
- deep-link routing and not-found behavior;
- JavaScript interop, viewport sizing, and browser storage;
- the Living GUI's 100-slot population, independent organism FSMs in one processing group, deterministic target distribution, travel, planting, growth, reproduction, parent recovery, gravity, and arrival/navigation sequence;
- Deep Dive's distinction between manifest-requested roots, resolved dependency closure, declared dependencies, and browser presentation;
- responsive behavior and the existing visual identity.

Do not delete `PageFSM`, `LivingGuiFsm`, `PageStateContext`, or `FSMManagerService` until replacement behavior is executable, tested, consumed by WebPage, and documented.

## Release and branch discipline

- Work on `development` or a short-lived branch based on it.
- Keep `master` stable and untouched until explicit promotion approval.
- Keep NuGet publication disabled by default; a successful build or package artifact is not publication authorization.
- Do not merge the FSM_COS alpha.6 integration branch while it is explicitly gated on package availability unless the owner chooses a documented alternative.
- Do not describe a proposed PR, a local package, or a source-based CI run as a published NuGet dependency.
- If a dependency's required contract is unavailable, document the blocker and keep the existing behavior rather than fabricating a compatibility API.

## Definition of done

The migration is review-ready only when the host builds with warnings treated as errors, relevant tests pass, the existing visitor behavior is preserved, the actual FSM_COS assembly is used at the composition boundary, the browser is not running a duplicate lifecycle authority, the documentation matches the implementation, and the release/deployment remains under explicit owner control.

*The browser manifests the Experience. The runtime owns its behavior. The composition system assembles its capabilities.*
