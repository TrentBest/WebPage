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
