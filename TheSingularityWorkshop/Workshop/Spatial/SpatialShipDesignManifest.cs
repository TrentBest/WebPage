namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Declarative ship-design model proving that propulsion choices alter interaction modes,
/// not the structure of the shared space environment.
/// </summary>
public sealed record SpatialShipDesignManifest(
    IReadOnlyList<SpatialShipDriveOption> Drives,
    IReadOnlyList<SpatialShipInteractionMode> InteractionModes)
{
    public static SpatialShipDesignManifest CreateDefault()
        => new(
            [
                new("chemical", "CHEMICAL ROCKET", "REAL", ["thrust", "orbital-flight"]),
                new("electric", "ELECTRIC PROPULSION", "REAL", ["efficient-thrust", "orbital-flight"]),
                new("warp", "WARP ENGINE", "SCI-FI", ["warp-flight"]),
                new("hyperspace", "HYPERSPACE ENGINE", "SCI-FI", ["hyperspace-flight"]),
                new("jump", "JUMP DRIVE", "SCI-FI", ["jump-flight"])
            ],
            [
                new("orbital-flight", "ORBITAL FLIGHT", "Navigate through ordinary space."),
                new("warp-flight", "WARP", "Expose a warp-style travel interaction."),
                new("hyperspace-flight", "HYPERSPACE", "Expose a hyperspace-style travel interaction."),
                new("jump-flight", "JUMP", "Expose a discrete jump interaction.")
            ]);

    /// <summary>Returns the interaction modes enabled by the selected drive.</summary>
    public IReadOnlyList<SpatialShipInteractionMode> ModesForDrive(string driveId)
    {
        var drive = Drives.FirstOrDefault(x => string.Equals(x.Id, driveId, StringComparison.OrdinalIgnoreCase));
        if (drive == default) return [];
        return [.. InteractionModes.Where(mode => drive.ModeIds.Contains(mode.Id, StringComparer.OrdinalIgnoreCase))];
    }
}

/// <summary>A selectable propulsion component in a vessel design.</summary>
public readonly record struct SpatialShipDriveOption(
    string Id,
    string Name,
    string TechnologyDomain,
    IReadOnlyList<string> ModeIds);

/// <summary>An interaction contract exposed by a ship's selected components.</summary>
public readonly record struct SpatialShipInteractionMode(
    string Id,
    string Name,
    string Description);
