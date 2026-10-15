# Living Water World — WebPage Integration Brief

**Purpose:** prepare a truthful WebPage / AnyApp demonstration of the Workshop's evolving-water physics.
**Status:** integration candidate; the 2D basin solver exists on a Physics draft PR, but the browser experience is not implemented.
**Do not publish, deploy, or claim a runnable water demo from this brief.**

## Visitor experience

The first scene should be a contained basin where a visitor can trigger a disturbance and watch the resulting wave travel and reflect. The visual surface, velocity indicators, and diagnostics must be derived from the solver's actual state. Later increments can turn the same world into a coastal/city environment with terrain, buildings, shorelines, and boats.

This is a candidate demonstration, not a blocker for other WebPage work.

## Evidence and current stage

### Implemented in Physics work — H0 and first H1 increment

- [Physics PR #14 — Linear wave reference](https://github.com/TrentBest/TheSingularityWorkshop.Physics/pull/14) adds a renderer-independent 1D linear wave reference with reflecting walls and a stability bound.
- [Physics PR #15 — 2D shallow-water basin](https://github.com/TrentBest/TheSingularityWorkshop.Physics/pull/15) adds `ShallowWaterBasin2D`: a flat-bed, closed-wall, depth-averaged 2D grid evolving depth and X/Y momentum through conservative Rusanov fluxes.
- [Latest PR #15 CI run](https://github.com/TrentBest/TheSingularityWorkshop.Physics/actions/runs/38767748851) passed for commit `aef447846445d73e0f046e55270b482de2a901a2`.

**Important integration qualification:** PR #15 is a draft based on `work/hydrodynamics-linear-wave`, itself the H0 work branch. These changes are not yet merged to the Physics `development` branch or released as a package. The WebPage host must not invent a package version or bind against an unverified public API.

### In development / next Physics gates

- Grid-refinement and convergence evidence against the analytical reference.
- More explicit numerical diagnostics and robustness checks for the supported regime.
- A defined model/input contract for scene geometry and boundary conditions.

### Planned physics stages

1. **Terrain and shorelines:** variable bathymetry and an explicit wet/dry policy.
2. **Buildings and obstacles:** explicit solid masks and tested boundary behavior.
3. **Boats:** bounded buoyancy and explicit water/body coupling, not independent animation.
4. **Interactive world:** visitor disturbances, reproducible parameters/run records, and live diagnostics.

These stages are not currently implemented by PR #15.

## Architecture contract

- **Physics core:** authoritative water state, equations, time stepping, boundary conditions, diagnostics, and observations. Keep it independent of WebPage, renderer, and Unity.
- **Scene description:** supply bathymetry, solid occupancy, basin geometry, and stable object identity as explicit data; do not infer physics from rendered pixels or meshes.
- **Composition and scheduling:** when the contract is stable, use the Workshop's existing MicroBundle / manifest / FSM_COS composition path. Do not create a parallel browser-owned simulation loop or a second fluid solver.
- **Renderer:** visualize snapshots/observations from the solver. It may interpolate presentation between known simulation states, but must not manufacture wave propagation or conservation readings.
- **WebPage / AnyApp:** host controls and presentation; keep the domain model reusable and testable without a UI.

## Browser integration sequence

1. **Contract gate:** confirm the Physics change is on an integration-ready branch and the supported public API/packaging path is known. No guessed package versions or copied solver implementation.
2. **Headless adapter:** create a narrow adapter that owns a basin instance, applies explicit disturbances through a supported Physics API, advances the solver, and exposes immutable observations. If the solver has no public disturbance API yet, add that capability in Physics with tests rather than mutating internal state from WebPage.
3. **Numerical-to-visual proof:** test that displayed cell values and measurements come from solver observations; verify a disturbance changes the sampled depth field and closed-basin volume remains within a declared tolerance.
4. **Local visual prototype:** render the observed depth field as a height/color surface with scale, basin walls, simulation time, and volume diagnostics. Label the scene experimental and identify the model's shallow-water / flat-bed limitations.
5. **Visitor interaction:** connect a click or pointer gesture to a deterministic, bounded disturbance through the adapter. Show a clear disabled/unavailable state until this path works end to end.
6. **Later scene expansion:** add terrain, obstacles, and boats only alongside corresponding Physics contracts and numerical tests.

## Acceptance checks before calling it a runnable demo

- The browser loads and advances the actual Physics solver; no shader-only or CSS-only wave substitute.
- A reproducible disturbance changes solver state and the rendered view reflects that change.
- Displayed depth, velocity, time, and volume are derived from solver outputs.
- Tests cover adapter behavior, mapping from solver observations to presentation data, and stable scene configuration.
- The scene states what is implemented, in development, and planned.
- Build/test and browser verification are recorded separately; a passing numerical test alone does not prove the browser experience works.

## Evidence boundary

The present solver is a flat-bed, positive-depth, closed-basin shallow-water model. It is not full 3D fluid dynamics and does not yet provide terrain, wet/dry shorelines, buildings, obstacles, boat coupling, breaking waves, spray, or an interactive browser scene. This brief specifies how to integrate the real model as it matures; it does not claim those features exist.
