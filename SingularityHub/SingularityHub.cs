using System;
using System.Collections.Generic;
using System.Linq;
using TheSingularityWorkshop.FSM_API;
using fsm_API = TheSingularityWorkshop.FSM_API.FSM_API;

namespace TheSingularityWorkshop.SingularityHub;

/// <summary>
/// Runtime Hub: coordinates MicroBundles, arbitration, process-group lifecycle,
/// Warehouse identity resolution, and FSM_API root scheduling.
/// </summary>
public sealed class SingularityHub : ISingularityHub
{
    private const int MaximumArbitrationRounds = 10;

    private readonly List<ProcessGroupRegistration> _registrations = new();
    private readonly Dictionary<ulong, ProcessGroupState> _processGroups = new();
    private readonly Dictionary<ulong, WarehouseAddress> _warehouseIdentities = new();
    private readonly List<IMicroBundle> _loadedBundles = new();
    private readonly Action<string> _update;

    public SingularityHub()
        : this(fsm_API.Interaction.Update)
    {
    }

    /// <summary>Creates a Hub with an injectable FSM scheduler for deterministic tests.</summary>
    public SingularityHub(Action<string> update)
    {
        _update = update ?? throw new ArgumentNullException(nameof(update));
        Audit = new ArbitrationAudit();
    }

    /// <summary>Bundles retained by the Hub after dependency validation.</summary>
    public IReadOnlyCollection<IMicroBundle> LoadedBundles => _loadedBundles;

    /// <summary>Arbitration mutation history retained for inspection and undo tooling.</summary>
    public IArbitrationAudit Audit { get; }

    /// <summary>Process groups registered with this Hub in registration order.</summary>
    public IReadOnlyList<ProcessGroupRegistration> ProcessGroups => _registrations;

    /// <summary>Currently active process groups.</summary>
    public IReadOnlyCollection<ProcessGroupSnapshot> ActiveGroups =>
        _processGroups
            .Where(pair => pair.Value == ProcessGroupState.Active)
            .Select(pair => new ProcessGroupSnapshot(pair.Key, pair.Value))
            .ToArray();

    /// <summary>Registers a process group with the Hub scheduler.</summary>
    public SingularityHub RegisterProcessGroup(string processGroup, string? parentProcessGroup = null)
    {
        if (string.IsNullOrWhiteSpace(processGroup))
            throw new ArgumentException("A process group name is required.", nameof(processGroup));

        if (_registrations.Exists(x => x.Name == processGroup))
            throw new InvalidOperationException($"Process group '{processGroup}' is already registered.");

        if (parentProcessGroup is not null && !_registrations.Exists(x => x.Name == parentProcessGroup))
            throw new InvalidOperationException($"Parent process group '{parentProcessGroup}' must be registered first.");

        _registrations.Add(new ProcessGroupRegistration(processGroup, parentProcessGroup));
        return this;
    }

    /// <summary>Registers several root process groups fluently.</summary>
    public SingularityHub RegisterProcessGroups(params string[] processGroups)
    {
        if (processGroups is null)
            throw new ArgumentNullException(nameof(processGroups));

        foreach (var processGroup in processGroups)
            RegisterProcessGroup(processGroup);

        return this;
    }

    /// <summary>Steps only Hub-owned root process groups.</summary>
    public void Update()
    {
        foreach (var registration in _registrations)
        {
            if (registration.ParentName is null)
                _update(registration.Name);
        }
    }

    /// <summary>Steps only the direct nested children of an owning process group.</summary>
    public void UpdateNestedProcessGroups(string parentProcessGroup)
    {
        if (string.IsNullOrWhiteSpace(parentProcessGroup))
            throw new ArgumentException("A parent process group name is required.", nameof(parentProcessGroup));

        foreach (var registration in _registrations)
        {
            if (registration.ParentName == parentProcessGroup)
                _update(registration.Name);
        }
    }

    public bool LoadBundle(IMicroBundle bundle)
    {
        if (bundle is null)
            throw new ArgumentNullException(nameof(bundle));

        if (_loadedBundles.Any(existing => existing.Id == bundle.Id))
            return false;

        foreach (var dependency in bundle.Dependencies)
        {
            if (_loadedBundles.All(existing => existing.Id != dependency))
                return false;
        }

        _loadedBundles.Add(bundle);
        _loadedBundles.Sort((left, right) => left.Id.CompareTo(right.Id));
        return true;
    }

    /// <summary>
    /// Runs bounded arbitration rounds. The return value is the number of
    /// bundle mutations accepted across all rounds, not the number of rounds.
    /// </summary>
    public int ExecuteArbitrationPipeline()
    {
        var mutations = 0;

        for (var round = 0; round < MaximumArbitrationRounds; round++)
        {
            var changedThisRound = false;

            foreach (var bundle in _loadedBundles.OrderBy(bundle => bundle.Id))
            {
                if (!bundle.Arbitrate(this, round))
                    continue;

                changedThisRound = true;
                mutations++;
                Audit.Record(new ArbitrationEvent(
                    round,
                    bundle.Id,
                    bundle.Ontology.StructuralId,
                    MutationType.StructuralMutation,
                    0));
            }

            if (!changedThisRound)
                break;
        }

        return mutations;
    }

    public bool TryResolve(ulong identity, out WarehouseAddress address) =>
        _warehouseIdentities.TryGetValue(identity, out address);

    public void MapWarehouseIdentity(ulong identity, WarehouseAddress address) =>
        _warehouseIdentities[identity] = address;

    public bool Register(ulong id)
    {
        if (_processGroups.ContainsKey(id))
            return false;

        _processGroups[id] = ProcessGroupState.Registered;
        return true;
    }

    public bool Activate(ulong id)
    {
        if (!_processGroups.TryGetValue(id, out var state) || state != ProcessGroupState.Registered)
            return false;

        _processGroups[id] = ProcessGroupState.Active;
        return true;
    }

    public bool Complete(ulong id)
    {
        if (!_processGroups.TryGetValue(id, out var state) || state != ProcessGroupState.Active)
            return false;

        _processGroups[id] = ProcessGroupState.Completed;
        return true;
    }

    /// <summary>Immutable description of one registered scheduler process group.</summary>
    public sealed record ProcessGroupRegistration(string Name, string? ParentName);
}

/// <summary>Ordered, append-only arbitration audit used by the Hub contract.</summary>
public sealed class ArbitrationAudit : IArbitrationAudit
{
    private readonly List<ArbitrationEvent> _events = new();

    public IReadOnlyList<ArbitrationEvent> Events => _events;

    public void Record(ArbitrationEvent arbitrationEvent) => _events.Add(arbitrationEvent);
}
