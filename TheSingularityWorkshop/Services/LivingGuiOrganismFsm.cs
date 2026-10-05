using System;
using TheSingularityWorkshop.FSM_API;
using fsm_API = TheSingularityWorkshop.FSM_API.FSM_API;

namespace TheSingularityWorkshop.Services;

/// <summary>
/// One independent FSM_API instance for one Living GUI organism. The FSM definition is
/// shared because FSM_API defines behavior once and instantiates it many times; each
/// organism still owns a distinct FSMHandle and a distinct organism context.
/// </summary>
public sealed class LivingGuiOrganismFsm : IDisposable
{
    public const string DormantState = "DORMANT";
    public const string InitializationState = "INITIALIZATION";
    public const string TravelingState = "TRAVELING";
    public const string PlantingState = "PLANTING";
    public const string ExistingState = "EXISTING";
    public const string ReproducingState = "REPRODUCING";
    public const string ReducingState = "REDUCING";

    private readonly LivingGuiOrganismContext _organismContext;
    private readonly string _processingGroup;
    private readonly string _fsmName;
    private readonly FSMHandle _handle;
    private bool _disposed;

    public LivingGuiOrganismFsm(
        PageStateContext context,
        PageStateContext.LivingNodeState node,
        string parentProcessingGroup,
        Action<PageStateContext.LivingNodeState> attachChild)
    {
        _processingGroup = parentProcessingGroup;
        _fsmName = DefinitionNameForGroup(parentProcessingGroup);
        _organismContext = new LivingGuiOrganismContext(context, node, attachChild);

        EnsureDefinition(_fsmName, _processingGroup);

        _handle = fsm_API.Create.CreateInstance(_fsmName, _organismContext, _processingGroup);

        if (!IsValid)
            throw new InvalidOperationException("Living GUI organism FSM was created with an invalid context or handle.");
    }

    public static string DefinitionNameForGroup(string processingGroup)
        => $"LivingGuiOrganismFSM:{processingGroup}";

    private static void EnsureDefinition(string fsmName, string processingGroup)
    {
        if (fsm_API.Interaction.Exists(fsmName, processingGroup))
            return;

        fsm_API.Create.CreateFiniteStateMachine(fsmName, -1, processingGroup)
            .State(
                DormantState,
                onEnter: null,
                onUpdate: null,
                onExit: null)
            .State(
                InitializationState,
                onEnter: ctx => GetContext(ctx).Initialize(),
                onUpdate: null,
                onExit: null)
            .State(
                TravelingState,
                onEnter: ctx => GetContext(ctx).EnterTraveling(),
                onUpdate: ctx => GetContext(ctx).AdvanceTravel(),
                onExit: null)
            .State(
                PlantingState,
                onEnter: ctx => GetContext(ctx).EnterPlanting(),
                onUpdate: ctx => GetContext(ctx).AdvancePlanting(),
                onExit: null)
            .State(
                ExistingState,
                onEnter: ctx => GetContext(ctx).EnterExisting(),
                onUpdate: ctx => GetContext(ctx).AdvanceExisting(),
                onExit: null)
            .State(
                ReproducingState,
                onEnter: ctx => GetContext(ctx).EnterReproducing(),
                onUpdate: ctx => GetContext(ctx).AdvanceReproduction(),
                onExit: null)
            .State(
                ReducingState,
                onEnter: ctx => GetContext(ctx).EnterReducing(),
                onUpdate: ctx => GetContext(ctx).AdvanceReducing(),
                onExit: null)
            .WithInitialState(DormantState)
            .Transition(DormantState, InitializationState, ctx => GetContext(ctx).Node.Phase != PageStateContext.LivingNodePhase.Dormant)
            .Transition(InitializationState, ExistingState, ctx => GetContext(ctx).Node.IsRoot)
            .Transition(InitializationState, TravelingState, ctx => !GetContext(ctx).Node.IsRoot)
            .Transition(TravelingState, PlantingState, ctx => GetContext(ctx).IsTravelComplete())
            .Transition(PlantingState, ExistingState, ctx => GetContext(ctx).IsPlantingComplete())
            .Transition(ExistingState, ReproducingState, ctx => GetContext(ctx).IsExistingComplete())
            .Transition(ReproducingState, ReducingState, ctx => GetContext(ctx).IsReproductionComplete())
            .Transition(ReducingState, ExistingState, ctx => GetContext(ctx).IsReducingComplete())
            .BuildDefinition();
    }

