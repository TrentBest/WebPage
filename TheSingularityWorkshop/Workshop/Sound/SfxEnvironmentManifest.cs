using System;
using System.Collections.Generic;

namespace TheSingularityWorkshop.Workshop.Sound;

/// <summary>
/// Describes an acoustic space separately from the sound sources placed inside it.
/// This is the boundary the future SFX Studio can author and the runtime can project.
/// </summary>
public sealed record SfxEnvironmentManifest(
    string Id,
    string Name,
    double ReverbSeconds,
    double EarlyReflectionDensity,
    double ReflectionGain,
    double LowFrequencyResonanceHz)
{
    public static SfxEnvironmentManifest LargeCavern =>
        new("ENV.CAVERN.001", "Large Open Cave", 5.8, .82, .72, 112.0);
}

public sealed record SfxEmitter(
    string Id,
    string Name,
    double X,
    double Y,
    double Z,
    string Source);

/// <summary>Sound sources projected into an authored acoustic environment.</summary>
public sealed class SfxScene
{
    public SfxScene(SfxEnvironmentManifest environment, IEnumerable<SfxEmitter> emitters)
    {
        Environment = environment ?? throw new ArgumentNullException(nameof(environment));
        Emitters = new List<SfxEmitter>(emitters ?? throw new ArgumentNullException(nameof(emitters)));
    }

    public SfxEnvironmentManifest Environment { get; }
    public IReadOnlyList<SfxEmitter> Emitters { get; }

    public static SfxScene FirstContactCavern() => new(
        SfxEnvironmentManifest.LargeCavern,
        new[]
        {
            new SfxEmitter("SRC.MONIKER.WATER", "Moniker water", 0, 0, 0, "lapping-water"),
            new SfxEmitter("SRC.PLANT.VINE", "Vine growth", -1, 0, 0, "rumbling-growth"),
            new SfxEmitter("SRC.PLANT.BLOOM", "Flower formation", 0, 1, 0, "bloom-resonance"),
            new SfxEmitter("SRC.PLANT.RIP", "Panel opening", 0, 1, 0, "fabric-rip")
        });
}
