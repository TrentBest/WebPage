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
