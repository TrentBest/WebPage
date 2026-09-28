# MicroBundles

MicroBundles are a current **architectural boundary** in The Singularity Workshop.
They are focused units of capability, behavior, content, and sensory contribution.

They are **not UI components**.

That distinction matters because the Workshop separates what software *does* from how a particular host chooses to *show it*.

## Runtime model

```text
MicroBundle
    |
    +-- identity / version / ontology
    +-- dependencies
    +-- state / lifecycle
    +-- semantic effects
    |
    v
Provider
    |
    +-- Blazor
    +-- WPF
    +-- Unity
    +-- WebGL
    +-- other manifestation
```

A semantic MicroBundle should not need to know which manifestation host will render it.

## Bootstrap role

The WebPage host launches the Hub with a host manifest. The manifest identifies the registry/repository and host policy; it does not contain a hardcoded list of MicroBundles or Experiences.

The Hub discovers available inventory, resolves versions/dependencies, arbitrates the selected composition, and owns runtime scheduling.

```text
WebPage host manifest
        |
        v
      Hub boot
        |
        v
MicroBundle / Experience registry
        |
        +--> discover
        +--> select by capability/policy
        +--> resolve
        +--> load
        +--> arbitrate
        v
Experience
        |
        v
FSM_API runtime
```

## Living Workshop application

The landing page is one manifestation of this architecture. Its current concrete showcase is the Living GUI Flex experience, but the bootstrap must not depend on the name `Living GUI`.

The internal Living GUI lifecycle remains:

```text
LIVING GUI
   |
   +-- isolated FSM_API groups
   |      Seed -> Grow -> Reproduce
   |
   +-- exactly 100 nodes
   |      -> freeze
   |
   +-- moniker reveal
   |
   +-- preallocated Gravity FSM_API group
   |      -> nodes fall away
   |
   +-- three-second dissipating phase
   |
   +-- page arrival
```

The page may render this state, but it must not become the authoritative lifecycle owner.

## Idler and Flex

Idler and Flex are capabilities/categories used by host selection policy.

The first concrete vertical slice is:

```text
Pong       -> Idler-capable
Living GUI -> Flex-capable
```

These are not special MicroBundle types and should not become hardcoded bootstrap keywords. Future Experiences can expose the same capabilities or introduce others.

## Addressing: ontology + integer variant

The Hub defines a nine-layer `OntologySignature`. A concrete MicroBundle address adds an integer variant slot:

```text
OntologySignature (9 integer layers)
        +
VariantId [0, int.MaxValue - 1]
        =
MicroBundleAddress
```

Each ontology coordinate therefore has `int.MaxValue` addressable variant slots. An **exact address has one occupant**. Variant `42` and variant `43` may both exist under the same ontology; two publishers cannot both claim `(ontology, 42)`.

The current code makes this boundary explicit:

- `SingularityHub.Abstractions/MicroBundleAddress.cs` — value/address contract.
- `SingularityHub/MicroBundleRegistry.cs` — in-process one-occupant rule.
- `SingularityHub.Tests/MicroBundleAddressTests.cs` — executable uniqueness proof.

This is intentionally separate from `OntologySignature.StructuralId`. The ontology identifies the structural kind; the variant identifies a concrete addressable slot.

## Public registry direction

The desired end state is that a published MicroBundle is available to every Workshop host immediately after publication, including bundles created by users.

The current WebPage is a client-side application, so an in-process registry cannot honestly provide that property. The next infrastructure layer therefore needs a shared/public catalog behind the same address contract:

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
   +--> immutable address
   |
   v
any Workshop host
   |
   v
resolve address -> retrieve version -> verify -> load -> arbitrate
```

The important constraint is that publication must be **address-first and globally unique**, not "last writer wins". Once an exact address is occupied, a second publisher must receive a collision rather than silently replacing the first bundle.

Persistence, authentication/ownership, package storage, versioning, trust/signing, and CDN retrieval belong behind this boundary. Do not leak those concerns into presentation components.

## AI relationship

AI, Grammar, Protocol, and command-pipeline work remains valuable future architecture. It is deliberately not a prerequisite for MicroBundle loading or runtime execution.

The future direction is:

```text
AI / Grammar / Protocol
          |
          v
 deterministic command / selection
          |
          v
MicroBundle / Experience boundary
          |
          v
Hub arbitration
          |
          v
FSM_API execution
```

The runtime must remain deterministic and fully usable without AI.

## Development guideposts

### Preserve lifecycle clarity

A MicroBundle should have a clear lifecycle. If a behavior becomes a collection of unrelated timer callbacks, reconsider the boundary.

### Preserve manifestation portability

Blazor is one manifestation. Unity/WebGL and WPF are other targets. A semantic MicroBundle should not become permanently dependent on its first host.

### Preserve integer identity

Strings remain useful for people, authoring, debugging, and communication. Runtime identity and AI-facing transport should increasingly use deterministic integer coordinates where appropriate.

### Preserve behavior during migration

Hardcoded catalogs and host-specific implementations may exist temporarily while the registry architecture is assembled. Do not delete them until their functionality has been located, tested, and represented by the Experience/MicroBundle boundary.

### Document architectural direction

The road is fluid. Update this document and `CURRENT_VERTICAL_SLICE.md` when a boundary changes instead of allowing the next agent to infer the architecture from stale code.

---

*The bundle is small. The idea is not.*

*This way leads to the Singularity.*


---

## 🔗 The Singularity Workshop

This project is part of a deliberately troublesome ecosystem:

- **[FSM_API](https://github.com/TrentBest/FSM_API)** — behavior and state.
- **[FSM_COS](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS)** — composition and runtime assembly.
- **[FSM_Serialization](https://github.com/TrentBest/TheSingularityWorkshop.FSM_Serialization)** — representation and the byte boundary.
- **[WebPage](https://github.com/TrentBest/WebPage)** — browser manifestation and proving ground.
- **[FSM_API_Unity](https://github.com/TrentBest/FSM_API_Unity)** — Unity manifestation.

<p align="center"><em>The Singularity Workshop — Tools for the curious, the bold, and the systemically inclined.</em><br><strong>Because state shouldn't be a mess.</strong><br><em>And because static boundaries are invitations to cause trouble.</em></p>
