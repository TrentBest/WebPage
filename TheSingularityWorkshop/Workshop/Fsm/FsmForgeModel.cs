namespace TheSingularityWorkshop.Gui;

/// <summary>Logical state-machine assembly being fabricated by the Forge.</summary>
public sealed class FsmForgeModel
{
    private readonly List<FsmForgeState> _states = [];
    private readonly List<FsmForgeCondition> _conditions = [];
    private readonly List<FsmForgeTransition> _transitions = [];
    private readonly List<FsmForgeToken> _tokens = [];

    /// <summary>Name shown on the fabrication workbench.</summary>
    public string Name { get; private set; } = "UNTITLED FSM";

    /// <summary>States currently placed on the workbench.</summary>
    public IReadOnlyList<FsmForgeState> States => _states;

    /// <summary>Reusable conditions available to transitions.</summary>
    public IReadOnlyList<FsmForgeCondition> Conditions => _conditions;

    /// <summary>Connections between state inputs.</summary>
    public IReadOnlyList<FsmForgeTransition> Transitions => _transitions;

    /// <summary>Small logical tokens available to condition expressions.</summary>
    public IReadOnlyList<FsmForgeToken> Tokens => _tokens;

    /// <summary>Renames the machine without coupling the domain to a GUI control.</summary>
    public void Rename(string name) => Name = string.IsNullOrWhiteSpace(name) ? "UNTITLED FSM" : name.Trim();

    /// <summary>Adds a blank state ingot to the workbench.</summary>
    public FsmForgeState AddState(string name = "NEW STATE")
    {
        var state = new FsmForgeState(Guid.NewGuid().ToString("N"), name);
        _states.Add(state);
        return state;
    }

    /// <summary>Adds a condition to the separate condition/control surface.</summary>
    public FsmForgeCondition AddCondition(string name = "NEW CONDITION")
    {
        var condition = new FsmForgeCondition(Guid.NewGuid().ToString("N"), name);
        _conditions.Add(condition);
        return condition;
    }

    /// <summary>Adds a transition between lifecycle inputs.</summary>
    public FsmForgeTransition Connect(string fromStateId, string fromInput, string toStateId, string toInput, string? conditionId = null)
    {
        var transition = new FsmForgeTransition(Guid.NewGuid().ToString("N"), fromStateId, fromInput, toStateId, toInput, conditionId);
        _transitions.Add(transition);
        return transition;
    }

    /// <summary>Adds a token used by the condition-expression surface.</summary>
    public FsmForgeToken AddToken(string value)
    {
        var token = new FsmForgeToken(Guid.NewGuid().ToString("N"), value);
        _tokens.Add(token);
        return token;
    }
}

/// <summary>A state ingot with independently placeable lifecycle slots.</summary>
public sealed class FsmForgeState(string id, string name)
{
    public string Id { get; } = id;
    public string Name { get; private set; } = name;
    public string Intent { get; private set; } = string.Empty;
    public IReadOnlyList<FsmForgeSlot> Slots { get; } =
    [
        new("OnEnter", "ENTER"),
        new("OnUpdate", "UPDATE"),
        new("OnExit", "EXIT")
    ];

    public void Rename(string name) => Name = string.IsNullOrWhiteSpace(name) ? "NEW STATE" : name.Trim();
    public void Describe(string intent) => Intent = intent?.Trim() ?? string.Empty;
}

/// <summary>A color-coded lifecycle slot on a state ingot.</summary>
public sealed record FsmForgeSlot(string Method, string Label);

/// <summary>A reusable logical condition independent of a particular transition.</summary>
public sealed class FsmForgeCondition(string id, string name)
{
    public string Id { get; } = id;
    public string Name { get; private set; } = name;
    public string Expression { get; private set; } = string.Empty;

    public void Rename(string name) => Name = string.IsNullOrWhiteSpace(name) ? "NEW CONDITION" : name.Trim();
    public void SetExpression(string expression) => Expression = expression?.Trim() ?? string.Empty;
}

/// <summary>A directed connection from one state input to another.</summary>
public sealed record FsmForgeTransition(
    string Id,
    string FromStateId,
    string FromInput,
    string ToStateId,
    string ToInput,
    string? ConditionId);

/// <summary>An atomic value/operator token used while composing a condition.</summary>
public sealed record FsmForgeToken(string Id, string Value);
