# Deep Dive Provider Architecture

## Purpose

A Deep Dive is an educational capability of the **WebPage**, not a universal runtime capability.

A MicroBundle may provide a WebPage-specific Deep Dive provider. If it does, WebPage can acquire that provider from the composed MicroBundle set.

Conceptually:

```csharp
foreach (var microBundle in microBundles)
{
    var provider = microBundle.TryGetProvider<IDeepDiveProvider>();

    if (provider is not null)
        return provider.Execute(experience, microBundles);
}
```

The exact provider plumbing may evolve, but the architectural rule does not.

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
