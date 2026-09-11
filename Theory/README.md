# Theory & Literature

This branch is the working library for the theory behind The Singularity Workshop and the technology developed within it.

It serves two audiences:

1. **Builders** — understand what a system does, how it works, and why its architecture is shaped that way.
2. **Readers and researchers** — professors, students, authors, and investigators who want stable material that can be cited or incorporated into larger works.

## Layers

- `Technology/` — implemented technology, architecture, mechanisms, invariants, algorithms, performance, and design rationale.
- `Literature/` — chapters, papers, references, and source material informing the Workshop's explorations.
- `Academic/` — Workshop-authored theory and exposition intended to stand on its own.

## Documentation rule

Where applicable, every document should answer:

- **What is it?**
- **How does it work?**
- **Why is it this way?**

Distinguish implemented facts, observations, design principles, hypotheses, and interpretations.

## Provenance

Literature entries preserve title, author, source, date/version when known, provenance, and the relationship to Workshop work. Adaptation and commentary must be identified rather than presented as source material.

## Relationship to source code

Source code remains authoritative for implementation. This branch records the explanations and theory surrounding it. The FSM_API documentation is the model: overview and motivation, quickstart, core concepts, features, internal architecture, and execution-cycle reasoning are kept as progressively deeper layers.

## Branch

`theory/literature` is deliberately separate from implementation branches so the theoretical library can evolve independently and later receive contributions from stable implementation work.
