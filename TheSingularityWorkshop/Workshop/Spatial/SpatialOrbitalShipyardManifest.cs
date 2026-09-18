namespace TheSingularityWorkshop.Gui;

/// <summary>Declarative orbital relationship between Singularity Station and its independent shipyard.</summary>
public sealed record SpatialOrbitalShipyardManifest(
    string Id,
    string Name,
    string StationId,
    SpatialPoint StationPosition,
    SpatialPoint ShipyardPosition,
    IReadOnlyList<SpatialShuttleRoute> ShuttleRoutes)
{
    /// <summary>Creates the first station/shipyard composition.</summary>
    public static SpatialOrbitalShipyardManifest CreateDefault()
        => new(
            "singularity-orbital-yard",
            "SINGULARITY ORBITAL SHIPYARD",
            "singularity-station",
            new SpatialPoint(72, 18),
            new SpatialPoint(88, 18),
            [
                new("station-to-yard", "STATION → SHIPYARD", "singularity-station", "singularity-shipyard", "SHUTTLE")
            ]);
}

/// <summary>A shuttle-only route between two independent spatial facilities.</summary>
public readonly record struct SpatialShuttleRoute(
    string Id,
    string Name,
    string OriginId,
    string DestinationId,
    string VehicleType);
