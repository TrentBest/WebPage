# Singularity Laboratory — Station and Experiment Contract

![Living GUI documentation lens: question, model, implementation, visible manifestation, and proof](https://raw.githubusercontent.com/TrentBest/WebPage/development/docs/assets/living-gui-documentation-lens.svg)

> **Evidence class:** architecture illustration, not live runtime telemetry. Follow the subject-specific source, view, fixture, or test below; if none exists, record that gap rather than implying it is implemented.


**Status:** architecture contract and implementation plan. The floorplan now exposes the existing live FSM_API lifecycle interaction as the first Laboratory station. The full station catalogue and MicroBundle prescription workflow are not yet implemented.

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
| FSM Physics Simulation | the live FSM_API lifecycle station, process groups, and deterministic model experiments | The current lifecycle lab is runnable from the Welcome floorplan. Use actual FSM_API/FSM_COS/Physics contracts; never imply that model-level replay already restores and plays a full physical world. |

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

## Cross-cutting observer experiment

The Renderer / Visualize handoff asks for a deliberately hidden **Magic switch** as a physical 3D interactable in the Laboratory. It should switch between top-down plan and first-person observation of the same world, then switch back, without cloning the world or losing stable entity identity. WebPage owns the interaction and observer-mode transition; world content owns the switch's identity and placement; Renderer owns observer-relative representation decisions; Visualize manifests the selected frame. Keep it a discoverable secret rather than a prominent global UI control. Until a true first-person scene and shared-world transition are evidenced, label the capability as a prototype/plan rather than a finished realistic experience.

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

The Welcome floorplan now separates the existing live FSM_API lifecycle demonstration into the **Singularity Laboratory** and routes **The Forge** to the existing FSM Forge at `/explore?room=forge`. The FSM Forge already provides real state/transition/condition/action scaffolds and a live FSM_API preview. It is a first construction tool, not yet a general MicroBundle/Experience authoring system.

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


## School science is a first-class Laboratory use case

The Laboratory is intended to cover the sciences taught in school and the deeper mathematics behind them, not merely showcase game-physics demos. Teachers should be able to open a station during a lesson, set initial conditions, reveal governing equations, change one variable, and show students the consequences. Students should be able to reproduce the run and progress from an intuitive picture to a mathematical explanation.

Every lesson should support: a question and prediction; explicit model/assumptions/units/validity limits; inspectable measurements and axes; a progressive mathematics ladder (intuition, diagram and variables, governing equation, derivation, worked numerical example, limiting cases, error analysis and extensions); controlled baseline/variant runs; and teacher-facing objectives, prerequisites, discussion questions and common misconceptions.

### Science-station coverage roadmap

This is a coverage roadmap, not a claim that every solver exists. Every station must show its maturity and authoritative model before presenting results as physics-backed.

| Station family | Questions and demonstrations |
|---|---|
| Classical mechanics and gravity | Free fall, vertical throw, inclined plane, pendulum, orbit, projectile motion, momentum/collisions, energy conservation, terminal velocity when drag is modeled, and reference frames. |
| Moving-train cannonball | Fire vertically relative to a moving train; show train and ground observers simultaneously; explain inherited horizontal velocity, vector components and ideal assumptions. |
| Fluids and hydrodynamics | Arbitrary-shaped basins/landscapes, depth and pressure, buoyancy, toy boats, waves/reflections, rivers/channels, flow around terrain/buildings, dams/spillways, rainfall/runoff, drainage and water-driven erosion. |
| Earth systems and geology | Plate motion and boundaries, mountain building, faults/earthquakes, volcano formation and eruption stages, Ring of Fire, lava/ash/deposit layers, stratification, sediment transport/deposition, glacial and river striations, weathering, erosion, soil formation and terrain evolution over time slices. |
| Weather and climate | Heating/cooling, pressure/wind, humidity/condensation, precipitation, supported storm models, climate/biome conditions and terrain-water-weather feedback. Do not label generative weather a validated forecast. |
| Thermodynamics and energy | Temperature versus heat, conduction/convection/radiation, phase changes, expansion, engines/efficiency, energy transfer and coupled thermal/fluid demonstrations where supported. |
| Electricity, magnetism and waves | Charge/current/voltage/resistance, circuits/power, magnetic fields, induction, frequency, amplitude, interference and propagation. State actual model scope. |
| Materials and chemistry | Density, strength, elasticity, fracture, corrosion/weathering, mixing, reactions and phase-change demonstrations when the corresponding model exists. Separate visual effects from computed state. |
| Astronomy and planetary science | Gravity/orbits, seasons, day/night, tides where modeled, planetary formation, and stars-to-surface evolution through tectonics, atmosphere, weathering, erosion, climate and biomes. |
| Scientific method and mathematics | Measurement, units, dimensional analysis, graphing, uncertainty, numerical error, conservation checks, sensitivity analysis and analytical-versus-numerical comparison. |

### Canonical example: cannonball fired vertically from a moving train

For an ideal train moving at constant horizontal velocity, a cannonball launched vertically relative to the train inherits the train's horizontal velocity at launch. Ignoring air resistance and assuming constant train velocity, the ground-frame trajectory has horizontal motion from the train and vertical motion from gravity; in the train frame, the ideal trajectory remains above the cannon. Show both observers simultaneously. Drag, wind or train acceleration must be declared as separate model extensions.

Display the ideal equations and their assumptions:

- Horizontal position: x(t) = x₀ + vₓt
- Vertical position: y(t) = y₀ + vᵧ₀t − ½gt²
- Vertical velocity: vᵧ(t) = vᵧ₀ − gt

Label axes, SI units, launch state and observer frame. Derive the result from constant acceleration instead of treating animation as proof, and compare numerical integration against the analytical reference solution.

### Teacher and student workflows

- **Teacher:** select station and course depth, choose a prompt, establish a baseline, project the visualization, reveal equations step by step, and save/share a reproducible lesson configuration when supported.
- **Student:** predict, run, inspect measurements, alter a supported parameter, compare with baseline, explain the result, then optionally descend into the derivation.
- **Truthfulness:** label model version, assumptions, units, supported controls and unavailable capabilities. Never present fabricated data or decorative effects as solver results, or claim validation without reference tests.
- **Progressive depth:** one experiment should serve introductory science and advanced study through progressively deeper explanations, without contradictory simulations.

Implementation should be catalogue-first, then connect stations to domain-owned models one by one. Prioritize the canonical gravity/reference-frame station and a verified hydrodynamics/terrain experiment. Tectonics, volcanism, weathering, striations and coupled Earth-system evolution remain explicit goals whose solver status must be checked independently. The Laboratory hosts experiments; the Forge constructs reusable new model capabilities or authored Experiences.


## Evidence map against Physics development source (2026-10-17)

This is the current implementation boundary, checked against Physics' [capability catalog](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/docs/PHYSICS_CAPABILITY_CATALOG.md), [learning path](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/docs/LEARNING_PATH.md), and [gap register](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/docs/PHYSICS_GAP_REGISTER.md). A source type is not by itself a finished Laboratory station; WebPage still needs to connect it to a declared experiment, display actual run output, and expose the model's limits.

| Learning area | Confirmed Physics source | Current evidence boundary |
|---|---|---|
| Free fall / constant acceleration | `Physics/Kinematics/ConstantAccelerationSolver.cs`, `Physics/Experiments/ConstantAccelerationExperiment.cs` | Analytical 1D state advancement and versioned sampled run records; not a general force-driven body solver. |
| Projectile launch / target | `Physics/Kinematics/ProjectileState.cs`, `Physics/Experiments/ProjectileTrajectoryExperiment.cs`, `Physics/Collision/ProjectileTargetIntersection.cs`, `Physics/Collision/ProjectileImpactDiagnostics.cs` | Analytical 3D point trajectory, fixed-step records, point-vs-fixed-AABB intersection, incoming impact diagnostics. No drag, moving targets, launch mechanism, masonry damage or structural failure. |
| Moving-train cannonball | Existing analytical projectile/constant-acceleration primitives | Can be assembled as a carefully declared frame-comparison lesson; no dedicated train scenario or enforced frame-transform abstraction exists yet. A frame label is metadata, not an axis convention. |
| Water / terrain | `Physics/Hydrodynamics/ShallowWaterBasin2D.cs`, `Physics/Hydrodynamics/BathymetricShallowWaterBasin2D.cs`, `Physics/Hydrodynamics/LinearWaveBasin1D.cs` | Bounded shallow-water and wave models. Bathymetric model supports fixed uneven terrain and wet/dry cells; no boat coupling, flow around buildings, rainfall/runoff, sediment transport or erosion. Not full 3D fluid dynamics. |
| Volcanic terrain | `Physics/Geology/VolcanicTerrain2D.cs` | Deterministic prescribed-volume, idealized cone deposition onto a 2D elevation field. No eruption dynamics, plate tectonics, ash, lava transport, cooling, weathering or erosion. |
| Heat / energy | `Physics/Thermodynamics/ThermalConduction.cs`, `Physics/Thermodynamics/LumpedThermalState.cs`, `Physics/Experiments/ElectricalThermalCouplingExperiment.cs` | Lumped thermal accounting, bounded two-body conduction, and a first electrical-load-to-thermal coupling experiment. Conductivity/absorbed fraction are declared inputs; no spatial heat equation, phase change, convection/radiation or general reaction model. |
| Electricity | `Physics/Electricity/ElectricalGrid.cs` | Bounded ideal fixed-voltage DC/resistive-load accounting; not a general circuit/transient or electromagnetic-field solver. |
| Sound | `Physics/Acoustics/FreeFieldAcoustics.cs`, `Physics/Acoustics/AcousticRoomBox.cs`, `Physics/Acoustics/FirstOrderRoomReflections.cs` | Analytical free-field observations and bounded first-order rectangular-room reflection path metadata; no waveform/audio rendering, diffraction, occlusion or general room acoustics. |
| Replay / time | `Physics/Universe/LogicalTimeLedger.cs`, checkpoint/branch/lineage and replay-plan types | Time/history contracts exist; they do not by themselves restore a coupled world or make a WebPage scrubber authoritative replay. |

**Not confirmed as implemented solvers:** plate tectonics, earthquakes/fault dynamics, general weather/climate, atmospheric circulation, weathering, erosion, sediment transport/deposition, stratigraphy/striations, chemistry/reaction kinetics, and coupled planetary evolution. Keep these visible as planned learning destinations, but clearly mark them *planned* and do not generate solver-backed measurements until Physics supplies a tested model and evidence.

### Integration rule

For every station, link to the owning model and focused tests, state the model ID/version, units, frame, assumptions and validity limits, and show only values emitted by the actual run. Where possible, display an analytical/reference comparison and a conservation or error residual. Keep the baseline immutable and create a distinct run for each supported variant. WebPage owns the lesson and presentation; Physics owns equations, numerical state, validation and run results. No model should be duplicated in Razor or inferred from decorative rendering.
