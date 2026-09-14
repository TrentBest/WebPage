using Microsoft.AspNetCore.Components;
using TheSingularityWorkshop.Infrastructure.FsmForge;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Recursive GUI composition for the FSM Forge.
/// The Forge exposes reusable FSM products and a live preview without exposing
/// the underlying Blazor implementation to the page.
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
        FsmForgePreview preview,
        Action startPreview,
        Action tickPreview,
        Action stopPreview,
        Action exitForge)
    {
        var workspace = WorkshopGui.MultiPanel(receiver)
            .Style("height", "100%")
            .Style("min-height", "0")
            .Style("display", "grid")
            .Style("grid-template-columns", "minmax(0, 1fr) minmax(280px, 34%)")
            .Style("gap", ".75rem")
            .Style("overflow", "hidden");

        workspace.Content(Stock(receiver));
        workspace.Content(Preview(receiver, preview, startPreview, tickPreview, stopPreview, exitForge));
        return workspace;
    }

    private static ElementBuilder Stock(object receiver)
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
            .Text("FSM FORGE // STOCK"));
        panel.Content(WorkshopGui.Element(receiver, "h2")
            .Style("margin", ".45rem 0 .25rem")
            .Style("font-size", "clamp(1.2rem,2vw,1.8rem)")
            .Text("Raw material → reusable behavior"));
        panel.Content(WorkshopGui.Element(receiver, "p")
            .Style("margin", "0 0 .8rem")
            .Style("color", "#718e99")
            .Style("font-family", "system-ui, sans-serif")
            .Style("font-size", ".75rem")
            .Text("Choose a piece of FSM behavior. Every product is a scaffold for the real FSM API, not a decorative code sample."));

        var products = WorkshopGui.MultiPanel(receiver)
            .Style("display", "grid")
            .Style("grid-template-columns", "repeat(auto-fit,minmax(180px,1fr))")
            .Style("gap", ".55rem")
            .Style("min-height", "0");

        foreach (var product in FsmForgeCatalog.Products)
            products.Content(Product(receiver, product));

        panel.Content(products);
        return panel;
    }

    private static ElementBuilder Product(object receiver, FsmForgeProduct product)
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
            .Style("gap", ".25rem");

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
        FsmForgePreview preview,
        Action startPreview,
        Action tickPreview,
        Action stopPreview,
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
            .Text("Watch the FSM work"));

        var state = WorkshopGui.Panel(receiver)
            .Style("align-items", "center")
            .Style("justify-content", "center")
            .Style("min-height", "150px")
            .Style("padding", "1rem")
            .Style("border", $"1px solid {StateAccent(preview.State)}55")
            .Style("background", $"{StateAccent(preview.State)}08")
            .Style("text-align", "center");
        state.Content(WorkshopGui.Element(receiver, "div")
            .Style("color", "#58747e")
            .Style("font-size", ".45rem")
            .Style("letter-spacing", ".16em")
            .Text("CURRENT STATE"));
        state.Content(WorkshopGui.Element(receiver, "strong")
            .Style("margin", ".35rem 0")
            .Style("font-size", "clamp(1.6rem,4vw,2.7rem)")
            .Style("color", StateAccent(preview.State))
            .Text(preview.State));
        state.Content(WorkshopGui.Element(receiver, "div")
            .Style("color", White)
            .Style("font-size", ".55rem")
            .Text($"UPDATE COUNT // {preview.TickCount}"));
        panel.Content(state);

        var controls = WorkshopGui.MultiPanel(receiver)
            .Style("display", "grid")
            .Style("grid-template-columns", "repeat(3,1fr)")
            .Style("gap", ".35rem")
            .Style("margin-top", ".65rem");
        controls.Content(Action(receiver, "FORGE", startPreview, Cyan));
        controls.Content(Action(receiver, "TICK", tickPreview, Green));
        controls.Content(Action(receiver, "RESET", stopPreview, Yellow));
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

        var trace = preview.Events.TakeLast(8).ToArray();
        foreach (var entry in trace)
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
