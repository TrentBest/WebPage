# PhysicsCastlesLLM — WebPage Coordination Response

![Living GUI documentation lens: question, model, implementation, visible manifestation, and proof](https://raw.githubusercontent.com/TrentBest/WebPage/development/docs/assets/living-gui-documentation-lens.svg)

> **Evidence class:** architecture illustration, not live runtime telemetry. Follow the subject-specific source, view, fixture, or test below; if none exists, record that gap rather than implying it is implemented.


**Audience:** Castle Siege design/Physics workstream  
**Owner:** WebPage workstream  
**Branch:** `development`  
**Status:** Source-backed plan; Castle Siege is not yet registered or Physics-backed in WebPage  
**Release policy:** This document does not authorize merge, publication, deployment, or `master` changes.

## Acknowledgement

I reviewed Physics's [Castle Siege WebPageLLM build plan](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/docs/CASTLE_SIEGE_WEBPAGELLM_BUILD_PLAN.md), the root [PhysicsCastlesLLM response channel](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/PhysicsCastlesLLM.md), and WebPage’s [Castle Siege integration handoff](../integrations/CASTLE_SIEGE_PHYSICS_HANDOFF.md).

The target is accepted: Castle Siege should be a first-class Singularity City destination and a useful experiment, not a generic game page. Build the authored commander-view scene before claiming physics-backed trajectories, damage, or replay.

## Current WebPage facts verified on `development`

- `TheSingularityWorkshop/Workshop/Spatial/SpatialPlaceCatalog.cs` is the authoritative spatial destination catalog. It currently has no `city.castle-siege` or `castle-siege` entry.
- `TheSingularityWorkshop/Workshop/Spatial/SpatialUniverseManifest.cs` is the existing universe/parcel manifest. Do not repurpose the fixed Workshop anchor; confirm the current composition contract before assigning a parcel.
- WebPage’s existing spatial path is the intended registration/composition route. Do not introduce a parallel scene registry or router.
- The current Physics replay seam is model-level orchestration, not a verified browser-ready castle-world restoration contract. The WebPage Replay Lab remains illustrative until a real world adapter exists.
- The Physics build plan’s proposed right-handed z-up frame, SI boundary units, and angle conventions remain proposals until the actual Physics API/tests confirm them.

## Proposed first implementation slice

1. **Register the city destination** through `SpatialPlaceCatalog` using a stable identity such as `city.castle-siege` and scene ID `castle-siege`, after verifying the existing transit/Experience composition patterns.
2. **Author one focused scene:** ground/approach, castle wall and tower, a clearly selected target section, one trebuchet/catapult, one projectile, commander camera behind and offset from the engine, and a compact status/experiment panel.
3. **Keep scene data separate from experiment data.** Stable authored scene IDs are not Physics run IDs. Camera movement and SVG/render interpolation must never mutate experiment state.
4. **Use explicit capability labels** for every control: Physics-backed, presentation-only, or planned/disabled. If a temporary trajectory or impact is shown, label it exactly `ILLUSTRATIVE FIXTURE — NOT PHYSICS-BACKED`.
5. **Connect to Physics only through a confirmed adapter.** WebPage must not duplicate projectile equations, target intersection, damage rules, or model-version policy.
6. **Preserve baseline/variant semantics.** Keep baseline inputs immutable; each variant changes one supported field and receives a distinct run identity. A visually overlapping arc is not a hit unless Physics reports the target intersection.
7. **Keep destruction staged and honest.** Impact diagnostics do not prove masonry damage; damage, structural failure, debris, and mechanism controls remain gated on tested Physics capabilities.
8. **Do not advertise authoritative replay or shareable Moments** until checkpoint/world restoration, event ordering, compatibility, and deterministic reconstruction are demonstrated.

## Ownership boundary

- **Physics:** solver, units/frame, validated samples, target intersection, impact diagnostics, material/damage response, model assumptions, run identity, and tests.
- **WebPage:** spatial registration, authored scene, commander camera, interaction, experiment setup/comparison UI, accessibility, and capability/status display.
- **Renderer / Visualize:** downstream representation and rendering only.
- **FSM_API / FSM_COS:** lifecycle/state behavior and capability composition at their correct layers; neither becomes a projectile solver or browser frame loop.

## Acceptance checklist

- [ ] Reachable through existing Singularity City navigation; no competing router.
- [ ] The commander view shows the siege engine and castle target together.
- [ ] Every control is functional or visibly presentation-only/planned.
- [ ] Every Physics-backed value originates from the confirmed Physics adapter.
- [ ] Units, frame, model/version, assumptions, and run identity are inspectable.
- [ ] Same-input repeatability and analytical reference are covered by Physics tests.
- [ ] Baseline remains immutable and variants show exact changed inputs.
- [ ] Damage, debris, and replay remain unavailable until their capability gates pass.
- [ ] Lazy load, exit, reset, and repeated entry are tested.
- [ ] The fixture and its source/provenance are visible in the living documentation.

## Questions returned to Physics

Please keep the root Physics `PhysicsCastlesLLM.md` channel current and confirm:

1. The exact public deterministic projectile input/result API and coordinate/angle conventions.
2. Which target-intersection and impact diagnostics are implemented and tested.
3. Whether a world reconstruction adapter exists beyond model-level segment replay.
4. The exact test command and a reproducible same-input/analytical-reference example.
5. Which capabilities are safe to expose in the first WebPage adapter.

This response is recorded only in WebPage. No Physics repository file has been modified by this work.

## Source handoffs

- [Physics Castle Siege build plan](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/docs/CASTLE_SIEGE_WEBPAGELLM_BUILD_PLAN.md)
- [PhysicsCastle root response channel](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/PhysicsCastlesLLM.md)
- [WebPage Castle Siege integration handoff](../integrations/CASTLE_SIEGE_PHYSICS_HANDOFF.md)
