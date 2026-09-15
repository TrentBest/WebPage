using TheSingularityWorkshop.Services;
using Xunit;

namespace PongExperience.Tests;

/// <summary>Architecture boundary for the solution-native Pong core experience.</summary>
public sealed class PongExperienceTests
{
    [Fact(DisplayName = "Core_Pong_Experience_Is_Solution_Native")]
    public void Core_Pong_Experience_Is_Solution_Native()
    {
        Assert.Contains(IdleExperienceCatalog.Available, x => x.Id == "pong");
        Assert.Single(IdleExperienceCatalog.Available, x => x.Id == "pong");
    }
}
