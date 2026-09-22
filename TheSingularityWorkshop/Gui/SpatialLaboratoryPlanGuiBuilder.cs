namespace TheSingularityWorkshop.Gui;

using System;

/// <summary>
/// Presents the laboratory interior as a walkable plan after security clearance.
/// The plan is the spatial truth: the visitor stands beyond the security line and
/// can walk to reception, the secure elevator, or the director's office when the
/// receptionist has granted access.
/// </summary>
public static class SpatialLaboratoryPlanGuiBuilder
{
    /// <summary>The laboratory is intentionally perceived through a closer building-local camera.</summary>
    public const double BuildingZoom = 1.35;

    private const string Cyan = "#00eaff";
    private const string Yellow = "#ffd34d";
    private const string Magenta = "#ff38d1";
    private const string Green = "#52e05a";
    private const string White = "#ffffff";
    private const string Muted = "#8ea7b2";
    private const string Ink = "#02080f";

    public static ElementBuilder Build(
        object receiver,
        string userName,
        double avatarX,
        double avatarY,
        bool directorAccessGranted,
        Action<Microsoft.AspNetCore.Components.Web.MouseEventArgs> worldClick,
        Action<Microsoft.AspNetCore.Components.Web.KeyboardEventArgs> keyDown,
        Action reception,
        Action elevator,
        Action director,
        Action research,
        Action exit)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0")
            .Style("overflow", "hidden")
            .Style("background", Ink)
            .Style("color", White)
            .Style("font-family", "Consolas,'Courier New',monospace")
            .Attribute("tabindex", "0")
            .OnKeyDown(keyDown);

        root.Content(Plan(receiver, userName, avatarX, avatarY, directorAccessGranted, worldClick, reception, elevator, director, research));
        root.Content(Header(receiver, directorAccessGranted));
        root.Content(WorkshopGui.Button(receiver).Label("← EXIT LAB")
            .Style("position", "fixed").Style("right", "1.5rem").Style("bottom", "1.25rem")
            .Style("z-index", "90").Style("padding", ".75rem 1rem")
            .Style("background", "rgba(2,8,15,.92)").Style("border", $"1px solid {Magenta}66")
            .Style("color", Magenta).Style("font-family", "inherit").Style("font-size", "1rem")
            .Style("letter-spacing", ".1em").Style("cursor", "pointer").OnClick(exit));

