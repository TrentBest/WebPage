# Deep Dive Provider Architecture

## Purpose

A Deep Dive is an educational capability of the **WebPage**, not a universal runtime capability.

A MicroBundle may provide a WebPage-specific Deep Dive provider. If it does, WebPage can acquire that provider from the composed MicroBundle set.

The current catalog performs this lookup after it has selected the Experience and established whether the available bundles come from a real runtime assembly or the catalog fallback:

```csharp
foreach (var microBundle in bundles)
{
    var provider = microBundle.TryGetProvider<IDeepDiveProvider>();

    if (provider is not null)
    {
        return provider.Execute(
            experience,
            bundles,
            runtimeAssembly,
            requestedRoots);
    }
}
```

Here, `runtimeAssembly` is the actual FSM_COS result when one is available, and `requestedRoots` are the Experience's requested MicroBundle IDs. The provider receives both the request and the resolved bundle set; it must not infer that every resolved bundle was a direct root. The catalog owns resolution and fallback decisions, while the provider builds the educational projection.

## Why the provider belongs to WebPage

The ontology identity is portable.

The runtime composition is portable.

The educational presentation is not.

A host such as AnyApp, MyVR, or another WebApp can resolve:

- Experience identity;
- MicroBundle identity;
- version;
- ontology;
- dependencies;
- capabilities;
- runtime composition.

It does **not** need to ship the Workshop's browser-only educational implementation.

That creates an intentional boundary:

```text
Portable runtime
  |
  +--> ontology / identity / composition
  |
  +--> any host

WebPage
  |
  +--> IDeepDiveProvider
  +--> Deep Dive presentation
  +--> creator education
```

The user can therefore discover what an Experience is without requiring every host to reproduce the Workshop's educational machinery.

## Current entry point and data boundary

### Current

The WebPage hub now exposes **SHOW DEEP DIVE** for the Experience named by the host manifest's `deepDive.experience` field. The link is resolved against the registered Experience catalog and navigates to:

```text
/deep-dive/{ExperienceId}
```

For the current manifest, the target is Living GUI (Experience ID `3002`). The route is a browser presentation feature; FSM_COS and MicroBundleDomain do not know that this route exists.

The `WorkshopExperienceService` retains the `RuntimeAssembly` returned by FSM_COS. When the visitor opens the Deep Dive from the running hub, `WorkshopDeepDiveCatalog` now passes that assembly into the educational model and provider. The page distinguishes manifest-requested roots from the resolved MicroBundles, and shows the assembly's runtime ID and arbitration-round count.

A direct link opened without an initialized startup assembly uses a catalog-based fallback. The page labels that fallback explicitly; it does **not** claim that catalog declarations prove the complete dependency-closed runtime graph. The catalog also checks that the supplied roots belong to the selected Experience contract and are present in the assembly before associating the two. A bundle ID merely existing in an assembly is not enough: a root from a different Experience must not make that assembly look like the requested Experience.

### Direction

The current implementation is a first truthful inspection surface, not yet a universal runtime debugger. Next improvements should keep these concepts separate:

- **Requested roots** — what the host manifest asked FSM_COS to compose.
- **Resolved composition** — what the returned RuntimeAssembly actually contains.
- **Declared dependencies** — what each MicroBundle contract states.
- **Browser manifestation** — what WebPage renders from the runtime.

The page consumes the assembly produced by FSM_COS; it does not reimplement dependency traversal. A later improvement can make the requested roots and resolved bundle details more interactive without changing that ownership boundary.

### Future

A Deep Dive should be discoverable from any manifest-declared Experience with an educational provider, not only from a hard-coded page or route. A missing provider should produce an honest general-purpose view or a clear unavailable state; it should not prevent the Experience from running.

## Provider collection

MicroBundles should be understood as collections of providers/capabilities rather than monolithic application objects.

A future canonical MicroBundle contract should support:

```text
MicroBundle
  |
  +--> state provider
  +--> rendering provider
  +--> interaction provider
  +--> persistence provider
  +--> metrics provider
  +--> DeepDive provider (WebPage only)
  +--> ...
```

A provider is optional. The absence of a provider is meaningful and must not be treated as an error.

## Host behavior

If a host wants educational material, it should ask the WebPage for it.

For a native host, the intended handoff is:

```text
Native Experience
   |
   | request Deep Dive
   v
default web browser
   |
   +--> existing Workshop tab, if the host can identify one
   |
   +--> otherwise open a new Workshop tab
   |
   v
WebPage /deep-dive/{ExperienceId}
```

Native applications should not attempt to embed the WebPage's Deep Dive renderer.

The browser-tab reuse requirement is a host integration goal. Desktop applications can reliably launch the user's default browser, but generic cross-browser discovery/activation of an already-open tab is not a portable OS-level guarantee. Implementations should use browser-specific automation only where explicitly supported and permitted.

## Publication and immutability

A published Experience should expose its stable identity and Deep Dive route without recompiling WebPage.

The Deep Dive provider therefore describes the capability; the WebPage resolves the published Experience and renders its educational surface.

## Privacy

Deep Dive is not an excuse to disclose user profile data. Educational content should describe the Experience, its composition, ontology, and creation path. User identity and private profile data remain behind the identity/access boundary.

## Non-goals

This architecture does not make:

- FSM_COS depend on WebPage;
- MicroBundleDomain depend on WebPage;
- AnyApp depend on Blazor;
- Deep Dive metadata a replacement for ontology;
- user profile data public.

*The runtime tells you what exists. The Workshop teaches you how to use it.*
