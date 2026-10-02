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
    public void AtomBuilder_Preserves_Structured_Physical_And_Provenance_Properties()
    {
        var properties = new ElementalPropertySet
        {
            DensityKgPerM3 = 7850,
            YoungsModulusPa = 200_000_000_000,
            FractureToughnessPaSqrtM = 50_000_000,
            SourceId = "test-dataset",
            SourceVersion = "1.0",
            Uncertainty = "illustrative test data"
        };

        var iron = new AtomBuilder("Iron", 26, "Fe")
            .WithProperties(properties)
            .Build();

        Assert.Same(properties, iron.Properties);
        Assert.Equal(7850, iron.Properties!.DensityKgPerM3);
        Assert.Equal(200_000_000_000, iron.Properties.YoungsModulusPa);
        Assert.Equal(50_000_000, iron.Properties.FractureToughnessPaSqrtM);
        Assert.Equal("test-dataset", iron.Properties.SourceId);
        Assert.Equal("1.0", iron.Properties.SourceVersion);
        Assert.Equal("illustrative test data", iron.Properties.Uncertainty);
    }

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
    public void ChemistryMicroBundle_Uses_Injected_Elemental_Property_Data_Source()
    {
        var ironProperties = new ElementalPropertySet
        {
            DensityKgPerM3 = 7874,
            YoungsModulusPa = 200_000_000_000,
            SourceId = "warehouse:test",
            SourceVersion = "2026.10"
        };

        var source = new ElementalCatalogDataSource(
            new Dictionary<string, ElementalPropertySet>
            {
                ["Fe"] = ironProperties
            });

        using var chemistry = new ChemistryMicroBundle(source);

        Assert.True(chemistry.TryGetProperties("Fe", out var resolved));
        Assert.Same(ironProperties, resolved);
        Assert.Equal("warehouse:test", resolved!.SourceId);
        Assert.Equal("2026.10", resolved.SourceVersion);

        Assert.False(chemistry.TryGetProperties("Og", out var missing));
        Assert.Null(missing);
    }

    [Fact]
    public void ChemistryMicroBundle_Exposes_Canonical_And_Fictional_Element_Spaces()
    {
        using var chemistry = new ChemistryMicroBundle();
        var custom = new AtomBuilder("Dilithium", 0, "Dl")
            .WithAtomicWeight(0)
            .WithOrigin(ElementOrigin.Fictional)
            .WithCategory("Fictional")
            .Build();

        Assert.Equal(1, chemistry.FictionalElements.Count);
        Assert.True(chemistry.TryGetElement("Ub", out var unobtanium));
        Assert.True(unobtanium!.IsFictional);

        Assert.True(chemistry.RegisterFictionalElement(custom));
        Assert.True(chemistry.TryGetElement("Dl", out var dilithium));
        Assert.Same(custom, dilithium);
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
    public void ChemistryArbitrator_Preserves_Multiple_Fictional_Elements_With_Unassigned_Atomic_Number()
    {
        var fictionalA = new AtomBuilder("Dilithium", 0, "Dl")
            .WithOrigin(ElementOrigin.Fictional)
            .WithCategory("Fictional")
            .Build();
        var fictionalB = new AtomBuilder("Unobtanium-X", 0, "Ux")
            .WithOrigin(ElementOrigin.Fictional)
            .WithCategory("Fictional")
            .Build();

        var material = new MaterialComposition(
            "fictional-alloy",
            "Fictional Alloy",
            MaterialApplication.Structural,
            new ElementalFraction(fictionalA, 0.40),
            new ElementalFraction(fictionalB, 0.30));

        var source = new MaterialSourceBundle(material);
        var result = Assert.Single(new ElementalArbitrator().Evaluate(new HubBundle[] { source }));

        Assert.Equal(2, result.Elements.Count);
        Assert.Contains(result.Elements, x => x.Symbol == "Dl");
        Assert.Contains(result.Elements, x => x.Symbol == "Ux");
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
