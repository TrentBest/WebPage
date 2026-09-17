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

    [Fact(DisplayName = "Incremental Unit Test 26 — WorkshopRouter owns nested spatial traversal")]
    public void WorkshopRouterRoutesIntoAndBackOutOfNestedScenes()
    {
        var router = new WorkshopRouter();

        router.Enter("forge", new SpatialPoint(78, 41));
        router.Enter("line-lab", new SpatialPoint(50, 50));

        Assert.Equal("line-lab", router.CurrentSceneId);
        Assert.Equal(3, router.Depth);

        Assert.True(router.TryExit(out var frame));
        Assert.Equal("line-lab", frame.SceneId);
        Assert.Equal("forge", router.CurrentSceneId);
        Assert.Equal(new SpatialPoint(50, 50), frame.ReturnPosition);

        Assert.True(router.TryExit(out frame));
        Assert.Equal("forge", frame.SceneId);
        Assert.Equal("workshop", router.CurrentSceneId);
        Assert.Equal(new SpatialPoint(78, 41), frame.ReturnPosition);
        Assert.False(router.TryExit(out _));
    }

    [Fact(DisplayName = "Incremental Unit Test 27 — WorkshopRouter places Forge visitors inside the building")]
    public void WorkshopRouterForgeEntryPointIsInsideInterior()
    {
        var router = new WorkshopRouter();

        var entry = router.GetEntryPoint("forge");

        Assert.Equal(new SpatialPoint(50, 78), entry);
        Assert.True(entry.X > 8 && entry.X < 92);
        Assert.True(entry.Y > 8 && entry.Y < 92);
    }

    [Fact(DisplayName = "Incremental Unit Test 27 — WorkshopRouter exposes the Linework Lab as a nested scene")]
    public void WorkshopRouterLineLabHasItsOwnEntryPoint()
    {
        var router = new WorkshopRouter();

        router.Enter("forge", new SpatialPoint(50, 78));
        router.Enter("line-lab", new SpatialPoint(50, 78));

        Assert.Equal("line-lab", router.CurrentSceneId);
        Assert.Equal(new SpatialPoint(50, 78), router.GetEntryPoint("line-lab"));
        Assert.Equal("forge", router.Frames[^1].ParentSceneId);
    }

    [Fact(DisplayName = "Incremental Unit Test 27 — Forge structure is authored as reusable line primitives")]
    public void ForgeLineworkFactoryBuildsStructuralPrimitives()
    {
        using var linework = SpatialForgeLineworkFactory.Create();

        Assert.Equal(32, linework.Lines.Count);
        Assert.Equal(32, linework.TextureBuffer.Width);
        Assert.Equal(1, SpatialLineTextureBuffer.Height);
    }

    [Fact(DisplayName = "Incremental Unit Test 27 — Forge structure passes through the shared line renderer")]
    public void ForgeStructureRendersThroughSpatialLineRenderer()
    {
        using var linework = SpatialForgeLineworkFactory.Create();

        var rendered = new SpatialLineRenderer().Render(linework, SpatialLineStyleCatalog.Blueprint);

        Assert.Equal(linework.Lines.Count, rendered.Count);
        Assert.All(rendered, line => Assert.Equal(SpatialLineStyleCatalog.Blueprint, line.Style));
        Assert.All(rendered, line => Assert.Equal(2, line.Points.Count));
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