        return root;
    }

    private static ElementBuilder Plan(
        object receiver,
        string userName,
        double avatarX,
        double avatarY,
        bool directorAccessGranted,
        Action<Microsoft.AspNetCore.Components.Web.MouseEventArgs> worldClick,
        Action reception,
        Action elevator,
        Action director,
        Action research)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", ViewBox(avatarX, avatarY))
            .Attribute("preserveAspectRatio", "xMidYMid meet")
            .Style("position", "absolute").Style("inset", "0")
            .Style("width", "100vw").Style("height", "100vh")
            .OnClick(worldClick);

        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("width", "100").Attribute("height", "100")
            .Attribute("fill", Ink));

        // The security line is behind the visitor. This is the physical transition.
        Room(svg, receiver, 8, 12, 84, 76, "LABORATORY // CLEARED INTERIOR", Cyan, .45);
        Wall(svg, receiver, 10, 28, 80, 28, Cyan);
        Label(svg, receiver, 50, 25, "SECURITY LINE // BEHIND YOU", Muted, 1.15, true);

        // Public arrival space.
        Zone(svg, receiver, 14, 33, 24, 20, "RECEPTION", Cyan);
        Label(svg, receiver, 26, 44, "RECEPTION", White, 1.35, true);
        Label(svg, receiver, 26, 48, "ASK FOR A VIP PASS", Muted, .75, false);

        // Director is deliberately behind reception authority.
        Zone(svg, receiver, 42, 33, 24, 20, "LAB ADMINISTRATOR", Yellow);
        Label(svg, receiver, 54, 44, "LAB ADMIN", directorAccessGranted ? Green : Yellow, 1.25, true);
        Label(svg, receiver, 54, 48, directorAccessGranted ? "ACCESS GRANTED" : "ADMINISTRATIVE AUTHORITY", Muted, .72, false);

        // Secure vertical circulation.
        Zone(svg, receiver, 70, 33, 16, 35, "ELEVATOR", Magenta);
        Label(svg, receiver, 78, 47, "ELEVATOR", White, 1.0, true);
        Label(svg, receiver, 78, 51, "VERTICAL CORE", Muted, .68, false);

        // Research floor.
        Zone(svg, receiver, 14, 57, 52, 22, "RESEARCH", Cyan);
        Label(svg, receiver, 40, 68, "RESEARCH FLOOR", White, 1.1, true);
        Label(svg, receiver, 40, 72, "AUTHORIZED LABORATORY SPACE", Muted, .7, false);

        // The rendering research level is deliberately hidden until the visitor has
        // reached the laboratory's director authority. It is discovered as a place,
        // not advertised as a campus tab.
        if (directorAccessGranted)
        {
            Zone(svg, receiver, 68, 57, 18, 22, "RENDERING RESEARCH", Magenta);
            Label(svg, receiver, 77, 67, "RENDERING", White, 1.0, true);
            Label(svg, receiver, 77, 71, "RESEARCH LEVEL", Muted, .68, false);
            Hotspot(svg, receiver, 68, 57, 18, 22, Magenta, "RESEARCH", research);
        }

        // Physical corridor from the cleared threshold.
        svg.Child(WorkshopGui.Element(receiver, "path")
            .Attribute("d", "M 50 28 V 57")
            .Attribute("stroke", Yellow).Attribute("stroke-width", "1")
            .Attribute("stroke-dasharray", "2 2").Attribute("opacity", ".7"));
        Label(svg, receiver, 50, 60, "WALK", Yellow, .8, true);

        // Transparent spatial hotspots. The plan remains the environment; these
        // only make the represented doors/desk discoverable.
        Hotspot(svg, receiver, 14, 33, 24, 20, Cyan, "RECEPTION", reception);
        Hotspot(svg, receiver, 70, 33, 16, 35, Magenta, "ELEVATOR", elevator);
        Hotspot(svg, receiver, 42, 33, 24, 20, directorAccessGranted ? Green : Yellow, "LAB ADMINISTRATOR", director);

        Avatar(svg, receiver, avatarX, avatarY, userName);

        svg.Child(WorkshopGui.Element(receiver, "text")
            .Attribute("x", "50").Attribute("y", "94")
            .Attribute("fill", Muted).Attribute("font-size", "1")
            .Attribute("text-anchor", "middle")
            .Text("THE MAP IS THE SPATIAL TRUTH // CLICK THE PLAN TO WALK"));

        return svg;
    }

    private static string ViewBox(double avatarX, double avatarY)
    {
        var width = 100d / BuildingZoom;
        var height = 100d / BuildingZoom;
        return $"{avatarX - width / 2d:0.###} {avatarY - height / 2d:0.###} {width:0.###} {height:0.###}";
    }

    private static ElementBuilder Header(object receiver, bool directorAccessGranted)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "fixed").Style("left", "1.25rem").Style("top", "1.1rem")
            .Style("z-index", "80").Style("padding", ".5rem .75rem")
            .Style("background", Ink).Style("border", $"1px solid {Cyan}66")
            .Style("color", Yellow).Style("font-size", "clamp(1rem,1.4vw,1.4rem)")
            .Style("font-weight", "700").Style("letter-spacing", ".12em")
            .Text(directorAccessGranted
                ? "SINGULARITY LABORATORY // DIRECTOR ACCESS"
                : "SINGULARITY LABORATORY // CLEARED INTERIOR");

    private static void Room(ElementBuilder svg, object receiver, double x, double y, double w, double h, string label, string stroke, double width)
    {
        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", x).Attribute("y", y).Attribute("width", w).Attribute("height", h)
            .Attribute("fill", "#071820").Attribute("stroke", stroke).Attribute("stroke-width", width));
    }

    private static void Wall(ElementBuilder svg, object receiver, double x1, double y1, double x2, double y2, string color)
        => svg.Child(WorkshopGui.Element(receiver, "line")
            .Attribute("x1", x1).Attribute("y1", y1).Attribute("x2", x2).Attribute("y2", y2)
            .Attribute("stroke", color).Attribute("stroke-width", ".45"));

    private static void Zone(ElementBuilder svg, object receiver, double x, double y, double w, double h, string label, string color)
        => svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", x).Attribute("y", y).Attribute("width", w).Attribute("height", h)
            .Attribute("fill", "#0b1b24").Attribute("stroke", color).Attribute("stroke-opacity", ".65")
            .Attribute("stroke-width", ".35"));

    private static void Label(ElementBuilder svg, object receiver, double x, double y, string text, string color, double size, bool bold)
        => svg.Child(WorkshopGui.Element(receiver, "text")
            .Attribute("x", x).Attribute("y", y)
            .Attribute("fill", color).Attribute("font-size", size)
            .Attribute("font-family", "Consolas,'Courier New',monospace")
            .Attribute("font-weight", bold ? "700" : "400")
            .Attribute("text-anchor", "middle")
            .Text(text));

    private static void Hotspot(ElementBuilder svg, object receiver, double x, double y, double w, double h, string color, string label, Action action)
    {
        // SVG button-like regions are represented as foreignObject so the existing
        // WorkshopGui interaction substrate remains responsible for the click.
        var button = WorkshopGui.Button(receiver).Label(label)
            .Style("width", "100%").Style("height", "100%")
            .Style("background", $"{color}08").Style("border", $"1px solid {color}22")
            .Style("color", $"{color}66").Style("font-family", "inherit")
            .Style("font-size", "1rem").Style("letter-spacing", ".08em")
            .Style("cursor", "pointer").OnClick(action);

        svg.Child(WorkshopGui.Element(receiver, "foreignObject")
            .Attribute("x", x).Attribute("y", y).Attribute("width", w).Attribute("height", h)
            .Child(button));
    }

    private static void Avatar(ElementBuilder svg, object receiver, double x, double y, string userName)
    {
        svg.Child(WorkshopGui.Element(receiver, "circle")
            .Attribute("cx", x).Attribute("cy", y).Attribute("r", "1.8")
            .Attribute("fill", Cyan).Attribute("stroke", White).Attribute("stroke-width", ".35"));

        svg.Child(WorkshopGui.Element(receiver, "text")
            .Attribute("x", x).Attribute("y", y - 3)
            .Attribute("fill", Cyan).Attribute("font-size", "1")
            .Attribute("text-anchor", "middle").Text(userName.ToUpperInvariant()));
    }
}
