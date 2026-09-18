namespace TheSingularityWorkshop.Gui;

using System.Collections.Generic;

/// <summary>
/// A playful island Experience reached through the Workshop transit network.
/// It is deliberately declarative: rides, discoveries, and MicroBundle recipes
/// can grow independently of the spatial renderer.
/// </summary>
public sealed record SpatialIslandExperience(
    string Id,
    string Name,
    string Description,
    IReadOnlyList<SpatialIslandAttraction> Attractions)
{
    public static SpatialIslandExperience SingularityIsland { get; } = new(
        "singularity-island",
        "SINGULARITY ISLAND",
        "An experimental amusement park where Workshop capabilities become rides, games, discoveries, and living demonstrations.",
        [
            new("maze", "THE WORKSHOP MAZE", "A navigation challenge whose results feed the leaderboard MicroBundle.", SpatialIslandAttractionKind.Game, "maze"),
            new("line-coaster", "LINEWORK COASTER", "Ride the Workshop line renderer through impossible geometry.", SpatialIslandAttractionKind.Ride, "line-lab"),
            new("camera-wheel", "CAMERA WHEEL", "Explore composition, zoom, framing, and changing semantic resolution.", SpatialIslandAttractionKind.View, "camera-lab"),
            new("recipe-garden", "RECIPE GARDEN", "Inspect the MicroBundles and arbitrations that make an Experience possible.", SpatialIslandAttractionKind.Laboratory, "recipe-lab"),
            new("creator-archive", "CREATOR ARCHIVE", "Trace a captured artifact through its lineage, attribution, and originating creator.", SpatialIslandAttractionKind.Archive, "creator-archive")
        ]);
}

/// <summary>One discoverable island attraction and its child Experience.</summary>
public sealed record SpatialIslandAttraction(
    string Id,
    string Name,
    string Description,
    SpatialIslandAttractionKind Kind,
    string ExperienceId);

public enum SpatialIslandAttractionKind
{
    Ride,
    Game,
    View,
    Laboratory,
    Archive
}
