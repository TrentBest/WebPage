using TheSingularityWorkshop.Workshop.Rendering;
using Xunit;

namespace SingularityHub.Tests;

public sealed class RenderingPerceptionTests
{
    [Fact]
    public void Perception_Becomes_Less_Frequent_As_Distance_Increases()
    {
        var model = new RenderingPerceptionModel();

        Assert.Equal("CONTACT", model.Resolve(.25).Name);
        Assert.Equal("REACH", model.Resolve(1).Name);
        Assert.Equal("CLOSE", model.Resolve(2).Name);
        Assert.Equal("MEDIUM", model.Resolve(4).Name);
        Assert.Equal("FAR", model.Resolve(8).Name);
        Assert.Equal("HORIZON", model.Resolve(64).Name);

        Assert.True(model.Resolve(.25).UpdateInterval < model.Resolve(64).UpdateInterval);
    }

    [Fact]
    public void Distant_Buffer_Refreshes_When_Enough_Space_Has_Been_Traversed()
    {
        var model = new RenderingPerceptionModel();

        Assert.False(model.ShouldRefresh(20, TimeSpan.FromMilliseconds(10), 1));
        Assert.True(model.ShouldRefresh(20, TimeSpan.FromMilliseconds(10), 4));
        Assert.True(model.ShouldRefresh(20, TimeSpan.FromMilliseconds(600), 0));
    }
}
