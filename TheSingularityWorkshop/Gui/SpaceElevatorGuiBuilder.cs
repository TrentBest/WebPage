using TheSingularityWorkshop.Workshop.Architecture;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Recursive fluent presentation of the Space Elevator Experience.
/// The ground facility is intentionally a transportation district: six terminals
/// surround the cable, freight and passenger traffic have distinct concourses,
/// and the orbital station is presented as the much larger destination.
/// </summary>
public static class SpaceElevatorGuiBuilder
{
    public static ElementBuilder Build(
        object receiver,
        SpaceElevatorStructure elevator,
        Action exit)
    {
        ArgumentNullException.ThrowIfNull(elevator);
        ArgumentNullException.ThrowIfNull(exit);

        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed")
            .Style("inset", "0")
            .Style("overflow", "auto")
            .Style("background", "rgba(1,3,9,.98)")
            .Style("color", "white")
            .Style("font-family", "Consolas, 'Courier New', monospace")
            .Style("padding", "1.2rem");

        root.Content(Hero(receiver, elevator, exit));
        root.Content(GroundHub(receiver, elevator));
        root.Content(TetherBlueprint(receiver, elevator));
        root.Content(OrbitalStation(receiver, elevator));
        root.Content(FacilityManifest(receiver, elevator));

        return root;
    }

    private static ElementBuilder Hero(object receiver, SpaceElevatorStructure elevator, Action exit)
        => WorkshopGui.Panel(receiver)
            .Style("max-width", "1500px")
            .Style("margin", "0 auto")
            .Style("display", "flex")
            .Style("justify-content", "space-between")
            .Style("gap", "1rem")
            .Style("align-items", "flex-start")
            .Content(WorkshopGui.Element(receiver, "div")
                .Content(WorkshopGui.Element(receiver, "div")
                    .Style("font-size", "1.15rem")
                    .Style("letter-spacing", ".2em")
                    .Text("WORKSHOP // SPACE ELEVATOR EXPERIENCE"))
                .Content(WorkshopGui.Element(receiver, "div")
                    .Style("margin-top", ".35rem")
                    .Style("opacity", ".65")
                    .Text("A TRANSPORTATION DISTRICT BUILT AROUND THE CABLE // NOT A TOWER"))
                .Content(WorkshopGui.Element(receiver, "div")
                    .Style("margin-top", ".65rem")
                    .Style("font-size", ".52rem")
                    .Style("letter-spacing", ".08em")
                    .Text($"{elevator.FreightTerminalCount} FREIGHT TERMINALS // {elevator.PassengerTerminalCount} PASSENGER TERMINALS // {elevator.MidTetherPowerTransferStations} POWER TRANSFER STATIONS")))
            .Content(WorkshopGui.Button(receiver)
                .Text("BACK TO WORKSHOP")
                .Style("padding", ".55rem .8rem")
                .Style("background", "rgba(255,255,255,.05)")
                .Style("border", "1px solid rgba(255,255,255,.2)")
                .Style("color", "white")
                .Style("font-family", "inherit")
                .OnClick(exit));

