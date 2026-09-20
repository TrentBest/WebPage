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
