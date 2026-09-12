# MicroBundle Content Boundary

## The distinction

The Workshop does **not** invent the knowledge represented by external content.

External content arrives as **pure data**. It has no environment, no ontological behavior, no process-group participation, and no Workshop tooling merely because it entered the system.

Forge/development time may then explicitly wrap that data in one or more MicroBundles. The MicroBundle is the authored semantic and tooling shell around the content.

```text
External Content
      |
      | explicit wrapping during Forge/development
      v
MicroBundle
      |
      +-- providers
      +-- editors
      +-- inspectors
      +-- diagnostics
      +-- documentation
      +-- ontology participation
      +-- relationships
      +-- child MicroBundles
      |
      v
deeper MicroBundle
      |
      v
runtime leaf
```

## No automated knowledge inception

The Forge may **discover relationships among knowledge that already exists**, but discovery is not authorship.

The canonical knowledge for an ontology belongs to its authoritative source. For the Workshop's own ontology, the Workshop remains the authoritative owner of its definitions unless an explicit source/authority policy says otherwise.

AI may assist development by prepopulating candidate knowledge, classifications, relationships, tests, and lookup tables. Those candidates become authored Workshop knowledge only through the normal development/curation process.

This preserves the distinction between:

- **knowledge creation/authority** — authored and owned by a source;
- **external content** — data imported without Workshop semantics;
- **MicroBundle wrapping** — an explicit authored decision that gives content Workshop semantics and tooling;
- **Forge-time relationship discovery** — finding useful connections between already-defined things;
- **runtime composition** — executing the compiled result without rediscovering its meaning.

## The onion

MicroBundles are recursively composable. An outer MicroBundle can provide the complete development surface for everything inside it, while progressively deeper bundles can shed tooling until the innermost runtime leaf contains only what execution requires.

The conceptual direction is:

```text
Conceptual Root
    |
    +-- development tooling
    +-- authoring tooling
    +-- diagnostics
    +-- inspectors
    +-- editors
    +-- relationship/review tooling
    |
    +-- MicroBundle
          |
          +-- MicroBundle
                |
                +-- runtime composition
                      |
                      +-- pure runtime asset/behavior/data
```

The onion continues until the **top-most conceptual word** is reached on the outside and pure runtime composition is reached on the inside.

## Environment is derived later

An external object does not arrive carrying an environment.

After wrapping, the Forge can examine the MicroBundle graph and ask:

```text
Is an environment explicitly present?
    |
    +-- yes -> use it
    |
    +-- no -> inspect the installed bundle set
                 |
                 +-- what environments can satisfy these bundles?
                 +-- what relationships are implied?
                 +-- what is required?
                 +-- what is merely related?
                 +-- what alternatives exist?
                 +-- what conflicts?
                 |
                 +-- present the proposed composition for review
```

That review is where intentional content must remain distinguishable from related or derived content. The user remains able to reject, replace, fork, or refine the proposed composition before runtime compilation.

## Runtime boundary

All of the expensive semantic work belongs to Forge/development time whenever possible:

```text
external data
    -> explicit MicroBundle wrapping
    -> ontology/relationship resolution
    -> recursive MicroBundle compilation
    -> environment inference/review
    -> static runtime closure
    -> Hub cache
    -> FSM_API process groups
```

The runtime should receive the compiled result, not perform open-ended knowledge inception.

**Forge discovers and composes authored knowledge. Hub executes compiled composition.**
