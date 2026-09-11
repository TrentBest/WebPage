namespace TheSingularityWorkshop.SingularityHub;

/// <summary>Platform-neutral concrete Hub. It coordinates; it never schedules hardware.</summary>
public sealed class SingularityHub : ISingularityHub
{
    private readonly Dictionary<ulong, IMicroBundle> _bundles = new();
    private readonly Dictionary<ulong, ProcessGroupState> _groups = new();
    private readonly Dictionary<ulong, WarehouseAddress> _warehouse = new();

    public SingularityHub(IArbitrationAudit? audit = null) => Audit = audit ?? new ArbitrationAudit();
    public IArbitrationAudit Audit { get; }
    public IReadOnlyCollection<IMicroBundle> LoadedBundles => _bundles.Values;
    public IReadOnlyCollection<ProcessGroupSnapshot> ActiveGroups => _groups.Select(p => new ProcessGroupSnapshot(p.Key, p.Value)).Where(p => p.State == ProcessGroupState.Active).ToArray();

    public bool LoadBundle(IMicroBundle bundle)
    {
        ArgumentNullException.ThrowIfNull(bundle);
        if (_bundles.ContainsKey(bundle.Id)) return false;
        foreach (var dependency in bundle.Dependencies)
            if (!_bundles.ContainsKey(dependency)) return false;
        _bundles.Add(bundle.Id, bundle);
        return true;
    }

    public int ExecuteArbitrationPipeline()
    {
        var mutations = 0;
        for (var round = 0; round < 10; round++)
        {
            var roundMutations = 0;
            foreach (var bundle in _bundles.Values.OrderBy(b => b.Id))
            {
                if (!bundle.Arbitrate(this, round)) continue;
                roundMutations++;
                Audit.Record(new ArbitrationEvent(round, bundle.Id, bundle.Ontology.StructuralId, MutationType.StructuralMutation, 0));
            }
            mutations += roundMutations;
            if (roundMutations == 0) break;
        }
        return mutations;
    }

    public bool TryResolve(ulong identity, out WarehouseAddress address) => _warehouse.TryGetValue(identity, out address);
    public void MapWarehouseIdentity(ulong identity, WarehouseAddress address) => _warehouse[identity] = address;
    public bool Register(ulong id) => _groups.TryAdd(id, ProcessGroupState.Registered);
    public bool Activate(ulong id) => _groups.TryGetValue(id, out var state) && state == ProcessGroupState.Registered && SetState(id, ProcessGroupState.Active);
    public bool Complete(ulong id) => _groups.TryGetValue(id, out var state) && state == ProcessGroupState.Active && SetState(id, ProcessGroupState.Completed);
    private bool SetState(ulong id, ProcessGroupState state) { _groups[id] = state; return true; }
}

public sealed class ArbitrationAudit : IArbitrationAudit
{
    private readonly List<ArbitrationEvent> _events = new();
    public IReadOnlyList<ArbitrationEvent> Events => _events;
    public void Record(ArbitrationEvent arbitrationEvent) => _events.Add(arbitrationEvent);
}
