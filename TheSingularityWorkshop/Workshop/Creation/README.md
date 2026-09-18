# Workshop Creation

Creation is the boundary between **creative intent** and a durable, executable artifact.

The Workshop is not primarily an application for editing one particular kind of file. It is a creative environment whose stations help a person turn an idea into something they can **define, compose, run, save, share, and eventually sell**.

The core rule is:

> **Persist the definition of a thing, not the runtime object that happens to manifest it.**

That rule lets one creation travel between Web, WPF, Unity, VR, and future hosts without rebuilding the user's work for every platform.

## The final creative constructs

The Workshop should converge on a small number of durable constructs rather than a different bespoke editor for every feature.

| Construct | Purpose | Example |
|---|---|---|
| **Resource** | Atomic creative material | image, audio, model, font, data |
| **GUI** | Recursive presentation definition | panel containing buttons, text, images |
| **FSM** | Recursive behavior definition | menu states, game states, workflow states |
| **Code** | Explicit programmable logic | C#, script, generated source |
| **Experience** | Composed environment/application | a game, tool, interactive scene |
| **Package** | Distributable/sellable product | versioned Experience + dependencies |

These are deliberately different things.

A GUI is not code. An FSM is not a GUI. An image is not an Experience.

But they can be composed:

```text
Resource
   |
   +---- GUI --------+
   |                 |
   +---- FSM --------+----> MicroBundle ----> Experience ----> Package
   |                 |
   +---- Code -------+
```

The **MicroBundle** remains the composition boundary. It is where focused capabilities become portable pieces. An Experience is then an environment assembled from those pieces.

## What the user does in the Workshop

The spatial Workshop should expose these constructs as physical/visual stations.

```text
                         WORKSHOP
                            |
        +-------------------+-------------------+
        |                   |                   |
     ART LAB            GUI LAB            FSM FORGE
        |                   |                   |
     Resources             GUI               Behavior
        |                   |                   |
        +-------------------+-------------------+
                            |
                       CODE LAB
                            |
                         Logic
                            |
                            v
                      CREATION BAY
                            |
                    Compose MicroBundles
                            |
                            v
                       EXPERIENCE
                            |
                            v
                        PACKAGE
```

The user should be able to wander into a station because they have an **intent**, not because they already understand our internal architecture.

For example:

- "I want to make this screen." → GUI.
- "I want this button to do something." → FSM.
- "I need an image for it." → Art/Resource.
- "I need custom computation." → Code.
- "I want these things to become a game/tool." → Experience.
- "I want somebody else to use or buy it." → Package.

The Workshop's job is to help discover and express that intent.

## GUI is CRUD over a definition

The GUI station should ultimately provide:

```text
GUI Library
   |
   +-- CREATE
   +-- READ / PREVIEW
   +-- UPDATE
   +-- DELETE
   |
   +-- DUPLICATE
   +-- COMPOSE
   +-- EXPORT
```

This is the important distinction from a conventional GUI designer.

We are not primarily editing HTML or CSS.

We are editing a **recursive GUI definition**.

The existing `GuiNode` is already the correct intermediate representation. `GuiBuilder` creates it, `WebGuiBuilder` manifests it, and other platform builders can consume the same semantic tree.

The browser should therefore be able to:

1. select a named GUI from the lightweight library/index;
2. rehydrate its definition;
3. inspect the recursive tree;
4. select any node;
5. edit its properties;
6. add/remove/reparent children;
7. see the live manifestation;
8. save the resulting definition as binary.

That is the first real GUI editor.

## FSM follows the same rule

The FSM Forge should eventually CRUD over an FSM definition just as the GUI station CRUDs over a GUI definition.

The persisted object is the graph:

```text
FSM Definition
   |
   +-- states
   +-- initial state
   +-- transitions
   +-- symbolic conditions
   +-- metadata
```

Executable delegates belong to the runtime/compiler boundary, not the persistence format.

That gives us a clean path:

```text
Definition
    |
    v
Binary Artifact
    |
    v
Rehydrate
    |
    +----> GUI renderer
    |
    +----> FSM_API compiler
    |
    +----> future platform/runtime
```

## What we scavenge from SingularityWarehouse

The old Warehouse contains several useful ideas that belong here, but they should be adapted rather than copied wholesale.

### 1. Lightweight manifests

`WarehouseManifest` separates a cheap registry/index from heavy payloads.

The Workshop should do the same:

```text
Library Index
    |
    | cheap: name / id / version / kind / modified
    v
Artifact
    |
    | heavy: complete binary definition
    v
Rehydrated Editor Model
```

The user should not have to load every creation merely to browse the library.

### 2. CRUD as a first-class interaction

`CRUDGuiBuilderWPF` demonstrates the useful idea of exposing collection management as a recursive editor rather than forcing every collection into bespoke UI.

Our Web implementation should take the stronger semantic approach:

**CRUD operates on Workshop definitions, not arbitrary runtime objects.**

### 3. Reflective/deep inspection

`ReflectiveGuiBuilder` contains the useful notion of an expandable "portal" into a complex object.

That maps directly to our recursive GUI/FSM editors:

```text
Root
 |
 +-- Panel
 |    |
 |    +-- Button
 |    +-- Text
 |
 +-- Image
```

Selecting a node opens its editable context without abandoning the parent definition.

### 4. Cache and staleness

Warehouse's local cache/manifest model gives us the right vocabulary for a future server-backed Workshop:

```text
local definition
      |
   version/hash
      |
remote manifest
      |
   compare
   /     \
fresh   dirty
         |
      synchronize
```

This matters when creations eventually become cloud-backed products.

### 5. Hot replacement belongs at the runtime boundary

Warehouse's hot-swap thinking is valuable, but the Workshop should not make compiled assemblies the primary authoring format.

The durable source of truth remains the definition.

A future runtime may replace a compiled manifestation while the user's artifact remains intact.

## The monetization boundary

This is where the architecture becomes commercially interesting.

The Workshop does not need to monetize every click. It needs to make the **thing the user creates** durable and valuable.

The progression is:

```text
Intent
  |
  v
Definition
  |
  v
Composition
  |
  v
Executable Experience
  |
  v
Versioned Package
  |
  +---- private
  +---- shared
  +---- published
  +---- licensed
```

That means monetization should eventually attach to **artifacts and capabilities**:

- creation/storage capacity;
- premium tooling;
- collaboration;
- publishing;
- deployment;
- distribution;
- reusable MicroBundles;
- marketplace/licensing.

Those are product concerns around the creation system, not reasons to contaminate the core definition model.

## What "code" means

Code is one of the constructs, but it should not swallow the Workshop.

There are three useful forms:

1. **Generated code** — the Workshop derives code from a declarative definition.
2. **User-authored code** — the creator explicitly writes programmable logic.
3. **Runtime code** — compiled/loaded manifestation used by an execution host.

The definition remains authoritative wherever possible.

That is especially important for GUI and FSM: a creator should be able to manipulate the structure without first becoming a programmer.

## The resulting architecture

The Workshop is therefore a **creation operating environment**:

```text
                         USER INTENT
                              |
                              v
                     +----------------+
                     |    WORKSHOP    |
                     +----------------+
                              |
             +----------------+----------------+
             |                |                |
          Resource           GUI              FSM
             |                |                |
             +----------------+----------------+
                              |
                            Code
                              |
                              v
                         MicroBundle
                              |
                              v
                         Experience
                              |
                              v
                           Package
                              |
                              v
                    Run / Share / Publish
```

The editor, binary codec, storage, manifest, renderer, and runtime compiler are **tools around these constructs**.

They are not the constructs themselves.

## Immediate implementation order

The architecture is now clear enough to stop inventing infrastructure and start making the Workshop usable:

1. **Creation Bay / Library** — browse, create, rename, duplicate, delete named artifacts.
2. **GUI Editor** — recursive tree + property editing + live Web preview.
3. **FSM Editor** — states/transitions + preview.
4. **Artifact Inspector** — definition metadata, version, binary size/hash, dependencies.
5. **Run** — compile the selected definition and execute it.
6. **MicroBundle composition** — turn authored definitions into focused capabilities.
7. **Experience assembly** — compose capabilities into an environment.
8. **Package/publish boundary** — version and distribute the resulting creation.

The next implementation should therefore be the **Creation Bay itself**. It is the place where the abstract architecture becomes visible to a person walking around the Workshop.

And importantly: **the Creation Bay should itself be built with the recursive GUI-builder architecture.**

That gives us the recursive proof we actually care about:

```text
Workshop
  -> Creation Bay
      -> Library
          -> GUI Editor
              -> GUI being edited
                  -> GUI nodes
```

The Workshop is using the same machinery it is teaching the user to use.
