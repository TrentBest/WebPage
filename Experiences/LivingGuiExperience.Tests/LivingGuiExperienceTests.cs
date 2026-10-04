using TheSingularityWorkshop.SingularityHub;
using TheSingularityWorkshop.Workshop.Experiences;
using TheSingularityWorkshop.Workshop.MicroBundles;
using Xunit;

namespace LivingGuiExperience.Tests;

/// <summary>Architecture boundary for the solution-native Living GUI core experience.</summary>
public sealed class LivingGuiExperienceTests
{
    [Fact(DisplayName = "Core_LivingGui_Experience_Is_Solution_Native")]
    public void Core_LivingGui_Experience_Is_Solution_Native()
    {
        var experience = new LivingGuiExperience();

        Assert.Equal(LivingGuiExperience.ExperienceId, experience.Id);
        Assert.Equal("LIVING GUI", experience.Name);
        Assert.Contains(LivingGuiExperience.LivingGuiBundleId, experience.MicroBundleIds);
        Assert.Contains(1UL, experience.Capabilities);
        Assert.NotEmpty(experience.ProcessingGroups);
    }

    [Fact(DisplayName = "LivingGui_MicroBundle_Declares_Canonical_Moniker_Dependency")]
    public void LivingGuiMicroBundleDeclaresCanonicalMonikerDependency()
    {
        var bundle = new LivingGuiExperienceMicroBundle();

        Assert.Contains(
            bundle.Dependencies,
            dependency => dependency.BundleId == (ulong)MonikerMicroBundle.BundleId);
        Assert.Equal(new BundleVersion(1, 0, 0), bundle.Descriptor.Version);
    }
}
