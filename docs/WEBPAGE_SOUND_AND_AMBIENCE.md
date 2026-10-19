# WebPage Soundscape and Sensory Manifest

![Living GUI documentation lens: question, model, implementation, visible manifestation, and proof](https://raw.githubusercontent.com/TrentBest/WebPage/development/docs/assets/living-gui-documentation-lens.svg)

> **Evidence class:** architecture illustration, not live runtime telemetry. Follow the subject-specific source, view, fixture, or test below; if none exists, record that gap rather than implying it is implemented.


## Purpose

Give the Workshop an audible identity without turning sound into hard-coded page behavior. WebPage is the browser-hosted manifestation of WebApp; WebApp should host an AnyApp-style experience definition. Sound, speech, visual animation, and interaction are sensory manifestations of experience semantics, not unrelated UI callbacks.

> The visitor should hear the Workshop before crossing its threshold — and the moniker should feel alive, not merely play a file.

This document defines the intended architecture and first implementation slice. It does not claim that the sound runtime is already implemented.

## Experience sequence

1. **First contact / outside the door** — distant, low-volume mechanical room tone; soft hum, occasional restrained clank, faint tool activity. The Enter Workshop control remains the visual focal point.
2. **Welcome / orientation** — optional narrator reads configured welcome labels in order. Speech is opt-in and must never unexpectedly talk over the visitor.
3. **Threshold / Enter Workshop** — one short latch/door transition, then a crossfade from muffled exterior ambience to a fuller interior workshop bed.
4. **Living Workshop** — contextual cues accompany living GUI interactions: subtle clicks for controls, small mechanical confirmations for state transitions, restrained electrical/tonal cues for signal or energy effects, and domain-specific SFX for simulations.
5. **Moniker** — the Workshop moniker can own a subtle tonal identity and interaction cues, with its animation and sound driven by the same semantic events where practical.

## Architecture

Keep three concerns separate:

- **Experience manifest:** declares soundscapes, cues, narration, triggers, and presentation intent.
- **Audio runtime:** resolves manifest IDs to browser audio resources, manages volume, concurrency, playback policy, crossfades, and disposal.
- **Experience/UI state:** emits semantic events such as `welcome.presented`, `workshop.enter.requested`, `workshop.entered`, `gui.control.activated`, and `moniker.reacted`. It should not know file paths or audio implementation details.

Suggested first manifest location: `TheSingularityWorkshop/wwwroot/experiences/workshop-soundscape.json` (or the established experience-manifest location if the current runtime already defines one). Do not introduce a second competing manifest system: inspect and extend the existing registry/configuration model.

### Illustrative manifest

This is a design example, not a claim that these exact fields already exist in the runtime:

```json
{
  "id": "workshop.soundscape",
  "version": 1,
  "enabled": true,
  "defaults": {
    "masterVolume": 0.65,
    "ambienceVolume": 0.22,
    "effectsVolume": 0.55,
    "narrationVolume": 0.8,
    "narrationEnabled": false,
    "respectReducedMotion": true
  },
  "scenes": {
    "first-contact": {
      "ambience": "workshop.exterior",
      "onEnter": ["workshop.exterior.start"]
    },
    "workshop": {
      "ambience": "workshop.interior",
      "onEnter": ["workshop.door.open", "workshop.interior.start"]
    }
  },
  "cues": {
    "workshop.exterior.start": {
      "kind": "loop",
      "asset": "audio/ambience/workshop-exterior.ogg",
      "gain": 0.22
    },
    "workshop.door.open": {
      "kind": "effect",
      "asset": "audio/sfx/industrial-door-open.ogg",
      "gain": 0.55
    },
    "gui.control.activated": {
      "kind": "effect",
      "asset": "audio/sfx/soft-mechanical-click.ogg",
      "gain": 0.28
    }
  },
  "narration": {
    "enabledByDefault": false,
    "voice": "browser-or-configured-voice",
    "lines": [
      { "id": "welcome", "text": "Welcome to the Singularity Workshop." },
      { "id": "enter", "text": "Enter the Workshop." }
    ]
  }
}
```

The final schema must be validated against the real WebApp/AnyApp manifest conventions before adoption. Asset IDs are logical identities; asset paths belong to the manifest/resource layer, not component code.

## Browser behavior and user agency

- Browsers commonly block audible autoplay. Start audio only after a clear user gesture; the Enter Workshop click is the natural activation point.
- The pre-entry atmosphere must be silent until the visitor has interacted with the page. The button can communicate that sound is available without secretly starting it.
- Narration is opt-in, skippable, and never required to understand the interface. Do not automatically read every hover or every label.
- Provide a persistent, discoverable sound control with mute/unmute and volume. Remember preferences only through an approved, privacy-conscious preference mechanism.
- Respect system reduced-motion/accessibility preferences where relevant; also respect a user’s explicit sound preference. Do not infer that reduced motion means sound should be disabled.
- Avoid overlapping narration, looping SFX, repeated triggers, audio stacking during re-render, and abrupt scene transitions.
- If an asset fails to load or audio is unsupported, the experience must continue normally with no blocking error.
- Do not download large audio assets until needed. Prefer short, compressed, licensed/original assets; document attribution and license for every non-original asset.
- Avoid playing sound on hover by default. Use deliberate activation or meaningful state transitions; keep cues short and quiet.

## Narrator voice direction: Female Power

The welcome narrator should project **female power**: the mature, resonant authority of a great documentary storyteller, combined with the dark, mythic, battle-worn atmosphere of the original *Warcraft: Orcs & Humans* opening. The target is an original voice, not a direct imitation or clone of Morgan Freeman or any specific performer.

- **Voice character:** mature contralto or low mezzo; full chest resonance, controlled grain, clear diction, and a little breath at phrase edges.
- **Performance:** measured, deliberate, assured. Let important words land; avoid rushed announcer cadence, chirpy assistant energy, and constant theatrical growling.
- **Emotional register:** awe, conviction, restrained menace only where appropriate, and the feeling that the Workshop has a long history and a serious purpose.
- **Processing:** subtle saturation and compression, gentle low-mid warmth, restrained darkening, and a short cavernous/stone-and-metal reverb. Preserve intelligibility; avoid excessive bass, distortion, or muddy consonants.
- **Spatial story:** before entry, the narrator can feel as if heard through the heavy Workshop door. On entry, the voice becomes clearer and more present as the ambience opens up.
- **Pacing:** short configured lines, with pauses between them. Never narrate every hover or read the whole interface automatically.

**Provider strategy:** keep narration behind a provider abstraction. Browser speech synthesis is useful for a quick functional prototype, but available voices and prosody vary by browser and operating system, and its output cannot be assumed to pass through the same effects chain as ordinary audio assets. For a consistent signature voice, prefer a properly licensed/generated narration asset (or a separately configured speech service) that can be processed and mixed through the audio runtime. Keep the narration text in the manifest so the provider can be changed without rewriting the experience.

**Example delivery direction (not required final script):** “Beyond this door, ideas become instruments… and instruments become worlds.” Deliver it with calm authority, a deliberate pause, and a rising sense of scale—not as a shout.

## Sound palette

| Domain | Sound character | Trigger policy |
|---|---|---|
| Exterior Workshop | Muffled hum, distant machinery, sparse metal clanks | Begins only after the first explicit audio-enabling gesture |
| Welcome narration | Calm, curious, human, measured | Explicit play/continue action; one line at a time |
| Threshold | Latch, heavy door, acoustic opening | Once on successful entry transition |
| Workshop ambience | Warm mechanical hum, tools, soft clatter, productive room tone | Loop while inside; crossfade rather than stack |
| Living GUI | Tactile clicks, snaps, soft confirmations | Meaningful activation/state transition; throttle repeats |
| Visualization / physics | Cue families such as roll, impact, rattle, energy pulse | Triggered by semantic simulation events, not every rendered frame |
| Moniker | Subtle tonal bed and tiny response accents | Optional, restrained, synchronized with meaningful moniker events |

## Implementation plan

1. Inspect the current Development branch’s WebPage/AnyApp configuration and welcome flow. Reuse existing manifest abstractions and current SFX work instead of creating a parallel architecture.
2. Define a small typed manifest model and validation/default behavior only where the existing architecture requires it.
3. Implement one lifecycle-owned browser audio service: preload-on-demand, single scene ambience, crossfade, short effects, volume controls, cleanup, and safe failure behavior.
4. Wire the first-contact → enter-workshop sequence using semantic events. Keep narration disabled by default until the UI explicitly enables it.
5. Add a visible sound toggle and narration play/skip controls; ensure mute stops both ambience and effects, and narration has a distinct control.
6. Add a tiny initial licensed/original sound set and an asset attribution file. Never use arbitrary remote URLs as implicit sound assets.
7. Test user-gesture activation, repeated clicks, navigation/re-render, mute, missing assets, reduced-motion preferences, and no duplicate loops.
8. Expand the same event/cue vocabulary to living GUIs and the moniker after the initial journey works reliably.

## Acceptance criteria

- The welcome page remains silent until the visitor explicitly enables audio or enters through a clearly documented audio gesture.
- Exterior ambience feels distant; entry transitions to interior ambience without overlapping loops or a jarring volume jump.
- Narration can be started, skipped, muted, and disabled independently.
- Sound behavior is configured through the experience manifest, not scattered component-specific file paths.
- Existing visual animation and navigation still work when audio is muted, unavailable, or fails.
- SFX are attached to semantic events and are not emitted every render/frame.
- All new assets have known provenance and licensing.
- Build and relevant tests pass on Development before promotion.

## Open architectural questions

- Which manifest/configuration contract is currently authoritative for WebApp and AnyApp?
- Does the current Physics/SFX work already define an asset catalogue or event vocabulary that WebPage should consume?
- Should browser speech synthesis be the first narration provider, or should narration assets be bundled for consistent voice and timing? Keep the provider replaceable.
- Which existing living-GUI events are stable enough to become shared sound cue IDs?

## Non-goals for the first slice

- A full adaptive music system.
- Mandatory speech or autoplay.
- Hard-coding sound into every page/component.
- Coupling the semantic experience model to Web Audio API details.
- Claiming spatial audio, 3D positioning, or a custom voice engine before those capabilities exist.
