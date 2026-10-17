# Workshop Welcome — Interactive Floorplan Experience

**Status: first interactive concept integrated on development.** The Welcome destination now renders WorkshopFloorplan in the existing WebPage host. The drawing is an SVG schematic, not a game-engine scene, surveyed building plan, or simulated physical workshop.

## Experience contract

The visitor's first two choices are intentionally simple:

- **Welcome** enters the Workshop floor plan and collapses the hub navigation.
- **About Us** remains an always-available direct route at /about-us, including compatibility with the existing Unity package link.

Welcome is a place, not a long scrolling brochure. The floor plan fills its available panel and keeps its map, room inspector, and controls in a fixed viewport. The visitor discovers the Workshop by pointing at rooms, reading concise explanations, and choosing a facility.

## First floor plan

- **Central Hall** — welcome and orientation.
- **The Forge** — links to the existing live FSM_API lab. The lab is a real FSM_API-backed interaction, separate from the floor-plan illustration.
- **Assembly Benches** — explain MicroBundles and the FSM_COS composition boundary. A future installed bundle may contribute a facility only through an actual, discoverable capability contract.
- **Observatory** — establishes the intended celestial assembly workflow: choose planets, moons, and bodies from parts buckets, then place and resize them. This is a concept today, not a functioning solar-system editor.
- **World Globe** — communicates that world-level terrain work happens through a selected globe, not by editing the solar-system container. Interactive globe manipulation remains planned.
- **Cartographers' Drafting Wing** — maps belong to expeditions, authored territories, or commissioned cartographers. This illustrates the intended workflow; it does not claim a hiring/economy system exists.

The visual vocabulary is a dragon-warrior-inspired stronghold drawn as a technical plan: dark blueprint field, measured lines, stone/fortification silhouette, forge heat, orbits, and drafting tables. Keep labels legible and the panel non-scrolling; adapt the SVG viewBox and room inspector on small screens rather than adding page scroll.

## Voice and sound

The component includes an explicit **VOICE ON/OFF** control. When enabled, it uses browser speechSynthesis to speak the welcome and the hovered room's explanation. This is an accessible prototype for spoken discovery, not a recorded Workshop voice, not SFX Studio output, and not evidence of Physics-driven audio.

Opening sound effects are **not yet wired**. Do not generate a generic chime and label it as the SFX Studio/Physics system. Coordinate the actual sound asset/contract with the SFX/virtual-production owner and Physics before adding sound:

1. SFX Studio owns authored sound identity, source assets, and any production mix/voice direction it actually exposes.
2. Physics may provide event/impact/acoustic context only through a real contract; WebPage must not invent physical audio events or simulate a physics engine to trigger them.
3. WebPage owns the visitor gesture, mute/voice preferences, playback presentation, and graceful fallback when browser audio is unavailable.
4. Audio must begin only after a visitor gesture where browser policy requires it, and mute must stop speech/effects immediately.
5. Document the exact asset source, licence, format, trigger, and integration test before claiming the opening sound is implemented.

Suggested sound design brief for the coordinating owner: short forged-metal resonance, low warm room tone, subtle ember/air texture, and a restrained transition swell. Avoid a loud fantasy sting or constant loop; this should feel like entering a working place, not a game trailer.

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
