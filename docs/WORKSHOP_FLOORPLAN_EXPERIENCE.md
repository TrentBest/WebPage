# Workshop Welcome — Interactive Floorplan Experience

**Status: first interactive concept integrated on development.** The Welcome destination now renders WorkshopFloorplan in the existing WebPage host. The drawing is an SVG schematic, not a game-engine scene, surveyed building plan, or simulated physical workshop.

## Experience contract

The visitor's first two choices are intentionally simple:

- **Welcome** enters the Workshop floor plan and collapses the hub navigation.
- **About Us** remains an always-available direct route at /about-us, including compatibility with the existing Unity package link.

Welcome is a place, not a long scrolling brochure. The floor plan fills its available panel and keeps its map, room inspector, and controls in a fixed viewport. The visitor discovers the Workshop by pointing at rooms, reading concise explanations, and choosing a facility.

## First floor plan

- **Central Hall** — welcome and orientation.
- **The Singularity Laboratory** — now contains the existing live FSM_API lifecycle demonstration as its first station. The broader catalogue and MicroBundle prescription/sandbox workflow are planned, not yet universal runtime capabilities.
- **The Forge** — now opens the existing FSM Forge through `/explore?room=forge`. It provides real FSM_API-backed construction scaffolds for states, transitions, conditions, and lifecycle actions, with a live preview. Authoring complete MicroBundles and Experiences remains a broader future capability.
- **Assembly Benches / Forge** — explain MicroBundles and the FSM_COS composition boundary. A future installed bundle may contribute a facility only through an actual, discoverable capability contract.
- **Observatory** — establishes the intended celestial assembly workflow: choose planets, moons, and bodies from parts buckets, then place and resize them. This is a concept today, not a functioning solar-system editor.
- **World Globe** — communicates that world-level terrain work happens through a selected globe, not by editing the solar-system container. Interactive globe manipulation remains planned.
- **Cartographers' Drafting Wing** — maps belong to expeditions, authored territories, or commissioned cartographers. This illustrates the intended workflow; it does not claim a hiring/economy system exists.

The visual vocabulary is a dragon-warrior-inspired stronghold drawn as a technical plan: dark blueprint field, measured lines, stone/fortification silhouette, forge heat, orbits, and drafting tables. Keep labels legible and the panel non-scrolling; adapt the SVG viewBox and room inspector on small screens rather than adding page scroll.

## Player-local arrival and camera policy

**Design requirement: entering a large place must not automatically frame the entire building.** The building may span a campus, city block, megastructure, or effectively unbounded complex; its size must not dictate the player's initial framing.

Use the Diablo-style area-transition principle as a presentation reference, not as a requirement to copy its exact camera or art direction:

- **Spawn at a meaningful local anchor.** The entry transition resolves to the player's actual arrival position (for the Workshop, initially the threshold / central-hall arrival anchor), not the building's geometric center unless that is truly where the player arrives.
- **Frame the player and nearby playable space.** The initial camera should show the player, the immediate room, nearby exits/interactables, and enough context to understand where they are. Do not zoom out to fit the whole structure.
- **Keep world scale independent of camera scale.** Camera translation and zoom change perception only; they must never rewrite world coordinates, room dimensions, or object positions. The existing SpatialCamera type in TheSingularityWorkshop.Gui already expresses this separation and avatar-centering policy; future in-world presentation should use that contract rather than inventing a second coordinate model.
- **Reveal the complex progressively.** Let movement and transitions expose adjacent rooms. Use a separate map/overview affordance for orientation; an overview is a deliberate mode, not the default arrival shot.
- **Make transitions local and legible.** A door/portal/route transition should resolve the destination area and spawn anchor, briefly transition the view, then settle on the new local camera framing. Preserve player identity and appropriate state across the transition.
- **Scale beyond one screen.** Large complexes must be represented as connected areas or spatial regions, not as one giant image that shrinks the player to a speck. Streaming, LOD, and update cadence are implementation concerns for the rendering/spatial owners; WebPage should not fake those capabilities in the schematic.

### Current versus intended behavior

The current WorkshopFloorplan is explicitly a **schematic orientation map** (viewBox="0 0 1200 720"), not a live player-controlled scene. Its overview is appropriate for choosing a destination and must not be misrepresented as the eventual in-world camera. The local-arrival rule applies when a destination opens as an explorable spatial experience; the floorplan remains a separate map/selection surface unless it is deliberately given a local-room mode with working navigation back to the overview.

### Acceptance checks for an explorable spatial destination

- Initial arrival frames the player's spawn anchor and nearby navigable space; the whole building is not fitted to the viewport by default.
- Changing zoom preserves the player's screen-space anchor and does not mutate world coordinates.
- A map/overview is explicitly invoked and can return to the player-following/local view.
- Moving to another area transitions to that area's spawn anchor rather than recentering on the entire complex.
- Large areas can extend beyond the viewport without breaking navigation, selection, or interaction.
- Tests cover camera centering, zoom clamping, area-transition anchoring, and the separation between world coordinates and camera transforms.

