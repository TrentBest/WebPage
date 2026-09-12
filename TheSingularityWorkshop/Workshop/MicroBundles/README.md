# MicroBundles

MicroBundles are a current **architectural experiment** in The Singularity Workshop.
They are independently schedulable units of behavior.

They are **not UI components**.

That distinction matters because the Workshop is separating what a piece of software
*does* from how a particular host chooses to *show it*.

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

The landing page is now a direct experiment in the same separation:

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

## Reusable animation direction

Animations are a natural next MicroBundle category.

For example, a future animation bundle could describe:

```text
BreathingGlow
PushThroughScreen
SpawnAndTravel
KelpSway
PanelContract
```

The semantic bundle should say what happens and expose state/progress. A provider
translates that behavior into CSS, SVG, Unity transforms, WPF animations, or another
host mechanism.

That lets the same animation capability be requested by multiple manifestations
without making the bundle itself know that a browser exists.

## Addressing: ontology + integer variant

The Hub already defines a nine-layer `OntologySignature`. A concrete MicroBundle
address adds an integer variant slot:

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

The important constraint is that publication must be **address-first and globally
unique**, not "last writer wins". Once an exact address is occupied, a second
publisher must receive a collision rather than silently replacing the first bundle.

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

### Document architectural direction

The road is fluid. Update this document and `CURRENT_VERTICAL_SLICE.md` when a
boundary changes instead of allowing the next agent to infer the architecture from
stale code.

---

*The bundle is small. The idea is not.*

*This way leads to the Singularity.*
