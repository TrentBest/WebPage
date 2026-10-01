namespace TheSingularityWorkshop.Infrastructure.FsmForge;

/// <summary>A reusable product stocked by the FSM Forge.</summary>
public sealed record FsmForgeProduct(
    string Id,
    string Name,
    string Kind,
    string Description,
    string Scaffold,
    string Category);

/// <summary>Initial stock for the FSM Forge.</summary>
public static class FsmForgeCatalog
{
    public static IReadOnlyList<FsmForgeProduct> Products { get; } =
    [
        new("state-ingot", "State Ingot", "STATE", "A blank state substrate. Place it on a State Plate and give it a name.", "STATE → STATE PLATE", "STATE LIFECYCLE"),
        new("transition-ingot", "Transition Ingot", "TRANSITION", "A transition substrate connecting two state plates.", "STATE A → STATE B", "FSM STRUCTURE"),
        new("condition-ingot", "Condition Ingot", "CONDITION", "A condition substrate for defining when a transition may fire.", "condition(ctx)", "FSM STRUCTURE"),
        new("on-enter", "OnEnter Scaffold", "STATE ACTION", "A ready-to-populate state-entry action.", "ctx => { }", "STATE LIFECYCLE"),
        new("on-update", "OnUpdate Scaffold", "STATE ACTION", "A ready-to-populate update action.", "ctx => { }", "STATE LIFECYCLE"),
        new("on-exit", "OnExit Scaffold", "STATE ACTION", "A ready-to-populate state-exit action.", "ctx => { }", "STATE LIFECYCLE"),
        new("conditional-transition", "Conditional Transition", "TRANSITION", "A transition scaffold driven by context.", "(ctx) => true/false", "FSM STRUCTURE"),
        new("any-state-transition", "Any-State Transition", "TRANSITION", "A transition that can react from any current state.", "AnyState → Target", "FSM STRUCTURE"),
        new("process-group", "Process Group", "RUNTIME", "A named execution lane.", "ProcessingGroup: Update", "RUNTIME"),
        new("manual-preview", "Manual Preview", "PREVIEW", "A preview mode that advances a live FSM one tick at a time.", "handle.Update()", "PREVIEW")
    ];
}
