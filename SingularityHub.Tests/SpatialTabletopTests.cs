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

        var gameMaster = manifest.FindSeat("game-master");
        var playerOne = manifest.FindSeat("player-1");
        var tabletop = manifest.FindSurface("tabletop");
        var avatar = manifest.FindUtility("avatar");
        var seatView = manifest.FindUtility("seat-view");

        Assert.True(gameMaster.HasValue);
        Assert.True(playerOne.HasValue);
        Assert.True(tabletop.HasValue);
        Assert.True(avatar.HasValue);
        Assert.True(seatView.HasValue);

        Assert.Equal("GAME MASTER", gameMaster.Value.Name);
        Assert.Equal("PLAYER 1", playerOne.Value.Name);
        Assert.Equal("TABLETOP", tabletop.Value.Name);
        Assert.Equal("TABLE AVATAR", avatar.Value.Name);
        Assert.Equal("SEAT VIEW", seatView.Value.Name);
    }
}
