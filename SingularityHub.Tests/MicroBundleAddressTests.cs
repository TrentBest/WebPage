using TheSingularityWorkshop.SingularityHub;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Exhaustive unit coverage for MicroBundleAddress validity and identity semantics.</summary>
public sealed class MicroBundleAddressTests
{
    private static OntologySignature Ontology => new(1, 2, 3, 4, 5, 6, 7, 8, 9);

    [Fact(DisplayName = "Variant zero is valid")]
    public void VariantZeroIsValid()
        => Assert.True(new MicroBundleAddress(Ontology, 0).IsValid);

    [Fact(DisplayName = "Last reserved variant is valid")]
    public void LastReservedVariantIsValid()
        => Assert.True(new MicroBundleAddress(Ontology, int.MaxValue - 1).IsValid);

    [Fact(DisplayName = "Negative variant is invalid")]
    public void NegativeVariantIsInvalid()
        => Assert.False(new MicroBundleAddress(Ontology, -1).IsValid);

    [Fact(DisplayName = "Int maximum variant is outside capacity")]
    public void IntMaximumVariantIsOutsideCapacity()
        => Assert.False(new MicroBundleAddress(Ontology, int.MaxValue).IsValid);

    [Fact(DisplayName = "Variant capacity is int maximum")]
    public void VariantCapacityIsIntMaximum()
        => Assert.Equal((long)int.MaxValue, MicroBundleAddress.VariantCapacity);

    [Fact(DisplayName = "Create accepts valid variant")]
    public void CreateAcceptsValidVariant()
    {
        var address = MicroBundleAddress.Create(Ontology, 7);
        Assert.Equal(Ontology, address.Ontology);
        Assert.Equal(7, address.VariantId);
        Assert.True(address.IsValid);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    public void CreateRejectsInvalidVariant(int variant)
        => Assert.Throws<ArgumentOutOfRangeException>(() => MicroBundleAddress.Create(Ontology, variant));

    [Fact(DisplayName = "Same ontology and variant produce equal addresses")]
    public void SameCoordinatesAreEqual()
        => Assert.Equal(new MicroBundleAddress(Ontology, 3), new MicroBundleAddress(Ontology, 3));

    [Fact(DisplayName = "Different variants produce different structural identities")]
    public void DifferentVariantsProduceDifferentStructuralIds()
        => Assert.NotEqual(new MicroBundleAddress(Ontology, 1).StructuralId, new MicroBundleAddress(Ontology, 2).StructuralId);

    [Fact(DisplayName = "Different ontology produces different structural identity")]
    public void DifferentOntologyProducesDifferentStructuralId()
    {
        var other = new OntologySignature(9, 8, 7, 6, 5, 4, 3, 2, 1);
        Assert.NotEqual(new MicroBundleAddress(Ontology, 1).StructuralId, new MicroBundleAddress(other, 1).StructuralId);
    }

    [Fact(DisplayName = "Registry indexes every bundle across all nine ontology layers")]
    public void RegistryIndexesAllNineLayers()
    {
        var registry = new MicroBundleRegistry();
        var bundle = new TestMicroBundle(101, Ontology);

        Assert.True(registry.TryPublish(MicroBundleAddress.Create(Ontology, 7), bundle));

        for (var layer = 0; layer < OntologySignature.LayerCount; layer++)
        {
            var matches = registry.GetByLayer(layer, Ontology[layer]);
            Assert.Single(matches);
            Assert.Same(bundle, matches.Single());
        }
    }

    [Fact(DisplayName = "Registry keeps exact address identity separate from ontology indexing")]
    public void RegistrySupportsMultipleVariantsUnderOneOntology()
    {
        var registry = new MicroBundleRegistry();
        var first = new TestMicroBundle(201, Ontology);
        var second = new TestMicroBundle(202, Ontology);

        Assert.True(registry.TryPublish(MicroBundleAddress.Create(Ontology, 1), first));
        Assert.True(registry.TryPublish(MicroBundleAddress.Create(Ontology, 2), second));

        Assert.True(registry.TryGet(MicroBundleAddress.Create(Ontology, 1), out var exact));
        Assert.Same(first, exact);

        var matches = registry.GetByLayer(8, Ontology[8]);
        Assert.Equal(2, matches.Count);
        Assert.Contains(first, matches);
        Assert.Contains(second, matches);
    }

    [Fact(DisplayName = "Registry withdraw removes the bundle from every ontology layer index")]
    public void RegistryWithdrawRemovesAllLayerIndexes()
    {
        var registry = new MicroBundleRegistry();
        var address = MicroBundleAddress.Create(Ontology, 3);
        var bundle = new TestMicroBundle(301, Ontology);

        Assert.True(registry.TryPublish(address, bundle));
        Assert.True(registry.TryWithdraw(address, out var withdrawn));
        Assert.Same(bundle, withdrawn);

        for (var layer = 0; layer < OntologySignature.LayerCount; layer++)
            Assert.Empty(registry.GetByLayer(layer, Ontology[layer]));

        Assert.False(registry.Contains(address));
    }

    [Fact(DisplayName = "Registry rejects a bundle whose ontology disagrees with its address")]
    public void RegistryRejectsOntologyMismatch()
    {
        var registry = new MicroBundleRegistry();
        var differentOntology = new OntologySignature(9, 8, 7, 6, 5, 4, 3, 2, 1);
        var bundle = new TestMicroBundle(401, differentOntology);

        Assert.False(registry.TryPublish(MicroBundleAddress.Create(Ontology, 1), bundle));
        Assert.Empty(registry.Bundles);
    }

    private sealed class TestMicroBundle(ulong id, OntologySignature ontology) : IMicroBundle
    {
        public ulong Id { get; } = id;
        public OntologySignature Ontology { get; } = ontology;
        public BundleVersion Version => new(1, 0, 0);
        public IReadOnlyList<ulong> Dependencies => Array.Empty<ulong>();
        public bool Arbitrate(IArbitrator arbitrator, int roundIndex) => true;
    }
}
