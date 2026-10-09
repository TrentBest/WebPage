using TheSingularityWorkshop.Workshop.Composition;
using TheSingularityWorkshop.Workshop.Experiences;
using TheSingularityWorkshop.Workshop.MicroBundles;
using Xunit;

namespace SingularityHub.Tests;

public sealed class WorkshopDeepDiveCatalogTests
{
    [Fact]
    public void DeepDiveCatalogResolvesTheManifestNamedExperienceByStableIdentity()
    {
        var experienceCatalog = new WorkshopExperienceCatalog();
        var compositionCatalog = new WorkshopCompositionCatalog();
        var deepDiveCatalog = new WorkshopDeepDiveCatalog(experienceCatalog, compositionCatalog);

        var experience = Assert.Single(experienceCatalog.Experiences);
        Assert.Equal("LIVING GUI", experience.Name);
        Assert.Equal(LivingGuiExperience.ExperienceId, experience.Id);

        Assert.True(deepDiveCatalog.TryResolve(experience.Id, out var model));
        Assert.NotNull(model);
        Assert.Equal(experience.Id, model!.Experience.Id);
        Assert.Equal("LIVING GUI", model.Experience.Name);
        Assert.Equal((ulong)LivingGuiExperienceMicroBundle.BundleId, model.PrimaryBundle!.Id);
        Assert.Equal("1 DECLARED DEPENDENCY", model.DependencySummary);
    }

    [Fact]
    public void DeepDiveCatalogDoesNotInventAnExperienceForAnUnknownIdentity()
    {
        var deepDiveCatalog = new WorkshopDeepDiveCatalog(
            new WorkshopExperienceCatalog(),
            new WorkshopCompositionCatalog());

        Assert.False(deepDiveCatalog.TryResolve(ulong.MaxValue, out var model));
        Assert.Null(model);
    }
}
