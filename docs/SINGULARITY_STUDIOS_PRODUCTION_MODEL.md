# Singularity Studios — Reusable Production Worlds

**Status:** product and data architecture direction; this is not a claim that a film-production toolchain already exists.  
**Owner:** WebPage integration/presentation; domain contracts belong in their owning Workshop packages.  
**Branch:** `development`. Do not publish or deploy from this brief.

## Current proof on development

The first reusable-data slice now exists in WebPage as an explicitly illustrative fixture:

- [Versioned giant-stage data](../TheSingularityWorkshop/wwwroot/Workshop/Forge/StreamingAssets/Experiences/singularity-studios-stage.json) defines the proposed stage dimensions, zones, reusable asset definitions, separate scene placements, rig definitions, camera placements, and lighting-rig placements.
- Two prop placements reference one canonical prop definition; two cameras reference the same scene identity. Camera views are observations of one scene, not copied worlds.
- [Singularity Studios page](../TheSingularityWorkshop/Pages/SingularityStudios.razor) exposes the JSON as a downloadable artifact. The schematic SVG remains a conceptual illustration and is not generated from this JSON yet.
- The browser smoke test fetches the artifact and checks its version, repeated prop reference, shared camera scene identity, and evidence disclosures.

**What this proves:** a small, host-served, versioned production-data example and an automated check of selected reuse/identity invariants.

**What it does not prove:** a canonical production-domain package, a JSON Schema or full validator, authoring/editing UI, save/reload workflow, renderer-generated SVG, camera-rig execution, acoustic simulation, or capacity to render the illustrative giant-stage dimensions. The dimensions are planning targets, not measured runtime limits.

## The ambition

**Singularity Studios is our own Hollywood studio—inside Singularity City.** It is a place where a creator can build a set, arrange actors and props, rig cameras and lights, choreograph action, capture takes, and reuse the resulting production data instead of rebuilding the same world for every shot or project.

The studio should be physically enormous, not a small editor room pretending to be a studio. It should support vast indoor sound stages for large battles, vehicle sequences, city-scale interiors, crowds, and other scenes that would be difficult or impossible to film in a conventional soundstage. Multiple stages, backlots, workshops, prop storage, motion/performance capture areas, sound stages, editing rooms, and production offices can grow from the same underlying facility data.

The key is not to build every scene from scratch. **Build reusable production data once; compose it into many scenes, takes, and experiences.**

## The production data ladder

Treat these as distinct reusable layers, with stable identities and explicit references. The exact schemas and owning packages must be reviewed before implementation; this is a domain model, not a new API declaration.

1. **Studio facility** — campus, sound stages, backlots, loading areas, workshops, storage, control rooms, and navigation between them.
2. **Stage / volume** — measured dimensions, floor and ceiling, grid, doors, rigging points, load limits, acoustic treatment, lighting infrastructure, access routes, and safety zones. Extremely large indoor stages are a first-class use case.
3. **Set / environment** — reusable architectural shell, terrain, streets, buildings, walls, doors, windows, platforms, scenery, and environmental dressing.
4. **Set dressing / prop library** — versioned reusable objects with dimensions, materials, attachment points, collision/interaction data where available, visual representation, and optional sound/physics metadata.
5. **Cast / agents / vehicles** — stable entity identity, appearance variant, movement/gesture capability, role, costume/skin, and scene-specific assignment. The production document references these; it does not duplicate the authoritative actor or vehicle definition.
6. **Rig library** — camera rigs, cranes, dollies, tracks, rails, mounts, robotic arms, stabilized rigs, lighting rigs, microphones/recorders, and virtual production equipment. Rig type and capabilities are data; each placement is a scene instance.
7. **Shot / camera setup** — camera identity, lens/sensor settings when supported, transform, target/focus, movement path, framing intent, time range, and any rig attachment. Distinguish a reusable camera/rig definition from a particular placement and from a recorded take.
8. **Cue / choreography / take** — time-indexed or event-linked actions, actor/vehicle movement, effects, lighting and sound cues, camera movement, take identifier, and provenance of generated or captured results.
9. **Sequence / production** — ordered shots and takes, continuity references, scene transitions, edit decisions, and export manifest.

A project should be able to reference a canonical set, prop, vehicle, rig, or actor and override only the scene-specific properties it owns. Avoid copying entire asset definitions into each scene. Overrides must be explicit, versioned, and inspectable.

## The giant indoor battle stage

The flagship studio demonstration should make scale obvious and useful:

- An indoor volume large enough for a large-scale battle or vehicle sequence, with a visible stage boundary and meaningful scale cues.
- Multiple zones that can be built, dressed, lit, simulated, and observed at different levels of detail.
- Reusable modular scenery: terrain sections, building shells, streets, walls, gates, towers, trenches, platforms, rubble, doors, and replaceable set pieces.
- Camera positions and movement rigs visible in a dedicated production/overhead view, with optional camera-frustum or shot-path overlays.
- Actor, vehicle, prop, and effect markers that preserve identity when the observer changes from the whole stage to one shot or close-up.
- Lighting, audio, effects, and physics treated as separate systems that reference the same scene entities and time/take records.
- Multiple cameras observing the same canonical scene simultaneously; camera views are observations, not duplicate worlds.
- A production view that can switch between floor plan, stage overview, selected camera, rigging plan, and close-focus inspection without changing scene identity.

