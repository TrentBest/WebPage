# True O(9) — Propositions

These propositions are working research claims. They are intentionally stated so that implementation, measurement, counterexamples, or independent research can strengthen, qualify, or reject them.

## P1 — Fixed-depth structural addressing

A fixed-depth ontological address can provide stable structural identity without requiring an unbounded semantic hierarchy.

### Rationale

A fixed number of layers makes the representation predictable, directly comparable, and suitable for compact runtime structures. The current Workshop hypothesis uses nine integer layers.

### Test

Demonstrate that representative domains can be expressed without requiring runtime expansion of the address itself.

## P2 — Constant layer-comparison cost

If the address depth is fixed at nine layers, comparing two complete addresses requires a bounded number of primitive comparisons and is therefore O(1) with respect to address depth.

### Qualification

This does not imply that lookup, allocation, graph traversal, serialization, or downstream work is automatically O(1).

## P3 — Numeric runtime identity

Semantic names should be translated into numeric or otherwise fixed structural identities before entering performance-sensitive runtime paths.

### Rationale

Human-readable strings are useful for authoring, diagnostics, documentation, and translation. They introduce variable-size data and comparison costs when used as the fundamental runtime identity.

### Test

Compare equivalent runtime workflows using string-backed and integer-backed identity while measuring allocation, comparison cost, and lookup behavior.

## P4 — Presentation independence

An ontological address should remain meaningful when the same underlying entity is presented through different UI technologies, application frameworks, or execution environments.

### Test

Represent the same conceptual capability through WebPage, WPF, Unity, or another host while retaining the same structural identity at the core boundary.

## P5 — Ontology as an interoperability boundary

A shared ontological coordinate system can reduce translation between otherwise independent subsystems by giving them a common structural vocabulary.

### Consequence

Micro-bundles, providers, warehouse records, tooling, and AI composition can refer to structural identities without requiring each subsystem to understand another subsystem's presentation vocabulary.

## P6 — Ontology precedes arbitration

Arbitration becomes more deterministic when bundle identity, capability relationships, dependencies, and mutation targets are represented structurally rather than inferred from free-form names.

### Test

Construct equivalent arbitration scenarios using string identifiers and fixed structural identifiers, then compare determinism, validation cost, and auditability.

## P7 — Nine is a hypothesis, not a theorem

The usefulness of nine layers must be demonstrated by domain coverage and architectural behavior; the number nine itself is not established as a universal constant by this theory.

This proposition is important because it prevents the architecture from becoming dogma. A useful theory must survive attempts to break it.

## P8 — The address is a coordinate, not the entity

An ontological signature identifies a structural position or identity within the Workshop's semantic space. It does not replace the entity, its data, its behavior, or its storage location.

This distinction permits the same coordinate system to participate in warehouse resolution, bundle arbitration, and tooling without collapsing those concerns into one object.

## P9 — Theory must remain traceable to implementation

Every strong claim in True O(9) should be traceable to one or more concrete implementations, experiments, measurements, or explicit counterexamples.

The repository therefore treats code, benchmarks, diagrams, and architectural decisions as evidence sources rather than as proof by themselves.
