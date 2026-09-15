namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>Focused MicroBundle for the Command pattern.</summary>
public sealed class CommandMicroBundle : SoftwarePatternMicroBundle
{
    public const int BundleId = 2205;

    public CommandMicroBundle() : base(
        BundleId,
        "SOFTWARE PATTERN // COMMAND",
        new MicroBundlePresentation(
            "Command",
            "SOFTWARE PATTERNS // BEHAVIORAL",
            "Represent an operation as an explicit capability that can be invoked, queued, or undone.",
            ["ACTION OBJECT", "QUEUEABLE WORK", "UNDO BOUNDARY"],
            "Cyan")) { }
}
