using System;
using System.Collections.Generic;
using System.Linq;
using TheSingularityWorkshop.FSM_API;
using HubKernel = TheSingularityWorkshop.SingularityHub.SingularityHub;
using fsm_API = TheSingularityWorkshop.FSM_API.FSM_API;

namespace TheSingularityWorkshop.Services;

/// <summary>
/// Runtime manager for the Living GUI population.
/// Each organism owns an independent LivingGuiOrganismFsm. All organism FSM instances
/// share one processing group; this manager activates pool slots and advances that group once per heartbeat.
/// </summary>
public sealed class LivingGuiFsm : IDisposable
{
    public const string LifecycleState = "ALIVE";
    public const string RootGrowthState = LivingGuiOrganismFsm.ExistingState;
    public const string SeedFlightState = LivingGuiOrganismFsm.TravelingState;
    public const string ChildRootingState = SeedFlightState;
    public const string SeedScalingState = LivingGuiOrganismFsm.PlantingState;
    public const string MatureGrowthState = LivingGuiOrganismFsm.ExistingState;
    public const string ReproductionState = LivingGuiOrganismFsm.ReproducingState;
    public const string ParentRecoveryState = LivingGuiOrganismFsm.ReducingState;
    public const string IdleState = "IDLE";

    private readonly HubKernel _hub;
    private readonly PageStateContext _context;
    private readonly string _processingGroup;
    private readonly List<LivingGuiOrganismFsm> _organisms = new();
    private readonly Dictionary<PageStateContext.LivingNodeState, LivingGuiOrganismFsm> _organismPool = new();
    private readonly HashSet<PageStateContext.LivingNodeState> _attachedNodes = new();
    private readonly Dictionary<PageStateContext.LivingNodeState, LivingGuiSeedlingFsm> _seedlings = new();
    private bool _disposed;

    public LivingGuiFsm(HubKernel hub, PageStateContext context, string parentProcessingGroup)
    {
        _hub = hub;
        _context = context;
        _processingGroup = $"LivingGui:{Guid.NewGuid():N}";

        fsm_API.Create.CreateProcessingGroup(_processingGroup);
        _hub.RegisterProcessGroup(_processingGroup, parentProcessingGroup);

        fsm_API.Create.CreateFiniteStateMachine("LivingGuiFSM", -1, _processingGroup)
            .State(LifecycleState, onEnter: null, onUpdate: null, onExit: null)
            .WithInitialState(LifecycleState)
            .BuildDefinition();

        fsm_API.Create.CreateInstance("LivingGuiFSM", context, _processingGroup);

        // Allocate the full organism/FSM population once. Reproduction later only
        // activates an already-existing slot; it never news another FSM instance or processing group.
        foreach (var node in _context.PreallocatedLivingNodes)
        {
            _organismPool[node] = new LivingGuiOrganismFsm(
                _context,
                node,
                _processingGroup,
                AttachChild);
        }
    }

    public string CurrentState => LifecycleState;
    public string ProcessingGroup => _processingGroup;
    public string RootGrowthProcessingGroup => _processingGroup;
    public string SeedFlightProcessingGroup => _processingGroup;
    public string ChildRootingProcessingGroup => _processingGroup;
    public string SeedScalingProcessingGroup => _processingGroup;
    public string MatureGrowthProcessingGroup => _processingGroup;
    public string ReproductionProcessingGroup => _processingGroup;
    public string ParentRecoveryProcessingGroup => _processingGroup;
    public string ActivePhase =>
        _organisms.Count == 0
            ? IdleState
            : string.Join(", ", _organisms
                .Take(8)
                .Select(organism => organism.CurrentState));

    public bool IsValid =>
        !_disposed &&
        _context.IsValid &&
        _organismPool.Count == _context.PreallocatedLivingNodes.Count;

    public int OrganismCount => _organisms.Count;
    public int PreallocatedOrganismCount => _organismPool.Count;
    public int SeedlingCount => _seedlings.Count;

    /// <summary>Raised after the independently ticking population has advanced.</summary>
    public event Action? PopulationChanged;

    public IReadOnlyList<LivingGuiOrganismFsm> Organisms => _organisms;

    public void Update()
    {
        if (_disposed || !_context.IsValid || _context.LivingGuiFrozen)
            return;

        // Activate newly acquired pool slots before advancing the population.
        // Every organism still owns its own FSM_API instance and lifecycle state;
        // the processing group is the shared scheduling boundary.
        AttachUntrackedOrganisms();

        // One group update advances every FSM instance registered in this population.
        // Do not update organism instances individually: doing so would step the same
        // shared processing group repeatedly in one heartbeat.
        _hub.UpdateProcessGroup(_processingGroup);

        // A seedling is a temporary composition layer. Once its own FSM has
        // completed planting, remove it and authorize the preallocated mature
        // organism FSM to become valid and take over the lifecycle.
        foreach (var pair in _seedlings.ToArray())
        {
            if (!pair.Value.IsPlanted)
                continue;

            pair.Value.Dispose();
            _seedlings.Remove(pair.Key);

            if (_organismPool.TryGetValue(pair.Key, out var organism))
                organism.Activate();
        }

        // This is an observation signal only. FSM_API remains the authority that
        // changed each organism; the event merely tells the presentation boundary
        // that its bound state is ready to be rendered.
        PopulationChanged?.Invoke();
    }

    private void AttachUntrackedOrganisms()
    {
        foreach (var node in _context.LivingNodes)
        {
            if (!_attachedNodes.Add(node))
                continue;

            if (!_organismPool.TryGetValue(node, out var organism))
                throw new InvalidOperationException("A Living GUI node has no preallocated organism FSM.");

            organism.Activate();
            _organisms.Add(organism);
        }
    }

    private void AttachChild(PageStateContext.LivingNodeState child)
    {
        if (!_attachedNodes.Add(child))
            return;

        if (!_organismPool.TryGetValue(child, out var organism))
            throw new InvalidOperationException("A Living GUI child has no preallocated organism FSM.");

        _organisms.Add(organism);
        _seedlings.Add(child, new LivingGuiSeedlingFsm(_context, child, _processingGroup));
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        foreach (var seedling in _seedlings.Values)
            seedling.Dispose();

        foreach (var organism in _organismPool.Values)
            organism.Dispose();

        _seedlings.Clear();
        _organisms.Clear();
        _attachedNodes.Clear();
        _organismPool.Clear();

        fsm_API.Interaction.DestroyFiniteStateMachine(
            LivingGuiOrganismFsm.DefinitionNameForGroup(_processingGroup),
            _processingGroup);
        fsm_API.Interaction.DestroyFiniteStateMachine("LivingGuiFSM", _processingGroup);
        _disposed = true;
    }
}