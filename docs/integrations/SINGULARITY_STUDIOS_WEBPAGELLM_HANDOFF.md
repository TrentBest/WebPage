# Singularity Studios — WebPageLLM Integration Brief

**Audience:** WebPageLLM, AnyApp, and contributors building the Workshop's public-facing experiences  
**Status:** design handoff; this document does not claim a working studio simulator, camera system, renderer integration, or audio pipeline  
**Source branches:** `development`  
**Do not publish or deploy from this brief.**

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
