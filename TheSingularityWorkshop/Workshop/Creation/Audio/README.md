# Workshop Audio & Performing Arts

Audio is not merely a resource to attach to an Experience. It is a composition domain.

The Workshop should give a creator the virtual equivalent of the facilities required to
make a film, score a scene, perform music, design Foley, or construct an entirely new
sound.

## Singularity Performing Arts Center

The Workshop campus should contain a **Singularity Performing Arts Center** with spaces
for:

- orchestration and composition;
- rehearsal and performance;
- instrument/ensemble arrangement;
- recording;
- mixing and mastering;
- sound design;
- Foley and prop recording;
- voice and dialogue;
- playback and review.

The center is a creation environment, not a single audio editor.

### Score composition

A creator should be able to work with music at several levels:

    IDEA
     |
     v
    ARRANGEMENT
     |
     +-- conductor / timeline
     +-- instruments / sections
     +-- sheet music
     +-- notes
     +-- tempo / meter
     |
     v
    PERFORMANCE
     |
     v
    RECORD / SYNTHESIZE
     |
     v
    MIX
     |
     v
    AUDIO RESOURCE

The durable model should describe the composition rather than capture only the resulting
waveform.

AudioTrack and AudioNote provide the first semantic boundary for that model. A future
renderer may turn the same notes into MIDI, synthesized audio, notation, or a live
performance representation.

This supports the envisioned workflow: place notes on a sheet, assign who plays what,
arrange the sections, press play, and hear the resulting composition.

## SFX / Foley Studio

The Workshop should also provide a dedicated **SFX / Foley Studio**.

A creator can enter with an intent such as:

> "I need the sound of a fairy godmother arriving."

The studio can expose:

    SFX STUDIO
     |
     +-- PROP WALL
     |    +-- doors
     |    +-- cloth
     |    +-- wood
     |    +-- metal
     |    +-- glass
     |
     +-- RECORDING
     +-- LAYERING
     +-- SYNTHESIS / GENERATION
     +-- TIMELINE
     +-- PREVIEW
     +-- SAVE RECIPE

A sound should be creatable from layers rather than forcing every sound to be a single
opaque file. SoundEffectRecipe is the durable boundary for that idea.

For example, a magical arrival might eventually be represented as:

    fairy-arrival
     |
     +-- bell sparkle
     +-- rising shimmer
     +-- soft impact
     +-- voice cue
     +-- music transition

The individual sources can be recorded, synthesized, generated, or reused from the
Workshop library.

## Film and animation workflow

The same audio system should plug into the Movie Lot / Animation pipeline:

    SCENE
     |
     +-- actors / puppets
     +-- dialogue
     +-- camera
     +-- lighting
     +-- special effects
     |
     +-- AUDIO
          |
          +-- score
          +-- ambience
          +-- dialogue
          +-- Foley
          +-- SFX
          +-- music transitions
          |
          v
       TIMELINE
          |
          v
       FINAL EXPERIENCE / FILM

An actor entering a scene should not require the creator to understand the audio system.
The creator describes the intent; the Workshop presents the relevant composition surfaces.

For the fairy-godmother example:

    Actor: Fairy Godmother
            |
            +-- arrival cue
            +-- visual effect
            +-- dialogue
            +-- music transition
            +-- SFX layers

The deterministic side owns timing, ordering, dependencies, and playback. AI can help
propose candidate sounds, arrangements, instrumentation, or transitions, but the saved
composition remains an explicit definition that the creator can inspect and edit.

## Why this belongs in the Workshop

The larger principle is:

> **Give the creator the virtual equivalent of the tools they would need in reality.**

A person who wants to make a movie should not receive a generic "media upload" box. They
should encounter actors, a stage, cameras, props, wardrobe, effects, sound, music, editing,
and review.

A person who wants to make music should encounter instruments, notation, arrangement,
performance, recording, and mixing.

A person who wants to invent a sound should encounter props, microphones, layers,
synthesis, and a timeline.

The Workshop can expose all of these as Experiences while keeping the underlying
definitions platform-neutral and reusable.
