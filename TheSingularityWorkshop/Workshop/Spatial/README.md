# Spatial Ontology Mall

The AEC interrogation does not own a fake list of answers.

It shops an inventory.

## Source of truth

The current building pipeline is:

```text
SpatialBuildingSpecificationCatalog
            |
            | canonical building program + physical constraints
            v
SpatialAECBuildingTypeFactory
            |
            | explicit ontology profile
            v
SpatialAECOntologyCatalog
            |
            | filtered inventory
            v
SpatialAECIntentModel
            |
            | selected ontology path
            v
SpatialBuildingGenerator / future GUI + BIM providers
```

This distinction is important.

A building specification contains information such as purpose, tier, footprint limits, floor limits, and occupancy. The nine-layer ontology contains semantic coordinates. The factory is the boundary where those two datasets are joined.

If an ontology value cannot be established from the source specification, the system should require an explicit ontology profile rather than inventing an answer.

## Shopping behavior

At each interrogation layer, the catalog filters the actual inventory using the answers already selected.

For example:

```text
REALITY
  |
  +-- BUILT ENVIRONMENT
        |
        +-- FACILITY
              |
              +-- LABORATORY
                    |
                    +-- RESEARCH FACILITY
                          |
                          +-- VERTICAL FACILITY
                                |
                                +-- ENGINEERING
                                      |
                                      +-- DEVELOPMENT
                                            |
                                            +-- RESEARCH LABORATORY
```

The final result is therefore an inventory item, not a sentence assembled from unrelated dropdown values.

## Fiction and scenarios

Fictional building concepts such as the high-tech research laboratory are retained as explicitly identified scenario inventory. They are not allowed to masquerade as canonical physical building specifications.

That lets the same interrogation machinery support:

- real AEC building types;
- authored fictional structures;
- future manufacturer/product catalogs;
- imported BIM libraries;
- user-authored building specifications;
- MicroBundle-provided inventories.

## Runtime

Human-readable catalog data belongs at the authoring boundary.

The runtime representation can flatten the selected values through `SpatialOntologyToken` into the nine-integer `OntologySignature`.

The renderer does not decide what a building is.

It receives the semantic result and decides how that result should be manifested.


## Code before geometry

A selected building is never treated as "fictional, therefore unconstrained."

The default experience attaches a SpatialAECCodeProfile before plan generation:

    Building Type
         |
         v
    Physical Specification
         |
         +----> Code Basis
         |       - occupancy
         |       - live load
         |       - floor-to-floor basis
         |       - structural system
         |       - life-safety notes
         |
         v
    Default Plan Experience

For a fictional building, the code profile is explicitly fictional but derived from a named real-world code family. The purpose is to preserve structural discipline and recognizable AEC constraints while allowing fictional technology to change the implementation.

This is a simulation/design basis, not a jurisdictional compliance or engineering certification system.

## Plan walking

After the user completes ontology interrogation, the selected inventory item becomes a SpatialAECPlanExperience.

The default experience contains:

- one or more semantic floor plans;
- rooms and functional zones;
- protected circulation;
- building systems;
- a pre-established code basis;
- interactables tied to actual plan coordinates.

The plan renderer shows an avatar inside the plan. Interactables breathe rather than appearing as ordinary UI controls. Approaching one reveals what that portion of the building can do.

The intended progression is:

    ONTOLOGY MALL
         |
         v
    SELECT BUILDING TYPE
         |
         v
    DEFAULT CODE BASIS
         |
         v
    DEFAULT PLAN
         |
         v
    WALK
         |
         +----> LIFE SAFETY
         +----> PROGRAM CAPABILITY
         +----> BUILDING SYSTEMS
         +----> ENTRY / ACCESS
         |
         v
    OPEN AUTHORING SURFACE

The plan is therefore not a drawing generated after the fact. It is the first navigable manifestation of the selected semantic building.

As the system grows, these interactables can become richer MicroBundle/provider boundaries: a laboratory door can expose its containment system, a mechanical room can expose its MEP systems, a structural bay can expose its load path, and a conference room can expose its actual program.

