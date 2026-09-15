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

`SenseCount` is the number of **distinct** sensory systems provided by the Experience. Sensory systems are represented by integer identifiers at runtime.

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

## Rule 4 — Composition is an explicit manifest

> **When a manifest is supplied, the execution host follows the manifest instead of reconstructing a default trail.**

An `ExperienceManifest` is an ordered composition plan. It identifies Experiences for `Startup`, `Transitioning`, and `Running` phases. Ordering is significant: the developer composes the lineup, and the host executes that lineup rather than silently substituting its own preferred sequence.

The manifest may also declare required capabilities. The canonical required branding capability is `HubCapabilityIds.Moniker`.

This separates two responsibilities:

- **Authoring/publishing policy** decides whether a composition is publishable and may require the Moniker capability.
- **Runtime execution** follows the published composition and resolves the capabilities supplied by the Experiences in that composition.

The Hub therefore does not need the author's Moniker implementation compiled into it. A separately compiled Experience can dock to the Hub by declaring the capability it provides.

## Rule 5 — The Moniker is an Experience, not a special animation

> **The Moniker is an Experience whose presentation is extensible through MicroBundles.**

The Workshop's Moniker is one Experience that provides the `Moniker` capability. Its current floating-wave glyph treatment is only one presentation. A Moniker Experience may instead provide any combination of presentation and removal MicroBundles: assemble, orbit, dissolve, fold, scatter, grow, collapse, or future effects not yet imagined.

Presentation and removal are deliberately separate contracts. This allows an Experience to define how it arrives **and** how it leaves without requiring the Hub to understand either animation.

The developer decides where the Moniker Experience appears in the startup lineup. The host's responsibility is to honor that composition and to provide the startup presentation window for the terminal startup Experience; the Moniker does not own a global three-second rule merely because the current composition ends with it.

## Rule 6 — The Hub can become the authoring surface

> **The composition of the current page is itself an Experience and can expose its own manifest.**

Once the startup presentation hands control to the Hub, a composition surface can expose the manifest that describes the page currently being experienced. A developer can inspect, edit, save, and eventually publish that composition rather than being forced to follow a hard-coded default trail.

The same authoring surface can become the place where a developer creates or selects a Moniker Experience. The Moniker is therefore extensible at the same architectural boundary as every other Experience: composition, capabilities, MicroBundles, presentation, and removal.

Publishing remains subject to the Workshop's mandatory Moniker policy: a composition without the required Moniker capability is not previewable/publishable. Runtime composition itself remains generic.

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
       ├── ordered Startup experiences
       ├── ordered Transitioning experiences
       ├── ordered Running experiences
       ├── required capabilities
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

Version comparisons should use an explicit `Is(expected, actual)` assertion helper rather than an assertion whose intent is hidden in a generic `true` heartbeat. The current heartbeat for this composition/Moniker contract pass is **0.0.98**.
