namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>Focused MicroBundle for the Builder pattern.</summary>
public sealed class BuilderMicroBundle : SoftwarePatternMicroBundle
{
    public const int BundleId = 2206;

    public BuilderMicroBundle() : base(
        BundleId,
        "SOFTWARE PATTERN // BUILDER",
        new MicroBundlePresentation(
            "Builder",
            "SOFTWARE PATTERNS // CREATIONAL",
            "Construct a rich capability or presentation through an expressive sequence of steps.",
            ["FLUENT CONSTRUCTION", "RECURSIVE BUILDING", "STEPWISE COMPOSITION"],
            "Magenta")) { }
}
