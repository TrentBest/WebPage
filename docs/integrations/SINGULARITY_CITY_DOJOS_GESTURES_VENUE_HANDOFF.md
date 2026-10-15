# Singularity City Dojos, Venues, and Gestures Integration Handoff

## Purpose

Define a coherent path for martial-arts dojos and training venues inside Singularity City, coordinated with the Gestures work and the existing arena/venue and Economy design handoffs. This is a design contract for the WebPage experience, not a claim that a dojo simulator or combat model already exists.

## Complementary dojo handoffs

[WebPage PR #117 — hidden martial-arts dojos for Singularity City](https://github.com/TrentBest/WebPage/pull/117) owns the first location/discovery slice: three tucked-away city anchors (market-side, transit-understructure, courtyard), discovery clues, and accessible directory/detail navigation. This document is deliberately complementary: it defines how a dojo venue consumes Gestures, Physics/contact observations, shared venue/event concepts, and Economy-owned accounting. Keep the two responsibilities separate rather than creating competing dojo catalogues or duplicate integration contracts.

## Experience shape

A dojo is a persistent place in the city, not just a menu or isolated animation demo. A first dojo experience should support:

- **Observe:** inspect the dojo floor, training areas, instructor, students, and current class/session.
- **Learn:** select an art or curriculum, see a demonstrated form, and compare a student's attempt with the reference.
- **Practice:** run repeatable solo forms and paired drills with selectable tempo, stance/role, and difficulty.
- **Spar safely:** only after movement/contact ownership is defined; clearly separate scripted choreography from a physically validated sparring model.
- **Progress:** track attendance, curriculum milestones, instructor qualifications, and tournament eligibility as explicit simulation state.
- **Host events:** class schedules, demonstrations, grading days, inter-dojo exhibitions, and tournaments reuse the shared venue/event concept.

Initial arts can be represented as curriculum/content definitions (for example karate, judo, taekwondo, boxing, wrestling, and aikido) rather than hard-coded branches in the rendering layer. Do not imply that a generic motion primitive is a validated representation of every art.

## Ownership and data flow

- **Gestures** owns reusable gesture definitions and CPU reference evaluation. Gesture data should describe authored pose/keyframe trajectories and variations; the evaluator produces evaluated pose data. Dojo content selects and sequences those reusable definitions but should not fork a second gesture engine.
- **Physics** owns authoritative movement/contact mechanics when an applicable model exists. It must decide physical contact, collision, balance, slip, and failure; a visual pose alone does not prove that contact or a throw succeeded.
- **FSM_API** coordinates lifecycle/state transitions and scheduled work. It is not the numerical biomechanics solver or the renderer.
- **FSM_COS** composes declared capabilities at the appropriate boundary; lower-level packages must not depend upward on WebPage or the venue experience.
- **WebPage** owns the visitor-facing dojo, curriculum selection, scene composition, and bindings to real observations. Until an owning model is connected, label choreography and outcome fixtures as authored/scripted.
- **Renderer / Visualize / GUI.SVG** may choose and present the appropriate representation. Gesture definitions must not hard-code event-horizon or renderer policy.
- **Economy** owns the authoritative simulated ledger and economic rules. WebPage must not duplicate balances, wagers, tuition charges, prize awards, or settlement logic.

Suggested path:

```text
Dojo curriculum + venue session
              │
              ▼
       FSM/API lifecycle
              │
       ┌──────┴─────────┐
       ▼                ▼
 Gesture definition   Physics/contact authority
       │                │
 evaluated pose      authoritative outcome
       └──────┬─────────┘
              ▼
     Renderer / Visualize
              ▼
       WebPage dojo scene
              │
              ▼
  attendance / event / Economy adapters
```

This diagram describes intended boundaries; it is not a statement that all adapters exist today.

## Coordination with current work

- [Gestures PR #13 — executable scalar spring-damper reference](https://github.com/TrentBest/TheSingularityWorkshop.Gestures/pull/13) is an early numerical reference and motion-response experiment. Its current scope is one-dimensional; it is not yet wired into `GestureEvaluator` and does not provide articulated-body dynamics, joint torque, collision, contact, balance, or human biomechanics.
- The [Gestures integration contract](https://github.com/TrentBest/TheSingularityWorkshop.Gestures/blob/development/docs/REQUESTS_AND_INTEGRATION.md) keeps gesture evaluation independent of Renderer/Visualize and leaves authoritative world movement to Physics/behavior owners.
- [WebPage PR #109 — Movement Lab](https://github.com/TrentBest/WebPage/pull/109) is the nearby WebPage experience to review for reusable presentation and movement-learning patterns. Reuse rather than create a competing gesture evaluator.
- [WebPage PR #111 — soccer arena handoff](https://github.com/TrentBest/WebPage/pull/111) describes the arena/venue path. Its first soccer attack remains a scripted fixture, not a complete playable sports simulation.
- [Economy PR #13 — long-range economy systems and WebPage handoff](https://github.com/TrentBest/TheSingularityWorkshop.Economy/pull/13) is the economy design source for simulated banking, credit, barter, corporations, exchanges, casinos, education, and integration boundaries.
- [Experiences PR #18 — casino and Story Forge visual prototypes](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/pull/18) demonstrates event/journal and reconciliation presentation ideas. Its casino HTML is explicitly a browser-side prototype, not wired to the authoritative ledger.

## Venue and Economy integration

A dojo should be one venue type among arenas, concert/event spaces, studios, schools, and other city destinations. Reuse shared venue identity, schedule, session capacity, attendance, and event-result concepts instead of inventing a dojo-only event system.

Economy integration should be an adapter to the Economy-owned ledger/contracts, not direct browser arithmetic. Possible future transactions include tuition, class passes, equipment purchases, event tickets, tournament entry, and prize awards. Until those contracts are available and connected, show no real charge and do not present sample balances as authoritative.

Dojo, arena, and city events should eventually share discoverability and scheduling while retaining their own domain rules. A martial-arts grading or tournament is not a soccer match with renamed labels.

## Staged implementation

1. **Venue shell:** consume the shared city-anchor/discovery contract from the companion dojo handoff and define a manifest-backed scene with training zones.
2. **Curriculum and demonstration:** authored reference sequences and instructor/student role routing using existing Gestures data and evaluator APIs.
3. **Comparison:** compare evaluated poses against reference with explicit timing and variation metadata; do not claim biomechanical correctness.
4. **Paired drills:** model two participants' roles and procedure states. Keep success/failure scripted until Physics exposes suitable contact/outcome authority.
5. **Events and progression:** connect schedule, attendance, qualifications, and tournaments to shared venue concepts.
6. **Economy adapter:** consume the Economy-owned contract and observations once stable; no duplicate ledger.
7. **Physical validation:** integrate contact, balance, collision, and failure only when the owning model and tests exist.

Each stage should have deterministic fixtures, tests for state transitions, and a clear distinction between authored content, model output, and presentation.

## Acceptance checks

- A dojo is discoverable as a city venue and does not obstruct existing campus navigation.
- At least one form is driven through the real Gestures definition/evaluation path, not a look-alike animation implementation.
- Role changes, interruption, and reset are explicit state transitions.
- Scripted outcomes are labelled; no visual pose is treated as proof of physical contact.
- Renderer/Visualize remain optional consumers, and the Gestures package stays graphics-independent.
- Economy values come from the Economy boundary when integrated; no duplicate browser-side accounting.
- No claim of validated fighting effectiveness, human biomechanics, or safety-critical physical fidelity without appropriate evidence.

## Status

Design handoff only. No dojo runtime, full martial-arts simulation, validated sparring physics, venue economy transactions, or production event system is claimed by this document. Coordinate API changes with the Gestures and domain owners before adding hard package dependencies.