## Voice and sound

The component includes an explicit **VOICE ON/OFF** control. When enabled, it uses browser speechSynthesis to speak the welcome and the hovered room's explanation. This is an accessible prototype for spoken discovery, not a recorded Workshop voice, not SFX Studio output, and not evidence of Physics-driven audio.

Opening sound effects are **not yet wired**. The existing [Physics free-field observation](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/docs/experiments/acoustic-observation.md) calculates distance, propagation delay, estimated received level, and direction for an idealized source/listener pair; its run record and SVG are model metadata, **not a generated or playable waveform**. Singularity Studios currently presents an explicitly labelled external reference recording, not a Workshop arrival cue. Do not reuse it as if it were the finished sound design or generate a generic chime and label it as the SFX Studio/Physics system. Coordinate the actual sound asset/contract with the SFX/virtual-production owner and Physics before adding sound:

1. SFX Studio owns authored sound identity, source assets, and any production mix/voice direction it actually exposes.
2. Physics may provide event/impact/acoustic context only through a real contract; WebPage must not invent physical audio events or simulate a physics engine to trigger them.
3. WebPage owns the visitor gesture, mute/voice preferences, playback presentation, and graceful fallback when browser audio is unavailable.
4. Audio must begin only after a visitor gesture where browser policy requires it, and mute must stop speech/effects immediately.
5. Document the exact asset source, licence, format, trigger, and integration test before claiming the opening sound is implemented.

Suggested sound design brief for the coordinating owner: short forged-metal resonance, low warm room tone, subtle ember/air texture, and a restrained transition swell. Avoid a loud fantasy sting or constant loop; this should feel like entering a working place, not a game trailer.

## Laboratory and Forge boundary

The floorplan is the visitor's orientation surface; the **Singularity Laboratory** is where visitors discover demonstrations as stations, compose existing MicroBundles into an experiment, prescribe the bundle set for a sandbox, and inspect evidence. The **Forge** is where creators construct reusable MicroBundles and Experiences. This is a deliberate distinction: experimentation with existing ingredients should not require entering the authoring environment.

See [Singularity Laboratory — Station and Experiment Contract](SINGULARITY_LABORATORY_STATIONS.md) for the station families, evidence rules, current demonstration maturity, and implementation gates. The current FSM_API lab still opens from the Forge room for continuity, but that placement is transitional and should move only when the Laboratory has a verified replacement route.

## Composition path for future facilities

Do not hardcode each future room into WebPage forever. The SVG currently establishes the visual grammar and a small set of authored rooms. The next architectural seam is a renderer-neutral facility contribution contract owned at the appropriate shared capability boundary. Each installed MicroBundle could declare a stable facility ID, display name, placement/anchor, icon or geometry reference, hover description, activation route/action, capability dependencies, and maturity/evidence label.

A manifest entry alone must not claim the tool is executable. Only render a facility as installed when the host has actually resolved the bundle and the facility provider. Unknown providers should be omitted or shown as clearly unavailable—not represented as functioning tools. WebPage presents the floor plan; FSM_COS composes bundle dependencies; the domain bundle owns the tool's semantics; Visualize/Renderer may own reusable manifestation when an agreed API exists.

## Celestial and map editing progression

Keep these as linked but distinct contexts:

1. **Solar-system arrangement:** manipulate the collection of celestial bodies and their relative placement/scale.
2. **Globe selection:** choose a world and inspect/spin its globe representation.
3. **Drafting/map selection:** choose an authored map, expedition map, or cartographer-produced map.
4. **Terrain/world editing:** edit land and geography within that map's world coordinate contract.

The current SVG only communicates this relationship. Future steps require stable celestial/world/map IDs, coordinate-frame contracts, persistence rules, and real editing operations. Do not turn the illustration into fake draggable planets or fake cartographer data before those contracts exist.

## Acceptance checks

- Selecting Welcome collapses navigation and shows the floor plan in the primary panel.
- The floor plan does not require vertical page scrolling at desktop and compact viewport sizes.
- Every room is keyboard-focusable and can be selected by Enter/Space.
- Voice is opt-in and can be cancelled; without speech support, visual descriptions still work.
- Forge entry opens the actual existing FSM lab; leaving it returns to the floor plan.
- /about-us remains directly addressable and its existing content is unchanged.
- UI labels distinguish current working interactions from conceptual workflows.
- No sound/physics/GPU/persistent-world claim is made without the owning implementation and evidence.

## Immediate next work

1. Validate the component with repository-source CI and a browser screenshot at desktop and compact viewport sizes.
2. Add a direct Welcome journey smoke test for nav collapse, hover/keyboard inspection, voice toggle, Forge entry/exit, and About Us URL.
3. Coordinate with the SFX Studio and Physics owners for an explicit audio asset/event contract before wiring effects.
4. Define the MicroBundle facility-contribution contract with the composition/domain owners before implementing dynamic room installation.
5. Turn the Observatory → Globe → Drafting path into real editable experiences only when stable world contracts and state persistence are ready.
