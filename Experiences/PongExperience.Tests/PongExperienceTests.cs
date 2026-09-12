using TheSingularityWorkshop.Workshop.Experiences;
using Xunit;

namespace TheSingularityWorkshop.Experiences.Pong.Tests;

public sealed class PongExperienceTests
{
    [Fact(DisplayName = "Pong_Experience_Provides_Stable_Identity_And_Version")]
    public void Pong_Experience_Provides_Stable_Identity_And_Version()
    {
        var experience = new PongExperience();

        Assert.Equal(PongExperience.ExperienceId, experience.Id);
        Assert.Equal("PONG", experience.Name);
        Assert.Equal(new BundleVersion(1, 0, 0), experience.Version);
    }

    [Fact(DisplayName = "Pong_Experience_Provides_Visual_Sound_And_Sensory_Systems")]
    public void Pong_Experience_Provides_Visual_Sound_And_Sensory_Systems()
    {
        var experience = new PongExperience();

        Assert.Contains(PongCapabilityIds.Visual, experience.Capabilities);
        Assert.Contains(PongCapabilityIds.Sound, experience.Capabilities);
        Assert.Contains(PongSenseIds.Sight, experience.SensorySystems);
        Assert.Contains(PongSenseIds.Hearing, experience.SensorySystems);
        Assert.Equal(2, experience.SenseCount);
    }

    [Fact(DisplayName = "Pong_Experience_Contains_Input_Bundle")]
    public void Pong_Experience_Contains_Input_Bundle()
    {
        var experience = new PongExperience();

        Assert.Contains(PongMicroBundleIds.InputBehavior, experience.MicroBundleIds);
    }

    [Fact(DisplayName = "Pong_Experience_Composes_Assets_Presentation_And_Behavior")]
    public void Pong_Experience_Composes_Assets_Presentation_And_Behavior()
    {
        var experience = new PongExperience();
        var bundles = PongExperience.MicroBundles;

        Assert.Equal(experience.MicroBundleIds.Count, bundles.Count);
        Assert.Contains(bundles, bundle => bundle.Kind == PongMicroBundleKind.Asset);
        Assert.Contains(bundles, bundle => bundle.Kind == PongMicroBundleKind.Presentation);
        Assert.Contains(bundles, bundle => bundle.Kind == PongMicroBundleKind.Behavior);
    }

    [Fact(DisplayName = "Pong_Presentation_Bundles_Declare_Asset_Dependencies")]
    public void Pong_Presentation_Bundles_Declare_Asset_Dependencies()
    {
        var visual = PongExperience.MicroBundles.Single(bundle => bundle.Id == PongMicroBundleIds.VisualPresentation);
        var sound = PongExperience.MicroBundles.Single(bundle => bundle.Id == PongMicroBundleIds.SoundPresentation);

        Assert.Contains(PongMicroBundleIds.ArenaAsset, visual.Dependencies);
        Assert.Contains(PongMicroBundleIds.PaddleAsset, visual.Dependencies);
        Assert.Contains(PongMicroBundleIds.BallAsset, visual.Dependencies);
        Assert.Contains(PongMicroBundleIds.ScoreAsset, visual.Dependencies);
        Assert.Contains(PongMicroBundleIds.BallAsset, sound.Dependencies);
    }

    [Fact(DisplayName = "Pong_Behavior_Bundles_Declare_Execution_Dependencies")]
    public void Pong_Behavior_Bundles_Declare_Execution_Dependencies()
    {
        var physics = PongExperience.MicroBundles.Single(bundle => bundle.Id == PongMicroBundleIds.BallPhysicsBehavior);
        var scoring = PongExperience.MicroBundles.Single(bundle => bundle.Id == PongMicroBundleIds.ScoreBehavior);

        Assert.Contains(PongMicroBundleIds.BallAsset, physics.Dependencies);
        Assert.Contains(PongMicroBundleIds.ArenaAsset, physics.Dependencies);
        Assert.Contains(PongMicroBundleIds.ScoreAsset, scoring.Dependencies);
        Assert.Contains(PongMicroBundleIds.BallPhysicsBehavior, scoring.Dependencies);
    }

    [Fact(DisplayName = "Pong_Experience_Exposes_Three_Hub_Process_Groups")]
    public void Pong_Experience_Exposes_Three_Hub_Process_Groups()
    {
        var experience = new PongExperience();

        Assert.Equal(3, experience.ProcessingGroups.Count);
        Assert.Contains("Experience.Pong.Input", experience.ProcessingGroups);
        Assert.Contains("Experience.Pong.Physics", experience.ProcessingGroups);
        Assert.Contains("Experience.Pong.Presentation", experience.ProcessingGroups);
    }

    [Fact(DisplayName = "Pong_Runtime_Executes_Physics")]
    public void Pong_Runtime_Executes_Physics()
    {
        var runtime = new PongExperienceRuntime(seed: 7);
        var initialX = runtime.BallX;
        var initialY = runtime.BallY;

        runtime.Step();

        Assert.NotEqual(initialX, runtime.BallX);
        Assert.NotEqual(initialY, runtime.BallY);
    }

    [Fact(DisplayName = "Pong_Runtime_Accepts_Up_And_Down_Input")]
    public void Pong_Runtime_Accepts_Up_And_Down_Input()
    {
        var runtime = new PongExperienceRuntime(seed: 7);
        runtime.MovePlayer(-10);
        var upPosition = runtime.PlayerPaddle;
        runtime.MovePlayer(20);
        var downPosition = runtime.PlayerPaddle;

        Assert.True(upPosition < 50);
        Assert.True(downPosition > upPosition);
    }

    [Fact(DisplayName = "Pong_Runtime_Emits_Sound_For_Collisions")]
    public void Pong_Runtime_Emits_Sound_For_Collisions()
    {
        var runtime = new PongExperienceRuntime(seed: 7);
        var sounds = new List<PongSoundEvent>();
        runtime.SoundTriggered += sounds.Add;

        while (sounds.Count == 0)
            runtime.Step();

        Assert.NotEmpty(sounds);
        Assert.Contains(sounds, sound => sound is PongSoundEvent.Wall or PongSoundEvent.Paddle or PongSoundEvent.Score);
    }
}
