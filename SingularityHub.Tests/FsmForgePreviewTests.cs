using TheSingularityWorkshop.Infrastructure.FsmForge;
using Xunit;

namespace SingularityHub.Tests;

public sealed class FsmForgePreviewTests
{
    [Fact(DisplayName = "FSM Forge preview starts with a real FSM definition")]
    public void PreviewStartsWithInitialState()
    {
        var preview = new FsmForgePreview();

        preview.Start();

        Assert.Equal("Idle", preview.State);
        Assert.True(preview.IsRunning);
        Assert.Empty(preview.Events);

        preview.Stop();
    }

    [Fact(DisplayName = "FSM Forge preview advances through real state lifecycle")]
    public void PreviewAdvancesThroughLifecycle()
    {
        var preview = new FsmForgePreview();
        preview.Start();

        preview.Tick();
        Assert.Equal("Forging", preview.State);
        Assert.Equal(
            new[]
            {
                "OnInitialize → Idle",
                "OnUpdate → Idle",
                "OnExit → Idle"
            },
            preview.Events);

        preview.Tick();
        Assert.Equal("Forging", preview.State);
        Assert.Contains("OnInitialize → Forging", preview.Events);
        Assert.Contains("OnUpdate → Forging", preview.Events);

        preview.Tick();
        preview.Tick();

        Assert.Equal("Finished", preview.State);
        Assert.Equal(4, preview.TickCount);
        Assert.Contains("OnExit → Forging", preview.Events);

        preview.Tick();
        Assert.Equal("Finished", preview.State);
        Assert.Contains("OnInitialize → Finished", preview.Events);

        preview.Stop();
        Assert.False(preview.IsRunning);
    }

    [Fact(DisplayName = "FSM Forge cannot preview an unbound transition")]
    public void UnboundTransitionBlocksPreview()
    {
        var definition = new FsmForgeDefinition();
        definition.AddState("Idle");
        definition.AddState("Running");
        definition.AddTransition(
            "Idle",
            "Running",
            "ShouldRun",
            new FsmForgeUnboundCondition());

        Assert.False(definition.CanPreview(out var reason));
        Assert.Contains("PLUG A CONDITION", reason, StringComparison.Ordinal);

        var preview = new FsmForgePreview();
        Assert.False(preview.CanPreview(definition, out _));
        Assert.Throws<InvalidOperationException>(() => preview.Start(definition));
    }

    [Fact(DisplayName = "FSM Forge condition ingot makes a scaffolded transition executable")]
    public void ConditionIngotMakesTransitionExecutable()
    {
        var definition = new FsmForgeDefinition();
        definition.AddState("Idle");
        definition.AddState("Running");
        definition.AddTransition(
            "Idle",
            "Running",
            "ShouldRun",
            new FsmForgeSignalCondition("RUN"));

        Assert.True(definition.CanPreview(out var reason));
        Assert.Equal("READY", reason);

        var preview = new FsmForgePreview();
        preview.Start(definition);

        preview.Tick();
        Assert.Equal("Idle", preview.State);

        preview.SetSignal("RUN");
        preview.Tick();
        Assert.Equal("Running", preview.State);

        preview.Stop();
    }

    [Fact(DisplayName = "FSM Forge stocks reusable lifecycle and transition products")]
    public void CatalogContainsReusableProducts()
    {
        Assert.Contains(TheSingularityWorkshop.Infrastructure.FsmForge.FsmForgeCatalog.Products, x => x.Id == "on-enter");
        Assert.Contains(TheSingularityWorkshop.Infrastructure.FsmForge.FsmForgeCatalog.Products, x => x.Id == "on-update");
        Assert.Contains(TheSingularityWorkshop.Infrastructure.FsmForge.FsmForgeCatalog.Products, x => x.Id == "on-exit");
        Assert.Contains(TheSingularityWorkshop.Infrastructure.FsmForge.FsmForgeCatalog.Products, x => x.Id == "conditional-transition");
    }

    [Fact(DisplayName = "FSM Forge scaffolder emits the assembled states and conditions")]
    public void ScaffolderEmitsDefinition()
    {
        var definition = new FsmForgeDefinition();
        definition.AddState("Idle");
        definition.AddState("Running");
        definition.AddTransition(
            "Idle",
            "Running",
            "ShouldRun",
            new FsmForgeSignalCondition("RUN"));

        var scaffold = FsmForgeScaffolder.Generate(definition);

        Assert.Contains(".State(\"Idle\"", scaffold, StringComparison.Ordinal);
        Assert.Contains(".State(\"Running\"", scaffold, StringComparison.Ordinal);
        Assert.Contains(".WithInitialState(\"Idle\")", scaffold, StringComparison.Ordinal);
        Assert.Contains(".Transition(\"Idle\", \"Running\", ShouldRun)", scaffold, StringComparison.Ordinal);
        Assert.Contains("Forge preview condition: signal \"RUN\"", scaffold, StringComparison.Ordinal);
        Assert.Contains("TODO: replace the scaffold", scaffold, StringComparison.Ordinal);
    }
}
