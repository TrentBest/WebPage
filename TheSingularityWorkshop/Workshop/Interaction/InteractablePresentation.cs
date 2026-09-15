namespace TheSingularityWorkshop.Workshop.Interaction;

/// <summary>
/// Workshop-facing visual intent for an interactable.
/// The interaction domain declares what may be shown; GUI builders decide how
/// that intent is rendered for a particular Experience.
/// </summary>
public sealed record InteractablePresentation(
    string Kicker,
    string Description,
    string SchematicId = "default")
{
    public static InteractablePresentation Default(string name)
        => new("INTERACTABLE", name, "default");
}
