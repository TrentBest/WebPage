# WebPage Host Boundary Migration

## Purpose

WebPage is the browser host and ultimate orchestrator for The Singularity Workshop. It is not another Workshop runtime.

The intended dependency direction is:

```text
Browser environment
       │
       ▼
    WebPage
       │  manifest / user input / browser data / presentation
       ▼
   FSM_COS
       │
       ▼
  MicroBundle graph
       │
       ▼
   FSM_API + Workshop packages
```

WebPage should assemble and present capabilities; it should not recreate capabilities that can exist independently of a browser.

## What WebPage owns

These are legitimate host responsibilities:

- loading and validating the host manifest;
- selecting startup and running Experiences from the manifest;
- invoking FSM_COS and consuming RuntimeAssembly;
- browser navigation and routing;
- viewport/client measurement and responsive presentation;
- pointer, keyboard, touch, and other browser input transport;
- authentication/session handoff;
- browser persistence and browser-specific data adapters;
- presenting package-owned state through GUI.Blazor;
- learning/deep-dive views that inspect real package/runtime state.

A browser adapter may translate browser events into a package API. It must not become the owner of the underlying domain behavior.

## What WebPage must stop owning

The following are migration debt, not foundations:

| Current WebPage implementation | Intended owner |
|---|---|
| PageFSM lifecycle/state machine | FSM_API through the manifest-selected Experience/runtime |
| LivingGuiFsm / autonomous Living GUI behavior | canonical Experience/MicroBundle package composed by FSM_COS |
| PageStateContext runtime state | canonical Experience/MicroBundle runtime |
| FSMManagerService heartbeat | package/runtime scheduler; WebPage observes it |
| BlazorFSMIntegration | remove once remaining legacy demos are migrated |
| local MicroBundle lifecycle shell | MicroBundleDomain + concrete package implementation |
| local composition/arbitration behavior | FSM_COS |
| local GUI primitives where a canonical GUI package exists | GUI.Blazor / appropriate GUI package |
| user input semantics | FSM_UserIO or the appropriate domain package; WebPage only transports browser events |
| user/profile/data domain semantics | Profiles and appropriate Workshop data packages; WebPage only hosts browser/session adapters |

## Migration rule

For every local implementation:

1. Identify what behavior it owns.
2. Identify the canonical Workshop NuGet package that should own that behavior.
3. Preserve the existing behavior with an architectural/runtime test.
4. Replace the local implementation with the package contract/runtime.
5. Keep only the browser adapter in WebPage.
6. Remove the duplicate implementation.
7. Update the learning documentation to show the actual package boundary.

Do not replace working behavior with empty stubs merely to make the code smaller.

## Current proof

The active manifest path already demonstrates the correct composition boundary:

```text
webpage-host.manifest.json
        │
        ▼
Living GUI root 2102
        │
        └── declares Moniker 2110
                    │
                    ▼
               FSM_COS
                    │
                    ▼
             RuntimeAssembly
```

The remaining work is to make the browser manifestation consume that runtime instead of continuing to run the older WebPage-owned PageFSM/LivingGui runtime beside it. This is a functionality-preserving migration: the old runtime remains until its behavior has a canonical replacement and executable proof.

## Current / Direction / Future

### Current

WebPage has a working manifest → FSM_COS → RuntimeAssembly proof, but legacy page/runtime services still coexist with it. The browser therefore has two architectural layers: canonical composition and transitional execution. The latter is migration debt, not yet safe to delete.

### Function-preservation gate

The complete Living GUI behavior contract is recorded in [`FUNCTIONALITY_PRESERVATION.md`](FUNCTIONALITY_PRESERVATION.md). It must be preserved during extraction; reducing WebPage line count is not considered progress if the experience stops growing, reproducing, moving independently, falling, or presenting its navigation handoff.

### Direction

One manifest-selected runtime path. WebPage becomes a thin browser host that:

- loads the manifest;
- requests composition;
- supplies browser environment/input/data adapters;
- observes package-owned runtime state;
- renders the result.

### Future

The same Experience should be consumable by other hosts without copying WebPage's runtime implementation. Browser, desktop, distributed, and other hosts should differ primarily in environment adapters and presentation, not in the underlying Experience definition.
