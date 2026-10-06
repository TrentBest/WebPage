# Experience Theory

This document records the current WebPage definition of an **Experience**. It is theory and contract, not a roadmap for unrelated hosts. It is theory and contract, not merely presentation documentation. Changes to the definition should be reflected in executable tests as the model becomes concrete.

## Definition

An **Experience** is an environment comprised of MicroBundles.

MicroBundles are the focused units of capability, behavior, content, and sensory contribution. An Experience is the environment in which those MicroBundles coexist and are executed.

An Experience is therefore not synonymous with a Blazor page, a host-specific scene, a component, or an idle/screen-saver state. Those are possible manifestations or hosts.

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

> **The Moniker is a capability/Experience participant used by the current WebPage proof. It is not a special exception to the composition model.**

The Moniker is not a special case in the runtime. Its current presentation is one possible manifestation of a capability-bearing Experience.

## Rule 6 — The Hub can become the authoring surface

> **The composition of the current page is itself an Experience and can expose its own manifest.**

A composition surface can eventually inspect, edit, save, and publish the Experience composition instead of following a hard-coded default trail.

## Rule 14 — Perception precedes explanation

> **The opening should demonstrate the Workshop before asking the visitor to understand it in prose.**

The public landing page is a perception boundary. Its job is to reduce cognitive friction by giving the visitor evidence of the system's character through a short authored sequence of visual, spatial, and eventually auditory manifestations.

The sequence is:

`ARRIVAL → IDENTITY → TENSION → SHOW → INVITATION → WORKSHOP`.

The semantic structure belongs to recursive GUI Builders. FSM_API should own meaningful presentation lifecycle as the sequence becomes stateful. CSS and browser audio APIs remain manifestation mechanisms rather than architectural owners.

"Shock and awe" means presentation intensity, not manipulation. The system must not hide consequential behavior, invent capabilities, or make consequential decisions for the visitor.

See `WORKSHOP_OPENING_EXPERIENCE.md`.
