using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialAECRemoteOfficeTests
{
    [Fact(DisplayName = "Incremental Unit Test 36 — AEC office exposes semantic disciplines and external boundaries")]
    public void AECManifestIsUsable()
    {
        var manifest = SpatialAECRemoteOfficeManifest.CreateDefault();

        Assert.Equal("aec-remote-office", manifest.Id);
        Assert.Contains(manifest.Disciplines, x => x.Id == "architecture");
        Assert.Contains(manifest.Disciplines, x => x.Id == "electrical");
        Assert.Contains(manifest.ExternalTargets, x => x.Id == "revit");
        Assert.True(manifest.SupportsLineType("electrical"));
    }

    [Fact(DisplayName = "Incremental Unit Test 37 — AEC office has a reachable exterior entry point")]
    public void AECOfficeEntryPointIsReachable()
    {
        var scene = SpatialWorkshopScene.CreateDefault();
        var office = scene.Interactables.Single(x => x.Id == "aec-remote-office");
        var target = new SpatialPoint(
            office.InteractionPoint.X + office.InteractionPoint.Width / 2d,
            office.InteractionPoint.Y + office.InteractionPoint.Height / 2d);

        var path = SpatialGeometryEngine.FindPath(
            scene.StartingPosition,
            target,
            SpatialGeometryEngine.FromInteractables(scene.Interactables));

        Assert.NotEmpty(path);
        Assert.Equal(target, path[^1]);
    }

    [Fact(DisplayName = "Incremental Unit Test 38 — AEC drafting snaps and auto-dimensions a wall")]
    public void DraftingCreatesDimensionedSemanticElement()
    {
        using var draft = new SpatialAECDraftModel
        {
            ElementKind = SpatialAECElementKind.Wall,
            Snap = 1,
            Orthogonal = true
        };

        draft.Begin(new SpatialPoint(10.2, 20.4));
        var element = draft.End(new SpatialPoint(25.7, 23.8));

        Assert.Equal(SpatialAECElementKind.Wall, element.Kind);
        Assert.Equal(new SpatialPoint(10, 20), element.Line.Start);
        Assert.Equal(new SpatialPoint(26, 20), element.Line.End);
        Assert.Equal(16, element.Length);
        Assert.Equal(0, element.AngleDegrees);
    }

    [Fact(DisplayName = "Incremental Unit Test 39 — AEC drafting undo preserves remaining semantic geometry")]
    public void UndoRemovesOnlyLastElement()
    {
        using var draft = new SpatialAECDraftModel();

        draft.Begin(new SpatialPoint(1, 1));
        draft.End(new SpatialPoint(5, 1));
        draft.ElementKind = SpatialAECElementKind.Column;
        draft.Begin(new SpatialPoint(5, 5));
        draft.End(new SpatialPoint(5, 8));

        draft.Undo();

        Assert.Single(draft.Elements);
        Assert.Equal(SpatialAECElementKind.Wall, draft.Elements[0].Kind);
        Assert.Single(draft.Linework.Lines);
    }
}
