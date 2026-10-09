# WebPage Creation Architecture — WebApp-Adjacent, FSM_COS-Driven

> **Purpose:** evolve WebPage from a hand-authored, monolithic presentation/runtime toward a manifest-driven Workshop experience that can, in theory, author and compose webpages using the Workshop's own WebApp technology.

## Corrected architectural interpretation

**WebApp is not a deployment target.** It is the Workshop's browser-application technology/manifestation, analogous to AnyApp's desktop manifestation. Choosing a web server, static host, WebAssembly, or Interactive Server is a separate implementation and deployment decision; none of those choices defines what WebApp means.

WebPage predates WebApp and is the Workshop's public proving ground and authoring experience. The goal is not simply to rewrite WebPage using a different hosting framework. The goal is for WebPage to demonstrate the Workshop's ability to describe, compose, present, and eventually create webpages from declared Experiences and capabilities.

The intended conceptual flow is:

```text
WEBPAGE / WORKSHOP AUTHORING EXPERIENCE
       │ create, configure, preview
       ▼
PAGE MANIFEST + CONFIGURATION
       │ declares the intended Experience and capabilities
       ▼
MICROBUNDLE DOMAIN / REPOSITORY
       │ describes and locates capabilities
       ▼
FSM_COS → RuntimeAssembly
       │ composes requested capability/dependency closure
       ▼
WEBAPP BROWSER MANIFESTATION
       │ renders the composed Experience
       ▼
CREATED WEBPAGE
```

AnyApp is a sibling manifestation for desktop experiences. WebPage and WebApp must consume canonical package/runtime behavior rather than becoming separate copies of FSM_COS or of reusable Experience logic.

## Current / Direction / Future

### Current — verified from the WebPage development branch

- The main WebPage project uses `Microsoft.NET.Sdk.BlazorWebAssembly`.
- It directly references FSM_API 1.0.13, MicroBundleDomain 1.0.1, FSM_COS 0.1.0-alpha.5, GUI.Blazor 0.1.0-alpha.8, ProtocolAi, and GrammarAi.
- Startup manifest composition resolves the Living GUI root (2102) and its declared Moniker dependency (2110) through FSM_COS.
- The composed `RuntimeAssembly` exists, but browser execution still depends on the transitional `FSMManagerService → PageFSM → LivingGuiFsm → PageStateContext` path.
- The existing WebApp repository is a sibling browser-application project. Its implementation and contract must be inspected directly before claiming WebPage has adopted or is built on top of it.
- The current WebPage deployment workflow uploads published `wwwroot` to Azure Static Web Apps. This describes today's deployment path only; it does not define the WebApp architecture.

### Direction — next implementation slices

1. **Inspect WebApp as a technology.** Read its actual project, manifest/startup path, FSM_COS integration, GUI/rendering boundary, and tests. Identify what WebPage can consume or reuse without assuming WebApp is a hosting service or that its current implementation already supports page authoring.
2. **Dependency/release checkpoint.** Verify the HeadlessAi candidate's exact branch, tests, package contents, documentation, and disabled publication gate. Verify the MicroBundleDomain and FSM_COS contracts WebPage intends to consume. Do not infer package readiness from an open PR or an older green run.
3. **Define the webpage artifact contract.** Establish what a created page consists of: manifest, configuration, selected Experiences/MicroBundles, presentation metadata/assets, and any required routing or sharing metadata. Reuse existing contracts where they exist; do not invent APIs in a package that does not own them.
4. **Prove author → compose → preview.** Start with one small, manifest-driven page authored through the Workshop path. Prove the manifest is the source of composition, FSM_COS resolves the intended dependency closure, and the browser presents the composed Experience.
5. **Separate rendering from runtime ownership.** WebApp owns browser-specific presentation and interaction. FSM_API owns state-machine execution; FSM_COS owns composition; MicroBundleDomain owns canonical capability contracts. Reusable Experience behavior belongs to its canonical package, not duplicated in each host.
6. **Preserve existing WebPage behavior.** Keep current entry/Moniker, Living GUI, routing, JavaScript interop, browser storage, and Deep Dive behavior until replacements are executable and regression-tested.
7. **Choose deployment only after runtime requirements are known.** Determine whether the selected WebApp rendering mode requires a running ASP.NET Core process or can be served statically. Then align deployment configuration with that proven choice. Do not switch to Interactive Server or change the production host merely because it was mistaken for the meaning of WebApp.
8. **Public release proof.** Update the functionality ledger, architecture docs, tests, and visual evidence. Prepare the WebPage update for review; do not promote, publish, or deploy it without explicit approval.

### Future — not yet claimed

- WebPage can author and configure a page artifact from Workshop-owned manifests and capabilities.
- A created page can be previewed and presented by WebApp without hand-written host-specific runtime behavior.
- The browser is fully driven by canonical Experience runtime state rather than transitional WebPage lifecycle classes.
- A completed author → compose → preview → publish → share flow.
- A production-ready public Workshop.

## Non-negotiable preservation gates

A refactor must preserve and prove:

- the first-contact / Moniker / primary Experience sequence;
- manifest-selected Experience and FSM_COS dependency closure;
- deep-link routing and not-found behavior;
- JavaScript interop, viewport sizing, and browser storage;
- the Living GUI's 100-slot population, independent organism FSMs in one processing group, deterministic target distribution, travel, planting, growth, reproduction, parent recovery, gravity, and arrival/navigation sequence;
- Deep Dive's distinction between manifest-requested roots, resolved dependency closure, declared dependencies, and browser presentation;
- responsive behavior and the existing visual identity;
- the created-page artifact can be consumed by the chosen WebApp presentation path.

Do not delete `PageFSM`, `LivingGuiFsm`, `PageStateContext`, or `FSMManagerService` until replacement behavior is executable, tested, consumed by WebPage, and documented.

## Release and branch discipline

- Work on `development` or a short-lived branch based on it.
- Keep `master` stable and untouched until explicit promotion approval.
- Keep NuGet publication disabled by default; a successful build or package artifact is not publication authorization.
- Do not merge the FSM_COS alpha.6 integration branch while it is explicitly gated on package availability unless the owner chooses a documented alternative.
- Do not describe a proposed PR, a local package, or a source-based CI run as a published NuGet dependency.
- If a dependency's required contract is unavailable, document the blocker and keep the existing behavior rather than fabricating a compatibility API.

## Definition of done

This direction is review-ready when the actual WebApp implementation has been inspected, the page-artifact contract is grounded in real package APIs, one author → compose → preview vertical slice works, existing WebPage behavior remains proven, runtime ownership is not duplicated, documentation matches the implementation, relevant CI/tests pass, and release/deployment remain under explicit owner control.

*The manifest describes the page. The composition system assembles its capabilities. WebApp presents the Experience. WebPage helps create it.*
