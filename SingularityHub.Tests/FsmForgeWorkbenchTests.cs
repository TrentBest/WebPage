using TheSingularityWorkshop.Infrastructure.FsmForge;

namespace SingularityHub.Tests;

public sealed class FsmForgeWorkbenchTests
{
    [Fact]
    public void StateIngotCreatesStatePlateAndExecutableDefinition()
    {
        var workbench = new FsmForgeWorkbench();

        workbench.SelectStock("state-ingot");
        var plate = workbench.PlaceSelectedState();

        Assert.Equal("State 1", plate.Name);
        Assert.Single(workbench.StatePlates);
        Assert.Equal("State 1", workbench.Definition.InitialState);
        Assert.Single(workbench.Definition.States);
        Assert.Null(workbench.SelectedStockId);
    }

    [Fact]
    public void NamingStatePlateChangesSemanticDefinition()
    {
        var workbench = new FsmForgeWorkbench();

        workbench.SelectStock("state-ingot");
        workbench.PlaceSelectedState();
        workbench.NameState(0, "Idle");

        Assert.Equal("Idle", workbench.StatePlates[0].Name);
        Assert.Equal("Idle", workbench.Definition.InitialState);
        Assert.Equal("Idle", workbench.Definition.States[0].Name);
    }

    [Fact]
    public void TransitionPlateRequiresBothStatesAndBecomesExecutableDefinition()
    {
        var workbench = new FsmForgeWorkbench();

        workbench.SelectStock("state-ingot");
        workbench.PlaceSelectedState();
        workbench.NameState(0, "Idle");

        workbench.SelectStock("state-ingot");
        workbench.PlaceSelectedState();
        workbench.NameState(1, "Running");

        var transition = workbench.PlaceTransition("Idle", "Running", "RUN");

        Assert.Equal("Idle", transition.FromState);
        Assert.Equal("Running", transition.ToState);
        Assert.Equal("RUN", transition.Signal);
        Assert.Single(workbench.Definition.Transitions);
        Assert.True(workbench.Definition.CanPreview(out var reason));
        Assert.Equal("READY", reason);
    }
}
