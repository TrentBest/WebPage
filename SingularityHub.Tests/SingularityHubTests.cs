using TheSingularityWorkshop.SingularityHub;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Direct behavioral coverage for the Hub orchestration boundary.</summary>
public sealed class SingularityHubTests
{
    [Fact(DisplayName = "Hub starts with empty operational surfaces")]
    public void StartsEmpty()
    {
        var hub = new TheSingularityWorkshop.SingularityHub.SingularityHub(_ => { });
        Assert.Empty(hub.LoadedBundles);
        Assert.Empty(hub.ProcessGroups);
        Assert.Empty(hub.ActiveGroups);
        Assert.Empty(hub.Audit.Events);
        Assert.Empty(hub.Routing.Routes);
    }

    [Fact(DisplayName = "Null update delegate is rejected")]
    public void NullUpdateDelegateIsRejected()
        => Assert.Throws<ArgumentNullException>(() => new TheSingularityWorkshop.SingularityHub.SingularityHub(null!));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void RegisterProcessGroupRejectsMissingName(string? name)
    {
        var hub = new TheSingularityWorkshop.SingularityHub.SingularityHub(_ => { });
        Assert.Throws<ArgumentException>(() => hub.RegisterProcessGroup(name!));
    }

    [Fact(DisplayName = "RegisterProcessGroup returns hub and records registration")]
    public void RegisterProcessGroupIsFluent()
    {
        var hub = new TheSingularityWorkshop.SingularityHub.SingularityHub(_ => { });
        Assert.Same(hub, hub.RegisterProcessGroup("root"));
        var registration = Assert.Single(hub.ProcessGroups);
        Assert.Equal("root", registration.Name);
        Assert.Null(registration.ParentName);
    }

    [Fact(DisplayName = "Duplicate process group is rejected")]
    public void DuplicateProcessGroupIsRejected()
    {
        var hub = new TheSingularityWorkshop.SingularityHub.SingularityHub(_ => { });
        hub.RegisterProcessGroup("root");
        Assert.Throws<InvalidOperationException>(() => hub.RegisterProcessGroup("root"));
    }

    [Fact(DisplayName = "Child process group requires registered parent")]
    public void ChildRequiresRegisteredParent()
    {
        var hub = new TheSingularityWorkshop.SingularityHub.SingularityHub(_ => { });
        Assert.Throws<InvalidOperationException>(() => hub.RegisterProcessGroup("child", "missing"));
    }

    [Fact(DisplayName = "Registered parent can own child")]
    public void RegisteredParentCanOwnChild()
    {
        var hub = new TheSingularityWorkshop.SingularityHub.SingularityHub(_ => { });
        hub.RegisterProcessGroup("root").RegisterProcessGroup("child", "root");
        Assert.Equal("root", hub.ProcessGroups[1].ParentName);
    }

    [Fact(DisplayName = "RegisterProcessGroups registers all names")]
    public void RegisterProcessGroupsRegistersAllNames()
    {
        var hub = new TheSingularityWorkshop.SingularityHub.SingularityHub(_ => { });
        Assert.Same(hub, hub.RegisterProcessGroups("a", "b", "c"));
        Assert.Equal(new[] { "a", "b", "c" }, hub.ProcessGroups.Select(x => x.Name));
    }

    [Fact(DisplayName = "Null process group array is rejected")]
    public void NullProcessGroupArrayIsRejected()
        => Assert.Throws<ArgumentNullException>(() => new TheSingularityWorkshop.SingularityHub.SingularityHub(_ => { }).RegisterProcessGroups(null!));

    [Fact(DisplayName = "Update advances only root process groups")]
    public void UpdateAdvancesOnlyRoots()
    {
        var updates = new List<string>();
        var hub = new TheSingularityWorkshop.SingularityHub.SingularityHub(updates.Add);
        hub.RegisterProcessGroup("root").RegisterProcessGroup("child", "root").RegisterProcessGroup("other");
        hub.Update();
        Assert.Equal(new[] { "root", "other" }, updates);
    }

