# Experience and GUI Boundary

## The rule

**Experience owns context. GUI owns presentation structure.**

A GUI definition is reusable semantic data. It does not know whether it is being shown in the Workshop, a Creation Bay, a web page, WPF, Unity, or VR.

An Experience decides where and why that GUI is presented.

```text
                    EXPERIENCE
                         |
              +----------+----------+
              |          |          |
           Surface    Surface    Surface
              |          |          |
             GUI        GUI        GUI
              |          |          |
          Renderer    Renderer    Renderer
              |          |          |
            Web/WPF   Unity/VR   future host
```

The first concrete boundary is:

- ExperienceDefinition — the environment/context.
- ExperienceSurfaceDefinition — a named presentation context.
- GuiNode — the recursive presentation definition.
- renderer — platform manifestation.
- FsmBlueprint — declarative behavior associated with a surface.
- runtime — compiled/living manifestation.

## What belongs where

```text
Experience
  |
  +-- spatial context
  +-- interactables
  +-- surfaces
  |     |
  |     +-- GUI definition
  |     +-- optional FSM definition
  |
  +-- Resources
  +-- MicroBundles
  +-- construction/progression state
  +-- execution boundary
```

The GUI must not absorb these responsibilities.

Conversely, the Experience should not reproduce GUI trees merely to place a button.

This gives us reuse:

```text
                 GUI Definition
                       |
          +------------+------------+
          |            |            |
     Creation Bay   Movie Lot    User App
          |            |            |
      Workshop       Experience   Experience
```

## A surface is the handshake

A surface is the point where an Experience says:

> Present this GUI here, and associate this behavior with it.

For example:

```text
Movie Lot Experience
  |
  +-- Stage Control Surface
  |       +-- GUI: stage-controls
  |       +-- FSM: stage-runtime
  |
  +-- Casting Surface
  |       +-- GUI: casting-board
  |       +-- FSM: casting-workflow
  |
  +-- Wardrobe Surface
          +-- GUI: wardrobe-browser
          +-- FSM: wardrobe-workflow
```

The same stage-controls GUI could later be presented somewhere else without changing its definition.

## Why this scales beyond games

The Experience is not a game container.

It is an **environment for digital creation and interaction**.

A book-making Experience could expose:

```text
Book Experience
  |
  +-- timeline
  +-- characters
  +-- locations
  +-- scenes
  +-- manuscript
  +-- visualization surface
  +-- AI intent surface
```

The visualization surface can present a GUI. The scene itself can be spatial. The underlying story can have an FSM or other executable model.

The same architecture therefore accommodates games, books, films, music, tools, simulations, education, and other digitizable creations.

## AI belongs between intent and definition

The deterministic artifact should remain authoritative.

```text
Human intent
     |
     v
AI interpretation
     |
     v
Candidate definitions
     |
     +---- deterministic validation
     |
     v
User approval / editing
     |
     v
Durable artifact
```

GrammarAi / ProtocolAi-style tooling can help convert ambiguous human intent into explicit structures. It should not silently replace the user's authoritative definition.

## The Workshop itself

The Workshop is the first large Experience we should exercise this architecture against.

It should be spatially substantial: studios, production facilities, labs, construction areas, circulation, equipment, and future specialist spaces.

The scale can be informed by real-world facilities and human factors. Zoom provides the abstraction layer that lets a user understand the campus from above without walking every high-detail distance.

Core facilities include:

- Artist / Art Studio
- Sound / Recording Studio
- Movie Lot
- Stage / Animation
- Casting
- Wardrobe
- GUI Lab
- FSM Forge
- Code Lab
- Writing / Book Studio
- Creation Bay

These are not merely menu destinations. They are places in the Experience.

## Construction is an Experience state

A facility can exist before it is operational:

```text
PLANNED -> UNDER CONSTRUCTION -> OPERATIONAL
```

While under construction, the site remains useful.

A fenced site can present an elevation, foreman, workers, equipment, signage, safety explanation, and a description of the completed capability.

Construction machinery can be an interactable sub-experience.

For a crane:

```text
SITE
 |
 +-- approach interaction point
       |
       v
   crane elevation
       |
       v
   climb ladder
       |
       v
    crane cab
       |
       v
 bird's-eye operation
       |
       v
 pick -> raise -> rotate -> position -> lower
       |
       v
 holographic placement target
       |
       v
 construction metric
```

The metric can later contribute to Workshop-wide progress and public leaderboards.

This remains a composition of Experience, GUI, FSM, Resources, and MicroBundles. It is not a special game subsystem.

## Architectural consequence

The next generation of the Workshop should be built **from the same grammar it exposes**:

```text
Workshop Experience
  |
  +-- Creation Bay
       |
       +-- Library
            |
            +-- GUI Editor
                 |
                 +-- GUI definition
                      |
                      +-- recursive GUI nodes
```

The editor is not outside the architecture. It is another Experience surface using the same GUI machinery.

That is the recursive proof.

## Construction crew implementation direction

The unfinished Workshop should be treated as a set of construction work packages rather than a pile of missing features.

Each package can eventually carry:

- facility identity;
- intended capability;
- construction state;
- required Resources;
- required MicroBundles;
- preview/elevation;
- explanatory GUI;
- construction interactables;
- completion criteria;
- completion metric.

That gives us a clean path to have a **construction crew** work through the Workshop while users continue exploring and understanding the intent of facilities that are not yet complete.

The first software boundary to stabilize is Experience ↔ GUI. Once that is sound, the spatial construction layer can consume it rather than inventing another presentation system.
