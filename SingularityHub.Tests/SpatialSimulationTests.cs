using System.Linq;
using TheSingularityWorkshop.Gui;
using TheSingularityWorkshop.Workshop.MicroBundles;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Spatial simulation breadcrumbs describing the machine underneath the visual manifestation.</summary>
public sealed class SpatialSimulationTests
{
    [Fact(DisplayName = "Incremental Unit Test 07 — Diablo pathfinding routes around blocked geometry")]
    public void Pathfinder_RoutesAroundObstacle()
    {
        var path = DiabloPathfinder.FindPath(9, 9, (x, y) => !(x is >= 3 and <= 5 && y is >= 2 and <= 6), new SpatialGridPoint(1, 4), new SpatialGridPoint(7, 4));
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

    [Fact(DisplayName = "Incremental Unit Test 09 — construction machines manifest as semantic part assemblies")]
    public void ConstructionVehicle_ManifestationContainsMachineParts()
    {
        var backhoe = ConstructionVehicleGuiBuilder.Parts(ConstructionVehicleKind.Backhoe);
        Assert.Contains(backhoe, part => part.Code == "CAB");
        Assert.Contains(backhoe, part => part.Code == "SCOOP");
        Assert.Contains(backhoe, part => part.Code == "BOOM");
        Assert.Contains(backhoe, part => part.Code == "BUCKET");
        Assert.Equal(4, backhoe.Single(part => part.Code == "TIRE").Count);

        var crane = ConstructionVehicleGuiBuilder.Parts(ConstructionVehicleKind.Crane);
        Assert.Equal(4, crane.Single(part => part.Code == "OUTRIGGER").Count);
        Assert.Equal(4, crane.Single(part => part.Code == "TIRE").Count);
    }

    [Fact(DisplayName = "Incremental Unit Test 10 — a Digitens can discover vehicle capabilities without knowing the machine type")]
    public void DigitensVehicleAgent_DiscoversCapabilitiesFromVehicle()
    {
        using var backhoe = new ConstructionVehicleMicroBundle(2210, "Excavator One", ConstructionMachineKind.Backhoe);

        Assert.Contains(backhoe.Capabilities, capability => capability.Id == "excavate");
        Assert.Contains(backhoe.Capabilities.SelectMany(capability => capability.Actions), action => action.Id == "dig");
        Assert.True(backhoe.CanPerform("dig"));
        Assert.False(backhoe.CanPerform("fly"));

        using var rover = new ConstructionVehicleMicroBundle(2211, "Survey Rover", ConstructionMachineKind.Rover);
        Assert.True(rover.CanPerform("scan"));
        Assert.True(rover.CanPerform("navigate"));
        Assert.False(rover.CanPerform("dig"));
    }
}
