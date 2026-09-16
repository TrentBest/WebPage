using System.Linq;
using TheSingularityWorkshop.Gui;
using TheSingularityWorkshop.Workshop.MicroBundles;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>
/// Spatial simulation breadcrumbs. These tests deliberately describe the machine
/// underneath the visual manifestation rather than asserting HTML/CSS details.
/// </summary>
public sealed class SpatialSimulationTests
{
    [Fact(DisplayName = "Incremental Unit Test 07 — Diablo pathfinding routes around blocked geometry")]
    public void Pathfinder_RoutesAroundObstacle()
    {
        var path = DiabloPathfinder.FindPath(
            9,
            9,
            (x, y) => !(x is >= 3 and <= 5 && y is >= 2 and <= 6),
            new SpatialGridPoint(1, 4),
            new SpatialGridPoint(7, 4));

        Assert.NotEmpty(path);
        Assert.Equal(new SpatialGridPoint(1, 4), path.First());
        Assert.Equal(new SpatialGridPoint(7, 4), path.Last());
        Assert.All(path, point => Assert.False(point.X is >= 3 and <= 5 && point.Y is >= 2 and <= 6));
    }

    [Fact(DisplayName = "Incremental Unit Test 08 — construction vehicles expose locomotion and work state independently")]
    public void ConstructionVehicle_TracksPrimaryAndSecondaryState()
    {
        using var dumpTruck = new ConstructionVehicleMicroBundle(2201, "Hauler One", ConstructionMachineKind.DumpTruck);
        dumpTruck.SetDriveStatus(VehicleDriveStatus.Driving);
        dumpTruck.SetOperatingStatus(VehicleOperatingStatus.Full);
        dumpTruck.SetSubsystemStatus("Bed", "Loaded");

        Assert.Equal(VehicleDriveStatus.Driving, dumpTruck.DriveStatus);
        Assert.Equal(VehicleOperatingStatus.Full, dumpTruck.OperatingStatus);
        Assert.Equal("Loaded", dumpTruck.SubsystemStatus["Bed"]);

        using var backhoe = new ConstructionVehicleMicroBundle(2202, "Excavator One", ConstructionMachineKind.Backhoe);
        backhoe.SetDriveStatus(VehicleDriveStatus.Parked);
        backhoe.SetOperatingStatus(VehicleOperatingStatus.Operating);
        backhoe.SetSubsystemStatus("Grounding", "Deployed");
        backhoe.SetSubsystemStatus("Boom", "Raised");
        backhoe.SetSubsystemStatus("Bucket", "Digging");

        Assert.Equal(VehicleOperatingStatus.Operating, backhoe.OperatingStatus);
        Assert.Equal("Deployed", backhoe.SubsystemStatus["Grounding"]);
        Assert.Equal("Raised", backhoe.SubsystemStatus["Boom"]);
        Assert.Equal("Digging", backhoe.SubsystemStatus["Bucket"]);
    }
}
