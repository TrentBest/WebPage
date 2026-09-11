using TheSingularityWorkshop.SingularityHub;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SingularityHubTests
{
    [Fact]
    public void OntologySignature_IsFixedNineLayers_AndDeterministic()
    {
        var a = new OntologySignature(1,2,3,4,5,6,7,8,9);
        var b = new OntologySignature(1,2,3,4,5,6,7,8,9);
        Assert.Equal(OntologySignature.LayerCount, 9);
        Assert.Equal(a, b);
        Assert.Equal(a.StructuralId, b.StructuralId);
    }

    [Fact]
    public void Hub_RejectsDuplicateBundle_AndArbitratesToHomeostasis()
    {
        var hub = new SingularityHub();
        var bundle = new TestBundle();
        Assert.True(hub.LoadBundle(bundle));
        Assert.False(hub.LoadBundle(bundle));
        Assert.Equal(1, hub.ExecuteArbitrationPipeline());
        Assert.Single(hub.Audit.Events);
        Assert.Equal(0, hub.ExecuteArbitrationPipeline());
    }

    [Fact]
    public void ProcessGroupLifecycle_IsTrackedWithoutExecutionMechanics()
    {
        var hub = new SingularityHub();
        Assert.True(hub.Register(42));
        Assert.True(hub.Activate(42));
        Assert.Contains(hub.ActiveGroups, g => g.Id == 42 && g.State == ProcessGroupState.Active);
        Assert.True(hub.Complete(42));
        Assert.Empty(hub.ActiveGroups);
    }

    private sealed class TestBundle : IMicroBundle
    {
        private bool _changed;
        public ulong Id => 7;
        public OntologySignature Ontology => new(1,0,0,0,0,0,0,0,0);
        public BundleVersion Version => new(1,0,0);
        public IReadOnlyList<ulong> Dependencies => Array.Empty<ulong>();
        public bool Arbitrate(IArbitrator arbitrator, int roundIndex) => !_changed && MarkChanged();
        private bool MarkChanged() { _changed = true; return true; }
    }
}
