# WebPage — FSM_COS Usage

> **This document explains how WebPage uses FSM_COS. It does not redefine FSM_COS.**

WebPage is the browser proving ground. Its job is to use the reusable composition machinery in a real application, make the result observable, and turn successful experiments into operational knowledge for developers.

For the kernel's own theory and API contract, read [The Singularity Workshop.FSM_COS](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS).

## The boundary

The intended direction is:

~~~text
WebPage experiment
       ↓
reusable responsibility discovered
       ↓
owning package / domain
       ↓
NuGet package
       ↓
WebPage consumes it
       ↓
browser proving ground
~~~

FSM_COS is the composition boundary. WebPage should not become a second composition engine.

~~~text
Experience
    ↓
RuntimeManifest
    ↓
FSM_COS
    ↓
RuntimeAssembly
    ↓
WebPage presentation
    ↓
FSM_API behavior
~~~

## WebPage's package references

The host currently consumes:

~~~xml
<PackageReference Include="TheSingularityWorkshop.FSM_API" Version="1.0.13" />
<PackageReference Include="TheSingularityWorkshop.FSM_COS" Version="0.1.0-alpha.6" />
~~~

CI currently builds the unpublished alpha.6 candidate from a pinned FSM_COS source commit, packs it into a local NuGet feed, and tests WebPage against that package. This proves compatibility without publishing the package; the version declaration does not imply that alpha.6 is available on nuget.org.

The responsibilities are separate:

- FSM_API — deterministic state-machine behavior.
- FSM_COS — composition.
- WebPage — browser presentation and visitor interaction.

## 1. Supply a catalog

WebPage supplies FSM_COS with an IMicroBundleCatalog.

The current catalog is host infrastructure and contains the WebPage composition participants, including Moniker, Living GUI Experience, Protocol, Grammar, and AI Exchange.

Conceptually:

~~~csharp
public bool TryResolve(ulong bundleId, out CosMicroBundle? bundle) =>
    _bundles.TryGetValue(bundleId, out bundle);
~~~

FSM_COS does not care whether the catalog is backed by an in-memory dictionary, cache, generated registry, or repository-backed resolver.

## 2. Define an Experience

The Experience identifies the roots of the composition.

The current Living GUI proof is intentionally small:

~~~csharp
public ulong Id => 3002;
public string Name => "LIVING GUI";

public IReadOnlyList<ulong> MicroBundleIds { get; } =
    [LivingGuiExperienceMicroBundle.BundleId];
~~~

The Experience does not manually instantiate its dependency graph.

## 3. Declare dependencies on the MicroBundle

The Living GUI MicroBundle declares Moniker as a dependency:

~~~csharp
public IReadOnlyList<BundleRequest> Dependencies { get; } =
[
    BundleRequest.Unconfigured((ulong)MonikerMicroBundle.BundleId)
];
~~~

That means:

~~~text
LivingGuiExperience
        ↓
LivingGuiExperienceMicroBundle
        └── Moniker
~~~

WebPage does not manually load Moniker first. FSM_COS resolves the dependency closure.

## 4. Execute a RuntimeManifest

The host turns the selected Experience into a RuntimeManifest:

~~~csharp
RuntimeAssembly = _compositionSystem.Execute(
    new RuntimeManifest(
        RuntimeId: SelectedExperience.Id,
        Bundles: SelectedExperience.MicroBundleIds
            .Select(BundleRequest.Unconfigured)
            .ToArray()));
~~~

The composition boundary is therefore:

~~~text
roots
  ↓
resolve
  ↓
dependency closure
  ↓
load
  ↓
arbitrate
  ↓
converge
  ↓
RuntimeAssembly
~~~

## 5. RuntimeAssembly is the handoff

A successful RuntimeAssembly means the requested composition was assembled and reached its stable arbitration result.

It does not mean that the browser has rendered the Experience.

After handoff, WebPage performs its own presentation transition and FSM_API-driven execution.

This distinction is fundamental:

> **Composition is complete before browser execution begins.**

## 6. Extracting functionality from the old monolith

When a WebPage feature becomes reusable, ask:

1. Does it have an independent responsibility?
2. Can another host consume it without importing WebPage?
3. Does it have a stable contract?
4. Does it participate in composition?
5. Is the remaining behavior genuinely browser-specific?

Use this decision:

~~~text
independent semantic capability
          │
          ├── reusable ───────→ owning package/domain
          │                         │
          │                         └── MicroBundle if appropriate
          │
          └── browser-only ───→ WebPage host
~~~

A package is not automatically a MicroBundle. A MicroBundle is a composition participant whose contract is owned by the appropriate domain package.

The goal is not to make WebPage artificially small. The goal is to make ownership explicit.

## 7. Deep Dive provider

Deep Dive is a WebPage-only educational capability.

A composed MicroBundle may expose an optional WebPage provider. WebPage discovers it after composition:

~~~csharp
foreach (var microBundle in microBundles)
{
    var provider = microBundle.TryGetProvider<IDeepDiveProvider>();
    if (provider is not null)
        return provider.Execute(experience, microBundles);
}
~~~

The dependency direction remains:

~~~text
MicroBundle
     ↓
optional WebPage provider
     ↓
Deep Dive
~~~

FSM_COS does not depend on WebPage.

## 8. The rule for package authors

Do not make a reusable package depend on FSM_COS merely because FSM_COS is the current composition host.

Prefer:

~~~text
package owns contract
       ▲
       │
   FSM_COS consumes
~~~

not:

~~~text
package → FSM_COS
~~~

This is the architectural lesson WebPage is intended to demonstrate.

## Operational invariant

A WebPage feature is not finished merely because it renders.

A developer should be able to answer:

- what the behavior is;
- which package owns it;
- which MicroBundle composes it, if any;
- what FSM_COS does with it;
- what WebPage does after RuntimeAssembly;
- what test proves the boundary;
- where the theory explains why the boundary exists.

> **The page demonstrates the machinery. The package owns the reusable machinery.**
