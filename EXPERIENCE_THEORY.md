# Experience Theory

This document records the working definition of an **Experience** as the architecture is rebuilt. It is theory and contract, not merely presentation documentation. Changes to the definition should be reflected in executable tests as the model becomes concrete.

## Definition

An **Experience** is an environment comprised of MicroBundles.

MicroBundles are the focused units of capability, behavior, content, and sensory contribution. An Experience is the environment in which those MicroBundles coexist and are executed.

An Experience is therefore not synonymous with a Blazor page, a Unity scene, a component, or an idle/screen-saver state. Those are possible manifestations or hosts.

## Rule 1 — Environment

> **An Experience is an environment comprised of MicroBundles.**

The composition is identified by the MicroBundles that participate in it. MicroBundles remain independently addressable; the Experience provides the environment and composition boundary.

## Rule 2 — Sensory definition and ontology

> **An Experience is defined by the sensory systems it provides, and an Experience is quantified by its sense count.**

The model is intentionally open to sensory systems beyond today's screen/audio stack. A future Experience may provide visual, auditory, touch, smell, taste, or other sensory systems.

`SenseCount` is the number of distinct sensory systems provided by the Experience. Sensory systems are represented by integer identifiers at runtime.

The Experience also respects the nine-layer ontology. Its MicroBundles carry ontological coordinates, and the ontological values at each level are semantically active rather than decorative metadata. Forge/development tooling may use human-readable names, but published runtime composition should converge on integer ontology tokens and integer identifiers.

## Rule 3 — Running belongs to the Hub

> **An Experience exposes the process group(s) it uses; the Hub owns the actual stepping.**

An Experience declares the scheduler process-group identifiers required to run it. It does not own the global scheduler heartbeat or independently step those groups.

The Hub becomes responsible for registering and indexing groups, deciding which groups are active, stepping them in execution order, coordinating lifecycle transitions, and releasing or invalidating runtime structures when an Experience leaves the active set.

## Rule 4 — Composition is an explicit manifest

> **When a manifest is supplied, the execution host follows the manifest instead of reconstructing a default trail.**

An `ExperienceManifest` is an ordered composition plan. It identifies Experiences for startup, transition, and running phases. Ordering is significant. Required capabilities and ontology coordinates belong to the manifest rather than to page-specific orchestration.

## Rule 5 — The Moniker is an Experience

> **The Moniker is an Experience whose presentation is extensible through MicroBundles.**

The Moniker is not a special case in the runtime. Its current presentation is one possible manifestation of a capability-bearing Experience.

## Rule 6 — The Hub can become the authoring surface

> **The composition of the current page is itself an Experience and can expose its own manifest.**

A composition surface can eventually inspect, edit, save, and publish the Experience composition instead of following a hard-coded default trail.

## Rule 7 — The Experience exists inside a persistent world

> **An Experience is encountered somewhere in Singularity World; it is not the universe itself.**

The current architecture now distinguishes three levels:

```text
Singularity World
      │
      ├── persistent spatial actors
      ├── persistent infrastructure
      ├── Domains
      │     └── Experiences
      │           └── MicroBundles
      │
      └── transit / wayfinding / portals
```

The Workshop is therefore a place inside Singularity City, which is itself a place inside Singularity World. A user can leave the Workshop and still exist as the same persistent actor.

## Rule 8 — Agents are consumers of world contracts

> **A Digiten chooses what to do; the world tells it how the available thing can be done.**

Digitens have intentionally small universal knowledge. They understand goals, destinations, navigation, and interaction. They do not contain machine-specific implementations.

A machine such as a backhoe exposes a capability manifest. A Digiten discovers that manifest, selects an action appropriate to its goal, and requests the operation. The machine remains authoritative for the specialized behavior.

This makes the following possible without adding special-case code to the agent:

```text
Goal
 │
 ▼
Digiten
 │
 ├── navigate to machine
 ├── discover capabilities
 ├── select permitted action
 └── request action
          │
          ▼
Construction Vehicle
 │
 ├── capability manifest
 ├── subsystem requirements
 └── FSM_API-controlled behavior
```

