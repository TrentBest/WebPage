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

## Verification checkpoint

The earlier source-vs-package FSM_API compatibility blocker is resolved on `development`. The standard NuGet build and the repository-source integration both pass, including the browser journey:

- [Build and source-integration proof](https://github.com/TrentBest/WebPage/actions/runs/45558427853) on [`957fec2`](https://github.com/TrentBest/WebPage/commit/957fec260575c973480d6d54f9dcc5dbb71f0e8d).
- [Follow-up .NET 10 smoke-check proof](https://github.com/TrentBest/WebPage/actions/runs/45570618182) on [`02a1c66`](https://github.com/TrentBest/WebPage/commit/02a1c66aed51fc42c8e40ba24cf59025d11ab09f).

The browser smoke check now verifies the .NET 10 WebAssembly bootstrap script at `/_framework/blazor.webassembly.js`; it does not expect the removed `blazor.boot.json` manifest. The README now reflects the .NET 10 project target, SDK requirement and bootstrap asset, and links to the successful browser-proof run.

No NuGet publication, no `master` changes, and no deletion of existing behavior are part of this work without explicit approval.


## Cross-repository contract checkpoint — 25 November 2026

The first City and Laboratory vertical slices now have executable evidence on their owner/consumer branches:

- **Singularity City** added JSON Schemas for the existing `singularity-city.catalog.v1` place catalog and the proposed `singularity-city.experience-entry.v1` handoff declaration, plus deterministic contract tests. The Underworks entry fixture resolves to an existing place and district but is not registered as a live destination. City Pages build/deployment and CiAi exact-commit evidence passed: [Pages run](https://github.com/TrentBest/TheSingularityWorkshop.SingularityCity/actions/runs/45700149617) · [CiAi evidence](https://github.com/TrentBest/TheSingularityWorkshop.SingularityCity/actions/runs/45700148709).
- **WebPage** now has a source-mode `LaboratoryCampusOverviewGuiBuilder` that consumes Laboratory's `LaboratoryCampusOverview` and projects owner-defined campus/workspace/facility/room/apparatus/instrument/measurement-channel metadata into the host GUI tree. Its focused test verifies identity, labels, units and sampling metadata.
- **WebPage CI** checks out Laboratory `development` and compiles/tests the adapter through the repository-source lane. The standard package lane remains unchanged. [Both CI jobs passed](https://github.com/TrentBest/WebPage/actions/runs/45701016711): standard browser-host tests and Workshop repository-source integration.

These are contract and presentation proofs, not a live hosted lab, live instrument telemetry, trusted authorization, Physics coordinate mapping or runtime City-to-Laboratory handoff. City map coordinates remain presentation coordinates until a complete spatial-frame contract exists. WebPage's legacy spatial laboratory models remain host-side presentation/visitor prototypes; they are not promoted to canonical scientific or security models.

Next slice: consume Laboratory-owned experiment definitions/configurations and execution/provenance contracts in a WebPage experience adapter, then connect City place/entry declarations to that experience through an explicit host handoff. Preserve existing visitor behavior and tests until replacements are proven.
