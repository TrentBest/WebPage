namespace TheSingularityWorkshop.Workshop.Rendering;

/// <summary>
/// Describes how a visual experience should be presented without requiring the
/// experience itself to know how the renderer implements the presentation.
/// </summary>
public sealed record RenderingIntent(
    string TargetId,
    double Dimension,
    RenderingCameraBehavior CameraBehavior)
{
    /// <summary>Gets the dimension value constrained to the renderer's 2D-to-3D continuum.</summary>
    public double ClampedDimension => Math.Clamp(Dimension, 0d, 1d);

    /// <summary>Gets the human-facing anchor of the current visual domain.</summary>
    public string Domain =>
        ClampedDimension switch
        {
            <= 0.25d => "2D",
            >= 0.75d => "3D",
            _ => "ISO / HYBRID"
        };
}

/// <summary>Describes the intent governing camera behavior around a rendering target.</summary>
public enum RenderingCameraBehavior
{
    /// <summary>Keep the camera framed on the target without autonomous movement.</summary>
    Frame,

    /// <summary>Keep the camera attached to a target that may move through the scene.</summary>
    Follow,

    /// <summary>Move the camera around the target while preserving the target as the focal point.</summary>
    Orbit
}
