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

A GUI is not code. An FSM is not a GUI. An image is not an Experience.

But they can be composed:

    Resource
       |
       +---- GUI --------+
       |                 |
       +---- FSM --------+----> MicroBundle ----> Experience ----> Package
       |                 |
       +---- Code -------+

The **MicroBundle** remains the composition boundary. It is where focused capabilities become portable pieces. An Experience is then an environment assembled from those pieces.

## The Workshop is a virtual equivalent of reality

The deeper requirement is larger than "provide an editor."

> **Give the creator the virtual equivalent of whatever they would need in reality.**

If someone wants to make a movie, the Workshop should not hand them a generic media-upload form. It should provide the functional equivalent of a production campus:

    MOVIE LOT
       |
       +-- CASTING
       +-- ACTORS / PUPPETS
       +-- SCRIPT
       +-- STAGE
       +-- WARDROBE
       +-- CAMERA
       +-- LIGHTING
       +-- SPECIAL EFFECTS
       +-- SOUND / FOLEY
       +-- MUSIC
       +-- EDITING
       +-- REVIEW

For a fairy-godmother scene:

    SCENE
       |
       +-- Fairy Godmother
       |     +-- puppet / actor
       |     +-- dialogue
       |     +-- arrival action
       |
       +-- VISUAL EFFECT
       |     +-- sparkle
       |     +-- transformation
       |
       +-- AUDIO
             +-- arrival SFX
             +-- music transition
             +-- dialogue
             +-- ambience

These are composable Experiences operating on durable definitions, not isolated editors.

## Singularity Performing Arts Center

The Workshop campus should include a **Singularity Performing Arts Center** as a major facility.

It should provide virtual equivalents of:

- orchestration and composition;
- rehearsal and performance;
- instrument and ensemble arrangement;
- sheet music and notation;
- recording;
- mixing and mastering;
- sound design;
- Foley and prop recording;
- voice and dialogue;
- playback and review.

The center should expose music as something that can be **composed**, not merely uploaded.

A creator should be able to place notes onto a sheet, assign instruments or sections, arrange who plays what, establish timing, press play, hear the result, and continue editing.

The durable composition can then become a Resource or be attached directly to an Experience.

See Creation/Audio/README.md for the initial audio-domain architecture.

## SFX / Foley Studio

A dedicated **SFX / Foley Studio** belongs beside the Performing Arts Center.

It should provide a virtual prop wall, recording area, layering tools, synthesis/generation surfaces, timeline, preview, and recipe storage.

A creator might enter with:

> "I need the sound of a fairy godmother arriving."

The Workshop can let them build that sound from components:

    FAIRY ARRIVAL
       |
       +-- bell sparkle
       +-- rising shimmer
       +-- soft impact
       +-- voice cue
       +-- music transition

The sources might be recorded from physical-style props, selected from the library, synthesized, generated, or combined from previous creations.

This is why SoundEffectRecipe exists as a definition rather than treating every sound as one opaque file.

## Audio is part of Experience composition

Audio needs to participate in the same deterministic timeline as other Experience behavior.

    EXPERIENCE TIMELINE
       |
       +-- actor enters
       +-- camera changes
       +-- visual effect begins
       +-- SFX begins
       +-- music changes
       +-- dialogue begins
       +-- effect ends

The Workshop can eventually make the whole sequence directly editable.

AI may propose an arrangement, sound, instrumentation, or transition from natural-language intent. The deterministic definition remains authoritative after the creator accepts or edits the proposal.

    Human intent
         |
         v
    AI candidate
         |
         v
    deterministic definition
         |
         v
    creator approval / editing
         |
         v
    durable artifact
         |
         v
    runtime manifestation

## What the user does in the Workshop

The spatial Workshop should expose these constructs as physical/visual stations.

    WORKSHOP
       |
       +-- ART LAB
       +-- GUI LAB
       +-- FSM FORGE
       +-- CODE LAB
       +-- PERFORMING ARTS CENTER
       |     +-- COMPOSITION
       |     +-- RECORDING
       |     +-- SFX / FOLEY
       |     +-- VOICE
       |     +-- MIXING
       +-- MOVIE LOT
       |     +-- CASTING
       |     +-- STAGE
       |     +-- EFFECTS
       |     +-- EDITING
       +-- CREATION BAY
             |
             +-- MicroBundles
             +-- Experiences
             +-- Packages

The user should be able to wander into a station because they have an **intent**, not because they already understand our internal architecture.

Examples:

- "I want to make this screen." -> GUI.
- "I want this button to do something." -> FSM.
- "I need an image for it." -> Art/Resource.
- "I need a musical score." -> Performing Arts / Composition.
- "I need a new sound." -> SFX / Foley.
- "I want actors and a scene." -> Movie Lot.
- "I need custom computation." -> Code.
- "I want these things to become a game/tool/film/interactive world." -> Experience.
- "I want somebody else to use or buy it." -> Package.

