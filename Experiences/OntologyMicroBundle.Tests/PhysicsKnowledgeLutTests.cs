using TheSingularityWorkshop.SingularityHub;
using TheSingularityWorkshop.Workshop.Experiences;
using Xunit;

namespace TheSingularityWorkshop.Experiences.Ontology.Tests;

public sealed class PhysicsKnowledgeLutTests
{
    [Fact(DisplayName = "Physics_Knowledge_LUT_Provides_Graduated_Detail")]
    public void Physics_Knowledge_LUT_Provides_Graduated_Detail()
    {
        var levels = PhysicsKnowledgeLut.Entries
            .Where(entry => entry.MicroBundleId == 0x33000002)
            .Select(entry => entry.Level)
            .Distinct()
            .OrderBy(level => level)
            .ToArray();

        Assert.Equal(
            new[]
            {
                KnowledgeDetailLevel.Essential,
                KnowledgeDetailLevel.Intermediate,
                KnowledgeDetailLevel.Advanced,
                KnowledgeDetailLevel.Expert
            },
            levels);
    }

    [Fact(DisplayName = "Physics_Knowledge_LUT_Is_Mathematical_And_Factual")]
    public void Physics_Knowledge_LUT_Is_Mathematical_And_Factual()
    {
        var heat = PhysicsKnowledgeLut.Entries.Single(entry =>
            entry.MicroBundleId == 0x33000002 && entry.Level == KnowledgeDetailLevel.Intermediate);

        Assert.Contains(heat.Facts, fact => fact.Contains("delta U = Q - W", StringComparison.Ordinal));
        Assert.Contains(PhysicsKnowledgeLut.Entries, entry =>
            entry.MicroBundleId == 0x36000002 &&
            entry.Level == KnowledgeDetailLevel.Advanced &&
            entry.Facts.Any(fact => fact.Contains("k-effective", StringComparison.Ordinal)));
    }

    [Fact(DisplayName = "Physics_Knowledge_LUT_Uses_Stable_Integer_Addresses")]
    public void Physics_Knowledge_LUT_Uses_Stable_Integer_Addresses()
    {
        Assert.All(PhysicsKnowledgeLut.Entries, entry => Assert.NotEqual(0UL, entry.MicroBundleId));
        Assert.Equal(
            PhysicsKnowledgeLut.Entries.Select(entry => (entry.MicroBundleId, entry.Level)).Distinct().Count(),
            PhysicsKnowledgeLut.Entries.Count);
    }

    [Fact(DisplayName = "Physics_Knowledge_Is_Carried_By_Independently_Addressable_MicroBundles")]
    public void Physics_Knowledge_Is_Carried_By_Independently_Addressable_MicroBundles()
    {
        Assert.Equal(10, PhysicsKnowledgeMicroBundleCatalog.All.Count);
        Assert.All(PhysicsKnowledgeMicroBundleCatalog.All,
            bundle => Assert.IsType<PhysicsKnowledgeMicroBundle>(bundle));
        Assert.All(PhysicsKnowledgeMicroBundleCatalog.All,
            bundle => Assert.Equal(4, ((PhysicsKnowledgeMicroBundle)bundle).Knowledge.Count));
        Assert.All(PhysicsKnowledgeMicroBundleCatalog.All,
            bundle => Assert.Equal(9, bundle.Ontology[0]));
    }
}
