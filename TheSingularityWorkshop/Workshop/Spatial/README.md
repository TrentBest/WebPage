# Spatial Composition

The Workshop spatial layer is a **composition system**, not a collection of bespoke scenes.

```text
Manifest
   |
   +-- Campus
   |    +-- Buildings
   |    +-- Laboratories
   |    +-- Transit
   |
   +-- Experience
   |    +-- Island
   |    +-- Maze
   |    +-- nested Experiences
   |
   +-- Universe
        +-- reserved Experiences
        +-- open parcels
        +-- infinite creation canvas
```

## Singularity Laboratory

The laboratory is a research tower whose floors represent fields rather than individual one-off rooms:

- Gravity & Spacetime
- Energy Systems
- Materials Science
- Atomics & Chemistry
- Quantum Systems
- Plasma & Field Studies
- Fluids & Hydraulics
- Cosmic Scale
- FSM Physics Simulation

Experiments are separate manifest data and can belong to multiple domains. This is intentional: an atom model, an FSM process group, and a physics rule should be reusable composition rather than duplicated scene code.

## Singularity Island

Island attractions are Experiences. The island itself also sits inside the larger `SpatialUniverseManifest`.

Open space is deliberate. A parcel is not an unfinished scene; it is an available composition boundary where a future authoring system can place a structure, game, simulation, or nested Experience.

## Transit

Grand Central is a destination router. Platform definitions identify destination scene IDs. Adding a destination should therefore be a manifest change first, not a new station renderer.

Current scaffolded destinations include:

- Air Terminal
- Water Terminal
- Rail Terminal
- Singularity Lab
- Space Elevator
- Singularity Island
- Singularity Station

## Design rule

Prefer:

**data -> composition -> generic renderer -> Experience**

over:

**new idea -> new hard-coded page**

The goal is that the Workshop eventually becomes able to author much of this graph without requiring source-code changes.


---

## 🔗 The Singularity Workshop

This project is part of a deliberately troublesome ecosystem:

- **[FSM_API](https://github.com/TrentBest/FSM_API)** — behavior and state.
- **[FSM_COS](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS)** — composition and runtime assembly.
- **[FSM_Serialization](https://github.com/TrentBest/TheSingularityWorkshop.FSM_Serialization)** — representation and the byte boundary.
- **[WebPage](https://github.com/TrentBest/WebPage)** — browser manifestation and proving ground.
- **[FSM_API_Unity](https://github.com/TrentBest/FSM_API_Unity)** — Unity manifestation.

<p align="center"><em>The Singularity Workshop — Tools for the curious, the bold, and the systemically inclined.</em><br><strong>Because state shouldn't be a mess.</strong><br><em>And because static boundaries are invitations to cause trouble.</em></p>
