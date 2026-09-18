namespace TheSingularityWorkshop.Gui;

/// <summary>Declarative manifest for the independent shipyard near Singularity Station.</summary>
/// <remarks>
/// The shipyard is intentionally not a station module. Transit between station and shipyard
/// is a shuttle journey, giving transportation, construction time, resource consumption,
/// and ownership a natural place in the simulation.
/// </remarks>
public sealed record SpatialShipyardManifest(
    string Id,
    string Name,
    string RelationshipToStation,
    IReadOnlyList<SpatialShipyardComponentFamily> ComponentFamilies,
    IReadOnlyList<SpatialShipyardBuildRule> BuildRules,
    IReadOnlyList<SpatialShipyardService> Services)
{
    public static SpatialShipyardManifest CreateDefault()
        => new(
            "singularity-shipyard",
            "SINGULARITY SHIPYARD",
            "INDEPENDENT FACILITY // SHUTTLE REQUIRED",
            [
                new("real", "REAL-WORLD COMPONENTS", ["chemical propulsion", "electric propulsion", "solar power", "habitat", "radiators"]),
                new("science-fiction", "SCI-FI COMPONENTS", ["warp engine", "hyperspace engine", "jump drive", "artificial gravity", "fictional shield"])
            ],
            [
                new("resource-gated", "RESOURCE GATED", "Construction consumes virtual resources."),
                new("time-gated", "TIME GATED", "Construction advances over simulated time rather than completing instantly."),
                new("capability-driven", "CAPABILITY DRIVEN", "Installed components determine available interaction modes."),
                new("genre-compatible", "GENRE COMPATIBLE", "Fictional components remain composable without changing the underlying environment.")
            ],
            [
                new("design", "SHIP DESIGN", "Compose and save a vessel specification."),
                new("construction", "SHIP CONSTRUCTION", "Queue and advance a construction project."),
                new("resource", "RESOURCE YARD", "Receive simulated mined and manufactured resources."),
                new("rental", "SHIP RENTAL", "Offer completed vessels for virtual-currency rental.")
            ]);
}

/// <summary>A family of ship components available to a designer.</summary>
public readonly record struct SpatialShipyardComponentFamily(string Id, string Name, IReadOnlyList<string> Components);

/// <summary>A rule governing how ship construction behaves.</summary>
public readonly record struct SpatialShipyardBuildRule(string Id, string Name, string Description);

/// <summary>A service provided by the shipyard.</summary>
public readonly record struct SpatialShipyardService(string Id, string Name, string Description);
