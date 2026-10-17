# Singularity City: MicroBundle Composition Contract

**Status:** architecture contract and integration plan. This document does not claim that the complete city is already a single composed runtime.

## The rule

**One WebPage host. One ecosystem composition boundary. Many manifest-selected Experiences.**

Singularity City is not a new standalone Blazor application, a second hub, or a collection of pages that independently own their worlds. It is a reusable Experience composed from MicroBundles and presented by the existing WebPage host.

The browser route is a presentation/navigation concern. It must not become the identity of the Experience, the source of world truth, or a substitute for MicroBundle composition.

## Canonical responsibility boundaries

| Concern | Owner |
|---|---|
| MicroBundle identity and canonical contract | MicroBundleDomain |
| Dependency closure, loading, arbitration, convergence and RuntimeAssembly | FSM_COS |
| Stateful lifecycle and process-group coordination | FSM_API, through supported package contracts |
| Spatial entities, coordinates, routes and world semantics | Owning world/domain contracts; not the renderer |
| Physical motion, contacts and dynamics | FSM_Physics, when its integration contract is available |
| Human/agent motion vocabulary and procedure evaluation | Gestures |
| Geometry manifestation, renderer backend, LOD and drawing | Visualize / Renderer |
| Browser host, manifest selection, visitor navigation and presentation adapters | WebPage |
| Remote artifact resolution/storage | Repository/provider boundary where configured |

Lower-level packages must not depend upward on WebPage. Renderer output must not become the authoritative location of a citizen, vehicle, building, seat, or route. WebPage must not recreate FSM_COS dependency traversal or create a parallel city simulation just to make a page appear alive.

## Experience composition

The existing WebPage host manifest already supports versioned MicroBundle roots for startup and running Experiences, manifest-defined hub entries, and supporting capabilities. The existing parser validates non-zero bundle IDs and non-empty versions. FSM_COS then resolves the actual dependency graph from the selected roots.

The intended composition is:

```text
WebPage host manifest + configuration
                  |
                  v
       Singularity City Experience
                  |
                  v
       declared MicroBundle roots
                  |
                  v
      FSM_COS dependency closure
                  |
                  v
      load / arbitrate / converge
                  |
                  v
            RuntimeAssembly
                  |
                  v
      WebPage manifestation adapter
                  |
                  v
       Visualize / Renderer output
```

The diagram is an architectural flow, not a claim that all city roots are registered and loaded today. Only add a root to the live host manifest when its exact ID/version is resolvable through the current composition catalog and tests prove it is composed. Do not add decorative manifest entries with fake IDs or treat a route URL as a MicroBundle.

## Compose capabilities, not pages

The eventual Singularity City Experience should compose domain capabilities such as:

- **World/spatial foundation:** stable places, coordinate frames, parent-child relationships, map scope transitions and navigation.
- **City content:** districts, streets, buildings, stations, port/harbor, freight yard, airport and public facilities.
- **Digiten population:** stable citizen identity, location, goal/activity and available interactions.
- **Transit service:** vehicle specification, route graph, stops, service lifecycle and passenger journey.
- **Vehicle layout / plan view:** renderer-neutral geometry for cab/operator station, cars/compartments, doors, seats, aisles and passenger-local coordinates.
- **Building plans and interiors:** rooms, walls, openings, fixtures, floor-to-world placement and navigable connections.
- **Civic systems:** public offices, government actions, public records, elections and services.
- **Optional domain experiences:** commerce, school, sports, heroes, villains, factions and underworld activity when their owning MicroBundles and rules exist.
- **Presentation capabilities:** plan-view rendering, city overview, semantic LOD, identity cards, route overlays and accessible interaction affordances.

This list is a capability vocabulary, not permission to invent MicroBundle IDs, claim a capability already exists, or create a giant city bundle that owns every domain. Each capability must be backed by an existing canonical contract or a reviewed, tested contract in the package that owns that domain.

## Current source-backed foundations

The WebPage repository already contains useful parts that should be reused rather than copied into standalone pages:

- `WebPageHostManifest` supports versioned bundle roots for startup and running Experiences, plus manifest-defined hub entries and capabilities.
- `WorkshopCompositionCatalog` and the existing FSM_COS tests demonstrate a real versioned root and canonical dependency composition for the current primary Experience.
- `TransitTrainMicroBundle` carries a renderer-neutral `TransitTrainSpecification`, eight-car example layout metadata, operating state and bounded route progress. This is not yet a complete walkable moving train.
- `PlanViewMicroBundleProvider` exposes a plan-view capability; `BuildingPlan` represents rooms, walls, openings and fixtures with stable local IDs and validated bounds.
- `SpatialDigitenManifest` describes roles, services, activities and routines; `SpatialDigiten` and the city-overview population supply current actor/presentation examples.
- `SpatialAirTrafficManifest` supplies deterministic aerial route presentation.
- The Workshop master map can show named city-overview Digiten markers, but its City scope is still explicitly under construction.

