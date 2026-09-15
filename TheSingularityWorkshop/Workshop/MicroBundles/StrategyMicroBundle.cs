namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>Focused MicroBundle for the Strategy pattern.</summary>
public sealed class StrategyMicroBundle : SoftwarePatternMicroBundle
{
    public const int BundleId = 2203;

    public StrategyMicroBundle() : base(
        BundleId,
        "SOFTWARE PATTERN // STRATEGY",
        new MicroBundlePresentation(
            "Strategy",
            "SOFTWARE PATTERNS // BEHAVIORAL",
            "Swap an algorithm or policy without changing the capability boundary.",
            ["POLICY SELECTION", "ALGORITHM SWAP", "RUNTIME BEHAVIOR"],
            "Green")) { }
}
