using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TheSingularityWorkshop.Workshop.Creation;

/// <summary>
/// Declarative description of an FSM that can be persisted without attempting to serialize
/// executable delegates. Runtime behavior is compiled from this blueprint and host-provided actions.
/// </summary>
public sealed class FsmBlueprint
{
    public FsmBlueprint(
        string name,
        IEnumerable<string> states,
        string initialState,
        IEnumerable<FsmTransitionBlueprint>? transitions = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("FSM blueprint name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(initialState))
            throw new ArgumentException("FSM initial state is required.", nameof(initialState));

        var stateList = states?.Where(s => !string.IsNullOrWhiteSpace(s)).ToList()
            ?? throw new ArgumentNullException(nameof(states));

        if (!stateList.Contains(initialState, StringComparer.Ordinal))
            throw new ArgumentException("The initial state must exist in the state collection.", nameof(initialState));

        Name = name;
        States = new ReadOnlyCollection<string>(stateList);
        InitialState = initialState;
        Transitions = new ReadOnlyCollection<FsmTransitionBlueprint>(
            (transitions ?? Enumerable.Empty<FsmTransitionBlueprint>()).ToList());
    }

    public string Name { get; }
    public IReadOnlyList<string> States { get; }
    public string InitialState { get; }
    public IReadOnlyList<FsmTransitionBlueprint> Transitions { get; }
}

/// <summary>Declarative edge in a persisted FSM graph.</summary>
public sealed record FsmTransitionBlueprint(string From, string To, string Condition);
