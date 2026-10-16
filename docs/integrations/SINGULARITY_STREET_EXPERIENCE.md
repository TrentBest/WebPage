# Singularity Street — Neighborhood Experience Proposal

**Status:** proposed first placement and experience brief; not a claim that the neighborhood or interactive train system already exists.  
**Owner:** WebPage experience composition, with Physics/Gestures/FSM integration only through their owned contracts.  
**Destination domain:** Singularity City, not the Workshop.

## Intent

Singularity Street is a welcoming, explorable neighborhood where visitors can walk around, meet original digital residents, discover everyday places, and interact with trains and small journeys. The emotional target is the delight of exploring a richly connected model neighborhood and watching a railway world unfold. It should feel warm, playful, curious, and alive without copying another property's protected expression.

This is an iterative Workshop experience for its creator to review first. It does not need to be treated as a public launch or monetized product.

## Where it belongs

Use the existing spatial composition architecture rather than creating a second map or a standalone navigation system.

- **Domain:** `SpatialDomain.SingularityCity`.
- **Proposed stable place ID:** `city.singularity-street`.
- **Proposed scene ID:** `singularity-street`.
- **Spatial anchor:** a distinct neighborhood parcel in the wider city/universe, with a comfortable walking route to a transit stop. Keep it separate from the fixed Workshop anchor and avoid placing it inside a lab or tooling destination.
- **Initial placement recommendation:** use an available southern neighborhood parcel in the open universe canvas as the first planning anchor, then refine its exact bounds against the live city layout and transit routes. The current `SpatialUniverseManifest.SingularityUniverse` includes an open `south-expanse` parcel; this is a candidate composition boundary, not a claim that it is already a city district.
- **Arrival:** expose it as a normal city Experience. Add a Grand Central destination/platform only through the existing transit manifest pattern after the Experience route resolves successfully.

Relevant existing architecture:
- `TheSingularityWorkshop/Workshop/Spatial/README.md` — manifest → composition → generic renderer → Experience.
- `TheSingularityWorkshop/Workshop/Spatial/SpatialPlaceCatalog.cs` — authoritative `SpatialPlace` records, scene identity, entry point, and Workshop/City separation.
- `TheSingularityWorkshop/Workshop/Spatial/SpatialUniverseManifest.cs` — reserved island core and intentionally open parcels.
- `TheSingularityWorkshop/Workshop/Spatial/SpatialTransitManifest.cs` — Grand Central destination and platform definitions.

## First visit: a living street, not a static diorama

Start with one compact, coherent neighborhood:

1. A readable main street with sidewalks, small original storefronts, homes, a pocket park, and a neighborhood station.
2. A small, distinctively designed train set with a visible route, platforms, signals, and a destination board.
3. A welcoming public square where visitors can orient themselves and choose what to explore.
4. A handful of original digital residents with simple, inspectable activities and clear interaction affordances.
5. One short rail journey the visitor can start, observe, and revisit; keep the train, route, and station identities stable.
6. A compact neighborhood map or wayfinding panel that shows the visitor's location and reachable places without taking over the experience.

The street should have enough detail to invite exploration but remain a bounded, testable vertical slice. The first pass can use authored scene data and deterministic route snapshots. It must distinguish an authored animation/fixture from a train simulation with validated motion, signaling, collision, dispatch, or passenger logic.

## Train interaction ladder

- **A — Authored route:** display the train and a clearly identified sample route. Route progress may be deterministic presentation data; do not call it a railway simulation.
- **B — Interactive journey:** visitor can board, select a destination, depart, ride, and disembark through real Experience state transitions. If motion is still a route snapshot, label that boundary honestly.
- **C — Operational railway:** add train movement, station dwell, signals, route reservations, and conflicts through a defined model and tests.
- **D — City integration:** connect the neighborhood station to Singularity Station, maglev, aerial taxis, and broader public transit through shared route/destination identities rather than duplicated transport logic.
- **E — Living neighborhood:** residents, shops, schedules, events, and visitor activities respond to state at appropriate FSM_API process cadences and event-horizon LOD. Distant activity should not run at the same rate as nearby interactions.

