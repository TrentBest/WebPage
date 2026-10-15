# Physics Replay / Shareable Moments — WebPageLLM Integration Brief

**Audience:** WebPageLLM and anyone implementing the Workshop's public-facing experience  
**Status:** updated integration brief; Physics now has a tested model-driven single-segment replay executor, but there is not yet a concrete world adapter or browser playback experience  
**Source branch:** `development` in both repositories  
**Do not publish or deploy from this brief.**

## The experience we are trying to make real

The Workshop should not merely explain that it can simulate and compose complex systems. It should let someone *see an event unfold, return to the important instant, scrub around it, and share that experience with another person*. A clutch moment should become an invitation: watch it, inspect it, branch it, remix it, and eventually publish your own creation.

The product loop is:

1. **Capture** a bounded moment from a running experience.
2. **Review** it with a timeline and a clear highlighted instant.
3. **Scrub** backward and forward within the captured window.
4. **Share** a portable, creator-attributed moment with another person.
5. **Remix** it into a new branch while preserving its origin and attribution.
6. **Discover** and, later, host and monetize creations.

This is a direction for the Workshop ecosystem, not a claim that all six steps exist today. Physics is developing the trustable reconstruction foundation first. WebPage is where we can make the experience visible and test it in a real browser.

## What Physics actually has today

