namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>Focused MicroBundle for the Composite pattern.</summary>
public sealed class CompositeMicroBundle : SoftwarePatternMicroBundle
{
    public const int BundleId = 2202;

    public CompositeMicroBundle() : base(
        BundleId,
        "SOFTWARE PATTERN // COMPOSITE",
        new MicroBundlePresentation(
            "Composite",
            "SOFTWARE PATTERNS // STRUCTURAL",
            "Treat individual capabilities and compositions through one common boundary.",
            ["TREE COMPOSITION", "UNIFORM ACCESS", "RECURSIVE STRUCTURE"],
            "Magenta")) { }
}
