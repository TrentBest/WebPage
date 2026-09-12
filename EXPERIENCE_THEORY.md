# Experience Theory

This document records the working definition of an **Experience** as the architecture is rebuilt. It is theory and contract, not merely presentation documentation. Changes to the definition should be reflected in the executable tests as the model becomes concrete.

## Definition

An **Experience** is an environment comprised of MicroBundles.

MicroBundles are the focused units of capability, behavior, content, and sensory contribution. An Experience is the environment in which those MicroBundles coexist and are executed.

An Experience is therefore not synonymous with a Blazor page, a Unity scene, a component, or an idle/screen-saver state. Those are possible manifestations or hosts.

## Rule 1 — Environment

> **An Experience is an environment comprised of MicroBundles.**

The composition is identified by the MicroBundles that participate in it. MicroBundles remain independently addressable; the Experience provides the environment and composition boundary.

## Rule 2 — Sensory definition and ontology

> **An Experience is defined by the sensory systems it provides, and an Experience is quantified by its sense count.**

The model is intentionally open to sensory systems beyond today's screen/audio stack. A future Experience may provide visual, auditory, touch, smell, taste, or other sensory systems, including forms of touch beyond today's haptics.

`SenseCount` is the number of distinct sensory systems provided by the Experience. Sensory systems are represented by integer identifiers at runtime.

The Experience also respects the nine-layer ontology. Its MicroBundles carry ontological coordinates, and the ontological values at each level are semantically active rather than decorative metadata. A MicroBundle automatically participates in rules, composition, and behavior associated with every ontological level represented by its `OntologySignature`.

The nine ordered layers are:

1. Paradigm
2. Domain
3. Kingdom
4. Phylum
5. Class
6. Order
7. Family
8. Genus
9. Species

Forge/development tooling may use human-readable names, but published runtime composition should converge on integer ontology tokens and integer identifiers.

## Rule 3 — Running belongs to the Hub

> **An Experience exposes the process group(s) it uses; the Hub owns the actual stepping.**

An Experience declares the scheduler process-group identifiers required to run it. It does **not** own the global scheduler heartbeat or independently step those groups.

The Hub becomes responsible for:

- registering and indexing Experience process groups;
- allocating/caching runtime scheduling structures;
- deciding which groups are active;
- stepping active groups in the correct execution order;
- coordinating lifecycle transitions across MicroBundles and Experiences; and
- releasing or invalidating runtime structures when an Experience leaves the active set.

This separation is deliberate. Because Experiences expose their process groups instead of directly driving them, the Hub can organize the runtime by index and cache execution structures rather than repeatedly discovering what needs to run.

Conceptually:

```text
Experience
   │
   ├── MicroBundle identities
   ├── Sensory systems
   ├── Ontology
   └── Process group identities
             │
             ▼
        Singularity Hub
             │
             ├── index/cache
             ├── scheduling
             └── stepping
```

The Experience describes **what participates**. The Hub decides **what runs now and when it steps**.

## Core Experiences

A **core Experience** is an Experience whose implementation/test project is built directly into this solution. It is therefore part of the solution's compiled architectural surface rather than an externally configured development environment.

The initial core Experience test projects are:

- `LivingGuiExperience.Tests` — the Living GUI/Flex experience;
- `PongExperience.Tests` — the Pong experience.

This distinction is architectural, not a quality ranking. External Experiences can still be valid; they simply arrive through the configured MicroBundle/Experience boundary instead of being solution-native projects.

## Runtime consequence

The distinction between Experience and Hub is important for the runtime cache/index design:

```text
Experience manifest
       │
       ├── ontology coordinates
       ├── sensory-system IDs
       ├── MicroBundle IDs
       └── process-group IDs
                    │
                    ▼
              Hub runtime index
                    │
             cached execution set
                    │
                    ▼
                 stepping
```

This is the intended direction for escaping page-specific orchestration. `PageFSM`, Blazor components, and other hosts should eventually become consumers/manifestations of this model rather than being the definition of an Experience.

## Version proof convention

Repository progress continues to use the incremental version heartbeat in `SingularityHub.Tests/IncrementalVersionTests.cs`.

Version comparisons should use an explicit `Is(expected, actual)` assertion helper rather than an assertion whose intent is hidden in a generic `true` heartbeat. The current heartbeat for this Experience-rule pass is **0.0.16**.

The architecture test coordinate added for this rule pass is **0.00.017**.
