using TheSingularityWorkshop.SingularityHub;
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
        Assert.Contains(PongMicroBundleIds.InputBehavior, new PongExperience().MicroBundleIds);
    }

    [Fact(DisplayName = "Pong_Experience_Composes_Assets_Presentation_And_Behavior")]
    public void Pong_Experience_Composes_Assets_Presentation_And_Behavior()
    {
        var experience = new PongExperience();
        var bundles = PongMicroBundleCatalog.All;
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

    [Fact(DisplayName = "Pong_Experience_Is_Explicitly_Admitted_To_Idle")]
    public void Pong_Experience_Is_Explicitly_Admitted_To_Idle()
        => Assert.IsAssignableFrom<IIdleExperience>(new PongExperience());

    [Fact(DisplayName = "Pong_Experience_Bundles_Inherit_Experience_Ontology")]
    public void Pong_Experience_Bundles_Inherit_Experience_Ontology()
    {
        var experience = new PongExperience();
        foreach (var bundle in PongMicroBundleCatalog.All)
            for (var layer = 0; layer < 8; layer++)
                Assert.Equal(experience.Ontology[layer], bundle.Ontology[layer]);
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
        Assert.Contains(sounds, sound => sound is PongSoundEvent.Wall or PongSoundEvent.Paddle or PongSoundEvent.Score);
    }

    [Fact(DisplayName = "Pong_Experience_Loads_From_Integer_Identity_And_Retains_All_Bundles")]
    public void Pong_Experience_Loads_From_Integer_Identity_And_Retains_All_Bundles()
    {
        var experiences = new ExperienceRegistry();
        var bundles = new TheSingularityWorkshop.Workshop.Experiences.MicroBundleRegistry();
        experiences.Register(new PongExperience());
        foreach (var bundle in PongMicroBundleCatalog.All)
            bundles.Register(bundle);

        var host = new ExperienceRuntimeHost(new ExperienceLoader(experiences, bundles));
        var loaded = host.Activate(PongExperience.ExperienceId, requireIdle: true);

        Assert.True(loaded.IsReady);
        Assert.Equal(PongMicroBundleCatalog.All.Count, loaded.Bundles.Count);
        Assert.Equal(PongMicroBundleCatalog.All.Count, loaded.DependencyOrder.Count);
        Assert.True(host.IsActive(PongExperience.ExperienceId));
    }

    [Fact(DisplayName = "Pong_Experience_Resolves_Deep_Dependencies_Before_Consumers")]
    public void Pong_Experience_Resolves_Deep_Dependencies_Before_Consumers()
    {
        var experiences = new ExperienceRegistry();
        var bundles = new TheSingularityWorkshop.Workshop.Experiences.MicroBundleRegistry();
        experiences.Register(new PongExperience());
        foreach (var bundle in PongMicroBundleCatalog.All)
            bundles.Register(bundle);

        var result = new ExperienceLoader(experiences, bundles).Load(PongExperience.ExperienceId);
        var order = result.DependencyOrder.ToList();

        Assert.True(result.IsLoadable);
        Assert.True(order.IndexOf(PongMicroBundleIds.BallPhysicsBehavior) < order.IndexOf(PongMicroBundleIds.ScoreBehavior));
        Assert.True(order.IndexOf(PongMicroBundleIds.ScoreAsset) < order.IndexOf(PongMicroBundleIds.ScoreBehavior));
    }

    [Fact(DisplayName = "Pong_MicroBundles_Are_Queryable_Through_The_Ontology_Index")]
    public void Pong_MicroBundles_Are_Queryable_Through_The_Ontology_Index()
    {
        var registry = new TheSingularityWorkshop.Workshop.Experiences.MicroBundleRegistry();
        foreach (var bundle in PongMicroBundleCatalog.All)
            registry.Register(bundle);
        var matches = registry.FindByOntologyLayer(0, 7);
        Assert.Equal(PongMicroBundleCatalog.All.Count, matches.Count);
        Assert.Contains(matches, bundle => bundle.Id == PongMicroBundleIds.BallAsset);
    }

    [Fact(DisplayName = "Pong_Experience_Fails_Load_When_A_Required_Bundle_Is_Missing")]
    public void Pong_Experience_Fails_Load_When_A_Required_Bundle_Is_Missing()
    {
        var experiences = new ExperienceRegistry();
        var bundles = new TheSingularityWorkshop.Workshop.Experiences.MicroBundleRegistry();
        experiences.Register(new PongExperience());
        foreach (var bundle in PongMicroBundleCatalog.All.Where(bundle => bundle.Id != PongMicroBundleIds.BallAsset))
            bundles.Register(bundle);

        var result = new ExperienceLoader(experiences, bundles).Load(PongExperience.ExperienceId);

        Assert.False(result.IsLoadable);
        Assert.Contains(result.Errors, error => error.Contains(PongMicroBundleIds.BallAsset.ToString()));
    }
}
