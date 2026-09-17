namespace TheSingularityWorkshop.Gui;

/// <summary>Visual transformation applied to a spatial line before rendering.</summary>
public enum SpatialLineEffect
{
    None,
    Squiggly
}

/// <summary>Reusable presentation definition for spatial linework.</summary>
public readonly record struct SpatialLineStyle(
    string Id,
    string Stroke,
    double Width,
    double Opacity,
    SpatialLineEffect Effect = SpatialLineEffect.None,
    double EffectAmplitude = 0,
    double EffectFrequency = 0);

/// <summary>
/// Central catalog of reusable line styles. Styles are data so the same
/// definitions can be applied by different spatial scenes and renderers.
/// </summary>
public static class SpatialLineStyleCatalog
{
    public static SpatialLineStyle Neon { get; } =
        new("neon", "#00e5ff", 1.5, 0.95);

    public static SpatialLineStyle Blueprint { get; } =
        new("blueprint", "#78a9ff", 1.25, 0.9);

    public static SpatialLineStyle Warning { get; } =
        new("warning", "#ff9f1c", 2, 0.95);

    public static SpatialLineStyle Squiggly { get; } =
        new("squiggly", "#f7d154", 1.75, 0.95, SpatialLineEffect.Squiggly, 1.8, 6);

    public static IReadOnlyList<SpatialLineStyle> All { get; } =
        [Neon, Blueprint, Warning, Squiggly];
}