## Rule 9 — Humans and Digitens use the same world

> **Automation is another decision source, not another universe.**

A human-controlled avatar and an autonomous Digiten use the same spatial objects, wayfinding systems, transit, tickets, gates, seats, shops, domain entrypoints, and interaction contracts.

This is essential to persistence. If an autonomous agent can travel from Singularity City to a distant domain, a user should be able to make the same journey manually.

## Rule 10 — FSM_API remains the behavior substrate

> **WebPage composes FSM_API; WebPage does not replace it.**

Actor lifecycle, navigation lifecycle, machine operations, transit stages, ticketing, boarding, arrival, and domain transitions should become deterministic FSMs using `TheSingularityWorkshop.FSM_API`.

The current MicroBundle lifecycle already delegates scheduling to FSM_API. The next agent increment extends the same principle from bundle lifecycle into actual actor behavior.

## Core Experiences

A **core Experience** is an Experience whose implementation/test project is built directly into this solution. It is part of the solution's compiled architectural surface rather than an externally configured development environment.

External Experiences can still be valid; they simply arrive through the configured MicroBundle/Experience boundary instead of being solution-native projects.

## Runtime consequence

The runtime direction is now:

```text
Persistent World
       │
       ├── actor index
       ├── spatial index
       ├── domain index
       ├── Experience index
       └── MicroBundle index
                    │
                    ▼
             Singularity Hub
                    │
             execution scheduling
                    │
                    ▼
                 FSM_API
```

This is the intended direction for escaping page-specific orchestration. `PageFSM`, Blazor components, and other hosts should become consumers/manifestations of the model rather than the definition of an Experience.

## Canonical journey

The canonical persistent-world demonstration is the **Space Elevator → Space Station → Domain Entry** journey documented in `SINGULARITY_UNIVERSE.md`. It is intentionally not a cutscene. The traveler has a goal and a route, but can deviate through permitted interactions while remaining on mission.

## Version proof convention

Repository progress continues to use incremental unit tests as architectural breadcrumbs. Each meaningful implementation increment gets a named test before the next architectural layer is added.


## Rule 11 — Installation precedes arbitration

A MicroBundle is loaded once, then arbitrated repeatedly until stable or the ten-round limit is reached.

`LoadBundle(IArbitrator)` is the installation/configuration boundary. `Arbitrate(IArbitrator, roundIndex)` is the logical convergence boundary.

No bundle fabricates a missing dependency. If that dependency arrives later, its presence becomes visible to later arbitration.

## Rule 12 — Installation order is policy

The Hub preserves installation completion order for its default arbitration pass. A future Experience manifest may replace that ordering policy without changing the bundle contract.


## Rule 13 — Human agency is a runtime boundary

> **Automation reduces cognitive friction; it does not remove human responsibility.**

The Workshop is intended to be self-describing and self-teaching, not self-governing. Procedural composition, recommendation, AI, and arbitration may produce inspectable proposals. Consequential world changes require an explicit human-controlled commit boundary.

The persistent-world model distinguishes the author-controlled Workshop from user-extensible Singularity City. City users can create buildings, Experiences, and content within the permissions of the world.

See `WORLD_COMPOSITION_AND_HUMAN_AGENCY.md` for the composition, authorship, applicability, provenance, and human-approval contract.


## Rule 14 — Perception precedes explanation

> **The opening should demonstrate the Workshop before asking the visitor to understand it in prose.**

The public landing page is a perception boundary. Its job is to reduce cognitive friction by giving the visitor evidence of the system's character through a short authored sequence of visual, spatial, and eventually auditory manifestations.

The sequence is:

`ARRIVAL → IDENTITY → TENSION → SHOW → INVITATION → WORKSHOP`.

The semantic structure belongs to recursive GUI Builders. FSM_API should own meaningful presentation lifecycle as the sequence becomes stateful. CSS and browser audio APIs remain manifestation mechanisms rather than architectural owners.

"Shock and awe" means presentation intensity, not manipulation. The system must not hide consequential behavior, invent capabilities, or make consequential decisions for the visitor.

See `WORKSHOP_OPENING_EXPERIENCE.md`.
