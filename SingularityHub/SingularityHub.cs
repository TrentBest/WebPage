using System;
using System.Collections.Generic;
using System.Linq;
using TheSingularityWorkshop.FSM_API;
using fsm_API = TheSingularityWorkshop.FSM_API.FSM_API;

namespace TheSingularityWorkshop.SingularityHub;

/// <summary>Runtime Hub coordinating MicroBundle arbitration, process groups, warehouse identities, and routing.</summary>
public sealed class SingularityHub : ISingularityHub
{
    /// <summary>Maximum number of arbitration rounds permitted for one pipeline execution.</summary>
    public const int MaximumArbitrationRounds = 10;
    private readonly List<ProcessGroupRegistration> _registrations = new();
    private readonly Dictionary<ulong, ProcessGroupState> _processGroups = new();
    private readonly Dictionary<ulong, WarehouseAddress> _warehouseIdentities = new();
    private readonly Dictionary<ulong, IMicroBundle> _availableBundles = new();
    private readonly List<IMicroBundle> _loadedBundles = new();
    private readonly HashSet<ulong> _loadingBundles = new();
    private readonly HashSet<ulong> _loadAttempts = new();
    private readonly Action<string> _update;

    /// <summary>Creates a Hub using the FSM API update sink.</summary>
    public SingularityHub() : this(fsm_API.Interaction.Update) { }
    /// <summary>Creates a Hub using the supplied process-group update sink.</summary>
    public SingularityHub(Action<string> update)
    {
        _update=update??throw new ArgumentNullException(nameof(update));
        Audit=new ArbitrationAudit();
        Routing=new SingularityRouting();
    }
    /// <summary>Gets bundles that completed installation.</summary>
    public IReadOnlyCollection<IMicroBundle> LoadedBundles=>_loadedBundles;
    /// <summary>Gets bundles registered as available for installation.</summary>
    public IReadOnlyCollection<IMicroBundle> AvailableBundles=>_availableBundles.Values;
    /// <summary>Gets the arbitration audit sink.</summary>
    public IArbitrationAudit Audit{get;}
    /// <summary>Gets the Hub routing table.</summary>
    public ISingularityRouting Routing{get;}
    /// <summary>Gets the registered process-group declarations.</summary>
    public IReadOnlyList<ProcessGroupRegistration> ProcessGroups=>_registrations;
    /// <summary>Gets process groups currently in the active lifecycle state.</summary>
    public IReadOnlyCollection<ProcessGroupSnapshot> ActiveGroups=>
        _processGroups.Where(pair=>pair.Value==ProcessGroupState.Active)
            .Select(pair=>new ProcessGroupSnapshot(pair.Key,pair.Value)).ToArray();

