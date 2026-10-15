# Singularity Labs: Scientific and Industrial Experience Handoff

## Purpose

This is a staged experience handoff for the WebPageLLM. **Singularity Labs** is the Workshop's scientific and industrial experience layer: visitors can experiment, observe cause and effect, learn the underlying model, and follow that model outward into Singularity City's utilities, factories, logistics, and delivered products. Electricity is one planned vertical slice, not the full remit.

**Important integration gate:** keep the electricity implementation handoff separate from broader Labs and factory planning until Physics exposes an agreed, usable integration boundary. The current proposed Physics model is ideal fixed-voltage DC resistive-load accounting—not a general circuit or transistor solver. WebPage must consume authoritative Physics observations through that boundary; it must not recreate Ohm's-law, current, power, circuit, or transistor calculations in browser code as a substitute. Until the boundary is agreed and usable, electricity is a scoped design/coordination item, not permission to ship a parallel solver. Other Labs work may proceed independently where its own model ownership and contracts are explicit.

## The scientific-use mission: plan real experiments before running them

Singularity Labs is intended not only for public science education or fictional-city operations. It should grow into a practical simulation workspace that scientists, engineers, and experimental teams can use to explore whether a proposed experiment is worth running, test the mathematics behind a hypothesis, inspect predicted behavior, and prepare a more informed real-world experiment.

The target workflow is:

1. **State the scientific question.** Record the hypothesis, model domain, geometry, initial and boundary conditions, material/device parameters, controlled variables, measured quantities, and the intended comparison.
2. **Establish the model's validity envelope.** Show which equations and physical effects are implemented, which assumptions are made, which conditions are supported, and which conditions lie outside the model. A model must be able to say “not suitable for this question.”
3. **Check the mathematics before trusting the picture.** Provide analytical/reference cases, dimensional consistency checks, conservation checks, limiting-case tests, and independent benchmark comparisons where applicable. Visual plausibility is not numerical validation.
4. **Run controlled experiments.** Support repeatable single runs first, then parameter sweeps, side-by-side comparisons, and sensitivity studies when the owning model can support them. Record the exact configuration and model version for each run.
5. **Observe the evolving result.** Allow pause, step, reset, inspection of intermediate state, plots and units, and export of the actual observations. Rendering and animation must not alter or invent the scientific result.
6. **Compare with evidence.** Where appropriate, let users import or reference measured datasets, align units and sampling conditions, compare predictions against observations, and document discrepancies. Keep measured data, simulated output, and explanatory annotations visibly distinct.
7. **Produce a reproducible experiment record.** Capture model/package version, parameters, units, initial conditions, time-step or solver settings where relevant, random seeds where used, assumptions, outputs, validation status, and provenance. A colleague should be able to understand what was run and reproduce it to the degree the model supports.

### Trust and scientific limitations are first-class features

A useful laboratory must expose the boundary between a calculation that has been tested and a hypothesis that remains unverified. Each experiment should identify its model owner, equations or method, units, applicable range, known omissions, numerical limitations, and evidence supporting the implementation. Where uncertainty quantification, parameter fitting, convergence studies, or calibration are not implemented, say so rather than implying precision from a smooth plot.

Do not market every model as research-grade. A model may be useful for conceptual exploration, preliminary planning, teaching, or a narrow validated operating range without being suitable for final engineering decisions. High-consequence experimental planning requires domain-expert review and appropriate independent validation; the Lab should help make evidence inspectable, not replace scientific judgment, laboratory safety review, or professional responsibility.

### A reusable scientific workflow, not a one-off demo

The experience layer should eventually provide common experiment capabilities without forcing every domain into one numerical solver:

- experiment definition and validation against each model's supported inputs;
- model/version and assumption disclosure before a run;
- deterministic run, step, pause, reset, and replay where supported;
- parameter sweeps and comparative runs where computationally appropriate;
- plots, tables, spatial views, and living diagrams bound to the same authoritative observations;
- measured-data import/comparison with explicit units and provenance;
- exportable run manifests and results;
- validation evidence linking to implementation, tests, benchmarks, and theory.

These are shared *workflow* capabilities. Physics and other domain packages still own their equations, state evolution, numerical methods, and observations. Singularity Labs must not become a second solver or make a generic UI pretend that every scientific domain supports every control.

The Labs experience should make the Workshop's core idea tangible:

> If a picture is worth a thousand words, what is a living picture worth?

