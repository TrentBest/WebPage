namespace TheSingularityWorkshop.Gui;

using System;

/// <summary>
/// Exterior elevation and physical access surface for the Singularity Laboratory.
/// The door, card reader, and intercom are manifestations of reusable security capabilities.
/// </summary>
public static class SpatialSingularityLabEntranceGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Yellow = "#ffd34d";
    private const string Magenta = "#ff38d1";
    private const string Green = "#52e05a";
    private const string White = "#ffffff";
    private const string Muted = "#8ea7b2";
    private const string Ink = "#02080f";

    public static ElementBuilder Build(
        object receiver,
        SpatialLaboratoryAccessModel access,
        SpatialLaboratoryTrafficModel traffic,
        string userName,
        Action callIntercom,
        Action badge,
        Action enterFacility,
        Action exit)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0")
            .Style("overflow", "hidden")
            .Style("background", Ink)
            .Style("color", White)
            .Style("font-family", "Consolas,'Courier New',monospace");

        root.Content(Elevation(receiver, access));
        root.Content(Traffic(receiver, traffic));
        root.Content(Controls(receiver, access, callIntercom, badge, enterFacility, exit));
        return root;
    }

    private static ElementBuilder Elevation(object receiver, SpatialLaboratoryAccessModel access)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 100")
            .Style("position", "absolute").Style("inset", "0")
            .Style("width", "100vw").Style("height", "100vh");

        svg.Child(WorkshopGui.Element(receiver, "defs")
            .Content(WorkshopGui.Element(receiver, "linearGradient")
                .Attribute("id", "labSky").Attribute("x1", "0").Attribute("y1", "0").Attribute("x2", "0").Attribute("y2", "1")
                .Content(WorkshopGui.Element(receiver, "stop").Attribute("offset", "0%").Attribute("stop-color", "#061923"))
                .Content(WorkshopGui.Element(receiver, "stop").Attribute("offset", "100%").Attribute("stop-color", "#02080f"))));

        svg.Child(WorkshopGui.Element(receiver, "rect").Attribute("x", "0").Attribute("y", "0").Attribute("width", "100").Attribute("height", "100").Attribute("fill", "url(#labSky)"));
        svg.Child(WorkshopGui.Element(receiver, "polygon").Attribute("points", "15,88 85,88 76,31 24,31").Attribute("fill", "#07131b").Attribute("stroke", Cyan).Attribute("stroke-width", ".45"));
        svg.Child(WorkshopGui.Element(receiver, "polygon").Attribute("points", "24,31 76,31 70,17 30,17").Attribute("fill", "#0b1d27").Attribute("stroke", Cyan).Attribute("stroke-width", ".45"));

        svg.Child(WorkshopGui.Element(receiver, "text").Attribute("x", "50").Attribute("y", "25")
            .Attribute("fill", Yellow).Attribute("font-size", "5.1").Attribute("font-weight", "700")
            .Attribute("text-anchor", "middle").Text("SINGULARITY LABORATORY"));

        svg.Child(WorkshopGui.Element(receiver, "text").Attribute("x", "50").Attribute("y", "29")
            .Attribute("fill", Muted).Attribute("font-size", "1.1").Attribute("letter-spacing", ".35em")
            .Attribute("text-anchor", "middle").Text("RESEARCH // PHYSICS // COMPUTATION"));

        // Main door.
        svg.Child(WorkshopGui.Element(receiver, "rect").Attribute("x", "44").Attribute("y", "58").Attribute("width", "12").Attribute("height", "25")
            .Attribute("fill", access.DoorOpen ? $"{Green}22" : $"{Cyan}08")
            .Attribute("stroke", access.DoorOpen ? Green : Cyan).Attribute("stroke-width", ".55"));

        svg.Child(WorkshopGui.Element(receiver, "text").Attribute("x", "50").Attribute("y", "69")
            .Attribute("fill", access.DoorOpen ? Green : White).Attribute("font-size", "1.25")
            .Attribute("text-anchor", "middle").Text(access.DoorOpen ? "OPEN" : "SECURE"));

        // Card reader to the right of the door.
        svg.Child(WorkshopGui.Element(receiver, "rect").Attribute("x", "58").Attribute("y", "65").Attribute("width", "8").Attribute("height", "6")
            .Attribute("fill", $"{Yellow}0a").Attribute("stroke", Yellow).Attribute("stroke-width", ".3"));
        svg.Child(WorkshopGui.Element(receiver, "text").Attribute("x", "62").Attribute("y", "68")
            .Attribute("fill", Yellow).Attribute("font-size", ".7").Attribute("text-anchor", "middle").Text("CARD"));
        svg.Child(WorkshopGui.Element(receiver, "text").Attribute("x", "62").Attribute("y", "69.8")
            .Attribute("fill", Yellow).Attribute("font-size", ".7").Attribute("text-anchor", "middle").Text("READER"));

        // Voice call box to the left.
        svg.Child(WorkshopGui.Element(receiver, "rect").Attribute("x", "34").Attribute("y", "65").Attribute("width", "8").Attribute("height", "7")
            .Attribute("fill", $"{Magenta}0a").Attribute("stroke", Magenta).Attribute("stroke-width", ".3"));
        svg.Child(WorkshopGui.Element(receiver, "circle").Attribute("cx", "38").Attribute("cy", "68").Attribute("r", "1.2").Attribute("fill", Magenta));
        svg.Child(WorkshopGui.Element(receiver, "text").Attribute("x", "38").Attribute("y", "71")
            .Attribute("fill", Magenta).Attribute("font-size", ".65").Attribute("text-anchor", "middle").Text("INTERCOM"));

        svg.Child(WorkshopGui.Element(receiver, "text").Attribute("x", "50").Attribute("y", "94")
            .Attribute("fill", Cyan).Attribute("font-size", "1.1").Attribute("text-anchor", "middle")
            .Text($"VISITOR // {access.Badge.Id} // {access.Badge.Clearance.ToString().ToUpperInvariant()}"));

        return svg;
    }

    private static ElementBuilder Traffic(object receiver, SpatialLaboratoryTrafficModel traffic)
    {
        var now = TimeOnly.FromDateTime(DateTime.Now);
        var panel = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("right", "1.5rem").Style("top", "1.5rem")
            .Style("width", "min(23rem,30vw)").Style("padding", ".7rem")
            .Style("background", "rgba(1,6,12,.88)").Style("border", $"1px solid {Cyan}44")
            .Style("z-index", "30");

        panel.Content(WorkshopGui.Element(receiver, "div").Style("font-size", ".4rem").Style("letter-spacing", ".16em").Style("color", Cyan)
            .Text("LIVE FACILITY TRAFFIC"));
        panel.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".3rem").Style("font-size", ".35rem").Style("color", Muted)
            .Text($"SHIFT CLOCK // {now:HH:mm}"));

        foreach (var employee in traffic.OnDutyEmployees(now))
        {
            panel.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".3rem").Style("font-size", ".37rem").Style("color", White)
                .Text($"● {employee.Name} // {employee.Destination} // {employee.Clearance}"));
        }

        panel.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".55rem").Style("font-size", ".4rem").Style("color", Yellow)
            .Text("SECURITY POSTS"));
        foreach (var guard in traffic.OnDutyGuards(now))
        {
            panel.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".25rem").Style("font-size", ".35rem").Style("color", Muted)
                .Text($"● {guard.Name} // {guard.Post} // {guard.Start:HH:mm}-{guard.End:HH:mm}"));
        }

        return panel;
    }

    private static ElementBuilder Controls(
        object receiver,
        SpatialLaboratoryAccessModel access,
        Action callIntercom,
        Action badge,
        Action enterFacility,
        Action exit)
    {
        var panel = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("left", "1.5rem").Style("bottom", "1.5rem")
            .Style("width", "min(30rem,40vw)").Style("padding", ".8rem")
            .Style("background", "rgba(1,6,12,.94)").Style("border", $"1px solid {Yellow}55")
            .Style("z-index", "40");

        panel.Content(WorkshopGui.Element(receiver, "div").Style("font-size", ".4rem").Style("letter-spacing", ".14em").Style("color", Yellow)
            .Text("PHYSICAL ACCESS // SECURITY BOUNDARY"));
        panel.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".4rem").Style("font-family", "system-ui,sans-serif").Style("font-size", ".75rem").Style("color", White)
            .Text(access.LastMessage));

        panel.Content(ActionButton(receiver, "USE VOICE CALL BOX", callIntercom, Magenta));
        panel.Content(ActionButton(receiver, "PRESENT SECURITY BADGE", badge, Cyan));

        if (access.DoorOpen)
            panel.Content(ActionButton(receiver, "ENTER RESEARCH FACILITY →", enterFacility, Green));

        panel.Content(WorkshopGui.Button(receiver).Label("← RETURN TO CAMPUS")
            .Style("margin-top", ".45rem").Style("padding", ".4rem .65rem")
            .Style("border", $"1px solid {Muted}66").Style("background", "transparent")
            .Style("color", Muted).Style("font-family", "inherit").Style("font-size", ".35rem")
            .Style("cursor", "pointer").OnClick(exit));

        return panel;
    }

    private static ElementBuilder ActionButton(object receiver, string label, Action action, string accent)
        => WorkshopGui.Button(receiver).Label(label)
            .Style("display", "block").Style("width", "100%").Style("margin-top", ".35rem")
            .Style("padding", ".5rem").Style("border", $"1px solid {accent}88")
            .Style("background", $"{accent}08").Style("color", accent)
            .Style("font-family", "inherit").Style("font-size", ".37rem")
            .Style("cursor", "pointer").OnClick(action);
}