These foundations are not yet proof that all city systems are registered as FSM_COS roots or composed into one live RuntimeAssembly. The next step is to connect existing domain contracts and prove the manifest path, not to make the browser route pretend composition already happened.

## Required integration sequence

1. **Inventory canonical contracts.** For every proposed city capability, record owning repository, actual type/provider, stable bundle ID, version, dependencies, and current tests. Unknown values remain unknown until checked in source.
2. **Establish the shared spatial contract.** City, district, street, building, room, vehicle and renderer representations refer to stable entity IDs and explicit coordinate frames. Camera zoom is never world state.
3. **Make one Experience manifest-composable.** Register only real, versioned MicroBundle roots in the existing composition catalog. Declare roots in the existing WebPage host manifest; let FSM_COS resolve dependencies.
4. **Use one world snapshot across views.** The overview map, district view, vehicle plan, building plan and room view must consume the same identity/location model. Renderer-specific representations may differ; entities do not.
5. **Prove the vehicle vertical slice.** Select a transit vehicle from a manifest-backed city, render its plan from its layout specification, board it, move within vehicle-local space, advance route state, and disembark at a destination in the same world. Keep the existing timed journey clearly labelled until this slice is real.
6. **Prove the renderer seam.** WebPage supplies renderer-neutral entity/layout/snapshot data; Visualize/Renderer selects representations and LOD. Tests verify identity stability across LOD and scope changes.
7. **Expand by composition.** Add port/logistics, civic life, commerce, school, heroes/villains and other domains as separately owned capabilities, not branches of page-specific logic.
8. **Keep the proof honest.** Test composition roots and versions, identity continuity, coordinate transforms, route progression, lifecycle and LOD transitions. Clearly distinguish static sample data, deterministic presentation, live simulation and persistent state.

## Visualize / Renderer collaboration contract

The renderer work should be developed with the Visualize/Renderer collaborator against the actual repositories and current interfaces. WebPage must not create a parallel rendering framework or dictate GPU backend choices prematurely.

The first shared fixture set should cover:

1. city overview with districts, roads, rail, port, intermodal station and named Digiten samples;
2. dense district with streets, building footprints, vehicles and actor identity cards;
3. an engineered train plan derived from the real `TransitTrainSpecification` / vehicle-layout contract;
4. a building plan derived from `BuildingPlan` (rooms, walls, doors/windows, fixtures);
5. a room view whose actor and object IDs match the parent building/world;
6. near/far snapshots to verify semantic LOD and stable identity across representation changes.

For each fixture, agree on the data source, coordinate frame, expected visible semantics, update cadence, LOD transition, and tests. Renderer consumes the snapshot; it does not invent actor schedules, vehicle physics, civic outcomes or route truth. GPU/native/browser specifics belong in Visualize/Renderer implementation decisions, with measured evidence rather than assumptions.

## Acceptance criteria

- Singularity City is selected as an Experience through the existing manifest/composition flow, not launched as an independent Blazor application.
- Every live MicroBundle root has a verified ID/version and a test proving it resolves through the current composition catalog.
- FSM_COS remains the dependency-closure and arbitration authority.
- City, vehicle and building/room views share stable IDs and explicit coordinate relationships.
- The train plan, passenger space and exterior vehicle representation derive from one layout specification.
- A vehicle journey agrees with route progress and passenger-local position at tested simulation times.
- Renderer LOD changes appearance/cadence without changing world identity.
- Adding a new city domain does not require duplicating the hub, world map, camera, or renderer.


## Active renderer collaboration handoffs

The implementation briefs are tracked as repository documents so the sibling agents can respond without editing one another's code:

- [RendererLLM](integrations/RendererLLM.md) — stable-world identity across six horizons, reproducible stress ladder, actual source-mode Renderer integration, and measured performance.
- [VisualizeLLM](integrations/VisualizeLLM.md) — deterministic dense-city visualization fixture, geometry-to-backend lifecycle, LOD invariants, and honest GPU instrumentation.

Each receiving agent should return its implementation evidence in a `WebPageLLM.md` file in its own repository. The proposed workload sizes are test steps, not capacity claims. No renderer or GPU capability should be described as integrated until the corresponding source path and tests prove it.
