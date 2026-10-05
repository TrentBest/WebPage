using System;
using TheSingularityWorkshop.FSM_API;
using HubKernel = TheSingularityWorkshop.SingularityHub.SingularityHub;
using fsm_API = TheSingularityWorkshop.FSM_API.FSM_API;

namespace TheSingularityWorkshop.Services;

/// <summary>
/// One independently ticking FSM for one Living GUI organism.
/// The organism owns its own lifecycle; the population never chooses a global phase.
/// </summary>
public sealed class LivingGuiOrganismFsm : IDisposable
{
    public const string InitializationState = "INITIALIZATION";
    public const string TravelingState = "TRAVELING";
    public const string PlantingState = "PLANTING";
    public const string ExistingState = "EXISTING";
    public const string ReproducingState = "REPRODUCING";
    public const string ReducingState = "REDUCING";

    private readonly HubKernel _hub;
    private readonly PageStateContext _context;
    private readonly PageStateContext.LivingNodeState _node;
    private readonly Action<PageStateContext.LivingNodeState> _attachChild;
    private readonly string _processingGroup;
    private readonly string _fsmName;
    private readonly FSMHandle _handle;
    private bool _disposed;

    public LivingGuiOrganismFsm(
        HubKernel hub,
        PageStateContext context,
        PageStateContext.LivingNodeState node,
        string parentProcessingGroup,
        Action<PageStateContext.LivingNodeState> attachChild)
    {
        _hub = hub;
        _context = context;
        _node = node;
        _attachChild = attachChild;
        _processingGroup = $"LivingGui:Organism:{Guid.NewGuid():N}";
        _fsmName = $"LivingGuiOrganismFSM:{Guid.NewGuid():N}";

        fsm_API.Create.CreateProcessingGroup(_processingGroup);
        _hub.RegisterProcessGroup(_processingGroup, parentProcessingGroup);

        fsm_API.Create.CreateFiniteStateMachine(_fsmName, -1, _processingGroup)
            .State(
                InitializationState,
                onEnter: _ => _context.InitializeOrganism(_node),
                onUpdate: null,
                onExit: null)
            .State(
                TravelingState,
                onEnter: _ => _node.Phase = PageStateContext.LivingNodePhase.Traveling,
                onUpdate: _ => _context.AdvanceTravel(_node),
                onExit: null)
            .State(
                PlantingState,
                onEnter: _ =>
                {
                    _node.Phase = PageStateContext.LivingNodePhase.Planting;
                    _node.GrowthReady = false;
                },
                onUpdate: _ => _context.AdvancePlanting(_node),
                onExit: null)
            .State(
                ExistingState,
                onEnter: _ =>
                {
                    _node.Phase = PageStateContext.LivingNodePhase.Existing;
                    _node.ReproductionComplete = false;
                },
                onUpdate: _ => _context.AdvanceExisting(_node),
                onExit: null)
            .State(
                ReproducingState,
                onEnter: _ => _node.Phase = PageStateContext.LivingNodePhase.Reproducing,
                onUpdate: _ =>
                {
                    if (_node.ReproductionComplete)
                        return;

                    if (_context.LivingNodes.Count >= PageStateContext.PopulationObservationThreshold)
                    {
                        _context.MarkReproductionComplete(_node);
                        return;
                    }

                    var child = _context.CreateOffspring(_node);
                    _attachChild(child);
                    _context.MarkReproductionComplete(_node);
                },
                onExit: null)
            .State(
                ReducingState,
                onEnter: _ =>
                {
                    _node.Phase = PageStateContext.LivingNodePhase.Reducing;
                    _node.ReproductionComplete = false;
                },
                onUpdate: _ => _context.AdvanceReducing(_node),
                onExit: null)
            .WithInitialState(InitializationState)
            .Transition(InitializationState, ExistingState, _ => _node.IsRoot)
            .Transition(InitializationState, TravelingState, _ => !_node.IsRoot)
            .Transition(TravelingState, PlantingState, _ => _context.IsTravelComplete(_node))
            .Transition(PlantingState, ExistingState, _ => _context.IsPlantingComplete(_node))
            .Transition(ExistingState, ReproducingState, _ => _context.IsExistingComplete(_node))
            .Transition(ReproducingState, ReducingState, _ => _context.IsReproductionComplete(_node))
            .Transition(ReducingState, ExistingState, _ => _context.IsReducingComplete(_node))
            .BuildDefinition();

        _handle = fsm_API.Create.CreateInstance(_fsmName, context, _processingGroup);

        if (!IsValid)
            throw new InvalidOperationException("Living GUI organism FSM was created with an invalid context or handle.");
    }

    public PageStateContext.LivingNodeState Node => _node;
    public string CurrentState => _handle.CurrentState;
    public string ProcessingGroup => _processingGroup;
    public bool IsValid => _context.IsValid && _handle.IsValid;

    public void Update()
    {
        if (_disposed || !_context.IsValid || _context.LivingGuiFrozen)
            return;

        _hub.UpdateProcessGroup(_processingGroup);
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        fsm_API.Interaction.DestroyFiniteStateMachine(_fsmName, _processingGroup);
        _disposed = true;
    }
}
