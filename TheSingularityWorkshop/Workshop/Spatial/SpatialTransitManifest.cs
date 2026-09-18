namespace TheSingularityWorkshop.Gui;

public sealed record SpatialTransitManifest(
    string Id,
    string Name,
    IReadOnlyList<SpatialPoint> TrackCenterline,
    SpatialBounds PlatformBounds,
    SpatialPoint DockPoint,
    IReadOnlyList<SpatialTransitPassenger> Passengers,
    IReadOnlyList<SpatialTransitDestination> Destinations,
    SpatialTransitTiming Timing,
    IReadOnlyList<SpatialTransitPlatform> Platforms,
    IReadOnlyList<SpatialTransitLevel> Levels)
{
    public static SpatialTransitManifest CreateDefault()
    {
        var destinations = new[]
        {
            new SpatialTransitDestination("air-terminal", "AIR TERMINAL", "air-terminal"),
            new SpatialTransitDestination("water-terminal", "WATER TERMINAL", "water-terminal"),
            new SpatialTransitDestination("rail-terminal", "RAIL TERMINAL", "rail-terminal"),
            new SpatialTransitDestination("singularity-lab", "SINGULARITY LAB", "singularity-lab"),
            new SpatialTransitDestination("space-elevator", "SPACE ELEVATOR", "space-elevator"),
            new SpatialTransitDestination("singularity-island", "SINGULARITY ISLAND", "singularity-island"),
            new SpatialTransitDestination("singularity-station", "SINGULARITY STATION", "singularity-station")
        };

        return new(
            "workshop-grand-central",
            "SINGULARITY GRAND CENTRAL",
            [
                new SpatialPoint(5, 52),
                new SpatialPoint(20, 52),
                new SpatialPoint(35, 52),
                new SpatialPoint(50, 52),
                new SpatialPoint(65, 52),
                new SpatialPoint(80, 52),
                new SpatialPoint(95, 52)
            ],
            new SpatialBounds(7, 48, 86, 8),
            new SpatialPoint(50, 52),
            [
                new("passenger-01", new SpatialPoint(19, 39), 0),
                new("passenger-02", new SpatialPoint(29, 39), 1),
                new("passenger-03", new SpatialPoint(39, 39), 2),
                new("passenger-04", new SpatialPoint(61, 39), 3),
                new("passenger-05", new SpatialPoint(71, 39), 4),
                new("passenger-06", new SpatialPoint(81, 39), 5),
                new("passenger-07", new SpatialPoint(24, 64), 6),
                new("passenger-08", new SpatialPoint(34, 64), 7),
                new("passenger-09", new SpatialPoint(66, 64), 8),
                new("passenger-10", new SpatialPoint(76, 64), 9)
            ],
            destinations,
            new SpatialTransitTiming(3, 2, 4, 3),
            [
                new("platform-01", 1, 34, 20, 16, 1, "air-terminal"),
                new("platform-02", 1, 34, 36, 16, 1, "water-terminal"),
                new("platform-03", 1, 34, 52, 16, 1, "rail-terminal"),
                new("platform-04", 1, 34, 68, 16, 1, "singularity-lab"),
                new("platform-05", 2, 50, 20, 16, 2, "space-elevator"),
                new("platform-06", 2, 50, 36, 16, 2, "singularity-island"),
                new("platform-07", 2, 50, 52, 16, 2, "singularity-station")
            ],
            [
                new(1, "CONCOURSE", "Main public concourse above the tracks."),
                new(2, "OVERHEAD CONCOURSE", "Expandable second-level circulation deck.")
            ]);
    }
}

public readonly record struct SpatialTransitPassenger(string Id, SpatialPoint WaitingPosition, int BoardingOrder);

public readonly record struct SpatialTransitPlatform(
    string Id,
    int Level,
    double X,
    double Y,
    double Width,
    int TrackIndex,
    string DestinationId);

public readonly record struct SpatialTransitLevel(int Number, string Name, string Purpose);

public readonly record struct SpatialTransitTiming(double ArrivalSeconds, double DoorSeconds, double BoardingSeconds, double DepartureSeconds)
{
    public double CycleSeconds => ArrivalSeconds + DoorSeconds + BoardingSeconds + DepartureSeconds;
}
