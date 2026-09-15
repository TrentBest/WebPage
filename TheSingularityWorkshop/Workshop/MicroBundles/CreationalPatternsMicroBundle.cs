namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>MicroBundle grouping the focused creational pattern bundles.</summary>
public sealed class CreationalPatternsMicroBundle : SoftwarePatternGroupMicroBundle
{
    public const int BundleId = 2210;

    public CreationalPatternsMicroBundle() : base(
        BundleId,
        "SOFTWARE PATTERNS // CREATIONAL",
        new MicroBundlePresentation(
            "Creational Patterns",
            "SOFTWARE PATTERNS // GROUP",
            "Patterns concerned with constructing capabilities while hiding construction policy.",
            ["FACTORY", "BUILDER", "PROVIDER"],
            "Yellow"),
        [
            new FactoryMicroBundle(),
            new BuilderMicroBundle(),
            new ProviderMicroBundle()
        ]) { }
}