A diagram should not merely illustrate a lesson. It should be a view onto an actual running experiment. When a visitor changes a switch, voltage, resistance, transistor bias, or load, the displayed circuit and measurements should respond to the same underlying state.

## Product placement and visitor journey

Create a **Singularity Labs** destination or area within the existing WebPage experience, following the current manifest, configuration, composition, and presentation boundaries. Do not create a second application framework or hard-code a parallel lifecycle just for the lab.

Suggested initial Labs landing experience:

1. **Electricity Lab** — voltage, current, resistance, power, switches, and simple circuits.
2. **Transistor Lab** — NPN and PNP bipolar junction transistors, followed by MOSFETs once the first model is trustworthy.
3. **Grid Lab** — connect household/building loads to feeders, districts, and declared generation capacity.
4. **Power Plant Lab** — explore how different generation technologies turn an energy source into electrical output and how dispatch responds to demand.

These are learning stations in one coherent lab, not four unrelated demos. The visitor should be able to move from a small circuit to a transistor and then see how electrical devices become loads on the city grid.

## First vertical slice: Electricity Lab

Deliver one polished, working experiment before attempting the full catalog—but only after the Physics integration boundary is agreed and usable. Before that gate, WebPage work is limited to experience design, capability discovery, and non-simulated explanatory material that is clearly labelled as such.

### Interaction

- Show a small, readable DC circuit with a source, a switch, a resistive load, and measurement points.
- Let the visitor toggle supported loads and adjust only parameters exposed by the agreed Physics contract. Do not imply source-voltage adjustment or arbitrary circuit controls unless the model supports them.
- Show voltage, current, resistance, and power with units.
- Show conventional current direction and distinguish it from electron drift where useful.
- Explain relationships from the live Physics observations and model documentation; do not duplicate the numerical calculations in WebPage.
- Make the switch visibly and causally affect the current and power.
- Include a reset-to-known-experiment action and a small set of guided challenges.

### Living-picture rule

The circuit drawing, highlighted current path, meter readouts, and explanatory text must derive from one authoritative experiment state supplied by Physics. WebPage may format and visualize observations, but must not calculate replacement values. Avoid independently authored animation values that can contradict the observations. If the underlying model cannot calculate a behavior—or no usable integration contract exists yet—label it as conceptual, not simulated, and do not present it as a live experiment.

The first Physics implementation is intentionally narrow: it accounts for switched resistive loads on an ideal fixed-voltage DC bus and aggregates demand against declared generation capacity. It is **not yet** a general circuit-network solver. Do not imply that arbitrary circuit topologies, voltage drop, transients, or transistor behavior have already been implemented.

Physics proposal (draft; not yet an agreed integration contract): https://github.com/TrentBest/TheSingularityWorkshop.Physics/pull/22
Physics electricity design notes: https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/work/electrical-grid-loads/docs/ELECTRICITY_AND_SINGULARITY_GRID.md

**Do not treat these design notes or the draft PR as a stable integration contract.** Coordinate with the Physics owner to agree on the public API, lifecycle, units, observation semantics, versioning, and supported controls before wiring WebPage to it.

## Transistor Lab: the signature experience

The user specifically wants transistors taught as *living behavior*, not a static NPN/PNP picture. Treat this as the next substantive lab after the basic circuit slice.

### NPN experiment

Present the emitter, base, and collector as distinct regions. Let a learner adjust a simplified base-emitter bias and collector supply/load. The visual should explain, with clear model limits:

- the base-emitter junction's forward-bias condition;
- the collector-base junction's usual reverse-bias condition in the introductory active-region example;
- how a small base drive controls a larger collector current in the model's valid operating region;
- the difference between conventional current direction and electron motion;
- cutoff, active-region behavior, and saturation as distinct operating regions.

Show the relevant voltages and currents alongside the device. If the model transitions between operating regions, the diagram should change in response to that state. Avoid portraying electrons as literal little balls whose animation alone proves the electrical equations.

### PNP experiment

Use the same experiment vocabulary and controls where possible, but make the polarity and current-direction differences explicit. Do not merely mirror the NPN art and leave the learner to infer the electrical behavior.

### Accuracy contract

Begin with a deliberately bounded, documented educational model. Explain its assumptions and where it is inaccurate. Validate it against known operating-point examples and equations before presenting it as a predictive transistor simulation. MOSFETs, capacitance, switching transients, temperature dependence, and device curves can follow in later stages.

## Grid Lab: close the loop with Singularity City

The grid experience should establish a causal chain:

