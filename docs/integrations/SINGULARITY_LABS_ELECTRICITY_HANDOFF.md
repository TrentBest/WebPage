# Singularity Labs: Electricity, Transistors, and the Living Grid

## Purpose

This is an implementation handoff for the WebPageLLM. Place the electricity-learning experience inside **Singularity Labs**—not as a detached utility page and not as a premature city infrastructure dashboard. Singularity Labs is where a visitor can experiment, observe cause and effect, learn the underlying model, and then follow that model outward into Singularity City's electrical infrastructure.

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

Deliver one polished, working experiment before attempting the full catalog.

### Interaction

- Show a small, readable DC circuit with a source, a switch, a resistive load, and measurement points.
- Let the visitor toggle the switch and adjust source voltage and load resistance within safe, meaningful educational ranges.
- Show voltage, current, resistance, and power with units.
- Show conventional current direction and distinguish it from electron drift where useful.
- Explain Ohm's law and power relationships from the live values, not from unrelated hard-coded numbers.
- Make the switch visibly and causally affect the current and power.
- Include a reset-to-known-experiment action and a small set of guided challenges.

### Living-picture rule

The circuit drawing, highlighted current path, meter readouts, and explanatory text must all derive from one authoritative experiment state. Avoid independently authored animation values that can contradict the numbers. If the underlying model cannot yet calculate a behavior, label it as a conceptual illustration rather than implying it is simulated.

The first Physics implementation is intentionally narrow: it accounts for switched resistive loads on an ideal fixed-voltage DC bus and aggregates demand against declared generation capacity. It is **not yet** a general circuit-network solver. Do not imply that arbitrary circuit topologies, voltage drop, transients, or transistor behavior have already been implemented.

Physics handoff: https://github.com/TrentBest/TheSingularityWorkshop.Physics/pull/22
Physics electricity design notes: https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/work/electrical-grid-loads/docs/ELECTRICITY_AND_SINGULARITY_GRID.md

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

### Stage 1 — Electricity Lab vertical slice
- Inspect the current WebPage experience/manifest architecture and determine the smallest compatible integration.
- Deliver one functioning switched resistive DC experiment.
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

## Acceptance checklist

- [ ] Singularity Labs is the natural home and entry point for the electricity experience.
- [ ] First experiment is usable and interactive, not a static mockup.
- [ ] Controls affect one authoritative model state.
- [ ] Measurements have correct units and derive from that state.
- [ ] Current/power calculations are tested against known cases.
- [ ] The UI distinguishes implemented behavior from conceptual illustrations.
- [ ] Physics remains renderer-independent; WebPage does not duplicate the solver.
- [ ] The README/docs link to the experiment and its theory/evidence.
- [ ] No unsupported claims about circuit solving, transistor fidelity, grid dynamics, or plant safety.
- [ ] Build and tests are run and their actual results are reported.

## Coordination note

This is a handoff and staged implementation plan, not a claim that all listed labs already exist. Start with the smallest real, testable vertical slice and leave clear notes about any Physics capability that is still pending. Keep the existing recording-studio work independent; this proposal is a separate Singularity Labs experience.
