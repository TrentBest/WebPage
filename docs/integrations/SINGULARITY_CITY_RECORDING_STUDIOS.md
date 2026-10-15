# Singularity City Recording Studios — Vision and Ownership

## Purpose

Make recording and music creation a first-class, reusable Experience in Singularity City and The Singularity Workshop. The first visible destination should be the Workshop's own recording studio; the architecture must also let independent creators establish distinct studio spaces without requiring the Workshop to own, approve, or economically operate every one.

This is a WebPageLLM handoff. It describes the intended Experience and staged work, not a claim that a recording studio, full audio workstation, music hosting service, or acoustic room solver already exists.

## Why this belongs in the city

A city becomes more convincing when people can make culture there, not only move through buildings or inspect technology. A studio can connect:

- the Workshop as a place to learn, record, and demonstrate the ecosystem;
- individual creators and fan communities;
- bands, musicians, podcasters, voice performers, sound designers, and other audio creators;
- rehearsal rooms, listening rooms, small venues, live events, and city festivals;
- optional connections to creator services, event tickets, commissions, and Singularity City Economy.

Treat the recording studio as a reusable capability and destination, not a hard-coded one-off screen. A studio may be an interior/place in the city and a functional workspace with its own configuration and identity.

## Current technical foundation: Physics acoustics