Do not block the visual neighborhood on the full railway model. Make the first place delightful, then let each new capability become real behind a stable boundary.

## Interaction principles

- Keep the visitor's position obvious; use a centered avatar or clear “you are here” indicator consistent with existing city experiences.
- Prefer discoverable interactions: doors, station boards, train controls, resident conversations, shop windows, and route markers should visibly react on hover/focus and explain what clicking does.
- Keep the main scene primary and avoid forcing the visitor through long pages or a large scroll area.
- Use accessible labels, keyboard navigation, reduced-motion-friendly behavior, and readable contrast.
- Keep state authoritative in the owning domain; presentation controls request transitions rather than secretly mutating unrelated city systems.

## Originality and IP boundary

Use the name **Singularity Street** as the working title for an original neighborhood. The creative reference is the broad idea of a friendly, explorable street and an intimate model railway—not the copying of specific expression.

Create original:
- street/building layouts, architectural silhouettes, color system, signs, typography, characters, names, voices, costumes, music, sound effects, train shapes, station design, storylines, and interactions;
- original scripts, dialogue, educational moments, and resident relationships;
- original assets or assets with documented licenses.

Do not use Sesame Street or Mister Rogers branding, their logos, signature characters, recognizable set/prop arrangements, theme music, dialogue, or confusingly similar character designs. Do not market the experience as affiliated with or endorsed by those properties. A disclaimer is not a substitute for original design. This is design guidance, not a legal determination; obtain qualified review before any public/commercial launch if the resemblance or naming remains a concern.

If a rights-holder makes a credible takedown request, preserve the request and relevant asset/source references, disable the specifically challenged material promptly while it is reviewed, and replace or remove it rather than arguing in the experience. Do not promise rights or revenue sharing on behalf of any third party; any license or commercial arrangement must be explicitly agreed by the actual rights-holder.

## Integration boundaries

- **WebPage:** owns the neighborhood composition, camera, visitor controls, wayfinding, authored scene, and presentation.
- **FSM_API:** owns state/lifecycle semantics for stateful residents, station journeys, and experience transitions where integrated.
- **FSM_COS:** composes declared capabilities at the proper boundary; it is not a second simulation loop.
- **Physics:** owns physical motion/collision/rail or vehicle dynamics only when a concrete supported contract exists. Do not duplicate a solver in Razor.
- **Gestures:** may provide reusable resident/crew choreography when its supported contract is available.
- **Visualize/Renderer/GUI.SVG:** downstream presentation consumers as the current integration proves them; don't claim GPU-backed rendering where the host uses authored SVG or fixture data.

## Proposed first implementation slice

1. Inspect the current spatial map and existing transit experience before assigning exact coordinates.
2. Add a dedicated neighborhood manifest and Experience composition using the existing generic spatial pattern.
3. Register `city.singularity-street` only when the destination can resolve to a real scene.
4. Place the neighborhood in the open southern universe parcel as the first layout proposal, leaving the Workshop anchor untouched and maintaining a walkable/transit connection.
5. Add one original train, one station, one short route, one pocket park, a few original storefronts/homes, and a small number of resident interaction points.
6. Add tests for stable identities, valid bounds/non-overlap, reachable entry point, route consistency, accessible navigation, disposal/cleanup, and honest fixture-vs-simulation labels.
7. Only then add the station to the transit destination manifest and connect wider transit.

## Acceptance checks

- The neighborhood is a Singularity City destination, not a Workshop tool.
- It uses the current spatial composition/navigation architecture and does not create a parallel map.
- The Workshop anchor remains fixed.
- Visitors can identify where they are, reach the station, and activate the first train journey.
- The first route is deterministic and its implementation level is described accurately.
- All characters, sets, signage, train designs, sound, and stories are original or properly licensed.
- No external property's logos, signature characters, music, or distinctive assets are included.
- No Physics, Gestures, FSM_COS, rendering, persistence, or economy capability is claimed before the relevant integration exists.
- No merge, publication, or deployment is part of this proposal.

## Immediate next step

Build the small original street-and-station Experience in WebPage, using the south-expanse parcel as the planning anchor. Keep the first train journey bounded and inspectable; grow it toward the larger Singularity City transport network after the first visit is coherent.
