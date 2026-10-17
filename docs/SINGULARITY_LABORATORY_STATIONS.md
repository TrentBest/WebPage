# Singularity Laboratory — Station and Experiment Contract

**Status:** architecture contract and implementation plan. The Laboratory's full station catalogue and MicroBundle prescription workflow are not yet implemented by this document.

## Purpose

The **Singularity Laboratory** is the Workshop's discovery and experimentation environment. It is where a visitor can find a station for a capability, inspect what it actually demonstrates, configure an experiment, combine already-existing MicroBundles, run a sandbox, and compare evidence.

The Laboratory should be the easiest next step when a visitor has an idea or wants to see a capability that does not yet exist as a polished Experience. It should make the boundary between demonstrated behavior and proposed behavior visible.

The Laboratory is not the Forge.

- **Laboratory — discover, compose, configure, experiment.** Select existing capabilities, prescribe the MicroBundles an experiment should use, vary declared parameters, preserve a baseline, run controlled comparisons, inspect outputs, and record assumptions and evidence.
- **Forge — author and construct.** Create the fundamental blocks: MicroBundles, reusable capabilities, and authored Experiences. The Forge owns the creation workflow, not a second implementation of each domain's calculations.
- **Welcome / Workshop floorplan — orient and navigate.** Show the Workshop as a place and help the visitor discover its facilities. Do not make this schematic the authoritative catalogue or simulation runtime.
- **Domain packages — calculate and own semantics.** Physics owns physical models and authoritative observations; Gestures owns gesture definitions/evaluation; Renderer owns observer-relative representation policy; Visualize owns visualization contracts and manifestations; FSM_API owns state-machine primitives; FSM_COS composes declared capabilities; WebPage hosts browser interaction and presentation.

An Experience is an environment comprised of MicroBundles, not a page or a room label. The Hub owns stepping of declared process groups; an Experience exposes the groups it needs and must not drive a competing global heartbeat. Follow `EXPERIENCE_THEORY.md` and the canonical package contracts.

## Visitor model: a station is a door into a capability

A station is a discoverable entry point, not a hard-coded one-off simulation. It should identify:

- a stable station and experiment ID;
- the scientific/engineering domain and the question the station helps explore;
- maturity: **implemented**, **experimental**, **source-only/unavailable here**, **planned**, or **conceptual**;
- the Experience and MicroBundles selected for the run, including dependencies resolved by composition;
- input parameters, units, assumptions, supported ranges, and model-validity limits;
- controls for run, pause/step, reset to baseline, and compare—only where backed by actual model capabilities;
- inspectable outputs/observations, provenance, and links to source, tests, theory, and benchmarks;
- reproducibility metadata sufficient to distinguish runs and reproduce supported results.

A smooth animation is not evidence that a model is correct. If a station cannot answer a visitor's question with its current model, it should say so and point to the missing capability rather than imply scientific, engineering, or safety-critical validity.

## Initial station map

The Laboratory's existing spatial design describes a research tower organized by fields, not a collection of disconnected rooms. Preserve those domains as station groupings:

| Floor / station family | Station examples and intent | Current integration rule |
|---|---|---|
| Gravity & Spacetime | gravity bodies, trajectories, bounded launch experiments | Consume Physics-owned models and observations; do not duplicate solvers in browser code. |
| Energy Systems | electricity, switched resistive loads, power generation | Keep the electricity integration gated until Physics provides an agreed, usable contract. Do not imply a general circuit solver or transistor model from fixed-voltage DC accounting. |
| Materials Science | material tests, material comparisons, destruction/deconstruction experiments | Distinguish a real model/output from a visual illustration; expose assumptions and tested limits. |
| Atomics & Chemistry | atom and chemistry experiments | Reuse shared domain capabilities; do not build duplicate scene-specific rules. |
| Quantum Systems | quantum demonstrations | Label conceptual content as conceptual unless an owning model and evidence are present. |
| Plasma & Field Studies | plasma and electromagnetic-field experiments | Use authoritative domain observations; keep visual manifestation separate from calculation. |
| Fluids & Hydraulics | water, waves, boats, terrain interaction | Living Water World is a candidate, not a runnable station until WebPage advances the actual Physics solver and browser verification passes. |
| Cosmic Scale | solar-system arrangement and celestial bodies | Keep this separate from selected-world/globe editing and from map/terrain drafting. Current Welcome Observatory is conceptual. |
| FSM Physics Simulation | process groups, lifecycle, deterministic model experiments | Use actual FSM_API/FSM_COS/Physics contracts; never imply that model-level replay already restores and plays a full physical world. |

These are catalogue groupings, not a claim that every station is currently executable.

## Demonstrations belong in the Laboratory catalogue

The current WebPage Demonstrations gallery is a manifest-backed **metadata registry**, not yet a universal executable Experience loader. Keep its real maturity distinctions:

