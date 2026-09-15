namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>MicroBundle grouping the focused structural pattern bundles.</summary>
public sealed class StructuralPatternsMicroBundle : SoftwarePatternGroupMicroBundle
{
    public const int BundleId = 2211;

    public StructuralPatternsMicroBundle() : base(
        BundleId,
        "SOFTWARE PATTERNS // STRUCTURAL",
        new MicroBundlePresentation(
            "Structural Patterns",
            "SOFTWARE PATTERNS // GROUP",
            "Patterns concerned with composing boundaries and structures around capabilities.",
            ["FACADE", "COMPOSITE"],
            "Magenta"),
        [
            new FacadeMicroBundle(),
            new CompositeMicroBundle()
        ]) { }
}
