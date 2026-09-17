using System.Collections.ObjectModel;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>
/// Declares the semantic vocabulary currently manifested by the Web GUI renderer.
/// This is deliberately data, not rendering logic, so other platform manifests can
/// expose the same vocabulary without coupling the shared builder to a platform.
/// </summary>
public static class WebGuiManifest
{
    private static readonly string[] SupportedKinds =
    {
        "Panel",
        "Button",
        "Image",
        "Text",
        "Warning",
        "Stack",
        "Grid"
    };

    /// <summary>Gets the immutable list of web-manifested semantic kinds.</summary>
    public static IReadOnlyList<string> Kinds { get; } =
        new ReadOnlyCollection<string>(SupportedKinds);

    /// <summary>Determines whether a semantic kind has a known web manifestation.</summary>
    public static bool Supports(string kind)
        => SupportedKinds.Contains(kind, StringComparer.OrdinalIgnoreCase);
}
