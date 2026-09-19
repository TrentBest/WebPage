# World Composition, Authorship, and Human Agency

The Workshop is intended to reduce cognitive friction, not to remove human agency.

> **The software may help decide how something can be expressed; it must not silently decide consequential matters on a person's behalf.**

## Two kinds of space

### The Workshop

The Workshop is the author's controlled, persistent place shared with visitors. The author controls its structure, presentation, available tools, and progression.

### Singularity City

Singularity City is the user-extensible world. Users may rent or otherwise acquire permitted spaces, construct buildings, create Experiences, publish content, and influence the shared city according to its rules.

```text
Singularity World
  +-- Workshop
  |     +-- author-controlled persistent place
  |     +-- tools / teaching / demonstration
  |
  +-- Singularity City
        +-- user-extensible persistent place
        +-- rented or owned creative spaces
        +-- user-created buildings, Experiences, and content
```

The commercial and governance mechanisms are future infrastructure. This document establishes the architectural distinction, not a finished economic system.

## Creation wrapped around exploration

The user experience should not begin with a giant configuration form. A person should be able to explore a place, encounter a thing, understand what it does, and then choose to create or modify something related to it.

```text
Explore -> Understand -> Create -> Publish / place / use -> Explore the result
```

The environment is therefore part of the teaching mechanism. The user learns the architecture by using it.

## Example: a creator's building

A video blogger can build a virtual skyscraper with a recording studio near the top. The building is not merely geometry; it is an Experience composed from MicroBundles:

```text
Creator Skyscraper Experience
  +-- building structure
  +-- elevator / transit
  +-- recording studio
  +-- cameras / microphones
  +-- editing tools
  +-- creator identity
  +-- published content
```

Content can itself become a hierarchy:

```text
Product -> series -> video -> clips
```

## Authored applicability

A MicroBundle can provide explicit applicability metadata such as:

- audience/context tags;
- compatible Experience categories;
- permitted manifestation contexts;
- age/context classifications where appropriate;
- genre or subject tags;
- required capabilities;
- excluded contexts;
- dependencies; and
- author-defined presentation guidance.

The important distinction is **authored applicability versus inferred identity**. A creator can say, "This content is intended for these kinds of contexts." The runtime should not need a hidden psychological profile of every person in the world.

## Contextual composition

Suppose another creator expresses only: "I want an office." The composition system can derive a useful generic office proposal:

```text
Office
  +-- building
  +-- offices / cubicles
  +-- conference room
  +-- desks / computers / phones
  +-- common props
  +-- typical occupants
  +-- applicable content
```

Those pieces can come from MicroBundles authored by different creators:

```text
Office MicroBundle
  +--> Desk MicroBundle
  +--> Phone MicroBundle
  +--> Computer MicroBundle
  +--> ConferenceRoom MicroBundle
  +--> Coworker MicroBundle
```

A Coworker MicroBundle can expose compatible contexts and references to other bundles. The system can then use the expressed context to select compatible content.

```text
Office context -> context requirements -> compatible actors / props / media -> composition proposal -> human accepts / edits
```

## Semantic affinity is not a stereotype engine

"Typical audience" is a compatibility signal, not a declaration about what a person is allowed to enjoy.

If a creator explicitly tags a video as appropriate for an office-media context, the composition system may select it as a candidate for a phone in an office scene. It should not require a rule such as "Person X is this type of person, therefore Person X must receive this content."

The system is matching **content to context**, not assigning people to cultural categories.

## References are first-class composition relationships

A MicroBundle should be able to say that another MicroBundle is relevant without copying that bundle into itself.

References may eventually support required dependencies, optional recommendations, compatible companions, default props, content channels, alternate manifestations, replacement providers, and related Experiences.

These relationships should remain explicit and inspectable.

## Human approval boundary

Procedural composition can make a proposal. It should not silently perform consequential external actions.

```text
generate proposal
    -> explain selected bundles
    -> user edits
    -> user commits
    -> publish / apply
```

Generating a layout and showing why it was suggested can be highly automated. Charging money, transferring ownership, signing an agreement, publishing on someone's behalf, or otherwise creating a consequential external commitment requires an explicit human-controlled boundary.

## Self-describing, not self-governing

The WebPage should be **self-describing and self-teaching**, not self-governing.

A visitor should be able to discover:

```text
What am I looking at?
  -> What MicroBundles make it?
  -> What does each provide?
  -> What does each reference?
  -> Why was this composition suggested?
  -> What can I change?
  -> What will happen if I commit it?
```

The GUI Builder, Ontology Mall, spatial world, MicroBundle manifests, and Experiences all contribute to this teaching loop.

## Architectural consequences

1. **MicroBundles need authored applicability.** Applicability is explicit data, not hidden page-specific logic.
2. **MicroBundles need inspectable references.** A bundle can point at other bundles without owning their implementation.
3. **Experience composition needs provenance.** A composed Experience can explain which bundle contributed which part.
4. **Composition should be previewable.** A derived composition is a proposal until committed.
5. **Human actions need a clear commit boundary.** Consequential changes have an explicit user action and auditable result.
6. **FSM_API remains the behavior substrate.** Stateful actors, buildings, vehicles, content pipelines, and publishing stages should not grow a competing state system.
7. **GUI Builders remain the manifestation boundary.** Semantic composition belongs to Experiences, MicroBundles, Providers, and manifests. CSS styles the manifestation; it does not define semantic construction.

## Relationship to the existing architecture

```text
Singularity World
  +-- persistent places
  |     +-- Workshop
  |     +-- Singularity City
  +-- Experiences
  |     +-- MicroBundles
  |           +-- authored semantics
  |           +-- applicability
  |           +-- references
  |           +-- providers
  +-- Hub
  |     +-- discovery / loading / arbitration / provenance
  +-- FSM_API
  +-- GUI Builders
  +-- human decision boundaries
```

> **Automation reduces cognitive friction. It does not remove human responsibility.**

*The Workshop helps people build the world. People decide what becomes part of it.*