using System.Collections.Generic;
using System.Linq;
using HubArbitrator = TheSingularityWorkshop.SingularityHub.IArbitrator;
using HubBundle = TheSingularityWorkshop.SingularityHub.IMicroBundle;
using TheSingularityWorkshop.SingularityHub;
using TheSingularityWorkshop.Workshop.Chemistry;
using TheSingularityWorkshop.Workshop.MicroBundles;
using Xunit;

namespace SingularityHub.Tests;

public sealed class ChemistryMicroBundleTests
{
    [Fact]
    public void AtomBuilder_Produces_Validated_Elemental_Data()
    {
        var carbon = ElementalCatalog.Core["C"];

        Assert.Equal("Carbon", carbon.Name);
        Assert.Equal("C", carbon.Symbol);
        Assert.Equal(6, carbon.AtomicNumber);
        Assert.Equal(6, carbon.Protons);
        Assert.Equal(6, carbon.Neutrons);
        Assert.Contains("2p2", carbon.ElectronConfiguration);
    }

    [Fact]
    public void ElementalCatalog_Contains_All_118_Named_Elements_And_Provides_Stable_Lookups()
    {
        Assert.Equal(118, ElementalCatalog.Count);
        Assert.Equal(118, ElementalCatalog.Core.Count);
        Assert.Equal(118, ElementalCatalog.ByAtomicNumber.Count);
        Assert.Equal("H", ElementalCatalog.GetByAtomicNumber(1).Symbol);
        Assert.Equal("Fe", ElementalCatalog.GetByAtomicNumber(26).Symbol);
        Assert.Equal("U", ElementalCatalog.GetByAtomicNumber(92).Symbol);
        Assert.Equal("Og", ElementalCatalog.GetByAtomicNumber(118).Symbol);
        Assert.Equal(26, ElementalCatalog.GetBySymbol("Fe").AtomicNumber);
    }

    [Fact]
    public void ElementalCatalog_Supports_Fictional_Elements_Through_The_Same_Domain_Model()
    {
        var unobtanium = ElementalCatalog.Fictional["Ub"];

        Assert.True(unobtanium.IsFictional);
        Assert.Equal(ElementOrigin.Fictional, unobtanium.Origin);
        Assert.Equal("Unobtanium", unobtanium.Name);
        Assert.Equal("Ub", unobtanium.Symbol);
        Assert.Equal("Fictional", unobtanium.Category);
    }

    [Fact]
    public void ChemistryArbitrator_Resolves_Material_By_Application_And_Elements()
    {
        var iron = ElementalCatalog.Core["Fe"];
        var carbon = ElementalCatalog.Core["C"];
        var material = new MaterialComposition(
            "armor-leather",
            "Leather Armor",
            MaterialApplication.Armor,
            new ElementalFraction(carbon, 0.80),
            new ElementalFraction(iron, 0.05));

        var source = new MaterialSourceBundle(material);
        var results = new ElementalArbitrator().Evaluate(new HubBundle[] { source });
        var result = Assert.Single(results);

        Assert.Equal(MaterialApplication.Armor, result.Application);
        Assert.Contains(result.Elements, x => x.Symbol == "C");
        Assert.True(result.Physics.Hardness > 0.5);
        Assert.True(result.Physics.Flexibility > 0.0);
    }

    [Fact]
    public void ChemistryMicroBundle_Arbitrates_Installed_Elemental_Content_Once()
    {
        using var chemistry = new ChemistryMicroBundle();
        var source = new MaterialSourceBundle(new MaterialComposition(
            "weapon-frame",
            "Weapon Frame",
            MaterialApplication.Structural,
            new ElementalFraction(ElementalCatalog.Core["Fe"], 0.90)));
        var arbitrator = new TestArbitrator(source);

        Assert.True(chemistry.Arbitrate(arbitrator, 0));
        Assert.False(chemistry.Arbitrate(arbitrator, 1));
        Assert.Single(chemistry.ResolvedMaterials);
        Assert.Equal("weapon-frame", chemistry.ResolvedMaterials.Single().MaterialId);
    }

    [Fact]
    public void ChemistryMicroBundle_Implements_The_Workshop_And_Hub_Contracts()
    {
        using var chemistry = new ChemistryMicroBundle();

        Assert.IsAssignableFrom<TheSingularityWorkshop.Workshop.MicroBundles.IMicroBundle>(chemistry);
        Assert.IsAssignableFrom<HubBundle>(chemistry);
    }

    private sealed class MaterialSourceBundle : HubBundle, IElementalMaterialSource
    {
        public MaterialSourceBundle(MaterialComposition material) => Materials = [material];
        public ulong Id => 99001;
        public OntologySignature Ontology => new(0, 0, 0, 0, 0, 0, 0, 0, 99001);
        public BundleVersion Version => new(1, 0, 0);
        public IReadOnlyList<ulong> Dependencies => [];
        public IReadOnlyList<MaterialComposition> Materials { get; }
        public bool Arbitrate(HubArbitrator arbitrator, int roundIndex) => false;
    }

    private sealed class TestArbitrator : HubArbitrator
    {
        public TestArbitrator(params HubBundle[] bundles) => LoadedBundles = bundles;
        public IReadOnlyCollection<HubBundle> LoadedBundles { get; }
        public bool LoadBundle(HubBundle bundle) => false;
        public int ExecuteArbitrationPipeline() => 0;
    }
}
