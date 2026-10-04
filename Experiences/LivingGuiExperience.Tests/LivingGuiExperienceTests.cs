using TheSingularityWorkshop.FSM_COS;
using TheSingularityWorkshop.Workshop.MicroBundles;
using Xunit;

using LivingGui = TheSingularityWorkshop.Workshop.Experiences.LivingGuiExperience;

namespace LivingGuiExperience.Tests;

/// <summary>Architecture boundary for the solution-native Living GUI core experience.</summary>
public sealed class LivingGuiExperienceTests
{
    [Fact(DisplayName = "Core_LivingGui_Experience_Is_Solution_Native")]
    public void Core_LivingGui_Experience_Is_Solution_Native()
    {
        var experience = new LivingGui();

        Assert.Equal(LivingGui.ExperienceId, experience.Id);
        Assert.Equal("LIVING GUI", experience.Name);
        Assert.Contains(LivingGui.LivingGuiBundleId, experience.MicroBundleIds);
        Assert.Contains(1UL, experience.Capabilities);
        Assert.NotEmpty(experience.ProcessingGroups);
    }

    [Fact(DisplayName = "LivingGui_MicroBundle_Declares_Canonical_Moniker_Dependency")]
    public void LivingGuiMicroBundleDeclaresCanonicalMonikerDependency()
    {
        var bundle = new LivingGuiExperienceMicroBundle();

        Assert.Contains(
            BundleRequest.Unconfigured((ulong)MonikerMicroBundle.BundleId),
            bundle.Dependencies);
        Assert.Equal("1.0.0", bundle.Descriptor.Version);
    }
}
