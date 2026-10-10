using TheSingularityWorkshop.FSM_COS;
using TheSingularityWorkshop.MicroBundleDomain;
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
    public void DeepDiveUsesTheActualResolvedRuntimeAssemblyWhenItContainsTheRequestedRoot()
    {
        var experienceCatalog = new WorkshopExperienceCatalog();
        var compositionCatalog = new WorkshopCompositionCatalog();
        var deepDiveCatalog = new WorkshopDeepDiveCatalog(experienceCatalog, compositionCatalog);
        var experience = Assert.Single(experienceCatalog.Experiences);
        var runtimeAssembly = new FsmCos(compositionCatalog).Execute(new RuntimeManifest(
            RuntimeId: 42,
            Bundles:
            [
                new MicroBundleManifestEntry(LivingGuiExperienceMicroBundle.BundleId, "1.0.0")
            ]));

        Assert.True(deepDiveCatalog.TryResolve(
            experience.Id,
            runtimeAssembly,
            [LivingGuiExperienceMicroBundle.BundleId],
            out var model));
        Assert.NotNull(model);
        Assert.True(model!.IsRuntimeAssemblyBacked);
        Assert.Equal((ulong)42, model.RuntimeAssembly!.RuntimeId);
        Assert.Equal(2, model.ResolvedBundleCount);
        Assert.Equal(new[] { (ulong)LivingGuiExperienceMicroBundle.BundleId }, model.RequestedBundleIds);
        Assert.Equal((ulong)LivingGuiExperienceMicroBundle.BundleId, model.PrimaryBundle!.Id);
        Assert.Contains(model.Bundles, bundle => bundle.Id == (ulong)MonikerMicroBundle.BundleId);
    }

    [Fact]
    public void DeepDiveDoesNotAttachAnUnrelatedRuntimeAssembly()
    {
        var experienceCatalog = new WorkshopExperienceCatalog();
        var compositionCatalog = new WorkshopCompositionCatalog();
        var deepDiveCatalog = new WorkshopDeepDiveCatalog(experienceCatalog, compositionCatalog);
        var experience = Assert.Single(experienceCatalog.Experiences);
        var unrelatedAssembly = new FsmCos(compositionCatalog).Execute(new RuntimeManifest(
            RuntimeId: 43,
            Bundles:
            [
                new MicroBundleManifestEntry((ulong)MonikerMicroBundle.BundleId, "1.0.0")
            ]));

        Assert.True(deepDiveCatalog.TryResolve(experience.Id, unrelatedAssembly, out var model));
        Assert.NotNull(model);
        Assert.False(model!.IsRuntimeAssemblyBacked);
        Assert.Equal(1, model.ResolvedBundleCount);
        Assert.Equal((ulong)LivingGuiExperienceMicroBundle.BundleId, model.PrimaryBundle!.Id);
    }

    [Fact]
    public void DeepDiveDoesNotAcceptAnAssemblyForRootsThatBelongToAnotherExperience()
    {
        var experienceCatalog = new WorkshopExperienceCatalog();
        var compositionCatalog = new WorkshopCompositionCatalog();
        var deepDiveCatalog = new WorkshopDeepDiveCatalog(experienceCatalog, compositionCatalog);
        var experience = Assert.Single(experienceCatalog.Experiences);
        var assembly = new FsmCos(compositionCatalog).Execute(new RuntimeManifest(
            RuntimeId: 44,
            Bundles:
            [
                MicroBundleDependencyRequest.Unconfigured((ulong)MonikerMicroBundle.BundleId)
            ]));

        // The supplied root exists in the assembly, but is not a root declared by Living GUI.
        Assert.True(deepDiveCatalog.TryResolve(
            experience.Id,
            assembly,
            [(ulong)MonikerMicroBundle.BundleId],
            out var model));
        Assert.NotNull(model);
        Assert.False(model!.IsRuntimeAssemblyBacked);
        Assert.Equal((ulong)LivingGuiExperienceMicroBundle.BundleId, model.PrimaryBundle!.Id);
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