```text
City object is switched on
        -> its electrical load changes
        -> building demand changes
        -> feeder / district demand changes
        -> grid capacity and constraints are observed
        -> generation dispatch may need to respond
        -> the city-facing indicators update
```

A lamp, lift, train, workshop machine, recording studio, or other electrical object should eventually contribute its declared load to the same grid accounting system. Turning something off should remove its demand. The values displayed in the lab and in the city should agree because they are observations of the same model, not two separate sets of presentation-only counters.

Introduce complexity in steps:

1. stable IDs and deterministic aggregation of enabled resistive loads;
2. explicit circuit topology and node/branch measurements;
3. Kirchhoff's current and voltage laws for a bounded DC resistive network;
4. feeder resistance, voltage drop, and line losses;
5. transformers, protection, overload behavior, brownouts, outages, and load shedding;
6. coherent coupling to generation, storage, and dispatch.

Do not fake voltage collapse or breaker trips by changing labels alone. Add each behavior only when the model owns it and tests cover it.

## Power Plant Lab: generation as a system, not a decorative building

Make the Singularity Power Plant a guided operational learning environment. It can later become a walkable/inspectable facility in Singularity City, but the first WebPage proof should focus on the causal energy-conversion chain and readable telemetry.

### Generation technologies

- **Thermal generation:** fuel/heat source -> working fluid -> turbine or engine -> generator -> electrical output.
- **Nuclear educational plant:** fission heat source -> heat removal -> steam/working-fluid cycle -> turbine -> generator, with explicit neutron-population approximation, decay heat, cooling, alarms, and protective shutdown states as the model grows.
- **Renewables:** wind, solar, hydro, and other sources represented by their actual modeled input conditions rather than one generic “green power” toggle.
- **Storage:** charging/discharging, state of charge, power limits, and losses when implemented.

Let visitors vary demand, available generation, and operating constraints. Show generated power, grid demand, reserve/capacity, and the consequences of dispatch choices. Guided scenarios could include a demand spike, a generator becoming unavailable, variable renewable output, storage supporting a peak, and a cooling-system fault in the educational nuclear model.

For the nuclear scenario, teach energy conversion, feedback, heat removal, decay heat, and protection at a conceptual level. Label the experience as educational simulation—not plant operating instructions, engineering design guidance, or a safety certification tool. Do not present simplified reactor controls as instructions for operating real equipment.

## Beyond electricity: Singularity Labs as the scientific and industrial front door

Electricity is the first vertical slice, not the boundary of the Labs. **Singularity Labs should become the place where the Workshop exposes its physical models and other scientific capabilities as inspectable, interactive experiences.** It is also a deliberate front door into the Physics package: discover what is modeled, run a controlled experiment, inspect observations and assumptions, and follow the implementation/tests/theory back to the owning repository.

Treat the following as a long-range destination map, not a claim that these simulations already exist. Prefer a few real, composable vertical slices over a wall of impressive but inert buttons.

### Factory Lab: make products through causal processes

A vehicle factory should not spawn a finished vehicle from a menu. It should transform inputs through a traceable production chain:

```text
orders / design specification
  -> raw materials and purchased components arrive
  -> inventory and storage
  -> work orders and machine reservations
  -> fabrication / machining / forming
  -> assembly stations and tools
  -> inspection, calibration, and quality disposition
  -> finished vehicle inventory
  -> loading dock and outbound freight
  -> delivery to the city / customer / construction site
```

Each station consumes inputs, occupies capacity and time, produces outputs or scrap, and may be blocked by missing stock, a failed machine, a quality hold, or a transport delay. The factory should make bottlenecks and cause-and-effect visible: a missing motor delays assembly; a machine failure queues work; a quality failure sends a part to rework; a loaded truck or freight train carries the completed vehicle to its destination.

Build the model in layers: first deterministic work orders, inventory quantities, station capacity, and explicit state transitions; then processing time, energy/material consumption, machine condition, quality checks, buffers, and failures; finally spatial layout, workers/robots, and richer physics. Do not claim detailed machining or material behavior until the corresponding model exists.

### Exoskeleton Factory: raWWar equipment is manufactured too

A soldier's exoskeleton should be a product with a bill of materials, fabrication route, assembly sequence, test results, serial identity, and delivery history—not a costume that appears on a soldier. The production experience can eventually include frame manufacture, joint/actuator installation, power and control systems, armor/shunt integration where defined by the raWWar design, calibration, load testing, maintenance, and repair.

