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
    public const string SeedFlightState = "SEED_FLIGHT";
    public const string ChildRootingState = SeedFlightState;
    public const string SeedScalingState = "SEED_SCALING";
    public const string MatureGrowthState = "MATURE_GROWTH";
    public const string ReproductionState = "REPRODUCTION";
    public const string ParentRecoveryState = "PARENT_RECOVERY";
    public const string IdleState = "IDLE";

    private readonly HubKernel _hub;
    private readonly PageStateContext _context;
    private readonly FSMHandle _handle;
    private readonly FSMHandle _rootGrowthHandle;
    private readonly FSMHandle _seedFlightHandle;
    private readonly FSMHandle _seedScalingHandle;
    private readonly FSMHandle _matureGrowthHandle;
    private readonly FSMHandle _reproductionHandle;
    private readonly FSMHandle _parentRecoveryHandle;
    private readonly string _processingGroup;
    private readonly string _rootGrowthGroup;
    private readonly string _seedFlightGroup;
    private readonly string _seedScalingGroup;
    private readonly string _matureGrowthGroup;
    private readonly string _reproductionGroup;
    private readonly string _parentRecoveryGroup;
    private bool _disposed;

    public LivingGuiFsm(HubKernel hub, PageStateContext context, string parentProcessingGroup)
    {
        _hub = hub;
        _context = context;
        _processingGroup = $"LivingGui:{Guid.NewGuid():N}";
        _rootGrowthGroup = $"LivingGui:RootGrowth:{Guid.NewGuid():N}";
        _seedFlightGroup = $"LivingGui:SeedFlight:{Guid.NewGuid():N}";
        _seedScalingGroup = $"LivingGui:SeedScaling:{Guid.NewGuid():N}";
        _matureGrowthGroup = $"LivingGui:MatureGrowth:{Guid.NewGuid():N}";
        _reproductionGroup = $"LivingGui:Reproduction:{Guid.NewGuid():N}";
        _parentRecoveryGroup = $"LivingGui:ParentRecovery:{Guid.NewGuid():N}";

        fsm_API.Create.CreateProcessingGroup(_processingGroup);
        fsm_API.Create.CreateProcessingGroup(_rootGrowthGroup);
        fsm_API.Create.CreateProcessingGroup(_seedFlightGroup);
        fsm_API.Create.CreateProcessingGroup(_seedScalingGroup);
        fsm_API.Create.CreateProcessingGroup(_matureGrowthGroup);
        fsm_API.Create.CreateProcessingGroup(_reproductionGroup);
        fsm_API.Create.CreateProcessingGroup(_parentRecoveryGroup);

        _hub
            .RegisterProcessGroup(_processingGroup, parentProcessingGroup)
            .RegisterProcessGroup(_rootGrowthGroup, _processingGroup)
            .RegisterProcessGroup(_seedFlightGroup, _processingGroup)
            .RegisterProcessGroup(_seedScalingGroup, _processingGroup)
            .RegisterProcessGroup(_matureGrowthGroup, _processingGroup)
            .RegisterProcessGroup(_reproductionGroup, _processingGroup)
            .RegisterProcessGroup(_parentRecoveryGroup, _processingGroup);

        fsm_API.Create.CreateFiniteStateMachine("LivingGuiFSM", -1, _processingGroup)
            .State(LifecycleState, onEnter: null, onUpdate: null, onExit: null)
            .WithInitialState(LifecycleState)
            .BuildDefinition();

        fsm_API.Create.CreateFiniteStateMachine("LivingGuiRootGrowthFSM", -1, _rootGrowthGroup)
            .State(RootGrowthState, null, _ => _context.AdvanceRootGrowth(), null)
            .WithInitialState(RootGrowthState)
            .BuildDefinition();

        fsm_API.Create.CreateFiniteStateMachine("LivingGuiSeedFlightFSM", -1, _seedFlightGroup)
            .State(SeedFlightState, null, _ => _context.AdvanceSeedFlight(), null)
            .WithInitialState(SeedFlightState)
            .BuildDefinition();

        fsm_API.Create.CreateFiniteStateMachine("LivingGuiSeedScalingFSM", -1, _seedScalingGroup)
            .State(SeedScalingState, null, _ => _context.AdvanceSeedScaling(), null)
            .WithInitialState(SeedScalingState)
            .BuildDefinition();

        fsm_API.Create.CreateFiniteStateMachine("LivingGuiMatureGrowthFSM", -1, _matureGrowthGroup)
            .State(MatureGrowthState, null, _ => _context.AdvanceMatureGrowth(), null)
            .WithInitialState(MatureGrowthState)
            .BuildDefinition();

        fsm_API.Create.CreateFiniteStateMachine("LivingGuiReproductionFSM", -1, _reproductionGroup)
            .State(ReproductionState, null, _ => _context.AdvanceReproduction(), null)
            .WithInitialState(ReproductionState)
            .BuildDefinition();

        fsm_API.Create.CreateFiniteStateMachine("LivingGuiParentRecoveryFSM", -1, _parentRecoveryGroup)
            .State(ParentRecoveryState, null, _ => _context.AdvanceParentRecovery(), null)
            .WithInitialState(ParentRecoveryState)
            .BuildDefinition();

        _handle = fsm_API.Create.CreateInstance("LivingGuiFSM", context, _processingGroup);
        _rootGrowthHandle = fsm_API.Create.CreateInstance("LivingGuiRootGrowthFSM", context, _rootGrowthGroup);
        _seedFlightHandle = fsm_API.Create.CreateInstance("LivingGuiSeedFlightFSM", context, _seedFlightGroup);
        _seedScalingHandle = fsm_API.Create.CreateInstance("LivingGuiSeedScalingFSM", context, _seedScalingGroup);
        _matureGrowthHandle = fsm_API.Create.CreateInstance("LivingGuiMatureGrowthFSM", context, _matureGrowthGroup);
        _reproductionHandle = fsm_API.Create.CreateInstance("LivingGuiReproductionFSM", context, _reproductionGroup);
        _parentRecoveryHandle = fsm_API.Create.CreateInstance("LivingGuiParentRecoveryFSM", context, _parentRecoveryGroup);

        if (!IsValid)
            throw new InvalidOperationException("Living GUI FSM instances were created with an invalid PageStateContext.");
    }

    public string CurrentState => _handle.CurrentState;
    public string ProcessingGroup => _processingGroup;
    public string RootGrowthProcessingGroup => _rootGrowthGroup;
    public string SeedFlightProcessingGroup => _seedFlightGroup;
    public string ChildRootingProcessingGroup => _seedFlightGroup;
    public string SeedScalingProcessingGroup => _seedScalingGroup;
    public string MatureGrowthProcessingGroup => _matureGrowthGroup;
    public string ReproductionProcessingGroup => _reproductionGroup;
    public string ParentRecoveryProcessingGroup => _parentRecoveryGroup;

    public bool IsValid =>
        _context.IsValid &&
        _handle.IsValid &&
        _rootGrowthHandle.IsValid &&
        _seedFlightHandle.IsValid &&
        _seedScalingHandle.IsValid &&
        _matureGrowthHandle.IsValid &&
        _reproductionHandle.IsValid &&
        _parentRecoveryHandle.IsValid;

    /// <summary>
    /// Selects the most urgent lifecycle work first. Reproduction and parent
    /// recovery must outrank newborn flight: a parent begins contracting as soon
    /// as it creates an offspring, rather than waiting behind the child's journey.
    /// Population handoff is not a lifecycle stop: once the first offspring exists,
    /// the GUI remains active until the page explicitly freezes it for gravity.
    /// A frozen or otherwise empty runtime has no active phase and must never
    /// masquerade as parent recovery.
    /// </summary>
    public string ActivePhase
    {
        get
        {
            if (_context.LivingGuiFrozen)
                return IdleState;
            if (_context.NeedsRootGrowth) return RootGrowthState;
            if (_context.NeedsReproduction) return ReproductionState;
            if (_context.NeedsParentRecovery) return ParentRecoveryState;
            if (_context.NeedsSeedFlight) return SeedFlightState;
            if (_context.NeedsSeedScaling) return SeedScalingState;
            if (_context.NeedsMatureGrowth) return MatureGrowthState;
            return IdleState;
        }
    }

    public void Update()
    {
        if (_disposed || !_context.IsValid || _context.LivingGuiFrozen)
            return;

        switch (ActivePhase)
        {
            case RootGrowthState:
                _hub.UpdateProcessGroup(_rootGrowthGroup);
                break;
            case ReproductionState:
                _hub.UpdateProcessGroup(_reproductionGroup);
                break;
            case ParentRecoveryState:
                _hub.UpdateProcessGroup(_parentRecoveryGroup);
                break;
            case SeedFlightState:
                _hub.UpdateProcessGroup(_seedFlightGroup);
                break;
            case SeedScalingState:
                _hub.UpdateProcessGroup(_seedScalingGroup);
                break;
            case MatureGrowthState:
                _hub.UpdateProcessGroup(_matureGrowthGroup);
                break;
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        fsm_API.Interaction.DestroyFiniteStateMachine("LivingGuiFSM", _processingGroup);
        fsm_API.Interaction.DestroyFiniteStateMachine("LivingGuiRootGrowthFSM", _rootGrowthGroup);
        fsm_API.Interaction.DestroyFiniteStateMachine("LivingGuiSeedFlightFSM", _seedFlightGroup);
        fsm_API.Interaction.DestroyFiniteStateMachine("LivingGuiSeedScalingFSM", _seedScalingGroup);
        fsm_API.Interaction.DestroyFiniteStateMachine("LivingGuiMatureGrowthFSM", _matureGrowthGroup);
        fsm_API.Interaction.DestroyFiniteStateMachine("LivingGuiReproductionFSM", _reproductionGroup);
        fsm_API.Interaction.DestroyFiniteStateMachine("LivingGuiParentRecoveryFSM", _parentRecoveryGroup);
        _disposed = true;
    }
}