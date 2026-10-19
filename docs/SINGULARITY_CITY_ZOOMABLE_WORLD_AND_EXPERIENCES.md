# Singularity City: Zoomable World and Reusable Experiences

![Living GUI documentation lens: question, model, implementation, visible manifestation, and proof](https://raw.githubusercontent.com/TrentBest/WebPage/development/docs/assets/living-gui-documentation-lens.svg)

> **Evidence class:** architecture illustration, not live runtime telemetry. Follow the subject-specific source, view, fixture, or test below; if none exists, record that gap rather than implying it is implemented.


**Status:** design contract and implementation sequence. This document distinguishes existing foundations from work still required; it does not claim a persistent city, street-level renderer, simulated train, or living crime system already exists.

## Product thesis

Singularity City should recreate recognizable experiences in a composable digital world. It should be possible to understand the city from orbit, choose a district, navigate its streets, inspect an engineered building plan, enter a room, board a vehicle, walk through its passenger space while it travels, and step off at a destination. At every scale, the visitor is seeing the same world and the same entities—not a separate illustration made for each screen.

The visual baseline is top-down, legible, and Dragon Warrior-inspired. Engineering and planning surfaces may use precise plan views and schematics. Perspective and richer rendering can be added later without changing the authoritative world.

## One world, multiple scales

The zoomable map is a navigation surface over shared spatial data. It should progressively reveal detail instead of merely magnifying a low-detail drawing.

| Scale | What the visitor sees | What becomes actionable |
| --- | --- | --- |
| City overview | Island/city silhouette, coast, districts, major roads, rail, port, airport, transit interchanges, landmarks and broad traffic flows | Select a district, destination, route, or civic service |
| District | Street network, blocks, building footprints, parks, public spaces, transit stops and traffic corridors | Choose a street, building, stop, or nearby Digiten |
| Street | Top-down walkable paths, entrances, street furniture, vehicles, pedestrians and readable position markers | Walk, approach, inspect, talk, board, or enter |
| Building plan | Architectural footprint, walls, doors, stairs, circulation, rooms, utilities and floors where modeled | Navigate to a door or room; inspect dimensions and purpose |
| Room / venue | Focused top-down local map with interactable furnishings, actors and activity | Use the room's capabilities; leave to return to the parent scale |

The exact zoom thresholds should be tuned from real viewport behavior, not hard-coded as a claim of physical scale. Each object has stable identity and world coordinates; renderers choose a useful representation and level of detail. Entering a room changes the camera's focus and detail, not the room's identity or contents.

### Spatial and renderer responsibilities

- **World model:** stable IDs, world coordinates, coordinate frames, parent/child placement, footprints, entrances, walkability, route graph, ownership and semantic kind.
- **Navigation and camera:** zoom, pan, focus, transitions between city/district/street/building/room, and return paths to the parent context. Camera state must never become the source of truth for entity location.
- **Renderer:** converts world objects and observations into plan-view geometry, Digiten markers/cards, route overlays, vehicle schematics and room detail. Rendering must not own simulation rules.
- **LOD policy:** far away, render aggregates and landmarks; at district scale, show footprints and routes; near the visitor, show named entities, doors, interactions and useful animation. LOD changes representation and update cadence, never identity.
- **Composition:** facilities, vehicles, rooms, activities, and UI affordances should be reusable capabilities/content packages. A new Experience composes existing contracts instead of copying their implementation.
- **Evidence:** static concept art, deterministic presentation models, live simulation, and persistent world state must be labelled honestly as different levels of capability.

The current Workshop map and Singularity City experience already contain separate map/camera foundations. The city scope in the Workshop master-map is still labelled under construction, and the city experience remains a separate rendering path. The next integration should consolidate these views behind shared spatial contracts rather than pretend that the full zoomable map already exists.

## Vehicles are places you can inhabit

A train, flying taxi, bus, maglev, cargo hauler, or ship is both a vehicle and a small navigable environment. Vehicle identity, route, motion, passenger spaces and exterior appearance should be reusable, with model-specific capabilities composed around a common contract.

### Planning and schematic view

Each vehicle should first be legible in top-down plan view. The plan should communicate its engineered layout rather than just show a decorative sprite:

- vehicle outline and stable vehicle ID / type;
- front, rear, orientation and direction of travel;
- doors, seats, standing zones, aisles and accessibility spaces;
- driver/operator station and a visible Digiten operator identity;
- route/track alignment, boarding platform or pickup/drop-off point;
- relevant compartments and interaction points, only when the model provides them.

The schematic is derived from vehicle layout data. It is not an independently drawn map that can disagree with the passenger space.

### Journey as a continuous Experience

1. The visitor chooses a service and destination at a station or pickup point.
2. The vehicle's arrival/boarding state is visible, with operator and passenger Digiten cards.
3. Boarding moves the visitor into the vehicle's local coordinate frame.
4. While the service moves, the visitor can walk through permitted vehicle spaces. The interior camera follows the visitor while the exterior world and route continue moving.
5. Windows or an exterior cutaway can show the city passing by, with the current route and next stop available as overlays.
6. Arrival resolves to a real destination in the same world. The visitor may disembark, continue riding, or inspect the vehicle before leaving.

A shared service schedule should eventually coordinate multiple vehicles and passengers. The passenger journey, vehicle motion, station timetable, and destination arrival must agree through explicit state contracts. Do not simulate a train by teleporting a player at the end of a timer and call it physical transit.

### Model ownership

- WebPage demonstrates and presents the Experience.
- FSM_API coordinates stateful lifecycle and process cadence where it fits the existing public API.
- FSM_Physics owns physical motion, contact and vehicle dynamics when its integration contract is available.
- Gestures owns the vocabulary and evaluation of operator/passenger movement and choreography.
- Visualize/Renderer owns efficient representation, LOD and drawing; it must consume world and simulation state instead of inventing it.
- A vehicle's composable manifest describes capabilities, layout, presentation references and dependencies without making lower-level packages depend on WebPage.

The current Grand Central journey is a timed passenger-state lifecycle. The current aerial traffic layer is deterministic route presentation. Neither is yet a physical train path, shared dispatch schedule, or walkable moving vehicle. Preserve these distinctions in UI and documentation.

## Digitens are visible citizens, not anonymous dots

At city and district scale, Digitens may be lightweight positional markers to keep the map readable. As the visitor zooms in, markers should gain inspectable identity and purpose:

- name and stable Digiten identity;
- current location and activity / intent;
- destination or route when meaningful;
- occupation, affiliation, civic office, or role when applicable;
- interaction affordances based on capabilities the Digiten actually exposes.

Selecting a marker opens a compact data card and focuses the map on that actor. Cards must reflect shared actor state. A citizen's location, job, journey, and room occupancy must not be independently fabricated by each renderer. Do not display sensitive or overly detailed attributes merely because a Digiten has an identity.

## A city with government, heroes, villains, and an underbelly

A living city needs civic order and conflict, not a generic lawless backdrop. Existing source already defines the Mayor's office, City Council, clerk, treasury, planning, public works, housing, transit and digital services. The civic Digiten catalog names Mayor Mara and other officials, and an election simulator provides a starting point for player candidacy and voting. These are foundations, not proof of a durable, currently elected administration with an operating public ledger and enacted decisions.

The next civic slice should connect institutions to visible places and understandable actions: city hall / mayor's mansion, public records, council sessions, election boards, permits, public works projects, transit operations and a public budget. The mayor and officials should be Digitens in the same world, with identities, locations and schedules—not UI-only portraits.

The city should also support its full fictional spectrum: ordinary citizens, investigators, criminals, villains, heroes, super-heroes, factions, public safety, courts, prisons or rehabilitation where appropriate, and consequences for choices. The underbelly can include hidden venues, illicit markets, secret organizations and super-villain schemes; heroic activity can respond to events and protect people. Keep these as composable domains with explicit rules and interfaces. Do not turn the core city renderer into the owner of law, combat, character abilities, or faction logic.

A useful first implementation is not a giant crime simulation. It is a coherent set of reusable identities, places, event/state contracts, visible clues and interaction boundaries. A later hero, villain, civic, or role-playing MicroBundle can compose those capabilities into a distinct Experience without forking the city.

## Implementation sequence

1. **Make map scopes honest and connected.** The master map should enter the same city spatial model used by the city experience. Replace placeholder-only City scope behavior only when it can select real city entities and preserve location/context.
2. **Establish shared spatial coordinates and zoom transitions.** Define/test transforms between world, viewport, building and room frames; preserve stable IDs across all scales.
3. **Render the city at overview and district scales.** Use the existing city structure catalog as initial massing, and progressively add streets, blocks, route graphs, and named POIs. Distinguish planned footprints from implemented locations.
4. **Make actors inspectable.** Add stable Digiten markers and compact data cards bound to the same actor state used for movement and activity.
5. **Make one vehicle inhabitable.** Extend Grand Central from timed journey status to a route-following vehicle with an engineered plan, driver, seats/aisles/doors, passenger-local movement, and destination disembarkation. Keep the contract vehicle-neutral so flying taxi and other modes can reuse it.
6. **Connect service scheduling.** Coordinate vehicle availability, stop times, passenger journeys and route progress; add deterministic tests before multiplying vehicles.
7. **Bring the civic core to life.** Make the mayor, council, civic buildings, public decisions and public works visible and navigable; keep elections and officeholder persistence explicit.
8. **Compose the city's fun and danger.** Add hero/villain/faction/event contracts and a first end-to-end scenario that uses existing city locations and actor identities.
9. **Prepare the rendering scale-up.** Once spatial and LOD contracts are stable, hand Visualize/Renderer a concrete data contract and representative workload: city overview, dense district, street actors, vehicle schematics and room detail. Avoid a parallel renderer or premature GPU-specific assumptions.

## Acceptance criteria

- Zooming from city to room preserves entity identity, location relationships, and navigable return paths.
- Every visible Digiten/vehicle/building has stable identity; cards and markers describe the same underlying state.
- A moving vehicle's route progress, passenger journey, and arrival agree at tested timestamps.
- A passenger can move within an authorized vehicle-local space while the vehicle progresses through the world.
- The same vehicle layout and spatial contracts support at least rail and flying taxi without copied lifecycle logic.
- Civic institutions and future hero/villain Experiences compose with the city instead of owning private duplicate maps.
- Tests cover coordinate transforms, zoom bounds, LOD identity stability, route progress, boarding/disembarking and cancellation.
- Documentation states which behavior is a static plan, deterministic presentation, live simulation, or persistent state.

## Renderer collaboration boundary

The Visualize/Renderer collaborator is available. Coordinate through the explicit integration contract in [Singularity City: MicroBundle Composition Contract](SINGULARITY_CITY_MICROBUNDLE_COMPOSITION.md), this document, canonical spatial contracts, representative snapshots, expected LOD transitions, and tests. The goal is one renderer over shared world data—not a parallel world model, renderer framework, or GPU-specific design invented inside WebPage.
