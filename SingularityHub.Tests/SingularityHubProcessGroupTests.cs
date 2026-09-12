using System.Collections.Generic;
using TheSingularityWorkshop.SingularityHub;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SingularityHubProcessGroupTests
{
    [Fact(DisplayName = "Hub_Update_Steps_Only_Root_Process_Groups")]
    public void Hub_Update_Steps_Only_Root_Process_Groups()
    {
        var stepped = new List<string>();
        var hub = new SingularityHub(stepped.Add)
            .RegisterProcessGroup("Simulation")
            .RegisterProcessGroup("Simulation.Physics", "Simulation")
            .RegisterProcessGroup("Presentation");

        hub.Update();

        Assert.Equal(new[] { "Simulation", "Presentation" }, stepped);
    }

    [Fact(DisplayName = "Hub_Nested_Update_Steps_Direct_Dependencies_In_Order")]
    public void Hub_Nested_Update_Steps_Direct_Dependencies_In_Order()
    {
        var stepped = new List<string>();
        var hub = new SingularityHub(stepped.Add)
            .RegisterProcessGroup("Simulation")
            .RegisterProcessGroup("Simulation.Physics", "Simulation")
            .RegisterProcessGroup("Simulation.Actors", "Simulation")
            .RegisterProcessGroup("Simulation.Physics.Collision", "Simulation.Physics");

        hub.Update();
        hub.UpdateNestedProcessGroups("Simulation");

        Assert.Equal(
            new[] { "Simulation", "Simulation.Physics", "Simulation.Actors" },
            stepped);
    }

    [Fact(DisplayName = "Hub_Rejects_Nested_Group_When_Parent_Is_Not_Registered")]
    public void Hub_Rejects_Nested_Group_When_Parent_Is_Not_Registered()
    {
        var hub = new SingularityHub(_ => { });

        Assert.Throws<InvalidOperationException>(() =>
            hub.RegisterProcessGroup("Simulation.Physics", "Simulation"));
    }

    [Fact(DisplayName = "Hub_Rejects_Duplicate_Process_Group")]
    public void Hub_Rejects_Duplicate_Process_Group()
    {
        var hub = new SingularityHub(_ => { }).RegisterProcessGroup("Simulation");

        Assert.Throws<InvalidOperationException>(() =>
            hub.RegisterProcessGroup("Simulation"));
    }
}
