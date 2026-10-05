# The Singularity Workshop — WebPage

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![FSM_API](https://img.shields.io/badge/FSM_API-1.0.13-00A98F?style=flat-square)](https://github.com/TrentBest/FSM_API)
[![Tests](https://img.shields.io/badge/tests-GitHub%20Actions-f39c12?style=flat-square&logo=githubactions&logoColor=white)](https://github.com/TrentBest/WebPage/actions)
[![Repository](https://img.shields.io/badge/repository-public-238636?style=flat-square&logo=github)](https://github.com/TrentBest/WebPage)

> **The page opens the door. The Experience gives you somewhere to stand. The machinery lets you look underneath it.**

## What this repository is

WebPage is the public browser proving ground for The Singularity Workshop.

It is a real Workshop host, not a screenshot gallery and not the canonical home of every runtime contract. It exists to make the architecture observable: visitors can witness real behavior, Experiences can be composed from MicroBundles, FSM_COS can assemble the selected runtime, and Deep Dives can explain what was just witnessed.

> **Show the behavior. Preserve the semantics. Explain the machinery.**

## First contact

The current opening is deliberately experiential rather than a conventional marketing hero:

ARRIVE → LABEL 1 → LABEL 2 → ENTER THE WORKSHOP → explicit entry → FSM_COS composition → LIVING GUI → population threshold → gravity → Moniker → Workshop navigation.

This is current WebPage behavior. It is not a specification for other Workshop hosts or future Experiences.

## Runtime boundary

WebPage owns browser presentation, visitor interaction, discovery, authoring UX, and proving-ground behavior. FSM_COS owns composition, dependency closure, loading, arbitration, convergence, and RuntimeAssembly. MicroBundles own focused capabilities. Experiences describe composed environments. The renderer owns rendering computation rather than becoming a WebPage identity.

```text
Visitor
  ↓
WebPage / Blazor
  ↓
Experience
  ↓
FSM_COS
  ↓
RuntimeAssembly
  ↓
WebPage manifestation
```

Shared runtime layers must not acquire a dependency on WebPage merely because this is the most visible host.

## MicroBundles and Deep Dives

MicroBundles are focused capability units and increasingly act as collections of optional providers. WebPage currently acquires a WebPage-only Deep Dive provider from a composed MicroBundle when one is available.

Deep Dive is the Workshop's educational unit:

WITNESS → WONDER → DEEP DIVE → UNDERSTAND → CREATE → RUN → PUBLISH

> **Don't copy the trick. Use the machinery.**

See [DEEP_DIVE_PROVIDER_ARCHITECTURE.md](DEEP_DIVE_PROVIDER_ARCHITECTURE.md).

## Identity

The Workshop keeps Experience ID, MicroBundle ID, and Ontology Signature distinct. Experience identity answers what is running; MicroBundle identity answers what capability is composed; ontology answers what the thing is structurally. WebPage proves these ideas but does not own the canonical runtime contracts.

## Rendering

> **REMOVE THE CUBES. RENDER WHAT REMAINS.**

The renderer direction explores observer-relative detail, Event Horizons, representation policy, computation/workload separation, and state-driven manifestation. WebPage demonstrates the work; the renderer remains a separate architectural concern.

## Public surfaces

Explore · Rendering · Create · Education · MadMen · About Us · Consult

These are different doors into one Workshop. See [PUBLIC_WORKSHOP_TABS.md](PUBLIC_WORKSHOP_TABS.md).

## Documentation

Start with [DOCUMENTATION_INDEX.md](DOCUMENTATION_INDEX.md) and [DOCUMENTATION_STANDARD.md](DOCUMENTATION_STANDARD.md). The documentation standard requires every document to distinguish current behavior, architectural direction, and future work. Documentation is engineering memory, not marketing fiction.

Key contracts:

- [WORKSHOP_RUNTIME_ARCHITECTURE.md](WORKSHOP_RUNTIME_ARCHITECTURE.md)
- [WEBPAGE_EXPERIENCE_ARCHITECTURE.md](WEBPAGE_EXPERIENCE_ARCHITECTURE.md)
- [WORKSHOP_OPENING_EXPERIENCE.md](WORKSHOP_OPENING_EXPERIENCE.md)
- [EXPERIENCE_THEORY.md](EXPERIENCE_THEORY.md)
- [MICROBUNDLE_ARBITRATION_MAP.md](MICROBUNDLE_ARBITRATION_MAP.md)
- [CURRENT_VERTICAL_SLICE.md](CURRENT_VERTICAL_SLICE.md)

## Platform integration

WebPage is browser-first. The separate Unity integration maintained by the Workshop is [FSM_UnityIntegrationAdvanced](https://github.com/TrentBest/FSM_UnityIntegrationAdvanced). It is not part of this WebPage runtime architecture.

## Development

```bash
git clone https://github.com/TrentBest/WebPage.git
cd WebPage
dotnet restore
dotnet run
```

Active engineering occurs on development. Before calling a change complete: build it, run the relevant tests, inspect the behavior, update the documentation, and verify GitHub Actions.

## Workshop quality bar

- zero warnings and zero avoidable errors;
- explicit lifecycle and state ownership;
- clean dependency direction;
- XML documentation for public contracts;
- tests that prove architectural intent;
- visual evidence for visual behavior;
- documentation that describes what the code actually does;
- incremental, inspectable changes;
- preserve valuable behavior before deleting or simplifying it.

> **Can a visitor experience it, can an engineer explain it, and can the architecture tell us who owns it?**

## The larger idea

The Singularity Workshop is not claiming that the Singularity has arrived. We are building machinery that makes increasingly strange software possible to compose, observe, understand, and create.

*This way leads to the Singularity.*

*Built by The Singularity Workshop.*

---
# FSM_API

`FSM_API` is the core state technology behind much of the Workshop.

The goal is not simply to have another finite-state-machine library. The larger
purpose is to establish a small, deterministic state vocabulary that can be used
across otherwise unrelated systems.

That makes state a useful boundary between:

- intent
- behavior
- runtime
- UI
- engines
- tools
- AI commands

The current FSM_API work includes both the established string-backed implementation
and the evolving integer-backed implementation.

The integer-backed direction matters because strings are excellent for humans but
are not always the representation we want on a hot path or inside an AI command
stream.

The intended progression is roughly:

```text
Human-readable authoring
        |
        v
String identity / documentation
        |
        v
Integer identity / compact transport
        |
        v
Deterministic runtime state
```

The exact API and representation are still evolving. Documentation should describe
what is currently true rather than pretending the future design is already finished.

---

# SingularityWarehouse

The **SingularityWarehouse** is the larger data/infrastructure experiment behind
another major part of the Workshop.

The interesting question is not merely:

> "Where do I store this object?"

It is:

> **"How does software know what this thing is, what it relates to, what identity
> it carries, and how it participates in the runtime?"**

That leads naturally toward:

```text
DATA
  |
  v
IDENTITY
  |
  v
ONTOLOGY
  |
  v
RELATIONSHIP
  |
  v
STATE
  |
  v
RUNTIME
```

The Warehouse work is therefore connected to the same larger problem as FSM_API:
how do we create a deterministic vocabulary that software — and eventually AI — can
operate on without continuously paying the cost of ambiguous human-readable text?

This is still an evolving body of work. Preserve that distinction in future docs.

---

# AI / Protocol / Grammar / CommandAI direction

Another thread of the Workshop grew out of an earlier C# AI execution environment.

The useful lesson was not "make an AI chatbot."

It was:

> **How do you let probabilistic intelligence operate a deterministic machine
> without allowing ambiguity to leak through the boundary?**

The evolving conceptual stack is:

```text
LLM
 |
 v
CommandAI
 |
 v
Grammar
 |
 v
ProtocolAI
 |
 v
Deterministic execution
```

The names and boundaries may change as the work progresses.
The problem being attacked is the durable part.

The eventual integer-backed mapping work extends this idea: let the AI understand
human-readable concepts while giving the runtime compact, deterministic identities.

---

# MicroBundles

MicroBundles are a concrete experiment in separating **behavior** from
**manifestation**.

The current model is:

```text
MicroBundle
    |
    +-- state / lifecycle
    |
    +-- semantic effects
    |
    v
Provider
    |
    +-- Blazor
    +-- WPF
    +-- other manifestation
```

A MicroBundle should describe what is happening without needing to know whether
that behavior eventually appears as HTML, CSS, a host object, sound, speech, or
something else.

See:

`TheSingularityWorkshop/Workshop/MicroBundles/README.md`

for the current implementation-level notes.

---

# Development guideposts

These are more important than any particular class name.

## 1. Preserve intent before replacing implementation

The Workshop has a long history of experiments. Before replacing something that
looks strange, inspect its history.

A simpler implementation may be technically cleaner while accidentally destroying
the behavior that made the experiment valuable.

## 2. The visual behavior is evidence

The living landing page is not just presentation.
It is an observable experiment.

If something breathes, reproduces, collapses, or crosses a runtime boundary, that
behavior should eventually have a clear architectural explanation.

## 3. Prefer deterministic state over event spaghetti

Timers, CSS animation, browser events, and rendering loops are implementation
mechanisms.

Meaningful lifecycle should have explicit state and ownership.

The long-term direction is for FSM_API to carry that meaning wherever practical.

## 4. Keep semantics independent from manifestation

Do not let a behavior become permanently coupled to Blazor simply because Blazor
was the first place we demonstrated it.

## 5. Performance is part of the design

The Workshop should be able to create the *impression* of overwhelming complexity
without actually allocating an absurd amount of unnecessary work.

Exponential visual behavior is useful precisely because the system can represent
many actors while keeping each actor lightweight.

## 6. Document the road, not just the destination

The implementation will change.

That is expected.

README files should therefore record:

- what currently exists
- what problem it solves
- why it exists
- what direction it is moving
- what is experimental
- what should not be casually removed

Do not turn temporary implementation details into false architectural promises.

## 7. Use history as engineering memory

Git history is part of the documentation.

When recovering or refactoring Workshop behavior, inspect earlier versions before
assuming that the current version contains the whole story.

## 8. A successful build is necessary, not sufficient

The Workshop is an architectural proving ground.

A feature is successful when its behavior is understandable, deterministic where it
needs to be, portable where it should be, performant enough for its purpose, and
consistent with the larger direction.

---

# Current repository role

This WebPage repository provides:

- the public Workshop presence
- publisher/compliance pages
- demonstrations
- the living landing experiment
- WebAssembly/Blazor runtime integration
- Workshop-facing developer infrastructure
- a place to make the larger architecture visible

It is **not** the entirety of The Singularity Workshop.

Think of it as the public laboratory door.

Behind that door are the other repositories and experiments.

---

# Local development

```bash
git clone https://github.com/TrentBest/WebPage.git
cd WebPage
dotnet restore
dotnet run
```

The HTTPS/HTTP port is determined by the project's launch settings.

For active development, work on `development`. The repository is deliberately consolidated
to two branches: `master` is the stable promotion target and `development` is the active
engineering branch. Do not create a new feature branch merely to avoid reconciling an
architectural problem; consolidate the work on `development` instead.

Do not introduce OneDrive-specific assumptions into the project or documentation.
The repository should remain portable to an ordinary local development workspace
and CI environment.

---

# Documentation and articles

The Workshop should document itself while it is being built.

The intended writing style is technical, honest, playful, and occasionally absurd.

Or, more simply:

> **Dr. Seuss for software developers.**

The writing should be capable of explaining a difficult architecture without
pretending software is less weird than it actually is.

Future articles should explore:

- SingularityWarehouse
- ontology and identity
- integer-backed mappings
- FSM_API design and performance
- MicroBundles
- deterministic AI command boundaries
- ProtocolAI / Grammar / CommandAI
- host and rendering runtime boundaries
- rebuilding lost systems better than their originals
- the strange engineering lessons discovered along the way

The articles are not merely marketing.
They are the public engineering log of the Workshop becoming itself.

---

# The larger idea

We are not claiming to have reached the Singularity.

We're building the machinery that might make the journey interesting.

The destination remains recognizable even when the road changes.

And the road **will** change.

That's the point of a workshop.

---

*This way leads to the Singularity.*

*Built by The Singularity Workshop.*


## Opening experience: perception before explanation

The landing page is a **perception boundary**, not merely a signpost.

The current gateway now expresses its semantic structure through `WorkshopGatewayGuiBuilder`. The next evolution is an authored opening sequence that demonstrates the Workshop before asking the visitor to understand it intellectually.

The design principle is:

> **Show me why. Don't make me read why.**

The intended progression is:

`ARRIVAL → IDENTITY → TENSION → SHOW → INVITATION → WORKSHOP`

Visual intensity may be paired with original audio and timed presentation beats. This is presentation intensity, not autonomous persuasion: the visitor remains the decision-maker, and every demonstrated capability must correspond to real Workshop behavior or be clearly identified as experimental.

See `WORKSHOP_OPENING_EXPERIENCE.md` for the current contract.

## Public-facing product contract

The WebPage is now being prepared as the publishable public face of the Workshop.
The visitor should be able to do three things without reading the source code first:

1. **Experience it** — enter the living Workshop and see real behavior, not a mockup.
2. **Understand it** — follow the path from state and composition through MicroBundles, repositories, semantics, and host manifestations.
3. **Decide what it means to them** — understand what becomes easier, smaller, more reusable, or more interoperable by using these boundaries.

The public navigation therefore has a deliberate progression:

```text
FIRST CONTACT
    |
    +--> EXPLORE       experience the system
    +--> RENDERING     see the manifestation research
    +--> CREATE        inspect the composition vocabulary
    +--> EDUCATION     see a domain application
    +--> MADMEN        see another domain application
    +--> UNDERSTAND    learn the architecture and impact
```

The **Understand** surface is the canonical explanation of the ecosystem. It should
stay aligned with the actual repositories and contracts rather than becoming a second
marketing fiction.

Inventor and Revit are intentionally not part of the current public opening sequence.
They remain useful future examples of how external data can enter the ecosystem, but
they are not required to explain the architecture or prove the WebPage itself.

The quality bar is simple: **the public site must be able to demonstrate what it claims,
and the explanation must describe what the code actually does.**

## WebPage experience architecture

The WebPage is intentionally not a conventional marketing site. It is the **playable public entrance to The Singularity Workshop**.

The visitor path is:

```text
ARRIVE -> WITNESS -> ENTER -> INHABIT -> UNDERSTAND -> CREATE -> PUBLISH / SHARE
```

The four primary Workshop surfaces are **Understand**, **Experiences**, **Create**, and **Publish**. Rendering, Education, Madmen, laboratories, and domain-specific destinations are instruments or destinations within that larger model; they do not redefine the Workshop's public identity.

The design rule is:

> **Do not ask the visitor to believe the architecture. Give them something they can touch.**

The page should let people change state, enter environments, inspect composition, alter observer context, and see real consequences. Explanation follows evidence rather than replacing it.

See `WEBPAGE_EXPERIENCE_ARCHITECTURE.md` for the canonical public-experience contract.
