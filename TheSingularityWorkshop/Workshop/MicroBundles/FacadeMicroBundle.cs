namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>Focused MicroBundle for the Facade pattern.</summary>
public sealed class FacadeMicroBundle : SoftwarePatternMicroBundle
{
    public const int BundleId = 2201;

    public FacadeMicroBundle() : base(
        BundleId,
        "SOFTWARE PATTERN // FACADE",
        new MicroBundlePresentation(
            "Facade",
            "SOFTWARE PATTERNS // STRUCTURAL",
            "Present a simple capability boundary over a more complicated subsystem.",
            ["SIMPLIFIED API", "SUBSYSTEM BOUNDARY", "CAPABILITY SHIELD"],
            "Cyan")) { }
}