    private static LivingGuiOrganismContext GetContext(IStateContext context)
        => context as LivingGuiOrganismContext
            ?? throw new InvalidOperationException("Living GUI organism FSM received an unexpected context type.");

    /// <summary>
    /// Activates this preallocated organism by transitioning its own FSM from DORMANT
    /// into INITIALIZATION. The transition executes the organism's initialization action;
    /// the shared processing group remains responsible for subsequent lifecycle ticks.
    /// </summary>
    public void Activate()
    {
        if (_disposed || !_handle.IsValid)
            throw new InvalidOperationException("Cannot activate an invalid Living GUI organism FSM.");

        if (_handle.CurrentState == DormantState)
            _handle.TransitionTo(InitializationState);
    }

    public PageStateContext.LivingNodeState Node => _organismContext.Node;
    public string CurrentState => _handle.CurrentState;
    public string ProcessingGroup => _processingGroup;
    public bool IsValid => !_disposed && _organismContext.IsValid && _handle.IsValid;

    public void Dispose()
    {
        if (_disposed)
            return;

        fsm_API.Interaction.DestroyInstance(_handle);
        _disposed = true;
    }

    private sealed class LivingGuiOrganismContext : IStateContext
    {
        private readonly PageStateContext _page;
        private readonly Action<PageStateContext.LivingNodeState> _attachChild;
        private bool _isValid = true;

        public LivingGuiOrganismContext(
            PageStateContext page,
            PageStateContext.LivingNodeState node,
            Action<PageStateContext.LivingNodeState> attachChild)
        {
            _page = page;
            Node = node;
            _attachChild = attachChild;
            Name = $"LivingGuiOrganismContext:{Guid.NewGuid():N}";
        }

        public string Name { get; set; }
        public bool IsValid
        {
            get => _isValid && _page.IsValid;
            set => _isValid = value;
        }

        public PageStateContext.LivingNodeState Node { get; }

        public void Initialize() => _page.InitializeOrganism(Node);

        public void EnterTraveling()
            => Node.Phase = PageStateContext.LivingNodePhase.Traveling;

        public void AdvanceTravel() => _page.AdvanceTravel(Node);

        public bool IsTravelComplete() => _page.IsTravelComplete(Node);

        public void EnterPlanting()
        {
            Node.Phase = PageStateContext.LivingNodePhase.Planting;
            Node.GrowthReady = false;
        }

        public void AdvancePlanting() => _page.AdvancePlanting(Node);

        public bool IsPlantingComplete() => _page.IsPlantingComplete(Node);

        public void EnterExisting()
        {
            Node.Phase = PageStateContext.LivingNodePhase.Existing;
            Node.ReproductionComplete = false;
        }

        public void AdvanceExisting() => _page.AdvanceExisting(Node);

        public bool IsExistingComplete() => _page.IsExistingComplete(Node);

        public void EnterReproducing()
            => Node.Phase = PageStateContext.LivingNodePhase.Reproducing;

        public void AdvanceReproduction()
        {
            if (Node.ReproductionComplete)
                return;

            if (_page.LivingNodes.Count >= PageStateContext.PopulationObservationThreshold)
            {
                _page.MarkReproductionComplete(Node);
                return;
            }

            var child = _page.CreateOffspring(Node);
            _attachChild(child);
            _page.MarkReproductionComplete(Node);
        }

        public bool IsReproductionComplete() => _page.IsReproductionComplete(Node);

        public void EnterReducing()
        {
            Node.Phase = PageStateContext.LivingNodePhase.Reducing;
            Node.ReproductionComplete = false;
        }

        public void AdvanceReducing() => _page.AdvanceReducing(Node);

        public bool IsReducingComplete() => _page.IsReducingComplete(Node);
    }
}
