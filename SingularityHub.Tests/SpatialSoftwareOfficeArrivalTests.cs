using TheSingularityWorkshop.Workshop.Spatial;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialSoftwareOfficeArrivalTests
{
    [Fact]
    public void ArrivalSequence_IsRampToHelipadThenTakeoffRevealLandingAndFloorPlan()
    {
        var arrival = new SpatialSoftwareOfficeArrivalModel();

        Assert.Equal(SpatialSoftwareOfficeArrivalStage.Rampway, arrival.Stage);

        arrival.BeginHelipadArrival();
        Assert.Equal(SpatialSoftwareOfficeArrivalStage.HelipadReady, arrival.Stage);

        arrival.EnterHelicopter();
        Assert.Equal(SpatialSoftwareOfficeArrivalStage.HelicopterTakeoff, arrival.Stage);

        while (arrival.Stage == SpatialSoftwareOfficeArrivalStage.HelicopterTakeoff)
            arrival.AdvanceCinematic(.2);

        Assert.Equal(SpatialSoftwareOfficeArrivalStage.TowerReveal, arrival.Stage);

        while (arrival.Stage == SpatialSoftwareOfficeArrivalStage.TowerReveal)
            arrival.AdvanceCinematic(.2);

        Assert.Equal(SpatialSoftwareOfficeArrivalStage.HelicopterLanding, arrival.Stage);

        while (arrival.Stage == SpatialSoftwareOfficeArrivalStage.HelicopterLanding)
            arrival.AdvanceCinematic(.2);

        Assert.Equal(SpatialSoftwareOfficeArrivalStage.TowerFloorPlan, arrival.Stage);

        arrival.EnterElevator();
        arrival.Ding();
        Assert.Equal(1, arrival.ElevatorStops);

        arrival.CompleteBriefing();
        Assert.Equal(SpatialSoftwareOfficeArrivalStage.FacilityAccess, arrival.Stage);
    }

    [Fact]
    public void HelicopterPhysics_IsDeterministicAndFullyLutDriven()
    {
        var physics = new SpatialHelicopterPhysicsMicroBundle();

        var takeoffStart = physics.Takeoff(0);
        var takeoffMid = physics.Takeoff(.5);
        var takeoffEnd = physics.Takeoff(1);
        var landingMid = physics.Landing(.5);
        var sway = physics.TowerSway(.5);

        Assert.Equal(0d, takeoffStart.Progress);
        Assert.Equal(1.60d, takeoffEnd.Y, 3);
        Assert.True(takeoffMid.Y > takeoffStart.Y);
        Assert.True(takeoffMid.Y < takeoffEnd.Y);
        Assert.True(landingMid.Y > 0d);
        Assert.NotEqual(0d, sway.Roll);
        Assert.NotEqual(0d, sway.Pitch);
        Assert.Equal(2500d, physics.Constants.MassKg);
        Assert.Equal(13.4d, physics.Constants.RotorDiameterMeters);
    }

    [Fact]
    public void BehaviorBuilder_DefaultsToTypedContextAndExtendsWithElseIf()
    {
        var model = new SpatialBehaviorBuilderModel("OnUpdate");

        Assert.Equal("if(context is UserSelectedType ust)", model.ContextBranches[0].Signature);
        var second = model.AddElseIf("AnotherType", "anotherType");

        Assert.Equal("else if", second.Keyword);
        Assert.Equal("else if(context is AnotherType anotherType)", second.Signature);
    }
}
