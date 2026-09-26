using TheSingularityWorkshop.FSM_API;
using FsmApi = TheSingularityWorkshop.FSM_API.FSM_API;

namespace TheSingularityWorkshop.Infrastructure.FsmForge;

/// <summary>
/// A tiny context used only by the Forge preview. It gives the real FSM API a
/// valid context while allowing optional MicroBundle capabilities to be attached.
/// </summary>
public class FsmForgePreviewContext : IStateContext
{
    public string Name { get; set; } = "ForgePreview";
    public int Context_ID => 1;
    public bool IsValid { get; set; } = true;
    public int UpdateCount { get; set; }
    public HashSet<string> Signals { get; } = new(StringComparer.Ordinal);
}

/// <summary>
/// Runs a real FSM_API definition assembled by the Forge. The preview consumes
/// the same state/transition shells that the workbench manipulates.
/// </summary>
public sealed class FsmForgePreview
{
    private static int _definitionSequence;
    private FSMHandle? _handle;
    private FsmForgeLogicContext? _context;
    private readonly List<string> _events = new();

    public string State => _handle?.CurrentState ?? "NOT STARTED";
    public int TickCount => _context?.UpdateCount ?? 0;
    public IReadOnlyList<string> Events => _events;
    public bool IsRunning => _handle?.IsValid == true;
    public FsmForgeDefinition? Definition { get; private set; }
    public FsmForgeLogicContext? Context => _context;

    public bool CanPreview(FsmForgeDefinition definition, out string reason)
        => definition.CanPreview(out reason);

    public void Start()
        => Start(CreateDefaultDefinition());

    public void Start(FsmForgeDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        if (!definition.CanPreview(out var reason))
            throw new InvalidOperationException($"Forge preview is not ready: {reason}.");

        Stop();

        var suffix = Interlocked.Increment(ref _definitionSequence);
        var name = $"WorkshopForgePreview_{suffix}";
        _context = new FsmForgeLogicContext();
        _events.Clear();
        Definition = definition;

        var builder = FsmApi.Create
            .CreateFiniteStateMachine(name, processRate: -1, processingGroup: "ForgePreview");

        foreach (var state in definition.States)
        {
            builder.State(
                state.Name,
                onEnter: _ => _events.Add($"OnInitialize → {state.Name}"),
                onUpdate: ctx =>
                {
                    _context.UpdateCount++;
                    _events.Add($"OnUpdate → {state.Name}");
                },
                onExit: _ => _events.Add($"OnExit → {state.Name}"));
        }

        builder.WithInitialState(definition.InitialState!);

        foreach (var transition in definition.Transitions)
        {
            builder.Transition(
                transition.FromState,
                transition.ToState,
                ctx => transition.Condition.Evaluate((FsmForgePreviewContext)ctx));
        }

        builder.BuildDefinition();
        _handle = FsmApi.Create.CreateInstance(name, _context, "ForgePreview");
    }

    public void AttachBundle(TheSingularityWorkshop.FSM_COS.IMicroBundle bundle)
        => _context?.AttachBundle(bundle);

    public void SetSignal(string signal, bool enabled = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(signal);
        if (enabled) _context?.Signals.Add(signal);
        else _context?.Signals.Remove(signal);
    }

    public void Tick()
    {
        if (_handle is null)
            return;

        FsmApi.Interaction.Update("ForgePreview");
    }

    public void Stop()
    {
        if (_context is not null)
            _context.IsValid = false;

        _handle = null;
        _context = null;
        Definition = null;
    }

    private static FsmForgeDefinition CreateDefaultDefinition()
    {
        var definition = new FsmForgeDefinition();
        definition.AddState("Idle");
        definition.AddState("Forging");
        definition.AddState("Finished");
        definition.AddTransition("Idle", "Forging", "BeginForging", new FsmForgeAlwaysCondition());
        definition.AddTransition("Forging", "Finished", "FinishForging", new FsmForgeUpdateCountCondition(3));
        return definition;
    }
}
