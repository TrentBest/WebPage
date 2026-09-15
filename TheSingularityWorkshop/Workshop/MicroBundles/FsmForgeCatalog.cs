namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// A semantic Forge product exposed to a host for presentation.
/// This is deliberately data, not GUI markup.
/// </summary>
public sealed record FsmForgeProduct(
    string Category,
    string Kind,
    string Name,
    string Description,
    string Scaffold);

/// <summary>
/// The initial Forge product catalog. The Forge owns what it teaches;
/// a GUI builder owns how those products are rendered.
/// </summary>
public static class FsmForgeCatalog
{
    public static IReadOnlyList<FsmForgeProduct> Products { get; } =
    [
        new(
            "STATE LIFECYCLE",
            "CAPABILITY",
            "State Context",
            "Carry the stateful context that gives a process meaning.",
            "IStateContext\n  ↓\nState\n  ↓\nTransition"),
        new(
            "FSM STRUCTURE",
            "STRUCTURE",
            "State Machine",
            "Compose states, transitions, and lifecycle callbacks into executable structure.",
            "States + Transitions\n  ↓\nDefinition\n  ↓\nInstance"),
        new(
            "RUNTIME",
            "EXECUTION",
            "Processing Group",
            "Give a capability an ordered runtime update surface.",
            "Group\n  ↓\nUpdate()\n  ↓\nTransition"),
        new(
            "LIVE PREVIEW",
            "EXPERIENCE",
            "Working Forge",
            "See the actual lifecycle execute rather than reading a static diagram.",
            "Start → Update\n       ↓\n    Forging\n       ↓\n   Finished")
    ];
}
