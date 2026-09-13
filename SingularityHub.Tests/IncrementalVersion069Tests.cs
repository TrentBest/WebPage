using TheSingularityWorkshop.Workshop.Experiences;
using Xunit;

namespace SingularityHub.Tests;

public sealed class IncrementalVersion069Tests
{
    [ArchitectureTest(0, 0, 69)]
    [Fact(DisplayName = "0.00.069 — Pong_Is_A_Concrete_Workshop_Experience")]
    public void PongIsAConcreteWorkshopExperience()
    {
        IExperience experience = new PongExperience();
        Assert.Equal("0.0.69", "0.0.69");
        Assert.Equal(3001UL, experience.Id);
        Assert.Equal("PONG", experience.Name);
        Assert.Equal(new ulong[] { 2001 }, experience.MicroBundleIds);
    }
}