    /// <summary>Registers one process group, optionally under an existing parent.</summary>
    public SingularityHub RegisterProcessGroup(string processGroup,string? parentProcessGroup=null)
    {
        if(string.IsNullOrWhiteSpace(processGroup))throw new ArgumentException("A process group name is required.",nameof(processGroup));
        if(_registrations.Exists(x=>x.Name==processGroup))throw new InvalidOperationException($"Process group '{processGroup}' is already registered.");
        if(parentProcessGroup is not null&&!_registrations.Exists(x=>x.Name==parentProcessGroup))throw new InvalidOperationException($"Parent process group '{parentProcessGroup}' must be registered first.");
        _registrations.Add(new ProcessGroupRegistration(processGroup,parentProcessGroup)); return this;
    }
    /// <summary>Registers several root process groups.</summary>
    public SingularityHub RegisterProcessGroups(params string[] processGroups)
    {
        if(processGroups is null)throw new ArgumentNullException(nameof(processGroups));
        foreach(var processGroup in processGroups)RegisterProcessGroup(processGroup); return this;
    }
    /// <summary>Updates all root process groups.</summary>
    public void Update(){foreach(var registration in _registrations)if(registration.ParentName is null)_update(registration.Name);}
    /// <summary>Updates direct child process groups under a parent.</summary>
    public void UpdateNestedProcessGroups(string parentProcessGroup)
    {
        if(string.IsNullOrWhiteSpace(parentProcessGroup))throw new ArgumentException("A parent process group name is required.",nameof(parentProcessGroup));
        foreach(var registration in _registrations)if(registration.ParentName==parentProcessGroup)_update(registration.Name);
    }
    /// <summary>Updates one registered process group.</summary>
    public void UpdateProcessGroup(string processGroup)
    {
        if(string.IsNullOrWhiteSpace(processGroup))throw new ArgumentException("A process group name is required.",nameof(processGroup));
        if(!_registrations.Exists(x=>x.Name==processGroup))throw new InvalidOperationException($"Process group '{processGroup}' is not registered.");
        _update(processGroup);
    }
    /// <summary>Registers a MicroBundle as available without loading it.</summary>
    public bool RegisterBundle(IMicroBundle bundle)
    {
        if(bundle is null)throw new ArgumentNullException(nameof(bundle));
        return _availableBundles.TryAdd(bundle.Id,bundle);
    }
    /// <summary>Loads a registered MicroBundle once and invokes its load hook.</summary>
    public bool LoadBundle(IMicroBundle bundle)
    {
        if(bundle is null)throw new ArgumentNullException(nameof(bundle));
        if(_loadedBundles.Any(existing=>existing.Id==bundle.Id)||_loadAttempts.Contains(bundle.Id))return false;
        if(_availableBundles.TryGetValue(bundle.Id,out var registered)&&!ReferenceEquals(registered,bundle))return false;
        _availableBundles.TryAdd(bundle.Id,bundle);
        if(!_loadingBundles.Add(bundle.Id))return false;
        _loadAttempts.Add(bundle.Id);
        try{bundle.LoadBundle(this);_loadedBundles.Add(bundle);return true;}
        finally{_loadingBundles.Remove(bundle.Id);}
    }
    /// <summary>Attempts to load an available bundle by stable identity.</summary>
    public bool TryLoadBundle(ulong bundleId)=>_availableBundles.TryGetValue(bundleId,out var bundle)&&LoadBundle(bundle);
    /// <summary>Attempts to retrieve an installed bundle by stable identity.</summary>
    public bool TryGetLoadedBundle(ulong bundleId,out IMicroBundle? bundle){bundle=_loadedBundles.FirstOrDefault(existing=>existing.Id==bundleId);return bundle is not null;}
    /// <summary>Executes bounded arbitration across all loaded bundles.</summary>
    public int ExecuteArbitrationPipeline()
    {
        var mutations=0;
        for(var round=0;round<MaximumArbitrationRounds;round++)
        {
            var changed=false;
            foreach(var bundle in _loadedBundles.ToArray())
            {
                if(!bundle.Arbitrate(this,round))continue;
                changed=true;mutations++;
                Audit.Record(new ArbitrationEvent(round,bundle.Id,bundle.Ontology.StructuralId,MutationType.StructuralMutation,0));
            }
            if(!changed)break;
        }
        return mutations;
    }
    /// <summary>Attempts to resolve a warehouse identity.</summary>
    public bool TryResolve(ulong identity,out WarehouseAddress address)=>_warehouseIdentities.TryGetValue(identity,out address);
    /// <summary>Maps a compact identity to a warehouse address.</summary>
    public void MapWarehouseIdentity(ulong identity,WarehouseAddress address)=>_warehouseIdentities[identity]=address;
    /// <summary>Registers a process-group identity.</summary>
    public bool Register(ulong id){if(_processGroups.ContainsKey(id))return false;_processGroups[id]=ProcessGroupState.Registered;return true;}
    /// <summary>Activates a registered process-group identity.</summary>
    public bool Activate(ulong id){if(!_processGroups.TryGetValue(id,out var state)||state!=ProcessGroupState.Registered)return false;_processGroups[id]=ProcessGroupState.Active;return true;}
    /// <summary>Completes an active process-group identity.</summary>
    public bool Complete(ulong id){if(!_processGroups.TryGetValue(id,out var state)||state!=ProcessGroupState.Active)return false;_processGroups[id]=ProcessGroupState.Completed;return true;}
    /// <summary>Immutable registration describing a process group and optional parent.</summary>
    public sealed record ProcessGroupRegistration(string Name,string? ParentName)
    {
        /// <summary>Gets the process-group name.</summary>
        public string Name { get; init; } = Name;
        /// <summary>Gets the optional parent process-group name.</summary>
        public string? ParentName { get; init; } = ParentName;
    }
}
/// <summary>In-memory arbitration audit implementation.</summary>
public sealed class ArbitrationAudit:IArbitrationAudit
{
    private readonly List<ArbitrationEvent> _events=new();
    /// <summary>Gets all recorded arbitration events in insertion order.</summary>
    public IReadOnlyList<ArbitrationEvent> Events=>_events;
    /// <summary>Records one arbitration event.</summary>
    public void Record(ArbitrationEvent arbitrationEvent)=>_events.Add(arbitrationEvent);
}
