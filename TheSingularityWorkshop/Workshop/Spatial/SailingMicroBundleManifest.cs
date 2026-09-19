namespace TheSingularityWorkshop.Gui;

using System.Collections.Generic;

/// <summary>
/// Micro-bundle manifest for the sailing Experience. It starts with a personal sailboat
/// and provides a progression path toward crews, tall ships, naval games, and modern vessels.
/// </summary>
public sealed record SailingMicroBundleManifest(
    string Id,
    string Name,
    IReadOnlyList<SailingVesselDefinition> Vessels,
    IReadOnlyList<SailingGameplaySystem> GameplaySystems,
    IReadOnlyList<SailingProgressionStage> Progression)
{
    public static SailingMicroBundleManifest CreateDefault()
        => new(
            "microbundle.sailing",
            "SAILING",
            [
                new("sailboat.personal", "Personal Sailboat", SailingVesselKind.Sailboat, 1, 1, true),
                new("sailboat.cruiser", "Cruising Sailboat", SailingVesselKind.Sailboat, 1, 6, true),
                new("ship.galley", "Galley", SailingVesselKind.Galley, 20, 80, true),
                new("ship.man-of-war", "Man of War", SailingVesselKind.ManOfWar, 150, 800, false),
                new("ship.modern-naval", "Modern Naval Vessel", SailingVesselKind.ModernNaval, 80, 400, false)
            ],
            [
                new("helm", "Helm", "Steer, tack, gybe, and maintain a desired course."),
                new("wind", "Wind", "Model apparent wind, sail trim, and changing conditions."),
                new("navigation", "Navigation", "Read charts, bearings, destinations, and hazards."),
                new("weather", "Weather", "Turn forecast, visibility, sea state, and wind shifts into gameplay."),
                new("crew", "Crew", "Assign player roles to stations and coordinate vessel operations."),
                new("damage-control", "Damage Control", "Create repair, flooding, fire, and systems-management gameplay."),
                new("boarding", "Boarding", "Provide crew-versus-crew encounters without requiring a particular historical setting."),
                new("naval-combat", "Naval Combat", "Support age-of-sail and modern vessel combat as separate rulesets.")
            ],
            [
                new("personal", "Personal Sailing", "Learn the boat, read wind, navigate, and complete coastal voyages."),
                new("voyage", "Voyaging", "Plan passages, manage provisions, weather, watch schedules, and navigation."),
                new("crew", "Crewed Vessel", "Invite players aboard and assign meaningful stations."),
                new("age-of-sail", "Age of Sail", "Operate larger sailing ships with coordinated crews and ship-to-ship gameplay."),
                new("fleet", "Fleet Operations", "Compose multiple vessels into cooperative or competitive scenarios."),
                new("modern", "Modern Maritime", "Move the same spatial/gameplay framework into contemporary vessels.")
            ]);
}

public readonly record struct SailingVesselDefinition(
    string Id,
    string Name,
    SailingVesselKind Kind,
    int MinimumCrew,
    int MaximumCrew,
    bool PlayerCanPilot);

public enum SailingVesselKind
{
    Sailboat,
    Galley,
    ManOfWar,
    ModernNaval
}

public readonly record struct SailingGameplaySystem(string Id, string Name, string Description);

public readonly record struct SailingProgressionStage(string Id, string Name, string Description);
