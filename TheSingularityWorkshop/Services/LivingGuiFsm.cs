using System;
using TheSingularityWorkshop.FSM_API;
using HubKernel = TheSingularityWorkshop.SingularityHub.SingularityHub;
using fsm_API = TheSingularityWorkshop.FSM_API.FSM_API;

namespace TheSingularityWorkshop.Services;

/// <summary>
/// Hierarchical runtime for the Living GUI.
/// The Living GUI owns its lifecycle, while each phase has its own FSM and
/// process group. The parent page decides when the Living GUI is allowed to run.
/// </summary>
public sealed class LivingGuiFsm : IDisposable
{
    public const string LifecycleState = "ALIVE";
    public const string RootGrowthState = "ROOT_GROWTH";
    public const string ChildRootingState = "CHILD_ROOTING";
    public const string MatureGrowthState = "MATURE_GROWTH";
    public const string ReproductionState = "REPRODUCTION";

    private readonly HubKernel _hub;
    private readonly PageStateContext _context;
    private readonly FSMHandle _handle;
    private readonly FSMHandle _rootGrowthHandle;
    private readonly FSMHandle _childRootingHandle;
    private readonly FSMHandle _matureGrowthHandle;
    private readonly FSMHandle _reproductionHandle;
    private readonly string _processingGroup;
    private readonly string _rootGrowthGroup;
    private readonly string _childRootingGroup;
    private readonly string _matureGrowthGroup;
    private readonly string _reproductionGroup;
    private bool _disposed;

    public LivingGuiFsm(HubKernel hub, PageStateContext context, string parentProcessingGroup)
    {
        _hub = hub;
        _context = context;
        _processingGroup = $"LivingGui:{Guid.NewGuid():N}";
        _rootGrowthGroup = $"LivingGui:RootGrowth:{Guid.NewGuid():N}";
        _childRootingGroup = $"LivingGui:ChildRooting:{Guid.NewGuid():N}";
        _matureGrowthGroup = $"LivingGui:MatureGrowth:{Guid.NewGuid():N}";
        _reproductionGroup = $"LivingGui:Reproduction:{Guid.NewGuid():N}";

        fsm_API.Create.CreateProcessingGroup(_processingGroup);
        fsm_API.Create.CreateProcessingGroup(_rootGrowthGroup);
        fsm_API.Create.CreateProcessingGroup(_childRootingGroup);
        fsm_API.Create.CreateProcessingGroup(_matureGrowthGroup);
        fsm_API.Create.CreateProcessingGroup(_reproductionGroup);

        _hub
            .RegisterProcessGroup(_processingGroup, parentProcessingGroup)
            .RegisterProcessGroup(_rootGrowthGroup, _processingGroup)
            .RegisterProcessGroup(_childRootingGroup, _processingGroup)
            .RegisterProcessGroup(_matureGrowthGroup, _processingGroup)
            .RegisterProcessGroup(_reproductionGroup, _processingGroup);

        fsm_API.Create.CreateFiniteStateMachine("LivingGuiFSM", -1, _processingGroup)
            .State(LifecycleState, onEnter: null, onUpdate: null, onExit: null)
            .WithInitialState(LifecycleState)
            .BuildDefinition();

        fsm_API.Create.CreateFiniteStateMachine("LivingGuiRootGrowthFSM", -1, _rootGrowthGroup)
            .State(RootGrowthState, null, _ => _context.AdvanceRootGrowth(), null)
            .WithInitialState(RootGrowthState)
            .BuildDefinition();

        fsm_API.Create.CreateFiniteStateMachine("LivingGuiChildRootingFSM", -1, _childRootingGroup)
            .State(ChildRootingState, null, _ => _context.AdvanceChildRooting(), null)
            .WithInitialState(ChildRootingState)
            .BuildDefinition();

        fsm_API.Create.CreateFiniteStateMachine("LivingGuiMatureGrowthFSM", -1, _matureGrowthGroup)
            .State(MatureGrowthState, null, _ => _context.AdvanceMatureGrowth(), null)
            .WithInitialState(MatureGrowthState)
            .BuildDefinition();

        fsm_API.Create.CreateFiniteStateMachine("LivingGuiReproductionFSM", -1, _reproductionGroup)
            .State(ReproductionState, null, _ => _context.AdvanceReproduction(), null)
            .WithInitialState(ReproductionState)
            .BuildDefinition();

        _handle = fsm_API.Create.CreateInstance("LivingGuiFSM", context, _processingGroup);
        _rootGrowthHandle = fsm_API.Create.CreateInstance("LivingGuiRootGrowthFSM", context, _rootGrowthGroup);
        _childRootingHandle = fsm_API.Create.CreateInstance("LivingGuiChildRootingFSM", context, _childRootingGroup);
        _matureGrowthHandle = fsm_API.Create.CreateInstance("LivingGuiMatureGrowthFSM", context, _matureGrowthGroup);
        _reproductionHandle = fsm_API.Create.CreateInstance("LivingGuiReproductionFSM", context, _reproductionGroup);

        // FSM_API refuses to process an instance whose context is invalid.
        // Fail immediately at composition time rather than allowing a visually
        // dead Living GUI to masquerade as a running FSM.
        if (!IsValid)
            throw new InvalidOperationException("Living GUI FSM instances were created with an invalid PageStateContext.");
    }

