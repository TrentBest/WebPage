# The Singularity Workshop — WebPage Agent Instructions

This repository is an active engineering proving ground and public learning resource.

## Branch safety

- development is active engineering.
- master is the stable promotion target.
- Do not modify master during ordinary development.
- Short-lived exploratory branches start from development, merge after proof/CI, then get deleted.
- Preserve valuable behavior before replacing it.

## Package-first rule

Before adding a local implementation, check the canonical Workshop packages.

Use:

- FSM_API for meaningful FSM behavior;
- FSM_COS for composition, dependency closure, arbitration, and RuntimeAssembly;
- MicroBundleDomain for the MicroBundle contract;
- the appropriate GUI package for reusable GUI behavior;
- other Workshop NuGet packages when they own the capability.

WebPage may provide browser adapters and proving-ground implementations. It must not silently fork reusable package responsibilities.

## Startup contract

The host manifest is the startup authority.

~~~text
manifest
   ↓
startup Experience(s)
   ↓
primary running Experience
~~~

The current primary root is LivingGuiExperienceMicroBundle 2102.

Its canonical Moniker dependency is 2110.

The host requests the primary root. FSM_COS resolves the dependency closure.

Do not manually compose the Moniker as a second root.

## FSM_API ownership

FSM_API is the authoritative state-machine implementation.

- State semantics belong to FSM_API.
- Processing-group ownership must remain explicit.
- UI components observe state; they do not become the state machine.
- Avoid anonymous timers for meaningful lifecycle transitions.
- Tests must prove lifecycle ownership.

## FSM_COS ownership

FSM_COS is the composition boundary.

WebPage may load the manifest, create RuntimeManifest, provide the MicroBundle catalog, execute FSM_COS, consume RuntimeAssembly, and present the result in the browser.

WebPage must not duplicate dependency traversal, implement a second arbitration engine, make FSM_COS understand browser concerns, or make reusable packages depend upward on WebPage.

## Documentation standard

Every authoritative document distinguishes Current, Direction, and Future.

Prefer:

> why the boundary exists → what the code does → how another developer uses it → what proves it

Do not turn current documentation into a fossil record of every experiment.

## Visual standard

When visual behavior matters:

- show it before explaining it;
- use diagrams for relationships;
- keep the visual claim truthful;
- inspect responsive behavior across client sizes.

## Tests are architectural evidence

Do not weaken tests to make a patch pass.

When a lifecycle or ownership contract changes, update its proof.

## Completion bar

Before calling work complete:

1. build;
2. run relevant tests;
3. inspect behavior;
4. update authoritative documentation;
5. verify GitHub Actions;
6. confirm dependency direction;
7. leave master untouched unless promotion was explicitly requested.

Zero warnings and zero avoidable errors is the target.
## Host-only rule

WebPage is the ultimate host/orchestrator for Workshop Experiences. Its responsibility is limited to:

- loading and validating the host manifest;
- selecting startup/running Experiences as declared by that manifest;
- invoking FSM_COS to assemble the requested RuntimeAssembly;
- providing browser-environment concerns such as navigation, viewport/client sizing, browser events, user input transport, authentication/session handoff, and browser persistence/data adapters;
- presenting state and capabilities produced by Workshop packages through GUI.Blazor or other canonical package surfaces;
- exposing learning/deep-dive views that observe real package/runtime state.

WebPage must not own reusable domain behavior, autonomous entity behavior, MicroBundle lifecycle semantics, composition/arbitration semantics, rendering-engine semantics, or a second application/game heartbeat.

If WebPage currently contains such behavior, treat it as migration debt:

1. identify the canonical Workshop NuGet package that should own it;
2. preserve the existing behavior with an architectural test or runtime proof;
3. replace the WebPage implementation with the package contract/runtime;
4. move only browser-specific manifestation/input/data adaptation into WebPage;
5. remove the duplicate implementation after proof;
6. update the learning documentation so the browser demonstrates the package rather than pretending WebPage invented it.

### Browser boundary

The browser is an environment, not the Workshop runtime. Browser concerns include DOM/rendering manifestation, viewport measurement, pointer/keyboard/touch input, navigation, authentication/session state, local/browser storage, network transport, and user-facing presentation. Those concerns may be implemented in WebPage or through canonical GUI/browser packages.

Everything that can exist independently of a browser should be pulled downward into a Workshop package and consumed here.

### Current migration targets

The following WebPage-owned implementations are explicitly transitional and must not become new foundations:

- Services/PageFSM.cs
- Services/LivingGuiFsm.cs
- Services/PageStateContext.cs
- Services/FSMManagerService.cs
- Services/BlazorFSMIntegration.cs
- Workshop/MicroBundles/MicroBundle.cs and its companion local lifecycle contracts
- any Razor component that directly advances those runtimes rather than observing a manifest-selected RuntimeAssembly

Do not delete these blindly. First identify the package/runtime replacement and preserve proof of the behavior being migrated.

