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

    [Fact(DisplayName = "Incremental Unit Test 39 — AEC drafting records selected floor as spatial Z")]
    public void DraftingCarriesFloorElevation()
    {
        using var draft = new SpatialAECDraftModel
        {
            SelectedFloor = 3,
            SelectedTrade = "structural",
            ElementKind = SpatialAECElementKind.Beam
        };

        draft.Begin(new SpatialPoint(10, 10));
        var element = draft.End(new SpatialPoint(20, 10));

        Assert.Equal(3, element.Floor);
        Assert.Equal(36, element.StartZ);
        Assert.Equal(36, element.EndZ);
        Assert.Equal(SpatialAECElementKind.Beam, element.Kind);
        Assert.Equal("structural", draft.SelectedTrade);
    }

    [Fact(DisplayName = "Incremental Unit Test 40 — AEC drafting undo preserves remaining semantic geometry")]
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
    [Fact(DisplayName = "Incremental Unit Test 41 — AEC intent begins with reality versus fiction")]
    public void IntentStartsAtParadigmLayer()
    {
        var intent = new SpatialAECIntentModel();

        Assert.Equal("Paradigm", SpatialAECIntentModel.LayerNames[intent.CurrentLayer]);
        Assert.Contains("REALITY", intent.OptionsForCurrentLayer);
        Assert.Contains("FICTION", intent.OptionsForCurrentLayer);
    }

    [Fact(DisplayName = "Incremental Unit Test 42 — AEC intent resolves a fictional high-tech laboratory")]
    public void IntentResolvesSciFiLaboratory()
    {
        var intent = new SpatialAECIntentModel();
        var answers = new[]
        {
            "FICTION", "RESEARCH", "FACILITY", "LABORATORY", "HIGH-TECH LABORATORY",
            "SINGLE BUILDING", "SCIENCE", "ADVANCED RESEARCH", "SCI-FI HIGH-TECH LABORATORY"
        };

        foreach (var answer in answers) intent.Answer(answer);

        Assert.True(intent.IsComplete);
        Assert.Equal("SCI-FI HIGH-TECH RESEARCH LABORATORY", intent.BuildingType);
        var structure = intent.Generate();
        Assert.Equal(1, structure.Floors);
        Assert.Contains(structure.Rooms, room => room.Name == "Research Core");
    }

    [Fact(DisplayName = "Incremental Unit Test 43 — AEC generation produces a spatial concept from captured intent")]
    public void IntentGeneratesSpatialConcept()
    {
        var intent = new SpatialAECIntentModel();
        foreach (var answer in new[]
        {
            "REALITY", "BUILT ENVIRONMENT", "FACILITY", "LABORATORY", "RESEARCH FACILITY",
            "VERTICAL FACILITY", "ENGINEERING", "DEVELOPMENT", "RESEARCH LABORATORY"
        }) intent.Answer(answer);

        var structure = intent.Generate(3600);

        Assert.Equal(6, structure.Floors);
        Assert.Equal(3600, structure.SquareUnits);
        Assert.Equal(6, structure.Rooms.Count);
        Assert.All(structure.Rooms, room => Assert.True(room.Width > 0 && room.Depth > 0));
    }
    [Fact(DisplayName = "Incremental Unit Test 44 — Singularity Lab forces the visitor through physical security")]
    public void SingularityLabSecurityIsSequential()
    {
        var lab = new SpatialSingularityLabModel();

        Assert.Equal(SpatialSingularityLabStage.ApproachSecurity, lab.Stage);
        lab.EnterSecurityLine();
        Assert.Equal(SpatialSingularityLabStage.SecurityQueue, lab.Stage);
        lab.PlaceBelongings();
        Assert.Equal(SpatialSingularityLabStage.SecurityScreening, lab.Stage);
        lab.CompleteScreening();

        Assert.True(lab.HasPassedSecurity);
        Assert.Equal(SpatialSingularityLabStage.SecurityComplete, lab.Stage);
    }

    [Fact(DisplayName = "Incremental Unit Test 45 — Singularity Lab elevator requires ID, fingerprint, and retina")]
    public void SingularityLabElevatorAuthenticationIsSequential()
    {
        var lab = new SpatialSingularityLabModel();
        lab.EnterSecurityLine();
        lab.PlaceBelongings();
        lab.CompleteScreening();
        lab.EnterElevator();

        Assert.Equal(SpatialSingularityLabStage.ElevatorElevation, lab.Stage);
        lab.ScanId();
        lab.ScanFingerprint();
        lab.ScanRetina();

        Assert.Equal(SpatialSingularityLabStage.FloorSelection, lab.Stage);
    }

    [Fact(DisplayName = "Incremental Unit Test 46 — Singularity Lab denies unauthorized lower floors")]
    public void SingularityLabDeniesRestrictedFloor()
    {
        var lab = new SpatialSingularityLabModel();
        lab.EnterSecurityLine();
        lab.PlaceBelongings();
        lab.CompleteScreening();
        lab.EnterElevator();
        lab.ScanId();
        lab.ScanFingerprint();
        lab.ScanRetina();
        lab.SelectFloor(5);

        Assert.Equal(SpatialSingularityLabStage.FloorChallenge, lab.Stage);
        lab.ResolveFloorChallenge();

        Assert.Equal(SpatialSingularityLabStage.AccessDenied, lab.Stage);
        Assert.Contains("not cleared", lab.LastMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact(DisplayName = "Incremental Unit Test 47 — Singularity Lab exposes authorized science floors")]
    public void SingularityLabAllowsAuthorizedScienceFloor()
    {
        var lab = new SpatialSingularityLabModel();
        lab.EnterSecurityLine();
        lab.PlaceBelongings();
        lab.CompleteScreening();
        lab.EnterElevator();
        lab.ScanId();
        lab.ScanFingerprint();
        lab.ScanRetina();
        lab.SelectFloor(3);
        lab.ResolveFloorChallenge();

        Assert.Equal(SpatialSingularityLabStage.FloorOpen, lab.Stage);
        Assert.Equal(3, lab.SelectedFloor);
        Assert.Contains("CHEMISTRY", lab.LastMessage);
    }

}
