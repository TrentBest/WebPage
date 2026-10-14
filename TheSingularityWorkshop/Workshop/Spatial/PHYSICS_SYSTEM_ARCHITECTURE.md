# Workshop Physics System Architecture

> **Purpose:** a deterministic, composable physical-world model whose results can be inspected, measured, explained, replayed, and rendered by more than one host.

This proposed implementation contract extends [Singularity Laboratory Architecture](SINGULARITY_LABORATORY_ARCHITECTURE.md) and the requirements in [WebPage issue #49](https://github.com/TrentBest/WebPage/issues/49). It does not claim a validated general-purpose solver already exists.

## 1. Governing rule

**Physical state is authoritative. Presentation is a projection of that state.**

- **FSM_API** owns deterministic behavior/state transitions, lifecycle, process scheduling, and explicit state changes wherever those are FSM concerns.
- **Physics MicroBundles** declare models, parameters, material interactions, and experiment capabilities.
- **Numerical kernels** evaluate equations over defined time steps. FSM_API orchestrates these computations; an FSM is not a substitute for a numerical solver for every continuous field.
- **MicroBundleDomain and FSM_COS** compose capabilities and dependencies. An experiment requests only what it uses.
- **Presentation providers** render authoritative results. SVG, WebGPU, WebPage, AnyApp, and future hosts do not invent motion, reactions, fire, collisions, or material failure.
- **Education providers** explain the exact model and parameters used by the run, rather than presenting a disconnected article.

The FSM_API-driven Pong MicroBundle is a useful architectural precedent for separating simulation ownership from the host. Its simple gameplay equations are not themselves a general physics engine; the next step is to make model, time, units, numerical method, and observations explicit.

## 2. Runtime layers

```text
Experiment / World Entity / Interactable
                  |
          Experiment MicroBundle
                  |
        Physics composition manifest
                  |
  +---------------+----------------------+
  |               |                      |
Material data  Process models       Boundary conditions
  |               |                      |
  +---------------+----------------------+
                  |
       Deterministic simulation kernel
       - fixed logical time step
       - stable update ordering
       - explicit numeric policy
       - declared seeded randomness
       - recorded inputs and model versions
                  |
          Authoritative state + events
                  |
      measurements / checkpoints / lineage
                  |
        renderer + living explanation
```

### Responsibilities

1. **FSM_API:** lifecycle, discrete process states, ordered transitions, scheduled behavior, deterministic orchestration, and event-driven changes.
2. **Physics kernel:** numerical integration, constraints, collision/contact calculations, and field/continuum solvers appropriate to the selected model.
3. **MicroBundles:** reusable material identity, process laws, apparatus, boundary conditions, measurements, and model capabilities.
4. **FSM_COS / MicroBundleDomain:** resolve dependencies, install minimal compositions, preserve configuration, and expose stable capability boundaries.
5. **Presentation providers:** draw or sonify sampled state. They may choose visual fidelity but cannot rewrite scientific results.
6. **Education providers:** explain the exact model and parameters used in a run.

## 3. Determinism contract

The same initial state, ordered input stream, model versions, solver configuration, and logical time steps should produce the same authoritative result within a declared determinism scope.

Every experiment declares:

- model and MicroBundle versions;
- units and coordinate convention;
- initial and boundary conditions;
- fixed logical step or specified event-driven schedule;
- integration/solver method and parameters;
- collision/contact ordering and tie-breaking rules;
- numeric precision and supported platform determinism;
- random seed and deterministic random source, if applicable;
- input/event ordering, measurement cadence, and stopping criteria;
- tolerances and validation cases.

Do not use render-frame delta time, browser animation callbacks, wall-clock time, unseeded random values, accidental collection ordering, or GPU race ordering as hidden inputs to authoritative simulation.

Cross-platform bitwise determinism requires constrained and tested numeric and parallel execution policies. Where it is not guaranteed, document the narrower reproducibility contract and compare results against declared tolerances. **Determinism is not the same as physical accuracy:** accuracy requires validated models and reference data.

## 4. Composable MicroBundle families

This is a capability taxonomy, not a demand to implement everything at once.

### Foundations
- **Units and dimensions:** canonical units, conversion, dimensional checks, explicit quantity semantics.
- **Deterministic time:** logical ticks, substeps, event scheduling, pause/resume, checkpoints, catch-up.
- **Spatial substrate:** coordinates, transforms, geometry, bounds, and scale conventions.
- **Material identity:** elements, compounds, mixtures, alloys, phases, provenance, and versions.
- **Measurements:** sensors, sampled quantities, uncertainty/tolerance, run records, repeatable comparisons.

### Physical processes
- **Mechanics:** mass, force, momentum, impulse, torque, constraints, rolling/sliding, drag, collision response.
- **Thermodynamics:** internal energy, temperature, heat capacity, work, heat transfer, entropy where modeled, equilibrium.
- **Heat transfer:** conduction, convection, radiation, and material/ambient exchange.
- **Hydrodynamics:** density, pressure, velocity, viscosity, buoyancy, free surfaces, flow, declared turbulence approximations, sediment, erosion, and fluid-structure coupling.
- **Electromagnetism:** charge, current, fields, potential, conductivity, induction, and energy transfer.
- **Phase transitions:** melting, freezing, boiling, condensation, sublimation, solidification, latent heat.
- **Chemistry/reaction kinetics:** species, stoichiometry, reaction rates, concentrations, and reaction energy.
- **Combustion/oxidation:** fuel and oxidizer availability, reaction rates, heat release, products, oxygen transport, heat loss, and extinction conditions.
- **Material response:** elasticity, plasticity, strain, hardness, fatigue, fracture, wear, and corrosion.
- **Ionization/plasma:** ionization and energy balance, charged-particle interactions, conductivity, and emission within declared regimes.
- **Waves/acoustics:** propagation, material coupling, attenuation, and explicit approximation scope.
- **Geology/geophysics:** layered material, pressure/temperature history, flow, fracture, and slow-time processes.
- **Gravity/orbits:** force and integration models appropriate to the scale and required accuracy.

An experiment combines only what it needs. For example, fire couples material composition, heat transfer, gas transport, reaction kinetics, and radiation; it is not an independent visual effect.

## 5. Elements are not materials

An element bundle describes elemental identity and reference-grounded properties. A material bundle describes an actual substance: pure element, compound, mixture, alloy, or composite. Bronze and steel must not be flattened into generic material labels.

A material record should support versioned, provenance-tagged fields as available:

- identity, composition, microstructure/treatment when known;
- density and phase/temperature/pressure ranges;
- mechanical, thermal, electrical, optical, magnetic, chemical, and thermodynamic properties;
- phase transitions, oxidation/combustion, and corrosion where applicable;
- mathematical models, fitted parameters, valid regimes, and uncertainty;
- measured data vs derived values vs assumptions;
- references, edition/version, page/section or stable locator, and license metadata;
- related materials and compatible process models.

Unknown values remain unknown; the system must not silently invent material constants. Models state valid ranges and omissions.

### Iron sword vs copper sword; construction vs deconstruction

This is not a material-color swap. Declare geometry, mass distribution, composition/purity, heat treatment or microstructure when known, initial temperature, impact target, velocity/energy, and boundary conditions. Applicable models can predict deformation, edge damage, fracture, heat generation, and wear within their validated scope.

Deconstruction inspects and changes causal state: identify parts/materials, expose interfaces and constraints, remove or break a component, transfer released energy and momentum, and preserve resulting fragments, deformation, temperature, and damage. It uses the same physical laws as construction and impact, not a separate scripted destruction animation.

## 6. Fire and physical effects

Fire should be an evolving physical system with an observable manifestation, not a billboard or generic particle emitter. Depending on fidelity tier, state can include:

- fuel type, mass, geometry, moisture, exposed area, and remaining combustible material;
- local oxygen availability and transport;
- energy/temperature of fuel, nearby solids, and surrounding gas;
- reaction/oxidation rate, heat release, and products;
- convection, conduction, radiation, gas expansion, buoyancy, and flow;
- ignition, sustained combustion, spread, smoldering, and extinction conditions;
- smoke/soot/particulate production and transport within declared assumptions;
- emitted light/color derived from temperature/species only where the model supports it.

An electrical spark similarly starts with an electrical event and energy budget. Depending on the selected model, it can represent charge/current, breakdown or ionization, energy deposited in nearby gas/material, rapid heating and expansion, radiation, and cooling. If a fidelity tier omits a process, it must say so rather than claim full simulation.

### Fidelity follows relevance, not just visibility

- **Close inspection:** resolve the local processes needed for the interaction—potentially fuel geometry, heat transfer, gas flow, reaction progression, and emissions.
- **Nearby world:** use coarser cells or reduced models while preserving quantities that affect nearby objects and future state.
- **Distant mountain:** advance compact deterministic state such as fuel consumed, energy released/lost, spread boundary, environmental conditions, and elapsed logical time while detailed rendering is unloaded.
- **Return to focus:** reconstruct detail from saved aggregate state and model, then continue the same simulation. Do not reset the fire or make it appear newly ignited.

A fidelity transition is a change of representation/model resolution, not permission to discard history. State transfer must preserve relevant invariants and be tested against detailed reference runs. Document the error of any aggregate model. The same pattern can apply to fluids, erosion, weathering, structural damage, and slow chemistry.

### Effects are observations of causes

```text
electrical energy / contact / fracture
                |
                v
      energy deposited in material
                |
                v
     local temperature and phase state
                |
                v
      gas expansion / ionization / flow
                |
                v
       light, sound, debris, smoke
                |
                v
       heat transfer and cooling
```

This chain is conditional on the selected models; not every spark includes every process. Brightness, color, particle count, and motion are observables derived from simulation state, not the state itself. A renderer may simplify visual output without changing authoritative causal state.

## 7. Experiments are executable, versioned records

An experiment MicroBundle should retain:

- hypothesis/question and expected observations;
- apparatus and material bundles;
- initial and boundary conditions;
- model composition and versions;
- parameters, units, and numeric configuration;
- ordered user interventions;
- deterministic run identity and seed where applicable;
- measurements, events, checkpoints, and observed results;
- comparison with analytical results or reference data;
- residuals, uncertainty, assumptions, and known limitations;
- explanatory content, provenance, and visualization providers;
- a generated write-up linked to the actual run.

The write-up distinguishes prediction, measurement, and observation. A scientist can prepare an experiment before formal execution; authorization governs when and where it runs, not what its equations mean. A public exhibit can expose bounded controls against the same authoritative experiment without duplicating the physics.

## 8. First end-to-end proof: the ramp race

Let a player predict which vehicle reaches the bottom first. The result must come from the model, not an animation selecting a winner.

Declare ramp profile/angle, path length, vehicle mass and geometry, wheel/axle or sliding model, friction/rolling resistance, air drag if enabled, initial state, and fixed-step policy. If testing gravity alone, isolate that variable; if vehicles differ, declare which properties differ.

The experiment should:
1. record the player's prediction;
2. show equations and assumptions;
3. execute the same deterministic model for every contestant;
4. record arrival times, trajectories, energy changes, and contact events;
5. compare prediction with result and explain why;
6. support reset/replay and one-variable-at-a-time changes;
7. test repeatability, known limiting cases, and energy accounting.

This is a mechanics sandbox, a kernel test fixture, and an educational artifact in one.

## 9. Validation ladder

A visually convincing result does not validate a general physics engine. Build confidence through narrow tests:

1. **Units/invariants:** dimensional checks, valid ranges, conservation/accounting where applicable.
2. **Analytical cases:** constant acceleration, ideal ramp, elastic collision, simple heat exchange, hydrostatic pressure.
3. **Reference data:** compare selected models against cited measurements or established benchmarks.
4. **Convergence:** vary step size, resolution, and solver tolerances; report changes.
5. **Replay:** run the same declared input stream on supported runtimes and check the declared determinism contract.
6. **Coupling:** combine models after standalone baselines are tested; verify transfer of energy/mass/momentum.
7. **LOD handoff:** compare detailed simulation with coarse advance plus reconstruction and record error.
8. **Experiment regression:** preserve canonical scenarios and expected measurements across model changes.

Tests assert both results and limits. If a model is illustrative, label it illustrative. If calibrated to data, record the data and procedure.

## 10. Living documentation through GUI.SVG

Teach each capability where it is used. The explanation is connected to the bundle and exact experiment version, not copied into a disconnected page.

An explainer should expose:

- **What:** phenomenon and observed quantities.
- **Why:** causal process.
- **How:** equations, variables, units, assumptions, and solver choice.
- **Interact:** change a declared parameter and rerun the authoritative simulation.
- **Observe:** plots, diagrams, and measurements from the actual run.
- **Validate:** analytical/reference comparison, uncertainty, and limits.
- **Trace:** model version, material provenance, and citations.

GUI.SVG can manifest original accessible diagrams: energy-flow graphs, process/state transitions, vector fields, trajectories, material cross-sections, phase diagrams, and measured plots. SVG is an explanatory projection. Its labels, vectors, points, and plots should bind to the same data where possible; decorative geometry must not pretend to be a computed field.

Layer the teaching path: intuition first, then equations and code-level detail on request. Experts must be able to inspect assumptions without forcing every visitor to start there.

## 11. Proposed implementation sequence

1. **Audit and preserve existing work.** Keep the FSM_API-driven Pong pattern, laboratory catalogs/models, existing TBD physics manifest, and issue #49 requirements. Classify code as data catalog, actual solver, or presentation.
2. **Define the runtime contract.** Logical time, units, deterministic event ordering, checkpoints, provenance, and numeric reproducibility scope.
3. **Build minimal executable mechanics.** Use the ramp race, analytical comparisons, measurements, replay, and living explainer as acceptance criteria.
4. **Create the material substrate.** Start with iron, copper, bronze, and steel; separate elemental identity/composition from model-dependent properties and document provenance.
5. **Add thermodynamics.** Heating/cooling and energy accounting, then heat transfer and phase changes.
6. **Add bounded hydrodynamics.** Begin with a flow/pressure scenario before sediment, erosion, dams, and spillways.
7. **Add deformation/fracture and deconstruction.** Use material properties and energy/contact accounting, not scripted breakage.
8. **Add combustion/fire and electrical arc.** Couple only after thermal, material, gas/flow, reaction, and relevant field foundations exist.
9. **Implement multi-scale execution.** Test checkpointed coarse state and deterministic catch-up for one slow process, then generalize from measured error and performance.
10. **Expand the Laboratory.** Each exhibit composes the smallest required capability set and derives teaching/measurement surfaces from the same versioned run.

## 12. Guardrails

- Do not introduce a third-party engine as the authority merely for quick visuals.
- Do not make Unity a dependency of the core physics contract.
- Do not put renderer-specific types or UI state in physics models.
- Do not install every phenomenon or material into every world.
- Do not claim atomistic, quantum, plasma, combustion, turbulence, or fracture fidelity unless the chosen model supports it.
- Do not silently change the experiment model when changing visual detail.
- Do not confuse deterministic output with experimentally validated accuracy.
- Do not copy restricted reference prose or imagery; retain provenance and create original diagrams when necessary.

**Success condition:** an experiment is composed from versioned MicroBundles, executed through FSM_API-governed deterministic behavior and declared numerical models, replayed, measured, explained through living documentation, and rendered by different hosts without the host becoming the source of physical truth.
