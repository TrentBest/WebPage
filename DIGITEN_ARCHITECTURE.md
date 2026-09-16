# Digiten Architecture

This document defines the Digiten as a first-class architectural primitive of Singularity World.

A Digiten is not a sprite, an NPC script, an FSM, or a neural network. A Digiten is a persistent inhabitant of the universe. Its visible body is a manifestation of persistent state; its goals determine what it is trying to accomplish; its policy determines how it tends to choose among available possibilities; and world capabilities determine what it is actually permitted and able to do.

The purpose of this document is to capture the rules strongly enough that another developer can implement Digitens without having to rediscover the architecture from individual code changes.

---

## 1. The Core Model

The Digiten is the smallest persistent autonomous inhabitant of Singularity World.

```text
                         DIGITEN
                            │
          ┌─────────────────┼─────────────────┐
          │                 │                 │
       IDENTITY           GOAL              ROLE
          │                 │                 │
          └─────────────────┼─────────────────┘
                            │
                     persistent state
                            │
             ┌──────────────┼──────────────┐
             │              │              │
          WORLD STATE    POLICY ID      WEIGHTS
             │              │              │
             └──────────────┼──────────────┘
                            │
                       POLICY / NN
                            │
                       next intent
                            │
                     WORLD CAPABILITY
                            │
                         FSM_API
                            │
                         WORLD
```

The central rule is:

> **The Digiten chooses what it wants or attempts to do; the world defines what can actually be done and how it is done.**

This prevents the Digiten implementation from becoming an encyclopedia of every machine, building, domain, transportation system, and activity in the universe.

---

## 2. The Digiten Is Persistent

A Digiten exists whether or not a human is currently observing it.

At minimum, a persistent Digiten eventually carries:

- stable identity;
- world/domain identity;
- spatial position;
- current Experience or environment;
- role/assignment;
- goal or mission;
- current behavioral classification;
- navigation state;
- interaction state;
- policy/NN identity;
- individualized policy weights;
- relevant inventory, relationships, permissions, and history.

The rendered character is not the authoritative representation. Rendering is a view over this state.

```text
persistent Digiten state
          │
          ├── no observer ──> compact simulation
          │
          └── observer ─────> richer manifestation
```

Human observation changes the required manifestation detail, not the existence of the actor.

---

## 3. The Digiten Has Goals

A Digiten possesses intent in the form of a goal or mission.

Example:

```text
Goal:
    Meet the systemic need for a factory worker.
```

This does not mean the Digiten receives a scripted animation called `BecomeFactoryWorker`.

Instead:

```text
systemic need
      │
      ▼
role opportunity
      │
      ▼
Digiten selects FactoryWorker
      │
      ▼
find assigned factory
      │
      ▼
world-scale navigation
      │
      ▼
factory entry
      │
      ▼
factory-local navigation
      │
      ▼
provider discovery
      │
      ▼
provider-defined work
```

The Digiten remains the same persistent actor throughout the process.

---

## 4. Hierarchical Navigation

A Digiten does not solve the entire universe as one pathfinding problem.

Navigation is hierarchical:

### 4.1 Universe navigation

The universe provides the means to reach the assigned destination. This can include walking, roads, transit, elevators, ships, portals, domain transitions, or other world infrastructure.

### 4.2 Domain navigation

Once inside a governed environment, the domain supplies its own navigation contracts.

### 4.3 Local navigation

A factory, ship, station, base, or other structure can own a local navigation system. The Digiten asks that system where to go rather than needing a global map of every internal space.

For the factory-worker example:

```text
DIGITEN
  │
  │ goal: factory worker
  ▼
UNIVERSE WAYFINDER
  │
  ▼
FACTORY 173
  │
  ▼
FACTORY NAVIGATION
  │
  ▼
ASSEMBLY LINE 42
  │
  ▼
WORK STATION 7
```

This is both more scalable and more faithful to the concept of governed environments.

---

## 5. Providers Define Specialized Behavior

A provider owns the behavior of the thing it provides.

A factory worker does not need to contain the implementation of every factory operation. The factory exposes a provider describing the work available at that location.

For example:

```text
FactoryWorkerProvider
    ├── station capabilities
    ├── required tools
    ├── safety requirements
    ├── available operations
    └── behavior contract
```

The Digiten discovers the provider and requests an available capability.

The provider then supplies the actual behavioral contract, with FSM_API remaining the authoritative execution substrate where stateful behavior is required.

---

## 6. Humans and Digitens Are Equals in the World

A Digiten must never possess a privileged interaction vocabulary unavailable to a human participant.

If a Digiten can:

- drive a bulldozer;
- operate a factory machine;
- pilot a craft;
- wear an item of clothing;
- enter a building;
- use a tool;
- board a vehicle;
- perform a job;

