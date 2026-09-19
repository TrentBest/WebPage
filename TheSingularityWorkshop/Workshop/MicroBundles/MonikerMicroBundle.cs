using System;
using System.Collections.Generic;
using TheSingularityWorkshop.FSM_API;
using TheSingularityWorkshop.SingularityHub;
using HubBundle = TheSingularityWorkshop.SingularityHub.IMicroBundle;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// Reusable MicroBundle providing the Workshop moniker as a safe semantic capability.
/// The bundle owns lifecycle through FSM_API; a host may choose any visual manifestation.
/// </summary>
public sealed class MonikerMicroBundle : IMicroBundle, HubBundle, IDisposable
{
    public const int BundleId = 2110;
    public const string DefaultText = "THE SINGULARITY WORKSHOP";
    public const string FsmNamePrefix = "WorkshopMonikerFSM";
    public const string ProcessingGroupPrefix = "WorkshopMoniker";

    private readonly string _fsmName = $"{FsmNamePrefix}_{Guid.NewGuid():N}";
    private readonly string _processingGroup = $"{ProcessingGroupPrefix}_{Guid.NewGuid():N}";
    private readonly FSMHandle _fsm;
    private bool _disposed;

    /// <summary>Creates the default moniker manifestation with no host-specific initialization required.</summary>
    public MonikerMicroBundle(string text = DefaultText)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Moniker text cannot be empty.", nameof(text));

        Text = text;
        Context = new MonikerContext(text);

        FSM_API.FSM_API.Create.CreateProcessingGroup(_processingGroup);
        FSM_API.FSM_API.Create.CreateFiniteStateMachine(_fsmName, -1, _processingGroup)
            .State("Created", onEnter: EnterCreated, onUpdate: _ => { }, onExit: _ => { })
            .State("Ready", onEnter: EnterReady, onUpdate: _ => { }, onExit: _ => { })
            .State("Presenting", onEnter: EnterPresenting, onUpdate: _ => { }, onExit: _ => { })
            .State("Removing", onEnter: EnterRemoving, onUpdate: _ => { }, onExit: _ => { })
            .State("Destroyed", onEnter: EnterDestroyed, onUpdate: _ => { }, onExit: _ => { })
            .Transition("Created", "Ready", _ => true)
            .Transition("Ready", "Presenting", c => ((MonikerContext)c).PresentationRequested)
            .Transition("Presenting", "Removing", c => ((MonikerContext)c).RemovalRequested)
            .Transition("Removing", "Ready", c => !((MonikerContext)c).RemovalRequested)
            .WithInitialState("Created")
            .BuildDefinition();

        _fsm = FSM_API.FSM_API.Create.CreateInstance(_fsmName, Context, _processingGroup);

        // Establish the safe default immediately. Consumers never need to know how
        // to initialize the bundle before they can obtain its provided capability.
        FSM_API.FSM_API.Interaction.Update(_processingGroup);
    }

    /// <summary>Stable integer identity used by compact Workshop protocols.</summary>
    public int Id => BundleId;

    /// <summary>Stable Hub identity projection.</summary>
    ulong HubBundle.Id => checked((ulong)Id);

    /// <summary>Semantic moniker text supplied by this bundle.</summary>
    public string Text { get; }

    /// <summary>Runtime context carried by this bundle's FSM.</summary>
    public IStateContext Context { get; }

    /// <summary>Current FSM state, suitable for lightweight state queries.</summary>
    public string Status => _fsm.CurrentState;

    /// <summary>Whether the bundle has reached its safe default state.</summary>
    public bool IsReady => Status == "Ready";

    /// <summary>Hub-facing ontology projection for the reusable moniker capability.</summary>
    OntologySignature HubBundle.Ontology => new(0, 0, 0, 0, 0, 0, 0, 0, BundleId);

    /// <summary>Hub-facing bundle version.</summary>
    BundleVersion HubBundle.Version => new(1, 0, 0);

    /// <summary>The moniker has no required MicroBundle dependencies.</summary>
    IReadOnlyList<ulong> HubBundle.Dependencies => Array.Empty<ulong>();

    /// <summary>Requests presentation without prescribing the host's visual implementation.</summary>
    public void Present()
    {
        if (_disposed) return;
        ((MonikerContext)Context).PresentationRequested = true;
    }

    /// <summary>Requests removal of the current presentation.</summary>
    public void Remove()
    {
        if (_disposed) return;
        ((MonikerContext)Context).RemovalRequested = true;
    }

    /// <summary>Advances the bundle's FSM lifecycle.</summary>
    public void Update()
    {
        if (_disposed) return;
        FSM_API.FSM_API.Interaction.Update(_processingGroup);
    }

    /// <summary>Participates in Hub arbitration without exposing implementation details.</summary>
    void HubBundle.LoadBundle(IArbitrator arbitrator)
    {
        ArgumentNullException.ThrowIfNull(arbitrator);
    }

    bool HubBundle.Arbitrate(IArbitrator arbitrator, int roundIndex)
    {
        ArgumentNullException.ThrowIfNull(arbitrator);
        return roundIndex >= 0 && !_disposed;
    }

    public void Dispose()
    {
        if (_disposed) return;
        FSM_API.FSM_API.Interaction.DestroyFiniteStateMachine(_fsmName, _processingGroup);
        _disposed = true;
    }

    private static void EnterCreated(IStateContext context)
        => ((MonikerContext)context).Phase = "Created";

    private static void EnterReady(IStateContext context)
        => ((MonikerContext)context).Phase = "Ready";

    private static void EnterPresenting(IStateContext context)
        => ((MonikerContext)context).Phase = "Presenting";

    private static void EnterRemoving(IStateContext context)
        => ((MonikerContext)context).Phase = "Removing";

    private static void EnterDestroyed(IStateContext context)
        => ((MonikerContext)context).Phase = "Destroyed";

    private sealed class MonikerContext : IStateContext
    {
        public MonikerContext(string text)
        {
            Name = "Workshop Moniker";
            Text = text;
        }

        public string Name { get; set; }
        public bool IsValid { get; set; } = true;
        public string Text { get; }
        public string Phase { get; set; } = "Created";
        public bool PresentationRequested { get; set; }
        public bool RemovalRequested { get; set; }
    }
}
