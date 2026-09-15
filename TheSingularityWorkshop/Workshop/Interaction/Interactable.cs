using System;
using System.Collections.Generic;
using System.Linq;

namespace TheSingularityWorkshop.Workshop.Interaction;

/// <summary>Presentation depth of an interactable hierarchy.</summary>
public enum InteractionScope
{
    World,
    Surface,
    Container,
    Contents
}

/// <summary>Input that may cause an interactable to execute.</summary>
[Flags]
public enum InteractionTrigger
{
    None = 0,
    Hover = 1,
    Click = 2,
    HoverOrClick = Hover | Click
}

/// <summary>Stable point at which a visitor arrives before an interaction executes.</summary>
public readonly record struct InteractionPoint(double X, double Y, double Z = 0);

/// <summary>Runtime context used to determine which interactions are currently eligible.</summary>
public sealed record InteractionContext(
    string SurfaceId,
    InteractionScope Scope,
    IReadOnlySet<string> OpenContainerIds)
{
    public bool IsContainerOpen(string id) => OpenContainerIds.Contains(id);
}

/// <summary>Behavior boundary for a runnable interaction.</summary>
public interface IInteractableBehavior
{
    void Execute(InteractionContext context);
}

/// <summary>Cheap eligibility predicate evaluated before behavior.</summary>
public interface IInteractableAvailabilityFilter
{
    bool CanRun(Interactable interactable, InteractionContext context);
}

/// <summary>Generic interaction node with hierarchical children and declared input triggers.</summary>
public sealed class Interactable
{
    private readonly List<Interactable> _children = [];

    public Interactable(
        string id,
        string name,
        InteractionScope scope,
        InteractionPoint interactionPoint,
        IInteractableBehavior behavior,
        IInteractableAvailabilityFilter? availabilityFilter = null,
        InteractionTrigger triggers = InteractionTrigger.Click)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Interactable id is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Interactable name is required.", nameof(name));
        ArgumentNullException.ThrowIfNull(behavior);

        Id = id;
        Name = name;
        Scope = scope;
        InteractionPoint = interactionPoint;
        Behavior = behavior;
        AvailabilityFilter = availabilityFilter;
        Triggers = triggers;
    }

    public string Id { get; }
    public string Name { get; }
    public InteractionScope Scope { get; }
    public InteractionPoint InteractionPoint { get; }
    public IInteractableBehavior Behavior { get; }
    public IInteractableAvailabilityFilter? AvailabilityFilter { get; }
    public InteractionTrigger Triggers { get; }
    public Interactable? Parent { get; private set; }
    public IReadOnlyList<Interactable> Children => _children;

    public Interactable AddChild(Interactable child)
    {
        ArgumentNullException.ThrowIfNull(child);
        if (ReferenceEquals(child, this)) throw new ArgumentException("An interactable cannot contain itself.", nameof(child));
        if (child.Parent is not null) throw new InvalidOperationException($"Interactable '{child.Id}' already has a parent.");

        child.Parent = this;
        _children.Add(child);
        return this;
    }

    public bool CanRun(InteractionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return AvailabilityFilter?.CanRun(this, context) ?? true;
    }

    public bool Supports(InteractionTrigger trigger) => (Triggers & trigger) == trigger;
}

/// <summary>Executes an eligible interactable only for a declared trigger.</summary>
public sealed class InteractableExecutor
{
    public bool TryExecute(Interactable interactable, InteractionTrigger trigger, InteractionContext context)
    {
        ArgumentNullException.ThrowIfNull(interactable);
        ArgumentNullException.ThrowIfNull(context);

        if (!interactable.Supports(trigger) || !interactable.CanRun(context))
            return false;

        interactable.Behavior.Execute(context);
        return true;
    }
}

/// <summary>Filters a hierarchy before behavior evaluation.</summary>
public sealed class InteractableFilter
{
    public IReadOnlyList<Interactable> GetRunnable(
        IEnumerable<Interactable> candidates,
        InteractionContext context)
    {
        ArgumentNullException.ThrowIfNull(candidates);
        ArgumentNullException.ThrowIfNull(context);

        return candidates.Where(x => x.CanRun(context)).ToArray();
    }
}

/// <summary>Common filter that activates a node only on its owning surface and scope.</summary>
public sealed class SurfaceScopeFilter : IInteractableAvailabilityFilter
{
    public bool CanRun(Interactable interactable, InteractionContext context)
        => string.Equals(interactable.Parent?.Id ?? interactable.Id, context.SurfaceId, StringComparison.Ordinal)
           && interactable.Scope == context.Scope;
}

/// <summary>Filter for a drawer/container that is eligible only after it has been opened.</summary>
public sealed class OpenContainerFilter : IInteractableAvailabilityFilter
{
    public bool CanRun(Interactable interactable, InteractionContext context)
        => context.IsContainerOpen(interactable.Parent?.Id ?? interactable.Id);
}