then a human participant must be able to access that same world capability when the same permissions, prerequisites, equipment, and physical circumstances are satisfied.

The decision source differs:

```text
HUMAN INPUT ───────┐
                   ├──> WORLD CONTRACT ──> FSM_API / behavior
DIGITEN INTENT ────┘
```

There is no secret `DigitenOnly` gameplay layer.

This is essential to the fantasy of the universe: when a player sees a Digiten doing something interesting, the player can enter that same system and participate.

---

## 7. The Neural Network Is a Behavioral Classification

The NN is not the Digiten.

A policy/NN describes a class of behavior. Many Digitens may use the same policy definition.

For example:

```text
FactoryWorkerPolicy v1
```

can be used by thousands of factory workers.

The policy defines the structural interpretation of its inputs and outputs:

```text
Policy
├── input positions
├── neuron positions
├── layers
├── activation semantics
├── output positions
└── connection topology
```

The policy therefore establishes the coordinate system in which a weight vector has meaning.

---

## 8. Positional Neuron Compatibility

Digitens using the same policy classification must share the same neuron positional model.

That means input position `0` means the same thing for every Digiten using that policy. Neuron position `37` means the same thing. Output position `5` means the same thing.

The topology may define different connections between those positions, but the positional contract must remain stable for a given policy version.

```text
Policy v3

Input positions      Hidden positions       Output positions
┌──────────────┐     ┌───────────────┐      ┌──────────────┐
│ 0            │────>│ 0             │─────>│ 0            │
│ 1            │────>│ 1             │─────>│ 1            │
│ 2            │────>│ 2             │─────>│ 2            │
│ ...          │     │ ...           │      │ ...          │
└──────────────┘     └───────────────┘      └──────────────┘
                         connections
                         provide behavior
```

The individual Digiten supplies the floating-point weights applied to this structure.

Therefore:

```text
Policy definition + Digiten weights = individualized policy behavior
```

Weights cannot be interpreted without the policy definition that gives their positions meaning.

---

## 9. Digiten-Owned Weights

Each Digiten may possess its own weight vector for its assigned policy.

Conceptually:

```text
Digiten A
    Policy = FactoryWorker v3
    Weights = W_A

Digiten B
    Policy = FactoryWorker v3
    Weights = W_B

Digiten C
    Policy = FactoryWorker v3
    Weights = W_C
```

The policy is shared. The learned/individualized parameters are not necessarily shared.

This allows Digitens to remain members of the same behavioral classification while developing different responses to similar conditions.

The architecture must preserve policy-version compatibility. A weight vector trained for one topology must not silently be interpreted against an incompatible topology.

---

## 10. The NN Service

The universe should eventually provide a shared **NN Service** or policy registry.

A factory registers its policy with the service if an equivalent policy is not already registered.

```text
FACTORY 001 ─────┐
FACTORY 002 ─────┤
FACTORY 003 ─────┼──> NN SERVICE
FACTORY 004 ─────┤       │
...              │       ├── FactoryWorkerPolicy
FACTORY N ───────┘       ├── MaintenancePolicy
                         ├── LogisticsPolicy
                         └── ...
```

The service provides identity, versioning, topology, input/output contracts, and registration for shared policies.

This is not merely an optimization. It establishes a reusable behavioral vocabulary across the universe.

Hundreds of factories can therefore use the same registered worker policy and process their populations in compatible batches.

A future implementation may schedule the work across CPU and GPU stages. The architecture should not require one specific execution device.

---

## 11. Population Batching

A massive population should be organized around shared behavioral classification whenever practical.

For example:

```text
FactoryWorkerPolicy
    │
    ├── Factory 001 workers
    ├── Factory 002 workers
    ├── Factory 003 workers
    └── Factory 400 workers
```

The execution system can process compatible populations as batches.

A conceptual GPU representation may look like:

```text
STATE TEXTURE
┌─────────────────────────────────────┐
│ D0 D1 D2 D3 D4 D5 D6 ...            │
└─────────────────────────────────────┘

WEIGHT STORAGE
┌─────────────────────────────────────┐
│ W0 W1 W2 W3 W4 W5 W6 ...            │
└─────────────────────────────────────┘

POLICY
┌─────────────────────────────────────┐
│ shared topology / interpretation    │
└─────────────────────────────────────┘
```

The GPU does not need to rediscover the behavioral classification for every actor if the population has already been organized by it.

---

## 12. State-Transition Buffers

Actors may change behavioral classification while a batch is being processed.

The current pass should not mutate the collection being iterated in a way that destroys deterministic batch semantics.

Instead:

```text
CURRENT BUCKET
      │
      ▼
process batch
      │
      ├── remains here
      │
      └── state changed
              │
              ▼
       transition buffer
              │
              ▼
       synchronization point
              │
              ▼
       destination bucket
```

This permits large populations to be processed in coherent stages while still allowing individual Digitens to change state.

The same concept can apply to CPU simulation, GPU compute, or a hybrid execution system.

---

## 13. Compact State and Textures

Compact state representations are useful because the universe may eventually contain enormous numbers of actors.

A byte provides 256 discrete indexes. Texture channels can therefore represent compact categorical state.

For example:

```text
R = overarching behavioral classification
G = role
B = local behavior
A = flags
```

The exact packing is implementation-specific.

The important rule is:

> **A pixel or byte is storage, not behavior.**

The value is interpreted through a versioned catalog/policy definition, and executable behavior remains owned by the appropriate system such as FSM_API.

This allows dense GPU-oriented state without creating a second hidden runtime inside image data.

---

## 14. Rendering Is an Observer-Dependent Manifestation

The simulation may maintain phenomena that are too numerous to render individually.

Smoke is a canonical example.

A smoke trail can exist as compact simulation state:

```text
source
origin
trajectory
age
density
persistence
dispersion
```

When a player is observing the relevant region, the renderer can manifest that state as detailed smoke.

When nobody is observing it, the simulation need only maintain the consequential state needed to preserve continuity.

The same principle applies to enormous battlefields:

```text
trillions of possible phenomena
          │
          ▼
compact simulation
          │
          ▼
observer relevance
          │
          ▼
rich manifestation
```

The renderer is therefore not required to simulate what it cannot display.

---

## 15. The Battlefield Principle

A raWWar battlefield may contain thousands of vehicles and vast numbers of persistent environmental effects.

The world can maintain:

- tank positions;
- vehicle condition;
- crew assignments;
- movement state;
- smoke sources;
- damage state;
- ammunition state;
- tactical goals;
- aircraft flight state;
- atmospheric effects;
- battlefield events.

A player inside a command vehicle may then see a localized manifestation of that enormous simulation.

As the battlefield becomes obscured by persistent smoke and distortion, the player's helmet HUD becomes increasingly important because the simulation itself remains authoritative even when direct visual observation becomes difficult.

This is a design target, not permission to fake world state behind the player's back.

---

## 16. The Same Principle Applies to the City

Singularity City should not be a static backdrop.

It is the initial persistent population laboratory for the universe.

The city is a massive floating city in the Pacific Ocean. The Workshop occupies a central underground facility, deliberately separating the Workshop's conceptual center from dependence on a particular surface location.

The city contains districts, infrastructure, factories, transit systems, residences, public spaces, and domain entry infrastructure.

The industrial district can contain hundreds of super factories.

Those factories can share policy definitions while retaining independent populations, world state, production state, and local navigation systems.

---

## 17. The Workshop's Purpose

The Workshop exists to showcase the technology of **TheSingularityWorkshop.FSM_API** by bringing a persistent universe to life.

It is not merely a webpage containing FSM demonstrations.

Users should eventually be able to:

1. enter Singularity City;
2. observe persistent Digitens;
3. create tools, clothing, machines, buildings, Experiences, and other content;
4. assign capabilities to their creations;
5. watch Digitens discover and interact with those creations;
6. enter those systems themselves;
7. participate using the same world contracts available to autonomous agents.

The Workshop is therefore simultaneously:

- an authoring environment;
- an architectural laboratory;
- a simulation demonstration;
- a UGC platform foundation;
- a window into Singularity World.

---

## 18. UGC and Appearance

Digitens can manifest creator-authored content.

A user may create:

- clothing styles;
- costumes;
- equipment;
- vehicles;
- tools;
- environments;
- buildings;
- services;
- experiences.

Digitens can wear, use, visit, operate, or otherwise interact with those creations according to the capabilities and rules exposed by the creator's MicroBundles and domain.

The visual simplicity of the universal Digiten body is intentional. Identity can be communicated through color, markings, equipment, clothing, accessories, and behavior rather than requiring unique high-cost character meshes.

---

## 19. FSM_API Remains the Behavioral Substrate

The NN does not replace FSM_API.

The NN selects or influences intent. Providers expose capabilities. FSM_API governs deterministic stateful execution where a behavioral lifecycle requires it.

```text
persistent goal
      │
      ▼
policy evaluation
      │
      ▼
selected intent
      │
      ▼
world capability
      │
      ▼
provider
      │
      ▼
FSM_API
      │
      ▼
world mutation
```

The exact number of FSMs per Digiten is not fixed.

A concern deserves an independent FSM when it requires independent lifecycle, transition semantics, scheduling, or concurrency. A simple property does not automatically require its own FSM.

