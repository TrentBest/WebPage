# Singularity City Hidden Martial-Arts Dojos — WebPageLLM Handoff

## Intent

Give Singularity City a network of small, distinctive martial-arts dojos tucked into the city fabric. They should reward exploration: a visitor might discover a training room behind a market, above a transit concourse, in a converted service level, on a quiet roof, inside a residential courtyard, or at the edge of an industrial district. Some entrances are discreet; the places themselves should still be legible and welcoming once discovered.

This is a city-life and choreography experience—not a claim that a complete combat simulator, martial-arts curriculum, or autonomous fighting system already exists.

## Experience goals

- Make dojos feel embedded in a lived-in city rather than presented as a detached list of menu destinations.
- Give each location a clear identity through architecture, soundscape, lighting, local context, training equipment, and the way people enter and leave.
- Support both obvious public schools and tucked-away specialist spaces. “Hidden” means discovered through environmental cues and exploration, not inaccessible, randomly generated, or dependent on real-world trespassing.
- Let visitors inspect a dojo, observe a short practice, discover its style and training purpose, and optionally enter a practice or choreography view.
- Make the city feel varied: compact neighborhood rooms, community-center halls, rooftop practice spaces, industrial training bays, and quiet courtyard schools can coexist.
- Keep the first slice small and testable. A few authored, distinct locations with stable identities are better than a large fabricated directory.

## Proposed first vertical slice

1. Add a data-driven dojo catalogue with stable IDs, district/anchor references, style tags, discovery cues, access status, opening/training schedule metadata, and an evidence/status field.
2. Place three initial authored dojo examples in different urban contexts:
   - **Market-side school** — a modest upstairs room reached through a public market passage.
   - **Transit-understructure dojo** — a quiet training hall in a repurposed concourse/service space, with a safe public entrance.
   - **Courtyard school** — a neighborhood practice room screened by a planted courtyard.
3. Give each entry a short discovery trail based on visible city clues (signage, repeated emblem, foot traffic, sound, lighting, or a public notice). Do not require arbitrary pixel hunting.
4. Provide an inspectable dojo card/detail panel: name, style, district, training focus, schedule, discovery clue, accessibility, and current implementation status.
5. Add a short observation/choreography fixture only where the underlying Gesture capability is available. Until then, label any authored motion as illustrative and do not imply it is a solver-backed combat simulation.
6. Include a route back to the city and preserve the current Workshop navigation and viewport constraints.

These three locations are design fixtures, not assertions that the current city model already contains them. Bind them to canonical district/parcel/transport identities when those APIs exist; do not create a parallel city geography inside a page component.

## Style and training representation

Use style as data, not a hard-coded Razor branch. Initial tags may include (for example) karate, judo, taekwondo, boxing, wrestling, aikido, kung fu, or mixed training, but do not imply that a label alone captures a whole school or lineage. Give each dojo a specific training emphasis—footwork, balance, throws, forms, controlled sparring, conditioning, partner drills, or movement research—and allow schools to teach more than one discipline.

Represent people as participants in a practice, not as combat props. The first demonstration should prioritize consent-based partner work, clear practice boundaries, controlled intensity, and visible recovery/rest states. Do not use injury, violence, or defeating a person as the sole success signal.

## Gesture / Physics / WebPage ownership boundaries

- **WebPage** owns discovery, city navigation, the dojo catalogue presentation, visitor controls, and the visible experience.
- **Gestures** owns reusable pose sequences, movement/choreography descriptions, parameterized variation, and later the validated execution/evaluation boundary for gesture channels.
- **FSM_API** owns state-machine primitives and lifecycle semantics.
- **FSM_COS** composes declared capabilities; it is not a city simulator or page-level execution loop.
- **Physics** owns authoritative physical calculations and observations when a suitable model is implemented. WebPage must not duplicate collision, force, energy, or motion calculations in browser code.
- **Singularity City / shared city model** owns canonical district, building, parcel, and transport identities when available. A dojo is a tenant/use of a place, not a second independent map.

Do not add a dependency on an unpublished Gestures or Physics package just to make the first page compile. Use a clearly labelled fixture adapter until a supported integration contract exists, then replace it with the owning package's real output and tests.

## Discovery and city integration

- Prefer several authored, coherent clues over a generic “nearby dojo” marker on every building.
- Keep accessibility and public-entry information visible after discovery. Hidden architecture must not conceal safety or access expectations.
- Let the user discover locations organically, but also provide an accessible directory/search path so the experience does not depend entirely on visual perception or perfect navigation.
- Do not invent real-world dojo businesses or imply affiliation with actual schools. These are fictional Singularity City locations unless explicitly sourced otherwise.
- Coordinate with the existing [Singularity City living-infrastructure direction](https://github.com/TrentBest/WebPage/blob/development/docs/integrations/SINGULARITY_CITY_LIVING_INFRASTRUCTURE.md) when that document is present on the integration line; this dojo network should complement transit, districts, public spaces, and city life rather than define those systems again.

## Acceptance checks

- Stable IDs and deterministic fixture output across repeated runs.
- Every dojo points to a known or explicitly provisional city anchor; no duplicate map/catalogue authority.
- At least three locations differ in spatial context and discovery clue.
- Catalogue/detail view exposes style, training focus, access, and implementation status.
- Keyboard and narrow-viewport navigation remain usable; discovery has a nonvisual alternative.
- Any motion demo clearly identifies whether it is authored fixture data or produced by a real Gesture integration.
- No WebPage-side physics calculations and no unsupported package dependency.
- Tests cover identity stability, catalogue completeness, discovery-route resolution, and fixture/provenance labels.
- Documentation separates implemented, illustrative, and planned capabilities.

## Delivery sequence

1. Review the current WebPage city/manifest/navigation models and reuse their extension points.
2. Add the focused catalogue model and tests.
3. Implement the three-location discovery slice and its accessible directory/detail panel.
4. Add one short, explicitly illustrative training sequence if a real Gesture adapter is not yet available.
5. When Gesture integration is ready, replace the fixture sequence with the package-owned choreography and test the handoff end-to-end.
6. Capture browser smoke evidence for discovery, detail view, navigation back to the city, and small-screen layout.

## Out of scope for this slice

A full martial-arts combat engine, ranking/tournament economy, autonomous rival behavior, real-world school directory, injury simulation, and a complete city parcel/transport simulator. These can be considered separately after the first vertical slice proves the visitor journey.

**Status:** WebPageLLM implementation brief. This document is a design handoff; it does not claim the experience is implemented, tested, merged, deployed, or published.
