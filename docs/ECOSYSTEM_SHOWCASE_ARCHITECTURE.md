# Ecosystem Showcase Architecture

## Purpose

WebPage is the browser host and living showcase for the Workshop ecosystem. Its job is to make independently owned capabilities discoverable, observable and usable—not to absorb their domain models.

This architecture is coordinated with:
- [Singularity Laboratories](https://github.com/TrentBest/TheSingularityWorkshop.SingularityLaboratories)
- [Singularity City](https://github.com/TrentBest/TheSingularityWorkshop.SingularityCity)
- [WebPage integration issue #137](https://github.com/TrentBest/WebPage/issues/137)
- [Laboratory extraction issue #7](https://github.com/TrentBest/TheSingularityWorkshop.SingularityLaboratories/issues/7)
- [City extraction issue #6](https://github.com/TrentBest/TheSingularityWorkshop.SingularityCity/issues/6)

## The three-repository boundary

### Singularity Laboratories — scientific capability

Owns portable laboratory and experiment semantics, facility/apparatus/instrument definitions, authoring/configuration, measurement semantics, experiment workflows, run provenance, replay, analysis, teaching and research.

A laboratory is a scientific capability and workspace. A building is one possible manifestation, not the laboratory itself.

### Singularity City — persistent spatial world

Owns City geography and spatial identity, districts, parcels, places, structures, entrances, routes, ownership, zoning, construction, residents/Digitens, institutions and civic state.

City resolves discoverability and place-to-experience entry points. It does not own the internal simulation of every destination.

### WebPage — browser host and proving ground

Owns browser routes/chrome, camera and input, visitor journey, browser-specific adapters, presentation composition, and integration/browser proofs.

WebPage consumes authoritative City and Laboratory contracts. It must not become the source of truth for scientific models, run provenance, City geography, or civic state.

## Neighboring ownership boundaries

| Concern | Owner |
|---|---|
| Physical models, numerical methods and terrain evolution | Physics |
| Experiment/campaign workflow lifecycle and cancellation | FSM_Execution |
| MicroBundle identity and contract | MicroBundleDomain |
| Composition, dependency closure and arbitration | FSM_COS |
| Geometry representation and visual output | Renderer / Visualize |
| State-machine substrate | FSM_API |
| Authentication, authorization enforcement, persistence and networking | Trusted host services |

## First showcase journey

The intended first cross-repository visitor path is:

```text
WebPage browser host
  → Singularity City place
    → City-owned entrance / spatial anchor
      → Laboratory-owned facility / experience entry
        → Laboratory-owned experiment definition
          → Physics-owned model
            → FSM_Execution run
              → immutable Laboratory run/result provenance
                → Renderer / Visualize manifestation
```

Ownership does not move along the visitor path. Each repository retains its own domain truth; WebPage coordinates the presentation and host handoffs.

The first scientific proof candidate is Physics' existing `ElectricalThermalCouplingExperiment`, wrapped by Laboratory contracts without duplicating or changing its physical semantics. City provides the place/anchor; Laboratory provides the experiment/facility; WebPage provides the browser journey.

## Existing WebPage material to inventory

- `Pages/Explore.razor`: browser camera/input, screen-to-world interaction and visitor movement. Keep browser presentation here; extract only canonical City semantics.
- `Workshop/Spatial/SpatialAECPlanExperience.cs` and its GUI builder: authored AEC plan and floor concepts that need explicit City/template/instance ownership decisions.
- `Workshop/Spatial/SpatialSingularityLabModel.cs`: current laboratory entrance/elevator visitor journey. It is a host-side spatial/presentation model, not the canonical laboratory experiment/facility model.
- `Workshop/Spatial/SpatialAECIntentModel.cs`: AEC intent interrogation and catalog selection; classify ontology/building semantics separately from browser workflow.
- City place, district, parcel, map, civic and terrain fixtures; laboratory facility, campaign, experiment and sourcebook prototypes; related tests and documentation.

Each item must be classified as canonical owner behavior, host presentation, adapter, transitional duplicate, fixture/illustration or unowned capability.

## Migration rules

1. Locate behavior and data before moving anything.
2. Preserve current behavior with tests.
3. Identify one canonical owner and versioned contract.
4. Implement or adapt the owner-side capability.
5. Prove WebPage consumes the owner-side contract.
6. Remove a duplicate only after the replacement is proven.
7. Keep documentation and work queues synchronized with implementation and CI evidence.

Coordinate frames, units, axis orientation, transforms, provenance, schema versions, identity mappings and authorization must be explicit. Never infer feet/meters, survey accuracy, physical telemetry, identity equivalence or access rights from presentation fixtures.

## Current evidence boundary

The Laboratory repository already has substantial versioned experiment/campaign contracts, Physics and FSM_Execution adapters, MicroBundle composition, immutable run records, replay, and result projections. Its browser campus and NIGHT SIGNAL pages are explicitly prototypes rather than live Physics execution or hosted facility telemetry.

The City repository already has place/blueprint data, spatial/civic models, layered maps, district/parcel generation and connected-district fixtures. Its README explicitly labels the current browser proof as a static navigation fixture rather than persistence, multiplayer or authoritative simulation.

WebPage's current AEC visitor animation is direct movement in a floor-local coordinate frame, not collision-aware routing. The room regions are semantic program regions, not authored walls. Preserve these distinctions until the corresponding topology and proofs exist.

## Verification blocker

WebPage workflow run [45345665643](https://github.com/TrentBest/WebPage/actions/runs/45345665643) failed in both standard NuGet and repository-source builds. The source branch of FSM_API now exposes `TheSingularityWorkshop.FSM_API.FsmApi`, while WebPage still calls the prior `FSM_API` static class. The standard package lane pins released FSM_API 1.0.13 and does not expose the newer `FsmApi` name. Establish a deliberate compatibility boundary and prove both modes before treating the AEC work or new integrations as verified.

No NuGet publication, no `master` changes, and no deletion of existing behavior are part of this work without explicit approval.
