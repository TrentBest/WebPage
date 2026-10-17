# Starport Departure Operations — WebPageLLM Handoff

## Intent

Build the first substantial, inspectable Starport Station experience inside Singularity City. The initial slice follows one spacecraft departure from crew muster to release, while establishing the visual and data seams that can later expand to passenger terminals, maglev/flying-cab connections, docking berths, shipyard service aprons, and freight/cargo handling.

This is an experience and integration brief, not a claim that the browser already simulates the scene.

## Current source of truth

- Gestures implementation work: [PR #16 — Start the starport departure experience](https://github.com/TrentBest/TheSingularityWorkshop.Gestures/pull/16).
- Detailed domain intent and limits: [Gestures starport experience document](https://github.com/TrentBest/TheSingularityWorkshop.Gestures/blob/work/starport-departure-experience/docs/STARPORT_DEPARTURE_EXPERIENCE.md).
- Current task contract: stable task IDs, role ownership, dependency ordering, ready/blocked evaluation, and explicit evidence gates for ATC and departure.
- Current pose samples: boarding-ramp, service-umbilical, and service-cover GestureDefinitions with named transforms and provider-owned semantic anchors.
- Important limit: these are CPU-evaluable pose sequences and a choreography/readiness contract. They are not a crowd solver, articulated IK/contact solution, equipment simulation, flight simulator, or browser physics model. PR #16 is a draft and must not be described as merged or released.

## Experience requested

Create a compact Starport Departure Operations view that feels like a living station, not a long static document. It should be possible to understand the procedure from a top-down schematic, role lanes, and a selected-task detail view without requiring a fully rendered 3D environment in the first milestone.

### Scene zones

- **Crew muster / briefing:** crew manifest and assigned flight/ground roles.
- **Approach corridor:** shared route from muster to the vessel's access point.
- **Craft berth and ramp:** vessel silhouette, access door, ramp/ladder, interior-entry marker, and named station anchors.
- **Ground-service apron:** service umbilicals, hose stow rack, removable covers, landing-gear inspection points, tools, and launch-interface / mass-driver covers when applicable.
- **Ground safe zone:** the location ground crew must reach before access sealing and departure release.
- **Readiness / manifest station:** flight crew confirms personnel, stations, equipment, access, and checklist evidence.
- **Departure-control handoff:** request clearance, show granted/denied/pending evidence, and release the craft only after the gate is satisfied.

Use the canonical provider's stable district/building/berth/craft/anchor IDs. Do not invent a second city topology or bake city/world coordinates into Gestures.

## Role lanes and gate logic

Show a shared initial path and then distinct flight-crew and ground-crew lanes:

1. All assigned personnel rally.
2. Everyone routes to the spacecraft door.
3. Flight crew boards via the ramp and reports every assigned member accounted for.
4. Ground crew disconnects/stows service connections, clears and secures covers, performs landing-gear/departure checks, and secures any launch-interface covers.
5. Ground crew returns to the safe zone.
6. The access route is sealed only after flight crew are aboard and ground crew are clear.
7. Flight crew verifies the manifest and readiness checklist.
8. Departure control requests ATC clearance only when all required task outcomes and external evidence are present.
9. The craft may depart only after the clearance request is complete and explicit clearance has been granted.

Use the stable IDs in the Gestures scenario rather than maintaining a second task list in WebPage. When a task is failed or evidence is missing, show the blocked descendants and the exact reason. Do not auto-mark tasks complete based on elapsed animation time.

## Interaction design

- Selecting a task highlights its role lane, dependency chain, relevant anchors, and required evidence.
- Offer a step/advance control for deterministic reference replay and a reset control for returning to the initial state.
- Clearly separate **scenario fixture state** from **observed host evidence**. The fixture can demonstrate dependency ordering; it cannot prove that a hose disconnected, a cover locked, a door sealed, or ATC clearance was granted.
- Provide an explicit failure injection menu for the reference experience: missing crew member, blocked ramp, stale anchor, failed connector release, cover not accounted for, ground crew not returned, door fails to seal, manifest mismatch, clearance denied, or clearance revoked.
- If the host does not supply an observation, show “unknown / awaiting evidence,” not success.
- Include accessible labels and text status; color must not be the only distinction.

## Visual language

Use a station operations-console treatment: berth schematic, clearly named service anchors, crew-role lanes, a concise task-state timeline, and an at-a-glance departure gate. The first view should fit without a main-panel scrollbar. Keep detailed theory and implementation notes behind contextual disclosure or linked documentation. Lazy-load heavier scene/animation content.

A future richer scene may show ships, docking structures, transit arrivals, cargo transfer, and service vehicles. Keep those as separate capability slices rather than blocking the first useful departure experience on a complete city simulation.

## Data and integration boundaries

- **WebPage:** navigation, discovery, selected task, panel layout, fixture replay controls, accessible presentation.
- **Gestures:** reusable pose definitions, stable task semantics, CPU reference pose evaluation, deterministic procedure/readiness evaluation.
- **Craft/service provider:** canonical craft/berth/anchor catalogue and equipment-specific procedures.
- **FSM_API:** optional consumer lifecycle/state orchestration; do not add it as a Gestures dependency.
- **Physics / world simulation:** authoritative collision, contact, obstruction, equipment release/locking, access seal, crew world state, and other physical outcomes.
- **Renderer / Visualize:** later observer-relative and GPU presentation path; not required for the first browser schematic.

Use a fixture adapter while the cross-repository contract stabilizes. Do not add a dependency on an unpublished Gestures or Physics package merely to make the browser compile. If a provider/Physics signal is unavailable, preserve its unknown status and show the seam where it will connect.

## Acceptance criteria for the first WebPage slice

- [ ] Reachable from a sensible Starport / Transportation entry in Singularity City.
- [ ] Shows the named zones and shared-to-role-branching structure.
- [ ] Reads the Gestures scenario's stable IDs or a clearly labeled fixture adapter based on those IDs.
- [ ] Displays ready, in-progress, completed, failed, and blocked states with dependency explanations.
- [ ] Does not allow ATC request without all pre-clearance tasks and evidence.
- [ ] Does not allow departure release without a completed clearance request and granted ATC evidence.
- [ ] Includes at least three replayable pose samples: ramp boarding, umbilical handling, and cover handling, labeled as CPU-reference pose data.
- [ ] Demonstrates at least one injected failure and safe blocked state.
- [ ] Preserves unknown physical outcomes rather than fabricating success.
- [ ] Has automated checks for task ordering/gates and accessible status semantics.
- [ ] Keeps the initial view concise, without a main-panel scrollbar, and lazy-loads optional detail.

## Explicit non-claims

The first WebPage slice does not establish crowd avoidance, collision correctness, IK/contact correctness, real crew coordination, measured population scale, GPU parity, a flight model, or live ATC integration. Those capabilities require their owning implementation and evidence. The browser is a consumer and visual proof surface, not the authority that decides physical outcomes.


## 2026-10-17 WebPageLLM response — current source status and next consumer step

I have now revisited this handoff and checked the current WebPage and Gestures development sources rather than treating the original PR description as current state.

### Verified status

- Gestures PR #16 is now **merged** (merge commit 34bafaf94f4282296b0ad41b1d7e22511448149); the opening section above is stale where it still calls the PR a draft. The merged scenario/evaluator and pose fixtures remain a reference task/readiness contract, not a physical crew/equipment simulation.
- WebPage currently contains TheSingularityWorkshop/Workshop/Spatial/SpatialStarportDepartureExperience.cs. It defines the host-side scenario ID starport.departure.v1, zone/task view models, a readiness-hold snapshot, and a landing-gear-failure snapshot. Its source comment explicitly says it does not evaluate live crew, provider, equipment, Physics, or ATC state.
- The fixture is not yet surfaced as a complete Starport Departure Operations browser experience, and the authoritative city destination/anchor catalogue is not yet wired to the Gestures scenario. I will not imply that the experience is end-to-end connected merely because the model fixture exists.
- SpatialPlaceCatalog currently contains city.transit, city.station, city.shipyard, and city.taxi, but no dedicated city.starport / starport-departure scene entry. That is the correct host-side integration gap to resolve rather than creating a second city topology inside Gestures.

### Consumer implementation contract

The first useful WebPage slice should render the existing host fixture and expose its two deterministic snapshots as scenario examples, with an explicit switch between readiness hold and injected landing-gear failure. It should show role lanes, task dependencies, evidence requirements, and blocked descendants; it must not advance tasks to completed because an animation or button was played. A future live scenario adapter should consume the merged Gestures scenario/evaluator through an available, versioned dependency only when the package/reference is actually resolvable in the WebPage source graph.

The initial page should be reachable from the existing city transportation/port experience and use stable task IDs. Before adding a route, I will verify how SpatialInteractable and the Explore destination manifest route existing scenes so this experience follows the current composition model. The page must label pose data as CPU reference output and all physical/equipment/ATC outcomes as fixture-only or awaiting authoritative evidence until their owners supply observations.

### Acceptance and boundaries

- Test unique task IDs and dependency references in the host fixture.
- Test that the failure snapshot visibly blocks access sealing, readiness, clearance, and departure as appropriate.
- Test that ATC request and departure controls remain disabled until their explicit evidence gates are satisfied; fixture-only snapshots must never fabricate that evidence.
- Keep the panel compact, accessible, and lazy-loaded; status must be expressed in text as well as color.
- Do not add a Physics or unpublished Gestures package dependency solely to compile the page. Do not add Renderer/Visualize as a requirement for the first schematic.

This response records current source facts and the next consumer slice; it does not claim that the browser experience has been implemented yet. No package publication, deployment, or master promotion is authorized.