Repository: [TheSingularityWorkshop.Physics](https://github.com/TrentBest/TheSingularityWorkshop.Physics/tree/development)

The current code now includes validated replay-route inputs and a model-driven executor for one validated segment. It is an executable seam, not yet a playable physics-world replay:

- [`TimelineBranch` / lineage foundations](https://github.com/TrentBest/TheSingularityWorkshop.Physics/tree/development/Physics/Universe) model timeline identity and parent/fork relationships.
- [`TimelineReplayPlan`](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/Physics/Universe/TimelineReplayPlan.cs) describes a forward route across fork-aware intervals.
- [`TimelineHistorySegment`](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/Physics/Universe/TimelineHistorySegment.cs) validates a checkpoint plus ordered events through a requested tick.
- [`TimelineReplaySegment`](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/Physics/Universe/TimelineReplaySegment.cs) binds one history segment to one planned interval and checks timeline identity, checkpoint tick and versions, interval coverage, and event boundaries.
- [Replay-segment tests](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/tests/TheSingularityWorkshop.Physics.Tests/TimelineReplaySegmentTests.cs) cover valid child-interval history and invalid timeline, exclusive-start event, and incomplete interval cases.\n- [`TimelineReplayExecutor`](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/Physics/Universe/TimelineReplayExecutor.cs) restores state through a caller-provided checkpoint decoder, applies events in sequence, and advances a model to any requested tick inside that segment's validated interval.\n- [Replay executor tests](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/tests/TheSingularityWorkshop.Physics.Tests/TimelineReplayExecutorTests.cs) verify ordered event application, scrubbing before a later event, and out-of-range rejection. [CI run](https://github.com/TrentBest/TheSingularityWorkshop.Physics/actions/runs/38672083529) passed.
- [Timeline/replay design contract](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/docs/TIMELINES_AND_MULTIVERSE.md) explains checkpointing, deterministic replay, branching, and the future shareable-moment package.
- [Physics roadmap](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/docs/ROADMAP.md) tracks the future portable-moment, preview/scrub, creator-controlled sharing, and hosting milestones.

**Important current limitations:** no replay executor, solver restoration, durable checkpoint/event store, interactive scrubber, portable moment package, share URL, hosted playback, or proof of replay fidelity is implemented yet. A `TimelineReplaySegment` validates the material supplied to it; it does not execute physics or restore a world. The opaque checkpoint payload cannot be assumed to be browser-renderable.

The Physics CI passed for the current interval-validation implementation and tests. This is evidence for those validation contracts only—not evidence that a scene can already be replayed.

## Where this belongs in WebPage

Make it a first-class **experience/demo**, not a paragraph buried in documentation and not a generic SaaS feature card.

**Suggested placement:** a new **Moments** or **Replay Lab** destination within the Workshop showcase/navigation, alongside other living demonstrations. If the current navigation is being reorganized, use the existing showcase/hub convention rather than adding a separate competing navigation system. Keep it discoverable from the main Workshop hub.

Suggested visitor journey:

1. **A moment worth revisiting** — a visual scene with a visible event timeline and a clearly marked highlight.
2. **Scrub the moment** — a play/pause control, a range slider, and labeled start / highlight / end ticks. The UI must distinguish loaded, reconstructable, approximate, and unavailable ranges.
3. **Understand the event** — concise event markers or annotations; optionally reveal the underlying deterministic history as an inspectable layer.
4. **Make it yours** — show a disabled or explicitly “coming next” remix action until branching works end to end.
5. **Share the experience** — eventually produce a portable moment or share link with creator attribution and a compatibility/availability status.

Visual direction: preserve the Workshop's living, instrumented aesthetic. The timeline should feel like part of the experience, not a conventional video player pasted beneath it. Let the event markers, playhead, and selected highlight react with the scene. Make the moment legible to a non-programmer first, with deeper event/history details available to developers.

## What to integrate now versus later

### Now: prepare a truthful integration point

- Add the **Moments / Replay Lab** destination and an intentionally small visual prototype if it fits the current hub architecture.
- Label it clearly as an **experimental preview** until real replay is connected.
- Use a deterministic, explicitly labeled illustrative fixture only if a visual prototype needs sample data. Do not imply that fixture is a Physics-generated replay or that controls reconstruct a physical world.
- Keep the replay UI behind a small, replaceable adapter boundary. Do not couple the page to Physics internals, parse opaque checkpoint bytes in the browser, or invent an API that the Physics package does not expose.
- Show a capability/status panel distinguishing **implemented**, **in progress**, and **planned**. A button must either work, explain why it is unavailable, or be disabled; it must not pretend to save, restore, scrub, or share.
- Keep all existing navigation and living-GUI behavior intact. The new experience must not make the page slower or eagerly load an expensive scene before the visitor opens it.

### Next: connect only after the Physics side provides a tested executable seam

Physics should first deliver a small deterministic reconstruction example that starts from a checkpoint, applies ordered inputs/events, and produces verifiable state at a target tick. Then define a versioned moment contract and an adapter that WebPage can consume. Until that exists, use a fixture only for clearly labeled layout/design work.

A likely eventual separation is:

- **Physics:** checkpoint compatibility, deterministic stepping/replay, timeline/fork semantics, and exactness/limitations.
- **Moment package/transport:** versioned metadata, event/input history, asset/content references or hashes, bounded start/highlight/end window, provenance, and compatibility declarations.
- **WebPage:** user interaction, visualization, scrubbing controls, annotations, discovery presentation, and graceful unsupported-state handling.
- **Hosting/sharing layer:** access control, creator consent, links, quotas, retention/deletion, attribution, and later monetization. Do not embed these responsibilities in the physics solver.

Do not assume that reversing time is implemented by running updates backward. General physics operations are not necessarily invertible. The initial trustworthy route is checkpoint + ordered history replay.

## Acceptance checks for the first visible prototype

- A visitor can find it from the Workshop hub without disrupting the existing landing experience.
- The selected moment and scrub range are visually clear and keyboard-accessible.
- The UI communicates when data is illustrative versus produced by the real Physics implementation.
- Unavailable playback, remix, and sharing are visibly unavailable rather than simulated as successful.
- Opening the experience does not trigger expensive background work before it is needed.
- The page remains responsive while the visitor interacts; measure load and interaction latency rather than relying on appearance alone.
- The implementation is small enough to replace as the real adapter and portable moment format mature.

## What WebPageLLM should do next

1. Read this brief and the linked Physics design contract.
2. Inspect the current WebPage navigation/hub and place the entry using the existing architecture.
3. Implement the smallest truthful, lazy-loaded visual prototype—or only the navigation/empty state if no real fixture is appropriate.
4. Do not make claims or UI affordances for real replay, sharing, hosting, or remix until those paths work.
5. Report the exact files changed, how to launch the local browser experience, and what Trent should click/test. Leave release/deploy actions untouched.

This is an integration handoff, not permission to publish or deploy. The goal is to bring the work into view early enough for human eyes and a real browser to validate it while the underlying reconstruction capability is built.
