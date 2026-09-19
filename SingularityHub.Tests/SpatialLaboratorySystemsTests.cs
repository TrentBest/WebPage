using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialLaboratorySystemsTests
{
    [Fact]
    public void Lab_ContainsColliderRoboticsRestrictedAiAndLanguageSystems()
    {
        var lab = SpatialLaboratoryManifest.CreateDefault();

        Assert.Contains(lab.Colliders.Configurations, x => x.Id == "hadron");
        Assert.Contains(lab.Robotics.Robots, x => x.Id == "survey-drone");
        Assert.Contains(lab.Robotics.Robots, x => x.Id == "lab-robot");
        Assert.True(lab.AiSecurity.IsRestricted(-2));
        Assert.True(lab.AiSecurity.IsRestricted(10));
        Assert.False(lab.AiSecurity.IsRestricted(1));
        Assert.NotNull(lab.Grammars.Find("tlh"));
        Assert.NotNull(lab.Grammars.Find("http"));
        Assert.NotNull(lab.Vocalization.FindLanguage("en"));
        Assert.NotNull(lab.Vocalization.FindLanguage("tlh"));
    }
}