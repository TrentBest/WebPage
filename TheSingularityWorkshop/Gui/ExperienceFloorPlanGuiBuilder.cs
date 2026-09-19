using TheSingularityWorkshop.Workshop.Experience;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Default floor-plan manifestation for an Experience that has not yet
/// supplied a more specialized GUI builder. The host still enters a typed
/// <see cref="FloorPlan"/> view; this builder is simply its current renderer.
/// </summary>
public static class ExperienceFloorPlanGuiBuilder
{
    public static ElementBuilder Build(
        object receiver,
        ExperienceSpace space,
        Action exit)
    {
        var shell = WorkshopGui.Panel(receiver)
            .Style("position", "fixed")
            .Style("inset", "0")
            .Style("overflow", "auto")
            .Style("box-sizing", "border-box")
            .Style("padding", "clamp(1rem, 4vw, 4rem)")
            .Style("background", "#020a12")
            .Style("color", "#ffffff")
            .Style("font-family", "Consolas, 'Courier New', monospace");

        shell.Content(WorkshopGui.Element(receiver, "div")
            .Style("max-width", "900px")
            .Style("margin", "0 auto")
            .Style("border", "2px solid #00eaff88")
            .Style("padding", "clamp(1rem, 4vw, 3rem)")
            .Style("box-sizing", "border-box")
            .Style("background", "rgba(1,4,10,.84)")
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("font-size", ".55rem")
                .Style("letter-spacing", ".24em")
                .Style("color", "#00eaff")
                .Style("margin-bottom", ".7rem")
                .Text("EXPERIENCE // FLOOR PLAN"))
            .Content(WorkshopGui.Element(receiver, "h1")
                .Style("margin", "0 0 .8rem")
                .Style("font-size", "clamp(1.4rem, 5vw, 3rem)")
                .Text(space.Name))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("font-size", ".7rem")
                .Style("letter-spacing", ".12em")
                .Style("color", "#52e0ff")
                .Style("margin-bottom", "1.4rem")
                .Text(space.Kind))
            .Content(WorkshopGui.Element(receiver, "p")
                .Style("line-height", "1.65")
                .Style("max-width", "70ch")
                .Text(space.Description))
            .Content(WorkshopGui.Element(receiver, "p")
                .Style("line-height", "1.65")
                .Style("max-width", "70ch")
                .Text($"CAPABILITY // {space.Capability}"))
            .Content(WorkshopGui.Button(receiver)
                .Label("BACK TO WORKSHOP")
                .Style("margin-top", "1.5rem")
                .Style("padding", ".65rem 1rem")
                .Style("border", "1px solid #00eaff99")
                .Style("background", "transparent")
                .Style("color", "#ffffff")
                .Style("font-family", "inherit")
                .Style("letter-spacing", ".12em")
                .OnClick(exit)));

        return shell;
    }
}
