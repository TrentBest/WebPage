using HubKernel = TheSingularityWorkshop.SingularityHub.SingularityHub;
using TheSingularityWorkshop.SingularityHub;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Layer 7: Process Groups. The Hub tracks eligibility/lifecycle without owning execution mechanics.</summary>
public sealed class ProcessGroupTests
{
    [ArchitectureTest(7, 1, 1)]
    [Fact(DisplayName = "7.01.001 — Register_Creates_Registered_Group")]
    public void Register_Creates_Registered_Group()
    {
        var hub = new HubKernel();
        Assert.True(hub.Register(7101));
        Assert.DoesNotContain(hub.ActiveGroups, g => g.Id == 7101);
    }

    [ArchitectureTest(7, 1, 2)]
    [Fact(DisplayName = "7.01.002 — Duplicate_Register_Is_Rejected")]
    public void Duplicate_Register_Is_Rejected()
    {
        var hub = new HubKernel();
        Assert.True(hub.Register(7102));
        Assert.False(hub.Register(7102));
    }

    [ArchitectureTest(7, 1, 3)]
    [Fact(DisplayName = "7.01.003 — Registered_Group_Can_Activate")]
    public void Registered_Group_Can_Activate()
    {
        var hub = new HubKernel();
        Assert.True(hub.Register(7103));
        Assert.True(hub.Activate(7103));
        var group = Assert.Single(hub.ActiveGroups);
        Assert.Equal(ProcessGroupState.Active, group.State);
    }

    [ArchitectureTest(7, 1, 4)]
    [Fact(DisplayName = "7.01.004 — Unknown_Group_Cannot_Activate")]
    public void Unknown_Group_Cannot_Activate()
    {
        var hub = new HubKernel();
        Assert.False(hub.Activate(7104));
        Assert.Empty(hub.ActiveGroups);
    }

    [ArchitectureTest(7, 1, 5)]
    [Fact(DisplayName = "7.01.005 — Active_Group_Can_Complete")]
    public void Active_Group_Can_Complete()
    {
        var hub = new HubKernel();
        Assert.True(hub.Register(7105));
        Assert.True(hub.Activate(7105));
        Assert.True(hub.Complete(7105));
        Assert.Empty(hub.ActiveGroups);
    }

    [ArchitectureTest(7, 1, 6)]
    [Fact(DisplayName = "7.01.006 — Registered_Group_Cannot_Complete")]
    public void Registered_Group_Cannot_Complete()
    {
        var hub = new HubKernel();
        Assert.True(hub.Register(7106));
        Assert.False(hub.Complete(7106));
    }

    [ArchitectureTest(7, 1, 7)]
    [Fact(DisplayName = "7.01.007 — Completed_Group_Cannot_Complete_Again")]
    public void Completed_Group_Cannot_Complete_Again()
    {
        var hub = new HubKernel();
        Assert.True(hub.Register(7107));
        Assert.True(hub.Activate(7107));
        Assert.True(hub.Complete(7107));
        Assert.False(hub.Complete(7107));
    }

    [ArchitectureTest(7, 1, 8)]
    [Fact(DisplayName = "7.01.008 — Completed_Group_Is_Not_Active")]
    public void Completed_Group_Is_Not_Active()
    {
        var hub = new HubKernel();
        Assert.True(hub.Register(7108));
        Assert.True(hub.Activate(7108));
        Assert.True(hub.Complete(7108));
        Assert.DoesNotContain(hub.ActiveGroups, g => g.Id == 7108);
    }
}
