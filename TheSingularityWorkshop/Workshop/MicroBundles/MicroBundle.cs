using System;
using TheSingularityWorkshop.FSM_API;
using HubBundle = TheSingularityWorkshop.SingularityHub.IMicroBundle;
using TheSingularityWorkshop.SingularityHub;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// Minimal runtime shell for a micro-bundle.
/// The bundle schedules lifecycle through FSM_API and leaves manifestation to
/// an <see cref="IMicroBundleProvider"/>.
/// It also projects itself through the Hub-level micro-bundle contract so the
/// runtime Element can participate in Hub arbitration without exposing its FSM mechanics.
/// </summary>
public sealed class MicroBundle : IMicroBundle, HubBundle, IDisposable
{
    private readonly FSMHandle _fsm;
    private readonly IMicroBundleProvider _provider;
    private bool _disposed;

    public MicroBundle(int id, string name, IMicroBundleProvider provider, int parentId = -1, int generation = 0)
    {
        Id = id;
        _provider = provider;
        Context = new MicroBundleContext(id, name)
        {
            ParentId = parentId,
            Generation = generation
        };

        FSM_API.FSM_API.Create.CreateProcessingGroup(ProcessingGroup);

        // Construction establishes Created conceptually. FSM_API enters the
        // configured initial state on the first explicit group update.
        FSM_API.FSM_API.Create.CreateFiniteStateMachine(FsmName, -1, ProcessingGroup)
            .State("Manifesting", onEnter: EnterManifesting, onUpdate: _ => { }, onExit: _ => { })
            .State("Active", onEnter: EnterActive, onUpdate: _ => { }, onExit: _ => { })
            .State("Collapsing", onEnter: EnterCollapsing, onUpdate: _ => { }, onExit: _ => { })
            .State("Destroyed", onEnter: EnterDestroyed, onUpdate: _ => { }, onExit: _ => { })
            .Transition("Manifesting", "Active", c => ((MicroBundleContext)c).ElapsedMilliseconds >= 1)
            .Transition("Active", "Collapsing", c => ((MicroBundleContext)c).IsInvalidated)
            .Transition("Collapsing", "Destroyed", c => ((MicroBundleContext)c).IsInvalidated && ((MicroBundleContext)c).ElapsedMilliseconds >= 1)
            .WithInitialState("Manifesting")
            .BuildDefinition();

        _fsm = FSM_API.FSM_API.Create.CreateInstance(FsmName, Context, ProcessingGroup);

        // The context is deliberately invalid while this constructor is wiring
        // the bundle. Only the completed object becomes runnable by FSM_API.
        ((MicroBundleContext)Context).IsValid = true;
    }

    private string FsmName => $"MicroBundle_{Id}";
    private string ProcessingGroup => $"MicroBundle_{Id}";

    public int Id { get; }
    public IStateContext Context { get; }

    /// <summary>Latest platform-specific manifestation produced by the provider.</summary>
    public MicroBundleManifestation? Manifestation { get; private set; }

    /// <summary>Hub-facing identity. It is the same stable identity represented by the Workshop integer id.</summary>
    ulong HubBundle.Id => checked((ulong)Id);

    /// <summary>Hub-facing ontology projection. Runtime Element identity is the first coordinate until richer ontology data is supplied.</summary>
    OntologySignature HubBundle.Ontology => new(0, 0, 0, 0, 0, 0, 0, 0, Id);

    /// <summary>Hub-facing bundle version.</summary>
    BundleVersion HubBundle.Version => new(1, 0, 0);

    /// <summary>Workshop Elements currently declare no Hub-level dependencies.</summary>
    IReadOnlyList<ulong> HubBundle.Dependencies => Array.Empty<ulong>();

    /// <summary>Lifecycle participation is represented as a successful Hub arbitration pass.</summary>
    bool HubBundle.Arbitrate(IArbitrator arbitrator, int roundIndex)
    {
        ArgumentNullException.ThrowIfNull(arbitrator);
        return roundIndex >= 0;
    }

    public void Update()
    {
        if (_disposed) return;

        // FSM_API's processing-group update owns the initial state entry and
        // regular transition evaluation. The lifecycle clock advances only
        // after the FSM has evaluated the current tick. This preserves the
        // contract that the first explicit update enters Manifesting without
        // immediately satisfying its one-tick transition to Active.
        var stateBeforeTick = _fsm.CurrentState;

        FSM_API.FSM_API.Interaction.Update(ProcessingGroup);

        if (stateBeforeTick != _fsm.CurrentState && !_fsm.HasEnteredCurrentState)
        {
            _fsm.TransitionTo(_fsm.CurrentState);
        }

        if (Context is MicroBundleContext context)
            context.ElapsedMilliseconds++;

        Manifestation = _provider.Manifest(Context);
    }

    public void Invalidate()
    {
        if (Context is MicroBundleContext context)
            context.IsInvalidated = true;
    }

    /// <summary>
    /// Destroys the MicroBundle lifecycle FSM through FSM_API. This keeps the
    /// lifecycle scheduler free of stale bundle instances when an Experience
    /// leaves the presentation surface.
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;

        FSM_API.FSM_API.Interaction.DestroyFiniteStateMachine(FsmName, ProcessingGroup);
        _disposed = true;
    }

    private void EnterManifesting(IStateContext context) => ((MicroBundleContext)context).Phase = "Manifesting";
    private void EnterActive(IStateContext context) => ((MicroBundleContext)context).Phase = "Active";
    private void EnterCollapsing(IStateContext context) => ((MicroBundleContext)context).Phase = "Collapsing";
    private void EnterDestroyed(IStateContext context) => ((MicroBundleContext)context).Phase = "Destroyed";
}
