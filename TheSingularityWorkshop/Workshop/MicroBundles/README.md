# Workshop MicroBundles

> **A MicroBundle is a composition participant, not a WebPage component with a better name.**

The canonical runtime MicroBundle contract is owned by [TheSingularityWorkshop.MicroBundleDomain](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleDomain).

FSM_COS consumes that contract. WebPage demonstrates it.

## Current WebPage participants

The current proving ground contains composition participants for:

- Moniker;
- Living GUI Experience;
- Protocol;
- Grammar;
- AI Exchange.

These local participants exist because the browser is still being used to prove the composition model. They should not be mistaken for the final location of every reusable capability.

## The relationship

~~~text
MicroBundleDomain
       │
       │ defines contract
       ▼
   MicroBundle
       │
       ▼
    FSM_COS
       │
       ▼
 RuntimeAssembly
       │
       ▼
   WebPage host
~~~

The dependency direction is deliberate.

A MicroBundle should not depend on FSM_COS merely because FSM_COS currently assembles it.

## What a MicroBundle contributes

A MicroBundle can contribute:

- identity and version;
- dependency requests;
- configuration;
- installation/loading behavior;
- arbitration behavior;
- optional providers or capabilities where the owning host contract is appropriate.

It should not become responsible for:

- browser navigation;
- WebPage routing;
- browser-only presentation;
- application-wide scheduling;
- unrelated persistence;
- another package's domain semantics.

## Dependency declaration

~~~text
Experience
    │
    ▼
root MicroBundle
    │
    ├── dependency A
    │      └── dependency B
    └── dependency C
~~~

The Experience names roots.

MicroBundles declare dependencies.

FSM_COS closes the reachable graph.

The host does not manually reconstruct that graph.

## Load and arbitration

Installation is not execution.

~~~text
resolve
   ↓
load / install
   ↓
arbitrate
   ↓
converge
   ↓
RuntimeAssembly
~~~

Load establishes the capability in the composition. Arbitration allows independently authored participants to reconcile the composition. The host begins execution and manifestation after the assembly boundary.

## Optional WebPage provider

WebPage defines IDeepDiveProvider for its browser-only educational surface.

A MicroBundle may expose that provider through the WebPage provider-source adapter.

Shared runtime packages do not depend on WebPage.

## How to decide whether to extract a WebPage feature

| Question | If yes |
|---|---|
| Can another application use it? | move toward a package or domain |
| Does it have an independent semantic contract? | give that contract an owner |
| Does it need composition with other capabilities? | consider a MicroBundle |
| Is it only browser presentation? | keep it in WebPage |
| Does it need page routing or browser APIs? | keep the host adapter here |

The goal is not to eliminate code from WebPage for its own sake.

The goal is to put each responsibility where it can remain understandable and reusable.

*The bundle is small. The idea is not.*
