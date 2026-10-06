using System;
using System.Collections.Generic;
using TheSingularityWorkshop.FSM_API;
using TheSingularityWorkshop.MicroBundleDomain;
using TheSingularityWorkshop.SingularityHub;
using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// Reusable MicroBundle providing the Workshop moniker as a safe semantic capability.
/// The same bundle is now consumable by the FSM_COS runtime composition boundary.
/// </summary>
public sealed class MonikerMicroBundle :
    TheSingularityWorkshop.MicroBundleDomain.IMicroBundle,
    TheSingularityWorkshop.SingularityHub.IMicroBundle,
    IDisposable
{
    public const int BundleId = 2110;
    public const string DefaultText = "THE SINGULARITY WORKSHOP";
    public const string FsmNamePrefix = "WorkshopMonikerFSM";
    public const string ProcessingGroupPrefix = "WorkshopMoniker";

    private readonly string _fsmName = $"{FsmNamePrefix}_{Guid.NewGuid():N}";
    private readonly string _processingGroup = $"{ProcessingGroupPrefix}_{Guid.NewGuid():N}";
    private readonly FSMHandle _fsm;
    private bool _disposed;

    public MonikerMicroBundle(string text = DefaultText)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Moniker text cannot be empty.", nameof(text));

        Text = text;
        Context = new MonikerContext(text);
        Descriptor = new MicroBundleDescriptor((ulong)BundleId, "1.0.0");

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
        FSM_API.FSM_API.Interaction.Update(_processingGroup);
    }

    public int Id => BundleId;

    ulong TheSingularityWorkshop.SingularityHub.IMicroBundle.Id => checked((ulong)Id);

    public string Text { get; }

    public IStateContext Context { get; }

    public string Status => _fsm.CurrentState;

    public bool IsReady => Status == "Ready";

    public GuiNode? Composition { get; private set; }

    /// <summary>Descriptor used by FSM_COS for composition identity and versioning.</summary>
    public MicroBundleDescriptor Descriptor { get; }

    public IReadOnlyList<MicroBundleDependencyRequest> Dependencies { get; } =
        Array.Empty<MicroBundleDependencyRequest>();

    public void Load(IMicroBundleLoadContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        Composition = GuiBuilder.Create("Panel", "moniker-composition")
            .Property("composition", "Moniker")
            .Child("Text", "the", text => text.Text("THE"))
            .Child("Text", "singularity", text => text.Text("SINGULARITY"))
            .Child("Text", "workshop", text => text.Text("WORKSHOP"))
            .Build();
    }

    public bool Arbitrate(
        IMicroBundleArbitrationContext context,
        int roundIndex)
    {
        ArgumentNullException.ThrowIfNull(context);
        return false;
    }

    OntologySignature TheSingularityWorkshop.SingularityHub.IMicroBundle.Ontology =>
        new(0, 0, 0, 0, 0, 0, 0, 0, BundleId);

    BundleVersion TheSingularityWorkshop.SingularityHub.IMicroBundle.Version =>
        new(1, 0, 0);

    IReadOnlyList<ulong> TheSingularityWorkshop.SingularityHub.IMicroBundle.Dependencies =>
        Array.Empty<ulong>();

    public void Present()
    {
        if (_disposed) return;
        ((MonikerContext)Context).PresentationRequested = true;
    }

    public void Remove()
    {
        if (_disposed) return;
        ((MonikerContext)Context).RemovalRequested = true;
    }

    public void Update()
    {
        if (_disposed) return;
        FSM_API.FSM_API.Interaction.Update(_processingGroup);
    }

    void TheSingularityWorkshop.SingularityHub.IMicroBundle.LoadBundle(
        TheSingularityWorkshop.SingularityHub.IArbitrator arbitrator)
    {
        ArgumentNullException.ThrowIfNull(arbitrator);
    }

    bool TheSingularityWorkshop.SingularityHub.IMicroBundle.Arbitrate(
        TheSingularityWorkshop.SingularityHub.IArbitrator arbitrator,
        int roundIndex)
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
