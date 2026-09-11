# True O(9) — Evidence Ledger

This ledger records observations that currently motivate or constrain the theory. It deliberately distinguishes implementation facts from theoretical conclusions.

## E1 — Nine-layer runtime representation

The Workshop architecture defines `OntologySignature` as a fixed-depth value containing nine integer layers plus version and capability metadata.

**Evidence type:** implementation architecture.

**What it supports:** the feasibility of representing a nine-layer structural address as a compact, immutable runtime value.

**What it does not prove:** that nine layers are sufficient or optimal for all domains.

## E2 — Deterministic structural comparison

The current signature design performs explicit comparison across its fixed layers and associated metadata.

**Evidence type:** implementation architecture.

**What it supports:** bounded comparison cost with respect to the number of ontology layers.

**Qualification:** any hash used as an optimization must not be mistaken for collision-free identity unless separately guaranteed.

## E3 — String-to-integer translation boundary

The broader Workshop design separates human-readable authoring vocabulary from runtime structural representation. Earlier ontology work used a registry to translate names into integer identities.

**Evidence type:** implementation history / architectural design.

**What it supports:** the practical usefulness of separating authoring vocabulary from runtime identity.

## E4 — Data structures keyed by ontological signatures

Workshop data-organization experiments have treated ontological signatures as keys for structured data resolution.

**Evidence type:** implementation history.

**What it supports:** the use of ontology as an addressable structural dimension for data tooling.

## E5 — Cross-platform design pressure

The Workshop is intentionally developing core structures that can be hosted by different application technologies. The Hub abstraction therefore avoids platform-specific scheduling primitives, while host integrations provide platform-specific behavior at the boundary.

**Evidence type:** architectural constraint.

**What it supports:** the presentation-independence motivation for a platform-neutral structural identity.

## E6 — FSM_API as a precedent

FSM_API demonstrates a related design pattern: pure C# core behavior separated from host/application concerns, with explicit processing groups and deterministic state logic. Its documentation describes the system as framework agnostic and identifies processing groups as a mechanism for organizing update cycles.

**Evidence type:** published implementation documentation.

**What it supports:** the broader architectural strategy of separating core structure from host-specific presentation and execution.

**Boundary:** FSM_API does not itself prove True O(9). It is precedent and engineering evidence, not direct proof of the ontology hypothesis.

## Evidence discipline

New evidence should record:

1. The implementation or experiment.
2. The observation.
3. Which proposition it bears on.
4. What alternative explanations remain.
5. What the observation cannot establish.

That distinction is essential if this corpus is eventually used as academic source material.
