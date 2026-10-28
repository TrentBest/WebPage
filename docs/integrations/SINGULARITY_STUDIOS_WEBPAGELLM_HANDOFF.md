# Singularity Studios — WebPageLLM Integration Brief

![Living GUI documentation lens: question, model, implementation, visible manifestation, and proof](https://raw.githubusercontent.com/TrentBest/WebPage/development/docs/assets/living-gui-documentation-lens.svg)

> **Evidence class:** architecture illustration, not live runtime telemetry. Follow the subject-specific source, view, fixture, or test below; if none exists, record that gap rather than implying it is implemented.


**Audience:** WebPageLLM, AnyApp, and contributors building the Workshop's public-facing experiences  
**Status:** living integration handoff; an initial source-mode Renderer → Visualize → GUI.SVG plan proof is wired, but this does not claim a working studio simulator, camera-view renderer, production GPU path, or complete audio pipeline  
**Source branches:** `development`  
**Do not publish or deploy from this brief.**

The matching native desktop consumer brief is [AnyApp's Singularity Studios handoff](https://github.com/TrentBest/AnyApp/blob/development/docs/integrations/SINGULARITY_STUDIOS_ANYAPP_HANDOFF.md). Both hosts are intended to consume the same versioned production data; this document does not establish a final shared API.

## Cross-repository guidance reviewed — 2026-10-17

This is a coordinated workload, not a WebPage-only scene. Keep the following owner documents active while implementing:

- **Renderer:** [Renderer's WebPageLLM response](https://github.com/TrentBest/TheSingularityWorkshop.Renderer/blob/development/docs/WebPageLLM.md) and [WebPage's Renderer handoff](RendererLLM.md). Renderer owns observer-relative representation choices, command reduction, refresh/freshness policy, and frame assembly. Its 5,000-candidate/1,000-entity diagnostic workload is useful prior evidence, not the studio scene or GPU evidence.
- **Visualize:** [Visualize's WebPageLLM coordination handoff](https://github.com/TrentBest/TheSingularityWorkshop.Visualize/blob/development/docs/integrations/WebPageLLM.md) and [city rendering integration contract](https://github.com/TrentBest/TheSingularityWorkshop.Visualize/blob/development/docs/integrations/SINGULARITY_CITY_RENDERING_HANDOFF.md). Visualize owns backend-neutral geometry/material/instance presentation and backend manifestation. GUI.SVG is inspectable CPU-side output, not GPU-performance proof.
- **Gestures:** [Singularity Studios procedure plan](https://github.com/TrentBest/TheSingularityWorkshop.Gestures/blob/development/docs/SINGULARITY_STUDIOS_GESTURE_PLAN.md). Reuse performer, crew, and equipment procedures by definition ID and bind them to stage/rig anchors. The production task graph coordinates them; Gestures does not own shot editing, physical contact, or audio mixing.
- **Physics:** [acoustic world model](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/docs/ACOUSTIC_WORLD_MODEL.md), [living acoustic demo](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/docs/ACOUSTIC_LIVING_DEMO.md), and the [Castle Siege response channel](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/PhysicsCastlesLLM.md) for that separate attraction. Physics owns physical outcomes and model-derived acoustic observations. The first-order room-reflection slice provides geometric path metadata, not synthesized room audio.
- **AnyApp:** [desktop consumer handoff](https://github.com/TrentBest/AnyApp/blob/development/docs/integrations/SINGULARITY_STUDIOS_ANYAPP_HANDOFF.md). Browser and desktop should consume the same versioned production fixture, not maintain parallel schemas.
- **Economy:** [Economy/WebPage handoff](https://github.com/TrentBest/TheSingularityWorkshop.Economy/blob/development/docs/ECONOMY_SYSTEMS_ROADMAP_AND_WEBPAGE_HANDOFF.md) is relevant later for optional creator shops, ownership, or simulated commerce. It is not a dependency of the first stage/camera proof.

### Current implementation checkpoint

- The versioned stage JSON remains conceptual planning data, with reusable definitions separate from placements and two cameras referencing the same scene identity.
- The source-mode plan generator routes draw commands through Renderer policy and `RenderFrame`, adapts with `RendererFrameAdapter`, then submits to Visualize's GUI.SVG backend. Package-only builds use the explicitly labelled static fallback.
- This is wiring proof only when source-mode build and smoke checks pass. GUI.SVG currently draws submitted commands; this is not measured LOD work reduction, a camera's rendered view, GPU output, or evidence that the illustrative 1000 m × 600 m × 80 m stage is supported.
- The fixture now declares an X-width/Y-elevation/Z-depth coordinate convention and X/Z plan projection. Treat this as a fixture-level declaration until the studio/camera/Gesture/Physics owners review a shared world-space contract. The plan projects X/Z to 2D; its current representation thresholds are normalized plan units, not physical metre thresholds.
- Next: verify CI; make the coordinate convention explicit; add a real two-camera observation of the same scene; then integrate a small reusable take/task fixture with Gestures. Keep acoustic path evidence as a separate, accurately labelled Studio sub-experience.


## The experience

**Singularity Studios is a data-driven digital filming studio, not a static showcase page.** A visitor should be able to assemble a production from reusable stage, set, performer, crew, equipment, camera, cue, and shot data; inspect how the pieces are bound together; and eventually replay the same take with changed blocking or camera coverage.

The long-term ambition includes enormous indoor stages capable of hosting battles and other scenes at a scale that would be difficult or impossible to film physically. Do not start by simulating an enormous stage. Start by proving that one small production can be assembled, run, inspected, reset, and reused without rewriting its contents.

The core product loop is:

1. **Choose a stage** — select a stage definition and its available marks, paths, props, rigs, and safe zones.
2. **Compose a take** — add performers, crew tasks, equipment moves, camera shots, and production cues.
3. **Bind reusable procedures** — assign semantic anchors such as performer marks, prop grips, dolly rails, camera targets, and docking points.
4. **Inspect readiness** — show missing anchors, conflicting paths, unmet prerequisites, and unsupported capabilities instead of silently pretending the take is ready.
5. **Run or scrub a reference take** — present the timeline, current task phases, transforms, and camera intent; label exactly which outputs are computed and which are illustrative.
6. **Reset and remix** — move a mark, change a camera, swap a performer, or reuse the same set for a new shot without duplicating every definition.

Keep the Workshop's living, instrumented visual language. The interface should reveal the machinery behind the spectacle, but it must remain clear to a visitor who has never read the source code.

## Where it belongs

The matching desktop consumer brief is [AnyApp's Singularity Studios handoff](https://github.com/TrentBest/AnyApp/blob/development/docs/integrations/SINGULARITY_STUDIOS_ANYAPP_HANDOFF.md). Both hosts should consume the same versioned fixture and production semantics; neither host should become a second owner of the studio domain.

Integrate Singularity Studios into the existing WebPage hub/showcase conventions. Do not introduce a competing navigation framework or turn the page into a conventional marketing landing page. The host's responsibility remains manifest-driven composition and browser presentation; reusable production behavior belongs to its owning package or provider.

Before coding, inspect the current WebPage and AnyApp development branches and preserve their existing navigation, lifecycle, and composition contracts. AnyApp is a useful desktop proving ground; WebPage is the browser proving ground. Neither should become the owner of a second implementation of the same studio semantics.

## The first vertical slice: one repeatable 30-second take

Build the smallest slice that proves reuse rather than a large mock city:

- One stage floor with explicit boundaries and a few movable set pieces.
- Two performer marks, one prop table, one prop, and one short dolly track.
- One static camera and one camera on a moving rig.
- A short task graph: performer A enters and reaches a mark; performer B turns and exchanges the prop; the camera dolly moves and settles; the static camera provides a second view; crew reset the prop, clear the path, and return the stage to its initial state.
- A timeline that displays task dependencies, current phases, and the selected playhead time.
- A reset/replay path that uses the same production data and stable seeds.
- One edit operation: move a mark or set piece, rebind the affected anchor, and replay. If the changed layout invalidates a path, report the obstruction or unresolved validation state; do not fake success.

The first slice can use a deterministic reference fixture while the world adapter is missing, but label that fixture as illustrative or reference-evaluated. A timeline that animates sample data is not proof of physical simulation.

## Keep the domain boundaries real

### Gestures

[Gestures](https://github.com/TrentBest/TheSingularityWorkshop.Gestures/tree/development) is the engine-neutral physical-procedure model and CPU reference evaluator. It can describe performer, crew, and equipment motion procedures. Its current implementation is an early alpha foundation, not a complete animation system, rig solver, physics solver, or studio simulator.

Use reusable procedure definitions with parameters and provider-supplied semantic anchors. Do not hard-code world positions into a generic procedure. A camera crane, dolly, lighting boom, curtain, or articulated set piece is equipment with its own anchors and degrees of freedom—not a humanoid rig.

Reference handoff: [Singularity Studios Gesture Plan](https://github.com/TrentBest/TheSingularityWorkshop.Gestures/blob/development/docs/SINGULARITY_STUDIOS_GESTURE_PLAN.md).

The current [`GestureEvaluator`](https://github.com/TrentBest/TheSingularityWorkshop.Gestures/blob/development/src/TheSingularityWorkshop.Gestures/GestureEvaluator.cs) is a stateless pose sampler: it does not own pause/resume state, interruption, looping, blending, task scheduling, anchor resolution, or authoritative physical outcomes. See the new [Gesture Evaluation Lifecycle contract](https://github.com/TrentBest/TheSingularityWorkshop.Gestures/blob/development/docs/GESTURE_EVALUATION_LIFECYCLE.md) before wiring a studio timeline or claiming a running procedure lifecycle.

### Physics

Physics owns collisions, contact, constraints, forces, obstruction, breakage, and physical outcomes. A gesture reaching the end of its requested time interval does not prove that a hand grasped a prop, a dolly cleared a performer, a cable connected, or a set piece locked.

Until a tested world adapter supplies those observations, display them as unavailable or not yet validated. Do not infer contact from a pose or attach an impact sound merely because a gesture reached an impact-like keyframe.

### FSM_API / FSM_COS

FSM_API may provide lifecycle and state-machine primitives in the consumer. FSM_COS is the composition boundary. Do not make Gestures depend upward on either system, and do not recreate FSM_COS dependency resolution or arbitration inside a studio page. Use the existing host composition path.

### Renderer / Visualize

Renderer decides what fidelity is needed for the observer and current shot; Visualize provides GPU-facing presentation. A close-up performer, a distant background extra, and a far-away camera crane should not necessarily cost the same to evaluate or draw. The studio timeline should not claim a performance win until CPU, upload, GPU, and visible-representation costs are measured.

### Audio and production sound

Keep authored dialogue, music, and designed cinematic cues distinct from physical sound sources. Physical footfalls, prop impacts, rolling equipment, rig latches, and breaking set pieces should originate from observed physical/material events and flow through the audio system. Gestures should not own an arbitrary list of sound effects. If SVG is used for the living documentation, it may visualize sound-source positions, event times, and propagation data; SVG alone is not the sound engine.

### Content providers

The studio owns production assembly and studio-specific anchors. A character/game provider owns its character identities and domain vocabulary. Reusable definitions should be referenced and bound, not copied into a WebPage-only collection.

## Suggested UI layout

Use a compact workbench, not a long scroll page:

- **Stage view:** overhead/set view with marks, set pieces, equipment tracks, camera frustums or target indicators, selected entity, and path/anchor warnings.
- **Take timeline:** shot/task lanes with dependencies, phases, event markers, and a scrubber. Distinguish authored intent, evaluator output, host-observed outcome, and illustrative data visually and in labels.
- **Inspector:** selected stage item, procedure, anchor binding, parameters, preconditions, current status, and provenance.
- **Production controls:** run, pause, reset, replay, and change-view controls only when their behavior is real. Unsupported operations should be disabled with a concise explanation.
- **Capability ledger:** a small implemented / reference-only / planned status list. Keep the limitations visible without burying the experience in disclaimers.

For an early alpha, the stage view and timeline should be the primary experience. Avoid building a full nonlinear editor, asset marketplace, collaborative editing, rendering farm, or sound mixer before the small take works end to end.

## Data model direction

These are conceptual records, not public APIs. Do not code against these names until the owning package's actual contracts have been inspected and agreed.

- **StageDefinition** — stage bounds, surfaces, zones, named anchors, and references to reusable set pieces.
- **RigDefinition** — mechanism type, degrees of freedom, limits, home/docking anchors, and supported control procedure IDs.
- **ProductionEntity** — stable identity, provider/type reference, and binding data for a performer, prop, crew member, camera, or set piece.
- **ProcedureBinding** — reusable procedure ID plus resolved semantic anchor IDs and explicit parameters.
- **ShotDefinition** — camera target/coverage intent, time interval, and references to production tasks.
- **ProductionTask** — task ID, prerequisites, requested procedure, start conditions, and host-supplied completion/failure observations.
- **TakeDefinition** — versioned stage/production references, stable seeds, timeline/cue references, and reset/replay policy.
- **TakeObservation** — timestamped evaluator output or authoritative host/Physics observation with source and confidence/status clearly identified.

Persist stable IDs and references, not copied snapshots of every reusable procedure. Version the definitions so a take can state which exact data it expects.

## Determinism, LOD, and truthfulness

- Replaying identical versioned inputs, bindings, stable identities, and seeds should produce identical reference-evaluator output.
- Random variation must be bounded and identity-stable, not freshly randomized each frame.
- A representation/LOD change must not reset the authoritative task phase or timeline position.
- A blocked path, missing anchor, failed prerequisite, or unavailable Physics observation must be visible.
- Never describe a planned integration as implemented, a reference fixture as a simulation, or a generated SVG as proof of a production-scale runtime.
- Profile before making scale claims: stage population, active procedure count, CPU evaluation time, allocations, upload bytes, GPU time, and representation transitions.

## Living documentation

Follow the Workshop principle: **“If a picture is worth a thousand words… then what is a living picture worth?”**

The stage map and timeline should ultimately be generated from the same versioned fixture and evaluator/task outputs used by tests. Show stage anchors, actor/equipment trajectories, camera intent, task gates, and key event times. Label concept-only layers separately. CI should regenerate the SVG and compare it to the checked-in snapshot so the picture cannot quietly drift away from the implementation.

The SVG is an inspectable documentation surface. It is not a substitute for the interactive WebPage workbench, a renderer, a sound engine, or a physical simulation.

## Acceptance gates

1. A fresh checkout builds and tests using the documented development workflow.
2. The same take can be reset and replayed with stable output.
3. A changed mark can be rebound without cloning the reusable procedure definition.
4. Missing anchors and invalid task dependencies are reported rather than silently ignored.
5. The timeline distinguishes intent, reference-evaluated output, and host-observed physical outcome.
6. The stage SVG is generated from real fixture/evaluator data and has a strict CI parity check.
7. Existing WebPage hub, manifest composition, and living-GUI behavior remain intact.
8. No package publish or deployment is implied by this brief.

## Immediate next steps

1. Inspect the actual WebPage hub/Experience architecture on `development`; choose the existing extension point.
2. Inspect AnyApp's current desktop test path and decide how it will consume the same data without owning its semantics.
3. Add a truthful, small studio workbench shell with an explicit reference fixture and capability ledger.
4. Define the 30-second fixture and semantic anchors in the owning provider/contract.
5. Add tested procedure outputs and a generated SVG before expanding to large stages.
6. Connect physical outcomes and sound only when the relevant Physics/audio adapter exists.

The goal is not to draw a giant studio. The goal is to make a production reusable data that can be assembled, evaluated, reset, inspected, and eventually filmed from any supported view.

## Extremus crowd representation coordination

Gestures now has a CPU reference baker, immutable input/output records, and an `ExtremusBakeManifest` in its [deterministic crowd-baking contract](https://github.com/TrentBest/TheSingularityWorkshop.Gestures/blob/development/docs/EXTREMUS_CROWD_BAKING.md). This is implemented CPU population-instance layout, not an impostor renderer or GPU crowd path. The planned first consumer fixture remains two reproducible starport population patches generated from distinct seeds and declared source-catalogue capabilities.

For WebPage, treat any first SVG/GUI.SVG view as a CPU-generated inspection artifact with explicit seed, profile, source IDs, count, and provenance. Keep the stage/world identity authoritative outside Renderer; do not introduce a parallel crowd schema just for the browser. Please coordinate the shared fixture shape and coordinate frame with Gestures, Renderer, Visualize, and Physics before presenting it as a runtime-linked studio crowd. The same versioned fixture should eventually be consumable by AnyApp.


## 2026-10-26 — Extremus crowd-bake consumer checkpoint

**Historical status as of 2026-10-26:** shared-fixture coordination was actionable and the bake was still described as design-only. The 2026-10-28 follow-up below supersedes that implementation status: the Gestures CPU reference bake and deterministic tests now exist, but WebPage and AnyApp still have no runtime-linked crowd consumer.

At this checkpoint the intended manifest fields were schema and bake-algorithm versions, source-catalogue and population-profile IDs/versions, stable region ID and coordinate frame/units, seed and deterministic algorithm, region/content revisions, generated identity plus activity/phase mapping where supported, and output asset IDs/hashes/generator version. The planned first fixture was two starport population patches from the same source vocabulary/profile family but different stable seeds. The later 2026-10-28 source check confirms the CPU baker and manifest now exist; it still found no WebPage or AnyApp consumer and no agreed immutable cross-host artifact/dependency-closure contract.

### Ownership and consumer boundary

- **Gestures / content providers:** own the reusable source/profile vocabulary and deterministic bake semantics; domain content providers own person/faction/equipment identity and domain-specific activities.
- **Physics:** supplies authoritative region, exclusion, flow, and world-revision inputs where implemented. A baked placement is not collision avoidance or a physical simulation.
- **Renderer:** owns the neutral representation descriptor, observer suitability, freshness/version policy, promotion/demotion continuity, and frame selection. See [Renderer issue #17](https://github.com/TrentBest/TheSingularityWorkshop.Renderer/issues/17).
- **Visualize:** realizes only representation/resource descriptions its current backend actually supports. See [Visualize issue #18](https://github.com/TrentBest/TheSingularityWorkshop.Visualize/issues/18). Do not describe a CPU/SVG snapshot as impostor rendering or GPU output.
- **WebPage and AnyApp:** must consume the same versioned fixture and expose its seed, profile, source IDs, generated count, and provenance. Do not invent a WebPage-only crowd schema or add per-frame randomness.

### Adjacent CI evidence — 2026-10-26

- [Visualize workflow run #40261788410](https://github.com/TrentBest/TheSingularityWorkshop.Visualize/actions/runs/40261788410) passed build/test, the dedicated Drone Construction frame proof, and GUI.SVG package proof; `publish_nuget` was skipped. The uploaded drone artifact is a **three-agent CPU/SVG proof**, not an Extremus bake, 100k-agent rendering, or a GPU-performance result.
- [Experiences workflow run #40261791798](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/actions/runs/40261791798) passed the source-integration, Castle Siege, Drone Construction, showcase, and quadrotor benchmark jobs; package publication was skipped. These results do not implement or validate the Extremus crowd contract.

**Next gate at the 2026-10-26 checkpoint:** obtain executable canonical bake records, deterministic/order-independence/seed-divergence tests, and a CI-generated manifest/snapshot. The 2026-10-28 follow-up below confirms those CPU-bake elements now exist. The remaining gate is agreement with MicroBundleDomain/Repository on the versioned immutable interchange identity and dependency closure, then Renderer/Visualize review of the neutral representation descriptor and golden-vector parity. WebPage and AnyApp consumers remain unimplemented; any SVG is still a CPU-generated inspection view.

No other repository was edited by this checkpoint. No merge, package publication, release, or deployment was performed.

## 2026-10-28 — Renderer → Visualize → GUI.SVG proof checkpoint

WebPage's source-integration workflow now checks out the current Renderer, Visualize, and GUI.SVG source graph, regenerates the Singularity Studios plan, and compares the result with the committed SVG. The [WebPage .NET Tests run on commit `5a361f2`](https://github.com/TrentBest/WebPage/actions/runs/40738066921) passed the `Workshop repository-source integration` job. This proves source-linked plan parity for that head, not a live studio runtime.

Related owner-side evidence:
- [Renderer build/test/package run #40435424052](https://github.com/TrentBest/TheSingularityWorkshop.Renderer/actions/runs/40435424052) passed on Renderer `43243b0`.
- [Visualize observer-driven presentation proof #40435427720](https://github.com/TrentBest/TheSingularityWorkshop.Visualize/actions/runs/40435427720) passed on Visualize `449fe14`.
- [Visualize build/test/package run #40435427754](https://github.com/TrentBest/TheSingularityWorkshop.Visualize/actions/runs/40435427754) failed only at `Verify deterministic frame and provenance manifest`; the other `build-and-test` and GUI.SVG package-proof jobs passed. The frame reports six submitted draws (three authoritative drones plus three illustrative overlays), while the current assertion expects three. This is a Visualize-owned proof assertion, tracked in [Visualize issue #18](https://github.com/TrentBest/TheSingularityWorkshop.Visualize/issues/18); WebPage will not edit Visualize's test or weaken the evidence boundary.

The artifact remains a **three-agent CPU/SVG proof**, not a 100,000-agent Extremus bake, GPU-performance result, or completed Singularity Studios production runtime. Extremus remains design-only until Gestures supplies executable canonical bake records and deterministic tests; WebPage and AnyApp must then consume the same versioned fixture rather than define separate crowd schemas.

No merge, publication, release, or deployment is implied by this checkpoint.

## 2026-10-28 follow-up — Extremus CPU bake exists; cross-host artifact remains open

**Correction to the next-gate wording above:** Gestures' current [`EXTREMUS_CROWD_BAKING.md`](https://github.com/TrentBest/TheSingularityWorkshop.Gestures/blob/development/docs/EXTREMUS_CROWD_BAKING.md) and development source already contain a CPU reference baker (`ExtremusCrowdBaker`), immutable input/output records, an `ExtremusBakeManifest`, deterministic stable IDs/positions/headings/scale/activity/phase, and tests for repeatability, input-order independence, seed diversity, bounds, exclusions, empty populations, and invalid inputs. The current Gestures development head `74a1b2e` passed [Build, test, and package run #39101076840](https://github.com/TrentBest/TheSingularityWorkshop.Gestures/actions/runs/39101076840) on 2026-10-18.

That advances the domain beyond a proposal, but only to **CPU population-instance layout**. Gestures explicitly does not yet provide a pose/animation bake, billboard atlas, impostor renderer, GPU-instancing path, crowd-LOD transition system, collision/clearance solver, or promotion back to live actors. The run is current-head evidence but is not a fresh post-18-October CI result.

### Remaining cross-host gate

WebPage and AnyApp still have no consumer for the canonical bake artifact. Before either host adds code, coordinate with Gestures and MicroBundleDomain/Repository on the versioned interchange schema, stable source-catalogue/profile/provider identities, region coordinate frame and units, seed/algorithm version, generated count/IDs, provenance, artifact hash, and dependency closure. Golden vectors and a CI-generated manifest/SVG should prove cross-language parity. Renderer then owns the neutral representation descriptor and observer/freshness policy; Visualize may realize only the representation resources its current backend supports.

The WebPage/Visualize proof cited above remains a three-agent CPU/SVG proof. It does not consume the Extremus bake, prove a crowd representation transition, or establish 100,000-agent/GPU performance. Do not duplicate the Gestures bake in WebPage or AnyApp while the shared artifact boundary is pending.

No other repository file was changed by this correction. No merge, publication, release, or deployment is implied.
