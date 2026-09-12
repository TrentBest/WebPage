# MicroBundles

MicroBundles are a current **architectural experiment** in The Singularity Workshop.
They are independently schedulable units of behavior.

They are **not UI components**.

That distinction matters because the Workshop is separating what a piece of software
*does* from how a particular host chooses to *show it*.

## Recursive composition

A MicroBundle may contain other MicroBundles, and those children may themselves contain
children without a fixed authored depth. A domain is therefore not a special container
concept: **a domain can itself be a MicroBundle** and can master the bundles beneath it.

Composition is resolved during Development. The compiler validates missing children and
cycles, deduplicates shared children, and emits a deterministic finite closure. Runtime
receives that compiled closure rather than recursively discovering definitions.

```text
Authored MicroBundle
        |
        +--> MicroBundle
        |       +--> MicroBundle
        |       +--> MicroBundle
        |
        +--> MicroBundle
                +--> ...
        |
        v
Development compiler
        |
        v
finite static runtime closure
```

This gives us the useful combination:

> **Composition is recursive. Compilation is finite. Runtime is static.**

## Ontology addressing

MicroBundles are also addressable through the Hub's nine-layer integer
`OntologySignature`. The ontology coordinate is semantic identity, not a folder tree.

The first ontology-backed library establishes broad families such as:

```text
Software Abstractions
Digital Logic
Physics
    +-- Newtonian
    +-- Electricity and Magnetism
    +-- Thermodynamics
    +-- Hydrodynamics
    +-- Plasmadynamics
```

Physics topics occupy the **ninth ontology layer**. Their final-layer tokens are globally
unique across the physics families, so a token identifies a topic without becoming a
per-family slot that collides with the same numeric value elsewhere.

The important rule is that all nine coordinates remain available even when a particular
library branch has not yet assigned semantic meaning to every intermediate layer.

## Current lifecycle model

The runtime shell uses `FSM_API` through `IStateContext` and delegates visible
manifestation to an `IMicroBundleProvider`.

```text
MicroBundle
    |
    +-- IStateContext
    +-- FSM_API lifecycle
    +-- IMicroBundleProvider
           |
           +-- Manifestation
```

## Living Workshop application

The landing page is a direct experiment in the same separation:

```text
GATEWAY
   |
   v
LIVING GUI
   |
   +-- isolated LivingGui FSM_API group
   |      Seed -> Grow -> Reproduce -> Fly
   |
   +-- exactly 100
   |      -> freeze group
   |
   v
MONIKER behind GUI
   |
   v
preallocated Gravity FSM_API group
   |
   v
FALL AWAY
   |
   v
3 second dissipating phase
   |
   v
browser chrome / page arrival
```

The landing behavior is deliberately not implemented as a collection of CSS timers.
`FSMManagerService` supplies the application heartbeat; `PageFSM` owns progression;
the LivingGui and Gravity scheduler groups are separate runtime units.

## Addressing: ontology + integer variant

A concrete MicroBundle address can add an integer variant slot to the nine-layer ontology:

```text
OntologySignature (9 integer layers)
        +
VariantId [0, int.MaxValue - 1]
        =
MicroBundleAddress
```

Each ontology coordinate therefore has `int.MaxValue` addressable variant slots.
An **exact address has one occupant**. Variant `42` and variant `43` may both exist
under the same ontology; two publishers cannot both claim `(ontology, 42)`.

The current code makes this boundary explicit:

- `SingularityHub.Abstractions/MicroBundleAddress.cs` — value/address contract.
- `SingularityHub/MicroBundleRegistry.cs` — in-process one-occupant rule.
- `SingularityHub.Tests/MicroBundleAddressTests.cs` — executable uniqueness proof.

This is intentionally separate from `OntologySignature.StructuralId`. The ontology
identifies the structural kind; the variant identifies a concrete addressable slot.

## Public hosting direction

The desired end state is that a published MicroBundle is available to every Workshop
host immediately after publication, including bundles created by users.

The current WebPage is a client-side application, so an in-process registry cannot
honestly provide that property. The next infrastructure layer therefore needs a
shared/public catalog behind the same address contract:

```text
Publisher
   |
   v
validate ontology + variant ownership
   |
   v
public catalog / manifest
   |
   +--> bundle package + version + dependencies
   |
   +--> immutable address
   |
   v
any Workshop host
   |
   v
resolve address -> retrieve version -> verify -> load -> arbitrate
```

Persistence, authentication/ownership, package storage, versioning, trust/signing,
and CDN retrieval belong behind this boundary. Do not leak those concerns into the
presentation components.

## Development guideposts

### Preserve lifecycle clarity

A MicroBundle should have a clear lifecycle. If a behavior becomes a collection of
unrelated timer callbacks, reconsider the boundary.

### Preserve manifestation portability

Blazor is one manifestation. Unity/WebGL and WPF are other targets. A semantic
MicroBundle should not become permanently dependent on its first host.

### Preserve integer identity

Strings remain useful for people, authoring, debugging, and communication. Runtime
identity and AI-facing transport should increasingly use deterministic integer
coordinates where appropriate.

### Preserve recursive composition

Do not introduce a separate Domain abstraction merely to hold MicroBundles. If something
masters a collection of lesser MicroBundles, model that master as a MicroBundle and let
the development compiler flatten its recursive composition.

### Document architectural direction

The road is fluid. Update this document and `CURRENT_VERTICAL_SLICE.md` when a
boundary changes instead of allowing the next agent to infer the architecture from
stale code.

---

*The bundle is small. The idea is not.*

*This way leads to the Singularity.*
