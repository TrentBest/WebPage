using TheSingularityWorkshop.Workshop.Architecture;

namespace TheSingularityWorkshop.Gui;

public static class SpaceElevatorGuiBuilder
{
    public static ElementBuilder Build(
        object receiver,
        SpaceElevatorStructure elevator,
        Action exit)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed")
            .Style("inset", "0")
            .Style("overflow", "auto")
            .Style("background", "rgba(1,3,9,.97)")
            .Style("color", "white")
            .Style("font-family", "Consolas, 'Courier New', monospace")
            .Style("padding", "2rem");

        var tower = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 420 620")
            .Style("width", "min(55vw, 420px)")
            .Style("height", "70vh")
            .Style("display", "block")
            .Style("margin", "1rem auto")
            .Style("background", "rgba(2,6,14,.9)")
            .Style("border", "1px solid rgba(255,255,255,.16)");

        tower.Child(WorkshopGui.Element(receiver, "line")
            .Attribute("x1", "210").Attribute("y1", "575")
            .Attribute("x2", "210").Attribute("y2", "45")
            .Attribute("stroke", "rgba(255,255,255,.65)").Attribute("stroke-width", "3"));
        tower.Child(WorkshopGui.Element(receiver, "circle")
            .Attribute("cx", "210").Attribute("cy", "45").Attribute("r", "22")
            .Attribute("fill", "none").Attribute("stroke", "white").Attribute("stroke-width", "3"));
        tower.Child(WorkshopGui.Element(receiver, "text")
            .Attribute("x", "245").Attribute("y", "50")
            .Attribute("fill", "white").Text("ORBITAL STATION"));
        tower.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", "165").Attribute("y", "555").Attribute("width", "90").Attribute("height", "20")
            .Attribute("fill", "none").Attribute("stroke", "white"));
        tower.Child(WorkshopGui.Element(receiver, "text")
            .Attribute("x", "25").Attribute("y", "565")
            .Attribute("fill", "white").Text("WORKSHOP ANCHOR"));
        tower.Child(WorkshopGui.Element(receiver, "circle")
            .Attribute("cx", "210").Attribute("cy", "280").Attribute("r", "13")
            .Attribute("fill", "none").Attribute("stroke", "rgba(100,210,255,.8)").Attribute("stroke-width", "3"));
        tower.Child(WorkshopGui.Element(receiver, "text")
            .Attribute("x", "245").Attribute("y", "285")
            .Attribute("fill", "white").Text("CLIMBER"));

        root.Content(WorkshopGui.Element(receiver, "div")
            .Style("max-width", "850px")
            .Style("margin", "0 auto")
            .Child(WorkshopGui.Element(receiver, "div")
                .Style("font-size", "1.2rem")
                .Style("letter-spacing", ".18em")
                .Text("WORKSHOP // SPACE ELEVATOR"))
            .Child(WorkshopGui.Element(receiver, "div")
                .Style("opacity", ".65")
                .Style("margin-top", ".35rem")
                .Text("GROUND ANCHOR // TETHER // CLIMBER // COUNTERWEIGHT // ORBITAL STATION"))
            .Child(tower)
            .Child(WorkshopGui.Element(receiver, "div")
                .Style("text-align", "center")
                .Text($"TETHER {elevator.TetherLengthKilometers:0,0} km // STATION {elevator.StationAltitudeKilometers:0,0} km // {elevator.Station?.Label ?? "NO STATION"}"))
            .Child(WorkshopGui.Button(receiver)
                .Text("BACK TO WORKSHOP")
                .Style("display", "block")
                .Style("margin", "1rem auto")
                .Style("padding", ".6rem 1rem")
                .Style("background", "rgba(255,255,255,.06)")
                .Style("border", "1px solid rgba(255,255,255,.18)")
                .Style("color", "white")
                .OnClick(exit)));

        return root;
    }
}
