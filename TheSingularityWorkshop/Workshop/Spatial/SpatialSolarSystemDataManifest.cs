namespace TheSingularityWorkshop.Gui;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Physical reference data used to keep the Workshop's solar-system representation
/// grounded in published NASA planetary facts. The map remains intentionally schematic;
/// these values are the semantic data behind the representation.
/// </summary>
/// <remarks>
/// The planetary values are transcribed from NASA Goddard's Planetary Fact Sheet - Metric.
/// Dynamic positions and ephemerides should come from JPL Horizons rather than being
/// hand-authored here.
/// </remarks>
public sealed record SpatialSolarSystemDataManifest(
    string SourceName,
    string SourceUrl,
    string EphemerisProvider,
    string EphemerisUrl,
    IReadOnlyList<SpatialPlanetaryReferenceData> Bodies)
{
    public static SpatialSolarSystemDataManifest Nasa { get; } = new(
        "NASA NSSDCA Planetary Fact Sheet - Metric",
        "https://nssdc.gsfc.nasa.gov/planetary/factsheet/",
        "NASA/JPL Solar System Dynamics - Horizons",
        "https://ssd.jpl.nasa.gov/horizons/",
        [
            new("mercury", "MERCURY", 0.330e24, 4879, 5429, 3.7, 4.3, 57.9, 88.0, 47.4),
            new("venus", "VENUS", 4.87e24, 12104, 5243, 8.9, 10.4, 108.2, 224.7, 35.0),
            new("earth", "EARTH", 5.97e24, 12756, 5514, 9.8, 11.2, 149.6, 365.2, 29.8),
            new("mars", "MARS", 0.642e24, 6792, 3934, 3.7, 5.0, 228.0, 687.0, 24.1),
            new("jupiter", "JUPITER", 1898e24, 142984, 1326, 23.1, 59.5, 778.5, 4331, 13.1),
            new("saturn", "SATURN", 568e24, 120536, 687, 9.0, 35.5, 1432.0, 10747, 9.7),
            new("uranus", "URANUS", 86.8e24, 51118, 1270, 8.7, 21.3, 2867.0, 30589, 6.8),
            new("neptune", "NEPTUNE", 102e24, 49528, 1638, 11.0, 23.5, 4515.0, 59800, 5.4),
            new("pluto", "PLUTO", 0.0130e24, 2376, 1850, 0.7, 1.3, 5906.4, 90560, 4.7)
        ]);

    public SpatialPlanetaryReferenceData? Find(string id)
        => Bodies.FirstOrDefault(x => x.Id == id);
}

/// <summary>NASA reference measurements for one planetary body.</summary>
public readonly record struct SpatialPlanetaryReferenceData(
    string Id,
    string Name,
    double MassKg,
    double DiameterKm,
    double DensityKgPerM3,
    double SurfaceGravityMS2,
    double EscapeVelocityKmPerS,
    double MeanDistanceFromSunMillionKm,
    double OrbitalPeriodDays,
    double OrbitalVelocityKmPerS);