---

## 20. Architectural Rules

These rules govern Digiten implementation:

1. **A Digiten is a persistent actor.**
2. **A Digiten is not an FSM.**
3. **A Digiten is not its NN.**
4. **Goals describe desired outcomes, not scripted animations.**
5. **Digitens discover specialized capabilities from the world.**
6. **The object/provider performing specialized work owns the implementation of that work.**
7. **Universe navigation and local navigation are separate contracts.**
8. **A factory may provide its own local navigation system.**
9. **Humans and Digitens use the same world capabilities.**
10. **A Digiten must not have a privileged action vocabulary unavailable to humans.**
11. **A policy/NN is a behavioral classification shared by compatible actors.**
12. **Neuron/input/output positions are stable within a policy version.**
13. **Connections define how positional neurons interact.**
14. **Individualized floating-point weights belong to the Digiten.**
15. **Weights are interpreted only against their compatible policy definition/version.**
16. **The NN service registers and versions reusable policy definitions.**
17. **Compatible populations should be batchable.**
18. **State changes during batch execution should flow through transition buffers.**
19. **Compact state representations may use bytes, channels, textures, or other dense storage.**
20. **Storage representations never replace authoritative executable behavior.**
21. **Rendering is a manifestation of simulation state.**
22. **Unobserved phenomena may be simulated compactly without being rendered in detail.**
23. **The world must remain coherent when an observer returns.**
24. **The Workshop is inside Singularity City, not synonymous with Singularity City.**
25. **The Workshop occupies an underground central facility so the conceptual center of the system is not tied to a surface coordinate.**
26. **Singularity City is a persistent population laboratory, not a static scene.**
27. **Tests are architectural breadcrumbs and must accompany meaningful implementation increments.**

---

## 21. Example: Factory Worker From Need to Work

The complete intended sequence is:

```text
SYSTEMIC NEED
    │
    │ "factory worker required"
    ▼
DIGITEN
    │
    │ selects role
    ▼
FACTORY WORKER ROLE
    │
    │ receives assignment
    ▼
UNIVERSE NAVIGATION
    │
    │ reaches factory
    ▼
FACTORY ENTRY
    │
    ▼
FACTORY LOCAL NAVIGATION
    │
    │ receives station destination
    ▼
WORK STATION
    │
    ▼
PROVIDER DISCOVERY
    │
    │ discovers available work
    ▼
POLICY / NN
    │
    │ evaluates local conditions using Digiten weights
    ▼
SELECTED INTENT
    │
    ▼
PROVIDER CONTRACT
    │
    ▼
FSM_API
    │
    ├── approach
    ├── prepare
    ├── perform
    ├── inspect
    └── report
    │
    ▼
FACTORY WORLD STATE
```

At any point a human participant may enter the same factory and use the same machine or station if the world permits it.

---

## 22. Example: raWWar Launch Sequence

A spacecraft preparing to depart can expose a world-level event and assignments to persistent Digitens.

Ground crew Digitens receive the assignment to prepare the craft. Flight crew Digitens receive their own role and flight assignment.

The observed sequence may naturally emerge as:

```text
flight assignment
      │
      ▼
ground crew mobilizes
      │
      ▼
crew approaches craft
      │
      ▼
storage protections removed
      │
      ▼
pre-flight checks
      │
      ▼
clearance
      │
      ▼
ground crew retreats
      │
      ▼
launch
      │
      ▼
energy / inertia transition
      │
      ▼
departure
```

If a player watches, the world can manifest each actor and environmental effect.

If nobody watches, the simulation can advance the consequential state without rendering every action.

When a player arrives later, they encounter the resulting world state rather than a newly invented scene.

---

## 23. Future Direction: A Population Engine

The eventual population engine should make the distinction between **simulation state**, **behavioral policy**, and **manifestation** explicit.

```text
             PERSISTENT WORLD
                    │
             ┌──────┴──────┐
             │             │
          DIGITENS      OBJECTS
             │             │
       ┌─────┴─────┐       │
       │           │       │
     STATE       WEIGHTS  CAPABILITIES
       │           │       │
       └─────┬─────┘       │
             ▼             ▼
          POLICY / NN   PROVIDERS
             │             │
             └──────┬──────┘
                    ▼
                 FSM_API
                    │
                    ▼
              WORLD CHANGES
                    │
                    ▼
              OBSERVER VIEW
```

This architecture is intended to scale from the first visible Digiten walking around Singularity City to enormous populations participating in industrial, social, transportation, and battlefield simulations.

The objective is not to make every actor visually complex.

The objective is to make the **world underneath the simple visuals complex enough that the simplicity becomes invisible**.
