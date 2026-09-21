using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialSecurityCheckpointTests
{
    [Fact]
    public void Checkpoint_Is_A_Physical_Sequence_Not_A_Menu_Sequence()
    {
        var checkpoint = new SpatialSecurityCheckpointModel();

        Assert.Equal(SpatialSecurityCheckpointStage.Approach, checkpoint.Stage);

        checkpoint.EnterQueue();
        Assert.Equal(SpatialSecurityCheckpointStage.Queue, checkpoint.Stage);

        checkpoint.AdvanceQueueTraffic();
        Assert.Contains(3, checkpoint.VacantQueueSlots);

        Assert.True(checkpoint.TryMoveToQueueSlot(3));
        Assert.Equal(3, checkpoint.PlayerQueueSlot);

        checkpoint.AdvanceQueueTraffic();
        Assert.Contains(2, checkpoint.VacantQueueSlots);

        Assert.True(checkpoint.TryMoveToQueueSlot(2));
        Assert.Equal(2, checkpoint.PlayerQueueSlot);
    }

    [Fact]
    public void Cutting_Is_Allowed_But_Digitens_Become_Agitated()
    {
        var checkpoint = new SpatialSecurityCheckpointModel();
        checkpoint.EnterQueue();
        checkpoint.AdvanceQueueTraffic();

        Assert.False(checkpoint.TryMoveToQueueSlot(0));
        Assert.Equal(1, checkpoint.CutAttempts);
        Assert.True(checkpoint.Agitated);
        Assert.Equal(SpatialSecurityCheckpointStage.Queue, checkpoint.Stage);
    }

    [Fact]
    public void Queue_Behind_Visitor_Becomes_Agitated_When_Visitor_Does_Not_Move()
    {
        var checkpoint = new SpatialSecurityCheckpointModel();
        checkpoint.EnterQueue();

        checkpoint.AdvanceQueueTraffic();
        Assert.False(checkpoint.Agitated);

        checkpoint.AdvanceQueueTraffic();

        Assert.True(checkpoint.Agitated);
        Assert.Contains("MOVE", checkpoint.GetAgentSpeech("security-research-03") ?? string.Empty);
    }

    [Fact]
    public void Tray_XRay_Scanner_And_Collection_Are_Separate_Physical_Stations()
    {
        var checkpoint = new SpatialSecurityCheckpointModel();
        checkpoint.EnterQueue();

        checkpoint.AdvanceQueueTraffic();
        checkpoint.TryMoveToQueueSlot(3);
        checkpoint.AdvanceQueueTraffic();
        checkpoint.TryMoveToQueueSlot(2);
        checkpoint.AdvanceQueueTraffic();
        checkpoint.TryMoveToQueueSlot(1);
        checkpoint.AdvanceQueueTraffic();
        checkpoint.TryMoveToQueueSlot(0);

        Assert.Equal(SpatialSecurityCheckpointStage.TrayReady, checkpoint.Stage);

        Assert.True(checkpoint.PlaceTray());
        Assert.True(checkpoint.TrayOnRollers);

        Assert.True(checkpoint.LoadBelongings());
        Assert.True(checkpoint.BelongingsOnTray);

        Assert.True(checkpoint.WalkThroughScanner());
        Assert.Equal("SCANNER CLEARED. GO TO THE OTHER SIDE AND COLLECT YOUR TRAY.", checkpoint.LastMessage);

        Assert.True(checkpoint.CollectTray());
        Assert.True(checkpoint.CanFreeRoam);
    }

    [Fact]
    public void Badge_Tiers_Are_Preserved_After_Checkpoint_Clearance()
    {
        var checkpoint = new SpatialSecurityCheckpointModel();

        checkpoint.EnterQueue();

        checkpoint.AdvanceQueueTraffic();
        Assert.True(checkpoint.TryMoveToQueueSlot(3));

        checkpoint.AdvanceQueueTraffic();
        Assert.True(checkpoint.TryMoveToQueueSlot(2));

        checkpoint.AdvanceQueueTraffic();
        Assert.True(checkpoint.TryMoveToQueueSlot(1));

        checkpoint.AdvanceQueueTraffic();
        Assert.True(checkpoint.TryMoveToQueueSlot(0));

        checkpoint.PlaceTray();
        checkpoint.LoadBelongings();
        checkpoint.WalkThroughScanner();
        checkpoint.CollectTray();

        Assert.True(checkpoint.TryUpgradeBadge("RESEARCH-13", SpatialSecurityClearance.Research));
        Assert.Equal("RESEARCH-13", checkpoint.BadgeId);
        Assert.Equal(SpatialSecurityClearance.Research, checkpoint.BadgeClearance);

        Assert.False(checkpoint.TryUpgradeBadge("VISITOR-2", SpatialSecurityClearance.Visitor));
        Assert.Equal(SpatialSecurityClearance.Research, checkpoint.BadgeClearance);
    }
}
