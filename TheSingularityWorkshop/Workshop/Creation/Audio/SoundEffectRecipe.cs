using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TheSingularityWorkshop.Workshop.Creation.Audio;

/// <summary>
/// Declarative Foley/sound-design recipe for constructing a sound from sources.
/// Sources can represent physical props, recordings, synthesized signals, or
/// generated material without coupling the definition to an audio engine.
/// </summary>
public sealed class SoundEffectRecipe
{
    public SoundEffectRecipe(string name, IEnumerable<SoundSourceLayer> layers)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Sound-effect recipe name is required.", nameof(name));

        Name = name;
        Layers = new ReadOnlyCollection<SoundSourceLayer>(
            (layers ?? throw new ArgumentNullException(nameof(layers))).ToList());
    }

    public string Name { get; }
    public IReadOnlyList<SoundSourceLayer> Layers { get; }
}

/// <summary>One source layer in a sound-effect recipe.</summary>
public sealed record SoundSourceLayer(
    string Source,
    double OffsetSeconds = 0,
    double Gain = 1.0,
    double PitchShift = 0);
