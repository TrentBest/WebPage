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
