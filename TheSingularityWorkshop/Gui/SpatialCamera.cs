namespace TheSingularityWorkshop.Gui;

/// <summary>Converts persistent Workshop world coordinates into an avatar-centered viewport.</summary>
/// <remarks>
/// The viewport is fixed. The world moves underneath the avatar, matching the presentation
/// model of early top-down RPGs while keeping world coordinates independent of the renderer.
/// </remarks>
public readonly record struct SpatialCamera(double AvatarX, double AvatarY)
{
    /// <summary>CSS viewport units occupied by one Workshop world unit horizontally.</summary>
    public const double WorldUnitWidthVw = 1.5;

    /// <summary>CSS viewport units occupied by one Workshop world unit vertically.</summary>
    public const double WorldUnitHeightVh = 1.5;

    /// <summary>Width of the persistent 0..100 logical world in CSS viewport units.</summary>
    public const double WorldWidthVw = 150;

    /// <summary>Height of the persistent 0..100 logical world in CSS viewport units.</summary>
    public const double WorldHeightVh = 150;

    /// <summary>Horizontal world offset that places the avatar at viewport center.</summary>
    public double OffsetVw => 50 - AvatarX * WorldUnitWidthVw;

    /// <summary>Vertical world offset that places the avatar at viewport center.</summary>
    public double OffsetVh => 50 - AvatarY * WorldUnitHeightVh;

    /// <summary>Returns a world X coordinate as a CSS viewport position before camera translation.</summary>
    public static double WorldToVw(double x) => x * WorldUnitWidthVw;

    /// <summary>Returns a world Y coordinate as a CSS viewport position before camera translation.</summary>
    public static double WorldToVh(double y) => y * WorldUnitHeightVh;
}
