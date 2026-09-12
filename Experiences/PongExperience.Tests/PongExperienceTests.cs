using TheSingularityWorkshop.Workshop.Experiences;
using Xunit;

namespace PongExperience.Tests;

/// <summary>Executable contract for the first concrete Workshop Experience.</summary>
public sealed class PongExperienceTests
{
    [Fact(DisplayName = "Core_Pong_Experience_Is_Solution_Native")]
    public void Core_Pong_Experience_Is_Solution_Native()
    {
        var experience = new PongExperience();

        Assert.Equal(PongExperience.ExperienceId, experience.Id);
        Assert.Equal("PONG", experience.Name);
        Assert.Equal(new(1, 0, 0), experience.Version);
    }

    [Fact(DisplayName = "Pong_Experience_Provides_Visual_And_Sound")]
    public void Pong_Experience_Provides_Visual_And_Sound()
    {
        var experience = new PongExperience();

        Assert.Contains(PongCapabilityIds.Visual, experience.Capabilities);
        Assert.Contains(PongCapabilityIds.Sound, experience.Capabilities);
        Assert.Contains(PongSenseIds.Sight, experience.SensorySystems);
        Assert.Contains(PongSenseIds.Hearing, experience.SensorySystems);
        Assert.Equal(2, experience.SenseCount);
    }

    [Fact(DisplayName = "Pong_Experience_Accepts_Up_Or_Down_Input")]
    public void Pong_Experience_Accepts_Up_Or_Down_Input()
    {
        var input = Assert.Single(PongMicroBundleCatalog.All, x => x.Id == PongMicroBundleIds.InputBehavior);

        Assert.Equal("Up Down Input", input.Name);
        Assert.Equal(PongMicroBundleKind.Behavior, input.Kind);
        Assert.Contains(PongCapabilityIds.Input, new PongExperience().Capabilities);
    }

    [Fact(DisplayName = "Pong_Experience_Composes_Assets_And_Behaviors")]
    public void Pong_Experience_Composes_Assets_And_Behaviors()
    {
        var experience = new PongExperience();
        var definitions = PongMicroBundleCatalog.All;

        Assert.Equal(definitions.Count, experience.MicroBundleIds.Count);
        Assert.All(definitions, bundle => Assert.Contains(bundle.Id, experience.MicroBundleIds));
        Assert.Contains(definitions, x => x.Kind == PongMicroBundleKind.Asset);
        Assert.Contains(definitions, x => x.Kind == PongMicroBundleKind.Presentation);
        Assert.Contains(definitions, x => x.Kind == PongMicroBundleKind.Behavior);
    }

    [Fact(DisplayName = "Pong_Experience_Visual_And_Sound_Depend_On_Assets")]
    public void Pong_Experience_Visual_And_Sound_Depend_On_Assets()
    {
        var visual = Assert.Single(PongMicroBundleCatalog.All, x => x.Id == PongMicroBundleIds.VisualPresentation);
        var sound = Assert.Single(PongMicroBundleCatalog.All, x => x.Id == PongMicroBundleIds.SoundPresentation);

        Assert.Contains(PongMicroBundleIds.ArenaAsset, visual.Dependencies);
        Assert.Contains(PongMicroBundleIds.PaddleAsset, visual.Dependencies);
        Assert.Contains(PongMicroBundleIds.BallAsset, visual.Dependencies);
        Assert.Contains(PongMicroBundleIds.ScoreAsset, visual.Dependencies);
        Assert.Contains(PongMicroBundleIds.BallAsset, sound.Dependencies);
    }

    [Fact(DisplayName = "Pong_Experience_Behavior_Dependency_Chain_Is_Declared")]
    public void Pong_Experience_Behavior_Dependency_Chain_Is_Declared()
    {
        var physics = Assert.Single(PongMicroBundleCatalog.All, x => x.Id == PongMicroBundleIds.BallPhysicsBehavior);
        var score = Assert.Single(PongMicroBundleCatalog.All, x => x.Id == PongMicroBundleIds.ScoreBehavior);

        Assert.Contains(PongMicroBundleIds.BallAsset, physics.Dependencies);
        Assert.Contains(PongMicroBundleIds.ArenaAsset, physics.Dependencies);
        Assert.Contains(PongMicroBundleIds.BallPhysicsBehavior, score.Dependencies);
        Assert.Contains(PongMicroBundleIds.ScoreAsset, score.Dependencies);
    }

    [Fact(DisplayName = "Pong_Experience_Exposes_Hub_Process_Groups")]
    public void Pong_Experience_Exposes_Hub_Process_Groups()
    {
        var groups = new PongExperience().ProcessingGroups;

        Assert.Equal(3, groups.Count);
        Assert.Contains("Experience.Pong.Input", groups);
        Assert.Contains("Experience.Pong.Physics", groups);
        Assert.Contains("Experience.Pong.Presentation", groups);
    }
}
