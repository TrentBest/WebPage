# True O(9) — Open Questions

A research theory becomes useful when it records where it can fail. These questions define the next investigations.

## Q1 — Why nine?

What recurring semantic decomposition led to nine layers, and can that decomposition be demonstrated across sufficiently different domains to justify the fixed depth?

## Q2 — What belongs in each layer?

Can layer semantics be defined precisely enough that two independent authors map the same domain to equivalent coordinates without relying on private intuition?

## Q3 — What happens at the boundary?

How should entities that appear to belong to multiple ontological locations be represented? Can one signature represent them, or should relationships between signatures carry the ambiguity?

## Q4 — Is the signature sufficient for identity?

A structural coordinate may locate an entity without uniquely identifying its instance. What additional identity mechanism is required when multiple runtime objects occupy the same ontological position?

## Q5 — How should versioning interact with ontology?

When an ontology layer's vocabulary evolves, how are old signatures preserved, translated, deprecated, or migrated without corrupting historical data?

## Q6 — How does O(9) interact with O(1) storage continuity?

Can ontological identity be translated directly into the Data Warehouse's page/shelf addressing model without introducing scans or secondary search structures?

## Q7 — Can arbitration remain deterministic?

Can MicroBundle arbitration use ontological coordinates to produce reproducible outcomes while still permitting controlled mutation and collaboration among bundles?

## Q8 — Can the audit graph explain ontology mutations?

When arbitration changes an entity's semantic structure, can the audit graph preserve enough causal information to reconstruct why the change occurred and which bundle caused it?

## Q9 — Can an AI compose against the ontology safely?

Can an AI system operate primarily against integer structural vocabulary while using human-readable mappings only at the translation boundary, reducing ambiguity without making the AI dependent on implementation-specific strings?

## Q10 — What would falsify True O(9)?

A serious investigation should identify counterexamples. Candidate falsifiers include domains that consistently require unbounded depth for correct representation, unavoidable ambiguity that cannot be represented structurally, or measurable system behavior showing that the fixed-depth coordinate introduces unacceptable coupling or cost.

## Next experiments

The most valuable experiments are expected to be:

1. Map several substantially different domains into the nine-layer model.
2. Measure integer-backed signature comparison and lookup behavior.
3. Connect signatures to the Data Warehouse liaison and measure continuity.
4. Run MicroBundle arbitration scenarios with structural identities and inspect the audit graph.
5. Attempt deliberate counterexamples rather than only successful demonstrations.
