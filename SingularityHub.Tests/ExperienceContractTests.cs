using TheSingularityWorkshop.SingularityHub;
using TheSingularityWorkshop.Workshop.Experiences;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Layer 4 tests for the portable Workshop experience contract.</summary>
public sealed class ExperienceContractTests
{
    [ArchitectureTest(4, 3, 1)]
    [Fact(DisplayName = "4.03.001 — Experience_Exposes_Stable_Identity_And_Version")]
    public void Experience_Exposes_Stable_Identity_And_Version()
    {
        IExperience experience = new TestExperience();

        Assert.Equal(42UL, experience.Id);
        Assert.Equal("Test Experience", experience.Name);
        Assert.Equal(new BundleVersion(1, 2, 3), experience.Version);
    }

    [ArchitectureTest(4, 3, 2)]
    [Fact(DisplayName = "4.03.002 — Experience_Exposes_Ontology")]
    public void Experience_Exposes_Ontology()
    {
        IExperience experience = new TestExperience();
        var ontology = experience.Ontology;

        Assert.Equal(9, OntologySignature.LayerCount);
        Assert.Equal(7, ontology[0]);
        Assert.Equal(42, ontology[8]);
    }

    [ArchitectureTest(4, 3, 3)]
    [Fact(DisplayName = "4.03.003 — Experience_References_MicroBundles_By_Identity")]
    public void Experience_References_MicroBundles_By_Identity()
    {
        IExperience experience = new TestExperience();

        Assert.Equal(new ulong[] { 1001, 1002 }, experience.MicroBundleIds);
        Assert.All(experience.MicroBundleIds, id => Assert.NotEqual(0UL, id));
    }

    [ArchitectureTest(4, 3, 4)]
    [Fact(DisplayName = "4.03.004 — Experience_Exposes_Integer_Capabilities")]
    public void Experience_Exposes_Integer_Capabilities()
    {
        IExperience experience = new TestExperience();

        Assert.Equal(new ulong[] { 7, 19 }, experience.Capabilities);
    }

    [ArchitectureTest(4, 3, 5)]
    [Fact(DisplayName = "4.03.005 — Experience_Is_Quantified_By_Distinct_Sensory_Systems")]
    public void Experience_Is_Quantified_By_Distinct_Sensory_Systems()
    {
        IExperience experience = new TestExperience();

        Assert.Equal(new ulong[] { 1, 2, 2, 3 }, experience.SensorySystems);
        Assert.Equal(3, experience.SenseCount);
    }

    [ArchitectureTest(4, 3, 6)]
    [Fact(DisplayName = "4.03.006 — Experience_Exposes_Process_Groups_For_Hub_Stepping")]
    public void Experience_Exposes_Process_Groups_For_Hub_Stepping()
    {
        IExperience experience = new TestExperience();

        Assert.Equal(new[] { "Experience.Page", "Experience.Living" }, experience.ProcessingGroups);
    }

    private sealed class TestExperience : IExperience
    {
        public ulong Id => 42;
        public string Name => "Test Experience";
        public BundleVersion Version => new(1, 2, 3);
        public OntologySignature Ontology => new(7, 1, 2, 3, 4, 5, 6, 8, 42);
        public IReadOnlyList<ulong> MicroBundleIds { get; } = new ulong[] { 1001, 1002 };
        public IReadOnlyList<ulong> Capabilities { get; } = new ulong[] { 7, 19 };
        public IReadOnlyList<ulong> SensorySystems { get; } = new ulong[] { 1, 2, 2, 3 };
        public IReadOnlyList<string> ProcessingGroups { get; } = new[] { "Experience.Page", "Experience.Living" };
    }
}