Keep ownership clean: Singularity Labs presents the factory experience; Physics provides physical models that genuinely belong in Physics; Gestures/IK provides movement and contact behavior; raWWar owns its soldier/equipment consumption and campaign context. The factory may compose those capabilities through established package boundaries, but must not copy their implementation or force their dependencies into the Physics core.

A completed exoskeleton can enter finished inventory, be loaded onto a vehicle, travel through a cargo network, arrive at a depot, and be issued to a soldier. That continuity is the point: production, logistics, and use are connected observations of a world, not unrelated scenes.

### Materials, energy, and utility facilities

Factories need utilities and material flows. Grow the Labs catalogue around shared contracts for mass, energy, heat, electrical demand, machine state, and material condition. Candidate experiences include:

- material testing: stress/strain, deformation, failure thresholds, thermal expansion, and comparative material samples;
- thermal processing: heating/cooling curves, heat transfer, and energy accounting;
- fluids: pumps, pipes, pressure/flow, tanks, and coolant loops when supported by validated models;
- water treatment and distribution, waste handling, recycling, and scrap recovery;
- machine shop and robotic workcells, including power consumption and scheduled maintenance;
- warehouse, loading dock, rail freight, truck fleet, and last-mile delivery;
- utility substations, generation, storage, and demand response connected to the same electrical accounting.

A facility is a physical system with inputs, outputs, constraints, and state. A dashboard can summarize it, but the underlying facility must own the causal behavior.

### Particle Accelerator / Hadron Collider Lab

Reach for a memorable science landmark: a hadron collider that visitors can explore as a chain of real, bounded models. Begin with a clearly labelled conceptual accelerator ring: particle source, accelerating cavities, magnets, beam path, detectors, and experiment readouts. Let visitors change a small set of parameters and see which observations change.

Build credibility in stages:

1. particle kinematics and relativistic energy/momentum calculations with units and checked reference cases;
2. magnetic bending and beam optics in a documented simplified model;
3. RF acceleration and staged energy gain, with explicit assumptions;
4. collision kinematics and conservation of energy/momentum for selected educational events;
5. detector layers and reconstructed tracks derived from generated event data;
6. statistical experiment views over reproducible event samples.

Do not imply that a browser visualization is a full accelerator lattice solver, a particle-physics event generator, or a research-grade detector simulation. Make model fidelity, simplifications, units, and provenance visible. Start with one testable event and a living diagram; add depth only as each layer gains numerical validation.

### How Singularity Labs wraps Physics and the wider Workshop

The Labs is the **experience and composition surface**, not a monolithic physics engine. It should wrap and expose capabilities owned by Physics and other packages through narrow, explicit adapters:

- a catalogue of experiments/facilities with their required capabilities, model status, and links to source/tests/theory;
- a shared run lifecycle: configure, initialize, step or run, pause, reset, inspect observations, and export/replay a reproducible scenario where supported;
- explicit model versions, units, assumptions, seeded inputs where randomness is used, and clear separation of measured output from explanatory annotation;
- adapters that translate WebPage controls into domain commands and domain observations into visual state;
- consistent time controls and event history without pretending all physical domains share one timestep;
- composition of Physics, FSM_API, FSM_COS, Visualize/Renderer/GUI.SVG, Gestures/IK, and application-specific packages only where their actual contracts permit it.

The Labs should make it easy to add a new experiment without making Physics depend on WebPage. It should also make missing capability honest and useful: a catalogue entry can say “planned” or “conceptual preview” rather than showing a fake live simulation. Do not build a second lifecycle or scheduling system in the UI if the Workshop already has an established composition path.

## Architecture and ownership boundaries

- **Physics** owns deterministic physical/electrical calculations, model state, and observations. It should remain independent of WebPage, Unity, SVG, and rendering technology.
- **FSM_API** supplies state-machine behavior and coordination/scheduling where appropriate; it is not a replacement for numerical solvers.
- **FSM_COS** composes declared capabilities at the Workshop boundary; do not duplicate its composition lifecycle inside the lab.
- **WebPage / Singularity Labs** owns browser experience, controls, presentation, guided lessons, and the visual binding between model observations and the visitor's view.
- **Renderer / Visualize / GUI.SVG**, when appropriate, can supply reusable visual surfaces; do not make Physics depend on them.
- **Singularity City** later consumes the same grid/load observations for building and infrastructure experiences.

Use the repository's existing manifest/configuration and composition patterns. Before adding new host-side infrastructure, inspect the current WebPage implementation and reuse its established experience path. Keep each capability in its owning package and expose it through a deliberate boundary.

## Living documentation and evidence

