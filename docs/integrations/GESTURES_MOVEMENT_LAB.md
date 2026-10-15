# Gestures Movement Lab — WebPage proving ground

## Purpose

The Movement Lab is a small, lazy-by-navigation browser view for inspecting the authored hurdle-course geometry described in the Gestures WebPageLLM handoff. It makes the element order, stable IDs, axis-aligned center/full-extents bounds, and unit label visible.

## Current status

- The view uses a local three-element display fixture matching the values in the handoff and current Gestures test fixture.
- Selecting an element updates its ID, kind, center, full extents, and units.
- The page is a WebPage browser adapter and teaching surface; it does not own movement or physics behavior.
- It does not depend on an unpublished Gestures package or claim that the fixture is canonical serialized data.
- No physical attempt, agent traversal, collision, clearance, support, momentum, or landing result is simulated or inferred.

## Open it

Run WebPage using the repository's current development instructions, enter the Workshop, select the manifest-declared **Movement Lab** destination, then choose any of the three elements to inspect its data. The direct route is `/movement-lab`.

## Evidence and next step

The upstream API remains on the Gestures working branch and requires PR review/validation before package integration: [PR #10](https://github.com/TrentBest/TheSingularityWorkshop.Gestures/pull/10).

After that contract is accepted, replace the fixture adapter with the real versioned interchange contract only after serialization is deliberately designed. Connect outcomes only to observations supplied by an authoritative physics owner. Do not turn synthetic observations into a purported real agent run.

## Verification checklist

- [ ] Build WebPage and run the relevant tests.
- [ ] Verify the Movement Lab destination appears in the existing Workshop hub.
- [ ] Inspect element selection by mouse and keyboard.
- [ ] Inspect desktop and narrow viewport layouts.
- [ ] Measure cold-open and selection latency.
- [ ] Confirm the landing sequence and existing experiences remain unchanged.
