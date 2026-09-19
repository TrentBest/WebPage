namespace TheSingularityWorkshop.Gui;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Physical specimens presented by the gravity laboratory.
/// Presentation uses an explicit influence radius; the radius is a visualization boundary,
/// not a claim that gravity becomes zero outside it.
/// </summary>
public sealed record SpatialGravityBodyCatalog(IReadOnlyList<SpatialGravityBody> Bodies)
{
    public static SpatialGravityBodyCatalog CreateDefault()
        => new(
        [
            new("sol", "SOL", "star", 1.989e30, "#ffd34d", 8, 24, 0),
            new("mercury", "MERCURY", "planet", 3.3011e23, "#a8a8a8", 1.2, 6, 0.20),
            new("venus", "VENUS", "planet", 4.8675e24, "#e3a24a", 1.8, 9, 0.38),
            new("earth", "EARTH", "planet", 5.9722e24, "#4da6ff", 2.2, 12, 0.55),
            new("moon", "MOON", "moon", 7.342e22, "#d8d8d8", .9, 3.2, 0.08),
            new("mars", "MARS", "planet", 6.4171e23, "#d65a3a", 1.5, 15, 0.82),
            new("ceres", "CERES", "dwarf", 9.393e20, "#9b8269", .7, 19, 0.14),
            new("jupiter", "JUPITER", "planet", 1.8982e27, "#d9b58c", 4.8, 28, 1.25),
            new("saturn", "SATURN", "planet", 5.6834e26, "#e6d0a3", 4.1, 35, 1.55),
            new("uranus", "URANUS", "planet", 8.681e25, "#76d7e8", 2.7, 42, 1.88),
            new("neptune", "NEPTUNE", "planet", 1.02413e26, "#4f7de8", 2.6, 49, 2.14)
        ]);

    public SpatialGravityBody? Find(string id)
        => Bodies.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Returns samples between two bodies. Each body's contribution is represented by its
    /// inverse-square influence; the direct-interaction band is the region where neither
    /// contribution has fallen below the configured ignore fraction.
    /// </summary>
    public IReadOnlyList<SpatialGravityFieldSample> Heatmap(string bodyAId, string bodyBId, int samples = 64)
    {
        var a = Find(bodyAId) ?? throw new KeyNotFoundException(bodyAId);
        var b = Find(bodyBId) ?? throw new KeyNotFoundException(bodyBId);
        samples = Math.Clamp(samples, 8, 512);

        var result = new SpatialGravityFieldSample[samples];
        for (var i = 0; i < samples; i++)
        {
            var t = i / (double)(samples - 1);
            var distanceA = Math.Max(.001, Math.Abs(a.OrbitDistance * (1 - t)));
            var distanceB = Math.Max(.001, Math.Abs(b.OrbitDistance * t));
            var influenceA = a.MassKg / (distanceA * distanceA);
            var influenceB = b.MassKg / (distanceB * distanceB);
            var total = influenceA + influenceB;
            var normalizedA = total <= 0 ? 0 : influenceA / total;
            var normalizedB = total <= 0 ? 0 : influenceB / total;
            var dominanceDelta = Math.Abs(normalizedA - normalizedB);
            var interaction = Math.Min(normalizedA, normalizedB);
            var active = interaction >= SpatialGravityFieldSample.IgnoreFraction;

            result[i] = new SpatialGravityFieldSample(
                t,
                interaction,
                active,
                dominanceDelta);
        }

        return result;
    }
}

public readonly record struct SpatialGravityBody(
    string Id,
    string Name,
    string Kind,
    double MassKg,
    string Color,
    double DisplayRadius,
    double OrbitDistance,
    double OrbitPhase);

/// <summary>One normalized point in the pairwise gravity presentation.</summary>
public readonly record struct SpatialGravityFieldSample(
    double PathPosition,
    double Intensity,
    bool GravityInteractionActive,
    double DominanceDelta)
{
    /// <summary>
    /// Presentation cutoff: below half of the pair's normalized contribution, the weaker
    /// contribution is treated as visually negligible. The SRPS integrator remains authoritative.
    /// </summary>
    public const double IgnoreFraction = 0.5;
}
