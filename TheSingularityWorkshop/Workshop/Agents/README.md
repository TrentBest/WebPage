# Agents

Agents are decision-makers operating persistent actors in Singularity World.

## What is a Digiten?

A **Digiten is a persistent digital inhabitant**, not an FSM and not a tiny script pretending to be a person.

A Digiten has an identity, a place in the world, goals, relationships, history, and time. It can exist when no human is watching. Human presence increases the need for visible simulation, interaction, and detail; it does not create the Digiten's existence.

The Digiten's universal knowledge should remain deliberately small:

- maintain identity and persistent state;
- understand its current goal or intent;
- choose among viable destinations and interactions;
- navigate using the world's wayfinding contracts;
- interact with MicroBundles through their public contracts;
- discover specialized capabilities from the object it is operating;
- participate in domains and Experiences without knowing their internal implementation.

A Digiten should **not** contain machine-specific knowledge such as how a backhoe digs or how a crane lifts.

## FSMs are behavior, not identity

The first Digiten implementation contains one behavioral FSM managed by `TheSingularityWorkshop.FSM_API`. That FSM is an orchestration layer, not the definition of the creature.

As the universe grows, a Digiten can legitimately have multiple concurrent FSMs for independent concerns, for example:

```text
                         DIGITEN
                            │
          ┌─────────────────┼─────────────────┐
          │                 │                 │
       EXISTENCE         LOCOMOTION         SOCIAL
          FSM                FSM               FSM
          │                 │                 │
          └──────────── GOAL / INTENT ────────┘
                            │
                      DOMAIN ROLE FSM
                            │
                       INTERACTION FSM
```

These are not a prescribed fixed count. We add an FSM when a behavioral concern needs independent lifecycle, scheduling, or transition semantics. We do **not** manufacture one FSM per trivial property.

The important distinction is:

> **A Digiten is the persistent actor. Its FSMs are the machinery by which that actor behaves.**

## Compact state representation

A behavior state can be represented compactly without making the representation itself the behavior model.

A byte gives 256 possible indexes. An image/texture channel can therefore carry one interpreted state index per channel. Multiple channels can carry independent compact indexes. The meaning of an index comes from the active FSM/domain behavior catalog rather than being permanently encoded into the pixel.

```text
texture / image channel
        │
        ▼
      0..255  ─────► behavior catalog ─────► FSM_API state/interpretation

       compact storage             meaning remains executable
```

This is particularly attractive for a large persistent population: dense world snapshots can carry compact behavioral indexes while the authoritative FSM definitions remain shared. A pixel is storage; it is not a replacement for FSM_API.

## Capability discovery

Specialized objects expose capabilities through contracts such as `IVehicleCapabilitySource`.

The pattern is:

```text
Digiten
   │
   │ discover
   ▼
Target MicroBundle
   │
   ├── capabilities
   └── actions + requirements
          │
          ▼
      Digiten FSM_API behavior
          │
          ▼
     target-owned operation FSM
```

The target remains authoritative. An agent may request `dig`, but it does not implement the bucket, boom, grounding, hydraulic, or engine behavior itself. The vehicle tells the agent what it can do and what the operation requires.

## Persistence

A Digiten's durable record should eventually include at least:

- stable actor identity;
- world/domain identity;
- spatial position and current Experience;
- active goal/intent;
- current FSM state and compact behavior indexes;
- navigation and interaction state;
- inventory, tickets, permissions and relationships where applicable;
- temporal/event state;
- lineage or provenance where the domain requires it.

This is what allows a Digiten to leave the Workshop, take a taxi, board a tram, use a space elevator, enter another domain, take a role there, and later return without becoming a newly spawned character.

## Human equivalence

A human-controlled avatar and an autonomous Digiten consume the same world contracts. There should be no privileged automation-only universe.

## Development heartbeat

Each new agent capability receives an incremental unit test before the next architectural layer is added. **Incremental Unit Test 11** establishes the first persistent Digiten actor whose intent is advanced by an FSM_API behavioral FSM.