The Workshop's job is to help discover and express that intent.

## GUI is CRUD over a definition

The GUI station should ultimately provide:

    GUI Library
       |
       +-- CREATE
       +-- READ / PREVIEW
       +-- UPDATE
       +-- DELETE
       +-- DUPLICATE
       +-- COMPOSE
       +-- EXPORT

We are not primarily editing HTML or CSS.

We are editing a **recursive GUI definition**.

The existing GuiNode is the intermediate representation. GuiBuilder creates it, WebGuiBuilder manifests it, and other platform builders can consume the same semantic tree.

The browser should therefore be able to select a named GUI, rehydrate its definition, inspect the recursive tree, edit properties, add/remove/reparent children, see the live manifestation, and save the resulting definition as binary.

## FSM follows the same rule

The FSM Forge should eventually CRUD over an FSM definition just as the GUI station CRUDs over a GUI definition.

The persisted object is the graph:

    FSM Definition
       |
       +-- states
       +-- initial state
       +-- transitions
       +-- symbolic conditions
       +-- metadata

Executable delegates belong to the runtime/compiler boundary, not the persistence format.

    Definition
        |
        v
    Binary Artifact
        |
        v
    Rehydrate
        |
        +----> GUI renderer
        +----> FSM_API compiler
        +----> future platform/runtime

## The creation campus should scale like a real place

The Workshop is a large creative campus, not a small game map.

Zoom becomes the compression mechanism:

    HIGH ZOOM
       -> room
       -> workstation
       -> control
       -> individual artifact

    LOW ZOOM
       -> studio
       -> building
       -> campus
       -> district

This lets the Workshop represent substantial facilities without forcing every interaction into one screen.

Construction can also be part of the Experience. A future facility can be fenced off, staffed by a foreman and workers, show active equipment, and expose an elevation or holographic preview of what is being built.

A crane can itself become an Experience: approach it, climb into the cab, obtain a site-wide view, pick a load, move it, align it with a ghost placement, and receive deterministic feedback. The result can contribute to the evolving state of the Workshop.

## What we scavenge from SingularityWarehouse

The old Warehouse contains several useful ideas that belong here, but they should be adapted rather than copied wholesale.

### 1. Lightweight manifests

A manifest separates a cheap registry/index from heavy payloads.

    Library Index
       |
       | cheap: name / id / version / kind / modified
       v
    Artifact
       |
       | heavy: complete binary definition
       v
    Rehydrated Editor Model

### 2. CRUD as a first-class interaction

CRUD should operate on Workshop definitions, not arbitrary runtime objects.

### 3. Reflective/deep inspection

A recursive editor should provide an expandable portal into complex definitions. Selecting a node opens its editable context without abandoning the parent definition.

### 4. Cache and staleness

A future server-backed Workshop needs local definition version/hash comparison against remote manifests so creations can synchronize safely.

### 5. Hot replacement belongs at the runtime boundary

The durable source of truth remains the definition. A future runtime may replace a compiled manifestation while the user's artifact remains intact.

## The resulting architecture

The Workshop is therefore a **creation operating environment**:

    USER INTENT
         |
         v
      WORKSHOP
         |
    +----+----+----+----+----------------+
    |    |    |    |    |                |
 Resource GUI  FSM Code Performing Arts  Movie
    |    |    |    |    |                |
    +----+----+----+----+----------------+
                         |
                       Audio
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

The editor, binary codec, storage, manifest, renderer, and runtime compiler are **tools around these constructs**.

They are not the constructs themselves.

## Immediate implementation order

The architecture is now clear enough to make the Workshop usable:

1. **Creation Bay / Library** - browse, create, rename, duplicate, delete named artifacts.
2. **GUI Editor** - recursive tree + property editing + live Web preview.
3. **FSM Editor** - states/transitions + preview.
4. **Audio composition surfaces** - score, arrangement, SFX/Foley, voice, recording, and timeline definitions.
5. **Artifact Inspector** - definition metadata, version, binary size/hash, dependencies.
6. **Run** - compile the selected definition and execute it.
7. **MicroBundle composition** - turn authored definitions into focused capabilities.
8. **Experience assembly** - compose capabilities into an environment.
9. **Package/publish boundary** - version and distribute the resulting creation.

The Creation Bay should itself be built with the recursive GUI-builder architecture.

That gives us the recursive proof we actually care about:

    Workshop
      -> Creation Bay
          -> Library
              -> GUI Editor
                  -> GUI being edited
                      -> GUI nodes

The same recursive machinery should eventually build the editors inside the Performing Arts Center and Movie Lot. The Workshop is using the same machinery it is teaching the user to use.