The scale is a stress-test target, not a performance claim. Define measurable tiers and report what actually works. Never label a static drawing as a live crowd, simulated battle, functioning camera rig, or real-time render unless the implementation and test prove it.

## Authoring workflow

The first usable workflow should be narrow enough to prove end-to-end:

1. Choose a stage or load a saved stage definition.
2. Compose a set from reusable modules and place a few props.
3. Add actors/vehicles by reference, not by copying their authoritative definitions.
4. Add at least two cameras and a rig placement; show both on the production plan.
5. Define one simple action or movement cue and capture a deterministic take/preview.
6. Select a camera and view the same entities from its observation point.
7. Save the production description and reopen it, proving stable IDs, references, transforms, and versioned overrides survive.
8. Reuse the same set/prop/rig definitions in a second scene without duplicating the source assets.

At first, “camera” may mean a renderer-neutral viewpoint and an inspectable preview rather than a physical camera simulation or full cinematic renderer. “Rig” may begin as a documented transform/path constraint. Make those limits visible and extend them only when the relevant subsystem supports them.

## Architecture and ownership

| Concern | Owner / rule |
|---|---|
| Studio location and world identity | World/spatial domain contracts; stable identity independent of camera or route |
| Stage, set, prop, rig, shot, take and production data contracts | A focused production/studio domain package once a canonical contract is agreed; do not bury reusable domain logic in WebPage |
| Physics, collision, material response and physical effects | FSM_Physics and the relevant domain owners |
| Character actions and choreography | Gestures and actor/vehicle owning domains |
| State and scheduled process behavior | FSM_API |
| Composition and dependency closure | FSM_COS |
| Geometry generation and reusable visual primitives | Visualize |
| Scene submission, backend, visibility/LOD and camera observation | Renderer, according to its actual API |
| Audio assets and physical acoustic output | Explicit audio/provenance contract plus Physics/acoustics where supported |
| Authoring controls, camera selector, production plan and browser preview | WebPage host and its presentation layer |

Do not create a second simulation loop in WebPage. Do not make the renderer the source of truth for where the stage, prop, actor, or camera is. Do not assume a renderer API, GPU feature, physics solver, or camera rig already exists: first identify its actual contract, then build an adapter and tests.

## Reuse and persistence rules

- Give every reusable asset and every scene instance a stable ID; these are different identities.
- Reference assets by stable ID plus version or content hash where the repository contract supports it.
- Separate canonical asset data from placement/instance data, scene overrides, shot framing, and take results.
- Store transforms in an explicit parent/child hierarchy with documented coordinate conventions and units.
- Use deterministic seeds for procedural set dressing, crowds, debris, and other generated content; persist the seed and generator/version, not only the final preview.
- A saved production should be reopenable and diffable. Include a manifest of asset references, versions, generator settings, cues, and takes.
- Asset changes must not silently rewrite old productions. Define whether a reference pins a version or intentionally follows the latest.
- Keep captured media separate from generated geometry and simulation output, with provenance and licensing.
- Reuse should be demonstrated by loading one asset definition into two scenes and proving the source definition was not copied or mutated.

## Proof and acceptance

A first meaningful proof should test data and identity before attempting a giant cinematic showcase:

- Serialize and reload a stage, set, two camera placements, a rig path, props, and a small cast.
- Assert stable asset and instance IDs, valid references, coordinate/parent transforms, and versioned overrides.
- Reuse one set module and one prop in two scenes; changes to one placement must not mutate the canonical asset or the other placement.
- Show multiple camera observations of the same scene entities without creating duplicate actor/prop identities.
- Run deterministic replay for the same seed and cue sequence; compare the canonical scene/take output.
- Generate an SVG production plan from the same underlying stage/camera/rig data. Clearly mark conceptual overlays versus actual renderer output.
- Add an opt-in scale ladder for stage geometry, set dressing, actors, vehicles, cameras and effects. Report generation, update, visibility, memory and frame timing only where genuinely measured.
- Keep CI assertions deterministic; separate hardware-dependent performance measurements from ordinary tests.

## Delivery order

1. **Data and identity proof:** schemas/records, stable IDs, serialization, references, transforms, validation and round-trip tests.
2. **Production-plan SVG:** stage dimensions, set modules, props, camera placements, rig paths, and selected camera view derived from the same data.
3. **Small shootable scene:** a few actors/props, two cameras, one rig path, one deterministic cue/take.
4. **Reusable asset library:** versioned sets, props, vehicles, actors and rig definitions; demonstrate cross-scene reuse.
5. **Scale stress:** expand toward the giant indoor battle stage and crowds, with renderer/visualization measurements and clear limits.
6. **Sound and physical causality:** connect reference/captured audio and physical sound models with explicit provenance; do not imply the Physics package produces waveforms until it does.

**North star:** Singularity Studios is not a single movie scene. It is a reusable production environment where the studio, sets, props, actors, rigs, cameras, and takes become versioned data that can be composed again and again.
