# Singularity Universe

This document formalizes the persistent-world model behind the Workshop.

The Workshop is no longer conceived as a webpage containing a collection of demonstrations. It is a place inside a persistent universe. Experiences are environments encountered within that universe, and MicroBundles are the focused units from which those environments are composed.

## 1. Singularity World

**Singularity World** is the persistent spatial substrate.

The initial anchor is **Singularity City**, a massive floating city in the Pacific Ocean. The geographic placement is intentional: it gives the fictional infrastructure a location that does not require pretending that it occupies an existing city or overwrites a real-world place.

The real world can still be reflected inside Singularity World. A real-world place may be represented as an explicitly modeled reflection, simulation, archive, or portal. It does not silently become the real place.

```text
REAL WORLD
    │
    ├── reflected / simulated places
    │
    ▼
SINGULARITY WORLD
    │
    └── SINGULARITY CITY
          │
          ├── Workshop Complex
          ├── transit
          ├── commerce
          ├── residences
          ├── public spaces
          └── domain entry infrastructure
```

## 2. Domains

A **Domain** is a governed environment with its own ontology, rules, content, capabilities, and Experiences.

The universe is deliberately open to domains such as:

- Star Wars;
- Warhammer;
- developer-created worlds;
- educational worlds;
- simulations;
- games;
- businesses and communities.

The platform does not need to copy or impersonate the content of a domain. It provides the infrastructure by which authorized domain owners and creators can construct, host, govern, and monetize their own Experiences.

A domain boundary is therefore a technical boundary as well as a fictional one.

## 3. Persistence

Persistence means that the world is not reconstructed solely from the current browser request.

At minimum, persistent world state eventually includes:

- world and domain identity;
- spatial coordinates;
- Experience membership;
- MicroBundle identity and version;
- active FSM state;
- actor identity;
- current goal/mission;
- navigation state;
- interaction state;
- inventory and tickets where applicable;
- permissions and domain rules;
- relationships and conversations;
- temporal/event state.

The first implementation may persist only the subset required for the current vertical slice. The model must not prevent the complete state from becoming persistent later.

## 4. Digitens

A **Digiten** is a persistent agent/avatar capable of moving through Singularity World and entering Experiences and Domains.

Digitens do not need to know every object in the universe.

Their universal knowledge is intentionally small:

1. where they are;
2. where they can attempt to go;
3. how to choose a destination for a goal;
4. how to navigate the current world's wayfinding system;
5. how to interact with an object through the object's exposed contract.

A Digiten should discover specialized behavior from the environment rather than containing a giant switch statement for every machine, building, transit system, or fictional universe.

## 5. Goals and Missions

A goal is a persistent intention such as:

> Visit the Star Wars universe.

The goal is not a hard-coded animation sequence.

The Digiten decomposes the goal into actions using the capabilities exposed by the world:

```text
Goal: Visit Star Wars
       │
       ▼
Find viable route
       │
       ├── walk to transit
       ├── inspect available services
       ├── choose taxi or tram
       ├── navigate station
       ├── obtain ticket
       ├── board assigned transport
       ├── travel
       ├── disembark
       ├── wayfind across destination
       └── enter Star Wars domain
```

This is a plan, not a rigid cutscene. The agent may discover alternatives, pause, converse, shop, or otherwise perform permitted activities without invalidating the goal.

## 6. Vehicle intelligence belongs to the vehicle

A construction vehicle, taxi, tram, elevator, ship, or other machine exposes what it can do and what is required to do it.

The agent does not carry a global encyclopedia of machine behavior.

```text
Construction Vehicle
       │
       ├── capability: excavate
       │      └── action: dig
       │            └── requires grounding + boom + stick + bucket
       │
       ├── capability: load
       │      └── action: scoop
       │
       └── capability: drive
              └── action: navigate

Digitens Agent
       │
       └── discovers manifest
              │
              └── asks FSM-controlled vehicle to perform an action
```

The vehicle is therefore both an object in the world and a capability provider.

The current WebPage implementation begins this contract with `IVehicleCapabilitySource`, `VehicleCapability`, and `VehicleAction`. Construction vehicles now expose machine-specific capability manifests while retaining their lifecycle through FSM_API.

## 7. FSM ownership

Behavior is derived from actual FSMs built with **TheSingularityWorkshop.FSM_API**.

The FSM is not merely a label for a visual animation. It owns the lifecycle and transition semantics of the actor or process it governs.

The intended relationship is:

```text
                 Goal
                  │
                  ▼
             Digitens Agent
                  │
           chooses what to do
                  │
                  ▼
        discovers world capability
                  │
                  ▼
          target MicroBundle
                  │
                  ▼
          FSM_API state machine
                  │
        ┌─────────┴─────────┐
        │                   │
     movement            operation
        │                   │
        └─────────┬─────────┘
                  ▼
             world state
```

The agent chooses and requests. The controlled object owns the specialized behavior. FSM_API supplies the deterministic lifecycle machinery.

## 8. Navigation is hierarchical

Navigation occurs at multiple scales.

### Local navigation

A Digiten walks around a building, concourse, ship, or station using local spatial geometry and pathfinding.

### Wayfinding

A major structure can expose a wayfinding system that provides routes through its own topology.

### World transit

Transit systems connect otherwise distant structures: taxis, trams, elevators, ships, portals, and other domain infrastructure.

### Domain transition

A domain entrypoint changes the active Experience/domain context. The Digiten retains its identity and persistent state while acquiring the rules and capabilities of the new domain.

