using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialTransitJourneyTests
{
    private static SpatialTransitJourney CreateJourney() =>
        new("journey-001", "passenger-01", "platform-01", "rail-terminal",
            RequestedAtSeconds: 10, PickupSeconds: 2, BoardingSeconds: 3,
            TravelSeconds: 20, ArrivalDwellSeconds: 4);

    [Theory]
    [InlineData(10, SpatialTransitJourneyStatus.Waiting)]
    [InlineData(12, SpatialTransitJourneyStatus.Boarding)]
    [InlineData(15, SpatialTransitJourneyStatus.InTransit)]
    [InlineData(35, SpatialTransitJourneyStatus.Arrived)]
    [InlineData(39, SpatialTransitJourneyStatus.Completed)]
    public void JourneyStagesAreDerivedFromExplicitSimulationTime(
        double time,
        SpatialTransitJourneyStatus expected)
    {
        var snapshot = CreateJourney().SnapshotAt(time);

        Assert.Equal(expected, snapshot.Status);
        Assert.Equal("journey-001", snapshot.JourneyId);
        Assert.Equal("passenger-01", snapshot.PassengerId);
        Assert.Equal("rail-terminal", snapshot.DestinationId);
    }

    [Fact]
    public void JourneySnapshotsAreDeterministicAndDoNotMutateTheJourney()
    {
        var journey = CreateJourney();

        Assert.Equal(journey.SnapshotAt(25), journey.SnapshotAt(25));
        Assert.Equal(10, journey.RequestedAtSeconds);
    }

    [Fact]
    public void JourneyRejectsTimeBeforeRequestAndInvalidTravelDuration()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CreateJourney().SnapshotAt(9));
        Assert.Throws<InvalidOperationException>(() => (CreateJourney() with { TravelSeconds = 0 }).SnapshotAt(20));
    }

    [Fact]
    public void JourneyProgressIsClampedAndArrivalIsDistinctFromCompletion()
    {
        var journey = CreateJourney();

        var arrival = journey.SnapshotAt(35);
        var completed = journey.SnapshotAt(100);

        Assert.Equal(SpatialTransitJourneyStatus.Arrived, arrival.Status);
        Assert.Equal(SpatialTransitJourneyStatus.Completed, completed.Status);
        Assert.Equal(1, completed.Progress);
    }
}