The GUI remains a manifestation of those capabilities rather than their source of truth.

## Singularity Laboratory

The Singularity Laboratory is the next active spatial vertical slice. The city, world, and solar-system layers are deliberately retained as future construction so the Workshop can first demonstrate a believable research facility and the machinery inside it.

The laboratory is a research tower whose floors represent fields rather than one-off rooms:

- Gravity & Spacetime
- Energy Systems
- Materials Science
- Atomics & Chemistry
- Quantum Systems
- Plasma & Field Studies
- Fluids & Hydraulics
- Cosmic Scale
- FSM Physics Simulation

The entrance is intentionally secure and diegetic. The current FSM_API-backed visitor progression is:

```
CAMPUS ARRIVAL
    |
    v
SECURITY LINE
    |
    v
BELONGINGS / SCREENING
    |
    v
SECURITY CLEARANCE
    |
    v
SECURE ELEVATOR
    |
    +--> ID CARD
    +--> FINGERPRINT
    +--> RETINAL SCAN
    |
    v
FLOOR SELECTION
    |
    +--> authorized floor -> research floor
    |
    +--> restricted floor -> access denied -> elevator
```

The implementation already has guards, reception, a director's office, conference space, screening, a secure vertical core, badge/identity concepts, floor authorization, research inventory, gravity bodies, 3D-rendering experiments, destruction sandboxes, robotics, research data, and restricted AI/security domains. These are architectural foundations, not permission to turn every future subsystem on at once.

The intended visual direction is a realistic science-fiction facility: security should feel like part of the building, not a modal; the visitor should understand where they are in the facility; the elevator should feel physically secure; and the research floors should become increasingly rich manifestations of actual data and experiments.


### Laboratory exterior, reusable security, and the first holodeck

The laboratory now begins as a physical exterior elevation rather than dropping the visitor directly into an interior plan.

The entrance has three distinct capabilities:

- a physical main door;
- a reusable, ontology-addressable card-reader MicroBundle boundary;
- a reusable voice intercom boundary.

The visitor is intentionally required to use the voice call box before the badge reader will accept the badge. Clearance is a property of the badge and reader, not a hard-coded laboratory permission. That means the same substrate can later be embedded in a starship, secure office, simulation game, or theft/caper scenario where the player must earn or steal a higher-clearance credential.

The entrance also exposes live facility traffic. Employees such as **Research #13** and **Administrator #2** have shifts, destinations, and clearance. Guards have posts and shifts covering security, the elevator, and the facility-head approach. This is the beginning of the laboratory behaving like a place inhabited by people rather than a static menu.

The laboratory's first holodeck experience is the existing authored maze. `SpatialHolodeckMazeModel` separates the maze data from its presentation and provides:

- 2D maze navigation as the initial mode;
- a toggle into a first-person presentation using the same maze state;
- a thirty-second player head start;
- a left-wall hunter;
- a right-wall hunter;
- capture and exit terminal states.

The hunters are deliberately data-driven wall-following agents rather than special-case animation. They can therefore become reusable pursuit MicroBundles later.

The intended progression is:

```text
LABORATORY EXTERIOR ELEVATION
        |
        +--> INTERCOM --> VOICE ACCESS
        |
        +--> CARD READER --> CLEARANCE CHECK
        |
        v
SECURITY / RECEPTION
        |
        +--> FACILITY HEAD
        |
        +--> SECURE ELEVATOR
        |       |
        |       +--> RESEARCH FLOORS
        |
        +--> HOLODECK
                |
                +--> 2D MAZE
                |
                +--> FIRST-PERSON MAZE
                |
                +--> 30 SECOND HEAD START
                |
                +--> LEFT-WALL HUNTER + RIGHT-WALL HUNTER
```

### Physics-first release discipline

The first useful laboratory floor is physics. FSMs, physics rules, measurements, reusable experiment data, image/mesh generation, and eventually 3D rendering can then become visible consequences of the laboratory's actual data model.

The city and solar-system presentations remain **UNDER CONSTRUCTION**. Their current maps are reference/blueprint manifestations only and are not the active release path.