    private static ElementBuilder GroundHub(object receiver, SpaceElevatorStructure elevator)
    {
        var panel = Section(receiver, "GROUND HUB // HEXAGONAL TERMINAL COMPLEX", "FREIGHT INBOUND // FREIGHT OUTBOUND // PASSENGERS // SECURITY // LIFE-SAFETY // TRAM TRANSFER");

        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 1200 760")
            .Style("width", "100%")
            .Style("height", "min(66vh, 760px)")
            .Style("margin-top", ".8rem")
            .Style("background", "rgba(2,6,14,.82)")
            .Style("border", "1px solid rgba(255,255,255,.15)");

        // Vast hub envelope.
        svg.Child(WorkshopGui.Element(receiver, "polygon")
            .Attribute("points", "600,70 950,270 950,490 600,690 250,490 250,270")
            .Attribute("fill", "none")
            .Attribute("stroke", "rgba(0,220,255,.55)")
            .Attribute("stroke-width", "4"));
        svg.Child(WorkshopGui.Element(receiver, "polygon")
            .Attribute("points", "600,145 875,305 875,455 600,615 325,455 325,305")
            .Attribute("fill", "rgba(0,220,255,.025)")
            .Attribute("stroke", "rgba(255,255,255,.22)")
            .Attribute("stroke-width", "2"));

        // Central cable and loading core.
        svg.Child(WorkshopGui.Element(receiver, "circle")
            .Attribute("cx", "600").Attribute("cy", "380").Attribute("r", "92")
            .Attribute("fill", "rgba(0,220,255,.04)")
            .Attribute("stroke", "rgba(0,220,255,.8)")
            .Attribute("stroke-width", "5"));
        svg.Child(WorkshopGui.Element(receiver, "circle")
            .Attribute("cx", "600").Attribute("cy", "380").Attribute("r", "28")
            .Attribute("fill", "none")
            .Attribute("stroke", "white")
            .Attribute("stroke-width", "4"));
        Label(svg, receiver, 600, 376, "CABLE / CLIMBER CORE", "white", "middle", "16");
        Label(svg, receiver, 600, 400, "TRI-RAIL MAGLEV", "rgba(255,255,255,.6)", "middle", "11");

        var positions = new[]
        {
            (600d, 112d), (850d, 255d), (850d, 505d), (600d, 648d), (350d, 505d), (350d, 255d)
        };

        for (var i = 0; i < elevator.Terminals.Count && i < positions.Length; i++)
        {
            var terminal = elevator.Terminals[i];
            var (x, y) = positions[i];
            var freight = terminal.IsFreight;
            var accent = freight ? "#ff9f43" : "#52e0ff";

            svg.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", "600").Attribute("y1", "380")
                .Attribute("x2", x.ToString("0")).Attribute("y2", y.ToString("0"))
                .Attribute("stroke", accent).Attribute("stroke-opacity", ".5")
                .Attribute("stroke-width", "3"));

            svg.Child(WorkshopGui.Element(receiver, "rect")
                .Attribute("x", (x - 82).ToString("0")).Attribute("y", (y - 42).ToString("0"))
                .Attribute("width", "164").Attribute("height", "84")
                .Attribute("fill", "rgba(1,4,10,.94)")
                .Attribute("stroke", accent).Attribute("stroke-width", "3"));

            Label(svg, receiver, x, y - 8, terminal.Label, accent, "middle", "13");
            Label(svg, receiver, x, y + 13, freight ? "FREIGHT / LOADING" : "PASSENGER / BOARDING", "rgba(255,255,255,.65)", "middle", "10");
        }

        // Security and safety are visible parts of the hub, not hidden implementation.
        Box(svg, receiver, 430, 215, 140, 62, "SECURITY OFFICE", "IDENTITY // CLEARANCE", "#ffd34d");
        Box(svg, receiver, 630, 483, 170, 62, "BIO LIFE-SAFETY", "SHIELDED CONTAINERS", "#b58cff");
        Box(svg, receiver, 390, 490, 170, 62, "TRAM TERMINAL", "FREIGHT / PASSENGER", "#52e0ff");
        Box(svg, receiver, 630, 215, 170, 62, "FREIGHT CONTROL", "INBOUND / OUTBOUND", "#ff9f43");

