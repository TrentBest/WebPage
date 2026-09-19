namespace TheSingularityWorkshop.Gui;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Selectable solar-system models for story construction and physics exploration.
/// Real astronomical systems and fictional story systems are deliberately distinguished.
/// </summary>
public sealed record SpatialSolarSystemScenarioCatalog(IReadOnlyList<SpatialSolarSystemScenario> Scenarios)
{
    public static SpatialSolarSystemScenarioCatalog CreateDefault()
        => new(
        [
            new(
                "solar-system",
                "SOLAR SYSTEM",
                SpatialSolarSystemModelKind.Astronomical,
                "Our familiar planetary arrangement for learning orbital and gravity relationships.",
                ["sol", "mercury", "venus", "earth", "moon", "mars", "jupiter", "saturn", "uranus", "neptune"]),
            new(
                "avatar-story-system",
                "PANDORA STORY SYSTEM",
                SpatialSolarSystemModelKind.FictionalStory,
                "A fictional, Avatar-inspired story configuration. It is a narrative model, not an astronomical claim.",
                ["story-star", "story-planet", "story-moon"]),
            new(
                "asteroids-2d",
                "ASTEROIDS HOMAGE 2D",
                SpatialSolarSystemModelKind.GameHomage,
                "A 2D vector-space homage used to demonstrate inertia, wraparound, projectiles, and collision geometry.",
                ["ship", "asteroid-field", "projectile"]),
            new(
                "asteroids-3d",
                "ASTEROIDS 3D LAB",
                SpatialSolarSystemModelKind.Experimental,
                "The same simple interaction model promoted into a measurable three-dimensional volume.",
                ["ship-3d", "asteroid-field-3d", "projectile-3d"])
        ]);

    public SpatialSolarSystemScenario? Find(string id)
        => Scenarios.FirstOrDefault(x => x.Id == id);
}

public readonly record struct SpatialSolarSystemScenario(
    string Id,
    string Name,
    SpatialSolarSystemModelKind Kind,
    string Description,
    IReadOnlyList<string> BodyOrObjectIds);

public enum SpatialSolarSystemModelKind
{
    Astronomical,
    FictionalStory,
    GameHomage,
    Experimental
}
