# Casino World: Micro-Bundle Composition and View Transitions

Status: design direction for the WebPage Economy prototype. This document describes intended behavior; it does not claim that the current browser prototype is connected to production economy or runtime services.

## Experience principle

The casino is one world with persistent spatial and interaction state. A floor-plan renderer, a poker-table perspective, and a slot-machine elevation are alternate views of that world—not separate scenes with duplicated game state.

The player avatar has a world position and orientation. In floor-plan mode, the camera follows the avatar so the avatar remains near the viewport center while the environment scrolls. A persistent YOU ARE HERE marker identifies the avatar. Pointer selection is a destination request; keyboard/controller input supports direct movement. Movement respects walkable space and collision/interaction boundaries.

## Micro-bundle responsibilities

- **Casino experience bundle** — composes the casino environment and its referenced capabilities.
- **Floor Plan renderer bundle** — turns environment-provided spatial data and interactables into a navigable top-down representation. It owns presentation and camera-follow behavior, not casino rules.
- **Interactable affordance bundle** — presents hover/focus/nearby affordances consistently (for example, a breathing outline, focus ring, prompt, and accessible name). It delegates the actual interaction to the selected target.
- **Facade resolver bundle** — supplies resolvers that map environment and composition inputs to appropriate facade/content choices. The environment may influence what content is resolved; resolution should remain explicit and inspectable.
- **Poker table bundle** — exposes table capabilities such as seat availability and the interaction entry point. It is authoritative for whether a seat is available; the floor renderer must not guess.
- **Seat / player-position capability** — identifies an available seat and its world-space approach/arrival position. A successful join moves the avatar there before requesting the table-level view.
- **Digitens bundle** — supplies role-aware actors such as dealers, operators, cashiers, and roaming staff, with movement/behavior presented by the active renderer.
- **Economy / ledger providers** — expose virtual or authoritative balances, wagers, and events through explicit providers. The current HTML prototype's counters remain illustrative until a real provider is connected.

These are responsibility boundaries, not assertions about exact existing interface names or method signatures. Bind them to the actual bundle and provider APIs when implementing.

## Interaction lifecycle: join a poker table

1. The floor renderer identifies the clicked table and asks its interaction capability whether joining is possible.
2. The table capability returns an available seat/arrival target, or a clear unavailable result.
3. If available, the world movement capability navigates the avatar to the target while the floor renderer follows the avatar.
4. Once arrival is confirmed, the experience requests the table's perspective renderer with the selected seat and shared table/player state.
5. Cards, opponents, dealer, chips, pot, and seat occupancy are rendered from that same state. Actions go through table/game capabilities; presentation does not invent outcomes.
6. Leaving the table returns to the floor renderer at the avatar's preserved world position.

## References and cycles

Micro-bundles may reference other micro-bundles, including cyclic references. Resolution must therefore be identity-aware: when a bundle is encountered again during a traversal, reuse the already-known canonical instance/reference rather than recursively constructing it again. Keep traversal and resolution bounded, and distinguish a legitimate cycle from a missing or failed reference. Do not rely on unbounded recursive expansion.

## Suggested build sequence

1. Expand the existing floor prototype into a larger, multi-zone world and add a centered avatar/camera-follow model.
2. Make station hover/focus affordances and destination movement work consistently.
3. Define a small, explicit interactable contract against the real available APIs.
4. Implement one complete vertical slice: poker table → seat availability → avatar movement → table perspective → leave and return.
5. Add Digitens roles and behavior, then additional stations and facade/content resolution.
6. Replace illustrative economy counters only when a real provider/ledger contract is available.

## Current truth

The existing casino-floor.html in wwwroot/economy is an interactive visual prototype. Its actor movement, close-up scenes, and virtual chip journal are illustrative. They are not yet backed by a production casino ledger, real seat reservation, canonical player state, or runtime micro-bundle composition.
