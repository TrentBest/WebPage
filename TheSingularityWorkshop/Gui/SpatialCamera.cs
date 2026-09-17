namespace TheSingularityWorkshop.Gui;

/// <summary>Converts persistent Workshop world coordinates into an avatar-centered viewport.</summary>
/// <remarks>
/// The viewport is fixed. The world moves underneath the avatar, matching the presentation
/// model of early top-down RPGs while keeping world coordinates independent of the renderer.
/// Zoom is a perception-layer concern: world coordinates never change when the visitor zooms.
/// </remarks>
public readonly record struct SpatialCamera(double AvatarX, double AvatarY, double Zoom = 1d)
{
    /// <summary>CSS viewport units occupied by one Workshop world unit horizontally at 1X.</summary>
    public const double WorldUnitWidthVw = 1.5;

    /// <summary>CSS viewport units occupied by one Workshop world unit vertically at 1X.</summary>
    public const double WorldUnitHeightVh = 1.5;

    /// <summary>Width of the persistent logical world in CSS viewport units at 1X.</summary>
    public const double WorldWidthVw = 150;

    /// <summary>Height of the persistent logical world in CSS viewport units at 1X.</summary>
    public const double WorldHeightVh = 150;

    /// <summary>Minimum supported perception zoom.</summary>
    public const double MinZoom = 0.35;

    /// <summary>Maximum supported perception zoom.</summary>
    public const double MaxZoom = 4d;

    /// <summary>Returns a camera with its zoom constrained to the supported range.</summary>
    public SpatialCamera Clamped()
        => this with { Zoom = Math.Clamp(Zoom, MinZoom, MaxZoom) };

    /// <summary>Horizontal world offset that places the avatar at viewport center.</summary>
    public double OffsetVw => 50 - AvatarX * WorldUnitWidthVw * Zoom;

    /// <summary>Vertical world offset that places the avatar at viewport center.</summary>
    public double OffsetVh => 50 - AvatarY * WorldUnitHeightVh * Zoom;

    /// <summary>Returns a world X coordinate as a CSS viewport position before camera translation.</summary>
    public static double WorldToVw(double x, double zoom = 1d) => x * WorldUnitWidthVw * zoom;

    /// <summary>Returns a world Y coordinate as a CSS viewport position before camera translation.</summary>
    public static double WorldToVh(double y, double zoom = 1d) => y * WorldUnitHeightVh * zoom;
}
