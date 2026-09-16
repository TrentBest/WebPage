namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Diegetic Workshop tram: a physical-looking transportation Experience that
/// carries the visitor through Singularity City toward the Space Elevator.
/// </summary>
public static class TramExperienceGuiBuilder
{
    public static ElementBuilder Build(
        object receiver,
        string destination,
        int progress,
        bool boarded,
        Func<Task> board,
        Action exit)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed")
            .Style("inset", "0")
            .Style("overflow", "auto")
            .Style("background", "#01040a")
            .Style("color", "white")
            .Style("font-family", "Consolas, 'Courier New', monospace")
            .Style("padding", "1rem")
            .Style("box-sizing", "border-box");

        root.Content(Header(receiver, exit));
        root.Content(Tram(receiver, destination, progress, boarded));
        root.Content(Announcement(receiver, boarded, progress));
        root.Content(Stops(receiver));

        if (!boarded)
        {
            root.Content(WorkshopGui.Button(receiver)
                .Label("BOARD TRAM // DEPART FOR SPACE ELEVATOR")
                .Style("display", "block")
                .Style("width", "100%")
                .Style("max-width", "1100px")
                .Style("margin", "1rem auto")
                .Style("padding", ".9rem")
                .Style("border", "2px solid rgba(82,224,255,.65)")
                .Style("background", "rgba(82,224,255,.06)")
                .Style("color", "#ffffff")
                .Style("font-family", "inherit")
                .Style("letter-spacing", ".12em")
                .OnClick(() => _ = board()));
        }

        return root;
    }

    private static ElementBuilder Header(object receiver, Action exit)
        => WorkshopGui.Panel(receiver)
            .Style("max-width", "1100px")
            .Style("margin", "0 auto .8rem")
            .Style("display", "flex")
            .Style("justify-content", "space-between")
            .Content(WorkshopGui.Element(receiver, "div")
                .Content(WorkshopGui.Element(receiver, "div")
                    .Style("font-size", "1.1rem")
                    .Style("letter-spacing", ".2em")
                    .Text("SINGULARITY CITY // TRAM"))
                .Content(WorkshopGui.Element(receiver, "div")
                    .Style("margin-top", ".3rem")
                    .Style("font-size", ".48rem")
                    .Style("opacity", ".62")
                    .Text("A VEHICLE YOU ENTER // A TRACK YOU FOLLOW // A CITY YOU PASS THROUGH")))
            .Content(WorkshopGui.Button(receiver)
                .Label("LEAVE TRAM")
                .Style("padding", ".5rem .7rem")
                .Style("background", "transparent")
                .Style("border", "1px solid rgba(255,255,255,.2)")
                .Style("color", "white")
                .Style("font-family", "inherit")
                .OnClick(exit));

    private static ElementBuilder Tram(object receiver, string destination, int progress, bool boarded)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 1200 560")
            .Style("display", "block")
            .Style("width", "100%")
            .Style("max-width", "1100px")
            .Style("height", "min(58vh,560px)")
            .Style("margin", "0 auto")
            .Style("background", "rgba(2,8,16,.9)")
            .Style("border", "1px solid rgba(82,224,255,.28)");

        svg.Child(WorkshopGui.Element(receiver, "path")
            .Attribute("d", "M90 440 C220 340 250 170 420 220 S620 470 770 300 S970 100 1110 170")
            .Attribute("fill", "none")
            .Attribute("stroke", "#52e0ff")
            .Attribute("stroke-opacity", ".55")
            .Attribute("stroke-width", "10"));
        svg.Child(WorkshopGui.Element(receiver, "path")
            .Attribute("d", "M90 462 C220 362 250 192 420 242 S620 492 770 322 S970 122 1110 192")
            .Attribute("fill", "none")
            .Attribute("stroke", "rgba(255,255,255,.2)")
            .Attribute("stroke-width", "3"));

        // Large vehicle envelope. The visitor is meant to read this as a place,
        // not as a button disguised as a rectangle.
        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", "405").Attribute("y", "235")
            .Attribute("width", "390").Attribute("height", "145")
            .Attribute("rx", "24")
            .Attribute("fill", "rgba(82,224,255,.06)")
            .Attribute("stroke", "#52e0ff")
            .Attribute("stroke-width", "5"));
        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", "450").Attribute("y", "265")
            .Attribute("width", "90").Attribute("height", "65")
            .Attribute("fill", "rgba(255,255,255,.04)")
            .Attribute("stroke", "rgba(255,255,255,.3)"));
        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", "555").Attribute("y", "265")
            .Attribute("width", "90").Attribute("height", "65")
            .Attribute("fill", "rgba(255,255,255,.04)")
            .Attribute("stroke", "rgba(255,255,255,.3)"));
        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", "660").Attribute("y", "265")
            .Attribute("width", "90").Attribute("height", "65")
            .Attribute("fill", "rgba(255,255,255,.04)")
            .Attribute("stroke", "rgba(255,255,255,.3)"));

        Label(svg, receiver, 600, 170, boarded ? "IN TRANSIT" : "TRAM READY", boarded ? "#52e0ff" : "white", "middle", "24");
        Label(svg, receiver, 600, 205, destination.ToUpperInvariant(), "#ffd34d", "middle", "13");
        Label(svg, receiver, 600, 360, $"TRACK PROGRESS // {progress}%", "rgba(255,255,255,.7)", "middle", "11");
        return svg;
    }

    private static ElementBuilder Announcement(object receiver, bool boarded, int progress)
        => WorkshopGui.Panel(receiver)
            .Style("max-width", "1100px")
            .Style("margin", ".8rem auto")
            .Style("padding", ".7rem")
            .Style("border", "1px solid rgba(255,211,77,.4)")
            .Style("background", "rgba(255,211,77,.025)")
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("font-size", ".48rem")
                .Style("letter-spacing", ".18em")
                .Style("color", "#ffd34d")
                .Text("ANNUNCIATION // SINGULARITY TRANSIT"))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".45rem")
                .Style("font-size", ".75rem")
                .Style("line-height", "1.5")
                .Text(!boarded
                    ? "NOW BOARDING // TRAM T-001 // SPACE ELEVATOR COMPLEX // DEPARTING WHEN CLEAR"
                    : progress < 100
                        ? $"TRAM T-001 IS UNDERWAY // NEXT STOP: SPACE ELEVATOR COMPLEX // ROUTE {progress}% COMPLETE"
                        : "ARRIVAL // SPACE ELEVATOR COMPLEX // PLEASE REMAIN SEATED UNTIL THE DOORS RELEASE"));

    private static ElementBuilder Stops(object receiver)
        => WorkshopGui.Panel(receiver)
            .Style("max-width", "1100px")
            .Style("margin", "0 auto")
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("font-size", ".48rem")
                .Style("letter-spacing", ".16em")
                .Style("opacity", ".6")
                .Text("ROUTE // WORKSHOP → CREATOR DISTRICT → COMMERCE → TRANSIT HUB → SPACE ELEVATOR"));

    private static void Label(ElementBuilder svg, object receiver, double x, double y, string text, string fill, string anchor, string size)
        => svg.Child(WorkshopGui.Element(receiver, "text")
            .Attribute("x", x).Attribute("y", y)
            .Attribute("fill", fill).Attribute("font-size", size)
            .Attribute("text-anchor", anchor)
            .Attribute("font-family", "Consolas, 'Courier New', monospace")
            .Text(text));
}
