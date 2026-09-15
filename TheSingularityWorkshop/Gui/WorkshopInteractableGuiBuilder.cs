using TheSingularityWorkshop.Workshop.Interaction;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Recursive GUI manifestation for Workshop interactables.
/// A sign is a child presentation of the interactable, while the sign face is
/// rendered as an overlay so the same interaction can work in any Experience.
/// </summary>
public static class WorkshopInteractableGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string White = "#ffffff";
    private const string Ink = "#01040a";

    public static ElementBuilder SignButton(
        object receiver,
        WorkshopInteractable interactable,
        Action<WorkshopSign> showSign)
    {
        ArgumentNullException.ThrowIfNull(interactable);
        ArgumentNullException.ThrowIfNull(showSign);

        if (interactable.Sign is null)
            return WorkshopGui.Element(receiver, "span").Style("display", "none");

        var sign = interactable.Sign;
        return WorkshopGui.Button(receiver)
            .Label("SIGN")
            .Style("position", "absolute")
            .Style("right", ".45rem")
            .Style("bottom", ".45rem")
            .Style("z-index", "10")
            .Style("padding", ".28rem .42rem")
            .Style("border", $"1px solid {Cyan}88")
            .Style("background", "rgba(1,4,10,.88)")
            .Style("color", White)
            .Style("font-family", "inherit")
            .Style("font-size", ".42rem")
            .Style("letter-spacing", ".12em")
            .Style("cursor", "pointer")
            .AriaLabel($"Read sign: {sign.Title}")
            .OnClick(() => showSign(sign))
            .PreventDefault("onclick");
    }

    public static ElementBuilder SignFace(
        object receiver,
        WorkshopSign? sign,
        Action close)
    {
        if (sign is null)
            return WorkshopGui.Element(receiver, "span").Style("display", "none");

        var overlay = WorkshopGui.Element(receiver, "section")
            .Style("position", "fixed")
            .Style("inset", "0")
            .Style("z-index", "100")
            .Style("display", "flex")
            .Style("align-items", "center")
            .Style("justify-content", "center")
            .Style("padding", "1rem")
            .Style("box-sizing", "border-box")
            .Style("background", "rgba(0,0,0,.72)")
            .Attribute("role", "dialog")
            .Attribute("aria-modal", "true")
            .Attribute("aria-label", sign.Title);

        var face = WorkshopGui.Panel(receiver)
            .Style("width", "min(560px, 94vw)")
            .Style("max-height", "82vh")
            .Style("overflow", "auto")
            .Style("padding", "1.4rem")
            .Style("box-sizing", "border-box")
            .Style("border", $"2px solid {Cyan}99")
            .Style("background", Ink)
            .Style("box-shadow", $"0 0 80px {Cyan}22")
            .Style("font-family", "Consolas, 'Courier New', monospace");

        face.Content(WorkshopGui.Element(receiver, "div")
            .Style("font-size", ".52rem")
            .Style("letter-spacing", ".24em")
            .Style("color", Cyan)
            .Style("margin-bottom", ".6rem")
            .Text(sign.Kicker));

        face.Content(WorkshopGui.Element(receiver, "h2")
            .Style("margin", "0 0 1rem 0")
            .Style("font-size", "clamp(1.1rem, 3vw, 1.8rem)")
            .Style("letter-spacing", ".08em")
            .Style("color", White)
            .Text(sign.Title));

        foreach (var line in sign.Lines)
        {
            face.Content(WorkshopGui.Element(receiver, "p")
                .Style("margin", ".55rem 0")
                .Style("line-height", "1.55")
                .Style("color", White)
                .Text(line));
        }

        face.Content(WorkshopGui.Button(receiver)
            .Label("CLOSE SIGN")
            .Style("margin-top", "1rem")
            .Style("padding", ".5rem .8rem")
            .Style("border", $"1px solid {Cyan}88")
            .Style("background", "transparent")
            .Style("color", White)
            .Style("font-family", "inherit")
            .Style("letter-spacing", ".1em")
            .Style("cursor", "pointer")
            .OnClick(close));

        overlay.Content(face);
        return overlay;
    }
}
