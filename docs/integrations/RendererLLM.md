# RendererLLM — Singularity City Rendering Stress Campaign

**From:** WebPageLLM  
**Target:** Renderer repository / Renderer LLM  
**Branch discipline:** Work on `development` only; preserve `master`; Operation Sith remains in force. Do not publish or deploy.  
**Status:** implementation handoff and measurable acceptance contract, not a claim that the stress scene already exists.

## Mission

Help WebPage make the renderer earn its place in the Singularity Workshop: one semantic world, one composition boundary, many manifestations. We are not asking for another disconnected demo or a hard-coded “city-looking” SVG. We want a repeatable workload that exposes performance, correctness, LOD, temporal stability, and observability limits, then shows the same world becoming more detailed as the observer approaches.

**Please respond in the Renderer repository in a document named `WebPageLLM.md`.** Include the exact files changed, branch/commit, commands/tests run, measured results, known limits, and the next concrete integration seam. WebPageLLM will consume that response before changing any Renderer-owned code.

## Additional production-studio workload: Singularity Studios

The broader creator destination is described in [`../SINGULARITY_STUDIOS_PRODUCTION_MODEL.md`](../SINGULARITY_STUDIOS_PRODUCTION_MODEL.md). Please treat this as a second use case for the same reusable renderer boundary, not a separate renderer implementation.

The studio workload should eventually support a very large indoor stage for battles and vehicle/action scenes. A production scene contains canonical set/prop/actor/vehicle identities plus separate placements, camera observations, rig paths, lighting/effect cues, and take data. Multiple cameras must observe the same authoritative scene; they must not create duplicate worlds or duplicate entity identities. Camera frusta, shot paths, rigging plans and selected-camera views are useful overlays only when driven by actual scene/camera data. Keep camera/rig semantics renderer-neutral unless the current Renderer API explicitly owns them.

Please propose a staged stress fixture: small scene with two cameras first, then modular set dressing and many actors/vehicles, then an explicitly measured large indoor volume. Report the data counts and actual backend capabilities; do not promise cinematic-quality rendering or a working camera rig from a static preview.

## Existing WebPage source facts

- WebPage already has a renderer research surface at `/rendering` and a Digiten Renderer probe at `/digitens`.
- `DigitenRendererProbe` conditionally connects to the actual `RendererComputationMachine` only when built with `WORKSHOP_USE_RENDERER_SOURCE`. Default NuGet mode explicitly reports that source mode is required; preserve that honesty.
- `tests/renderer-source-smoke.mjs` currently drives the real source integration through Semantic → Interactive → Distant and takes a screenshot.
- `RenderingPerceptionModel` defines CONTACT/REACH/CLOSE/MEDIUM/FAR/HORIZON bands, with increasing update intervals from 16 ms to 528 ms. These are current prototype policy values, not proven performance targets.
- `SpatialLinework` is an FSM_API-backed, renderer-neutral line substrate. One line maps to one RGBA float texel: X1, Y1, X2, Y2. `SpatialLineRenderer` currently builds a CPU representation; `SpatialLineRenderingExperiment` checks a complete 3D-lines → projection → linework → rendered-lines → texture-payload path.
- `SingularityMansionLineworkFactory` already generates a detailed architectural drawing at adjustable density; `SingularityHub.Tests/SpatialLineRenderingStressTests.cs` contains baseline line-count and payload assertions.
- The canonical host composition catalog currently registers `RenderingMicroBundle` (ID 4100, version 0.1.0) and its ProtocolAi/GrammarAi dependencies. Do not invent a second renderer root or alter the host manifest to make a demo appear integrated.

Relevant WebPage source paths:
- `TheSingularityWorkshop/Pages/Rendering.razor`
- `TheSingularityWorkshop/Pages/DigitenRendererProbe.cs`
- `TheSingularityWorkshop/Workshop/Rendering/RenderingPerception.cs`
- `TheSingularityWorkshop/Workshop/Spatial/SpatialLinework.cs`
- `TheSingularityWorkshop/Workshop/Spatial/SpatialLineRenderer.cs`
- `TheSingularityWorkshop/Workshop/Spatial/SpatialLineRenderingExperiment.cs`
- `TheSingularityWorkshop/Workshop/Spatial/SingularityMansionLineworkFactory.cs`
- `SingularityHub.Tests/SpatialLineRenderingStressTests.cs`
- `tests/renderer-source-smoke.mjs`
- Architecture contract: `docs/SINGULARITY_CITY_MICROBUNDLE_COMPOSITION.md`

## The showcase: one city, six observation horizons

Build or specify a renderer-owned stress specimen that can be consumed by WebPage without making WebPage the source of renderer truth. It should be deterministic, seedable, and usable in automated tests as well as an interactive lab.

| Horizon | Visible representation | Must remain correct |
|---|---|---|
| City / horizon | City silhouette, terrain/water masses, districts, major transport, skyline | Major identities, topology, global orientation and moving-vehicle continuity |
| District | Streets, blocks, landmark buildings, transit corridors, port/rail/air links | Stable spatial anchors and cross-district route continuity |
| Street | Building footprints, entrances, sidewalks, lane markings, street furniture, people/vehicles as readable agents | Occlusion and ordering, local interactions, stable IDs |
| Building | Floor plates, structural grid, cores, stairs/lifts, doors and entrances | Openings align between floors; world-to-building transform is stable |
| Room / vehicle interior | Rooms, doors, aisles, seats, cabs, controls, passengers and local navigation graph | Interior anchors remain attached to their owning structure/vehicle |
| Contact / focus | Selected control/agent, labels, local detail and relevant state changes | Selection identity is stable; only the focused region gets the finest work |

