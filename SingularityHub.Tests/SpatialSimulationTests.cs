using System.Linq;
using TheSingularityWorkshop.Gui;
using TheSingularityWorkshop.Workshop.Agents;
using TheSingularityWorkshop.Workshop.City;
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

    [Fact(DisplayName = "Incremental Unit Test 10 — a Digiten can discover vehicle capabilities without knowing the machine type")]
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

    [Fact(DisplayName = "Incremental Unit Test 11 — a Digiten is a persistent actor whose intent is interpreted by FSM_API")]
    public void Digiten_BehaviorIsAnFSM_NotTheIdentityItself()
    {
        using var digiten = new DigitenMicroBundle(2301, "Aster", initialBehaviorIndex: 17);

        Assert.Equal(17, digiten.BehaviorIndex);
        Assert.Equal("Existing", digiten.CurrentBehavior);

        digiten.SetGoal(new DigitenGoal("visit-workshop", "Visit the Workshop", BehaviorIndex: 23));
        Assert.Equal("visit-workshop", digiten.Goal!.Id);
        Assert.Equal(23, digiten.BehaviorIndex);

        digiten.Update();
        digiten.Update();

        Assert.Equal("PursuingGoal", digiten.CurrentBehavior);
        Assert.Equal(23, digiten.BehaviorIndex);
    }

    [Fact(DisplayName = "Incremental Unit Test 12 — Digitens can move to and fro using compact waypoint storage")]
    public void DigitenWaypointTexture_ProvidesDeterministicToAndFroMovement()
    {
        var texture = new DigitenWaypointTexture(new[]
        {
            (0.10, 0.20),
            (0.30, 0.40),
            (0.50, 0.60)
        });

        Assert.Equal(3, texture.Count);
        var first = texture.Get(0);
        var last = texture.Get(2);
        Assert.InRange(first.X, 0.0999, 0.1001);
        Assert.InRange(first.Y, 0.1999, 0.2001);
        Assert.InRange(last.X, 0.4999, 0.5001);
        Assert.InRange(last.Y, 0.5999, 0.6001);

        var cursor = new DigitenWaypointCursor(texture.Count);
        Assert.Equal(0, cursor.Index);
        Assert.Equal(1, cursor.Advance());
        Assert.Equal(2, cursor.Advance());
        Assert.Equal(1, cursor.Advance());
        Assert.Equal(0, cursor.Advance());
        Assert.Equal(1, cursor.Advance());
    }

    [Fact(DisplayName = "Incremental Unit Test 13 — the city texture is the occupancy substrate for a crowd vector field")]
    public void DigitenCityTexture_ProvidesOccupancyAndLocalFlow()
    {
        var city = new DigitenCityTexture(7, 5);
        city.SetBlocked(3, 2);
        city.SetBlocked(3, 1);
        city.SetBlocked(3, 3);

        Assert.True(city.TryOccupy(1, 2, 2401));
        Assert.False(city.TryOccupy(1, 2, 2402));
        Assert.Equal(2401, city.GetOccupant(1, 2));
        Assert.False(city.IsWalkable(3, 2));

        var crowd = new DigitenCrowdField(city);
        var direction = crowd.ChooseDirection(2, 2, new DigitenVector(1, 0), frustration: 0);
        Assert.Equal(new DigitenVector(0, -1), direction);

        Assert.True(city.TryMove(2401, 1, 2, 1, 1));
        Assert.Equal(2401, city.GetOccupant(1, 1));
        Assert.Equal(DigitenCityTexture.Empty, city.GetOccupant(1, 2));
    }

    [Fact(DisplayName = "Incremental Unit Test 14 — accumulated directional frustration can trigger a reversal")]
    public void DigitenCrowdField_ReversalHeuristicChangesPreferredFlow()
    {
        var city = new DigitenCityTexture(5, 3);
        city.SetBlocked(2, 0);
        city.SetBlocked(2, 1);
        city.SetBlocked(2, 2);

        var crowd = new DigitenCrowdField(city);
        var normal = crowd.ChooseDirection(1, 1, new DigitenVector(1, 0), frustration: 0);
        var frustrated = crowd.ChooseDirection(1, 1, new DigitenVector(1, 0), frustration: 1.0);

        Assert.NotEqual(normal, frustrated);
        Assert.Equal(new DigitenVector(-1, 0), frustrated);
    }

    [Fact(DisplayName = "Incremental Unit Test 15 — waypoint destinations become imperfect walkable targets")]
    public void DigitenWaypointSampler_ShrinksRadiusAroundObstacles()
    {
        var city = new DigitenCityTexture(7, 7);
        for (var y = 1; y <= 5; y++)
            for (var x = 1; x <= 5; x++)
                city.SetBlocked(x, y);

        city.SetBlocked(3, 3, blocked: false);

        var found = DigitenWaypointSampler.TrySample(city, 3, 3, radius: 2, new System.Random(42), out var point);

        Assert.True(found);
        Assert.Equal((3, 3), point);
    }

    [Fact(DisplayName = "Incremental Unit Test 16 — travel frustration grows off-line and clears on aligned travel")]
    public void DigitenTravelFrustration_AccumulatesAndRecovers()
    {
        var frustration = new DigitenTravelFrustration(reversalThreshold: 1.0);
        var desired = new DigitenVector(1, 0);

        frustration.Observe(desired, new DigitenVector(0, 1), amount: 0.5);
        Assert.True(frustration.Value > 0);
        Assert.False(frustration.HasHadEnough);

        frustration.Observe(desired, new DigitenVector(-1, 0), amount: 0.5);
        Assert.True(frustration.HasHadEnough);

        frustration.Observe(desired, desired, amount: 0.5);
        Assert.True(frustration.Value < 1.0);
    }

    [Fact(DisplayName = "Incremental Unit Test 17 — a citizen can live at home, spend Singularity currency, and feed attendance economics")]
    public void SingularityCityEconomy_PersistsHomeCitizenAndBusinessActivity()
    {
        var economy = new SingularityCityEconomy();
        var home = new SingularityHome("home-aster", 10, 12, Capacity: 2);
        var citizen = new SingularityCitizen(3001, "Aster", home, startingBalance: 25);
        var business = new SingularityBusiness("night-market", "Night Market", 18, 14, attendancePrice: 7);

        economy.Register(citizen);
        economy.Register(business);

        Assert.True(citizen.Visit(business));
        Assert.Equal(18, citizen.Wallet.Balance);
        Assert.Equal(1, business.Attendance);
        Assert.Equal(7, business.Revenue);
        Assert.Equal("Attending:night-market", citizen.CurrentActivity);

        citizen.ReturnHome();
        Assert.Equal("Home", citizen.CurrentActivity);
        Assert.Equal(25, economy.TotalCurrency);
    }

    [Fact(DisplayName = "Incremental Unit Test 18 — DigiGroups turn arrival into intentional cooperative or competitive activity")]
    public void DigiGroup_SeparatesIntentionalAggregationFromCrowdMovement()
    {
        var group = new DigiGroup("forge-shift-alpha", "Build Workshop foundation", DigiGroupMode.Cooperative);

        Assert.True(group.Add(3001));
        Assert.True(group.Add(3002));
        Assert.False(group.Add(3001));
        Assert.Equal(2, group.Members.Count);
        Assert.True(group.Contains(3002));
        Assert.Equal(DigiGroupMode.Cooperative, group.Mode);

        Assert.True(group.Remove(3001));
        Assert.False(group.Contains(3001));
    }
}
