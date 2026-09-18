using TheSingularityWorkshop.Workshop.MicroBundles;

namespace SingularityHub.Tests;

/// <summary>Heartbeat tests proving the leaderboard is a reusable, evolving MicroBundle capability.</summary>
public sealed class WorkshopLeaderboardMicroBundleTests
{
    [Fact]
    public void Leaderboard_Exposes_Reusable_MicroBundle_Capability()
    {
        using var leaderboard = new LeaderboardMicroBundle(
            "Workshop Maze Leaderboard",
            [
                new LeaderboardEntry("Avery", 102_180, 118),
                new LeaderboardEntry("Morgan", 127_510, 143)
            ]);

        Assert.Equal(2201UL, leaderboard.Id);
        Assert.Equal("LEADERBOARD", leaderboard.Presentation.Surface);
        Assert.Equal(2, leaderboard.Entries.Count);
        Assert.Equal("01:42.18", leaderboard.Entries[0].DisplayTime);
    }

    [Fact]
    public void Leaderboard_Changes_When_New_Better_Data_Arrives()
    {
        using var leaderboard = new LeaderboardMicroBundle("Workshop Maze Leaderboard");

        Assert.True(leaderboard.Record("Casey", 180_000, 210));
        Assert.False(leaderboard.Record("Casey", 190_000, 220));
        Assert.True(leaderboard.Record("Casey", 175_000, 205));

        Assert.True(leaderboard.TryGet("Casey", out var entry));
        Assert.Equal(175_000, entry.TimeMilliseconds);
        Assert.Equal(205, entry.Steps);
    }
}
