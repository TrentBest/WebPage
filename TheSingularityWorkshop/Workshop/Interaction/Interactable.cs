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

/// <summary>Stable point at which a visitor arrives before an interaction executes.</summary>
public readonly record struct InteractionPoint(double X, double Y, double Z = 0);

/// <summary>
/// Runtime context used to determine which interactions are currently eligible.
/// Context is deliberately small so it can be projected into 2D, 2.5D, or 3D later.
/// </summary>
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

/// <summary>
/// Cheap eligibility predicate evaluated before behavior. Expensive interaction
/// logic must not be invoked for an interactable that fails this filter.
/// </summary>
public interface IInteractableAvailabilityFilter
{
    bool CanRun(Interactable interactable, InteractionContext context);
}

/// <summary>
/// A generic interaction node. Children form a hierarchy such as:
/// Workshop → Chemistry Lab → Desk → Drawer → Drawer Contents.
/// A child is not evaluated until its parent surface is active.
/// </summary>
public sealed class Interactable
{
    private readonly List<Interactable> _children = [];

    public Interactable(
        string id,
        string name,
        InteractionScope scope,
        InteractionPoint interactionPoint,
        IInteractableBehavior behavior,
        IInteractableAvailabilityFilter? availabilityFilter = null)
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
    }

    public string Id { get; }
    public string Name { get; }
    public InteractionScope Scope { get; }
    public InteractionPoint InteractionPoint { get; }
    public IInteractableBehavior Behavior { get; }
    public IInteractableAvailabilityFilter? AvailabilityFilter { get; }
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
}

/// <summary>
/// Filters a hierarchy before behavior evaluation. Only the active surface's
/// runnable children are returned; hidden or unavailable branches are not inspected.
/// </summary>
public sealed class InteractableFilter
{
    public IReadOnlyList<Interactable> GetRunnable(
        IEnumerable<Interactable> candidates,
        InteractionContext context)
    {
        ArgumentNullException.ThrowIfNull(candidates);
        ArgumentNullException.ThrowIfNull(context);

        return candidates
            .Where(x => x.CanRun(context))
            .ToArray();
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
