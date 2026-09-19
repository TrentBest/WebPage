namespace TheSingularityWorkshop.Gui;

/// <summary>Very early world-scale placeholders surrounding Singularity City.</summary>
/// <remarks>These are not claims of completed geography; they are construction markers for the future world layer.</remarks>
public static class SpatialWorldRegionCatalog
{
    public static IReadOnlyList<SpatialWorldRegion> Regions =>
    [
        new("north-american-network", "North American Network", new SpatialBounds(8, 10, 22, 20), "Regional cities, ports, airports, roads, and digital infrastructure."),
        new("atlantic-network", "Atlantic Network", new SpatialBounds(37, 16, 18, 15), "Future intercontinental transport and communication corridors."),
        new("european-network", "European Network", new SpatialBounds(62, 9, 22, 18), "Dense future city and transit network spanning multiple metropolitan regions."),
        new("pacific-network", "Pacific Network", new SpatialBounds(70, 39, 22, 18), "Future maritime, aviation, and technology corridor across the Pacific."),
        new("south-american-network", "South American Network", new SpatialBounds(15, 47, 20, 17), "Future metropolitan, ecological, agricultural, and transport systems."),
        new("african-network", "African Network", new SpatialBounds(43, 43, 18, 20), "Future urban, energy, research, and continental transport network."),
        new("asian-network", "Asian Network", new SpatialBounds(64, 64, 24, 20), "Future megacity, manufacturing, research, and transit network."),
        new("oceanic-network", "Oceanic Network", new SpatialBounds(28, 72, 18, 15), "Future island, marine, and deepwater infrastructure."),
    ];
}

public readonly record struct SpatialWorldRegion(
    string Id,
    string Name,
    SpatialBounds Bounds,
    string FutureFunction);
