# Workshop Runtime Architecture

> This document describes the runtime boundary WebPage is proving.

## Ownership

~~~text
FSM_API
   └── deterministic state-machine execution

MicroBundleDomain
   └── MicroBundle contract and meaning

FSM_COS
   └── dependency closure / loading / arbitration / convergence / RuntimeAssembly

Experience
   └── describes the environment being requested

WebPage
   ├── loads the host manifest
   ├── selects startup and primary Experience entries
   ├── supplies the FSM_COS catalog
   ├── consumes RuntimeAssembly
   └── manifests the result in the browser
~~~

## Current startup contract

The host manifest is loaded before the running Experience is presented.

~~~text
webpage-host.manifest.json
        |
        +--> startup[]
        |      |
        |      +--> Workshop Moniker
        |
        +--> running[]
               |
               +--> Living GUI
                      |
                      +--> Moniker dependency
~~~

The manifest separates presentation startup from the primary running Experience.

The current primary root is MicroBundle 2102.

## FSM_COS composition

WebPage requests only the primary root:

~~~csharp
RuntimeAssembly = _compositionSystem.Execute(
    new RuntimeManifest(
        RuntimeId: 1,
        Bundles: PrimaryManifestExperience.MicroBundleIds
            .Select(BundleRequest.Unconfigured)
            .ToArray()));
~~~

The primary MicroBundle declares its own dependency:

~~~text
2102 LivingGuiExperienceMicroBundle
        |
        +--> 2110 MonikerMicroBundle
~~~

FSM_COS closes that graph.

WebPage does not manually recreate dependency traversal and does not request the Moniker as an unrelated second root.

## RuntimeAssembly versus execution

A RuntimeAssembly proves composition:

- requested roots were resolved;
- dependency closure succeeded;
- reachable capabilities were loaded;
- arbitration converged.

It does not mean the browser has finished executing the Experience.

After assembly:

~~~text
RuntimeAssembly
      ↓
primary Experience activation
      ↓
FSM_API process groups / lifecycle
      ↓
browser manifestation
~~~

## Hub ownership

The WebPage Hub remains a host orchestration boundary.

It is not fabricated as another FSM_COS Experience merely to create a navigation destination.

The running Experience is the manifest's primary Experience.

The Hub may provide orchestration, arbitration, discovery, and presentation infrastructure around that Experience without changing the Experience identity.

## Package-first implementation rule

WebPage may contain proving-ground MicroBundles and host adapters.

When a capability is reusable:

~~~text
WebPage experiment
      ↓
identify canonical owner
      ↓
extract/package
      ↓
test package
      ↓
WebPage consumes package
      ↓
browser proves integration
~~~

FSM_API and FSM_COS are consumed as NuGet packages. Reusable Workshop functionality should follow the same direction.

## Verification loop

~~~text
change
  ↓
build
  ↓
tests
  ↓
observe
  ↓
documentation
  ↓
CI
  ↓
boundary complete
~~~

The proof must agree with the documentation.

## Deep Dive boundary

A Deep Dive is a WebPage educational capability.

It may inspect a composed Experience and explain it, but shared runtime packages must not depend upward on WebPage's Deep Dive provider.

> WebPage proves the architecture. It should not quietly become the architecture.