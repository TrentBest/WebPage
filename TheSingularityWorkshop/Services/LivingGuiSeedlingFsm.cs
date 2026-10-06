using System;
using TheSingularityWorkshop.FSM_API;
using fsm_API = TheSingularityWorkshop.FSM_API.FSM_API;

namespace TheSingularityWorkshop.Services;

/// <summary>
/// Temporary composition FSM for a newly spawned Living GUI organism.
/// A seedling owns the flight and planting phase; once planted, it is removed and
/// the preallocated mature organism FSM is made valid and activated.
/// </summary>
public sealed class LivingGuiSeedlingFsm : IDisposable
{
    public const string TravelingState = "SEEDLING_TRAVELING";
    public const string PlantingState = "SEEDLING_PLANTING";

    private readonly PageStateContext _page;
    private readonly PageStateContext.LivingNodeState _node;
    private readonly FSMHandle _handle;
    private readonly string _processingGroup;
    private bool _disposed;

    public LivingGuiSeedlingFsm(
        PageStateContext page,
        PageStateContext.LivingNodeState node,
        string processingGroup)
    {
        _page = page;
        _node = node;
        _processingGroup = processingGroup;

        var definitionName = DefinitionNameForGroup(processingGroup);
        EnsureDefinition(definitionName, processingGroup);

        Context = new SeedlingContext(page, node);
        _handle = fsm_API.Create.CreateInstance(definitionName, Context, processingGroup);

    }

    private static void EnsureDefinition(string definitionName, string processingGroup)
    {
        if (fsm_API.Interaction.Exists(definitionName, processingGroup))
            return;

        fsm_API.Create.CreateFiniteStateMachine(definitionName, -1, processingGroup)
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
            .WithInitialState(TravelingState)
            .Transition(TravelingState, PlantingState, ctx => GetContext(ctx).IsTravelComplete())
            .BuildDefinition();
    }

    private static SeedlingContext GetContext(IStateContext context)
        => context as SeedlingContext
            ?? throw new InvalidOperationException("Living GUI seedling FSM received an unexpected context type.");

    public static string DefinitionNameForGroup(string processingGroup)
        => $"LivingGuiSeedlingFSM:{processingGroup}";

    public string CurrentState => _handle.CurrentState;
    public string ProcessingGroup => _processingGroup;
    public bool IsValid => !_disposed && Context.IsValid && _handle.IsValid;
    public bool IsPlanted => _node.Phase == PageStateContext.LivingNodePhase.Existing;
    public SeedlingContext Context { get; }

    public void Dispose()
    {
        if (_disposed)
            return;

        fsm_API.Interaction.DestroyInstance(_handle);
        _disposed = true;
    }

    public sealed class SeedlingContext : IStateContext
    {
        private readonly PageStateContext _page;

        internal SeedlingContext(PageStateContext page, PageStateContext.LivingNodeState node)
        {
            _page = page;
            Node = node;
            Name = $"LivingGuiSeedlingContext:{Guid.NewGuid():N}";
        }

        public string Name { get; set; }
        public bool IsValid { get; set; } = true;
        public PageStateContext.LivingNodeState Node { get; }

        public void EnterTraveling()
        {
            Node.Phase = PageStateContext.LivingNodePhase.Traveling;
        }

        public void AdvanceTravel()
        {
            _page.AdvanceTravel(Node);
        }

        public bool IsTravelComplete()
            => _page.IsTravelComplete(Node);

        public void EnterPlanting()
        {
            Node.Phase = PageStateContext.LivingNodePhase.Planting;
            Node.GrowthReady = false;
        }

        public void AdvancePlanting()
        {
            _page.AdvancePlanting(Node);
        }
    }
}
