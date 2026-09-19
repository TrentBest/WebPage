namespace TheSingularityWorkshop.Gui;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Physical specimens presented by the gravity laboratory. The catalog is intentionally
/// dimensionless at the presentation boundary; SRPS/physics providers supply the actual
/// units and numerical integration when an experiment is executed.
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

    /// <summary>Builds a compact field strip between two bodies for the diegetic heatmap.</summary>
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
            var dominanceDelta = total <= 0 ? 1 : Math.Abs(influenceA - influenceB) / total;
            var intensity = Math.Clamp(1 - dominanceDelta, 0, 1);
            // This is a presentation-level interaction threshold. The eventual SRPS
            // integrator will replace it with a validated influence/Hill-sphere calculation.
            var interactionActive = intensity >= .65;
            result[i] = new SpatialGravityFieldSample(t, intensity, interactionActive, dominanceDelta);
        }

        return result;
    }
}

/// <summary>A drawable celestial specimen and its simplified orbital presentation.</summary>
public readonly record struct SpatialGravityBody(
    string Id,
    string Name,
    string Kind,
    double MassKg,
    string Color,
    double DisplayRadius,
    double OrbitDistance,
    double OrbitPhase);

/// <summary>One normalized point in the gravity-field presentation.</summary>
public readonly record struct SpatialGravityFieldSample(
    double PathPosition,
    double Intensity,
    bool GravityInteractionActive,
    double DominanceDelta);
