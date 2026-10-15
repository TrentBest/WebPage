# Castle Siege Experience: Physics-to-WebPage Integration Handoff

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
