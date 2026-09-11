using HubKernel = TheSingularityWorkshop.SingularityHub.SingularityHub;
using TheSingularityWorkshop.SingularityHub;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Layer 6: Hub. The Hub owns coordination, eligibility, arbitration and liaison—not execution mechanics.</summary>
public sealed class HubDomainTests
{
    [ArchitectureTest(6, 1, 1)]
    [Fact(DisplayName = "6.01.001 — Hub_Implements_All_Coordination_Contracts")]
    public void Hub_Implements_All_Coordination_Contracts()
    {
        var hub = new HubKernel();
        Assert.IsAssignableFrom<IArbitrator>(hub);
        Assert.IsAssignableFrom<IDataWarehouseLiaison>(hub);
        Assert.IsAssignableFrom<IProcessGroupHost>(hub);
        Assert.IsAssignableFrom<ISingularityHub>(hub);
    }

    [ArchitectureTest(6, 1, 2)]
    [Fact(DisplayName = "6.01.002 — Hub_Starts_Empty")]
    public void Hub_Starts_Empty()
    {
        var hub = new HubKernel();
        Assert.Empty(hub.LoadedBundles);
        Assert.Empty(hub.ActiveGroups);
        Assert.Empty(hub.Audit.Events);
    }

    [ArchitectureTest(6, 1, 3)]
    [Fact(DisplayName = "6.01.003 — Hub_Loads_Valid_Bundle")]
    public void Hub_Loads_Valid_Bundle()
    {
        var hub = new HubKernel();
        var bundle = new TestBundle(6003);
        Assert.True(hub.LoadBundle(bundle));
        Assert.Contains(bundle, hub.LoadedBundles);
    }

    [ArchitectureTest(6, 1, 4)]
    [Fact(DisplayName = "6.01.004 — Hub_Rejects_Duplicate_Identity")]
    public void Hub_Rejects_Duplicate_Identity()
    {
        var hub = new HubKernel();
        Assert.True(hub.LoadBundle(new TestBundle(6004)));
        Assert.False(hub.LoadBundle(new TestBundle(6004)));
        Assert.Single(hub.LoadedBundles);
    }

    [ArchitectureTest(6, 1, 5)]
    [Fact(DisplayName = "6.01.005 — Hub_Rejects_Missing_Dependency")]
    public void Hub_Rejects_Missing_Dependency()
    {
        var hub = new HubKernel();
        var dependent = new TestBundle(6005, 99999);
        Assert.False(hub.LoadBundle(dependent));
        Assert.Empty(hub.LoadedBundles);
    }

    [ArchitectureTest(6, 1, 6)]
    [Fact(DisplayName = "6.01.006 — Hub_Loads_Dependency_Before_Dependent")]
    public void Hub_Loads_Dependency_Before_Dependent()
    {
        var hub = new HubKernel();
        var dependency = new TestBundle(6006);
        var dependent = new TestBundle(6007, 6006);
        Assert.True(hub.LoadBundle(dependency));
        Assert.True(hub.LoadBundle(dependent));
        Assert.Equal(2, hub.LoadedBundles.Count);
    }

    [ArchitectureTest(6, 1, 7)]
    [Fact(DisplayName = "6.01.007 — Arbitration_Orders_By_Identity")]
    public void Arbitration_Orders_By_Identity()
    {
        var hub = new HubKernel();
        Assert.True(hub.LoadBundle(new TestBundle(6010)));
        Assert.True(hub.LoadBundle(new TestBundle(6009)));
        Assert.Equal(2, hub.ExecuteArbitrationPipeline());
        Assert.Equal(new ulong[] { 6009, 6010 }, hub.Audit.Events.Select(e => e.ActorId));
    }

    [ArchitectureTest(6, 1, 8)]
    [Fact(DisplayName = "6.01.008 — Arbitration_Stops_At_First_Stable_Round")]
    public void Arbitration_Stops_At_First_Stable_Round()
    {
        var hub = new HubKernel();
        Assert.True(hub.LoadBundle(new TestBundle(6011)));
        Assert.Equal(1, hub.ExecuteArbitrationPipeline());
        var eventCount = hub.Audit.Events.Count;
        Assert.Equal(0, hub.ExecuteArbitrationPipeline());
        Assert.Equal(eventCount, hub.Audit.Events.Count);
    }

