# Singularity City — Aerial Traffic Model

**Status:** first deterministic route-model slice; not yet wired into the city GUI.

## Purpose

Singularity City needs visible aerial traffic that can be inspected and reproduced in tests. This model provides a small renderer-independent contract for declared flying vehicles and route-position snapshots, without introducing a second simulation engine into WebPage.

## Current contract

`SpatialAirTrafficManifest.CreateDefault()` declares four stable vehicle identities. Each route records:

- vehicle and model identity;
- named corridor;
- normalized city-plan origin and destination;
- discrete altitude band;
- travel duration and phase offset.

`SnapshotAt(elapsedSeconds)` computes a deterministic out-and-back position for every route. It is a pure query over declared route data and elapsed time: it does not own a clock, mutate world state, or start background work.

## Boundaries and limitations

This is a first route/presentation reference, not a complete aerial-traffic simulation:

- altitude bands are discrete presentation lanes, not physical heights;
- routes do not yet avoid collisions, negotiate intersections, or react to buildings;
- dispatch, passenger pickup/drop-off, and taxi ride lifecycle are not implemented here;
- the city GUI does not yet consume these snapshots, so adding the model alone does not make vehicles visible;
- no claim is made that the current branch has passed CI until a workflow reports success.

A host may later connect the snapshots to the existing city renderer and real transport state. Keep aerial lanes separate from ground transit, and do not describe route interpolation as autonomous intelligence or physical flight dynamics.

## Acceptance checks

- Vehicle IDs are stable and unique.
- Route snapshots at the same elapsed time are identical.
- Snapshots change predictably as elapsed time advances.
- Invalid time and invalid route parameters are rejected.
- The manifest remains unchanged when snapshots are evaluated.
- Presentation consumes this model rather than inventing a parallel stream of moving icons.
