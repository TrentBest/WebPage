# WebPage as a Manifest-Defined WebApp

> **Architectural invariant:** WebPage should ultimately be a WebApp—AnyApp's web manifestation—not a compiled website whose identity and behaviors are fixed in Razor pages. The compiled host supplies stable runtime machinery; manifests, Experiences, and MicroBundles define what the running application is.

## The correction

WebPage is currently a Blazor WebAssembly application and a proving ground for Workshop capabilities. That implementation fact must not define the target architecture.

**WebApp is AnyApp for the web.** A WebApp is a manifest-driven application host. WebPage should ultimately be one concrete WebApp running on that host, with its visible identity, routes, rooms, interactions, and behaviors composed from declared Experiences and MicroBundles. It should be possible to redefine the WebPage experience without recompiling the host, within the capabilities exposed by its installed runtime and trusted MicroBundles.

This is stronger than loading a manifest that selects one hard-coded Razor page. The manifest must be authoritative for the application composition, and the host must interpret it rather than silently reconstructing a fixed default application in code.

## Target architecture

~~~text
COMPILED WEBAPP HOST (stable machinery)
  ├─ manifest/configuration loader
  ├─ capability and artifact resolution
  ├─ lifecycle + FSM_API execution integration
  ├─ FSM_COS composition boundary
  ├─ browser input / rendering / navigation adapters
  └─ policy, validation, and failure handling
                 |
                 v
WEBAPP MANIFEST + CONFIGURATION
  ├─ identity / entry Experience
  ├─ available and active Experiences
  ├─ MicroBundle roots and configuration
  ├─ navigation / presentation declarations
  └─ permissions and compatibility requirements
                 |
                 v
EXPERIENCE COMPOSITION
  ├─ Experiences describe the environments and journeys
  └─ MicroBundles provide reusable capabilities and behaviors
                 |
                 v
FSM_COS resolves dependency closure → RuntimeAssembly
                 |
                 v
HOST INTERPRETS AND RUNS THE DECLARED COMPOSITION
                 |
                 v
A WEBAPP THAT CAN BE REDEFINED WITHOUT RECOMPILING ITS HOST
~~~

The host is the machinery, not the application definition. The manifest is the application-level declaration, not merely startup decoration. Experiences compose what the user encounters; MicroBundles provide reusable behavior and capabilities. FSM_COS owns dependency closure and composition; FSM_API owns state-machine semantics and execution primitives. WebPage owns browser-specific adapters and presentation mechanics, not a second copy of those responsibilities.

For the operational sequence and acceptance gates, see [WebApp Showcase Orchestration and Living Documentation](WEBAPP_SHOWCASE_ORCHESTRATION.md). That contract also requires GUI.SVG or the owning renderer to produce visual evidence from the real model wherever a concept can be shown, while clearly distinguishing SVG presentation from FSM execution and GPU rendering.

## What “without recompiling” means

The target is **runtime redefinition through data and already-available capabilities**, not magical execution of arbitrary new code.

Without recompiling the host, an authorized manifest/configuration update should be able to change, where supported:

- which Experience opens first and which Experiences are available;
- the navigation and room/scene composition;
- which compatible MicroBundles participate and how they are configured;
- the behavior, presentation, and content contributed by those Experiences and MicroBundles;
- the composition's transitions and lifecycle within the host's declared runtime contracts.

A manifest cannot conjure a capability that is neither installed nor resolvable. New executable code may require a separately trusted, versioned MicroBundle artifact and may be subject to compatibility, security, and policy checks. The host must validate manifests, resolve dependencies, report missing/incompatible capabilities, and fail predictably. Reconfiguration must not require a rebuild merely because the application's composition changed.

## Ownership and boundaries

| Concern | Canonical owner |
|---|---|
| State-machine semantics and execution primitives | FSM_API |
| MicroBundle identity and contract | MicroBundleDomain |
| Dependency closure, loading, arbitration, convergence, RuntimeAssembly | FSM_COS |
| Environment, user journey, declared composition | Experience and its manifest contracts |
| Reusable behavior and capability | The MicroBundle/package that owns it |
| Browser input, rendering adapters, URL integration, browser lifecycle | WebApp host |
| WebPage's own selected identity/content | WebPage's WebApp manifest, Experiences, and MicroBundles—not hard-coded page-specific runtime logic |