        panel.Content(svg);
        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("margin-top", ".45rem")
            .Style("font-size", ".48rem")
            .Style("opacity", ".62")
            .Text("THE SIX TERMINALS FORM A HUB AROUND THE CABLE // THE CABLE IS INFRASTRUCTURE, NOT THE BUILDING"));
        return panel;
    }

    private static ElementBuilder TetherBlueprint(object receiver, SpaceElevatorStructure elevator)
    {
        var panel = Section(receiver, "TETHER // POWER // CLIMBER INFRASTRUCTURE", elevator.TetherMaterial);
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 1200 720")
            .Style("width", "100%")
            .Style("height", "min(60vh, 720px)")
            .Style("margin-top", ".8rem")
            .Style("background", "rgba(2,6,14,.82)")
            .Style("border", "1px solid rgba(255,255,255,.15)");

        // A broad schematic gives the tether visual weight without pretending to simulate it.
        svg.Child(WorkshopGui.Element(receiver, "line")
            .Attribute("x1", "600").Attribute("y1", "660")
            .Attribute("x2", "600").Attribute("y2", "55")
            .Attribute("stroke", "rgba(0,220,255,.8)").Attribute("stroke-width", "12"));
        svg.Child(WorkshopGui.Element(receiver, "line")
            .Attribute("x1", "590").Attribute("y1", "660")
            .Attribute("x2", "590").Attribute("y2", "55")
            .Attribute("stroke", "rgba(255,255,255,.18)").Attribute("stroke-width", "2"));
        svg.Child(WorkshopGui.Element(receiver, "line")
            .Attribute("x1", "610").Attribute("y1", "660")
            .Attribute("x2", "610").Attribute("y2", "55")
            .Attribute("stroke", "rgba(255,255,255,.18)").Attribute("stroke-width", "2"));

        var transferAltitudes = new[] { 12_000d, 24_000d, 31_000d };
        for (var i = 0; i < transferAltitudes.Length; i++)
        {
            var y = 610 - i * 185;
            svg.Child(WorkshopGui.Element(receiver, "rect")
                .Attribute("x", "455").Attribute("y", y.ToString("0"))
                .Attribute("width", "290").Attribute("height", "48")
                .Attribute("fill", "rgba(1,4,10,.92)")
                .Attribute("stroke", "#ffd34d")
                .Attribute("stroke-width", "2"));
            Label(svg, receiver, 600, y + 21, $"POWER TRANSFER // {transferAltitudes[i]:0,0} KM", "#ffd34d", "middle", "12");
        }

        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", "555").Attribute("y", "305").Attribute("width", "90").Attribute("height", "125")
            .Attribute("fill", "rgba(82,224,255,.08)")
            .Attribute("stroke", "#52e0ff")
            .Attribute("stroke-width", "4"));
        Label(svg, receiver, 600, 350, "CLIMBER", "white", "middle", "14");
        Label(svg, receiver, 600, 372, "TRI-RAIL", "#52e0ff", "middle", "11");
        Label(svg, receiver, 600, 392, "MAGLEV SLED", "#52e0ff", "middle", "11");

        Label(svg, receiver, 130, 655, "GROUND HUB", "white", "start", "15");
        Label(svg, receiver, 820, 70, "ORBITAL STATION", "white", "start", "15");
        Label(svg, receiver, 820, 92, $"{elevator.StationAltitudeKilometers:0,0} KM", "rgba(255,255,255,.6)", "start", "11");

        panel.Content(svg);
        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("margin-top", ".45rem")
            .Style("font-size", ".48rem")
            .Style("opacity", ".62")
            .Text("THE GUI IS A BLUEPRINT OF THE INFRASTRUCTURE // TRANSPORT MOTION WILL BE AN EXPERIENCE STATE, NOT A FAKE PHYSICS SIMULATION"));
        return panel;
    }

    private static ElementBuilder OrbitalStation(object receiver, SpaceElevatorStructure elevator)
    {
        var panel = Section(receiver, "ORBITAL STATION // WAYFINDING DESTINATION", "THE STATION IS THE LARGE DESTINATION // THE GROUND HUB IS ONLY THE GATE");
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 1400 720")
            .Style("width", "100%")
            .Style("height", "min(62vh, 720px)")
            .Style("margin-top", ".8rem")
            .Style("background", "rgba(2,6,14,.82)")
            .Style("border", "1px solid rgba(255,255,255,.15)");

        svg.Child(WorkshopGui.Element(receiver, "ellipse")
            .Attribute("cx", "700").Attribute("cy", "360")
            .Attribute("rx", "500").Attribute("ry", "245")
            .Attribute("fill", "rgba(0,220,255,.035)")
            .Attribute("stroke", "rgba(0,220,255,.72)").Attribute("stroke-width", "5"));
        svg.Child(WorkshopGui.Element(receiver, "ellipse")
            .Attribute("cx", "700").Attribute("cy", "360")
            .Attribute("rx", "350").Attribute("ry", "155")
            .Attribute("fill", "none")
            .Attribute("stroke", "rgba(255,255,255,.2)").Attribute("stroke-width", "2"));

        var destinations = new[]
        {
            (430d, 240d, "WORLD TERMINALS", "TRANSFER TO CREATED WORLDS", "#52e0ff"),
            (970d, 240d, "DIGITAL SHOPS", "CONCOURSE REAL ESTATE", "#ffd34d"),
            (390d, 485d, "CREATOR DISTRICT", "PUBLISH / BUILD / VISIT", "#52e05a"),
            (1010d, 485d, "SINGULARITY CASINO", "STATION ENTERTAINMENT", "#ff70d9")
        };

        foreach (var (x, y, title, detail, accent) in destinations)
        {
            Box(svg, receiver, x - 105, y - 34, 210, 68, title, detail, accent);
        }

        Label(svg, receiver, 700, 350, "ORBITAL", "white", "middle", "26");
        Label(svg, receiver, 700, 382, "CONCOURSE", "#52e0ff", "middle", "18");
        Label(svg, receiver, 700, 410, "WAYFINDING CORE", "rgba(255,255,255,.6)", "middle", "11");

        panel.Content(svg);
        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("margin-top", ".45rem")
            .Style("font-size", ".48rem")
            .Style("opacity", ".62")
            .Text("THE ORBITAL STATION IS WHERE THE WORKSHOP STOPS BEING TERRESTRIAL-SCALE // TERMINALS BECOME GATEWAYS TO OTHER EXPERIENCES"));
        return panel;
    }

    private static ElementBuilder FacilityManifest(object receiver, SpaceElevatorStructure elevator)
    {
        var panel = Section(receiver, "FACILITY MANIFEST", "STRUCTURE IS DATA // PRESENTATION IS A MANIFESTATION OF THAT DATA");
        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("display", "grid")
            .Style("grid-template-columns", "repeat(auto-fit,minmax(230px,1fr))")
            .Style("gap", ".55rem")
            .Style("margin-top", ".8rem")
            .Content(ManifestCard(receiver, "TETHER", $"{elevator.TetherLengthKilometers:0,0} KM", elevator.TetherMaterial))
            .Content(ManifestCard(receiver, "STATION", $"{elevator.StationAltitudeKilometers:0,0} KM", elevator.Station?.Label ?? "NO STATION"))
            .Content(ManifestCard(receiver, "TERMINALS", $"{elevator.Terminals.Count}", "4 FREIGHT // 2 PASSENGER"))
            .Content(ManifestCard(receiver, "CLIMBER", "TRI-RAIL", elevator.ClimberArchitecture))
            .Content(ManifestCard(receiver, "POWER", $"{elevator.MidTetherPowerTransferStations}", "MID-TETHER TRANSFER STATIONS")));
        return panel;
    }

    private static ElementBuilder Section(object receiver, string title, string subtitle)
        => WorkshopGui.Panel(receiver)
            .Style("max-width", "1500px")
            .Style("margin", "1.2rem auto 0")
            .Style("padding", ".75rem")
            .Style("border", "1px solid rgba(255,255,255,.14)")
            .Style("background", "rgba(255,255,255,.018)")
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("font-size", ".62rem")
                .Style("letter-spacing", ".18em")
                .Text(title))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".25rem")
                .Style("font-size", ".43rem")
                .Style("opacity", ".55")
                .Text(subtitle));

    private static ElementBuilder ManifestCard(object receiver, string title, string value, string detail)
        => WorkshopGui.Panel(receiver)
            .Style("padding", ".7rem")
            .Style("border", "1px solid rgba(255,255,255,.14)")
            .Style("background", "rgba(1,4,10,.65)")
            .Content(WorkshopGui.Element(receiver, "div").Style("font-size", ".4rem").Style("letter-spacing", ".16em").Style("opacity", ".55").Text(title))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".25rem").Style("font-size", ".75rem").Text(value))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".25rem").Style("font-size", ".4rem").Style("opacity", ".6").Text(detail));

    private static void Box(ElementBuilder svg, object receiver, double x, double y, double width, double height, string title, string detail, string accent)
    {
        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", x.ToString("0.##")).Attribute("y", y.ToString("0.##"))
            .Attribute("width", width.ToString("0.##")).Attribute("height", height.ToString("0.##"))
            .Attribute("fill", "rgba(1,4,10,.94)")
            .Attribute("stroke", accent).Attribute("stroke-width", "2"));
        Label(svg, receiver, x + width / 2, y + height / 2 - 3, title, accent, "middle", "11");
        Label(svg, receiver, x + width / 2, y + height / 2 + 14, detail, "rgba(255,255,255,.58)", "middle", "8");
    }

    private static void Label(ElementBuilder svg, object receiver, double x, double y, string text, string fill, string anchor, string size)
        => svg.Child(WorkshopGui.Element(receiver, "text")
            .Attribute("x", x.ToString("0.##"))
            .Attribute("y", y.ToString("0.##"))
            .Attribute("fill", fill)
            .Attribute("text-anchor", anchor)
            .Attribute("font-size", size)
            .Attribute("font-family", "Consolas, 'Courier New', monospace")
            .Text(text));
}
