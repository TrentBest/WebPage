# Singularity City: Living World Plan

![Living GUI documentation lens: question, model, implementation, visible manifestation, and proof](https://raw.githubusercontent.com/TrentBest/WebPage/development/docs/assets/living-gui-documentation-lens.svg)

> **Evidence class:** architecture illustration, not live runtime telemetry. Follow the subject-specific source, view, fixture, or test below; if none exists, record that gap rather than implying it is implemented.


**Status:** implementation plan grounded in current WebPage work; not a claim that all listed systems are already running together.

Singularity City is a science-fiction city that should feel inhabited, connected, and in motion—not a skyline render with decorative dots. The Workshop is an anchor inside a larger world. Digitens have their own lives and destinations; transportation connects neighborhoods; public places create reasons to gather; and the same world should support both everyday activity and special events such as a soccer match.

## The city has several kinds of life

The shared-world direction and scale hierarchy are documented in [Singularity City: Zoomable World and Reusable Experiences](SINGULARITY_CITY_ZOOMABLE_WORLD_AND_EXPERIENCES.md), while [Singularity City: MicroBundle Composition Contract](SINGULARITY_CITY_MICROBUNDLE_COMPOSITION.md) defines how those capabilities must be composed through the existing host manifest and FSM_COS rather than split into standalone pages. The Workshop master map now also renders named city-overview Digiten markers with their sample goal descriptions. Their dedicated stable identities are distinct from civic-office identities; they are inspectable presentation samples, not yet a persistent or citywide citizen schedule.

- **Digitens:** persistent digital citizens with identities, places, goals, and movement. A visible avatar is not the same thing as durable citizen/world state.
- **Ground circulation:** streets, walkable public space, station approaches, plazas, and paths. Crowd occupancy must respect blocked cells and prevent two actors from claiming the same cell.
- **Air circulation:** abundant flying cars and hover vehicles, with recognizable routes, altitude bands, pickup/drop-off points, and landing or charging areas. Air traffic is its own transport layer; it should not be drawn as if cars were driving on streets.
- **Mass transit:** rail and intermodal connections move many citizens between districts. Stations and destinations should be addressable places in the world.
- **Personal and service vehicles:** taxis, shuttles, cargo and construction vehicles give the city visible movement at different scales.
- **Civic destinations:** homes, markets, schools, hospitals, research facilities, parks, terminals, arenas, and the Workshop should each have a purpose beyond their silhouette.

## Build on the work that already exists

The current work/digiten-world-slice branch contains useful foundations:

- SingularityCityCatalog defines stable city structure identities and normalized footprints.
- SpatialTransitManifest describes transit destinations, platforms, passengers, levels, and timing.
- SpatialTaxiManifest defines vehicle classes, destinations, passengers, and ride rules.
- DigitenCityTexture provides a CPU-side occupancy surface with blocked cells and unique actor occupancy; DigitenCrowdField provides a deterministic local movement heuristic.
- SingularityCityGuiBuilder renders the explorable city and visible Digitens.
- CityLab.razor is a separate deterministic isometric massing view. It is a planning/rendering prototype, not the live city simulation itself.

These are related layers, not yet proof of one fully integrated city simulation. The next work should connect them through explicit contracts and tests rather than replace them with a parallel demo implementation.

## The arena is a place in the city

The soccer venue should be a stable city destination with a reserved footprint and visible access. It needs a pitch, goals, boundary markings, player/team areas, spectator seating, concourses, entrances, and transit/drop-off connections. A large event should visibly affect the city around it: Digitens arrive, gather, enter the stands, watch, play, and disperse afterward.

The soccer experience must use the actual Soccer MicroBundle contract when available. The city owns the venue and its access; the Soccer MicroBundle owns the soccer domain behavior it truly implements; Gestures supplies the allowed action vocabulary for the field and player roles; FSM_COS/manifest composition connects the capabilities; WebPage presents observable state. A rendered field or animated ball alone is not evidence of a composed match.

## A practical implementation sequence

1. **Make the world legible:** retain stable district and place IDs, and give the soccer arena its own explicit city identity and footprint.
2. **Make citizens move:** use the occupancy/movement substrate for grounded pedestrians, with destinations and crowd density driving visible activity. Avoid rendering thousands of untracked decorative people.
3. **Give the sky its own traffic:** define flying-vehicle identity, 3D/altitude lane, origin, destination, and route state separately from ground movement. Show many vehicles at city scale, but keep motion deterministic and bounded for reproducible tests.
4. **Connect transport to places:** route citizens through transit stations, taxi/drop-off points, and arena entrances. A trip should be inspectable as a journey, not just a teleport effect.
5. **Stage the event:** populate the arena with spectators and players from real city/experience state; show arrival, match, and departure states only when the underlying contracts support them.
6. **Compose soccer:** verify the canonical Soccer MicroBundle identity, version, declared dependencies, and available behavior. Connect the field-scoped Gestures vocabulary without inventing package IDs or rules.
7. **Prove it:** test stable place identities, movement occupancy, deterministic vehicle routing, arena access, bundle composition, and match-state transitions. Keep static visualization, simulated state, and live composed behavior clearly distinguished.

## Visual direction

Think layered, dense, and busy: elevated lanes threaded between towers; hover cars flowing at several heights; ground-level sidewalks and transit concourses; Digitens moving with purpose; lit homes and workplaces; plazas with people stopping to talk; and a bright arena whose crowd creates a visible pulse in the surrounding district.

The city should communicate scale through coordinated activity, not just a large building count. At a distance, the viewer sees a living metropolis; at district scale, transport flows and gathering places; at street scale, citizens and vehicles have inspectable identities and destinations.

## Evidence boundary

This plan records the intended direction. It does **not** assert that flying-car traffic, persistent citizen schedules, spectator simulation, or the Soccer MicroBundle match are already integrated. Each becomes a shipped capability only after its implementation and tests exist.

## First aerial-traffic model slice

The city now has two explicit aerial traffic profiles:

- `CreateDefault()` retains the compact four-vehicle fixture for focused tests and small scenes.
- `CreateCityTraffic()` extends that fixture to twelve stable vehicle identities for the city map, with varied vehicle models, routes, corridor IDs, discrete altitude bands, travel durations, and phase offsets.

`SnapshotAt(elapsedSeconds)` derives repeatable positions without starting a clock or mutating the manifest. The Explore host now selects the denser city profile and advances its snapshots on the existing one-second city-map loop. The renderer places these vehicles in the aerial layer, distinct from the ground-level citizen markers.

This makes the sky visibly busier in the current city-map experience, but it remains a bounded presentation model—not a full flight simulation. Altitude bands are discrete presentation lanes rather than physical heights; routes do not yet negotiate intersections, avoid collisions, respond to buildings, dispatch for passengers, or connect to the taxi passenger lifecycle.

A presentation-independent journey model now defines deterministic, caller-clocked stages: `Waiting → Boarding → InTransit → Arrived → Completed`. It records stable passenger, origin-platform, and destination identities and derives snapshots from explicit elapsed time; it does not start a timer or mutate the journey. Tests cover each transition, invalid timing, repeatability, and the distinction between arrival and completion.

Grand Central now consumes those snapshots during boarding: selecting a destination and walking to its platform starts a clocked journey, and a live station panel exposes the passenger, origin platform, destination, stage, and progress. The host advances the model on a 250 ms presentation loop; after the arrival dwell it resolves drop-off into the destination scene. Leaving the station cancels the journey clock. This is a deterministic presentation lifecycle driven by wall-clock elapsed time, not yet a persisted schedule or a physically simulated train path.

Ground crowd occupancy, station operations, and aerial routing remain distinct systems and should be integrated through explicit contracts rather than duplicated in the page. The train's visual path and shared multi-passenger service schedule remain future work.
