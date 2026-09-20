using TheSingularityWorkshop.Workshop.Spatial;

namespace SingularityHub.Tests;

public sealed class SpatialSoftwareOfficeArrivalTests
{
    [Fact]
    public void ArrivalSequence_IsExplicitAndDeterministic()
    {
        var arrival = new SpatialSoftwareOfficeArrivalModel();

        Assert.Equal(SpatialSoftwareOfficeArrivalStage.Rampway, arrival.Stage);
        arrival.BeginHelipadArrival();
        Assert.Equal(SpatialSoftwareOfficeArrivalStage.HelicopterApproach, arrival.Stage);
        arrival.EnterHelicopter();
        Assert.Equal(SpatialSoftwareOfficeArrivalStage.HelicopterDeparting, arrival.Stage);
        arrival.ArriveAtTowerHelipad();
        arrival.EnterElevator();
        arrival.Ding();
        Assert.Equal(1, arrival.ElevatorStops);
        arrival.CompleteBriefing();
        Assert.Equal(SpatialSoftwareOfficeArrivalStage.FacilityAccess, arrival.Stage);
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
