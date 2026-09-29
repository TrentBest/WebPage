using TheSingularityWorkshop.Services;
using TheSingularityWorkshop.Workshop.Composition;
using Xunit;

namespace SingularityHub.Tests;

public sealed class WorkshopIntegrationSelectionTests
{
    [Fact(DisplayName = "FSM_COS integration is selected before runtime assembly")]
    public void FsmCosSelectionBuildsRuntimeAssembly()
    {
        using var experience = new WorkshopExperienceService();
        experience.Initialize();

        while (experience.FirstContact.CurrentState != "Gateway")
            experience.Tick();

        experience.RequestEntry();
        experience.Tick();

        Assert.True(experience.FirstContact.IsIntegrationChoice);
        Assert.Null(experience.RuntimeAssembly);

        experience.SelectIntegration(WorkshopIntegrationMode.FsmCos);

        Assert.Equal(WorkshopIntegrationMode.FsmCos, experience.IntegrationMode);
        Assert.NotNull(experience.RuntimeAssembly);
        Assert.Equal(5, experience.RuntimeAssembly!.Bundles.Count);
    }

    [Fact(DisplayName = "Blazor integration leaves FSM_COS uninvoked")]
    public void BlazorSelectionDoesNotAssembleCosRuntime()
    {
        using var experience = new WorkshopExperienceService();
        experience.Initialize();

        while (experience.FirstContact.CurrentState != "Gateway")
            experience.Tick();

        experience.RequestEntry();
        experience.Tick();
        experience.SelectIntegration(WorkshopIntegrationMode.Blazor);

        Assert.Equal(WorkshopIntegrationMode.Blazor, experience.IntegrationMode);
        Assert.Null(experience.RuntimeAssembly);
    }
}