The Physics repository has a draft, unmerged first implementation at [PR #21 — Add first deterministic free-field acoustics baseline](https://github.com/TrentBest/TheSingularityWorkshop.Physics/pull/21), on branch `work/acoustic-free-field` based on `work/hydrodynamics-wet-dry`.

The first baseline provides:

- metre-based 3D positions for acoustic sources and listeners;
- a deterministic observation calculation with propagation delay using a default speed of sound of 343 m/s;
- a free-field sound-pressure-level estimate that falls by approximately 6 dB for each doubling of distance, using a 20 log10 distance relationship;
- a normalized direction vector from listener toward source, where a direction exists;
- focused tests authored for propagation delay, attenuation, direction, coincident positions, near-field reference-level clamping, and invalid input.

The implementation is an analytical observation model, **not recorded or generated audio**. It does not yet implement audio capture, a sampled signal pipeline, playback, mixing, waveform editing, room impulse responses, frequency-dependent material absorption, reflections, occlusion, Doppler, binaural rendering, acoustic baking, or agent-level interpretation. The PR's CI/build/test status must be checked live; do not describe the tests as passing unless the current workflow proves it.

The broader design guide is [Physics acoustic world model](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/work/acoustic-free-field/docs/ACOUSTIC_WORLD_MODEL.md). It frames sound as source → propagation → listener → perception → presentation, with resolution adapted to what matters around a listener.

## Experience concept

### 1. Workshop Recording Studio

Create a first inspectable studio destination connected to the existing WebPage experience/gallery architecture. Its initial purpose is to demonstrate the workflow and data model—not to pretend that a complete digital audio workstation exists.

A useful first presentation can show:

- a studio identity and room layout;
- distinct zones such as live room, vocal booth, control room, instrument area, and listening position;
- named audio sources and listener positions;
- source-to-listener distance, direction, propagation delay, and the current free-field level estimate when the Physics adapter is available;
- clear status labels separating live model output from illustrative fixture data;
- an obvious capability roadmap for recording, signal playback, room treatment, and collaboration.

If audio capture/playback is not implemented, the initial destination must be described as an **Acoustic Studio Lab / studio prototype**, not as a functional recording console. A static fixture can demonstrate layout, but it must be labeled as fixture data.

### 2. Creator-owned studios

Design the data model so the Workshop studio is one instance of a general studio definition, not a special case. A future creator could establish a studio with:

- creator/display identity and optional verified external links;
- studio name, description, style, room configuration, and available capabilities;
- private, invite-only, or public discovery settings;
- rooms or stations, source/listener positions, session metadata, and allowed participants;
- explicit ownership and editing permissions;
- optional event, marketplace, or Economy integration.

Fans should be able to create fan-run studios and tribute spaces, but the interface must distinguish fan-created spaces from official artist or label spaces. Never imply endorsement, partnership, ownership, or identity verification merely because a real-world name or logo is entered. Verified identity should be a separate future capability with a defined process.

### 3. Sessions, collaboration, and public culture

The long-term direction can include multitrack sessions, overdubs, takes, editing, shared sessions, audio import/export, listening rooms, scheduled live events, and connections to the Market/Craft Fair, Musical Venue, and other Experiences. These are roadmap items until implemented and tested.

A studio may also be a small business in the simulated city—renting time, offering mixing services, or hosting a ticketed session—but that is optional. A creator can use a studio as a free creative space without activating an Economy integration. Economy must be a composable capability, not a mandatory prerequisite for making or sharing art.

## Architectural ownership boundaries

- **Physics** owns reusable physical/acoustic observation and future signal/room simulation primitives. Keep it independent of WebPage, UI, and creator social features.
- **WebPage** owns the browser Experience, navigation, studio editor/presentation, and host-side interaction adapters.
- **FSM_API / FSM_COS / MicroBundleDomain** retain their existing state execution and composition responsibilities. Do not create a parallel simulation heartbeat in a Razor component.
- **Renderer / Visualize** may later provide spatial presentation; they are not prerequisites for an honest first acoustic studio model.
- **Creator identity, access control, sharing, hosting, moderation, licensing, and monetization** belong above Physics and require explicit design. Do not store those concerns in acoustic source/listener classes.

Follow the existing manifest/gallery pattern where it fits. Lazy-load the destination through existing navigation; preserve the Workshop opening journey, Living GUI, ProtocolAi clipboard workflow, and current Experiences. Do not replace or fork the host just to add this destination.

## A realistic sequence

1. **Inspect the current WebPage development branch and integration conventions.** Identify the existing ExperienceGallery/manifest route and relevant browser smoke coverage before choosing exact source paths.
2. **Build a clearly labeled Studio Lab vertical slice.** Use a general studio definition and a deterministic, inspectable room/source/listener fixture. Show calculated values only when they come from the actual Physics baseline; label any fallback as illustrative.
3. **Test the actual visitor journey.** Cover navigation, room/source selection, units, calculated delay/level/direction, responsive layout, and preservation of existing navigation.
4. **Add the signal layer only when its contracts exist.** Define sample rate, channel layout, buffer ownership, deterministic time semantics, and tests before calling anything a recording engine.
5. **Grow into room acoustics.** Add validated material/occlusion/reflection representations and cache invalidation as separate increments; do not simulate a fine grid everywhere simply because a room exists.
6. **Generalize studio creation.** Add creator-owned definitions, persistence, permissions, public/private discovery, and clear official-versus-fan identity labeling only when the backing services and safety rules are real.
7. **Add collaboration and optional Economy integration.** Keep audio creation usable without a city business account or transaction.

## Acceptance criteria for the first WebPage slice

- A visitor can reach the Studio Lab through existing WebPage navigation without breaking current Experiences.
- The Workshop studio is data-driven and does not require a hard-coded special branch for every future creator.
- Source and listener positions use metres and produce deterministic observations through the actual Physics model when that integration is available.
- The UI explains what propagation delay and free-field attenuation mean, and does not equate a dB estimate with a recorded waveform or perceived loudness guarantee.
- Any fixture, mock, unavailable package, or future capability is visibly identified.
- Browser smoke or focused tests cover the visitor path and model-derived values.
- No real-world artist is presented as participating or officially represented without an actual authorized basis.
- Economy is optional.
- No merge, package publication, deployment, or release is implied by this handoff.

## Guiding idea

A recording studio is a place to create, collaborate, and be heard. Physics can help the city understand how sound travels; WebPage can make the studio tangible; creator-owned definitions can let the ecosystem grow beyond the Workshop itself. Keep those responsibilities separate so the experience can grow without making promises the implementation has not yet earned.
