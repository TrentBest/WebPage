namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>Focused MicroBundle for the Provider pattern.</summary>
public sealed class ProviderMicroBundle : SoftwarePatternMicroBundle
{
    public const int BundleId = 2204;

    public ProviderMicroBundle() : base(
        BundleId,
        "SOFTWARE PATTERN // PROVIDER",
        new MicroBundlePresentation(
            "Provider",
            "SOFTWARE PATTERNS // CREATIONAL",
            "Expose a capability through an implementation supplied by a provider boundary.",
            ["CAPABILITY SUPPLY", "IMPLEMENTATION BOUNDARY", "HOST ADAPTATION"],
            "Yellow")) { }
}
