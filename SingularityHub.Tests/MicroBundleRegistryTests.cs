using TheSingularityWorkshop.SingularityHub;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Full unit coverage for MicroBundleRegistry publication and withdrawal semantics.</summary>
public sealed class MicroBundleRegistryTests
{
    private static OntologySignature Ontology => new(1, 2, 3, 4, 5, 6, 7, 8, 9);

    [Fact(DisplayName = "Registry starts empty")]
    public void StartsEmpty()
    {
        var registry = new MicroBundleRegistry();
        Assert.Empty(registry.Bundles);
    }

    [Fact(DisplayName = "Valid bundle is published")]
    public void ValidBundleIsPublished()
    {
        var registry = new MicroBundleRegistry();
        var address = MicroBundleAddress.Create(Ontology, 0);
        var bundle = new TestBundle(100);

        Assert.True(registry.TryPublish(address, bundle));
        Assert.True(registry.Contains(address));
        Assert.Single(registry.Bundles);
        Assert.True(registry.TryGet(address, out var resolved));
        Assert.Same(bundle, resolved);
    }

    [Fact(DisplayName = "Duplicate address cannot be published")]
    public void DuplicateAddressCannotBePublished()
    {
        var registry = new MicroBundleRegistry();
        var address = MicroBundleAddress.Create(Ontology, 0);
        Assert.True(registry.TryPublish(address, new TestBundle(1)));
        Assert.False(registry.TryPublish(address, new TestBundle(2)));
        Assert.Single(registry.Bundles);
    }

    [Fact(DisplayName = "Invalid address is rejected")]
    public void InvalidAddressIsRejected()
    {
        var registry = new MicroBundleRegistry();
        var address = new MicroBundleAddress(Ontology, -1);
        Assert.False(registry.TryPublish(address, new TestBundle(1)));
        Assert.Empty(registry.Bundles);
    }

    [Fact(DisplayName = "Null bundle is rejected")]
    public void NullBundleIsRejected()
        => Assert.Throws<ArgumentNullException>(() => new MicroBundleRegistry().TryPublish(MicroBundleAddress.Create(Ontology, 0), null!));

    [Fact(DisplayName = "Unknown address cannot be resolved")]
    public void UnknownAddressCannotBeResolved()
    {
        var registry = new MicroBundleRegistry();
        var address = MicroBundleAddress.Create(Ontology, 0);
        Assert.False(registry.Contains(address));
        Assert.False(registry.TryGet(address, out var bundle));
        Assert.Null(bundle);
    }

    [Fact(DisplayName = "Withdraw removes and returns bundle")]
    public void WithdrawRemovesAndReturnsBundle()
    {
        var registry = new MicroBundleRegistry();
        var address = MicroBundleAddress.Create(Ontology, 2);
        var bundle = new TestBundle(20);
        registry.TryPublish(address, bundle);

        Assert.True(registry.TryWithdraw(address, out var withdrawn));
        Assert.Same(bundle, withdrawn);
        Assert.False(registry.Contains(address));
        Assert.Empty(registry.Bundles);
    }

    [Fact(DisplayName = "Withdraw of unknown address returns false")]
    public void WithdrawUnknownReturnsFalse()
        => Assert.False(new MicroBundleRegistry().TryWithdraw(MicroBundleAddress.Create(Ontology, 2), out var bundle));

    [Fact(DisplayName = "Different variants coexist under one ontology")]
    public void DifferentVariantsCoexist()
    {
        var registry = new MicroBundleRegistry();
        var first = MicroBundleAddress.Create(Ontology, 0);
        var second = MicroBundleAddress.Create(Ontology, 1);
        registry.TryPublish(first, new TestBundle(1));
        registry.TryPublish(second, new TestBundle(2));
        Assert.Equal(2, registry.Bundles.Count);
    }

    private sealed class TestBundle : IMicroBundle
    {
        public TestBundle(ulong id) => Id = id;
        public ulong Id { get; }
        public OntologySignature Ontology => MicroBundleRegistryTests.Ontology;
        public BundleVersion Version => new(1, 0, 0);
        public IReadOnlyList<ulong> Dependencies => Array.Empty<ulong>();
        public bool Arbitrate(IArbitrator arbitrator, int roundIndex) => false;
    }
}
