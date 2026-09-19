namespace TheSingularityWorkshop.Gui;

using System.Collections.Generic;
using System.Linq;

/// <summary>Robotic and drone specimens used as physical laboratory instruments.</summary>
public sealed record SpatialLaboratoryRoboticsCatalog(IReadOnlyList<SpatialRobotSpec> Robots)
{
    public static SpatialLaboratoryRoboticsCatalog CreateDefault()
        => new(
        [
            new("lab-robot", "LAB ROBOT", "Mobile manipulation and instrument handling.", ["navigation", "manipulation", "sensors"]),
            new("survey-drone", "SURVEY DRONE", "Aerial mapping and spatial measurement.", ["flight", "camera", "lidar"]),
            new("orbital-drone", "ORBITAL DRONE", "Microgravity inspection and maintenance.", ["orbital", "thrusters", "inspection"]),
            new("construction-drone", "CONSTRUCTION DRONE", "Remote assembly of structures and apparatus.", ["assembly", "tools", "navigation"]),
            new("swarm-node", "SWARM NODE", "A small autonomous unit for distributed experiments.", ["coordination", "sensing", "communication"])
        ]);

    public SpatialRobotSpec? Find(string id)
        => Robots.FirstOrDefault(x => x.Id == id);
}

public readonly record struct SpatialRobotSpec(
    string Id,
    string Name,
    string Description,
    IReadOnlyList<string> CapabilityIds);
