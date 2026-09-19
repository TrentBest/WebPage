using System;

namespace TheSingularityWorkshop.Workshop.Interaction;

/// <summary>
/// Workshop-facing interaction node.
/// The core interaction contract remains platform-neutral; this specialization
/// adds Workshop presentation semantics that can travel with any Experience,
/// including low-dimensional software Experiences.
/// </summary>
public sealed class WorkshopInteractable : Interactable
{
    public WorkshopInteractable(
        string id,
        string name,
        InteractionScope scope,
        InteractionPoint interactionPoint,
        IInteractableBehavior behavior,
        WorkshopSign? sign = null,
        IInteractableAvailabilityFilter? availabilityFilter = null,
        InteractionTrigger triggers = InteractionTrigger.Click,
        InteractablePresentation? presentation = null)
        : base(id, name, scope, interactionPoint, behavior, availabilityFilter, triggers, presentation)
    {
        Sign = sign;
    }

    /// <summary>Optional physical/visual sign associated with this Workshop interactable.</summary>
    public WorkshopSign? Sign { get; }

    public bool HasSign => Sign is not null;
}

/// <summary>
/// Data describing the face of a sign.
/// Rendering remains the responsibility of a GUI builder; this model is usable
/// in spatial, desktop, web, or other Experience manifestations.
/// </summary>
public sealed record WorkshopSign
{
    public WorkshopSign(string id, string title, params string[] lines)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Sign id is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Sign title is required.", nameof(title));
        ArgumentNullException.ThrowIfNull(lines);

        Id = id;
        Title = title;
        Lines = lines;
    }

    public string Id { get; }
    public string Title { get; }
    public string[] Lines { get; }
    public string Kicker { get; init; } = "WORKSHOP // SIGN";
}
