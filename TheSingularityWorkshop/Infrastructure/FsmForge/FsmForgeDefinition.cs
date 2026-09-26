namespace TheSingularityWorkshop.Infrastructure.FsmForge;

/// <summary>
/// Diegetic definition assembled by the FSM Forge workbench.
/// The definition is deliberately independent of rendering: the GUI manipulates
/// these same shells, while the preview/compiler consumes them.
/// </summary>
public sealed class FsmForgeDefinition
{
    private readonly List<FsmForgeState> _states = [];
    private readonly List<FsmForgeTransition> _transitions = [];

    public IReadOnlyList<FsmForgeState> States => _states;
    public IReadOnlyList<FsmForgeTransition> Transitions => _transitions;
    public string? InitialState { get; private set; }

    public FsmForgeState AddState(
        string name,
        string? onInitializeMethod = null,
        string? onUpdateMethod = null,
        string? onExitMethod = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var state = new FsmForgeState(
            name,
            onInitializeMethod ?? $"OnInitialize_{name}",
            onUpdateMethod ?? $"OnUpdate_{name}",
            onExitMethod ?? $"OnExit_{name}");

        _states.Add(state);
        InitialState ??= name;
        return state;
    }

    public void SetInitialState(string name)
    {
        RequireState(name);
        InitialState = name;
    }

    public FsmForgeTransition AddTransition(
        string fromState,
        string toState,
        string methodName,
        FsmForgeCondition condition)
    {
        RequireState(fromState);
        RequireState(toState);
        ArgumentException.ThrowIfNullOrWhiteSpace(methodName);
        ArgumentNullException.ThrowIfNull(condition);

        var transition = new FsmForgeTransition(
            fromState,
            toState,
            methodName,
            condition);

        _transitions.Add(transition);
        return transition;
    }

    public bool CanPreview(out string reason)
    {
        if (_states.Count == 0)
        {
            reason = "ADD AT LEAST ONE STATE";
            return false;
        }

        if (string.IsNullOrWhiteSpace(InitialState))
        {
            reason = "SELECT AN INITIAL STATE";
            return false;
        }

        if (_transitions.Count == 0)
        {
            reason = "ADD AT LEAST ONE TRANSITION";
            return false;
        }

        var disconnected = _transitions
            .Where(t => t.Condition is FsmForgeUnboundCondition)
            .Select(t => t.MethodName)
            .FirstOrDefault();

        if (disconnected is not null)
        {
            reason = $"PLUG A CONDITION INTO {disconnected}";
            return false;
        }

        reason = "READY";
        return true;
    }

    private void RequireState(string name)
    {
        if (_states.All(state => !state.Name.Equals(name, StringComparison.Ordinal)))
            throw new InvalidOperationException($"Forge state '{name}' does not exist.");
    }
}

public sealed record FsmForgeState(
    string Name,
    string OnInitializeMethod,
    string OnUpdateMethod,
    string OnExitMethod);

public sealed record FsmForgeTransition(
    string FromState,
    string ToState,
    string MethodName,
    FsmForgeCondition Condition);

/// <summary>
/// Executable condition ingot. A transition may be scaffolded without one,
/// but a Forge preview requires an actual condition implementation.
/// </summary>
public abstract record FsmForgeCondition
{
    public abstract bool Evaluate(FsmForgePreviewContext context);
    public abstract string Scaffold { get; }
}

public sealed record FsmForgeUnboundCondition : FsmForgeCondition
{
    public override bool Evaluate(FsmForgePreviewContext context) => false;
    public override string Scaffold => "return false;";
}

public sealed record FsmForgeAlwaysCondition : FsmForgeCondition
{
    public override bool Evaluate(FsmForgePreviewContext context) => true;
    public override string Scaffold => "return true;";
}

public sealed record FsmForgeUpdateCountCondition(int Minimum) : FsmForgeCondition
{
    public override bool Evaluate(FsmForgePreviewContext context)
        => context.UpdateCount >= Minimum;

    public override string Scaffold
        => $"// Forge preview condition: UpdateCount >= {Minimum}. Bind this to the application context.\n        return false;";
}

public sealed record FsmForgeSignalCondition(string Signal) : FsmForgeCondition
{
    public override bool Evaluate(FsmForgePreviewContext context)
        => context.Signals.Contains(Signal);

    public override string Scaffold
        => $"// Forge preview condition: signal \"{Signal}\". Bind this to the application context.\n        return false;";
}
