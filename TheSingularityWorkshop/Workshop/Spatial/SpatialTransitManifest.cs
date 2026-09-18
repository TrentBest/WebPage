namespace TheSingularityWorkshop.Gui;

/// <summary>Data-only description of the Workshop tram service and its first station.</summary>
/// <remarks>
/// The manifest deliberately contains no rendering or navigation behavior. The perception layer
/// consumes this data to draw rails, platforms, passengers, and the tram sequence.
/// </remarks>
public sealed record SpatialTransitManifest(
    string Id,
    string Name,
    IReadOnlyList<SpatialPoint> TrackCenterline,
    SpatialBounds PlatformBounds,
    SpatialPoint DockPoint,
    IReadOnlyList<SpatialTransitPassenger> Passengers,
    IReadOnlyList<SpatialTransitDestination> Destinations,
    SpatialTransitTiming Timing)
{
    public static SpatialTransitManifest CreateDefault()
        => new(
            "workshop-tram",
            "WORKSHOP TRAM",
            [
                new SpatialPoint(58, 51),
                new SpatialPoint(68, 51),
                new SpatialPoint(74, 51),
                new SpatialPoint(82, 51),
                new SpatialPoint(90, 51),
                new SpatialPoint(97, 51)
            ],
            new SpatialBounds(70, 48.5, 18, 4),
            new SpatialPoint(78, 51),
            [
                new("passenger-01", new SpatialPoint(72.5, 50), 0),
                new("passenger-02", new SpatialPoint(74.5, 50), 1),
                new("passenger-03", new SpatialPoint(76.5, 50), 2),
                new("passenger-04", new SpatialPoint(79.5, 50), 3),
                new("passenger-05", new SpatialPoint(81.5, 50), 4),
                new("passenger-06", new SpatialPoint(83.5, 50), 5)
            ],
            [
                new("air-terminal", "AIR TERMINAL", "air-terminal"),
                new("water-terminal", "WATER TERMINAL", "water-terminal"),
                new("rail-terminal", "RAIL TERMINAL", "rail-terminal"),
                new("singularity-lab", "SINGULARITY LAB", "singularity-lab"),
                new("space-elevator", "SPACE ELEVATOR", "space-elevator"),
                new("singularity-island", "SINGULARITY ISLAND", "singularity-island"),
                new("singularity-station", "SINGULARITY STATION", "singularity-station")
            ],
            new SpatialTransitTiming(4, 2, 4, 2));
}

/// <summary>One deterministic passenger waiting for the Workshop tram.</summary>
public readonly record struct SpatialTransitPassenger(string Id, SpatialPoint WaitingPosition, int BoardingOrder);

/// <summary>Animation timing for the tram arrival, boarding, and departure cycle.</summary>
public readonly record struct SpatialTransitTiming(double ArrivalSeconds, double DoorSeconds, double BoardingSeconds, double DepartureSeconds)
{
    public double CycleSeconds => ArrivalSeconds + DoorSeconds + BoardingSeconds + DepartureSeconds;
}
