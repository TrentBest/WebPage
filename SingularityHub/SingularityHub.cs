using System;
using System.Collections.Generic;
using TheSingularityWorkshop.FSM_API;
using fsm_API = TheSingularityWorkshop.FSM_API.FSM_API;

namespace TheSingularityWorkshop.SingularityHub;

/// <summary>
/// Fluent registration and scheduler boundary for FSM_API process groups.
///
/// The Hub owns top-level process-group stepping. Nested groups remain part of
/// the same registered graph, but are stepped explicitly by the owning FSM when
/// its update method requires them. This preserves composition without causing
/// the Hub to double-step nested work.
/// </summary>
public sealed class SingularityHub
{
    private readonly List<ProcessGroupRegistration> _registrations = new();
    private readonly Action<string> _update;

    /// <summary>Creates a Hub that delegates group stepping to FSM_API.</summary>
    public SingularityHub()
        : this(fsm_API.Interaction.Update)
    {
    }

    /// <summary>
    /// Creates a Hub with an injectable step operation for deterministic tests
    /// and alternate hosting environments.
    /// </summary>
    public SingularityHub(Action<string> update)
    {
        _update = update ?? throw new ArgumentNullException(nameof(update));
    }

    /// <summary>Gets the groups registered with this Hub in registration order.</summary>
    public IReadOnlyList<ProcessGroupRegistration> ProcessGroups => _registrations;

    /// <summary>
    /// Registers a process group. A null parent makes the group a Hub-owned root.
    /// A parent makes the group nested and therefore excluded from normal Hub.Update().
    /// </summary>
    public SingularityHub RegisterProcessGroup(string processGroup, string? parentProcessGroup = null)
    {
        if (string.IsNullOrWhiteSpace(processGroup))
            throw new ArgumentException("A process group name is required.", nameof(processGroup));

        if (_registrations.Exists(x => x.Name == processGroup))
            throw new InvalidOperationException($"Process group '{processGroup}' is already registered.");

        if (parentProcessGroup is not null && !_registrations.Exists(x => x.Name == parentProcessGroup))
            throw new InvalidOperationException($"Parent process group '{parentProcessGroup}' must be registered first.");

        _registrations.Add(new ProcessGroupRegistration(processGroup, parentProcessGroup));
        return this;
    }

    /// <summary>Registers several root process groups fluently.</summary>
    public SingularityHub RegisterProcessGroups(params string[] processGroups)
    {
        if (processGroups is null)
            throw new ArgumentNullException(nameof(processGroups));

        foreach (var processGroup in processGroups)
            RegisterProcessGroup(processGroup);

        return this;
    }

    /// <summary>
    /// Steps only Hub-owned root groups. Nested groups are deliberately not
    /// discovered or stepped here.
    /// </summary>
    public void Update()
    {
        foreach (var registration in _registrations)
        {
            if (registration.ParentName is null)
                _update(registration.Name);
        }
    }

    /// <summary>
    /// Steps the direct nested children of an owning process group. An FSM may
    /// call this from its own update method when its simulation/functionality
    /// requires those dependent groups to run before that FSM continues.
    /// </summary>
    public void UpdateNestedProcessGroups(string parentProcessGroup)
    {
        if (string.IsNullOrWhiteSpace(parentProcessGroup))
            throw new ArgumentException("A parent process group name is required.", nameof(parentProcessGroup));

        foreach (var registration in _registrations)
        {
            if (registration.ParentName == parentProcessGroup)
                _update(registration.Name);
        }
    }

    /// <summary>Immutable description of one registered process group.</summary>
    public sealed record ProcessGroupRegistration(string Name, string? ParentName);
}
