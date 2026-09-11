using TheSingularityWorkshop.FSM_API;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// Minimal runtime shell for a micro-bundle.
/// The bundle schedules lifecycle through FSM_API and leaves manifestation to
/// an <see cref="IMicroBundleProvider"/>.
/// </summary>
public sealed class MicroBundle : IMicroBundle
{
    private readonly FSMHandle _fsm;
    private readonly IMicroBundleProvider _provider;

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
    private const string ProcessingGroup = "MicroBundles";

    public int Id { get; }
    public IStateContext Context { get; }

    /// <summary>Latest platform-specific manifestation produced by the provider.</summary>
    public MicroBundleManifestation? Manifestation { get; private set; }

    public void Update()
    {
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
        {
            context.ElapsedMilliseconds++;
        }

        Manifestation = _provider.Manifest(Context);
    }

    public void Invalidate()
    {
        if (Context is MicroBundleContext context)
        {
            context.IsInvalidated = true;
        }
    }

    private void EnterManifesting(IStateContext context) => ((MicroBundleContext)context).Phase = "Manifesting";
    private void EnterActive(IStateContext context) => ((MicroBundleContext)context).Phase = "Active";
    private void EnterCollapsing(IStateContext context) => ((MicroBundleContext)context).Phase = "Collapsing";
    private void EnterDestroyed(IStateContext context) => ((MicroBundleContext)context).Phase = "Destroyed";
}
