# Castle Siege Experience: Physics-to-WebPage Integration Handoff

![Living GUI documentation lens: question, model, implementation, visible manifestation, and proof](https://raw.githubusercontent.com/TrentBest/WebPage/development/docs/assets/living-gui-documentation-lens.svg)

> **Evidence class:** architecture illustration, not live runtime telemetry. Follow the subject-specific source, view, fixture, or test below; if none exists, record that gap rather than implying it is implemented.


## Purpose

Build a physics-led castle siege experience in WebPage, beginning with a readable, grounded scene: **the commander stands just behind and to one side of a catapult, looking toward a castle**. The castle is the target and experimental apparatus—not merely a static backdrop for a game animation.

Physics work is tracked in [TheSingularityWorkshop.Physics issue #23: deterministic projectile experiment](https://github.com/TrentBest/TheSingularityWorkshop.Physics/issues/23). This document defines the WebPage-side responsibilities and the contract expectations. It does not duplicate or pre-empt the Physics implementation.

## Experience sequence

1. **Establish the view.** Place the camera at the commander's shoulder, just behind and offset to the side of the catapult. Keep the launch apparatus, projectile path, and castle target legible in the same composition. Do not begin with a detached chart or a generic fly-through.
2. **Inspect the apparatus.** Identify the catapult/trebuchet, projectile, ground, castle wall/target region, and relevant geometry. Clearly label any simplified or illustrative mechanism.
3. **Configure an experiment.** Expose only parameters supported by the active Physics model, such as launch position, initial speed, elevation, azimuth, projectile mass (where the model uses it), gravity, and timestep. Display units and coordinate conventions. Do not imply that mass affects the trajectory if the selected model has no mass-dependent forces.
4. **Run and observe.** The trajectory, sampled positions/velocities, target intersection, and impact readout must derive from the same Physics run record. Camera motion and visual interpolation must not alter the simulation.
5. **Compare experiments.** Preserve a baseline run; let the user change one supported parameter and rerun, then compare trajectory and impact results. Clearly identify changed values and keep the remaining conditions fixed.
6. **Inspect the outcome.** Report whether the projectile intersects the declared target region, with impact position and velocity when available. Do not manufacture an impact simply because the projectile visually appears to touch the wall.

## Ownership and dependency boundaries

- **TheSingularityWorkshop.Physics:** authoritative equations, state evolution, units/conventions, deterministic samples, target-region intersection, impact observations, model assumptions, validation, and tests.
- **WebPage / Singularity Labs experience:** scene composition, commander's camera, user controls, experiment setup UI, baseline/variant selection, charts and readouts, explanation, and presentation of Physics observations.
- **FSM_API / FSM_COS:** coordinate process/state composition at the appropriate integration layer. Do not turn FSM_COS into a projectile solver or a game loop.
- **Visualize / Renderer / GUI.SVG:** consume scene/observation data to render it. They must not become the authority for projectile motion or impact decisions.

WebPage must not independently reimplement projectile equations to create a plausible-looking path. The model output is authoritative; the visual path is a view of that output. Keep the Physics core free of WebPage, Blazor, Unity, and renderer dependencies.

## Initial model versus later mechanism fidelity

The current Physics issue's initial scope is a deterministic, renderer-independent projectile model with explicit SI units, validated initial conditions, position/velocity samples, target intersection, and a comparison against an analytical constant-acceleration reference. The initial model explicitly omits air drag, wind, spin, arm inertia, sling/counterweight mechanics, masonry fracture, and general rigid-body contact unless later work implements and validates them.

Therefore:

- Treat launch speed as a declared input/proxy in the initial slice. Do not claim that draw weight, arm length, counterweight, sling release, tension, or stored potential energy physically determine that speed until a validated mechanism model exposes those outputs.
- Present castle impact as a target-intersection/impact observation first. Do not imply realistic wall damage, cracks, rubble, or structural collapse until Physics has a defined and tested material/contact/damage model.
- If a later mechanism model becomes available, let its validated geometry, energy/tension state, arm motion, and release condition drive the launch state. Do not keep a second, conflicting UI-side launch calculation.
- Keep visual effects, debris, dust, and camera shake downstream of actual impact observations, and label effects as presentation when they are not physically simulated.

## Validation and trust

- Show the model's supported inputs, coordinate convention, units, assumptions, and omitted effects before the user relies on the result.
- Make repeatability and a baseline-versus-variant comparison easy to inspect.
- Validate a simple reference trajectory against the analytical constant-acceleration solution and expose the declared tolerance through Physics documentation/tests.
- Keep simulated samples, derived visual interpolation, explanatory diagrams, and any future measured data distinguishable.
- Reject invalid inputs and surface out-of-domain conditions rather than quietly clamping values into a seemingly valid result.
- Record model/configuration version and experiment parameters with each run so a result can be reproduced.

## Integration gate

The experience may proceed with scene layout, camera, controls design, and data-adapter scaffolding, but do not label the scene as a working Physics simulation until a usable Physics API and validated observations are available. The adapter should consume the agreed Physics contract rather than assume unmerged names or invent a temporary parallel model.

This is a handoff/design document, not evidence that the scene or integration is already implemented. No merge, package release, or deployment is implied.


## Cross-LLM design review: requirements accepted

The Physics-side design and WebPageLLM response in [PhysicsCastlesLLM.md](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/PhysicsCastlesLLM.md) have been reviewed. This section records how those findings translate into the WebPage workstream so the castle design remains an active input to implementation.

### Existing WebPage extension points

- Register the experience through `TheSingularityWorkshop/Workshop/Spatial/SpatialPlaceCatalog.cs`, using a stable city destination such as `city.castle-siege` and scene ID `castle-siege`, if it is presented as a city attraction. The catalog already distinguishes `SpatialDomain.SingularityCity` from `SpatialDomain.Workshop` and resolves destinations by scene ID.
- Follow the existing spatial composition/Experience and transit-manifest patterns for navigation. Do not add a competing scene registry or route system.
- `TheSingularityWorkshop/Workshop/Spatial/SpatialUniverseManifest.cs` already declares open parcels and a reserved island core. Do not repurpose the fixed Workshop anchor; only claim/assign an Experience parcel if the actual composition path requires it.
- Keep the first scene focused: one castle, one trebuchet, one projectile, one target wall section, and the commander camera behind and offset from the engine. Scene visuals and camera can be authored before the numerical adapter is available.

### Implementation gates carried forward

1. **Experience scaffold:** route/lazy-load through the current spatial architecture; show the commander view, authored siege engine, castle silhouette, target marker, and compact control/measurement layout. Any temporary arc or impact output must say `ILLUSTRATIVE FIXTURE — NOT PHYSICS-BACKED`.
2. **Physics contract alignment:** agree the canonical frame and stored angle units with Physics implementation before freezing types. The current proposal is right-handed, z-up; azimuth in the x-y plane; elevation measured from that plane; SI units at the boundary. Treat this as proposed until implementation and tests confirm it.
3. **Real trajectory and hit/miss:** consume Physics-owned run records and samples. Use the solver's target intersection, never visual overlap. Keep the baseline immutable and give every variant its own run identity and exact changed parameter.
4. **Impact diagnostics:** display impact point/velocity, momentum and kinetic energy only when returned or safely derived from the authoritative run record. These measurements explain the strike but do not prove masonry damage.
5. **Bounded wall response:** add damage presentation only after Physics supplies a documented, tested wall/material response and transition reason. Keep structural failure, topology/debris, and visual effects as distinct later steps.
6. **Mechanism and history:** do not represent pull-back/tension/counterweight as physically controlling launch until a tested mechanism model maps those inputs to launch state. Do not advertise authoritative replay or shareable Moments until world/checkpoint reconstruction is implemented and tested.

### Ownership and next coordination checkpoint

Physics owns equations, run metadata, trajectory samples, target intersection, impact/material rules, assumptions and tests. WebPage owns spatial registration, scene/camera, supported controls, comparison UI and explanatory presentation. FSM_API/FSM_COS remain composition/scheduling boundaries, and Visualize/Renderer consume observations downstream. Gestures may later consume declared crew/impact events; they must not become a Physics dependency.

The next cross-workstream checkpoint is the Physics implementation LLM's report of existing constant-acceleration, run-record and test extension points, followed by a tested projectile milestone under [Physics issue #23](https://github.com/TrentBest/TheSingularityWorkshop.Physics/issues/23). WebPage can advance the authored scene scaffold in parallel, but must not freeze an adapter against speculative API names.

This review incorporates the Physics-side design requirements into the WebPage handoff; it does not claim the Experience, solver adapter, damage model, or replay restoration is implemented. PR #115 remains a draft and no release/deployment is implied.

## 2026-10-17 coordination update — Experience-owned Castle Siege source is available

The authored Castle Siege source now exists in [TheSingularityWorkshop.Experiences](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/tree/development/experiences/CastleSiege), including its [battlespace content contract](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/blob/development/experiences/CastleSiege/design/BATTLESPACE_CONTENT_CONTRACT.md), [schema/example](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/tree/development/experiences/CastleSiege/contracts), and [session handoff](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/blob/development/experiences/CastleSiege/HANDOFF.md). WebPage should consume this authored source when the repository-backed Experience boundary is ready; do not create a second authoritative copy of castle objects or semantic IDs in Razor/scene code.

**Current boundary:** the Experience fixture declares a right-handed Y-up frame with metre-valued lengths. That is an authored-content convention, not a Physics guarantee. The Physics experiment is tracked in [issue #23](https://github.com/TrentBest/TheSingularityWorkshop.Physics/issues/23) and must provide actual tested trajectory samples and target intersection. Before connecting the two, agree on and test the coordinate conversion against the Physics API's real frame; do not assume the authored AABBs are collision geometry. The Experiences handoff explicitly says its trajectory adapter currently parses representative fixture evidence and does not call the live Physics model.

**Next consumer step:** once the browser host can load a pinned Experience artifact, parse/validate the Castle Siege content contract into a neutral scene model, preserve authored IDs/revision/frame metadata, and render an explicitly authored scene from that data. Keep projectile controls/results unavailable or clearly illustrative until the actual Physics contract is consumable. Do not block visible scene composition on the future damage/fracture model, and do not present a placeholder trajectory as Physics-backed.


## 2026-10-23 checkpoint — scene composition versus authoritative physics

The Experience-owned Castle Siege source remains the canonical authored definition. Its [session handoff](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/blob/development/experiences/CastleSiege/HANDOFF.md) and [battlespace contract](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/tree/development/experiences/CastleSiege/contracts) describe the authored scene and tabletop behavior; WebPage must not create a second authoritative set of castle object IDs or geometry in Razor.

Physics [issue #23](https://github.com/TrentBest/TheSingularityWorkshop.Physics/issues/23) remains open. Current source has analytical projectile, fixed-AABB target-intersection, and impact-diagnostic primitives, but the requested Castle Siege run/adapter is not yet proven end to end. The recent merged Physics [#25](https://github.com/TrentBest/TheSingularityWorkshop.Physics/pull/25)–[#29](https://github.com/TrentBest/TheSingularityWorkshop.Physics/pull/29) changes concern bounded seeded point sampling, Mandelbrot sampling, and run-record exporters; they do not close the projectile request. Physics Build and Tests passed on the reviewed PR heads.

The Experience's authored coordinate convention is right-handed Y-up with metre-valued lengths. That is not a Physics guarantee. Before connecting authored target geometry to the Physics API, define and test the coordinate transform against the actual Physics frame, and confirm target geometry semantics; an authored AABB is not automatically collision geometry.

**Current gate:** WebPage can own the commander-view composition, scene labels, accessibility, and experiment presentation. It must not show a Physics-derived trajectory, hit, impact, damage, or structural failure until the selected Physics run record supplies those outputs. Repository-backed loading of the pinned Experience is also blocked by [WebPage #100](https://github.com/TrentBest/WebPage/issues/100), [Experiences #19](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/issues/19), [MicroBundleDomain #17](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleDomain/issues/17), [MicroBundleRepository #22](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleRepository/issues/22) and [#24](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleRepository/issues/24), and [FSM_COS #11](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/issues/11). Do not bypass that boundary by copying Experience data into a parallel host-owned schema.

**Next step:** resolve the immutable Experience artifact/closure contract, then consume the authored Castle Siege data through the supported host path. In parallel, Physics should deliver the deterministic projectile run record and target-intersection validation requested by issue #23. No release, publication, merge, or deployment is implied by this checkpoint.

## 2026-10-24 — Authored spatial scaffold implemented on development

**Status:** navigable presentation fixture exists; no canonical Experience geometry or Physics solver adapter is connected.

- `TheSingularityWorkshop/Workshop/Spatial/SpatialPlaceCatalog.cs` registers city place `city.castle-siege` with scene ID `castle-siege`.
- `SpatialWorkshopScene.cs` adds the destination as a clickable spatial place; `Explore.razor` routes that scene through the existing spatial composition path.
- `TheSingularityWorkshop/Gui/SpatialCastleSiegeGuiBuilder.cs` presents the commander-shoulder composition with authored castle/siege-engine silhouettes, selectable **presentation-only** left/center/right zones, a working scene-label toggle, and explicit experiment-setup, baseline/variant, and measurement-status cards.
- `SingularityHub.Tests/SpatialCastleSiegeRegistrationTests.cs` checks the stable destination and that the default scene can be constructed with its existing catalog/overlap validation.

The highlighted zones are not Experience semantic IDs and are not collision geometry. The baseline/variant and measurement cards are status placeholders, not runnable experiment controls. The page explicitly says `ILLUSTRATIVE FIXTURE — NOT PHYSICS-BACKED` and does not compute or display a trajectory, hit/miss, impact, damage, or replay result. This scaffold is deliberately useful before the Physics adapter exists without claiming to load the Experience-owned Castle Siege artifact.

**Verification limit:** the .NET Tests workflow is configured for pushes to `development`, but no confirmed workflow result/status was available for the latest commits during this review. Do not treat the implementation as CI-verified yet.

**Next gate:** consume the canonical Experience-owned geometry only after the immutable artifact/closure path is resolved, then map its coordinates to Physics through a tested adapter. Physics issue [#23](https://github.com/TrentBest/TheSingularityWorkshop.Physics/issues/23) remains open.

## 2026-10-28 checkpoint — Physics primitives are present; cross-repository record consumption is not

The current Physics `development` branch now contains tested `ProjectileExperiment`, `ProjectileExperimentRunRecord`, `ProjectileTargetIntersection`, and `ProjectileImpactDiagnostics` contracts. Physics CI [run #40737290307](https://github.com/TrentBest/TheSingularityWorkshop.Physics/actions/runs/40737290307) passed after the authorized WebPageLLM coordination append. The older wording above that lists target intersection and a run record as entirely missing is now stale at the primitive level; the complete Castle Siege experiment is still not proven.

A source comparison found that Experiences' [`ProjectileTrajectoryEvidenceAdapter`](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/blob/development/src/CastleSiege/ProjectileTrajectoryEvidenceAdapter.cs) does not currently read the JSON shape emitted by Physics' [`ProjectileExperimentRunRecord`](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/Physics/Experiments/ProjectileExperimentRunRecord.cs):
- Physics emits root `coordinateConvention` and `units.time = "s"`; the adapter requires `definition.coordinateFrame` and `units.elapsedTime`.
- Physics' `definition` contains `deltaTime` and `stepCount`; it does not contain `coordinateFrame`.
- The current Experiences adapter test fixture is hand-authored to the adapter's expected shape, rather than generated by Physics' real record serializer.

This is an **Experiences-owned adapter/contract correction**, not permission for WebPage to add another parser or to change Physics' authoritative output. The shared consumer path remains blocked until the owner aligns the adapter with the actual Physics contract (or defines an explicitly versioned shared contract) and tests against real Physics-produced JSON.

The authored Castle Siege manifest still has a null artifact hash, and Workshop Showcase still has `experienceId: null`; immutable Experience publication and deterministic MicroBundle dependency closure remain open in [Experiences #19](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/issues/19), [MicroBundleRepository #22](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleRepository/issues/22) and [#24](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleRepository/issues/24), [MicroBundleDomain #17](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleDomain/issues/17), and [FSM_COS #11](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/issues/11). Until those gates are resolved, the WebPage scene remains an authored presentation fixture and must not display a Physics-backed trajectory or impact.

WebPage is also adding a source-integration check that regenerates the Singularity Studios plan from current Renderer → Visualize → GUI.SVG source and compares it with the committed SVG. The first attempt selected a stale nested Renderer source snapshot; [commit e9c4e61](https://github.com/TrentBest/WebPage/commit/e9c4e6144513e74353b9feb56bde5137b5aa746f) now passes the sibling source root explicitly. Exact-head CI is pending; do not claim the generated-plan proof is green until it completes.

No other repository handoff was edited. No merge, package publication, release, or deployment is implied.


## 2026-10-28 follow-up — distinguish Physics trajectory record types

**Correction to the checkpoint immediately above:** it compared Experiences' trajectory adapter with Physics' `ProjectileExperimentRunRecord`, but the checked-in `docs/experiments/projectile-trajectory.run.json` is the `ProjectileTrajectoryRunRecord` shape. The latter declares `units.elapsedTime` and `definition.coordinateFrame`, and the current `ProjectileTrajectoryEvidenceAdapter` requires those same fields. The adapter's test fixture matches this trajectory record shape. The checked-in trajectory fixture is therefore not mismatched with the adapter.

The real contract hazard is that Physics also has a distinct `ProjectileExperimentRunRecord` with root `coordinateConvention`, `units.time` / `units.speed`, and a different definition shape, while both record types currently use the same model ID/version and record schema version. WebPage appended this distinction to the authorized root [PhysicsCastlesLLM.md](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/PhysicsCastlesLLM.md) in [commit `1f152f1`](https://github.com/TrentBest/TheSingularityWorkshop.Physics/commit/1f152f1b8f76dbf4a79afd5252b9b7ad39676544).

**Owner action:** Physics should designate the canonical record for Castle Siege or give the two JSON contracts distinct model/schema identities and explicit consumers. Experiences owns its adapter and should validate it against the chosen Physics-produced record—not a separately hand-authored lookalike. WebPage will not add a third parser or silently normalize the two shapes.

This clarification does not activate the scene. WebPage still does not load the immutable Experience artifact or display a computed target-intersection result; Physics issue [#23](https://github.com/TrentBest/TheSingularityWorkshop.Physics/issues/23) and the immutable Experience/dependency-closure gates remain open. No merge, publication, release, or deployment is implied.
