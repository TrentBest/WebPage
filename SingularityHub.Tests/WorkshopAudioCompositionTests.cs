using TheSingularityWorkshop.Workshop.Creation.Audio;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Verifies the first platform-neutral audio composition boundary.</summary>
public sealed class WorkshopAudioCompositionTests
{
    [Fact(DisplayName = "V0.0.164 — Audio_Score_Persists_Semantic_Notes")]
    public void AudioScorePersistsSemanticNotes()
    {
        var score = new AudioComposition(
            "Fairy Arrival",
            [
                new AudioTrack(
                    "Strings",
                    "Violin",
                    [
                        new AudioNote("C", 0, 1),
                        new AudioNote("E", 1, 1),
                        new AudioNote("G", 2, 2)
                    ])
            ],
            [
                new SoundEffectCue("Arrival Sparkle", "sparkle.generated", 2.0, 1.5)
            ]);

        Assert.Equal("Fairy Arrival", score.Name);
        Assert.Single(score.Tracks);
        Assert.Equal(3, score.Tracks[0].Notes.Count);
        Assert.Equal("G", score.Tracks[0].Notes[2].Pitch);
        Assert.Equal("sparkle.generated", score.SoundEffects[0].Source);
    }

    [Fact(DisplayName = "V0.0.165 — SFX_Recipe_Composes_Layers")]
    public void SfxRecipeComposesLayers()
    {
        var recipe = new SoundEffectRecipe(
            "Fairy Arrival",
            [
                new SoundSourceLayer("bell.recording"),
                new SoundSourceLayer("shimmer.generated", 0.25, 0.8),
                new SoundSourceLayer("soft.impact", 1.1)
            ]);

        Assert.Equal("Fairy Arrival", recipe.Name);
        Assert.Equal(3, recipe.Layers.Count);
        Assert.Equal("shimmer.generated", recipe.Layers[1].Source);
        Assert.Equal(0.25, recipe.Layers[1].OffsetSeconds);
    }
}
