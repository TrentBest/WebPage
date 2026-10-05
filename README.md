# The Singularity Workshop — WebPage

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![FSM_API](https://img.shields.io/badge/FSM_API-1.0.13-00A98F?style=flat-square)](https://github.com/TrentBest/FSM_API)
[![Tests](https://img.shields.io/badge/tests-GitHub%20Actions-f39c12?style=flat-square&logo=githubactions&logoColor=white)](https://github.com/TrentBest/WebPage/actions)
[![Repository](https://img.shields.io/badge/repository-public-238636?style=flat-square&logo=github)](https://github.com/TrentBest/WebPage)

> **The page opens the door. The Experience gives you somewhere to stand. The machinery lets you look underneath it.**

## What this repository is

WebPage is the public browser proving ground for The Singularity Workshop.

It is a real Workshop host, not a screenshot gallery and not the canonical home of every runtime contract. It exists to make the architecture observable: visitors can witness real behavior, Experiences can be composed from MicroBundles, FSM_COS can assemble the selected runtime, and Deep Dives can explain what was just witnessed.

> **Show the behavior. Preserve the semantics. Explain the machinery.**

## First contact

The current opening is deliberately experiential rather than a conventional marketing hero:

ARRIVE → LABEL 1 → LABEL 2 → ENTER THE WORKSHOP → explicit entry → FSM_COS composition → LIVING GUI → population threshold → gravity → Moniker → Workshop navigation.

This is current WebPage behavior. It is not a specification for other Workshop hosts or future Experiences.

## Runtime boundary

WebPage owns browser presentation, visitor interaction, discovery, authoring UX, and proving-ground behavior. FSM_COS owns composition, dependency closure, loading, arbitration, convergence, and RuntimeAssembly. MicroBundles own focused capabilities. Experiences describe composed environments. The renderer owns rendering computation rather than becoming a WebPage identity.

```text
Visitor
  ↓
WebPage / Blazor
  ↓
Experience
  ↓
FSM_COS
  ↓
RuntimeAssembly
  ↓
WebPage manifestation
```

Shared runtime layers must not acquire a dependency on WebPage merely because this is the most visible host.

## MicroBundles and Deep Dives

MicroBundles are focused capability units and increasingly act as collections of optional providers. WebPage currently acquires a WebPage-only Deep Dive provider from a composed MicroBundle when one is available.

Deep Dive is the Workshop's educational unit:

WITNESS → WONDER → DEEP DIVE → UNDERSTAND → CREATE → RUN → PUBLISH

> **Don't copy the trick. Use the machinery.**

See [DEEP_DIVE_PROVIDER_ARCHITECTURE.md](DEEP_DIVE_PROVIDER_ARCHITECTURE.md).

## Identity

The Workshop keeps Experience ID, MicroBundle ID, and Ontology Signature distinct. Experience identity answers what is running; MicroBundle identity answers what capability is composed; ontology answers what the thing is structurally. WebPage proves these ideas but does not own the canonical runtime contracts.

## Rendering

> **REMOVE THE CUBES. RENDER WHAT REMAINS.**

The renderer direction explores observer-relative detail, Event Horizons, representation policy, computation/workload separation, and state-driven manifestation. WebPage demonstrates the work; the renderer remains a separate architectural concern.

## Public surfaces

Explore · Rendering · Create · Education · MadMen · About Us · Consult

These are different doors into one Workshop. See [PUBLIC_WORKSHOP_TABS.md](PUBLIC_WORKSHOP_TABS.md).

## Documentation

Start with [DOCUMENTATION_INDEX.md](DOCUMENTATION_INDEX.md) and [DOCUMENTATION_STANDARD.md](DOCUMENTATION_STANDARD.md). The documentation standard requires every document to distinguish current behavior, architectural direction, and future work. Documentation is engineering memory, not marketing fiction.

Key contracts:

- [WORKSHOP_RUNTIME_ARCHITECTURE.md](WORKSHOP_RUNTIME_ARCHITECTURE.md)
- [WEBPAGE_EXPERIENCE_ARCHITECTURE.md](WEBPAGE_EXPERIENCE_ARCHITECTURE.md)
- [WORKSHOP_OPENING_EXPERIENCE.md](WORKSHOP_OPENING_EXPERIENCE.md)
- [EXPERIENCE_THEORY.md](EXPERIENCE_THEORY.md)
- [MICROBUNDLE_ARBITRATION_MAP.md](MICROBUNDLE_ARBITRATION_MAP.md)
- [CURRENT_VERTICAL_SLICE.md](CURRENT_VERTICAL_SLICE.md)

## Platform integration

WebPage is browser-first. The separate Unity integration maintained by the Workshop is [FSM_UnityIntegrationAdvanced](https://github.com/TrentBest/FSM_UnityIntegrationAdvanced). It is not part of this WebPage runtime architecture.

## Development

```bash
git clone https://github.com/TrentBest/WebPage.git
cd WebPage
dotnet restore
dotnet run
```

Active engineering occurs on development. Before calling a change complete: build it, run the relevant tests, inspect the behavior, update the documentation, and verify GitHub Actions.

## Workshop quality bar

- zero warnings and zero avoidable errors;
- explicit lifecycle and state ownership;
- clean dependency direction;
- XML documentation for public contracts;
- tests that prove architectural intent;
- visual evidence for visual behavior;
- documentation that describes what the code actually does;
- incremental, inspectable changes;
- preserve valuable behavior before deleting or simplifying it.

> **Can a visitor experience it, can an engineer explain it, and can the architecture tell us who owns it?**

## The larger idea

The Singularity Workshop is not claiming that the Singularity has arrived. We are building machinery that makes increasingly strange software possible to compose, observe, understand, and create.

*This way leads to the Singularity.*

*Built by The Singularity Workshop.*

---
# Runtime foundations

## FSM_API

FSM_API provides the deterministic state vocabulary used by the Workshop. The important boundary is explicit state ownership: intent enters a stateful runtime, behavior executes there, and presentation observes the result.

See [FSM_API](https://github.com/TrentBest/FSM_API) for the canonical package and benchmarks.

## FSM_COS

FSM_COS is the composition boundary. A manifest identifies requested capabilities; FSM_COS resolves dependencies, loads bundles, arbitrates the composition, converges it, and exposes a RuntimeAssembly.

WebPage consumes that assembly. WebPage does not redefine the composition algorithm.

See [TheSingularityWorkshop.FSM_COS](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS).

## SingularityWarehouse

SingularityWarehouse is the data/infrastructure direction behind persistent artifacts, identity, ontology, relationships, and runtime-addressable content.

WebPage may prove concepts against that ecosystem, but storage ownership belongs to the repository/service responsible for the data.

## AI and deterministic boundaries

Protocol, Grammar, and future command-oriented AI work remain part of the Workshop's direction. They are producers of deterministic inputs, not owners of runtime semantics.

```text
Probabilistic intelligence
        ↓
Grammar / Protocol
        ↓
deterministic command
        ↓
Experience / MicroBundle
        ↓
FSM_COS
```

## Renderer boundary

The Workshop renderer is a separate runtime concern. WebPage can demonstrate renderer behavior, but rendering research should not quietly become a browser-specific architecture.

## Development discipline

Work on development. Keep master stable.

Every meaningful change should answer:

1. What does the visitor experience?
2. What capability does it prove?
3. Which runtime boundary owns it?
4. Which Experience or MicroBundle composes it?
5. What test proves the intended behavior?
6. Which document explains the result?
7. Can another appropriate host eventually consume the same semantics?

Do not publish packages or releases without explicit approval.

## Documentation discipline

Documentation must be:

- current rather than historical by accident;
- explicit about what is implemented versus intended;
- free of obsolete platform claims;
- anchored to real repository contracts;
- visual when a diagram communicates more than prose;
- honest about transitional code;
- written so a new engineer can find the owner of a behavior.

See [DOCUMENTATION_STANDARD.md](DOCUMENTATION_STANDARD.md).

---

*This way leads to the Singularity.*

*Built by The Singularity Workshop.*
