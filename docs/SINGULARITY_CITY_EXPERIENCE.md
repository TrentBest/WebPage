# Singularity City Experience

![Living GUI documentation lens: question, model, implementation, visible manifestation, and proof](https://raw.githubusercontent.com/TrentBest/WebPage/development/docs/assets/living-gui-documentation-lens.svg)

> **Evidence class:** architecture illustration, not live runtime telemetry. Follow the subject-specific source, view, fixture, or test below; if none exists, record that gap rather than implying it is implemented.


## Purpose

Singularity City is the initial spatial anchor of Singularity World: a massive floating city in the Pacific Ocean. The Singularity Workshop remains at the city's center. The city is not a static backdrop; it is an editable, navigable environment in which Digitens live and work, and in which creators can compose and publish MicroBundle-backed Experiences.

This document separates **Current** implementation evidence from **Direction** and **Future** behavior. It must not be read as claiming that the complete persistent city, building editor, or casino runtime already exists.

## Current: source-backed foundations

- `SpatialWorldGuiBuilder` already documents a viewport-first world in which the visitor is the camera anchor, the avatar stays centered while the world moves, and spatial objects expose interaction points. It accepts room data, avatar coordinates, keyboard input, movement callbacks, and a room-interaction callback.
- `SpatialDigiten` represents a named citizen with a position, goal, route-planning operation, and discrete movement step. `SpatialDigitenCatalog.CreateStarterPopulation()` contains a small starter set of examples whose goals include the Workshop, transit, market, and research district.
- `SpatialDigitenPresenceService` manages connected user avatars as live presence. Its contract deliberately separates ephemeral presence from durable identity, civic office, buildings, economy, and world time.
- `DigitenMicroBundle` uses the repository's actual `FSM_API` types (`FSMHandle`, `IStateContext`) and defines a life/intent behavior progression. The Digiten is the actor; the FSM is behavioral machinery.
- `UserMicroBundle` owns visitor identity intent and exposes display name and avatar information separately from rendering.
- `WebMicroBundleProvider` implements the repository's `IMicroBundleProvider` and returns semantic manifestation effects (`Dormant`, `Trace`, `Breathe`, `Collapse`, `Remove`) with parameters. This is an existing example of a provider describing presentation intent while the host remains responsible for rendering.
- `SingularityCivicDigitenCatalog` contains a concrete catalog of civic Digiten roles, including Mayor Mara, councilors, clerk, treasury, planning, public works, housing, transit, and digital services.

Primary sources:

- [`SpatialWorldGuiBuilder.cs`](../TheSingularityWorkshop/Gui/SpatialWorldGuiBuilder.cs)
- [`SpatialDigitenSimulation.cs`](../TheSingularityWorkshop/Workshop/Spatial/SpatialDigitenSimulation.cs)
- [`SpatialDigitenPresenceService.cs`](../TheSingularityWorkshop/Workshop/Spatial/SpatialDigitenPresenceService.cs)
- [`DigitenMicroBundle.cs`](../TheSingularityWorkshop/Workshop/Agents/DigitenMicroBundle.cs)
- [`UserMicroBundle.cs`](../TheSingularityWorkshop/Workshop/MicroBundles/UserMicroBundle.cs)
- [`WebMicroBundleProvider.cs`](../TheSingularityWorkshop/Workshop/MicroBundles/WebMicroBundleProvider.cs)
- [`SingularityCivicDigitenCatalog.cs`](../TheSingularityWorkshop/Workshop/Spatial/SingularityCivicDigitenCatalog.cs)
- [`Digiten Architecture`](DIGITEN_ARCHITECTURE.md)
- [`Singularity Universe`](SINGULARITY_UNIVERSE.md)
- [`Experience Theory`](EXPERIENCE_THEORY.md)

## Direction: one city, multiple renderers

The zoomable city-to-room navigation model, inhabitable vehicle experiences, Digiten data cards, civic life, and future hero/villain composition are specified in [Singularity City: Zoomable World and Reusable Experiences](SINGULARITY_CITY_ZOOMABLE_WORLD_AND_EXPERIENCES.md). The current map surfaces are separate foundations; the document defines the target shared-world contract and a staged implementation path.

The city must have one authoritative spatial and semantic model. A floor plan, city overview, building facade, interior, and table-level perspective are views over that model. Switching renderers must not create a duplicate person, building, seat, or transaction.

The Workshop is the fixed center anchor. Other city objects may be repositioned, replaced with another building archetype, or reconfigured through explicit authoring capabilities. A city editor should express changes as edits to the world model, not as ad hoc DOM mutations or hard-coded per-page coordinates. The Digitens page's current city layout explorer is only a browser-local concept sketch; it does not persist changes or modify runtime city objects.

### World and renderer boundaries

1. **World / spatial model** owns identity, position, object kind, parent/child relationships, walkability, and world-scale constraints.
2. **MicroBundle composition** describes which capabilities, providers, content, and renderers are available for an Experience. A bundle can reference other bundles, including cycles; resolution must reuse an already discovered canonical reference rather than recursively constructing it forever.
3. **Provider contracts** publish the information and capabilities that an environment or interactable exposes. Providers should be discoverable and inspectable; consumers should not need a hard-coded list of every building or machine in the universe.
4. **Interactable affordance** gives a consistent visual and accessible signal for hover, keyboard focus, proximity, and activation. It delegates to the target's capability rather than impersonating that capability.
5. **Navigation** resolves world-scale destination, district/domain entry, and local destination in stages. A building can own specialized interior navigation without forcing the Digiten to know every room in every building.
6. **Renderer selection** presents the most relevant view of the same state. For example, the floor renderer follows the visitor, while entering a poker experience selects a seat and then requests the table-level perspective.
7. **FSM_API** orchestrates stateful behavior where appropriate. A Digiten is not an FSM, and a renderer should not become the authority for game rules, seating, citizenship, or economy.

These responsibility boundaries are architectural direction, not declarations of new interface names or signatures. Any implementation must use the current public contracts of FSM_API, FSM_COS, MicroBundleDomain, and the GUI packages.

## Interaction example: a visitor joins a poker table

1. The floor renderer identifies the clicked table and invokes the table's public interaction capability.
2. The table capability checks its actual seat state and returns an available seat and arrival target, or a clear unavailable result.
3. World navigation moves the avatar to that target. The camera continues following the avatar.
4. Arrival selects the table renderer and the same seat identity. Other players, dealer, cards, chips, and pot are rendered from the shared table state.
5. Wagers and outcomes are submitted to the responsible game/economy capability; the renderer does not invent authoritative results.
6. Leaving restores the floor renderer at the preserved world position.

## Future: scale without faking scale

The city should become super-massive through hierarchical spatial composition, locality, compact state, and level-of-detail—not by asking the browser to render every building and every citizen at full detail at once. Observed and nearby regions can receive richer manifestations, while distant districts use compact representations or summaries. Persistent identity and world state remain independent of whether a human is currently observing a Digiten.

Authoring should eventually support replacing building types, moving objects, inspecting their providers, composing MicroBundles, previewing the resulting Experience, and publishing through the Workshop's real publishing path. Until those paths are wired to authoritative providers and persistence, interfaces must label local previews as previews and must not imply changes were saved.

## Acceptance gates

- The Workshop anchor cannot be moved or replaced by ordinary city-layout edits.
- The same actor and object identities survive view transitions.
- Camera-follow keeps the avatar centered during movement.
- Interactables use public capability/provider contracts; renderers do not hard-code game rules.
- A poker seat is confirmed by the table capability before the avatar moves into it.
- City edits are visibly labeled as local drafts until a persistence/publication provider confirms success.
- Reduced-motion preferences and keyboard-operable focus/activation are supported.
- CI/build and a manual browser pass are recorded before describing the feature as working.

## Current claim boundary

The repository contains a viewport-first spatial GUI builder, starter spatial Digiten simulation types, a live-presence service, a Digiten micro bundle using FSM_API, provider-based manifestation code, and a civic Digiten catalog. The full persistent mega-city, editable server-backed city model, authoritative poker seat reservation, and connected casino economy are not established by those files alone.