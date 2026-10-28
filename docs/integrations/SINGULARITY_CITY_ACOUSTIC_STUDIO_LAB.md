# Workshop Acoustic Studio Lab

![Living GUI documentation lens: question, model, implementation, visible manifestation, and proof](https://raw.githubusercontent.com/TrentBest/WebPage/development/docs/assets/living-gui-documentation-lens.svg)

> **Evidence class:** architecture illustration, not live runtime telemetry. Follow the subject-specific source, view, fixture, or test below; if none exists, record that gap rather than implying it is implemented.


**Status:** first navigable WebPage prototype; acoustic observations are explicitly illustrative fixtures. The Physics adapter is not connected.

## What exists

- The Workshop campus has an **Acoustic Studio Lab** building and stable place identity.
- Explore routes visitors into a dedicated studio experience and back to the Workshop.
- A data-driven manifest defines a live room, vocal booth, control room, three sources, and two listeners with positions in metres.
- Visitors can select a source and listener and inspect identity, authored position, and any authored fixture observation.
- Missing source/listener fixture pairs are reported as unavailable; WebPage does not infer acoustic values.

## Physics boundary

The authoritative acoustic calculation belongs in [The Singularity Workshop Physics](https://github.com/TrentBest/TheSingularityWorkshop.Physics), not this UI. [Physics PR #21](https://github.com/TrentBest/TheSingularityWorkshop.Physics/pull/21) is a draft, unmerged free-field observation baseline. The numbers shown by this prototype are authored fixture values and **are not produced by that solver**.

The current fixture does not model room reflections, materials, occlusion, waveform generation, recording, playback, mixing, or export. There are no recording controls because those capabilities are not implemented.

## Next increment

After the Physics API is available through the repository's normal dependency/source workflow, add a narrow adapter that maps selected source/listener identities and metre-based positions to the real Physics observation. Preserve units and provenance in the UI, test the mapping, and remove fixture values only when the adapter produces verified observations. Keep room simulation, capture/playback, creator-owned studios, and optional Economy integration as separate increments.

## 2026-10-28 correction — Physics free-field baseline is on development

The earlier statement that Physics PR #21 is a draft, unmerged baseline is stale. [PR #21](https://github.com/TrentBest/TheSingularityWorkshop.Physics/pull/21) was **closed unmerged on 2026-10-16 as part of Operation Sith after its FreeFieldAcoustics source, tests, documentation, and package tags were ported directly to Physics `development`**. The current [FreeFieldAcoustics implementation](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/Physics/Acoustics/FreeFieldAcoustics.cs), [versioned acoustic-observation record](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/docs/experiments/acoustic-observation.run.json), and Physics Build and Tests workflow are the source of truth.

This does **not** mean the WebPage Acoustic Studio Lab is integrated: the current lab still shows authored fixture values, and its Physics adapter is not connected. WebPage should use the current source/dependency workflow to build a narrow adapter and verify provenance; it must not wait on the closed PR or relabel fixture values as solver output. No package publication or release is implied.
