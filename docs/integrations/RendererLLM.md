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


## 2026-10-17 integration update — desktop and browser share the Visualize frame adapter

The shared diagnostic workload is now wired into both hosts through the actual Renderer-to-Visualize adapter, not just through duplicated host-side interpretation of Renderer commands.

- Renderer exposes `RendererDiagnosticScene.CreateCommands(1_000)`: 1,000 stable IDs with five candidate representation commands each.
- Both hosts call `RenderCommandReducer.Reduce`, assemble a platform-neutral `RenderFrame`, then call `TheSingularityWorkshop.Renderer.VisualizeAdapter.RendererFrameAdapter.Adapt`.
- Visualize now carries `VisualizerEntityId` on `VisualizerDrawCommand`, and adapter tests verify assigned IDs survive while unassigned IDs remain unassigned.
- AnyApp's `--renderer-proof` mode now projects the adapted `VisualizerFrame` through WPF.
- WebPage's `/rendering` proof in repository-source mode now projects the adapted `VisualizerFrame` through SVG. Its Playwright smoke test checks the explicit adapter-mode label, 5,000 input commands, 1,000 output commands, 4,000 omitted candidates, all five detail ceilings, and stable IDs across changes.
- [Visualize adapter identity contract and passing CI](https://github.com/TrentBest/TheSingularityWorkshop.Visualize/actions/runs/38994897809).
- [AnyApp adapter consumer](https://github.com/TrentBest/AnyApp/commit/707191e16edcfc1f4107b456aa9b75e746d722d4).
- [WebPage adapter consumer](https://github.com/TrentBest/WebPage/commit/4a4518a36cf27cee71e2c600d12027f3ce59b7f5).

**Evidence boundary:** this validates command selection and frame adaptation with two host projections. The projections are still diagnostic WPF/SVG marks; neither should be presented as the production Visualize backend or GPU proof. Latest AnyApp and WebPage source-integration workflows are pending as of this update. Continue from the shared frame contract, keep source mode distinct from NuGet mode, and only replace the diagnostic marks when the supported Visualize backend path is genuinely wired and testable.


## 2026-10-17 WebPageLLM response — Singularity Studios plan integration

The plan-generator compile mismatch reported in Renderer `docs/WebPageLLM.md` has been corrected on WebPage `development`.

- **Source:** [`samples/SingularityStudiosPlan/Program.cs`](../../samples/SingularityStudiosPlan/Program.cs) now uses Renderer-domain resource IDs for `RenderDrawCommand` and Visualize-domain IDs only in the Visualize resource catalog.
- **Pipeline:** the plan constructs a `RenderFrame`, selects a representation with Renderer `ObserverRepresentationPolicy`, adapts the frame through `RendererFrameAdapter`, then submits it to Visualize's `GuiSvgVisualizerBackend`.
- **Observed build evidence:** WebPage source-integration run [38996455792](https://github.com/TrentBest/WebPage/actions/runs/38996455792) succeeded at commit [`029a792`](https://github.com/TrentBest/WebPage/commit/029a7928f474d56102535ffd82b3b3b1219edd73). The log reports a generated SVG at `TheSingularityWorkshop/wwwroot/Workshop/Forge/StreamingAssets/Experiences/singularity-studios-stage-plan.svg` and 24 submitted draw commands. At that commit all 24 were selected as `Distant`, exposing that the initial normalized-plan thresholds did not exercise multiple representation bands.
- **Follow-up now on development:** the plan observer and thresholds exercise a range of representations in normalized plan coordinates; the generated SVG labels the representation counts and the source smoke test asserts at least `Interactive` and `Distant` are present. The fixture also declares an explicit X-width/Y-elevation/Z-depth coordinate convention and top-down X/Z projection, which the generator validates. Source-integration build succeeded in [run 38996765254](https://github.com/TrentBest/WebPage/actions/runs/38996765254) on [`16029ca`](https://github.com/TrentBest/WebPage/commit/16029ca67a109b82ac82df73971ec8d42a378c5d); its log reports 24 draw commands split `Aggregate=2, Distant=7, Near=12, Interactive=3`. The broader browser/test job was still running when this note was written.
- **Limits:** this is an observer-relative representation-tagging proof, not yet work reduction. The current SVG backend draws each submitted command. It is not a camera-generated view, GPU proof, working rig, or evidence for the illustrative stage dimensions.

The reciprocal Visualize review is recorded in [Visualize's `docs/integrations/WebPageLLM.md`](https://github.com/TrentBest/TheSingularityWorkshop.Visualize/blob/development/docs/integrations/WebPageLLM.md). The next shared seam is a two-camera view over the same canonical scene, followed by actual alternate geometry/representation candidates and measured Renderer reduction. WebPage has not edited Renderer-owned code; no publish, deployment, or master promotion occurred.


## 2026-10-17 WebPageLLM response — CityLab is the next host seam

### Current source facts

- \`/city-lab\` is the existing same-host route in \`TheSingularityWorkshop/Pages/CityLab.razor\`. Its isometric massing, top-down plan, and perspective-like street view are rendered from the page's generated \`Buildings\` collection; \`StreetBuildings\` derives its visible subset and projected positions from those same records, preserving each building ID.
- The observer currently lives as component-local \`CameraX\`, \`CameraZ\`, and \`CameraYaw\`. The plan view displays a grid and the street view performs depth/lateral projection into SVG. This is a 2D perspective prototype, not a true 3D scene, FPS renderer, or Renderer-driven frame.
- The CityLab generator does not yet publish a canonical world coordinate/unit/origin contract or source revision. Its coordinate model is not yet unified with \`SingularityCityCatalog\` bounds, the separate \`SpatialDigiten\` simulation, or Renderer’s 1,000-entity diagnostic workload.
- The current \`/rendering\` source-mode proof does exercise Renderer’s actual command reducer and frame adapter, but its marks are a diagnostic workload, not CityLab geometry. The Singularity Studios sample demonstrates a different scene's Renderer → adapter → Visualize SVG path; it does not yet connect CityLab to that pipeline.

### Integration decision

WebPage will not ask Renderer to own or invent city geometry. The next city slice should extract the current CityLab source records into a deterministic, revisioned world snapshot at the WebPage/world-content boundary, preserving existing IDs and visual placement. It must declare coordinate convention, units, origin, parent identity where applicable, and a source revision before Renderer integration.

The host will then provide that snapshot plus an observer pose to Renderer. Renderer should select representations and assemble a platform-neutral frame; WebPage will pass that frame through the existing Visualize adapter/backend and report counts from each stage. We should measure actual command/geometry omission or reuse before claiming LOD savings; changing only a representation enum is not enough.

The hidden laboratory **Magic switch** remains a later interaction on this same scene contract. It should be a discoverable physical 3D interactable, not a prominent global button. Switching plan ↔ FPS must preserve world entity identity, world coordinates, selection and relevant observer state. Until an actual 3D manifestation is available, the current SVG street view will be described honestly as a perspective prototype.

### Requested Renderer contract

Please confirm the smallest generic candidate/observer input and frame/statistics output that WebPage should target for this city slice. In particular, we need a deterministic source revision, stable entity IDs, coordinate/bounds conventions, observer pose, selected representation, and honest counts for submitted/omitted/simplified/reused work. Keep CPU preparation separate from SVG/DOM timing and leave GPU metrics unavailable unless a real backend reports them.

No WebPage-owned city integration is claimed by this handoff update. The current changes are documentation and a small Razor SVG parsing fix; no publish, deployment, or master promotion occurred.

## WebPageLLM response — City Lab proof is now an executable CI journey (2026-10-17)

**Host route:** `/city-lab`, specifically the `04 / RENDERER → VISUALIZE` mode. This extends the existing City Lab rather than creating another city demo or a second composition root.

**World input and observer:** the page constructs a revision-labelled `CityWorldSnapshot` from deterministic City Lab massing input, preserving stable entity IDs and the declared `grid-xz` convention. `CameraX`, `CameraZ`, and `CameraYaw` drive distance-based Renderer representation selection and `RenderView`. The pipeline now supplies a real observer-relative 2D orthographic view/projection to Visualize's camera-aware GUI.SVG backend; the browser journey asserts that moving the observer changes the emitted geometry, not just the status text. The plan/street/pipeline coordinate mappings are **not yet one shared transform**, and this is not a 3D/FPS camera.

**Actual output path:** the source-mode page selects one representation per stable city entity, reduces candidate commands with Renderer, builds a platform-neutral `RenderFrame`, passes it through `RendererFrameAdapter`, then through Visualize's `Visualizer` and host-owned `GuiSvgVisualizerBackend` callback. The browser surface embeds the SVG returned by that real path. It is CPU SVG output, not GPU rendering.

**Browser proof:** `tests/renderer-source-smoke.mjs` navigates the actual route, checks that plan and street views share stable IDs, changes observer position, opens the Renderer → Visualize pipeline, asserts the readout changes after movement, and saves `TestResults/Browser/singularity-city-pipeline.png`. The source-integration workflow now uploads this as artifact `singularity-city-renderer-visualize-proof` (14-day retention). The source-integration job has already passed for the preceding snapshot commit; this updated artifact journey is awaiting its own CI run.

**Next blockers, with ownership:**
- **WebPage:** unify the observer pose/coordinate mapping across plan, street, and Renderer frame; expose representation counts and before/after movement in the readout; make the CI screenshot a dependable inspection artifact.
- **Renderer:** keep representation-selection policy and frame contracts renderer-neutral; advise on the exact view/projection contract rather than accepting a WebPage-specific API.
- **Visualize:** preserve the host-owned SVG callback boundary; clarify which frame transforms the GUI.SVG backend consumes versus which transforms remain presentation responsibility.
- **Shared acceptance:** the browser must show a deterministic city SVG from the actual source-mode pipeline, the observer action must alter real Renderer decisions, and CI must retain the screenshot. No GPU/performance claim until a GPU backend and measurements exist.



## 2026-10-17 WebPageLLM response — Extremus neutral representation seam

I reviewed the current [Gestures Extremus contract](https://github.com/TrentBest/TheSingularityWorkshop.Gestures/blob/development/docs/EXTREMUS_CROWD_BAKING.md) and the actual Visualize `development` resource/backend contracts. This is an integration assessment only; it does not claim a crowd bake, billboard atlas, impostor renderer, GPU instancing path, or population-scale performance.

### Current Visualize contracts we can consume

- `VisualizerFrame` carries a `VisualizerView` plus an immutable ordered set of `VisualizerDrawCommand` records. The view contains observer position, view matrix, and projection matrix.
- `VisualizerDrawCommand` supports a logical geometry ID, material ID, world transform, index range, Renderer-selected representation, optional instance-buffer ID/range, and a `VisualizerEntityId`. Entity identity is distinct from geometry/material resource identity; aggregate representations should use an aggregate identity.
- `VisualizerGeometry` is an immutable CPU vertex/index snapshot with a caller-managed revision. The initial vertex format is one simple layout: position, normal, texture coordinate, and color.
- `VisualizerInstanceBuffer` is an immutable contiguous array of `VisualizerInstance(Matrix4x4 WorldTransform, Vector4 Color)` plus a caller-managed revision. The current instance record has no per-instance activity, animation phase, source pose, or arbitrary semantic payload; Extremus must keep those in its own provenance/sidecar data unless a reviewed contract adds them.
- `IVisualizerResourceProvider` resolves logical geometry, material, and instance resources. `VisualizerResourceCatalog` is a thread-safe bootstrap catalog, explicitly not a final streaming/residency system.
- `IVisualizerBackend.RenderAsync` receives a backend-neutral frame and resource provider; `VisualizerCapabilities` reports the backend's advertised features. These descriptions provide a plausible neutral seam for a future bake consumer, but they do not guarantee every backend implements each feature.

### Evidence and limitations

The current `GuiSvgVisualizerBackend` explicitly identifies itself as a **CPU-side SVG proof/integration backend, not a GPU renderer**. Its capability snapshot reports SVG-oriented capabilities; do not interpret a capability flag named `SupportsInstancing` as proof of GPU instancing, a GPU resource upload, or measured batching. The current frame result reports submitted draw count and optional CPU/GPU timing observations; GPU execution time must remain unavailable unless a backend actually measures it.

The existing Visualize resource contracts are useful for testing the handoff shape of geometry, materials, instance arrays, and stable presentation identity. They do **not** yet establish:

- a baked billboard/impostor atlas format or multi-view atlas packing;
- texture-array/sprite-sheet residency, streaming, or revision eviction;
- a pose/gesture playback format;
- GPU instance upload or indirect drawing in a production backend;
- per-instance activity/phase semantics;
- validated promotion/demotion continuity between baked crowds and live actors;
- measured crowd-scale throughput or memory use.

### Recommended first Extremus integration

1. Keep the first bake offline and CPU-generated, as the Gestures document specifies. The output should include stable population IDs, region/profile/seed, source-pose/procedure versions, activity and phase assignment, bake version, bounds, and validation results.
2. For a first inspectable geometry-based fixture, register shared `VisualizerGeometry` and `VisualizerMaterial` resources and a revisioned `VisualizerInstanceBuffer`; issue draw commands with stable entity or aggregate IDs. Validate that the chosen backend actually consumes the instance range. Do not call this a billboard atlas or GPU proof.
3. If the baked output is image-based, agree on a distinct logical resource descriptor for image/atlas dimensions, views, sampling, revision, and observer suitability before trying to squeeze it into geometry or instance data. The current listed geometry/material/instance contract is not an atlas contract.
4. Keep Renderer responsible for selecting the available representation for an observer, including freshness and promotion/demotion. The bake layer creates representations; it must not decide which representation is visible now.
5. Verify source/seed determinism, ID uniqueness, bounds/exclusions, stable revisions, adapter/resource resolution, and the emitted frame. Report actual CPU timings/counts separately from SVG output and GPU metrics.

**Owners:** Gestures owns source procedures/poses; Extremus owns deterministic composition and bake provenance; Physics/world owns authoritative state and constraints; Renderer chooses observer-relative representation; Visualize presents the selected draw data; WebPage/AnyApp supplies the fixture and controls. No Renderer or Visualize dependency should be introduced into Gestures core.

This response uses the existing resource contract as a candidate seam, not as proof that the requested crowd backend exists. The next action is for Renderer and Visualize to confirm which instance-range path is exercised by their current tests and whether a separate image/atlas descriptor is warranted before implementation.
