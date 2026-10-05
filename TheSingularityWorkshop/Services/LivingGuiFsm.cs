using System;
using System.Collections.Generic;
using System.Linq;
using TheSingularityWorkshop.FSM_API;
using HubKernel = TheSingularityWorkshop.SingularityHub.SingularityHub;
using fsm_API = TheSingularityWorkshop.FSM_API.FSM_API;

namespace TheSingularityWorkshop.Services;

/// <summary>
/// Runtime manager for the Living GUI population.
/// Each organism owns an independent LivingGuiOrganismFsm. This manager only
/// attaches new organisms, ticks every organism once per frame, and owns disposal.
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
    private readonly HashSet<PageStateContext.LivingNodeState> _attachedNodes = new();
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
        _context.IsValid &&
        _organisms.All(organism => organism.IsValid);

    public int OrganismCount => _organisms.Count;

    public IReadOnlyList<LivingGuiOrganismFsm> Organisms => _organisms;

    public void Update()
    {
        if (_disposed || !_context.IsValid || _context.LivingGuiFrozen)
            return;

        // Attachment is the only orchestration performed here. FSM_API owns
        // execution through the Hub heartbeat; each organism's processing group
        // is independently advanced exactly once by that heartbeat.
        AttachUntrackedOrganisms();
    }

    private void AttachUntrackedOrganisms()
    {
        foreach (var node in _context.LivingNodes)
        {
            if (!_attachedNodes.Add(node))
                continue;

            _organisms.Add(new LivingGuiOrganismFsm(
                _hub,
                _context,
                node,
                _processingGroup,
                AttachChild));
        }
    }

    private void AttachChild(PageStateContext.LivingNodeState child)
    {
        if (_attachedNodes.Contains(child))
            return;

        _attachedNodes.Add(child);
        _organisms.Add(new LivingGuiOrganismFsm(
            _hub,
            _context,
            child,
            _processingGroup,
            AttachChild));
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        foreach (var organism in _organisms)
            organism.Dispose();

        _organisms.Clear();
        _attachedNodes.Clear();

        fsm_API.Interaction.DestroyFiniteStateMachine("LivingGuiFSM", _processingGroup);
        _disposed = true;
    }
}