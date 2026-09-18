using System;
using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.Workshop.Creation;

/// <summary>
/// A named Workshop artifact composed from a GUI definition and an optional FSM blueprint.
/// The artifact is declarative: it can be persisted, transported, inspected, and rehydrated
/// before any platform-specific runtime objects are created.
/// </summary>
public sealed class WorkshopAsset
{
    public WorkshopAsset(string name, GuiNode gui, FsmBlueprint? fsm = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Workshop asset name is required.", nameof(name));

        Name = name;
        Gui = gui ?? throw new ArgumentNullException(nameof(gui));
        Fsm = fsm;
    }

    public string Name { get; }
    public GuiNode Gui { get; }
    public FsmBlueprint? Fsm { get; }
}
