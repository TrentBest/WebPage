namespace TheSingularityWorkshop.Gui;

/// <summary>Named reusable line styles that can be applied by any Workshop perception surface.</summary>
public static class SpatialLineStyleCatalog
{
    public static SpatialLineStyle Neon => new("neon", "#00eaff", 0.42, 0.95);
    public static SpatialLineStyle Blueprint => new("blueprint", "#f7f7ff", 0.32, 0.8);
    public static SpatialLineStyle Warning => new("warning", "#ffd34d", 0.5, 0.95);
    public static SpatialLineStyle Squiggly => Neon with
    {
        Effect = SpatialLineEffect.Squiggly,
        EffectAmount = 0.9d
    };

    /// <summary>Returns the built-in styles available to a style selector.</summary>
    public static IReadOnlyList<SpatialLineStyle> All => [Neon, Blueprint, Warning, Squiggly];
}
