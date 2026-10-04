using TheSingularityWorkshop.Workshop.Rendering;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Preserves the renderer's intent boundary as an independently testable contract.</summary>
public sealed class RenderingIntentTests
{
    [Fact]
    public void RenderingIntent_Clamps_Dimension_And_Exposes_Visual_Domain()
    {
        var plan = new RenderingIntent("hull", -1d, RenderingCameraBehavior.Frame);
        var isometric = new RenderingIntent("bridge", .5d, RenderingCameraBehavior.Follow);
        var perspective = new RenderingIntent("engineering", 2d, RenderingCameraBehavior.Orbit);

        Assert.Equal(0d, plan.ClampedDimension);
        Assert.Equal("2D", plan.Domain);

        Assert.Equal(.5d, isometric.ClampedDimension);
        Assert.Equal("ISO / HYBRID", isometric.Domain);
        Assert.Equal(RenderingCameraBehavior.Follow, isometric.CameraBehavior);

        Assert.Equal(1d, perspective.ClampedDimension);
        Assert.Equal("3D", perspective.Domain);
        Assert.Equal("engineering", perspective.TargetId);
    }
    [Fact(DisplayName = "Rendering research module uses an explicit cache-busting asset version")]
    public void RenderingResearchModuleUsesExplicitCacheBustingAssetVersion()
    {
        var explore = File.ReadAllText(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "TheSingularityWorkshop", "Pages", "Explore.razor"));

        Assert.Contains("voxel-substrate-lab.js?v=", explore);
        Assert.DoesNotContain("voxel-substrate-lab.js?v=166", explore);
    }

}