    [Fact(DisplayName = "Nested update advances only direct children")]
    public void UpdateNestedAdvancesDirectChildren()
    {
        var updates = new List<string>();
        var hub = new TheSingularityWorkshop.SingularityHub.SingularityHub(updates.Add);
        hub.RegisterProcessGroup("root").RegisterProcessGroup("child", "root").RegisterProcessGroup("grandchild", "child");
        hub.UpdateNestedProcessGroups("root");
        Assert.Equal(new[] { "child" }, updates);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void NestedUpdateRejectsMissingParent(string? parent)
        => Assert.Throws<ArgumentException>(() => new TheSingularityWorkshop.SingularityHub.SingularityHub(_ => { }).UpdateNestedProcessGroups(parent!));

    [Fact(DisplayName = "Explicit process group update requires registration")]
    public void ExplicitUpdateRequiresRegistration()
    {
        var hub = new TheSingularityWorkshop.SingularityHub.SingularityHub(_ => { });
        Assert.Throws<InvalidOperationException>(() => hub.UpdateProcessGroup("missing"));
    }

    [Fact(DisplayName = "Explicit process group update invokes delegate")]
    public void ExplicitUpdateInvokesDelegate()
    {
        string? updated = null;
        var hub = new TheSingularityWorkshop.SingularityHub.SingularityHub(value => updated = value);
        hub.RegisterProcessGroup("root");
        hub.UpdateProcessGroup("root");
        Assert.Equal("root", updated);
    }

    [Fact(DisplayName = "Process group lifecycle follows registered active completed")]
    public void ProcessGroupLifecycleFollowsValidSequence()
    {
        var hub = new TheSingularityWorkshop.SingularityHub.SingularityHub(_ => { });
        Assert.True(hub.Register(42));
        Assert.Empty(hub.ActiveGroups);
        Assert.True(hub.Activate(42));
        var active = Assert.Single(hub.ActiveGroups);
        Assert.Equal(42UL, active.Id);
        Assert.Equal(ProcessGroupState.Active, active.State);
        Assert.True(hub.Complete(42));
        Assert.Empty(hub.ActiveGroups);
    }

    [Fact(DisplayName = "Process group lifecycle rejects invalid transitions")]
    public void ProcessGroupLifecycleRejectsInvalidTransitions()
    {
        var hub = new TheSingularityWorkshop.SingularityHub.SingularityHub(_ => { });
        Assert.False(hub.Activate(1));
        Assert.False(hub.Complete(1));
        Assert.True(hub.Register(1));
        Assert.False(hub.Register(1));
        Assert.False(hub.Complete(1));
        Assert.True(hub.Activate(1));
        Assert.False(hub.Activate(1));
        Assert.True(hub.Complete(1));
        Assert.False(hub.Complete(1));
    }

    [Fact(DisplayName = "Warehouse identity can be mapped and resolved")]
    public void WarehouseIdentityCanBeMappedAndResolved()
    {
        var hub = new TheSingularityWorkshop.SingularityHub.SingularityHub(_ => { });
        var address = new WarehouseAddress(900);
        Assert.False(hub.TryResolve(7, out _));
        hub.MapWarehouseIdentity(7, address);
        Assert.True(hub.TryResolve(7, out var resolved));
        Assert.Equal(address, resolved);
    }

    [Fact(DisplayName = "Warehouse identity remapping replaces existing address")]
    public void WarehouseIdentityRemappingReplacesExistingAddress()
    {
        var hub = new TheSingularityWorkshop.SingularityHub.SingularityHub(_ => { });
        hub.MapWarehouseIdentity(7, new WarehouseAddress(1));
        hub.MapWarehouseIdentity(7, new WarehouseAddress(2));
        Assert.True(hub.TryResolve(7, out var resolved));
        Assert.Equal(new WarehouseAddress(2), resolved);
    }

    [Fact(DisplayName = "Null bundle cannot be loaded")]
    public void NullBundleCannotBeLoaded()
        => Assert.Throws<ArgumentNullException>(() => new TheSingularityWorkshop.SingularityHub.SingularityHub(_ => { }).LoadBundle(null!));

    [Fact(DisplayName = "Bundle can be loaded once")]
    public void BundleCanBeLoadedOnce()
    {
        var hub = new TheSingularityWorkshop.SingularityHub.SingularityHub(_ => { });
        var bundle = new TestBundle(20);
        Assert.True(hub.LoadBundle(bundle));
        Assert.False(hub.LoadBundle(bundle));
        Assert.Single(hub.LoadedBundles);
    }

    [Fact(DisplayName = "Bundle can resolve an available dependency during load")]
    public void BundleCanResolveAvailableDependencyDuringLoad()
    {
        var hub = new TheSingularityWorkshop.SingularityHub.SingularityHub(_ => { });
        var dependency = new TestBundle(10);
        var dependent = new TestBundle(20, 10);
        Assert.True(hub.RegisterBundle(dependency));
        Assert.True(hub.RegisterBundle(dependent));
        Assert.True(hub.LoadBundle(dependent));
        Assert.Equal(new ulong[] { 10, 20 }, hub.LoadedBundles.Select(x => x.Id));
        Assert.Equal(1, dependency.LoadCount);
        Assert.Equal(1, dependent.LoadCount);
    }

    [Fact(DisplayName = "Missing dependency does not fabricate a bundle")]
    public void MissingDependencyDoesNotFabricateBundle()
    {
        var hub = new TheSingularityWorkshop.SingularityHub.SingularityHub(_ => { });
        var dependent = new TestBundle(20, 999);
        Assert.True(hub.LoadBundle(dependent));
        Assert.Single(hub.LoadedBundles);
        Assert.DoesNotContain(hub.LoadedBundles, bundle => bundle.Id == 999);
        Assert.Equal(1, dependent.LoadCount);
    }

    [Fact(DisplayName = "Loaded bundles preserve installation order")]
    public void LoadedBundlesPreserveInstallationOrder()
    {
        var hub = new TheSingularityWorkshop.SingularityHub.SingularityHub(_ => { });
        hub.LoadBundle(new TestBundle(30));
        hub.LoadBundle(new TestBundle(10));
        hub.LoadBundle(new TestBundle(20));
        Assert.Equal(new ulong[] { 30, 10, 20 }, hub.LoadedBundles.Select(x => x.Id));
    }

    [Fact(DisplayName = "Arbitration stops when no bundle mutates")]

    public void ArbitrationStopsWhenNoBundleMutates()
    {
        var hub = new TheSingularityWorkshop.SingularityHub.SingularityHub(_ => { });
        hub.LoadBundle(new TestBundle(1));
        Assert.Equal(0, hub.ExecuteArbitrationPipeline());
        Assert.Empty(hub.Audit.Events);
    }

    [Fact(DisplayName = "Arbitration records structural mutations")]
    public void ArbitrationRecordsStructuralMutations()
    {
        var hub = new TheSingularityWorkshop.SingularityHub.SingularityHub(_ => { });
        hub.LoadBundle(new TestBundle(1, mutateRounds: 2));
        Assert.Equal(2, hub.ExecuteArbitrationPipeline());
        Assert.Equal(2, hub.Audit.Events.Count);
        Assert.All(hub.Audit.Events, entry => Assert.Equal(MutationType.StructuralMutation, entry.MutationType));
        Assert.Equal(new[] { 0, 1 }, hub.Audit.Events.Select(x => x.RoundIndex));
    }

    [Fact(DisplayName = "Arbitration never exceeds ten rounds")]
    public void ArbitrationIsBoundedToTenRounds()
    {
        var hub = new TheSingularityWorkshop.SingularityHub.SingularityHub(_ => { });
        hub.LoadBundle(new TestBundle(1, mutateRounds: 100));
        Assert.Equal(10, hub.ExecuteArbitrationPipeline());
        Assert.Equal(10, hub.Audit.Events.Count);
    }

    private sealed class TestBundle : IMicroBundle
    {
        private readonly int _mutateRounds;
        private readonly ulong[] _dependencies;
        public TestBundle(ulong id, ulong dependency = 0, int mutateRounds = 0)
        {
            Id = id;
            _dependencies = dependency == 0 ? Array.Empty<ulong>() : new[] { dependency };
            _mutateRounds = mutateRounds;
        }
        public ulong Id { get; }
        public OntologySignature Ontology => new(1, 2, 3, 4, 5, 6, 7, 8, 9);
        public BundleVersion Version => new(1, 0, 0);
        public IReadOnlyList<ulong> Dependencies => _dependencies;
        public int LoadCount { get; private set; }
        public void LoadBundle(IArbitrator arbitrator)
        {
            LoadCount++;
            foreach (var dependency in Dependencies) arbitrator.TryLoadBundle(dependency);
        }
        public bool Arbitrate(IArbitrator arbitrator, int roundIndex) => roundIndex < _mutateRounds;
    }
}
