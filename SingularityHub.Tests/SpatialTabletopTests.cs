using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialTabletopTests
{
    [Fact(DisplayName = "Incremental Unit Test 70 — tabletop provides neutral GM utilities")]
    public void TabletopProvidesNeutralUtilities()
    {
        var manifest = SpatialTabletopManifest.CreateDefault();

        Assert.Contains(manifest.Utilities, x => x.Id == "gm-control");
        Assert.Contains(manifest.Utilities, x => x.Id == "tile-placement");
        Assert.Contains(manifest.Utilities, x => x.Id == "piece-placement");
        Assert.Contains(manifest.Utilities, x => x.Id == "seat-view");
        Assert.Contains(manifest.Utilities, x => x.Id == "custom-rule-panel");
    }

    [Fact(DisplayName = "Incremental Unit Test 71 — tabletop does not embed a commercial ruleset")]
    public void TabletopDefinesEnvironmentNotRules()
    {
        var manifest = SpatialTabletopManifest.CreateDefault();

        Assert.Null(manifest.FindUtility("combat-rules"));
        Assert.Null(manifest.FindUtility("character-class-rules"));
        Assert.Null(manifest.FindUtility("published-game-rules"));
    }

    [Fact(DisplayName = "Incremental Unit Test 72 — players receive seat-bound perspectives and avatars")]
    public void SeatsProvidePerspectiveAndAvatarSupport()
    {
        var manifest = SpatialTabletopManifest.CreateDefault();

        Assert.Equal("GAME MASTER", manifest.FindSeat("game-master")!.Name);
        Assert.Equal("PLAYER 1", manifest.FindSeat("player-1")!.Name);
        Assert.Equal("TABLETOP", manifest.FindSurface("tabletop")!.Name);
        Assert.Equal("TABLE AVATAR", manifest.FindUtility("avatar")!.Name);
        Assert.Equal("SEAT VIEW", manifest.FindUtility("seat-view")!.Name);
    }
}
