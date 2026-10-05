# Workshop Runtime Architecture

> **This document describes the runtime that exists in WebPage today.**

WebPage is a browser proving ground. Reusable behavior is deliberately moving out of the page and into independently owned packages and composition participants.

## Ownership

~~~text
FSM_API
   └── deterministic state-machine behavior

MicroBundleDomain
   └── MicroBundle runtime contract

FSM_COS
   └── composition / dependency closure / arbitration / convergence

Experience layer
   └── describes what the host wants to compose

WebPage
   ├── visitor interaction
   ├── browser presentation
   ├── discovery / authoring UX
   └── proving-ground integration
~~~

> **A visual element being rendered by WebPage does not make WebPage the owner of the underlying capability.**

## Current first-contact runtime

~~~text
FirstContact
    ↓
visitor explicitly requests entry
    ↓
select LivingGuiExperience
    ↓
RuntimeManifest
    ↓
FSM_COS
    ├── resolve Living GUI
    ├── resolve Moniker dependency
    ├── load
    ├── arbitrate
    └── converge
    ↓
RuntimeAssembly
    ↓
Page / Living GUI runtime
    ↓
population threshold
    ↓
gravity
    ↓
Moniker presentation
    ↓
Workshop navigation
~~~

The Experience is not composed during service construction. It is composed only after explicit visitor entry.

## Host composition catalog

WorkshopCompositionCatalog implements the FSM_COS catalog boundary.

It currently supplies the WebPage composition participants, including Moniker, Living GUI Experience, Protocol, Grammar, and AI Exchange.

The catalog is host infrastructure. FSM_COS does not know whether participants came from an in-memory dictionary, generated registry, cache, or repository-backed resolver.

## Experience to manifest

WorkshopExperienceService owns the browser-side transition from selected Experience to composition request.

The essential operation is:

~~~csharp
RuntimeAssembly = _compositionSystem.Execute(
    new RuntimeManifest(
        RuntimeId: SelectedExperience.Id,
        Bundles: SelectedExperience.MicroBundleIds
            .Select(BundleRequest.Unconfigured)
            .ToArray()));
~~~

The host supplies the roots. FSM_COS derives the reachable composition.

## Dependency closure

The current Living GUI dependency is:

~~~text
LivingGuiExperience
        ↓
LivingGuiExperienceMicroBundle
        └── Moniker
~~~

The Experience names the root. The MicroBundle names its dependency. FSM_COS closes the graph.

If a dependency cannot be resolved, composition must fail rather than silently inventing a substitute.

## Installation versus execution

A successful RuntimeAssembly means:

- requested roots were resolved;
- dependencies were closed;
- reachable capabilities were installed;
- arbitration converged.

It does not mean the browser has executed the Experience.

After assembly, WebPage performs its host-specific transition into Living GUI presentation and FSM_API-driven behavior.

~~~text
composition complete
        ↓
RuntimeAssembly
        ↓
WebPage runtime
        ↓
FSM_API behavior
        ↓
browser manifestation
~~~

## Runtime state ownership

| State / service | Responsibility |
|---|---|
| WorkshopExperienceService | first-contact and Experience selection/composition |
| RuntimeAssembly | assembled composition result |
| PageFSM | page-level presentation lifecycle |
| PageStateContext | Living GUI state/data |
| FSM_API | deterministic FSM execution machinery |
| Blazor components | browser manifestation |

The semantic owner matters more than the exact class name.

## MicroBundles as reusable participants

WebPage has local composition participants because this repository is still a proving ground.

The extraction rule is:

~~~text
local experiment
     ↓
independent responsibility
     ↓
package/domain ownership
     ↓
MicroBundle when the composition contract fits
~~~

A package is not automatically a MicroBundle. A MicroBundle is a composition participant whose contract is owned by the appropriate domain package.

## What this architecture protects

### Composition is not presentation

FSM_COS should not know how a browser presents an assembly.

### Reusable capability is not WebPage infrastructure

If another host can consume a capability, its implementation should be considered for extraction.

### Experience identity is not routing

A Deep Dive route is a WebPage presentation mechanism, not the identity model.

### Proven behavior survives extraction

When code moves from WebPage to a package, the behavior that justified the extraction moves with tests and documentation.

## Verification loop

~~~text
change code
   ↓
build
   ↓
run tests
   ↓
observe behavior
   ↓
update theory / usage documentation
   ↓
run CI
   ↓
call the boundary complete
~~~

## Current invariant

~~~text
Experience
   ↓
RuntimeManifest
   ↓
FSM_COS
   ↓
RuntimeAssembly
   ↓
WebPage host
   ↓
FSM_API + browser manifestation
~~~

> **WebPage proves the architecture. It should not quietly become the architecture.**

## Optional WebPage-only providers

MicroBundles are provider collections. A composed MicroBundle may expose optional providers to the host that is executing it. WebPage uses one such optional capability: IDeepDiveProvider.

The WebPage acquisition pattern is:

~~~csharp
foreach (var microBundle in microBundles)
{
    var provider = microBundle.TryGetProvider<IDeepDiveProvider>();
    if (provider is not null)
        return provider.Execute(experience, microBundles);
}
~~~

IDeepDiveProvider is deliberately defined inside WebPage. FSM_COS, MicroBundleDomain, AnyApp, and other runtime hosts do not depend on it. Ontology and runtime identity remain portable; the Workshop's educational Deep Dive remains a WebPage capability.

A native host that wants the Deep Dive should hand the Experience identity to the user's default browser and open /deep-dive/{ExperienceId}. Browser-tab reuse is host/browser-specific and cannot be promised as a portable OS primitive.
