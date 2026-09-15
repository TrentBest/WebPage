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
                "OnUpdate → Idle",
                "OnExit → Idle"
            },
            preview.Events);

        preview.Tick();
        Assert.Equal("Forging", preview.State);
        Assert.Contains("OnEnter → Forging", preview.Events);
        Assert.Contains("OnUpdate → Forging (1)", preview.Events);

        preview.Tick();
        preview.Tick();

        Assert.Equal("Finished", preview.State);
        Assert.Equal(3, preview.TickCount);
        Assert.Contains("OnExit → Forging", preview.Events);

        preview.Tick();
        Assert.Equal("Finished", preview.State);
        Assert.Contains("OnEnter → Finished", preview.Events);

        preview.Stop();
        Assert.False(preview.IsRunning);
    }

    [Fact(DisplayName = "FSM Forge stocks reusable lifecycle and transition products")]
    public void CatalogContainsReusableProducts()
    {
        Assert.Contains(FsmForgeCatalog.Products, x => x.Id == "on-enter");
        Assert.Contains(FsmForgeCatalog.Products, x => x.Id == "on-update");
        Assert.Contains(FsmForgeCatalog.Products, x => x.Id == "on-exit");
        Assert.Contains(FsmForgeCatalog.Products, x => x.Id == "conditional-transition");
    }
}
