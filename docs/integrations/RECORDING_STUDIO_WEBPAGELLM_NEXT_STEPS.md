# WebPageLLM Implementation Handoff — Recording Studio Vertical Slice

**Target repository:** [TrentBest/WebPage](https://github.com/TrentBest/WebPage)  
**Integration line:** `development` (this handoff branch is based on `development`)  
**Vision document:** [Singularity City Recording Studios](SINGULARITY_CITY_RECORDING_STUDIOS.md)  
**Physics baseline:** [Physics PR #21](https://github.com/TrentBest/TheSingularityWorkshop.Physics/pull/21)  
**Status:** implementation plan; not yet an implemented WebPage studio experience.

## Your assignment

Take the recording-studio concept into WebPage as an inspectable, manifest/gallery-friendly vertical slice. Preserve the existing Workshop and its navigation. Make the first destination useful and honest even if the Physics acoustic PR has not merged or become available through the normal dependency workflow.

Do not assume APIs from a draft PR are already available through a published package. Inspect the actual WebPage `development` source, current Physics branches, project dependency policy, and test harness before choosing implementation details. If direct integration is blocked by the unpublished state of Physics PR #21, use a narrow adapter boundary and a clearly labeled deterministic fixture for the browser prototype; do not duplicate Physics implementation in WebPage or claim that the fixture is running the Physics solver.

## Proposed first vertical slice

### Visitor journey

1. From the existing Workshop navigation/gallery, open **Recording Studio**.
2. Enter a schematic Workshop studio with clearly named spaces: live room, vocal booth, control room, and listening position.
3. Select an audio-source marker and a listener marker.
4. Inspect source identity, listener identity, positions in metres, distance, direction, propagation delay, and a free-field level estimate when those values are provided by the actual acoustic model.
5. Read a concise explanation of what those values mean and what is not yet simulated.
6. Return to the Workshop and verify existing routes and experiences remain intact.

The first UI may be a spatial diagram/SVG and data inspector. It need not pretend to be a full DAW. If no real capture/playback exists, do not show functional record, stop, mix, or export controls that imply otherwise.

### Data shape

Prefer a small data-driven studio definition over UI-specific constants. Exact types should fit existing WebPage conventions, but conceptually it needs:

- stable studio ID, display name, creator/ownership label, and prototype status;
- rooms/zones with stable IDs, names, and spatial bounds or positions;
- acoustic sources with stable IDs, labels, and positions in metres;
- listeners with stable IDs, labels, and positions in metres;
- optional observation results produced by an adapter to the Physics model;
- provenance/status metadata distinguishing computed values, authored configuration, and illustrative fixture values.

Keep the acoustic calculation in Physics. WebPage can map a studio's authored source/listener selection to a Physics observation and render the result. If the model package cannot yet be consumed, use an adapter contract whose seam is explicit and avoid copying the `FreeFieldAcoustics` algorithm into the host.

## Physics source-of-truth

The current first baseline is described in [`FreeFieldAcoustics.cs`](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/work/acoustic-free-field/Physics/Acoustics/FreeFieldAcoustics.cs) on the draft implementation branch. It models point-source/listener observations in free field:

- propagation delay = distance / sound speed;
- default sound speed = 343 m/s;
- free-field level estimate uses a 20 log10 distance relationship relative to the source's reference distance and level;
- normalized direction from listener to source, where positions differ.

Its limitation is decisive: this is not audio waveform generation/capture/playback, a room solver, or a full perception model. PR #21 is draft and unmerged. Confirm its current status and current code before depending on it. CI was not confirmed when this handoff was written; run the repository's actual checks and report their outcomes rather than assuming success.

Do not claim the prototype simulates reflection, material absorption, occlusion, room impulse response, stereo/binaural perception, Doppler, acoustic baking, or semantic hearing. These can be staged future increments with separate contracts and tests.

## Existing WebPage integration rules

- Work from the current `development` line and use the existing manifest/ExperienceGallery route pattern.
- Lazy-load the new destination through the existing host; do not replace the landing journey or build a parallel application shell.
- Preserve the FSM_API/FSM_COS ownership boundary. The browser component presents state and sends user intent; it must not invent a second simulation heartbeat.
- Keep the host's current dependencies and source-versus-package CI expectations. Avoid adding a dependency on a package that has not been published or merged unless the repository's existing PR source-integration job intentionally checks out the exact companion branch.
- Extend the existing browser smoke journey and focused unit tests rather than creating a disconnected test framework.
- Keep documentation aligned with the exact code paths and behavior that ship in this PR.

## Creator studios: model for reuse, do not overbuild yet

The first slice should establish a reusable definition that could later be instantiated by a creator, not build a whole social network. Use the Workshop studio as the initial instance. Do not hard-code one UI branch per named artist or label.

Future creator-created spaces may need identity, persistence, access control, invitation, public/private discovery, content rights, moderation, and official-versus-fan labeling. Those are separate features and must not be faked with local in-memory forms. For this first slice:

- clearly label the Workshop studio as the Workshop prototype/instance;
- do not imply any real artist, label, or rights holder is a partner or participant;
- do not upload, scrape, or embed third-party recordings or logos without an appropriate basis;
- treat user-authored creator metadata as unverified unless a real verification system exists;
- keep fan-made studios distinguishable from official artist-controlled studios.

Economy integration is a later optional capability. Studio creation, experimentation, and creative participation must not require a simulated business registration or transaction.

## Test and review checklist

### Automated checks

- Unit tests cover deterministic studio fixture identity, valid spatial metadata, and source/listener selection.
- If the Physics adapter is active, test the mapping from selected source/listener to returned observation and verify units/IDs are preserved.
- Browser smoke opens the Studio Lab from existing navigation, selects at least two source/listener combinations, and checks the visible observation.
- Browser smoke verifies the prototype/fixture disclosure and absence of fake recording controls.
- Existing Workshop opening, navigation, and relevant Experience smoke journeys continue to pass.
- Run the exact build/test commands required by the repository and report actual workflow results.

### Visual review

- The room diagram, controls, and observation panel fit the available viewport without introducing a competing page-level scrollbar.
- The diagram is readable on narrow screens and does not rely only on color to distinguish source/listener markers.
- The user can tell which values are calculated, which room/source data are authored, and which features are future work.
- The experience feels like a place in the Workshop, not a generic admin dashboard.

## Deliverables

1. A working first Studio Lab destination in WebPage.
2. Focused automated tests and browser smoke coverage.
3. Updated integration/experience documentation linked from the appropriate existing index if the repo convention requires it.
4. A PR summary listing exact changed paths, build/test results, visual-review needs, dependency assumptions, and unimplemented features.
5. A concise handoff back to the coordinating agent explaining what exists now and what the next concrete increment should be.

Do not merge, publish NuGet packages, deploy Azure, or promote branches. Keep the PR draft until its actual behavior and evidence have been reviewed.
