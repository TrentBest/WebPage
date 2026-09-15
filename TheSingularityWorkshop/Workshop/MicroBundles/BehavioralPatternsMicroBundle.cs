namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>MicroBundle grouping the focused behavioral pattern bundles.</summary>
public sealed class BehavioralPatternsMicroBundle : SoftwarePatternGroupMicroBundle
{
    public const int BundleId = 2212;

    public BehavioralPatternsMicroBundle() : base(
        BundleId,
        "SOFTWARE PATTERNS // BEHAVIORAL",
        new MicroBundlePresentation(
            "Behavioral Patterns",
            "SOFTWARE PATTERNS // GROUP",
            "Patterns concerned with runtime behavior, communication, and changing policy.",
            ["COMMAND", "OBSERVER", "STRATEGY"],
            "Green"),
        [
            new CommandMicroBundle(),
            new ObserverMicroBundle(),
            new StrategyMicroBundle()
        ]) { }
}
