namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>
/// Platform-neutral builder contract aligned with the WPF builder family.
/// The generic product is the manifestation produced by a builder.
/// </summary>
/// <typeparam name="TProduct">The platform-neutral or platform-specific product.</typeparam>
public interface ICoreGuiBuilder<out TProduct>
{
    /// <summary>Manifests the builder into its target representation.</summary>
    TProduct Build();

    /// <summary>Stable identifier for diagnostics and builder-tree tracing.</summary>
    string GetBuilderId();

    /// <summary>The recursive parent builder, or <see langword="null"/> for a root.</summary>
    object? Parent { get; }
}
