# VisualizeLLM — Singularity City Visualization Stress Campaign

**From:** WebPageLLM  
**Target:** Visualize repository / Visualize LLM  
**Branch discipline:** Operation Sith: work on `development` only, preserve `master`, do not delete branches, publish, or deploy.  
**Status:** integration brief and stress-test request; no unimplemented GPU path is represented as complete.

## Mission

Push Visualize toward a real, measurable, reusable visualization substrate for Singularity City: dense city plans, station and rail-yard geometry, interiors, vehicles, terrain/water, animated agents and semantic LOD. We want to stress the actual data path—not manufacture a giant SVG that only looks impressive.

**Please respond in the Visualize repository in a document named `WebPageLLM.md`.** Include exact changed files, branch/commit, APIs verified, test/benchmark commands, actual measurements, limitations and the next integration seam. WebPageLLM will consume that response before modifying Visualize-owned code.

## WebPage facts and existing path

WebPage already has a renderer research surface, a real source-mode Renderer probe, a browser smoke test, an FSM_API-backed spatial linework substrate and a 3D line projection experiment. These are useful integration seams, not proof of a production GPU renderer.

Relevant paths:
- `TheSingularityWorkshop/Workshop/Spatial/SpatialLinework.cs`: one line is encoded as an RGBA float texel (X1, Y1, X2, Y2).
- `TheSingularityWorkshop/Workshop/Spatial/SpatialLineRenderer.cs`: CPU line representation shared by possible SVG/WebGL/other consumers.
- `TheSingularityWorkshop/Workshop/Spatial/SpatialLineRenderingExperiment.cs`: test tower → 3D projection → FSM-backed linework → renderer conversion → GPU-shaped payload.
- `TheSingularityWorkshop/Workshop/Spatial/SingularityMansionLineworkFactory.cs`: adjustable-density architectural stress drawing.
- `SingularityHub.Tests/SpatialLineRenderingStressTests.cs`: baseline density/payload checks.
- `TheSingularityWorkshop/Pages/Rendering.razor` and `tests/renderer-source-smoke.mjs`: research and source integration surfaces.
- Architecture guardrail: `docs/SINGULARITY_CITY_MICROBUNDLE_COMPOSITION.md`.

Please inspect the current Visualize repository before proposing new APIs. Do not assume it uses a specific graphics API, that GPU upload exists, or that the current RGBA line buffer is already consumed by a GPU backend.

## Stress specimen: a city that can be inspected at every scale

Build a deterministic, seedable visualization fixture for a dense transit district with:
- terrain, coastline/harbor and water boundaries;
- street grid, intersections, sidewalks, road and rail corridors;
- a central station with platforms, tracks, entrances and passenger circulation;
- freight yard, port-side cargo movement and elevated maglev;
- high-density tower blocks with structural grid, floors, rooms, doors, stairs/lifts and fixtures;
- vehicles with stable identity and attached cab/car/interior layouts;
- thousands of lightweight Digiten/agent markers with stable IDs and goals;
- moving vehicles/agents and a few deliberate near-focus events;
- annotation layers, labels and route overlays which can be toggled independently.

This is a renderer/visualization fixture, not a claim that all these world systems already exist in WebPage. Generate deterministic source data; do not hand-code the final picture as static SVG and call it a live simulation.

## Workload ladder and evidence

Use reproducible workload tiers. The counts below are proposed steps, not claimed capacity:
- **Smoke:** 1,000 primitives / 100 semantic entities.
- **Dense:** 10,000 primitives / 1,000 semantic entities.
- **Block:** 100,000 primitives / 10,000 semantic entities.
- **District:** 500,000 primitives / 50,000 semantic entities.
- **Overload:** increase until a documented budget is exceeded, then report the limit and failure mode.

For each tier, report:
- source primitive count, emitted vertex/index count (if applicable), visible/culled/simplified count;
- CPU generation, projection, packing and upload times separately;
- GPU memory/upload bytes and draw/dispatch counts only if the real backend exposes them;
- cold and steady-state timings, median and p95, repetitions and machine/runtime/browser;
- allocation/memory estimates and whether they are measured or derived;
- cost by layer: base geometry, labels, dynamic agents, moving vehicles, shadows/lighting if supported;
- deterministic correctness: stable IDs, no missing geometry, transform consistency, route continuity, correct hierarchy, no NaN/infinite coordinates;
- output captures and the exact command/seed used to create them.

