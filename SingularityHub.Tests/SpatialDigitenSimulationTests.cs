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
    [Fact]
    public void CityOverviewPopulationHasStableDistinctIdentitiesAndCityScaleGoals()
    {
        var digitens = SpatialDigitenCatalog.CreateCityOverviewPopulation();

        Assert.Equal(4, digitens.Count);
        Assert.Equal(digitens.Count, digitens.Select(x => x.Id).Distinct().Count());
        Assert.All(digitens, digiten =>
        {
            Assert.StartsWith("city-resident-", digiten.Id);
            Assert.InRange(digiten.Position.X, 0, 100);
            Assert.InRange(digiten.Position.Y, 0, 100);
            Assert.False(string.IsNullOrWhiteSpace(digiten.Goal.Description));
        });

        Assert.DoesNotContain(digitens, x => x.Id == "digiten.mayor");
        Assert.DoesNotContain(digitens, x => x.Name == "Mayor Mara");
    }

}
