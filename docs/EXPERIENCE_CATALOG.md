# Experience Catalog — Events, Markets, and VR Spellcraft

**Status: composition direction, not a claim that each Experience is implemented.** This document extends the Experience theory with concrete candidate environments. The Experience remains an environment comprised of MicroBundles; it is not synonymous with a route, Blazor page, scene, or hard-coded page component.

## The organizing rule

An Experience defines the environment and the capabilities that make participation possible. Each MicroBundle owns a focused capability. A provider can add domain behavior without teaching WebPage what a goal, stall, chorus, spell, or transaction means.

The same host can manifest several environments:

- **Singularity Arena** — a venue whose schedule can host soccer, tournaments, concerts, karaoke, and other timed events.
- **Market Fair** — a public environment for stalls, browsing, demonstrations, makers, conversation, and discovery.
- **Craft Fair** — an environment for artisans, materials, works in progress, commissions, and demonstrations.
- **Workshop / Class** — instructor-led lessons, practice, feedback, and learner progression.
- **Exhibition / Gallery** — exhibits, guided tours, creator stories, and visitor attention.
- **Festival / Civic Gathering** — multiple activities, public spaces, volunteering, and scheduled performances.
- **Transit / Cargo Hub** — arrivals, departures, loading, transfers, and logistics.
- **VR Magic / Spellcraft** — tracked physical procedures and a typed elemental weave that can be validated as a spell.

These are Experience candidates and domain scenarios. The catalog does not imply that all are currently available as selectable, manifest-backed WebPage Experiences.

## Singularity Arena is a venue, not a soccer-only Experience

A shared venue identity and scheduling capability can host event kinds from different providers. Soccer rules remain with the soccer domain; concert cue and audience policy remain with performance capabilities; a fair's stalls and maker activities remain with fair providers.

The neutral scheduler should own only common scheduling facts: event identity, venue identity, provider identity, time interval, and lifecycle. It should not hard-code goals, songs, stall sales, or other event-specific outcomes.

See the canonical [Singularity Arena reference fixture](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleDomain/blob/work/soccer-domain-microbundle/docs/SINGULARITY_ARENA_PERFORMANCE.md) and [Experiences Beyond the Arena](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleDomain/blob/work/soccer-domain-microbundle/docs/EXPERIENCES_AND_OPTIONAL_ECONOMY.md). The linked branch is a review proposal, not a merged release.

## Economy is optional

Market and craft fairs should remain meaningful without Economy. Visitors can browse, talk, watch a maker, attend a demonstration, or learn without a checkout path. A stall can expose a non-commercial profile and activity without prices, inventory, or payment.

When Economy is composed, selected providers may expose optional offers, prices, inventory, purchases, or commissions through adapters. The Experience must not require Economy merely to exist, and the presence of an offer must not be treated as proof of a completed transaction.

This also applies to the other environments:

- An Arena event does not require Economy to be scheduled or attended.
- A workshop does not require a paid lesson to teach.
- A gallery does not require artwork to be for sale.
- A spellcasting Experience does not require paid materials or an Economy provider to cast.
- A transit hub may run its logistics simulation without fares or commerce enabled.

Economy is exploratory; this document does not claim production payment handling or live money movement. The adapter is optional and provider results remain authoritative.

## VR Magic — a distinct Gestures use case

Magic proves a different side of the Gestures model from locomotion, boarding, or crowd performance. Pulling the left trigger enters spell-weaving mode. Earth, Air, Water, Fire, and Spirit appear around a central weave; Spirit carries a good, neutral, or evil alignment.

As the player reaches toward an element, the consumer requests an appropriate physical procedure. The visual flow follows the tracked arm toward the elbow, then arcs to a casting sphere and becomes a colored thread with a thickness. A spell declares its required weave, accepted thread-thickness ranges, and permitted modifiers. Thickness may influence power or amplification only where the spell allows it.

Releasing the left trigger requests casting; it must not bypass validation. Tracking loss, an unreachable node, invalid thickness, missing elements, or a stale trigger-release event must not silently cast a spell.

The ownership boundary is explicit:

- Gestures owns ideal physical procedure data and deterministic evaluated pose output.
- VR/input and kinematics integration own tracked joints, reachability, IK target resolution, and tracking confidence.
- The spell domain owns element identity, Spirit alignment, weave grammar, thickness limits, modifiers, acceptance, and effects.
- Renderer/Visualize present the accepted element trails and threads.
- Audio owns sound and spatial mixing.
- Economy may optionally participate in crafting or vendors but is not required for spellcasting.

The test-backed reference scenario is in [Gestures: VR Magic Spellcasting](https://github.com/TrentBest/TheSingularityWorkshop.Gestures/blob/work/planar-ik-reference/docs/MAGIC_SPELLCASTING_SCENARIO.md). That branch contains a test fixture for the current CPU evaluator and a small consumer-owned acceptance model; it is not a tracked-controller integration or live VR demo.

## Manifest composition and proof

A future WebPage visitor journey should prove a real composition, not merely show a card with a convincing title:

1. The visitor selects an Experience.
2. The manifest declares its required capability roots.
3. The composition host resolves and assembles those capabilities.
4. WebPage presents the resulting Experience.
5. The visible interactions correspond to actual capability behavior.
6. Tests and runtime evidence support each claim.

Optional capabilities should be represented as optional composition choices or adapters. The base Experience should have a test that proves it still works when the optional provider is absent. Do not duplicate the canonical scheduler, Economy semantics, Gesture evaluator, or spell rules inside WebPage.

## Current / Direction / Future

- **Current:** WebPage has a manifest-driven composition direction and an Experience theory contract. The Arena reference fixture and the Gestures magic scenario live in their owning repositories on review branches.
- **Direction:** use the catalog to guide Experience discovery and to compose provider-owned domain capabilities without hard-coding each domain into WebPage.
- **Future:** a visitor-selectable catalog, actual market/craft-fair provider behavior, optional Economy adapters, and tracked VR spellcasting. Those require integration work and observable proofs before being described as implemented.
