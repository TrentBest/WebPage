using System.Linq;
using TheSingularityWorkshop.Gui;
using TheSingularityWorkshop.Workshop.MicroBundles;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Heartbeat tests for the laboratory, physics, and education foundations.</summary>
public sealed class LaboratoryEducationalFoundationTests
{
    [Fact]
    public void TbdPhysicsEngine_Exposes_FromMicro_ToUniversalScales()
    {
        var engine = TbdPhysicsEngineManifest.Default;

        Assert.Equal("TBD PHYSICS ENGINE", engine.Name);
        Assert.Equal("MICRO", engine.Scales.First().Name);
        Assert.Equal("UNIVERSAL", engine.Scales.Last().Name);
        Assert.Contains("structural behavior", engine.Capabilities);
        Assert.Contains("multi-scale visualization", engine.Capabilities);
    }

    [Fact]
    public void LaboratoryWorkspace_IsHumanScaleManifestation_OfExpandableMindSpace()
    {
        var workspace = LaboratoryResearchWorkspace.CreateFor("researcher-1", "Researcher");

        Assert.Equal("researcher-1-lab", workspace.Id);
        Assert.Empty(workspace.Instruments);
        Assert.Empty(workspace.Records);
        Assert.Empty(workspace.Visualizations);
    }

    [Fact]
    public void EducationalProvider_IsFreeAndCapturesReusableQuestions()
    {
        using var provider = new EducationalProviderMicroBundle();

        provider.AddLesson(new EducationalLesson(
            "lesson-1",
            "Introduction to State Machines",
            "Computer Science",
            "teacher-1",
            ["state", "transition"]));

        provider.CaptureQuestionAnswer(new EducationalQuestionAnswer(
            "What is a state machine?",
            "A model of states and transitions.",
            "teacher-1",
            "singularity-course",
            ["state", "transition"]));

        Assert.Single(provider.Lessons);
        Assert.Single(provider.FindExactQuestionMatches("  What is a state machine? "));
    }
}
