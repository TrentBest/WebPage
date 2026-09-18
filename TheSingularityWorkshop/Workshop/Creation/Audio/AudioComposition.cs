using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TheSingularityWorkshop.Workshop.Creation.Audio;

/// <summary>
/// Declarative audio composition owned by the Workshop.
/// A composition can contain musical tracks and discrete sound-effect cues without
/// depending on a particular audio engine or runtime host.
/// </summary>
public sealed class AudioComposition
{
    public AudioComposition(string name, IEnumerable<AudioTrack>? tracks = null, IEnumerable<SoundEffectCue>? soundEffects = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Audio composition name is required.", nameof(name));

        Name = name;
        Tracks = new ReadOnlyCollection<AudioTrack>((tracks ?? Enumerable.Empty<AudioTrack>()).ToList());
        SoundEffects = new ReadOnlyCollection<SoundEffectCue>((soundEffects ?? Enumerable.Empty<SoundEffectCue>()).ToList());
    }

    public string Name { get; }
    public IReadOnlyList<AudioTrack> Tracks { get; }
    public IReadOnlyList<SoundEffectCue> SoundEffects { get; }
}

/// <summary>A declarative musical track containing ordered notes.</summary>
public sealed class AudioTrack
{
    public AudioTrack(string name, string instrument, IEnumerable<AudioNote>? notes = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Audio track name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(instrument))
            throw new ArgumentException("Audio track instrument is required.", nameof(instrument));

        Name = name;
        Instrument = instrument;
        Notes = new ReadOnlyCollection<AudioNote>((notes ?? Enumerable.Empty<AudioNote>()).ToList());
    }

    public string Name { get; }
    public string Instrument { get; }
    public IReadOnlyList<AudioNote> Notes { get; }
}

/// <summary>One symbolic musical note in a Workshop score.</summary>
public sealed record AudioNote(
    string Pitch,
    double StartBeat,
    double DurationBeats,
    int Octave = 4,
    double Velocity = 1.0);

/// <summary>
/// A timed sound-effect event. Source may identify a resource, generated sound,
/// Foley recipe, or future synthesis provider.
/// </summary>
public sealed record SoundEffectCue(
    string Name,
    string Source,
    double StartSeconds,
    double DurationSeconds = 0);