- **GPU Instancing — Browser WebGPU Proof:** runnable locally, not hosted by WebPage.
- **Native WPF / Direct3D 12 Surface Proof:** experimental, source-only.
- **Event Horizon Buckets — Renderer Architecture:** experimental architecture model; do not invent performance measurements.
- **Soldier Field — Exoskeleton and Event Horizon Lab:** live exploratory WebGPU experience; not a production .NET Visualize backend.
- **Living Water World:** planned until actual solver-to-browser integration and verification exist.
- **Physics Replay / Moments:** Physics currently provides a tested model-driven single-segment executor, not a physical-world adapter or browser playback experience.

Do not conflate a source link with a run link, a passing build with a runtime proof, or metadata discovery with an executable MicroBundle-backed Experience. Existing demonstrations should become Laboratory stations as their launch mode and evidence support them; do not relabel every gallery entry as runnable.

## The experiment prescription loop

The intended visitor workflow is:

1. **Choose a station** by domain, question, or capability.
2. **Inspect the contract**: what is implemented, which package owns it, and what is not proven.
3. **Prescribe MicroBundles** from the available catalogue. Resolve dependencies through the canonical composition path; do not hand-build a parallel dependency resolver in WebPage.
4. **Configure the experiment** using declared parameters and units. Preserve a baseline before changing parameters.
5. **Run a sandbox** through the owning model and the Hub/runtime contract. A UI-only mock must not be presented as a run.
6. **Observe and compare** actual outputs. Distinguish changed model parameters from hypothetical changes to physical constants, and flag configurations outside validated ranges.
7. **Save an experiment record** containing stable IDs, bundle selection, parameter values, versions, assumptions, output provenance, and run status where persistence is supported.
8. **Promote the idea to the Forge** only when the visitor wants to author a reusable block or a new Experience rather than merely compose existing capabilities.

This contract is a target workflow. The current demonstration registry does not yet provide all these controls, persistence, validation, or execution lifecycle.

## Relationship to the Forge

The Forge is where a creator builds or changes the reusable ingredients and authored environments. It should provide authoring tools for MicroBundles and Experiences and validate their declarations against canonical contracts. It should not become the place visitors must enter just to inspect a demo or change a sandbox parameter.

The existing Welcome floorplan currently opens the legacy FSM_API interactive lab from its room labeled **Forge**. That is a transitional placement, not the intended product boundary. The FSM_API lab is a demonstration and should be discoverable from the Laboratory; the Forge should ultimately open construction/authoring workflows. Do not remove the live lab until a verified Laboratory entry replaces the path.

A future installed MicroBundle may contribute a facility or station only through a stable, discoverable provider contract after composition actually resolves it. A manifest label by itself is not proof of an installed capability.

## Cross-repository contracts read for this plan

- [Physics Replay / Shareable Moments handoff](integrations/PHYSICS_REPLAY_WEBPAGELLM_HANDOFF.md): model-level replay is not a full physics-world playback; keep world reconstruction and replay claims gated.
- [Singularity Labs science and industry PR #115](https://github.com/TrentBest/WebPage/pull/115): scientific/industrial lab direction, causal manufacturing, evidence-backed experiments, and ownership boundaries; electricity remains gated until the Physics integration contract is usable.
- [Physics-side WebPageLLM coordination note](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/docs/coordination/WebPageLLM.md): physical/acoustic observations are model metadata, not waveform synthesis; ask for tested domain events instead of inventing them.
- [Renderer / Visualize coordination handoff](https://github.com/TrentBest/TheSingularityWorkshop.Visualize/blob/development/docs/integrations/WebPageLLM.md): Laboratory is the home for the hidden physical Magic switch; plan view and first-person view must preserve shared world identity, and the existing city proof is not yet proof of full Renderer-driven LOD.
- [Gestures / Renderer handoff](integrations/GESTURES_RENDERER_WEBPAGELLM_HANDOFF.md): a Gesture evaluates an intended procedure, not successful physical contact, load-bearing support, or completed boarding.
- [Demonstrations](DEMONSTRATIONS.md): current entries, launch modes, evidence limits, and requirements.
- [Spatial composition and Laboratory source notes](../TheSingularityWorkshop/Workshop/Spatial/README.md): the research tower's floor families, reusable security substrate, and Experience composition direction.

## Implementation gates

1. **Catalogue:** build one station catalogue from stable IDs, domain ownership, maturity, launch mode, and evidence links. Keep current station metadata distinct from executable Experience registration.
2. **Discovery:** expose the Laboratory as a destination in the existing hub and make the current FSM_API lab discoverable there without breaking its existing route.
3. **Sandbox contract:** agree on how an experiment declares MicroBundles, configuration, and process groups; use FSM_COS for composition and Hub-owned stepping.
4. **Evidence panel:** show model assumptions, units, actual observations, source/tests, and unsupported effects at each station.
5. **Replay and comparison:** add only after the domain model supplies the needed state/checkpoint and replay guarantees; keep the current Physics model-executor scope honest.
6. **Authoring handoff:** link to Forge when a visitor wants to create or alter a reusable MicroBundle/Experience.
7. **Verification:** add catalogue tests and browser journey tests, then validate the real station launch and outputs. Do not publish/deploy or promote branches as part of this work.

*The Laboratory lets us ask, "What happens if…?" The Forge lets us build the thing that makes a new answer possible.*
