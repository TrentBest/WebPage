using System.Linq;
using TheSingularityWorkshop.Gui;
using TheSingularityWorkshop.Workshop.MicroBundles.AI;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialArrangementAndAiTests
{
    [Fact]
    public void Workshop_Arrangement_Moves_A_Building_Without_Overlapping_Another()
    {
        var scene = SpatialWorkshopScene.CreateDefault();
        var arrangement = new SpatialWorkshopArrangement(scene);

        var forge = scene.Interactables.Single(item => item.Id == "forge");
        var before = arrangement.GetBounds(forge);

        Assert.True(arrangement.TryMove(scene, forge.Id, -2, 0));

        var after = arrangement.GetBounds(forge);
        Assert.Equal(before.X - 2, after.X);
        Assert.Equal(before.Y, after.Y);
        Assert.Equal(before.Width, after.Width);
        Assert.Equal(before.Height, after.Height);
    }

    [Fact]
    public void Workshop_Arrangement_Rejects_Overlap()
    {
        var scene = SpatialWorkshopScene.CreateDefault();
        var arrangement = new SpatialWorkshopArrangement(scene);

        var forge = scene.Interactables.Single(item => item.Id == "forge");
        var other = scene.Interactables.First(item => item.Id != "forge");

        var otherBounds = arrangement.GetBounds(other);
        var forgeBounds = arrangement.GetBounds(forge);

        var deltaX = otherBounds.X - forgeBounds.X;
        var deltaY = otherBounds.Y - forgeBounds.Y;

        Assert.False(arrangement.TryMove(scene, forge.Id, deltaX, deltaY));
        Assert.Equal(forgeBounds, arrangement.GetBounds(forge));
    }

    [Fact]
    public void Spatial_Ai_Context_Is_Integer_Addressed_And_Focused()
    {
        var scene = SpatialWorkshopScene.CreateDefault();
        var arrangement = new SpatialWorkshopArrangement(scene);
        var item = scene.Interactables.Single(candidate => candidate.Id == "forge");

        var context = SpatialAiContextFactory.BuildContext(item, arrangement.GetBounds(item));

        Assert.True(context.SubjectId > 0);
        Assert.NotEmpty(context.ContextIds);
        Assert.NotEmpty(context.AvailableOperations);
        Assert.Contains(ProtocolAi.MoveNorth, context.AvailableOperations);
        Assert.Contains(ProtocolAi.MoveEast, context.AvailableOperations);
        Assert.DoesNotContain("forge", context.ToClipboardText(), System.StringComparison.OrdinalIgnoreCase);
    }
}
