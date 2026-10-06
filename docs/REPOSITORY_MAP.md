# WebPage Repository Map

WebPage is both a browser application and a teaching/proving ground for The Singularity Workshop.

The repository is intentionally split by responsibility. When code appears to duplicate a capability already owned by a Workshop package, that is a migration signal—not permission to create a second implementation.

## Read the repository in this order

1. ../README.md — why WebPage exists.
2. LEARNING_PATH.md — how to learn the technology from the repository and the running site.
3. WORKSHOP_RUNTIME_ARCHITECTURE.md — who owns runtime responsibilities.
4. FSM_COS_USAGE.md — how this host actually composes a runtime.
5. EXPERIENCE_THEORY.md — what an Experience means.
6. MICROBUNDLE_ARBITRATION_MAP.md — how capabilities are loaded and arbitrated.
7. CURRENT_VERTICAL_SLICE.md — what is currently being proven.
8. DOCUMENTATION_STANDARD.md — how this documentation stays honest.

## Source tree

| Area | Purpose | Ownership rule |
|---|---|---|
| TheSingularityWorkshop/Pages | Browser routes and page manifestations | Rendering/presentation only |
| TheSingularityWorkshop/Components | Reusable browser presentation | Observe runtime state; do not invent lifecycle |
| TheSingularityWorkshop/Layout | Browser chrome and navigation | Manifestation of host state |
| TheSingularityWorkshop/Gui | Semantic GUI builders | Describe GUI structure; renderer manifests it |
| TheSingularityWorkshop/Services | WebPage host adapters and transitional orchestration | Migrate reusable behavior outward |
| TheSingularityWorkshop/Workshop/Experiences | WebPage Experience descriptions | Identify what is being composed |
| TheSingularityWorkshop/Workshop/MicroBundles | Concrete proving-ground composition participants | Use FSM_API and package contracts |
| TheSingularityWorkshop/Workshop/Composition | WebPage FSM_COS catalog | Resolve host-available composition participants |
| TheSingularityWorkshop/Infrastructure | Browser, Hub, FSM_COS, storage and other host adapters | Never become a second runtime kernel |
| TheSingularityWorkshop/wwwroot | Manifest/configuration/static browser assets | Data and browser resources, not hidden business logic |
| SingularityHub.Abstractions | WebPage-local Hub abstraction boundary | Keep canonical contracts portable |
| SingularityHub | WebPage Hub proving implementation | Host-facing orchestration, not an FSM_COS replacement |
| Experiences/* | Experience-specific test projects | Prove concrete Experience behavior |
| SingularityHub.Tests | Cross-cutting architecture and integration tests | Executable architectural evidence |
| docs/ | Architecture, theory, usage and handoff documentation | Keep current/direction/future explicit |

## Dependency direction

The important direction is:

~~~text
WebPage
  |
  +--> FSM_API (NuGet)
  +--> FSM_COS (NuGet)
  +--> MicroBundleDomain (NuGet)
  +--> GUI packages (NuGet)
  +--> other Workshop packages as required
  |
  v
browser manifestation
~~~

The packages must not depend upward on WebPage.

For behavior, the useful mental model is:

~~~text
FSM_API
   ↓
state execution

MicroBundleDomain
   ↓
MicroBundle meaning

FSM_COS
   ↓
composition / dependency closure / arbitration

WebPage
   ↓
manifest + browser host + presentation

renderer / browser
   ↓
visible manifestation
~~~

## Package-first rule

Before adding a new local implementation, ask:

- Does FSM_API already provide the state-machine behavior?
- Does FSM_COS already provide the composition behavior?
- Does another Workshop NuGet package own this capability?
- Is the remaining code genuinely browser-specific?
- Is this actually an Experience or MicroBundle?

If the package owns it, consume the package. Do not copy the implementation into WebPage.

## Documentation ownership

A WebPage document explains how WebPage consumes a boundary.

The owning package explains the canonical theory and API of that boundary.

That keeps this repository useful without turning it into a duplicate manual for every Workshop package.

## Historical code

This repository contains old experiments. They are not automatically current architecture.

A historical subsystem remains until:

~~~text
locate behavior
    ↓
identify owner
    ↓
preserve behavior with tests
    ↓
replace WebPage implementation
    ↓
prove package/runtime path
    ↓
remove obsolete code
~~~

Do not delete code merely because it looks old. Do not preserve duplicate architecture merely because it already exists.

---

*The repository should teach the machine, not the archaeology.*