namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>Focused MicroBundle for the Factory pattern.</summary>
public sealed class FactoryMicroBundle : SoftwarePatternMicroBundle
{
    public const int BundleId = 2208;

    public FactoryMicroBundle() : base(
        BundleId,
        "SOFTWARE PATTERN // FACTORY",
        new MicroBundlePresentation(
            "Factory",
            "SOFTWARE PATTERNS // CREATIONAL",
            "Select and construct a capability without exposing construction details to its consumer.",
            ["CREATION BOUNDARY", "TYPE SELECTION", "CONSTRUCTION POLICY"],
            "Yellow")) { }
}
