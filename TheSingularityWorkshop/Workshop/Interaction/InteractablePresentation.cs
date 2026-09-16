namespace TheSingularityWorkshop.Workshop.Interaction;

/// <summary>Visual state supplied to an interactable presentation renderer.</summary>
public enum InteractableVisualState
{
    Resting,
    Hovered,
    Interacting
}

/// <summary>Renderer-neutral visual effect requested by an interactable.</summary>
public enum InteractableVisualEffect
{
    None,
    Breathe,
    Pulse,
    Illuminate,
    Expand
}

/// <summary>Renderer-neutral visual behavior for one interaction state.</summary>
public sealed record InteractableVisualIntent(
    InteractableVisualEffect Effect,
    double Intensity = 1.0)
{
    public static InteractableVisualIntent Resting => new(InteractableVisualEffect.None, 0);
    public static InteractableVisualIntent Breathes(double intensity = 1.0)
        => new(InteractableVisualEffect.Breathe, intensity);
    public static InteractableVisualIntent Pulses(double intensity = 1.0)
        => new(InteractableVisualEffect.Pulse, intensity);
}

/// <summary>
/// Renderer-neutral wayfinding presentation declared by an interactable.
/// A map renderer decides how the line is actually drawn; the interactable
/// supplies the semantic label and visual parameters.
/// </summary>
public sealed record InteractableMapPresentation(
    string ThreadLabel,
    string Accent,
    double LineWidth = 1.1,
    double LineOpacity = 0.86)
{
    public static InteractableMapPresentation Default(string name)
        => new(name.ToUpperInvariant(), "#ffffff");
}

/// <summary>
/// Workshop-facing visual intent for an interactable.
/// The interaction domain declares what may be shown; a renderer decides how
/// that intent is manifested for a particular Experience.
/// </summary>
public sealed record InteractablePresentation(
    string Kicker,
    string Description,
    string SchematicId = "default",
    InteractableVisualIntent? Hover = null,
    InteractableVisualIntent? Interacting = null,
    InteractableMapPresentation? Map = null)
{
    public InteractableVisualIntent HoverIntent => Hover ?? InteractableVisualIntent.Breathes();
    public InteractableVisualIntent InteractingIntent => Interacting ?? InteractableVisualIntent.Pulses();
    public InteractableMapPresentation MapIntent => Map ?? InteractableMapPresentation.Default(Kicker);

    public static InteractablePresentation Default(string name)
        => new(
            "INTERACTABLE",
            name,
            "default",
            InteractableVisualIntent.Breathes(),
            InteractableVisualIntent.Pulses(),
            InteractableMapPresentation.Default(name));
}
