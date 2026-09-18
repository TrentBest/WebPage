namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Declarative manifest for the future Singularity Starship Experience.
/// </summary>
/// <remarks>
/// Singularity Station is the gateway; the starship is the next compositional scale.
/// Shipboard activity is modeled as environments containing things to observe and
/// things to operate. The interaction model deliberately allows a complex machine
/// to become an instrument: the environment itself is the interface.
/// </remarks>
public sealed record SpatialStarshipManifest(
    string Id,
    string Name,
    IReadOnlyList<SpatialStarshipStation> Stations,
    IReadOnlyList<SpatialStarshipVisualization> Visualizations,
    IReadOnlyList<string> CrewDomains)
{
    public static SpatialStarshipManifest CreateDefault()
        => new(
            "singularity-starship",
            "SINGULARITY STARSHIP",
            [
                new("bridge", "BRIDGE", "navigation", ["flight controls", "sensors", "communications"]),
                new("engineering", "ENGINEERING", "engineering", ["power", "propulsion", "systems"]),
                new("science", "SCIENCE", "science", ["observatory", "laboratory", "analysis"]),
                new("operations", "OPERATIONS", "operations", ["ship systems", "alerts", "routing"]),
                new("crew", "CREW COMMONS", "crew", ["coordination", "briefing", "social"])
            ],
            [
                new("planetary", "PLANETARY VIEW", "SOLAR SYSTEM"),
                new("stellar", "STELLAR VIEW", "STELLAR"),
                new("galactic", "GALACTIC VIEW", "GALACTIC"),
                new("universal", "UNIVERSAL VIEW", "UNIVERSAL")
            ],
            ["navigation", "engineering", "science", "operations", "education", "command"]);

    /// <summary>Returns the stations intended for a named crew domain.</summary>
    public IReadOnlyList<SpatialStarshipStation> ForCrewDomain(string domain)
        => [.. Stations.Where(x => string.Equals(x.CrewDomain, domain, StringComparison.OrdinalIgnoreCase))];
}

/// <summary>A human-scale shipboard station that exposes a focused interaction surface.</summary>
public readonly record struct SpatialStarshipStation(
    string Id,
    string Name,
    string CrewDomain,
    IReadOnlyList<string> Interactables);

/// <summary>A scale-changing visualization available from the ship.</summary>
public readonly record struct SpatialStarshipVisualization(
    string Id,
    string Name,
    string Scale);