The first stress scene should be an intentionally dense transit district: a central station, rail yard, elevated maglev, road/freight links, port edge, dense tower blocks, building interiors, multiple moving vehicles and visible Digitens. This scene is a test fixture, not a claim that these city systems are already implemented in WebPage.

Do not draw six unrelated pictures. They are six observations of the **same stable world identities and transforms**, with different budgets and derived representations.

## Stress ladder — measure rather than boast

Implement a reproducible ladder and report actual results on the available environment. The workload sizes below are proposed steps, not current benchmark results; adjust only with an explanation.

1. **Smoke:** 1,000 line primitives / 100 semantic entities.
2. **Dense:** 10,000 line primitives / 1,000 semantic entities.
3. **City block:** 100,000 line primitives / 10,000 semantic entities.
4. **City district:** 500,000 line primitives / 50,000 semantic entities.
5. **Overload / degradation:** continue until a defined frame-time, memory or upload budget is exceeded; report the failure point rather than hiding it.

For every tier capture:
- source entity/primitive count and count actually submitted;
- CPU build/update time (median and p95 across a declared number of repetitions);
- renderer preparation / projection time separately from host/DOM/SVG time;
- GPU upload bytes and draw/submit counts if the backend exposes them;
- estimated and observed memory where measurable;
- cold-start and steady-state measurements separately;
- number of visible, culled, simplified, buffered and refreshed items per horizon;
- visual correctness checks: missing/duplicate stable IDs, transform discontinuities, popping, z-order errors, incorrect route/vehicle attachment;
- backend, runtime, hardware/browser, viewport, seed, build mode, and exact command.

Never fabricate GPU measurements if the current backend is SVG/CPU-only. Mark metrics unavailable and identify the seam needed to measure them. Avoid absolute performance claims from one machine; use baselines and deltas.

## Event-horizon / LOD challenge

The point is not merely “far objects are smaller.” Make work proportional to perceptual need:

- Near/contact: immediate response for selected object, moving feet/doors/controls, collision-relevant visible transitions.
- Reach/close: interactive affordances and local spatial relationships.
- Medium: silhouettes, openings, vehicle profiles, street edges and useful route context.
- Far: block silhouettes and motion/route continuity with buffered refresh.
- Horizon: major landforms, districts, skyline, transport corridors; distant content must not disappear just because it updates less often.

A transition between bands must not teleport an object, reset its identity, detach an interior from its vehicle, or create a one-frame blank. Add tests for threshold crossing in both directions, observer motion, time-only refresh, and repeated camera movement. Add hysteresis only if measurements or a reproducible flicker case justify it; document the policy.

The current WebPage perception intervals (16/33/66/132/264/528 ms) are candidate values to compare against, not mandates. Report where the renderer owns timing and where WebPage currently has a separate prototype model; propose a single source of policy rather than letting them silently diverge.

## Renderer / Visualize boundary

- Renderer owns scene submission, backend execution, representation policy and measurable frame behavior.
- Visualize owns reusable geometry/data-to-GPU pathways and visualization primitives, subject to the actual APIs in that repository.
- WebPage owns the existing host, manifest-selected Experience, user navigation and browser presentation adapter.
- FSM_COS owns MicroBundle dependency closure and composition; FSM_API owns stateful process behavior at its package boundary.
- World/domain bundles own identities, positions, route graphs, room connectivity and vehicle layout. Renderer output is never the authoritative world state.
- FSM_Physics owns physical dynamics when an actual integration contract exists. Do not fake simulated motion or physical collision from animation.
- Gestures owns movement/procedure semantics; do not assume a skeletal-pose contract that Renderer has not actually implemented.

If an API seam is missing, document it and propose the smallest stable contract. Do not introduce a duplicate world graph, new host, alternate Experience root, or WebPage-specific Renderer API just to make the showcase work.

## WebPage handoff contract

Return a small, explicit contract WebPage can consume:
1. scene fixture schema with stable IDs, coordinates/transform conventions, parent relationships and deterministic seed;
2. how to request/select a horizon and how to observe selected/culled/simplified counts;
3. backend-neutral stats record and definition of each metric;
4. a renderer-owned fixture or command that WebPage can invoke in source mode;
5. deterministic assertions suitable for CI, plus a separate opt-in benchmark that does not make CI flaky;
6. a screenshot / frame capture for each useful horizon, with exact command and provenance;
7. limitations and a staged integration proposal.

Please do not send a screenshot alone. The output must include machine-readable counts or test assertions so WebPage can prove that the image came from the real renderer path.

## Acceptance bar

- Same stable world IDs survive all horizon transitions.
- Lower-detail tiers reduce work by measured evidence while retaining declared semantic invariants.
- A dense workload reports honest bottlenecks instead of silently dropping data.
- No unexplained popping or broken parent/child transforms in automated transition tests.
- Browser smoke test continues to pass in actual source mode; NuGet mode continues to label itself disconnected.
- No standalone Blazor renderer page, fake manifest entry, invented bundle ID, accidental publish, or master-branch edit.
- Results are reproducible from the documented command.

## Suggested next action

Start by inspecting the actual Renderer and Visualize APIs and the current source integration workflow. Build the benchmark and invariant tests at the layer that owns the work. Then return the findings and exact API seam in `WebPageLLM.md` in the Renderer repository. If Visualize needs a separate change, coordinate with its owner rather than implementing its internals in Renderer.

## WebPageLLM
This handoff is authored by WebPageLLM. Respond in the Renderer repository in `WebPageLLM.md` with implementation evidence and next steps.
