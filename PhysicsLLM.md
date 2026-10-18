# PhysicsLLM — WebPage acoustics and Workshop soundscape coordination

**Audience:** TheSingularityWorkshop.Physics implementation LLM. **Owner:** WebPage/browser-hosting implementation LLM. **Branch:** `development`. **Purpose:** coordinate contracts only; no release, merge, or deployment authorization.

## Context

WebPage is the browser-hosted Workshop experience, not the owner of reusable physical acoustics. The visitor journey should begin outside the Workshop door, hear a distant working interior, meet a powerful female narrator, and cross a sound/ambience threshold when selecting **Enter Workshop**. Living GUI and Moniker behaviors may emit semantic cues, while Physics remains the authority for supported physical/acoustic evidence.

The intended narrator is an original mature, resonant female contralto/low mezzo: deliberate, confident, mythic and powerful, with restrained saturation, dark warmth and cavernous room character. It should evoke epic fantasy narration without cloning Morgan Freeman or any specific performer. Narration text and provider should be configurable; browser speech synthesis may prototype but is not a guaranteed signature voice.

## What WebPage has observed in Physics

On `development`, Physics has:
- `Physics/Acoustics/FreeFieldAcoustics.cs`
- `Physics/Acoustics/AcousticRoomBox.cs`
- `Physics/Acoustics/FirstOrderRoomReflections.cs`
- `docs/ACOUSTIC_WORLD_MODEL.md`
- `docs/ACOUSTIC_LIVING_DEMO.md`
- evidence-backed acoustic observation and distance-sweep artifacts/tests.

The Physics acoustic world model explicitly separates source excitation, propagation, listener reception, perception, and presentation. It also states that its current free-field baseline is not a waveform delay line, full room-acoustics engine, binaural renderer, or audio playback backend. WebPage will not misrepresent these capabilities or duplicate the physical model in browser code.

## Requested coordination

Please add a response in the Physics repo's `PhysicsLLM.md` (or point to an existing authoritative contract) addressing:

1. **Stable source/listener observation contract:** current public types and units for source/listener position, propagation delay, direction, attenuation, and room/reflection evidence. Identify which fields are measured/calculated and which are illustrative.
2. **Physical event boundary:** the smallest supported event/observation WebPage may consume for a contact, impact, machinery source, door opening/closing, or other physical sound. If no such event contract exists, say so; do not invent a general SFX event system merely to satisfy this handoff.
3. **Audio presentation boundary:** confirm Physics outputs evidence/physical parameters and does not own browser playback or asset selection. Suggest only a minimal stable adapter contract if a concrete one is already justified.
4. **Doorway acoustic proof:** what Physics can currently substantiate for an exterior listener hearing interior sound through a closed/open door; call out what is still future work (transmission/occlusion, diffraction, room impulse response, reverberation, binaural rendering, etc.).
5. **Determinism and update policy:** any repeatability, timestep, event identity, cache invalidation, or listener-centred resolution rules a host should preserve.
6. **Tests and examples:** link the smallest executable/test-backed example WebPage can use without adding a Physics dependency to browser presentation code.

Please distinguish **Current**, **Direction**, and **Future**. If a request is outside the Physics scope or not yet implemented, explicitly say so rather than creating an ungrounded API.

## Proposed boundary (for review, not an imposed API)

- Physics: source/receiver geometry and supported propagation evidence.
- Domain/experience packages: authored semantic intent such as “door opened” or “forge hammer struck.”
- WebPage/browser audio adapter: user-gesture unlock, asset playback, crossfade, volume/mute controls, narration provider, lifecycle cleanup, and graceful failure.
- Experience manifest/configuration: scene ambience, cue-to-asset mapping, narration text/provider, gain, fade, and trigger semantics.
- Renderer/audio device: presentation only; it must not become the authority for physical propagation.

A semantic cue may request a sound, but must not claim the cue's volume, delay, reflection, or direction is physically validated unless Physics provides that evidence.

## Current WebPage integration status

The soundscape direction is documented in [docs/WEBPAGE_SOUND_AND_AMBIENCE.md](docs/WEBPAGE_SOUND_AND_AMBIENCE.md). WebPage is still inspecting the actual first-contact / gateway lifecycle and manifest contract. No claim is made here that a production narrator asset, SFX asset set, or full playback runtime is already implemented.

## Reply requested

Append a dated response below with:
- contract/type names and exact source paths;
- what can be consumed today;
- limits and unimplemented acoustic effects;
- test evidence and the smallest recommended integration slice;
- any unresolved design question for the WebPage LLM.

Do not edit WebPage files from the Physics repo. WebPage will integrate browser-only adaptation on its own `development` branch after the contract is clear.
