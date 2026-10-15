# Singularity City — Aerial Traffic Model

**Status:** deterministic route model, GUI snapshot renderer, and city-map host loop are implemented. The loop advances snapshots once per second while the City map is open; it is a presentation-time prototype, not yet an FSM_API-owned simulation clock.

## Purpose

Singularity City needs visible aerial traffic that can be inspected and reproduced in tests. This model provides a renderer-independent contract for declared flying vehicles and route-position snapshots, without introducing a second simulation engine into WebPage.

## Current contract

`SpatialAirTrafficManifest.CreateDefault()` declares four stable vehicle identities. Each route records:

- vehicle and model identity;
- named corridor;
- normalized city-plan origin and destination;
- discrete altitude band;
- travel duration and phase offset.

`SnapshotAt(elapsedSeconds)` computes a deterministic out-and-back position for every route. It is a pure query over declared route data and elapsed time: it does not own a clock, mutate world state, or start background work.

The city GUI builder now accepts optional `SpatialAirTrafficSnapshot` values and renders each supplied vehicle with stable identity, model, corridor/lane, altitude band, and route-progress metadata. It does not manufacture positions or animate vehicles on its own.

## Boundaries and limitations

This is a first route/presentation reference, not a complete aerial-traffic simulation:

- altitude bands are discrete presentation lanes, not physical heights;
- routes do not yet avoid collisions, negotiate intersections, or react to buildings;
- dispatch, passenger pickup/drop-off, and taxi ride lifecycle are not implemented here;
- the Explore host currently derives elapsed time from a component-local UTC epoch and refreshes snapshots once per second only while the City map is open; this proves visible route progression, not authoritative simulation time, persistence, or FSM_API tick scheduling;
- no claim is made that the current branch has passed CI until a workflow reports success.

A host may connect snapshots to the city renderer and real transport state. Keep aerial lanes separate from ground transit, and do not describe route interpolation as autonomous intelligence or physical flight dynamics.

## Acceptance checks

- Vehicle IDs are stable and unique.
- Route snapshots at the same elapsed time are identical.
- Snapshots change predictably as elapsed time advances.
- Invalid time and invalid route parameters are rejected.
- The manifest remains unchanged when snapshots are evaluated.
- Presentation consumes this model rather than inventing a parallel stream of moving icons.
- GUI rendering exposes the supplied snapshot identity and coordinates without taking ownership of the simulation clock.
- the host starts the update loop only for the open City map and cancels it when the map closes, scope changes, or Explore is disposed.
