namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// Semantic presentation contract owned by a MicroBundle.
/// A host decides how to render it; the capability decides what it is presenting.
/// This keeps presentation intent out of individual pages while avoiding a GUI
/// dependency in the Workshop runtime.
/// </summary>
public sealed record MicroBundlePresentation(
    string Title,
    string Surface,
    string Description,
    IReadOnlyList<string> Capabilities,
    string Accent = "Cyan");