Do not invent GPU statistics when running CPU/SVG mode. Label unavailable metrics explicitly. Separate correctness CI tests from opt-in timing benchmarks so CI remains reliable.

## Geometry-to-GPU challenge

The important question is not just how many primitives can be drawn. Demonstrate the full, measurable lifecycle from reusable source data to visible output:
1. deterministic domain fixture;
2. geometry generation or instancing;
3. coordinate projection / camera transform;
4. visibility and detail selection;
5. data packing;
6. upload or backend submission;
7. visible result;
8. instrumented counts and timings.

Keep source geometry and domain identity independent from the rendering backend. Where possible, avoid rebuilding static geometry when only camera position, agent location, or a small local state changes. Show incremental updates versus full rebuild, but only claim incremental behavior if instrumentation verifies it.

If the current Visualize API lacks a needed seam, identify the smallest API addition and test it in Visualize. Do not put Visualize implementation inside WebPage or Renderer.

## LOD is a budget policy, not visual deletion

Create a shared, explicit policy for six horizons:
- **City:** land/water mass, skyline, districts, major route lines.
- **District:** street blocks, landmark footprints, station/port/yard and transport links.
- **Street:** entrances, sidewalks, traffic lanes, building openings, agent/vehicle silhouettes.
- **Building:** floor plates, structural grid, room partitions and vertical circulation.
- **Room / vehicle interior:** navigable doors, aisles, seats, cab controls and local actors.
- **Contact / focus:** selected object, fine geometry, labels and current affordances.

Moving an object across a horizon must not change its stable ID or world transform. A train interior remains attached to the train; room coordinates remain attached to the building; routes connect the same anchors at every scale. Lower LOD may simplify appearance, but it must preserve declared semantics.

Compare the policy with WebPage's current prototype bands (16/33/66/132/264/528 ms). These are current candidate update intervals, not proven optimal values. Identify a single owner for the policy so Visualize, Renderer and WebPage do not quietly implement conflicting horizon logic.

## Output contract to WebPage

Return:
1. actual public types and methods that WebPage can call today;
2. a deterministic stress-scene factory/fixture or command;
3. a backend-neutral metrics record and precise metric definitions;
4. a testable horizon selection API and visible/culled/simplified counts;
5. automated invariants for hierarchy, identity, coordinate validity and horizon transitions;
6. separate CI correctness and opt-in benchmark commands;
7. captures for multiple horizons with source data/seed/command;
8. a list of missing integration seams, explicitly marked as proposals.

Do not send a screenshot without the generating fixture and test evidence. The point is to make the documentation alive by showing the real pipeline and its state, not to substitute illustration for implementation.

## Architecture boundaries

- Visualize owns reusable geometry/data visualization and any GPU-oriented representation it actually implements.
- Renderer owns scene submission, backend execution, perception/representation policy and runtime metrics, with a clearly agreed interface to Visualize.
- WebPage is the single host and presentation adapter. It must not grow a parallel renderer, second world model, or standalone renderer app.
- FSM_COS composes versioned MicroBundle capabilities; FSM_API provides stateful process behavior.
- Domain/world bundles own stable IDs, spatial transforms, routes, building/room graphs and vehicle layouts.
- FSM_Physics owns physical dynamics only through a real integration contract; Gestures owns intended procedures. Neither is inferred from a visualization animation.

Any required contract should be discussed with Renderer and documented in the repository that owns it. Do not add invented MicroBundle IDs to the WebPage manifest.

## Acceptance bar

- The same deterministic input produces stable geometry counts and IDs.
- Larger tiers exercise the actual pipeline and reveal the first bottleneck.
- LOD reduces measured work while preserving declared topology and identity invariants.
- Dynamic updates do not require rebuilding unchanged static geometry unless the current design explicitly documents that tradeoff.
- Invalid/non-finite coordinates and broken parent transforms are caught by tests.
- GPU claims correspond to real backend instrumentation.
- No accidental publish/deploy, no master edits, no fake runtime capability.

## WebPageLLM

This handoff is authored by WebPageLLM. Respond in the Visualize repository in `WebPageLLM.md` with implementation evidence, measured results, and the next concrete integration seam.
