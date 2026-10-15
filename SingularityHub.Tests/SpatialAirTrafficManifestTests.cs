using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialAirTrafficManifestTests
{
    [Fact]
    public void DefaultManifestDeclaresStableUniqueVehiclesAndSeparateAltitudeBands()
    {
        var manifest = SpatialAirTrafficManifest.CreateDefault();

        Assert.Equal("singularity-air-traffic", manifest.Id);
        Assert.Equal(4, manifest.Vehicles.Count);
        Assert.Equal(
            manifest.Vehicles.Count,
            manifest.Vehicles.Select(vehicle => vehicle.Id).Distinct(StringComparer.OrdinalIgnoreCase).Count());
        Assert.Contains(manifest.Vehicles, vehicle => vehicle.AltitudeBand == 1);
        Assert.Contains(manifest.Vehicles, vehicle => vehicle.AltitudeBand == 2);
        Assert.Contains(manifest.Vehicles, vehicle => vehicle.AltitudeBand == 3);
        Assert.Equal("air-vehicle-001", manifest.FindVehicle("AIR-VEHICLE-001")?.Id);
    }

    [Fact]
    public void RouteSnapshotsAreRepeatableAndAdvanceWithoutMutatingTheManifest()
    {
        var manifest = SpatialAirTrafficManifest.CreateDefault();
        var originalVehicles = manifest.Vehicles.ToArray();

        var first = manifest.SnapshotAt(12);
        var repeat = manifest.SnapshotAt(12);
        var later = manifest.SnapshotAt(18);

        Assert.Equal(first, repeat);
        Assert.Equal(originalVehicles, manifest.Vehicles);
        Assert.Equal(first.Count, later.Count);

        var firstVehicle = Assert.Single(first, snapshot => snapshot.VehicleId == "air-vehicle-001");
        var laterVehicle = Assert.Single(later, snapshot => snapshot.VehicleId == "air-vehicle-001");

        Assert.NotEqual(firstVehicle.Position.X, laterVehicle.Position.X);
        Assert.InRange(firstVehicle.RouteProgress, 0, 1);
        Assert.InRange(laterVehicle.RouteProgress, 0, 1);
        Assert.Equal(1, firstVehicle.AltitudeBand);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void SnapshotRejectsInvalidElapsedTime(double elapsedSeconds)
    {
        var manifest = SpatialAirTrafficManifest.CreateDefault();

        Assert.Throws<ArgumentOutOfRangeException>(() => manifest.SnapshotAt(elapsedSeconds));
    }

    [Fact]
    public void RouteRejectsMissingIdentityAndInvalidFlightParameters()
    {
        var route = new SpatialAirTrafficVehicle(
            "test-vehicle",
            "hover-cab",
            "lane-a",
            new SpatialPoint(10, 10),
            new SpatialPoint(90, 20),
            1,
            30,
            0);

        Assert.Throws<InvalidOperationException>(() => (route with { AltitudeBand = 0 }).SnapshotAt(0));
        Assert.Throws<InvalidOperationException>(() => (route with { TravelSeconds = 0 }).SnapshotAt(0));
        Assert.Throws<InvalidOperationException>(() => (route with { LaneId = "" }).SnapshotAt(0));
    }
}
