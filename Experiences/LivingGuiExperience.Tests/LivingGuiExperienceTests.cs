using TheSingularityWorkshop.Services;
using Xunit;

namespace LivingGuiExperience.Tests;

/// <summary>Architecture boundary for the solution-native Living GUI core experience.</summary>
public sealed class LivingGuiExperienceTests
{
    [Fact(DisplayName = "Core_LivingGui_Experience_Is_Solution_Native")]
    public void Core_LivingGui_Experience_Is_Solution_Native()
    {
        Assert.True(FlexExperienceCatalog.IsAvailable("living-gui"));
        Assert.Contains(FlexExperienceCatalog.Available, x => x.Id == "living-gui");
    }
}
