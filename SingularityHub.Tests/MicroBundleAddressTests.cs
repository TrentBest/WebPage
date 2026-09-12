using TheSingularityWorkshop.SingularityHub;
using Xunit;

namespace SingularityHub.Tests;

public sealed class MicroBundleAddressTests
{
    [Fact(DisplayName = "MicroBundle address permits distinct variants under one ontology")]
    public void Address_AllowsDistinctVariants()
    {
        var ontology = new OntologySignature(1, 2, 3, 4, 5, 6, 7, 8, 9);

        var first = MicroBundleAddress.Create(ontology, 0);
        var second = MicroBundleAddress.Create(ontology, int.MaxValue - 1);

        Assert.NotEqual(first, second);
        Assert.True(first.IsValid);
        Assert.True(second.IsValid);
        Assert.Equal(int.MaxValue, MicroBundleAddress.VariantCapacity);
    }

    [Fact(DisplayName = "MicroBundle registry permits one occupant per exact address")]
    public void Registry_EnforcesOneOccupantPerAddress()
    {
        var ontology = new OntologySignature(10, 20, 30, 40, 50, 60, 70, 80, 90);
        var address = MicroBundleAddress.Create(ontology, 42);
        var registry = new MicroBundleRegistry();
        var first = new TestBundle(1, ontology);
        var second = new TestBundle(2, ontology);

        Assert.True(registry.TryPublish(address, first));
        Assert.False(registry.TryPublish(address, second));
        Assert.True(registry.TryGet(address, out var resolved));
        Assert.Same(first, resolved);
    }

    private sealed class TestBundle : IMicroBundle
    {
        public TestBundle(ulong id, OntologySignature ontology)
        {
            Id = id;
            Ontology = ontology;
        }

        public ulong Id { get; }
        public OntologySignature Ontology { get; }
        public BundleVersion Version => new(1, 0, 0);
        public IReadOnlyList<ulong> Dependencies => Array.Empty<ulong>();
        public bool Arbitrate(IArbitrator arbitrator, int roundIndex) => true;
    }
}
