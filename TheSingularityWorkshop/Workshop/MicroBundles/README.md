# MicroBundles

MicroBundles are the small, independently schedulable units of Workshop behavior.
They are intentionally **not** UI components.

The lifecycle belongs to `FSM_API` through `IStateContext`. A MicroBundle carries
its state and delegates its visible manifestation to an `IMicroBundleProvider`.

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

## Why this matters

The Workshop ultimately needs to operate across different manifestation domains.
A bundle should therefore describe **what is happening**, while a provider decides
**how that happening is expressed** in WPF, Blazor, Unity, WebGL, TTS, or another
host.

The first web provider intentionally returns semantic effects such as `Trace`,
`Breathe`, and `Collapse`. It does not know anything about CSS.

## Intended evolution

The next layer is integer-backed ontology metadata. Human-readable names remain
useful during authoring, but transport and hot-path lookup should resolve strings
to integer identities. That allows compact AI commands such as:

```text
[AvailableTools=3]
[Inspect=37]
```

and ultimately a dense integer stream validated sequentially against the active
grammar.

MicroBundles are the first concrete place in the WebPage project where that
architecture can be exercised rather than merely described.
