# The Singularity Workshop — WebPage

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![FSM_API](https://img.shields.io/badge/FSM_API-1.0.13-00A98F?style=flat-square)](https://github.com/TrentBest/FSM_API)
[![Tests](https://img.shields.io/badge/tests-GitHub%20Actions-f39c12?style=flat-square&logo=githubactions&logoColor=white)](https://github.com/TrentBest/WebPage/actions)

> **The page opens the door. The Experience gives you somewhere to stand. The machinery lets you look underneath it.**

## What this repository is

WebPage is the browser proving ground for The Singularity Workshop.

It exists to build a real host around reusable Workshop machinery, make that machinery observable and interactive, and document how another developer can use the underlying packages without copying the WebPage.

This repository is therefore both a working browser application and scaffolding for operational documentation.

> **Show the behavior. Preserve the semantics. Explain the machinery.**

## The architectural decision

Useful functionality originally accumulated inside this application because WebPage was the fastest place to prove it. That experimental history is valuable, but it is not the desired permanent boundary.

When a capability proves reusable, we move it into the appropriately owned NuGet package or domain repository. WebPage then becomes the consumer, adapter, and proving ground.

~~~text
experiment in WebPage
        ↓
identify reusable responsibility
        ↓
extract to owning package/domain
        ↓
test and document the package
        ↓
WebPage consumes the package
        ↓
prove it in a real browser
~~~

Some reusable packages may eventually become MicroBundles when their semantics fit the composition model. A package is not automatically a MicroBundle merely because WebPage references it.

## FSM_COS is the composition boundary

WebPage uses [FSM_COS](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS) rather than implementing a second composition system.

~~~text
Experience
    │
    │ MicroBundle IDs
    ▼
RuntimeManifest
    │
    ▼
FSM_COS
    │
    ├── resolve roots
    ├── close dependencies
    ├── carry configuration
    ├── install / load
    ├── arbitrate
    └── require convergence
    │
    ▼
RuntimeAssembly
    │
    ▼
WebPage host
    │
    └── presentation / interaction
~~~

> **FSM_COS assembles. WebPage manifests and presents.**

For the concrete integration, read [FSM_COS_USAGE.md](FSM_COS_USAGE.md). For the kernel's own theory and API contract, read the [FSM_COS repository](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS).

## Current first contact

~~~text
LABEL 1
   ↓
LABEL 2
   ↓
ENTER THE WORKSHOP + advisory
   ↓
explicit visitor entry
   ↓
FSM_COS composes LIVING GUI
   ↓
Moniker dependency resolves as part of composition
   ↓
living GUI grows / reproduces
   ↓
population threshold
   ↓
gravity
   ↓
Moniker presentation
   ↓
Workshop navigation
~~~

This is current WebPage behavior, not a universal startup contract for other hosts.

The Living GUI is useful because it makes composition observable: the visitor sees behavior that did not exist until the Experience was explicitly composed.

## Experiences and MicroBundles

An Experience describes an environment to be composed. Its MicroBundle IDs identify the focused capabilities required for that environment.

~~~text
LivingGuiExperience
        │
        └── LivingGuiExperienceMicroBundle
                    │
                    └── Moniker dependency
~~~

The Experience does not manually walk its dependency graph. The MicroBundle declares its dependency and FSM_COS resolves the closure.

See [EXPERIENCE_THEORY.md](EXPERIENCE_THEORY.md), [WORKSHOP_RUNTIME_ARCHITECTURE.md](WORKSHOP_RUNTIME_ARCHITECTURE.md), and [MicroBundleDomain](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleDomain).

## Deep Dives

A Deep Dive is the educational surface that follows a working proof:

~~~text
WITNESS → WONDER → DEEP DIVE → UNDERSTAND → CREATE → RUN → PUBLISH
~~~

The purpose is not to teach a developer to copy a WebPage implementation. It is to show the Experience, MicroBundle, dependency, ontology, and FSM_COS boundaries so the developer can use the machinery correctly.

WebPage-only educational providers remain on the host side. Shared runtime packages do not depend upward on WebPage.

See [DEEP_DIVE_PROVIDER_ARCHITECTURE.md](DEEP_DIVE_PROVIDER_ARCHITECTURE.md).

## Identity

The Workshop keeps these concepts distinct:

| Identity | Answers |
|---|---|
| Experience ID | What Experience is being run? |
| MicroBundle ID | What capability is being composed? |
| Ontology Signature | What is the thing structurally? |

WebPage can prove and display these distinctions. The canonical contracts remain in their owning runtime/domain layers.

## Rendering

> **REMOVE THE CUBES. RENDER WHAT REMAINS.**

Rendering is a separate architectural concern. WebPage is a place to observe renderer work, not the owner of the renderer's general runtime contract.

## Documentation philosophy

The strongest Workshop repositories do two jobs:

- **theory** explains why a boundary exists and which decisions should remain invariant;
- **usage documentation** shows a developer how to cross that boundary correctly.

Start with [DOCUMENTATION_INDEX.md](DOCUMENTATION_INDEX.md).

Then use:

- [FSM_COS_USAGE.md](FSM_COS_USAGE.md) — how this host actually uses FSM_COS;
- [WORKSHOP_RUNTIME_ARCHITECTURE.md](WORKSHOP_RUNTIME_ARCHITECTURE.md) — current runtime ownership;
- [EXPERIENCE_THEORY.md](EXPERIENCE_THEORY.md) — current Experience decisions;
- [MICROBUNDLE_ARBITRATION_MAP.md](MICROBUNDLE_ARBITRATION_MAP.md) — composition flow;
- [DEEP_DIVE_PROVIDER_ARCHITECTURE.md](DEEP_DIVE_PROVIDER_ARCHITECTURE.md) — host-only educational capability;
- [CURRENT_VERTICAL_SLICE.md](CURRENT_VERTICAL_SLICE.md) — implementation state and proof work;
- [DOCUMENTATION_STANDARD.md](DOCUMENTATION_STANDARD.md) — documentation rules.

### Current / Direction / Future

Every document should make this distinction visible.

**Current** is code and behavior that exists.

**Direction** is architecture we are deliberately implementing.

**Future** is not part of the current WebPage contract.

Side conversations, abandoned platforms, speculative host worlds, and historical implementation ideas belong in issue history or the repository that owns them—not in the operational WebPage architecture.

## Development

~~~bash
git clone https://github.com/TrentBest/WebPage.git
cd WebPage
dotnet restore
dotnet run
~~~

Active engineering occurs on development. master is the stable promotion target.

Before calling a change complete:

1. preserve or explicitly relocate existing functionality;
2. build it;
3. run relevant tests;
4. inspect behavior;
5. document the architectural decision;
6. verify GitHub Actions.

Do not publish packages or releases without explicit approval.

## Quality bar

- zero warnings and zero avoidable errors;
- explicit lifecycle and state ownership;
- clean dependency direction;
- public contracts documented;
- tests prove architectural intent;
- visual behavior has visual evidence;
- reusable functionality moves to the correct package when justified;
- WebPage does not become a dumping ground for package responsibilities;
- documentation explains both how and why;
- current behavior is not confused with future architecture.

> **Can a visitor experience it, can a developer use it, and can the architecture tell us who owns it?**

## Platform integration

WebPage is browser-first. The Workshop's separate [FSM_UnityIntegrationAdvanced](https://github.com/TrentBest/FSM_UnityIntegrationAdvanced) package is the appropriate pointer for developers who specifically need that integration. It is not part of this WebPage runtime architecture.

---

*This way leads to the Singularity.*

*Built by The Singularity Workshop.*
