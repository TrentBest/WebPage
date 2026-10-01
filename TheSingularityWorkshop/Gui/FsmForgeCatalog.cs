namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Presentation-ready Forge products exposed by the Forge facade.
/// The MicroBundle owns the semantic identity; the GUI builder owns rendering.
/// </summary>
public sealed record FsmForgeProduct(
    string Id,
    string Category,
    string Kind,
    string Name,
    string Description,
    string Scaffold);

/// <summary>
/// Default product vocabulary for the FSM Forge facade.
/// This is deliberately data, not rendering logic.
/// </summary>
public static class FsmForgeCatalog
{
    public static IReadOnlyList<FsmForgeProduct> Products { get; } =
    [
        new(
            "state-ingot",
            "STATE LIFECYCLE",
            "LIFECYCLE",
            "State Context",
            "A controlled state boundary with enter, update, transition, and exit semantics.",
            "OnEnter()\nOnUpdate()\nTransition()\nOnExit()"),
        new(
            "transition-ingot",
            "FSM STRUCTURE",
            "STRUCTURE",
            "State Machine",
            "Composable states and transitions become the executable topology of the Forge.",
            "State A → State B\n       ↘ State C"),
        new(
            "process-group",
            "RUNTIME",
            "RUNTIME",
            "Execution Loop",
            "The runtime advances the machine through its authoritative lifecycle.",
            "Update()\n  ↓\nState → Transition\n  ↓\nNext State"),
        new(
            "manual-preview",
            "LIVE PREVIEW",
            "PREVIEW",
            "Forge Preview",
            "A live surface for exercising the actual FSM_API behavior rather than a simulation.",
            "START → UPDATE → TRANSITION\n              ↓\n             EXIT")
    ];
}
