# MicroBundles

MicroBundles are a current **architectural experiment** in The Singularity Workshop.
They are intentionally small, independently schedulable units of behavior.

They are **not UI components**.

That distinction matters because the Workshop is trying to separate what a piece of
software *is doing* from how a particular host chooses to *show it*.

The current lifecycle model uses `FSM_API` through `IStateContext`. A MicroBundle
carries state/lifecycle information and delegates visible manifestation to an
`IMicroBundleProvider`.

```text
MicroBundle
    |
    +-- IStateContext
    |      +-- Phase
    |      +-- ParentId
    |      +-- Generation
    |
    +-- FSM_API lifecycle
    |
    +-- IMicroBundleProvider
           |
           +-- Manifestation
```

## Why this exists

The larger Workshop needs to operate across different manifestation domains.
A behavior should therefore be able to describe **what is happening** without
assuming that the answer is HTML, CSS, WPF, Unity, WebGL, TTS, or another host.

For example, a semantic effect such as:

```text
Trace
Breathe
Collapse
```

should not require the MicroBundle itself to understand CSS animation. The provider
owns that translation.

This is the same general separation the Workshop is pursuing elsewhere:

```text
SEMANTICS
   |
   v
STATE / IDENTITY
   |
   v
MANIFESTATION
```

## Relationship to the living landing page

The Workshop landing experience is deliberately becoming a test case for this idea.

A GUI control can begin as an ordinary interface element and become a living
behavior:

```text
Seed
  |
  v
Grow
  |
  v
Live
  |
  v
Reproduce
  |
  +----> child begins its own lifecycle
  |
  v
Critical Mass
  |
  v
Freeze
  |
  v
Collapse / Gravity
```

The current WebPage implementation is still experimental. Do not assume that the
landing page's timer or rendering code represents the final MicroBundle architecture.
The behavior is the important artifact; the implementation is still being forged.

## Integer-backed identity direction

Another Workshop thread is moving human-readable identity toward compact integer
identity for runtime and AI-facing transport.

Strings remain useful — especially for people, authoring, debugging, and
communication — but they do not have to be the representation carried through
every hot path.

The intended direction is approximately:

```text
Human-readable concept
        |
        v
String identity / documentation
        |
        v
Integer identity / mapping
        |
        v
Deterministic command / runtime state
```

The exact representation is still evolving. This README intentionally describes
the direction rather than pretending the future API is already final.

## AI / command boundary

MicroBundles are also relevant to the Workshop's AI work because a deterministic
runtime needs a deterministic boundary.

The broader experimental stack currently looks conceptually like:

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
FSM_API / deterministic execution
 |
 v
MicroBundle / provider / manifestation
```

Names and boundaries may change as the work evolves. The durable problem is how to
let probabilistic intelligence interact with deterministic machinery without
allowing ambiguity to leak into execution.

## Development guideposts

### Preserve the separation

Do not casually put host-specific rendering logic into the semantic bundle.

### Preserve lifecycle clarity

A MicroBundle should have a clear state/lifecycle story. If a behavior is becoming
a collection of unrelated timer callbacks, stop and reconsider the boundary.

### Preserve the experiment

The current implementation is allowed to be temporary. The behavior and lessons
learned from it are not disposable.

### Document changes in direction

The Workshop's destination is comparatively stable while the road is deliberately
fluid. Update this document when the architectural model changes, rather than
freezing temporary implementation details into doctrine.

---

*The bundle is small. The idea is not.*

*This way leads to the Singularity.*
