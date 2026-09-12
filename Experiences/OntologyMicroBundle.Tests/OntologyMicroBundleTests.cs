using TheSingularityWorkshop.Workshop.Experiences;
using Xunit;

namespace TheSingularityWorkshop.Experiences.Ontology.Tests;

public sealed class OntologyMicroBundleTests
{
    [Fact(DisplayName = "Ontology_Library_Contains_A_Broad_Initial_Inventory")]
    public void Ontology_Library_Contains_A_Broad_Initial_Inventory()
        => Assert.True(OntologyMicroBundleCatalog.All.Count >= 45);

    [Fact(DisplayName = "Ontology_Library_Has_Stable_Unique_Integer_Identities")]
    public void Ontology_Library_Has_Stable_Unique_Integer_Identities()
    {
        var bundles = OntologyMicroBundleCatalog.All;
        Assert.Equal(bundles.Count, bundles.Select(x => x.Id).Distinct().Count());
        Assert.All(bundles, bundle => Assert.NotEqual(0UL, bundle.Id));
    }

    [Fact(DisplayName = "Ontology_Library_Uses_Nine_Layer_Signatures")]
    public void Ontology_Library_Uses_Nine_Layer_Signatures()
        => Assert.All(OntologyMicroBundleCatalog.All,
            bundle => Assert.Equal(9, bundle.Ontology.LayerCount));

    [Fact(DisplayName = "Ontology_Library_Separates_Software_Logic_And_Physics")]
    public void Ontology_Library_Separates_Software_Logic_And_Physics()
    {
        var bundles = OntologyMicroBundleCatalog.All;
        Assert.Equal(10, bundles.Count(x => x.Ontology[0] == OntologyFamilies.SoftwareAbstractions));
        Assert.Equal(10, bundles.Count(x => x.Ontology[0] == OntologyFamilies.DigitalLogic));
        Assert.Equal(25, bundles.Count(x => x.Ontology[0] == OntologyFamilies.Physics));
    }

    [Theory(DisplayName = "Physics_Family_Is_Addressable_By_Integer_Ontology")]
    [InlineData(OntologyFamilies.Newtonian, 5)]
    [InlineData(OntologyFamilies.ElectricityAndMagnetism, 5)]
    [InlineData(OntologyFamilies.Thermodynamics, 5)]
    [InlineData(OntologyFamilies.Hydrodynamics, 5)]
    [InlineData(OntologyFamilies.Plasmadynamics, 5)]
    public void Physics_Family_Is_Addressable_By_Integer_Ontology(int family, int expected)
    {
        var matches = OntologyMicroBundleCatalog.All
            .Where(bundle => bundle.Ontology[0] == OntologyFamilies.Physics && bundle.Ontology[1] == family)
            .ToArray();
        Assert.Equal(expected, matches.Length);
    }

    [Fact(DisplayName = "Physics_Subtopics_Have_Distinct_Final_Ontology_Tokens")]
    public void Physics_Subtopics_Have_Distinct_Final_Ontology_Tokens()
    {
        var physics = OntologyMicroBundleCatalog.All.Where(x => x.Ontology[0] == OntologyFamilies.Physics).ToArray();
        Assert.Equal(physics.Length, physics.Select(x => x.Ontology[8]).Distinct().Count());
    }

    [Fact(DisplayName = "Registry_Can_Index_The_Whole_Library_By_Ontology_Layer")]
    public void Registry_Can_Index_The_Whole_Library_By_Ontology_Layer()
    {
        var registry = new MicroBundleRegistry();
        foreach (var bundle in OntologyMicroBundleCatalog.All)
            registry.Register(bundle);

        var physics = registry.FindByOntologyLayer(0, OntologyFamilies.Physics);
        Assert.Equal(25, physics.Count);
        Assert.Contains(physics, x => x.Name == "Newtonian: Gravity");
        Assert.Contains(physics, x => x.Name == "Plasmadynamics: Magnetohydrodynamics");
    }

    [Fact(DisplayName = "Registry_Rejects_Duplicate_Integer_MicroBundle_Identity")]
    public void Registry_Rejects_Duplicate_Integer_MicroBundle_Identity()
    {
        var registry = new MicroBundleRegistry();
        var bundle = OntologyMicroBundleCatalog.All[0];
        registry.Register(bundle);
        Assert.Throws<InvalidOperationException>(() => registry.Register(bundle));
    }

    [Fact(DisplayName = "Registry_Rejects_Invalid_Ontology_Layer_Query")]
    public void Registry_Rejects_Invalid_Ontology_Layer_Query()
    {
        var registry = new MicroBundleRegistry();
        Assert.Throws<ArgumentOutOfRangeException>(() => registry.FindByOntologyLayer(-1, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => registry.FindByOntologyLayer(9, 1));
    }

    [Fact(DisplayName = "Every_Library_Bundle_Is_Arbitration_Ready")]
    public void Every_Library_Bundle_Is_Arbitration_Ready()
    {
        var arbitrator = new TestArbitrator();
        Assert.All(OntologyMicroBundleCatalog.All, bundle => Assert.False(bundle.Arbitrate(arbitrator, 0)));
    }

    private sealed class TestArbitrator : IArbitrator
    {
        public IReadOnlyCollection<IMicroBundle> LoadedBundles { get; } = Array.Empty<IMicroBundle>();
        public bool LoadBundle(IMicroBundle bundle) => true;
    }
}
