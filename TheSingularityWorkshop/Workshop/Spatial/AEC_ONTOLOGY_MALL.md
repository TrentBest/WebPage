# AEC Ontology Mall

The AEC consultation is an ontology-shopping workflow.

It does not invent a list of choices independently at each layer. Instead, the visitor is walking through a catalog of concrete building-type records. Each answer narrows the inventory that remains compatible with the selected ontology path.

```text
                    AEC ONTOLOGY MALL
                           |
                           v
                 concrete catalog inventory
                           |
             +-------------+-------------+
             |                           |
          browse                       select
             |                           |
             v                           v
      compatible records        ontology path becomes
             |                  more specific
             +-------------+-------------+
                           |
                           v
                 concrete building type
                           |
                           v
                integer runtime signature
                           |
                           v
                 spatial manifestation
```

## The important distinction

The catalog is the data.

The consultation UI is a manifestation of that data.

`SpatialAECIntentModel` asks the catalog what is actually available for the current path. `SpatialAECConsultationGuiBuilder` renders those results. It should not become a second catalog.

This is the first deliberate step away from pseudo-data: values are no longer merely a list of choices with no relationship to a concrete building record.

The flow is:

```text
BuildingType records
        |
        +-- Reality / Fiction
        +-- Domain
        +-- Kingdom
        +-- Phylum
        +-- Class
        +-- Order
        +-- Family
        +-- Genus
        +-- Species
        |
        v
filter compatible inventory
        |
        v
show choices that actually exist
```

## Runtime boundary

Authoring data remains readable because people need to inspect and edit it.

Before runtime composition, the nine ontology values can be flattened into `OntologySignature` integer coordinates. The catalog therefore provides both a human-readable authored record and a deterministic integer runtime coordinate.

The renderer does not own either identity.

## What this is not yet

This catalog is a concrete reference inventory authored inside WebPage. It is **not** being presented as an authoritative copy of an external AEC classification standard.

The next safe expansion is to introduce importers/adapters for authoritative classification datasets such as building-type/classification systems, while keeping the catalog contract stable. That lets external data become inventory without coupling the consultation UI to a particular source.

## Safety rule for the domain

Do not make geometry the first source of truth.

A building type should first exist as semantic inventory. Geometry, rooms, systems, equipment, elevations, BIM objects, Revit representations, Unity representations, and WebPage manifestations can then be selected or generated downstream from the semantic record.

That is the core reason for the mall metaphor:

> We are shopping the ontology before we draw the building.
