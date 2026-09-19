# Architecture Audit — September 2026

## Reference standard

FSM_API is the reference implementation for deterministic lifecycle behavior, unit-test discipline, coverage collection, documentation, and release hygiene.

WebPage consumes FSM_API; it should not grow a competing state-management framework.

## Current findings

### Already aligned

- MicroBundle lifecycle is backed by FSM_API.
- Page-level lifecycle has a dedicated PageFSM.
- Hub arbitration is bounded to ten rounds.
- Runtime ontology uses nine integer layers.
- GUI construction is increasingly routed through recursive GUI builders.
- Experiences are explicitly defined as environments composed from MicroBundles.
- Test projects are separate from application projects.

### Transitional architecture to migrate

1. **Static Idler/Flex catalogs** — concrete inventory still lives in `IdleExperienceCatalog` and `FlexExperienceCatalog`. Target: registry discovery.
2. **WorkshopDemoBundle** — useful proof, but target is normal registry/manifest composition.
3. **WorkshopExperienceService** — string lifecycle states should become FSM/API context state.
4. **MainLayout startup timer** — presentation timing currently participates in lifecycle signaling; FSM_API should own the transition.
5. **FlexExperienceHost** — owns a timer and generation progression; target is an Experience/MicroBundle FSM.
6. **IdleExperienceHost** — Pong is a MicroBundle, but the host still calls its update directly; target is Hub-owned heartbeat.
7. **Explore.razor** — contains substantial movement, transit, maze, mansion, lab, and scene orchestration; target is Experience/MicroBundle + FSM_API contexts.
8. **CSS and inline styles** — builders exist, but semantic GUI construction still leaks into Razor components; CSS should remain manifestation styling.

## Target dependency direction

WebPage host → Hub → Experience registry → MicroBundles → FSM_API

Not:

Razor page → bespoke state → bespoke timer → bespoke domain logic.

## Arbitration model

1. Register available MicroBundles.
2. Install with `LoadBundle(IArbitrator)` exactly once.
3. During load, inspect installed/available bundles and request dependency loading.
4. Preserve installation completion order.
5. Run `Arbitrate(IArbitrator, roundIndex)` for at most ten rounds.
6. Each round can inspect the current installed composition and conditionally mutate other bundles.
7. Never fabricate a missing bundle.
8. If a bundle arrives later, later arbitration can see it.

This supports Magic modifying Elements only when Elements exists, or Weapons receiving a Singing Sword when its provider bundle exists.

## Physics direction

The Elements bundle should become the reusable material/physics boundary: every physical thing gets a material integer, that integer resolves to physics data, and users can opt out or override applicable behavior.

## Workshop arbitration

The Workshop itself should become a composition participant. Buildings, tools, GUI builders, Experiences, and MicroBundles should be discoverable and arbiter-visible rather than page-owned.

## Migration rule

For every transitional subsystem: locate behavior → identify its MicroBundle/Experience boundary → cover it → migrate it → remove obsolete host code.
