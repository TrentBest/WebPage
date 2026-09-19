using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialDigitenSimulationTests
{
    [Fact]
    public void DigitenCanPlanAndAdvanceTowardAGoal()
    {
        var digiten = new SpatialDigiten(
            "test",
            "Test Citizen",
            new SpatialPoint(.5, .5),
            new SpatialDigitenGoal("destination", "Reach the destination", new SpatialPoint(4.5, 4.5)));

        Assert.True(digiten.PlanRoute(5, 5, static (_, _) => true));
        var start = digiten.Position;

        Assert.True(digiten.Step());
        Assert.NotEqual(start, digiten.Position);
    }

    [Fact]
    public void GoalChangeInvalidatesThePreviousRoute()
    {
        var digiten = new SpatialDigiten(
            "test",
            "Test Citizen",
            new SpatialPoint(.5, .5),
            new SpatialDigitenGoal("a", "First destination", new SpatialPoint(4.5, .5)));

        Assert.True(digiten.PlanRoute(5, 5, static (_, _) => true));
        Assert.True(digiten.HasRoute);

        digiten.SetGoal(new SpatialDigitenGoal("b", "Second destination", new SpatialPoint(.5, 4.5)));

        Assert.False(digiten.HasRoute);
    }
}
