using Microsoft.AspNetCore.Components;
using TheSingularityWorkshop.Workshop.MicroBundles;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Recursive GUI composition for the FSM Forge.
/// The builder consumes the Forge MicroBundle facade and its semantic presentation
/// contract; it does not reach through the Experience into Forge infrastructure.
/// </summary>
public static class FsmForgeGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Magenta = "#ff2cff";
    private const string Green = "#52e05a";
    private const string Yellow = "#ffd34d";
    private const string White = "#ffffff";
    private const string Ink = "#01040a";

    public static ElementBuilder Build(
        object receiver,
        FsmForgeMicroBundle forge,
        Action exitForge)
    {
        var workspace = WorkshopGui.MultiPanel(receiver)
            .Style("height", "100%")
            .Style("min-height", "0")
            .Style("display", "grid")
            .Style("grid-template-columns", "minmax(0, 1fr) minmax(280px, 34%)")
            .Style("gap", ".75rem")
            .Style("overflow", "hidden");

        workspace.Content(Stock(receiver, forge));
        workspace.Content(Preview(receiver, forge, exitForge));
        return workspace;
    }

    private static ElementBuilder Stock(object receiver, FsmForgeMicroBundle forge)
    {
        var panel = WorkshopGui.Panel(receiver)
            .Style("min-width", "0")
            .Style("min-height", "0")
            .Style("padding", "1rem")
            .Style("border", "1px solid rgba(0,234,255,.2)")
            .Style("background", "rgba(2,8,17,.92)")
            .Style("overflow", "hidden");

        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("color", Cyan)
            .Style("font-size", ".5rem")
            .Style("letter-spacing", ".18em")
            .Text(forge.Presentation.Surface + " // STOCK"));
        panel.Content(WorkshopGui.Element(receiver, "h2")
            .Style("margin", ".45rem 0 .25rem")
            .Style("font-size", "clamp(1.2rem,2vw,1.8rem)")
            .Text(forge.Presentation.Title));
        panel.Content(WorkshopGui.Element(receiver, "p")
            .Style("margin", "0 0 .8rem")
            .Style("color", "#718e99")
            .Style("font-family", "system-ui, sans-serif")
            .Style("font-size", ".75rem")
            .Text(forge.Presentation.Description));

        var products = WorkshopGui.MultiPanel(receiver)
            .Style("display", "grid")
            .Style("grid-template-columns", "repeat(auto-fit,minmax(180px,1fr))")
            .Style("gap", ".55rem")
            .Style("min-height", "0");

        foreach (var product in FsmForgeCatalog.Products)
            products.Content(Product(receiver, forge, product));

        panel.Content(products);

        var workbench = WorkshopGui.Panel(receiver)
            .Style("margin-top", ".75rem")
            .Style("padding", ".75rem")
            .Style("border", "1px solid rgba(82,224,90,.2)")
            .Style("background", "rgba(82,224,90,.03)");

        workbench.Content(WorkshopGui.Element(receiver, "div")
            .Style("color", Green)
            .Style("font-size", ".45rem")
            .Style("letter-spacing", ".16em")
            .Text("WORKBENCH // AUTHORING"));

        workbench.Content(WorkshopGui.Element(receiver, "div")
            .Style("color", "#78939c")
            .Style("font-size", ".58rem")
            .Style("margin", ".35rem 0 .55rem")
            .Text(forge.Workbench.SelectedStockId is null
                ? "Select an ingot from stock."
                : $"SELECTED // {forge.Workbench.SelectedStockId}"));

        var workbenchActions = WorkshopGui.MultiPanel(receiver)
            .Style("display", "flex")
            .Style("flex-wrap", "wrap")
            .Style("gap", ".35rem");

        workbenchActions.Content(Action(receiver, "PLACE STATE INGOT", forge.Workbench.PlaceSelectedState, Cyan));

        if (forge.Workbench.StatePlates.Count > 0)
            workbenchActions.Content(Action(receiver, "NAME FIRST STATE // IDLE", () => forge.Workbench.NameState(0, "Idle"), Yellow));

        if (forge.Workbench.StatePlates.Count > 1)
            workbenchActions.Content(Action(receiver, "NAME SECOND STATE // RUNNING", () => forge.Workbench.NameState(1, "Running"), Yellow));

        if (forge.Workbench.StatePlates.Count >= 2)
        {
            workbenchActions.Content(Action(
                receiver,
                "PLACE TRANSITION // RUN",
                () => forge.Workbench.PlaceSelectedTransition(
                    forge.Workbench.StatePlates[0].Name,
                    forge.Workbench.StatePlates[1].Name,
                    "RUN"),
                Magenta));
        }

        workbench.Content(workbenchActions);

        foreach (var plate in forge.Workbench.StatePlates)
            workbench.Content(WorkshopGui.Element(receiver, "div")
                .Style("display", "inline-block")
                .Style("margin", ".45rem .35rem 0 0")
                .Style("padding", ".4rem .6rem")
                .Style("border", $"1px solid {Cyan}44")
                .Style("color", White)
                .Style("font-size", ".55rem")
                .Text($"STATE PLATE // {plate.Name}"));

        foreach (var plate in forge.Workbench.TransitionPlates)
            workbench.Content(WorkshopGui.Element(receiver, "div")
                .Style("display", "inline-block")
                .Style("margin", ".45rem .35rem 0 0")
                .Style("padding", ".4rem .6rem")
                .Style("border", $"1px solid {Magenta}44")
                .Style("color", White)
                .Style("font-size", ".55rem")
                .Text($"TRANSITION PLATE // {plate.FromState} → {plate.ToState} // {plate.Signal}"));

        panel.Content(workbench);
        return panel;
    }

    private static ElementBuilder Product(object receiver, FsmForgeMicroBundle forge, FsmForgeProduct product)
    {
        var accent = product.Category switch
        {
            "STATE LIFECYCLE" => Cyan,
            "FSM STRUCTURE" => Magenta,
            "RUNTIME" => Green,
            _ => Yellow
        };

        var card = WorkshopGui.Panel(receiver)
            .Style("min-width", "0")
            .Style("padding", ".65rem")
            .Style("border", $"1px solid {accent}33")
            .Style("background", $"{accent}06")
.Style("gap", ".25rem")
            .Style("cursor", "pointer")
            .OnClick(() => forge.Workbench.SelectStock(product.Id));

        card.Content(WorkshopGui.Element(receiver, "div")
            .Style("color", accent)
            .Style("font-size", ".42rem")
            .Style("letter-spacing", ".12em")
            .Text(product.Kind));
        card.Content(WorkshopGui.Element(receiver, "strong")
            .Style("font-size", ".72rem")
            .Style("color", White)
            .Text(product.Name));
        card.Content(WorkshopGui.Element(receiver, "p")
            .Style("margin", ".25rem 0")
            .Style("color", "#78939c")
            .Style("font-family", "system-ui, sans-serif")
            .Style("font-size", ".62rem")
            .Text(product.Description));
        card.Content(WorkshopGui.Element(receiver, "pre")
            .Style("margin", "0")
            .Style("padding", ".4rem")
            .Style("white-space", "pre-wrap")
            .Style("font-size", ".52rem")
            .Style("color", "#b9d8df")
            .Style("background", Ink)
            .Style("border", "1px solid rgba(255,255,255,.06)")
            .Text(product.Scaffold));
        return card;
    }

    private static ElementBuilder Preview(
        object receiver,
        FsmForgeMicroBundle forge,
        Action exitForge)
    {
        var panel = WorkshopGui.Panel(receiver)
            .Style("min-width", "0")
            .Style("min-height", "0")
            .Style("padding", "1rem")
            .Style("border", "1px solid rgba(255,44,255,.25)")
            .Style("background", "rgba(7,2,12,.94)")
            .Style("overflow", "hidden");

        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("color", Magenta)
            .Style("font-size", ".5rem")
            .Style("letter-spacing", ".18em")
            .Text("LIVE FORGE // PREVIEW"));
        panel.Content(WorkshopGui.Element(receiver, "h2")
            .Style("margin", ".45rem 0")
            .Style("font-size", "1.35rem")
            .Text(forge.Presentation.Title));

        var state = WorkshopGui.Panel(receiver)
            .Style("align-items", "center")
            .Style("justify-content", "center")
            .Style("min-height", "150px")
            .Style("padding", "1rem")
            .Style("border", $"1px solid {StateAccent(forge.State)}55")
            .Style("background", $"{StateAccent(forge.State)}08")
            .Style("text-align", "center");
        state.Content(WorkshopGui.Element(receiver, "div")
            .Style("color", "#58747e")
            .Style("font-size", ".45rem")
            .Style("letter-spacing", ".16em")
            .Text("CURRENT STATE"));
        state.Content(WorkshopGui.Element(receiver, "strong")
            .Style("margin", ".35rem 0")
            .Style("font-size", "clamp(1.6rem,4vw,2.7rem)")
            .Style("color", StateAccent(forge.State))
            .Text(forge.State));
        state.Content(WorkshopGui.Element(receiver, "div")
            .Style("color", White)
            .Style("font-size", ".55rem")
            .Text($"UPDATE COUNT // {forge.TickCount}"));
        panel.Content(state);

        var controls = WorkshopGui.MultiPanel(receiver)
            .Style("display", "grid")
            .Style("grid-template-columns", "repeat(3,1fr)")
            .Style("gap", ".35rem")
            .Style("margin-top", ".65rem");
        controls.Content(Action(receiver, "FORGE", forge.Start, Cyan));
        controls.Content(Action(receiver, "TICK", forge.Tick, Green));
        controls.Content(Action(receiver, "RESET", forge.Reset, Yellow));
        panel.Content(controls);

        var events = WorkshopGui.Panel(receiver)
            .Style("margin-top", ".65rem")
            .Style("padding", ".6rem")
            .Style("min-height", "0")
            .Style("border", "1px solid rgba(0,234,255,.12)")
            .Style("background", Ink);
        events.Content(WorkshopGui.Element(receiver, "div")
            .Style("color", Cyan)
            .Style("font-size", ".42rem")
            .Style("letter-spacing", ".12em")
            .Text("LIFECYCLE TRACE"));

        foreach (var entry in forge.Events.TakeLast(8))
            events.Content(WorkshopGui.Element(receiver, "div")
                .Style("color", "#8caab3")
                .Style("font-size", ".55rem")
                .Style("line-height", "1.45")
                .Text($"> {entry}"));

        panel.Content(events);
        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("margin-top", "auto")
            .Style("padding-top", ".7rem")
            .Style("color", "#58747e")
            .Style("font-family", "system-ui, sans-serif")
            .Style("font-size", ".65rem")
            .Text("The preview is driven by the real FSM_API lifecycle: OnEnter → OnUpdate → transition → OnExit → OnEnter."));
        panel.Content(Action(receiver, "← RETURN TO WORKSHOP", exitForge, Magenta));
        return panel;
    }

    private static ElementBuilder Action(object receiver, string label, Action action, string accent)
        => WorkshopGui.Button(receiver)
            .Label(label)
            .Style("padding", ".65rem .4rem")
            .Style("border", $"1px solid {accent}66")
            .Style("background", $"{accent}0c")
            .Style("color", accent)
            .Style("font-family", "inherit")
            .Style("font-size", ".5rem")
            .Style("letter-spacing", ".1em")
            .Style("cursor", "pointer")
            .OnClick(action);

    private static string StateAccent(string state)
        => state switch
        {
            "Idle" => Cyan,
            "Forging" => Magenta,
            "Finished" => Green,
            _ => Yellow
        };
}
