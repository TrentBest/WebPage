namespace TheSingularityWorkshop.Infrastructure.FsmForge;

/// <summary>
/// A reusable product stocked by the FSM Forge.
/// Products are recipes/scaffolds for FSM behavior rather than implementation-specific UI.
/// </summary>
public sealed record FsmForgeProduct(
    string Id,
    string Name,
    string Kind,
    string Description,
    string Scaffold,
    string Category);

/// <summary>
/// Initial stock for the FSM Forge.
/// The catalog deliberately describes API capabilities without coupling the storefront
/// to a particular consumer's implementation.
/// </summary>
public static class FsmForgeCatalog
{
    public static IReadOnlyList<FsmForgeProduct> Products { get; } =
    [
        new(
            "on-enter",
            "OnEnter Scaffold",
            "STATE ACTION",
            "A ready-to-populate state-entry action. The FSM remains fully functional while you add the domain behavior.",
            "ctx =>\n{\n    // Your entry behavior here.\n}",
            "STATE LIFECYCLE"),
        new(
            "on-update",
            "OnUpdate Scaffold",
            "STATE ACTION",
            "A ready-to-populate update action. The FSM can execute it immediately on every applicable update.",
            "ctx =>\n{\n    // Your update behavior here.\n}",
            "STATE LIFECYCLE"),
        new(
            "on-exit",
            "OnExit Scaffold",
            "STATE ACTION",
            "A ready-to-populate state-exit action for cleanup, persistence, or handoff behavior.",
            "ctx =>\n{\n    // Your exit behavior here.\n}",
            "STATE LIFECYCLE"),
        new(
            "conditional-transition",
            "Conditional Transition",
            "TRANSITION",
            "A transition scaffold that connects two states through a context-driven condition.",
            "(ctx) =>\n{\n    // Return true when the FSM should transition.\n    return false;\n}",
            "FSM STRUCTURE"),
        new(
            "any-state-transition",
            "Any-State Transition",
            "TRANSITION",
            "A transition that can react from any current state when its condition is satisfied.",
            "FSM.AnyStateIdentifier → TargetState\ncondition: ctx => true/false",
            "FSM STRUCTURE"),
        new(
            "process-group",
            "Process Group",
            "RUNTIME",
            "A named execution lane for organizing when a family of FSM instances is updated.",
            "ProcessingGroup: \"Update\"",
            "RUNTIME"),
        new(
            "manual-preview",
            "Manual Preview",
            "PREVIEW",
            "A preview mode that advances a live FSM one tick at a time so its lifecycle can be observed before reuse.",
            "handle.Update();\nobserve handle.CurrentState;",
            "PREVIEW")
    ];
}
