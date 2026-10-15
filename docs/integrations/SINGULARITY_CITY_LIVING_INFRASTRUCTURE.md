# Singularity City — Living Infrastructure Architecture

**Status:** architecture contract and implementation sequence. This document describes the next integration seams; it does not claim that every listed system is implemented.

## The city is the showcase

Singularity City should feel like a coherent place, not a collection of unrelated demo panels. The Singularity Workshop is its fixed civic/technical anchor. Digitens should have observable routines and destinations; transport should carry people and goods; venues should host scheduled events; shops should eventually connect digital storefronts with real-world inventory and fulfillment.

The proof must be living state: the same state that drives an interaction should drive its visible result, tests, and explainers. Avoid a decorative animation that merely implies a subsystem exists.

## Place model

Model the city as a catalog of distinct places and their relationships:

- **Singularity Station:** a major public-transit interchange, with arrival/departure boards, accessible passenger circulation, platform/berth identity, and transfers between ground, aerial, and high-capacity services.
- **Ground transit:** walking paths first, then road vehicles, buses, freight trucks, and service vehicles.
- **Aerial transit:** flying cabs and shuttles with named corridors, explicit altitude lanes, boarding locations, and passenger journeys.
- **Maglev:** a high-capacity, schedule-driven line connecting station, shopping districts, port, industrial districts, and arena. Stations, platforms, service schedules, and vehicle runs are data—not labels painted onto a map.
- **Shipyard and harbor:** docks, berths, vessels, loading zones, containers, and links to inland freight.
- **Cargo yard/intermodal terminal:** cargo lots, container identity, storage slots, cranes, truck bays, train consists, and transfer events. Cargo must be accounted for as it moves between carriers.
- **Commerce districts:** storefronts, product catalogs, stock/availability, purchases, and fulfillment boundaries.
- **Singularity Arena:** a venue that can host soccer, concerts, and open-mic/karaoke performances. Audience reactions should be derived from authoritative event signals and audience/agent state, not a scripted cheer that pretends a goal or note occurred.

These are connected places and services. They should share stable identities and explicit links, not be hard-coded as one monolithic city script.

## Data and responsibility boundaries

Keep responsibilities separate and composable:

1. **Catalog/manifest — what exists:** stable IDs, types, footprints, platforms, routes, berths, stock definitions, schedules, and declared capabilities.
2. **Domain state — what is happening:** passenger journeys, vehicle runs, cargo custody, store inventory/orders, venue booking, match state, performance cues, and Digiten activities.
3. **FSM_API — how stateful processes are represented and advanced:** named process groups, explicit state transitions, deterministic update boundaries, observable state, and independent tick cadence. Use actual FSM_API primitives rather than creating a parallel generic state-machine abstraction in WebPage.
4. **MicroBundleDomain — what can be assembled:** independently selectable station, maglev, aerial taxi, harbor, freight, retail, arena, audience, and performance capabilities with clear dependencies and manifests.
5. **FSM_COS / host composition — how capabilities are assembled:** resolve the selected bundles and their dependencies at the composition boundary. Lower-level bundles must not depend upward on the host.
6. **Renderer / GUI.SVG / WebPage — what is presented:** render snapshots and domain events from the source state. The renderer must not become the authority for schedules, cargo custody, scores, or audience reactions.
7. **External commerce adapters — what is connected to the real world:** product feeds, real inventory, orders, payments, shipping, tax, refunds, seller consent, and attribution. Treat these as explicit adapter boundaries; never imply a real product is available or an order is placed when only a demo record exists.

A model that currently only defines routes or a scripted match fixture must be described as such. It is not yet a complete transport, physics, match, or commerce service.

## FSM_API and event-horizon LOD

The city is an opportunity to prove that independent process groups can update at different rates without forcing every Digiten, vehicle, particle, or building to tick every frame.

- **Near interaction horizon:** precise avatar movement, active boarding, collision/contact candidates, current match play, and the performer/audience interaction.
- **District horizon:** visible vehicles, pedestrian/Digiten routines, station arrivals, shop activity, and crowd response.
- **City horizon:** maglev timetables, cargo routing, scheduled performances, and city-level demand.
- **Distant/background horizon:** aggregate occupancy, inventory reconciliation, and slow environmental or economic processes.

These are proposed policy bands, not a claim that LOD scheduling is already implemented. Each process should declare its cadence, required fidelity, and observable output. When an entity crosses a horizon, preserve state and continuity; reduce update frequency or representation without teleporting it, duplicating it, or losing custody of its cargo/passengers.

Use deterministic inputs and explicit elapsed time for replayable tests. Prefer event-driven updates when a state change is sufficient; reserve high-frequency ticks for systems whose behavior actually requires them. Physics-specific effects such as water displacement, dust, debris, and material response should be supplied by the physics capability, not faked by the GUI.

## Example end-to-end journeys

### Passenger

A Digiten or player requests a ride → a journey is created → a route/service is selected → a vehicle is dispatched → boarding and alighting events advance the journey → the renderer shows the same vehicle and passenger state → tests can replay the journey from the same inputs.

### Freight

A product/order or industrial job creates a shipment → cargo is assigned a stable container identity → the harbor records loading → the container is transferred to a rail consist or truck → the receiving yard records custody → delivery updates the order. Each transfer has a source, destination, time, and custody outcome.

### Arena night

A venue schedule books a soccer match or musical performance → arrivals and transit demand increase → crowd/seat occupancy is represented → the match or performance system emits authoritative events → audience response follows those events and individual/group state → transit and commerce can react to the event ending.

For karaoke, microphone input and permission must be explicit; performance visualization can respond to actual input/audio analysis, while lyrics, recording, and public sharing require their own rights and privacy rules.

### Commerce

A storefront displays catalog data with provenance and freshness → the user explicitly chooses a product → an order/checkout adapter validates price, stock, seller, location, and fulfillment → confirmed results return as domain events. Affiliate or orchestration revenue must be disclosed and calculated from actual attributed transactions, not inferred from clicks.

## Suggested implementation order

1. **Station as a real place:** catalog entry, platform/berth identities, transfers, schedule model, and GUI inspection.
2. **Time-driven transport snapshots:** a first presentation prototype now advances the four declared aerial routes once per second while the City map is open, and cancels its loop when the map closes or scope changes. Next, replace the component-local UTC epoch with an authoritative host/simulation clock and add integration-level replay tests.
3. **One end-to-end passenger journey:** walking/boarding/ride/alighting state before multiplying vehicle counts.
4. **Maglev service:** stations, timetable, vehicle runs, capacity, and arrival boards.
5. **Cargo chain:** container IDs and auditable handoff events between ship, yard, train, and truck.
6. **Venue/event calendar:** arena schedules shared by transit, crowd state, and commerce.
7. **Audience and performance interaction:** authoritative match/performance cues, audience response, and optional karaoke input.
8. **Retail integration seam:** local mock catalog first, then separately configured real-world adapters with explicit fulfillment and attribution.

Each step should add a living proof, deterministic tests, and an educational explanation of the underlying process. Expand the number of visible vehicles and Digitens only after the source state and lifecycle are credible.

## Non-goals for the first slice

- A single giant FSM that owns the entire city.
- A second execution loop hidden inside the GUI.
- Fake physics, fake schedules, fake stock, fake audience reactions, or fake real-world orders.
- Coupling core domain contracts to a specific renderer, game engine, store, carrier, or LLM provider.
- Treating every distant entity as if it needs the same update frequency as the entity under the player's hands.

The long-term proof is not the number of neon icons on screen. It is that the city can be observed, queried, replayed, composed from microbundles, and selectively updated while remaining coherent.
