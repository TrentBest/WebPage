# Physics Replay / Shareable Moments — WebPageLLM Integration Brief

![Living GUI documentation lens: question, model, implementation, visible manifestation, and proof](https://raw.githubusercontent.com/TrentBest/WebPage/development/docs/assets/living-gui-documentation-lens.svg)

> **Evidence class:** architecture illustration, not live runtime telemetry. Follow the subject-specific source, view, fixture, or test below; if none exists, record that gap rather than implying it is implemented.


**Audience:** WebPageLLM and anyone implementing the Workshop's public-facing experience  
**Status:** Physics has tested model-level replay and fork-aware route coordination. WebPage now contains a local illustrative `/replay-lab` experience with scrubbing, playback of fixture data, event markers, Physics evidence links, and explicit unavailable states. It is not connected to Physics checkpoints or a reconstructed physical world; browser verification remains outstanding.  
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
- [Replay-segment tests](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/tests/TheSingularityWorkshop.Physics.Tests/TimelineReplaySegmentTests.cs) cover valid child-interval history and invalid timeline, exclusive-start event, and incomplete interval cases.
- [`TimelineReplayExecutor`](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/Physics/Universe/TimelineReplayExecutor.cs) restores state through a caller-provided checkpoint decoder, applies events in sequence, and advances a model to any requested tick inside that segment's validated interval.
- [Replay executor tests](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/tests/TheSingularityWorkshop.Physics.Tests/TimelineReplayExecutorTests.cs) verify ordered event application, scrubbing before a later event, and out-of-range rejection. [CI run](https://github.com/TrentBest/TheSingularityWorkshop.Physics/actions/runs/38672083529) passed.
- [Timeline/replay design contract](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/docs/TIMELINES_AND_MULTIVERSE.md) explains checkpointing, deterministic replay, branching, and the future shareable-moment package.
- [Physics roadmap](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/docs/ROADMAP.md) tracks the future portable-moment, preview/scrub, creator-controlled sharing, and hosting milestones.

**Important current limitations:** the executor is model-driven and requires caller-provided checkpoint decoding and event application. It is not a physics-world adapter; it does not restore a solver/world, guarantee deterministic physics fidelity, or supply a durable checkpoint/event store, interactive browser scrubber, portable moment package, share URL, or hosted playback. The opaque checkpoint payload cannot be assumed to be browser-renderable.

The [Physics build-and-test run](https://github.com/TrentBest/TheSingularityWorkshop.Physics/actions/runs/38672083529) passed for the replay-segment and model-executor contracts. This is evidence for those tested model-level behaviors only—not evidence that a physical scene can already be replayed.

## Where this belongs in WebPage

Make it a first-class **experience/demo**, not a paragraph buried in documentation and not a generic SaaS feature card.

**Current placement:** the manifest already declares a **Replay Lab** destination at `/replay-lab`, and `TheSingularityWorkshop/Pages/ReplayLab.razor` implements the local fixture. Keep it discoverable through the existing Workshop hub; do not add a competing navigation system.

Suggested visitor journey:

1. **A moment worth revisiting** — a visual scene with a visible event timeline and a clearly marked highlight.
2. **Scrub the moment** — a play/pause control, a range slider, and labeled start / highlight / end ticks. The UI must distinguish loaded, reconstructable, approximate, and unavailable ranges.
3. **Understand the event** — concise event markers or annotations; optionally reveal the underlying deterministic history as an inspectable layer.
4. **Make it yours** — show a disabled or explicitly “coming next” remix action until branching works end to end.
5. **Share the experience** — eventually produce a portable moment or share link with creator attribution and a compatibility/availability status.

Visual direction: preserve the Workshop's living, instrumented aesthetic. The timeline should feel like part of the experience, not a conventional video player pasted beneath it. Let the event markers, playhead, and selected highlight react with the scene. Make the moment legible to a non-programmer first, with deeper event/history details available to developers.

## What to integrate now versus later

### Current: truthful illustrative browser surface

- [x] Add the **Replay Lab** destination using the existing manifest/hub convention.
- [x] Provide a small local timeline fixture with a visible playhead, event markers, keyboard-accessible scrubbing, and explicit fixture status.
- [x] Link to the Physics replay executor and solver-backed/route replay tests.
- [x] Mark the browser-world adapter as unavailable and sharing/remix as not implemented.
- [ ] Verify the route and interaction in a real browser; source inspection is not browser evidence.

Keep it clearly labeled as an **illustrative preview** until real world replay is connected.
- Use a deterministic, explicitly labeled illustrative fixture only if a visual prototype needs sample data. Do not imply that fixture is a Physics-generated replay or that controls reconstruct a physical world.
- Keep the replay UI behind a small, replaceable adapter boundary. Do not couple the page to Physics internals, parse opaque checkpoint bytes in the browser, or invent an API that the Physics package does not expose.
- Show a capability/status panel distinguishing **implemented**, **in progress**, and **planned**. A button must either work, explain why it is unavailable, or be disabled; it must not pretend to save, restore, scrub, or share.
- Keep all existing navigation and living-GUI behavior intact. The new experience must not make the page slower or eagerly load an expensive scene before the visitor opens it.

### Next: connect after the Physics side provides a world-reconstruction adapter

The model-level executor now provides a tested seam: it decodes a checkpoint into a caller-owned model, applies ordered events, and advances to a target tick within one validated segment. The next Physics milestone is a concrete world/solver adapter plus a deterministic reconstruction example whose resulting state can be independently verified. In parallel, define a versioned moment contract only after the payload and compatibility requirements are grounded in real code. Until a browser-consumable adapter exists, use a fixture only for clearly labeled layout/design work.

A likely eventual separation is:

- **Physics:** checkpoint compatibility, deterministic stepping/replay, timeline/fork semantics, and exactness/limitations.
- **Moment package/transport:** versioned metadata, event/input history, asset/content references or hashes, bounded start/highlight/end window, provenance, and compatibility declarations.
- **WebPage:** user interaction, visualization, scrubbing controls, annotations, discovery presentation, and graceful unsupported-state handling.
- **Hosting/sharing layer:** access control, creator consent, links, quotas, retention/deletion, attribution, and later monetization. Do not embed these responsibilities in the physics solver.

Do not assume that reversing time is implemented by running updates backward. General physics operations are not necessarily invertible. The initial trustworthy route is checkpoint + ordered history replay.

## Prototype acceptance status and remaining verification

- **Source-level present:** the host manifest declares `/replay-lab`; the route provides event markers, a range slider, step controls, play/pause for fixture data, and a local event ledger.
- **Source-level present:** the page labels the scene illustrative, links to Physics implementation/test evidence, and states that no data is uploaded/shared.
- **Source-level present:** physical world reconstruction is marked unavailable; portable sharing/remix is explicitly not implemented.
- **Not yet verified:** browser navigation and keyboard interaction, responsive layout, console/runtime errors, and whether entry from the hub preserves the intended landing sequence.
- **Not yet measured:** route load and interaction latency. Do not claim performance based on visual inspection.

## What WebPageLLM should do next

1. Browser-verify `/replay-lab` and its entry from the manifest hub; check keyboard use, mobile/desktop layout, console output, and the complete opening sequence.
2. Keep the fixture visibly illustrative. Do not connect arbitrary opaque checkpoint bytes or invent a browser-facing Physics API.
3. Coordinate the next implementation with the Physics owner: a concrete world/solver adapter and independently verifiable deterministic reconstruction example must exist before this page can claim physical replay.
4. Define a portable moment contract only when its checkpoint, event, asset, version, and compatibility requirements are grounded in actual Physics code.
5. Report exact files changed, test evidence, and the local click path. Leave release/deploy actions untouched.

This is an integration handoff, not permission to publish or deploy. The goal is to bring the work into view early enough for human eyes and a real browser to validate it while the underlying reconstruction capability is built.


## 2026-10-23 verification note — browser fixture versus Physics reconstruction

The current `tests/browser-smoke.mjs` source includes a manifest-driven route check: it expects exactly one “Physics Replay Lab — Shareable Moments Prototype” demonstration card, opens its explanation, and verifies the embedded Replay Lab heading. `TheSingularityWorkshop/Pages/ReplayLab.razor` continues to label its timeline as demo data and links to Physics' executor and route-replay tests.

This confirms that browser-smoke coverage exists in source; it does **not** prove that the latest `development` head's workflow ran successfully, nor that the page reconstructs a Physics world. No exact-head WebPage CI result was available in this review. Keep browser runtime verification pending until the current workflow result is checked, and keep physical replay unavailable until Physics provides a concrete world/solver adapter with independently verifiable reconstruction output.

Physics' model-level executor and fork-aware route coordination remain useful prerequisites, but neither the local fixture nor a generated SVG is a replay of a coupled physical world. Shareable Moments, portable checkpoint/event payloads, and world reconstruction remain unimplemented at this boundary.
