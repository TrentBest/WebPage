namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>Focused MicroBundle for the Observer pattern.</summary>
public sealed class ObserverMicroBundle : SoftwarePatternMicroBundle
{
    public const int BundleId = 2207;

    public ObserverMicroBundle() : base(
        BundleId,
        "SOFTWARE PATTERN // OBSERVER",
        new MicroBundlePresentation(
            "Observer",
            "SOFTWARE PATTERNS // BEHAVIORAL",
            "Propagate capability changes to interested participants without coupling them to the source.",
            ["EVENT STREAM", "SUBSCRIBERS", "DECOUPLED REACTION"],
            "Green")) { }
}
