using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Incremental Unit Test 26 — nested spatial scenes reverse through their traversal history.</summary>
public sealed class SpatialSceneAndLineworkTests
{
    [Fact(DisplayName = "Incremental Unit Test 26 — entering a child scene records its parent and return position")]
    public void SceneStackPushRecordsTraversalFrame()
    {
        var stack = new SpatialSceneStack("workshop");

        stack.Push("forge", new SpatialPoint(78, 41));

        Assert.Equal(2, stack.Depth);
        Assert.Equal("forge", stack.CurrentSceneId);
        Assert.Equal("workshop", stack.Frames[^1].ParentSceneId);
        Assert.Equal(new SpatialPoint(78, 41), stack.Frames[^1].ReturnPosition);
    }

    [Fact(DisplayName = "Incremental Unit Test 26 — leaving a child scene restores the parent traversal level")]
    public void SceneStackPopReversesTraversal()
    {
        var stack = new SpatialSceneStack("workshop");
        stack.Push("forge", new SpatialPoint(78, 41));
        stack.Push("line-lab", new SpatialPoint(50, 50));

        Assert.True(stack.TryPop(out var frame));
        Assert.Equal("line-lab", frame.SceneId);
        Assert.Equal("forge", stack.CurrentSceneId);
        Assert.Equal(2, stack.Depth);
        Assert.False(new SpatialSceneStack("workshop").TryPop(out _));
    }

    [Fact(DisplayName = "Incremental Unit Test 26 — squiggly styles expand a line into reusable render geometry")]
    public void LineRendererAppliesSquigglyEffect()
    {
        using var linework = new SpatialLinework();
        linework.BeginLine(new SpatialPoint(10, 50));
        linework.ContinueLine(new SpatialPoint(90, 50));
        linework.EndLine();

        var rendered = new SpatialLineRenderer().Render(linework, SpatialLineStyleCatalog.Squiggly);

        Assert.Single(rendered);
        Assert.Equal(SpatialLineEffect.Squiggly, rendered[0].Style.Effect);
        Assert.True(rendered[0].Points.Count > 2);
        Assert.NotEqual(rendered[0].Points[1].Y, rendered[0].Points[0].Y);
    }

    [Fact(DisplayName = "Incremental Unit Test 26 — line styles are reusable definitions")]
    public void LineStyleCatalogProvidesReusableStyles()
    {
        Assert.Contains(SpatialLineStyleCatalog.All, style => style.Id == "neon");
        Assert.Contains(SpatialLineStyleCatalog.All, style => style.Id == "blueprint");
        Assert.Contains(SpatialLineStyleCatalog.All, style => style.Id == "warning");
        Assert.Contains(SpatialLineStyleCatalog.All, style => style.Id == "squiggly");
    }
}