    [ArchitectureTest(6, 1, 9)]
    [Fact(DisplayName = "6.01.009 — Arbitration_Is_Bounded_To_Ten_Rounds")]
    public void Arbitration_Is_Bounded_To_Ten_Rounds()
    {
        var hub = new HubKernel();
        var bundle = new TenRoundBundle(6012);
        Assert.True(hub.LoadBundle(bundle));
        Assert.Equal(10, hub.ExecuteArbitrationPipeline());
        Assert.Equal(10, hub.Audit.Events.Count);
        Assert.Equal(9, hub.Audit.Events.Max(e => e.RoundIndex));
    }

    [ArchitectureTest(6, 1, 10)]
    [Fact(DisplayName = "6.01.010 — Audit_Records_Actor_Target_And_Mutation")]
    public void Audit_Records_Actor_Target_And_Mutation()
    {
        var hub = new HubKernel();
        var bundle = new TestBundle(6013);
        Assert.True(hub.LoadBundle(bundle));
        Assert.Equal(1, hub.ExecuteArbitrationPipeline());
        var audit = Assert.Single(hub.Audit.Events);
        Assert.Equal((ulong)6013, audit.ActorId);
        Assert.Equal(bundle.Ontology.StructuralId, audit.TargetCoordinates);
        Assert.Equal(MutationType.StructuralMutation, audit.MutationType);
    }

    [ArchitectureTest(6, 1, 11)]
    [Fact(DisplayName = "6.01.011 — Warehouse_Liaison_Resolves_Identity")]
    public void Warehouse_Liaison_Resolves_Identity()
    {
        var hub = new HubKernel();
        var address = new WarehouseAddress(7001);
        Assert.False(hub.TryResolve(7001, out _));
        hub.MapWarehouseIdentity(7001, address);
        Assert.True(hub.TryResolve(7001, out var resolved));
        Assert.Equal(address, resolved);
    }

    [ArchitectureTest(6, 1, 12)]
    [Fact(DisplayName = "6.01.012 — Warehouse_Unknown_Identity_Does_Not_Fabricate_Address")]
    public void Warehouse_Unknown_Identity_Does_Not_Fabricate_Address()
    {
        var hub = new HubKernel();
        Assert.False(hub.TryResolve(7002, out _));
    }

    [ArchitectureTest(6, 1, 13)]
    [Fact(DisplayName = "6.01.013 — Hub_Does_Not_Expose_Execution_Provider")]
    public void Hub_Does_Not_Expose_Execution_Provider()
    {
        var publicMethods = typeof(HubKernel).GetMethods().Where(m => m.DeclaringType == typeof(HubKernel));
        Assert.DoesNotContain(publicMethods, m => m.Name == "Execute" && m.GetParameters().Any(p => p.ParameterType == typeof(IExecutionProvider)));
    }

    private class TestBundle : IMicroBundle
    {
        private bool _changed;
        public TestBundle(ulong id, params ulong[] dependencies) => (Id, Dependencies) = (id, dependencies);
        public ulong Id { get; }
        public OntologySignature Ontology => new((int)Id, 0, 0, 0, 0, 0, 0, 0, 0);
        public BundleVersion Version => new(1, 0, 0);
        public IReadOnlyList<ulong> Dependencies { get; }
        public virtual bool Arbitrate(IArbitrator arbitrator, int roundIndex) => !_changed && MarkChanged();
        private bool MarkChanged() { _changed = true; return true; }
    }

    private sealed class TenRoundBundle : TestBundle
    {
        private int _round;
        public TenRoundBundle(ulong id) : base(id) { }
        public override bool Arbitrate(IArbitrator arbitrator, int roundIndex)
        {
            _round++;
            return _round <= 10;
        }
    }
}