    public string CurrentState => _handle.CurrentState;
    public string ProcessingGroup => _processingGroup;
    public string RootGrowthProcessingGroup => _rootGrowthGroup;
    public string ChildRootingProcessingGroup => _childRootingGroup;
    public string MatureGrowthProcessingGroup => _matureGrowthGroup;
    public string ReproductionProcessingGroup => _reproductionGroup;

    /// <summary>
    /// True only when the Living GUI context and every live phase instance are valid.
    /// This is intentionally derived from the actual FSM handles rather than duplicated state.
    /// </summary>
    public bool IsValid =>
        _context.IsValid &&
        _handle.IsValid &&
        _rootGrowthHandle.IsValid &&
        _childRootingHandle.IsValid &&
        _matureGrowthHandle.IsValid &&
        _reproductionHandle.IsValid;

    public string ActivePhase
    {
        get
        {
            if (_context.NeedsRootGrowth) return RootGrowthState;
            if (_context.NeedsChildRooting) return ChildRootingState;
            if (_context.NeedsMatureGrowth) return MatureGrowthState;
            return ReproductionState;
        }
    }

    /// <summary>
    /// Ticks exactly one phase process group. The page heartbeat is the only
    /// caller allowed to open this gate, so the Living GUI is inert until the
    /// user enters it and the outer FSM reaches its population state.
    /// </summary>
    public void Update()
    {
        if (_disposed || !_context.IsValid || _context.LivingGuiFrozen)
            return;

        switch (ActivePhase)
        {
            case RootGrowthState:
                _hub.UpdateProcessGroup(_rootGrowthGroup);
                break;
            case ChildRootingState:
                _hub.UpdateProcessGroup(_childRootingGroup);
                break;
            case MatureGrowthState:
                _hub.UpdateProcessGroup(_matureGrowthGroup);
                break;
            default:
                _hub.UpdateProcessGroup(_reproductionGroup);
                break;
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        fsm_API.Interaction.DestroyFiniteStateMachine("LivingGuiFSM", _processingGroup);
        fsm_API.Interaction.DestroyFiniteStateMachine("LivingGuiRootGrowthFSM", _rootGrowthGroup);
        fsm_API.Interaction.DestroyFiniteStateMachine("LivingGuiChildRootingFSM", _childRootingGroup);
        fsm_API.Interaction.DestroyFiniteStateMachine("LivingGuiMatureGrowthFSM", _matureGrowthGroup);
        fsm_API.Interaction.DestroyFiniteStateMachine("LivingGuiReproductionFSM", _reproductionGroup);
        _disposed = true;
    }
}
