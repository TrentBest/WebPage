# Singularity City — Soccer Arena Integration

**Status:** integration plan; the Soccer MicroBundle contract has not yet been verified on the public development branches inspected on 2026-10-15. This document does not claim that a match is running.

## Intended experience

Singularity City should contain a visible, named soccer arena as part of the persistent civic world. The arena is a venue inside the city, not a separate application or a hard-coded replacement for the city model. The Workshop remains the city's fixed composition anchor.

The arena experience should ultimately load a real soccer game through the Soccer MicroBundle. WebPage supplies the host presentation and exposes the composed experience; it must not quietly implement a second set of soccer rules inside a Razor page.

## Responsibility boundaries

- **City / arena:** venue identity, footprint, pitch dimensions, goals, boundary lines, stands, access and the arena's place in the city map.
- **Soccer MicroBundle:** the canonical game setup and soccer-domain behavior it actually implements, including teams, match phases, score, ball possession and rule transitions to the extent supported by its contract.
- **Gestures:** field- and role-scoped permitted gesture vocabulary. A soccer field defines the acceptable gesture set; players or teams choose from that set. Gestures are inputs to the domain, not a substitute for match rules.
- **FSM_COS / manifest:** declare and compose the required MicroBundles and their dependencies. Do not invent bundle IDs, versions, or capability contracts in WebPage.
- **WebPage:** present the arena and the composed match, expose inspectable state, and clearly distinguish live composed behavior from static previews.

## Integration sequence

1. Verify the canonical Soccer MicroBundle ID, version, contract, and provider/behavior entry points from its owning repository or published artifact.
2. Reserve a deterministic arena plot in the Singularity City model, with a named venue and stable identity. Keep the Workshop anchor fixed.
3. Add the arena to the city presentation as a recognizable soccer pitch with goals, touchlines, halfway line, center circle, and spectator structure.
4. Compose the verified Soccer MicroBundle through the normal manifest / FSM_COS path. Do not create a parallel WebPage-only soccer engine.
5. Connect Gestures only through the real supported contract: field-scoped permitted actions, player/team selection, and validation of incoming gesture proposals.
6. Show a match only when its domain state is supplied by the composed bundle. If composition is unavailable, label the arena as a venue preview and say what is missing.
7. Add tests for stable arena identity, required bundle declarations, valid/invalid gesture choices, match-state transitions exposed by the bundle, and browser rendering. Tests must not claim behaviors the bundle does not implement.

## Evidence boundary

A rendered pitch proves the venue presentation only. A match scoreboard or animated ball is not evidence of the Soccer MicroBundle running. A gesture catalog is not evidence that a gesture was accepted by the match rules. The integrated experience should expose the actual composed state and retain these distinctions in its documentation.
