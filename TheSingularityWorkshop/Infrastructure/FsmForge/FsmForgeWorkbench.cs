namespace TheSingularityWorkshop.Infrastructure.FsmForge;

/// <summary>
/// Human-scale authoring surface for turning Forge stock into an executable FSM definition.
/// </summary>
public sealed class FsmForgeWorkbench
{
    private readonly List<FsmForgeStatePlate> _statePlates = [];
    private readonly List<FsmForgeTransitionPlate> _transitionPlates = [];

    public IReadOnlyList<FsmForgeStatePlate> StatePlates => _statePlates;
    public IReadOnlyList<FsmForgeTransitionPlate> TransitionPlates => _transitionPlates;
    public string? SelectedStockId { get; private set; }
    public FsmForgeDefinition Definition { get; } = new();

    /// <summary>Selects an item from the physical stock represented by the Forge.</summary>
    public void SelectStock(string productId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(productId);
        if (!FsmForgeCatalog.Products.Any(product => product.Id.Equals(productId, StringComparison.Ordinal)))
            throw new InvalidOperationException($"Forge stock '{productId}' does not exist.");

        SelectedStockId = productId;
    }

    /// <summary>Places the selected state ingot onto a new state plate.</summary>
    public FsmForgeStatePlate PlaceSelectedState()
    {
        if (!string.Equals(SelectedStockId, "state-ingot", StringComparison.Ordinal))
            throw new InvalidOperationException("Select a state ingot before placing it on a state plate.");

        var index = _statePlates.Count + 1;
        var plate = new FsmForgeStatePlate($"State {index}");
        _statePlates.Add(plate);
        Definition.AddState(plate.Name);
        SelectedStockId = null;
        return plate;
    }

    /// <summary>Names an existing state plate and keeps the executable definition synchronized.</summary>
    public void NameState(int index, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (index < 0 || index >= _statePlates.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        var existing = _statePlates[index];
        if (existing.Name.Equals(name, StringComparison.Ordinal))
            return;

        if (Definition.States.Any(state => state.Name.Equals(name, StringComparison.Ordinal)))
            throw new InvalidOperationException($"Forge state '{name}' already exists.");

        var replacement = existing with { Name = name };
        _statePlates[index] = replacement;

        // The definition is intentionally rebuilt from the plates so naming remains
        // a semantic authoring operation rather than a UI-only mutation.
        RebuildDefinition();
    }

    /// <summary>Places a transition plate between two named states.</summary>
    public FsmForgeTransitionPlate PlaceTransition(string fromState, string toState, string signal)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fromState);
        ArgumentException.ThrowIfNullOrWhiteSpace(toState);
        ArgumentException.ThrowIfNullOrWhiteSpace(signal);

        if (_statePlates.All(plate => !plate.Name.Equals(fromState, StringComparison.Ordinal)) ||
            _statePlates.All(plate => !plate.Name.Equals(toState, StringComparison.Ordinal)))
            throw new InvalidOperationException("Both transition endpoints must exist on the workbench.");

        var plate = new FsmForgeTransitionPlate(fromState, toState, signal);
        _transitionPlates.Add(plate);
        RebuildDefinition();
        return plate;
    }

    /// <summary>Rebuilds the executable definition from the current physical plates.</summary>
    public void RebuildDefinition()
    {
        var rebuilt = new FsmForgeDefinition();

        foreach (var plate in _statePlates)
            rebuilt.AddState(plate.Name);

        foreach (var plate in _transitionPlates)
            rebuilt.AddTransition(
                plate.FromState,
                plate.ToState,
                $"Transition_{Sanitize(plate.FromState)}_To_{Sanitize(plate.ToState)}_{_transitionPlates.IndexOf(plate) + 1}",
                new FsmForgeSignalCondition(plate.Signal));

        Definition.ReplaceWith(rebuilt);
    }

    private static string Sanitize(string value)
        => new(value.Select(character => char.IsLetterOrDigit(character) ? character : '_').ToArray());
}

/// <summary>A physical/diegetic state authoring plate.</summary>
public sealed record FsmForgeStatePlate(string Name);

/// <summary>A physical/diegetic transition authoring plate.</summary>
public sealed record FsmForgeTransitionPlate(string FromState, string ToState, string Signal);