This hierarchy prevents the global universe from becoming one enormous pathfinding grid.

## 9. Example: the Space Elevator journey

The canonical demonstration journey is deliberately richer than a teleport:

1. A Digiten declares a goal to visit the Star Wars domain.
2. The Singularity World wayfinder identifies viable transit options.
3. The Digiten walks to a taxi or tram.
4. If using the tram, the Digiten navigates the surrounding monumental structure.
5. At ticketing, the traveler selects a seat.
6. A persistent ticket is issued and carried by the Digiten.
7. At boarding, the assigned seat is highlighted for that traveler while other Digitens independently find theirs.
8. Boarding behavior is governed by FSMs rather than a single animation timer.
9. During ascent, a first-time visitor may be shown an external view of the elevator technology, including the electromagnetic energy-recovery visualization.
10. The experience returns to the cab interior/top-down view.
11. Arrival is announced.
12. Digitens disembark according to their own navigation and goals; the world is allowed to look messy rather than forcing perfect choreography.
13. The traveler acquires the space station's wayfinding capabilities.
14. The traveler follows a loose route toward the Star Wars entrypoint while being free to shop, converse, inspect, or pause.
15. A transport ship provides the domain transition.
16. During transit, the traveler may explore the ship.
17. Arrival completes the original mission.
18. The Digiten then requests available local goals from the Star Wars domain.
19. A Clone Wars opportunity may be selected.
20. The domain presents role choices such as Jedi, Sith, trooper, or droid according to that domain's rules.
21. The Digiten is rolled into the selected role and continues as the same persistent agent within a new governed context.

Every one of these stages is an opportunity for reusable MicroBundles and FSMs. None should require the WebPage host to know the story in advance.

## 10. User and agent equivalence

A user-controlled avatar and an autonomous Digiten should operate against the same world contracts.

The user may personally make the Star Wars journey described above. An autonomous agent may make the same journey. They should encounter the same buildings, transit systems, tickets, gates, seats, shops, domain rules, and interaction contracts.

The difference is the decision source:

```text
Human input ───────┐
                   ├──> World interaction contracts ──> FSMs
Digitens decision ─┘
```

This is a foundational requirement. We do not build an artificial path for agents and a separate path for humans.

## 11. Other Digitens are real world actors

The visible world contains other persistent actors.

A user entering Singularity City should see other Digitens moving through the same space. Those actors may be:

- human-controlled users;
- autonomous Digitens;
- domain-controlled characters;
- service agents;
- workers;
- attendants;
- temporary visitors.

Their spatial presence is a manifestation of persistent state, not decorative random animation.

## 12. Workshop inside Singularity City

The Workshop becomes a place within the world rather than the world itself.

It is a massive high-technology complex under construction. Construction vehicles are therefore not merely decorative sprites: they are the first visible actors proving that world infrastructure can possess state, capabilities, FSM behavior, and spatial agency.

The Workshop contains capabilities such as:

- FSM creation and inspection;
- image tooling;
- NPC development;
- blueprint/design tooling;
- simulation and experimentation;
- future UGC authoring and publishing systems.

The Workshop can eventually become the place where creators build the very MicroBundles and Experiences that populate Singularity World.

## 13. UGC and domain economics

The platform is intended to let creators build useful things for their chosen domains and communities.

A creator may eventually publish:

- shops;
- tools;
- machines;
- Experiences;
- MicroBundles;
- domain infrastructure;
- characters and services;
- educational simulations;
- games and activities.

Monetization belongs to the appropriate ownership and domain contracts. The platform provides the infrastructure rather than assuming that every domain should be owned by the platform itself.

## 14. Architectural laws

These laws are now part of the working architecture:

1. **The world is spatial.** Objects have position, bounds, geometry, and interaction points.
2. **The world is persistent.** Actors and important state survive beyond a single render.
3. **Experiences are environments.** An Experience is comprised of MicroBundles.
4. **Objects expose capabilities.** Agents discover specialized behavior from the object being operated.
5. **Agents remain generic.** Digitens know navigation and goal selection, not every domain's implementation.
6. **FSM_API owns FSM behavior.** We do not create a second FSM implementation in WebPage.
7. **Humans and Digitens use the same world contracts.** Different decision sources must not produce different physics or interaction rules.
8. **Navigation is hierarchical.** Local paths, wayfinding, transit, and domain transitions are distinct layers.
9. **Rendering is manifestation.** GUI is allowed to be crude during development; it must not become the source of world truth.
10. **Domain content remains domain-owned.** The platform supplies construction and execution infrastructure without pretending to be the owner of every fictional universe.
11. **Construction is simulation.** Vehicles and workers should eventually perform actual work against world geometry.
12. **Tests are architectural breadcrumbs.** Each meaningful increment receives an incremental unit test before the next architectural layer is added.

## 15. Immediate implementation sequence

The next vertical slices are:

1. capability discovery from vehicles;
2. a Digitens actor whose lifecycle is governed by FSM_API;
3. vehicle-operation requests flowing through the discovered capability contract;
4. renderer-neutral path scheduling and incremental movement;
5. persistent spatial actor state;
6. first-class floor-plan/world geometry;
7. hierarchical wayfinding;
8. transit MicroBundles;
9. ticketing/boarding MicroBundles;
10. domain entry/exit contracts;
11. autonomous goal continuation inside a destination domain.

The purpose is not to implement the entire universe at once. The purpose is to establish contracts strong enough that the universe can grow without replacing its foundations.