AnyApp and WebApp are sibling host manifestations. AnyApp supplies the desktop-specific lifecycle and presentation boundary; WebApp supplies the browser-specific one. Neither should fork the reusable Experience behavior or FSM_COS composition logic.

## Current implementation facts — not target-state claims

The development branch already has useful foundations:

- a host manifest contract and startup/primary Experience declarations;
- FSM_COS composition that resolves a primary root and its declared dependency closure;
- a real RuntimeAssembly artifact;
- browser presentation and navigation;
- FSM Forge authoring/preview scaffolding.

But the current host still has transitional page-specific lifecycle/runtime paths, and significant behavior is expressed in compiled Razor/C# code. A manifest selecting an Experience does **not** by itself prove that the whole application can be redefined without recompilation. Forge authoring/preview does not yet prove that an authored application artifact can be loaded by WebApp as the authoritative runtime definition.

These are explicit gaps to close, not capabilities to claim as already complete.

## Migration sequence

1. **Inventory behavior currently hard-coded in the host.** Identify navigation, room selection, entry sequence, stateful interactions, and presentation behavior. Record each behavior, its current owner, tests, and intended MicroBundle/Experience owner.
2. **Define the versioned WebApp manifest contract.** Ground it in existing package contracts. It must describe the application composition, not just a fixed list of boot screens. Include schema/version compatibility, capability references/configuration, routing/presentation declarations, and failure behavior where supported.
3. **Make manifest interpretation authoritative.** The host must not override a supplied manifest with hard-coded page identity, default room lists, or parallel lifecycle rules. Any defaults must be explicit, documented, and distinguishable from manifest-specified values.
4. **Move behavior into Experiences and MicroBundles.** Keep the browser host generic. Put reusable behavior in its canonical package; use WebPage-local bundles only for truly host-specific adapters or experiments.
5. **Connect author → compose → run.** An authored manifest and its selected artifacts must pass through the canonical composition boundary, produce a resolved RuntimeAssembly, and drive the actual browser Experience. Preview must exercise the same contracts as runtime, not a decorative parallel implementation.
6. **Prove hot redefinition.** With one built host instance, change a test manifest/configuration and demonstrate a meaningfully different application composition without rebuilding the host. Verify lifecycle cleanup, dependency resolution, and errors for unavailable capabilities.
7. **Retire duplicate implementation only after parity.** Preserve the existing opening sequence, routing, browser storage/JS interop, Living GUI behavior, and Deep Dive behavior through executable tests before deleting transitional paths.
8. **Verify and document.** Add contract tests, runtime integration tests, and browser smoke tests; update the functionality-preservation ledger and this document from observed behavior. Keep master untouched and do not publish or deploy without explicit approval.

## Proof obligations

A meaningful proof must demonstrate all of the following:

1. The host loads a versioned manifest at runtime.
2. The manifest—not a hard-coded page branch—selects the Experience and declared capabilities.
3. FSM_COS resolves the requested roots and dependency closure; the resulting RuntimeAssembly is inspectable.
4. The host executes/presents that resolved composition rather than a duplicate compiled experience path.
5. A manifest/configuration change produces a different running WebApp without recompilation.
6. Missing, incompatible, or unauthorized MicroBundles fail visibly and safely.
7. Existing WebPage journeys and behavior-preservation tests remain green.
8. CI verifies the same contract; documentation distinguishes current proof from future direction.

## Branch and release discipline

- Work on development; preserve the two-branch Sith policy (master and development only).
- Do not modify or promote master without explicit approval.
- Do not publish packages or deploy a public site without explicit approval.
- Do not invent APIs or claim that planned dynamic loading, runtime replacement, or general authoring is already implemented.

> **The host is compiled once. The WebApp is defined by its manifest. Experiences compose the environment. MicroBundles supply the behaviors. The runtime makes that composition real.**
