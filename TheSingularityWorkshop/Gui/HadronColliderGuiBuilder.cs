using TheSingularityWorkshop.Workshop.Chemistry;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Workshop presentation of the collider domain model. The collider remains a
/// domain object; this builder only turns its current state into an interactive
/// research instrument surface.
/// </summary>
public static class HadronColliderGuiBuilder
{
    public static ElementBuilder Build(
        object receiver,
        HadronColliderSimulator simulator,
        Action configure,
        Action inject,
        Action accelerate,
        Action collide,
        Action reset,
        Action exit)
    {
        var collision = simulator.LastCollision;
        var status = simulator.State.ToString().ToUpperInvariant();

        var ring = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 800 460")
            .Style("width", "min(90vw, 800px)")
            .Style("height", "auto")
            .Style("display", "block")
            .Style("margin", "1rem auto")
            .Style("background", "rgba(2,6,14,.9)")
            .Style("border", "1px solid rgba(255,255,255,.16)");

        ring.Child(WorkshopGui.Element(receiver, "ellipse")
            .Attribute("cx", "400")
            .Attribute("cy", "230")
            .Attribute("rx", "285")
            .Attribute("ry", "145")
            .Attribute("fill", "none")
            .Attribute("stroke", "rgba(100,210,255,.55)")
            .Attribute("stroke-width", "12"));

        ring.Child(WorkshopGui.Element(receiver, "ellipse")
            .Attribute("cx", "400")
            .Attribute("cy", "230")
            .Attribute("rx", "285")
            .Attribute("ry", "145")
            .Attribute("fill", "none")
            .Attribute("stroke", "rgba(255,255,255,.15)")
            .Attribute("stroke-width", "2")
            .Attribute("stroke-dasharray", "4 12"));

        ring.Child(WorkshopGui.Element(receiver, "line")
            .Attribute("x1", "365")
            .Attribute("y1", "230")
            .Attribute("x2", "435")
            .Attribute("y2", "230")
            .Attribute("stroke", "white")
            .Attribute("stroke-width", "4"));

        ring.Child(WorkshopGui.Element(receiver, "circle")
            .Attribute("cx", "400")
            .Attribute("cy", "230")
            .Attribute("r", "34")
            .Attribute("fill", "none")
            .Attribute("stroke", "white")
            .Attribute("stroke-width", "2"));

        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed")
            .Style("inset", "0")
            .Style("overflow", "auto")
            .Style("background", "rgba(1,3,9,.97)")
            .Style("color", "white")
            .Style("font-family", "Consolas, 'Courier New', monospace")
            .Style("padding", "2rem");

        root.Content(WorkshopGui.Element(receiver, "div")
            .Style("max-width", "900px")
            .Style("margin", "0 auto")
            .Child(WorkshopGui.Element(receiver, "div")
                .Style("font-size", "1.2rem")
                .Style("letter-spacing", ".18em")
                .Text("CHEMISTRY LAB // HADRON COLLIDER"))
            .Child(WorkshopGui.Element(receiver, "div")
                .Style("opacity", ".65")
                .Style("margin-top", ".35rem")
                .Text("BEAM INJECTION // ACCELERATION // COLLISION // EVENT ANALYSIS"))
            .Child(ring)
            .Child(WorkshopGui.Element(receiver, "div")
                .Style("display", "grid")
                .Style("grid-template-columns", "repeat(auto-fit,minmax(150px,1fr))")
                .Style("gap", ".5rem")
                .Child(Readout(receiver, "STATE", status))
                .Child(Readout(receiver, "BEAM ENERGY", $"{simulator.BeamEnergyTeV:0.###} TeV"))
                .Child(Readout(receiver, "LUMINOSITY", simulator.Luminosity.ToString("0.###")))
                .Child(Readout(receiver, "EVENT", collision?.EventId.ToString() ?? "—")))
            .Child(WorkshopGui.Element(receiver, "div")
                .Style("display", "flex")
                .Style("flex-wrap", "wrap")
                .Style("gap", ".5rem")
                .Style("margin", "1rem 0")
                .Child(ActionButton(receiver, "CONFIGURE 7 TeV", configure))
                .Child(ActionButton(receiver, "INJECT", inject))
                .Child(ActionButton(receiver, "ACCELERATE", accelerate))
                .Child(ActionButton(receiver, "COLLIDE", collide))
                .Child(ActionButton(receiver, "RESET", reset))
                .Child(ActionButton(receiver, "BACK", exit)))
            .Child(WorkshopGui.Element(receiver, "div")
                .Style("border", "1px solid rgba(255,255,255,.12)")
                .Style("padding", "1rem")
                .Text(collision is null
                    ? "NO COLLISION EVENT // INSTRUMENT READY"
                    : $"EVENT {collision.Value.EventId:000000} // {collision.Value.DetectorChannel} // √s = {collision.Value.CenterOfMassEnergyTeV:0.###} TeV")));

        return root;
    }

    private static ElementBuilder Readout(object receiver, string label, string value)
        => WorkshopGui.Element(receiver, "div")
            .Style("border", "1px solid rgba(255,255,255,.12)")
            .Style("padding", ".65rem")
            .Child(WorkshopGui.Element(receiver, "div").Style("opacity", ".5").Text(label))
            .Child(WorkshopGui.Element(receiver, "div").Style("margin-top", ".2rem").Text(value));

    private static ElementBuilder ActionButton(object receiver, string label, Action action)
        => WorkshopGui.Button(receiver)
            .Text(label)
            .Style("padding", ".55rem .75rem")
            .Style("background", "rgba(255,255,255,.06)")
            .Style("border", "1px solid rgba(255,255,255,.18)")
            .Style("color", "white")
            .OnClick(action);
}
