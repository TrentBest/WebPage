# Workshop Package Ecosystem

WebPage is the browser proving ground for The Singularity Workshop. Its job is to **demonstrate the ecosystem**, not recreate it.

The most useful way to read this repository is to ask, for every capability:

> **Which Workshop package owns this behavior, and what is WebPage doing only because it is the browser host?**

## Current direct package boundary

| Capability | Package | WebPage responsibility |
|---|---|---|
| Deterministic FSM lifecycle | `TheSingularityWorkshop.FSM_API` | Provide contexts/FSM definitions where a browser Experience genuinely needs autonomous behavior. |
| Runtime composition | `TheSingularityWorkshop.FSM_COS` | Supply the catalog and execute the manifest-selected runtime assembly. |
| MicroBundle domain contract | `TheSingularityWorkshop.MicroBundleDomain` | Declare domain-owned bundle identity and configuration contracts; do not invent a second domain contract. |
| Browser GUI manifestation | `TheSingularityWorkshop.GUI.Blazor` | Render Workshop-owned GUI structures into the browser. |
| Protocol semantics | `TheSingularityWorkshop.ProtocolAi` | Provide protocol definitions where the WebPage AI demonstration uses them. |
| Grammar semantics | `TheSingularityWorkshop.GrammarAi` | Provide grammar definitions where the WebPage AI demonstration uses them. |

The project also contains the local `SingularityHub` assembly because the browser needs a host boundary and existing Hub-facing compatibility surface. That assembly is not a replacement for FSM_COS.

## Runtime path

```text
webpage-host.manifest.json
        |
        v
LivingGuiExperienceMicroBundle (2102)
        |
        | declared dependency
        v
MonikerMicroBundle (2110)
        |
        v
FSM_COS RuntimeAssembly
        |
        v
FSM_API-owned autonomous behavior + WebPage presentation
```

The important lesson is that **presentation order is not composition order**. The Moniker appears first because the manifest says it is the startup presentation. FSM_COS still receives only the primary Experience root and resolves the Moniker dependency from that root.

## Why the repository contains local code

WebPage necessarily owns browser concerns:

- loading a browser-host manifest;
- adapting browser lifecycle events to Workshop runtime boundaries;
- translating runtime state into Blazor presentation;
- providing host-specific adapters and visual proving-ground surfaces;
- demonstrating integrations that are intentionally browser-specific.

WebPage should **not** own a second implementation of:

- FSM execution;
- MicroBundle domain identity;
- dependency closure;
- composition arbitration;
- RuntimeAssembly construction;
- reusable package capabilities already provided elsewhere in the Workshop ecosystem.

## Transitional code

Older WebPage experiments predate the current package architecture. In particular, `Workshop/MicroBundles/MicroBundle.cs` and its companion local contracts contain a historical lifecycle shell built on FSM_API.

That code is a migration target, not a new architectural foundation. Concrete legacy demos should be migrated when their behavior becomes part of the active learning surface:

1. identify the behavior being demonstrated;
2. identify its canonical Workshop package owner;
3. preserve the behavior with tests;
4. move composition to FSM_COS where appropriate;
5. keep FSM_API as the execution authority for autonomous behavior;
6. remove the duplicate WebPage implementation only after the replacement is proven.

## Evidence rule

Every current architectural claim in this repository should be traceable to at least one of:

- executable code;
- an automated test;
- a runtime manifest;
- a composed `RuntimeAssembly`;
- a benchmark or measured result;
- a published Workshop package contract.

Ideas that are not yet implemented belong under **Direction** or **Future**, not under the current runtime contract.

## Learning sequence

1. **Start with WebPage** — understand the browser host and manifest.
2. **Read FSM_COS** — understand composition, dependency closure, arbitration, and RuntimeAssembly.
3. **Read MicroBundleDomain** — understand what a MicroBundle is without tying it to a host.
4. **Read FSM_API** — understand how autonomous behavior is actually executed.
5. **Read GUI.Blazor** — understand how Workshop-owned GUI structures become browser presentation.
6. **Read ProtocolAi / GrammarAi** — see how semantic capabilities can be composed independently.

WebPage is therefore a map into the Workshop ecosystem as much as it is an application.