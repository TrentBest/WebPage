# Workshop Documentation Standard

## Purpose

The Singularity Workshop does not use documentation as decoration. Documentation is part of the engineering system.

A good document lets a reader answer three questions:

1. What exists?
2. Why does it exist?
3. Who owns it?

## The three-state rule

Every architectural document must distinguish:

### Current

State facts that can be verified in the repository or running product.

### Direction

Design intent that is actively guiding implementation but is not necessarily complete.

### Future

Ideas, research, or capabilities that are explicitly not yet implemented.

Never use future language to imply a feature already works.

## The ownership rule

For every significant behavior, documentation should identify its owner.

```text
Experience       = what is being composed
MicroBundle      = focused capability
FSM_COS          = composition and RuntimeAssembly
FSM_API          = deterministic state execution
Renderer         = rendering computation / representation policy
WebPage          = browser presentation and proving-ground UX
Repository       = artifact discovery / persistence when assigned
```

Do not move ownership upward merely because the current host happens to call the code.

## The evidence rule

A claim should have an observable proof:

- a test;
- a running behavior;
- a benchmark;
- a repository contract;
- or an explicitly labeled research hypothesis.

Visual behavior deserves visual evidence. Architecture deserves tests and diagrams. Performance claims deserve measurements.

## The stale-reference rule

Documentation must not carry obsolete platform assumptions forward simply because they existed in an earlier experiment.

If a historical integration remains relevant, point to its dedicated repository rather than rebuilding its architecture inside WebPage documentation.

WebPage is browser-first. Platform-specific integrations are separate concerns.

## The language rule

Prefer precise Workshop vocabulary:

- Experience
- MicroBundle
- RuntimeAssembly
- composition
- arbitration
- ontology
- manifestation
- provider
- Deep Dive
- proving ground

Do not invent synonyms that make ownership ambiguous.

## The visual rule

Use a diagram when a diagram is clearer than prose.

Use a screenshot when the reader needs to see the actual product.

Use a table when comparing contracts or ownership.

Do not turn every README into a wall of text.

## The honesty rule

Say when something is transitional.

Say when a path is experimental.

Say when another repository owns the canonical implementation.

Say when a capability does not exist yet.

That is not weakness. It is how a workshop earns trust.

## The public-writing rule

Public documentation should be technically honest while still sounding like The Singularity Workshop.

Be precise. Be playful when useful. Be strange when the software is strange.

> **Dr. Seuss for software developers — but with the tests passing.**

## Completion rule

A meaningful implementation change is not complete until:

1. the code is correct;
2. the tests prove the intended behavior;
3. the relevant documentation agrees with the code;
4. obsolete claims are removed;
5. CI is checked;
6. the commit can be understood by the next engineer.

*If the documentation lies, the Workshop has already begun to drift.*


## Package extraction

WebPage is allowed to be the fastest place to prove a capability. It is not automatically the permanent owner.

When behavior moves into a reusable package, documentation should move with it:

~~~text
WebPage proof
   ↓
package contract
   ↓
package usage
   ↓
theory / decision
   ↓
WebPage integration proof
~~~

The WebPage repository should explain how it consumes the package and why the boundary exists. The package repository should explain how developers consume the reusable capability.

Do not copy an entire package's theory into WebPage. Link to the package's authoritative documentation and document only the WebPage integration.

## FSM_COS usage

If a WebPage feature crosses the composition boundary, the authoritative local guide is FSM_COS_USAGE.md.

That document should answer:

- what Experience is selected;
- which MicroBundles are requested;
- how the catalog resolves them;
- how the RuntimeManifest is constructed;
- what FSM_COS owns;
- what RuntimeAssembly means;
- what WebPage does after handoff.

This is the operational companion to the deeper FSM_COS theory.
