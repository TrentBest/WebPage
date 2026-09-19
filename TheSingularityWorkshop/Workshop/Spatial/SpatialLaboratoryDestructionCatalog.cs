namespace TheSingularityWorkshop.Gui;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Diegetic destruction sandboxes for the laboratory. These are physics demonstrations,
/// not a castle game: the user composes structures, launchers, projectiles, and fields,
/// then observes the physical result.
/// </summary>
public sealed record SpatialLaboratoryDestructionCatalog(
    IReadOnlyList<SpatialLaboratoryDestructionSandbox> Sandboxes)
{
    public static SpatialLaboratoryDestructionCatalog CreateDefault()
        => new(
        [
            new(
                "castle-impact",
                "CASTLE IMPACT CHAMBER",
                "Blocks become a structure. A controlled projectile becomes an input. The result is measured destruction, not a game score.",
                ["stone-block", "wood-block", "target-wall", "projectile"]),
            new(
                "ramp-ball",
                "RAMP & BALL CHAMBER",
                "Change ramp angle, mass, and surface, then compare acceleration, momentum, and impact.",
                ["adjustable-ramp", "steel-ball", "wood-ball", "impact-plate"]),
            new(
                "catapult",
                "CATAPULT PHYSICS CHAMBER",
                "Expose launch geometry, projectile mass, spring or counterweight energy, and target material as explicit variables.",
                ["catapult-arm", "launch-stone", "target-wall", "chronograph"]),
            new(
                "orbital-launcher",
                "3D ORBITAL LAUNCH RAIL",
                "Move a launcher along a spatial track, aim through a three-dimensional field, and fire a projectile while the laboratory records its trajectory.",
                ["track", "launcher-carriage", "projectile", "trajectory-volume"])
        ]);

    public SpatialLaboratoryDestructionSandbox? Find(string id)
        => Sandboxes.FirstOrDefault(x => string.Equals(x.Id, id, System.StringComparison.OrdinalIgnoreCase));
}

/// <summary>A physics demonstration assembled from semantic apparatus rather than a game level.</summary>
public readonly record struct SpatialLaboratoryDestructionSandbox(
    string Id,
    string Name,
    string Description,
    IReadOnlyList<string> ApparatusIds);