Every experiment should answer four questions at a glance:

1. **What can I change?**
2. **What changed in the model?**
3. **What evidence explains the result?**
4. **What is simplified or not implemented yet?**

Prefer live measurements, annotated state, and model-linked explanations over a scroll-heavy article. A visitor should be able to pause at a result, inspect the values, and understand the rule that produced them. Provide links to the relevant Physics implementation, tests, and theory notes so the visual is an entry point into the actual system.

The visuals should follow the Workshop's existing documentation style and the current WebPage design. Do not invent a parallel visual language, use decorative controls that do nothing, or claim a model is live when the visual is only a static illustration.

## Suggested delivery sequence

### Stage 1 — Electricity integration boundary (blocked until agreed)
- Coordinate with Physics to agree on a supported public contract for configuring/observing the existing load-accounting model.
- Establish units, lifecycle, versioning, validation behavior, and which controls are actually supported.
- Do not duplicate Physics calculations in WebPage or infer support for circuit topologies/transistors.

### Stage 1b — Electricity Lab vertical slice (after the boundary is usable)
- Inspect the current WebPage experience/manifest architecture and determine the smallest compatible integration.
- Deliver one functioning switched-resistive-load experience backed by Physics observations.
- Bind all displayed readings and visual state to a single experiment state.
- Explain Ohm's law and power using the actual values.
- Include reset, one or two guided challenges, and visible scope notes.
- Add appropriate tests for any new state transitions or model adapters.

### Stage 2 — Circuit model integration
- Coordinate with Physics as the DC network solver develops.
- Add node and branch measurements and KCL/KVL validation examples.
- Keep the lab adapter thin; don't reimplement circuit calculations in browser code.

### Stage 3 — NPN / PNP Transistor Lab
- Add the bounded model only after its equations, operating regions, and known examples are validated.
- Visualize junction bias, current paths, measurements, and cutoff/active/saturation.
- Make the assumptions inspectable.

### Stage 4 — Grid Lab
- Connect city/building load events to deterministic aggregate demand.
- Show capacity, demand, overload state, and later modeled feeder constraints.
- Ensure multiple city experiences read from shared model observations.

### Stage 5 — Power Plant Lab
- Start with an energy-conversion and generator-output model.
- Add dispatch and storage scenarios.
- Introduce the educational nuclear model only with explicit simplifications and conservative, testable state transitions.

### Stage 6 — Factory and delivery loop
- Model one simple product with explicit inputs, work orders, stations, inventory, inspection, and finished goods.
- Deliver the finished product through a real simulated logistics chain to a named destination.
- Add a vehicle factory, then an exoskeleton manufacturing route composed with the owning Gestures/IK and raWWar capabilities.
- Track shortages, queues, machine downtime, quality holds, energy consumption, and delivery state from model observations.

### Stage 7 — Scientific landmark experiments
- Add a validated particle-kinematics experiment, then expand toward a conceptual hadron-collider ring.
- Derive detector views from reproducible event data; disclose the fidelity of each physics layer.
- Grow the Labs catalogue into material, thermal, fluid, and other experiments only when each has an explicit model owner and testable scope.

## Acceptance checklist

- [ ] Singularity Labs is the natural home and entry point for the electricity experience.
- [ ] Electricity implementation begins only after a usable Physics integration boundary is agreed; until then, any preview is explicitly conceptual.
- [ ] First implemented experiment is usable and interactive, not a static mockup.
- [ ] Controls affect one authoritative model state.
- [ ] Measurements have correct units and derive from that state.
- [ ] Current/power calculations are tested against known cases.
- [ ] The UI distinguishes implemented behavior from conceptual illustrations.
- [ ] Physics remains renderer-independent; WebPage consumes authoritative observations and does not duplicate the solver or accounting formulas.
- [ ] The README/docs link to the experiment and its theory/evidence.
- [ ] No unsupported claims about circuit solving, transistor fidelity, grid dynamics, plant safety, factory fidelity, or collider accuracy.
- [ ] Products are manufactured from modeled inputs and process steps, then transported/delivered; they do not simply appear.
- [ ] Labs catalogue distinguishes implemented, planned, and conceptual experiments and points to the owning package.
- [ ] Build and tests are run and their actual results are reported.

## Coordination note

This is a handoff and staged implementation plan, not a claim that all listed labs already exist. Start with the smallest real, testable vertical slice and leave clear notes about any Physics capability that is still pending. Keep the existing recording-studio work independent; this proposal is a separate Singularity Labs experience.
