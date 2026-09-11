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
        // configured initial state on the first explicit step.
        FSM_API.FSM_API.Create.CreateFiniteStateMachine(FsmName, -1, ProcessingGroup)
            .State("Manifesting", onEnter: EnterManifesting, onUpdate: _ => { }, onExit: _ => { })
            .State("Active", onEnter: EnterActive, onUpdate: _ => { }, onExit: _ => { })
            .State("Collapsing", onEnter: EnterCollapsing, onUpdate: _ => { }, onExit: _ => { })
            .State("Destroyed", onEnter: EnterDestroyed, onUpdate: _ => { }, onExit: _ => { })
            .Transition("Manifesting", "Active", c => ((MicroBundleContext)c).ElapsedMilliseconds >= 1)
            .Transition("Active", "Collapsing", c => !((MicroBundleContext)c).IsValid)
            .Transition("Collapsing", "Destroyed", c => !((MicroBundleContext)c).IsValid && ((MicroBundleContext)c).ElapsedMilliseconds >= 1)
            .WithInitialState("Manifesting")
            .BuildDefinition();

        _fsm = FSM_API.FSM_API.Create.CreateInstance(FsmName, Context, ProcessingGroup);
    }

    private string FsmName => $"MicroBundle_{Id}";
    private const string ProcessingGroup = "MicroBundles";

    public int Id { get; }
    public IStateContext Context { get; }

    /// <summary>Latest platform-specific manifestation produced by the provider.</summary>
    public MicroBundleManifestation? Manifestation { get; private set; }

    public void Update()
    {
        if (Context is MicroBundleContext context)
        {
            context.ElapsedMilliseconds++;
        }

        // FSMHandle.Update performs exactly one FSM step. Do not route this
        // lifecycle clock through group validity filtering: invalidity is itself
        // a lifecycle condition that must be consumed by the bundle FSM.
        _fsm.Update();
        Manifestation = _provider.Manifest(Context);
    }

    public void Invalidate()
    {
        if (Context is MicroBundleContext context)
        {
            context.IsValid = false;
        }
    }

    private void EnterManifesting(IStateContext context) => ((MicroBundleContext)context).Phase = "Manifesting";
    private void EnterActive(IStateContext context) => ((MicroBundleContext)context).Phase = "Active";
    private void EnterCollapsing(IStateContext context) => ((MicroBundleContext)context).Phase = "Collapsing";
    private void EnterDestroyed(IStateContext context) => ((MicroBundleContext)context).Phase = "Destroyed";
}
