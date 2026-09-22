namespace TheSingularityWorkshop.Gui;

using System;
using System.Linq;

/// <summary>
/// Security checkpoint presented as a readable plan view.
/// The normal route teaches the visitor by physical arrangement: belongings on
/// the left, a short queue in the middle, screening on the right, and collection
/// beyond the scanner. The VIP route intentionally switches to an elevation so
/// the card reader and staffed bypass are unmistakable.
/// </summary>
public static class SpatialSecurityCheckpointPlanGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Yellow = "#ffd34d";
    private const string Magenta = "#ff38d1";
    private const string Green = "#52e05a";
    private const string Red = "#ff5a70";
    private const string White = "#ffffff";
    private const string Muted = "#8ea7b2";
    private const string Ink = "#02080f";

    public static ElementBuilder Build(
        object receiver,
        SpatialSecurityCheckpointModel checkpoint,
        string userName,
        Action<Microsoft.AspNetCore.Components.Web.MouseEventArgs> worldClick,
        Action<int> moveQueue,
        Action placeTray,
        Action loadBelongings,
        Action scanner,
        Action collectTray,
        Action returnToCampus,
        Action enterFacility,
        bool vipRoute,
        Action clearVipRoute)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0")
            .Style("overflow", "hidden")
            .Style("background", Ink).Style("color", White)
            .Style("font-family", "Consolas,'Courier New',monospace");

        root.Content(vipRoute
            ? VipElevation(receiver, checkpoint, userName, clearVipRoute)
            : SecurityPlan(receiver, checkpoint, userName, worldClick, moveQueue, placeTray, loadBelongings, scanner, collectTray, enterFacility));

        root.Content(WorkshopGui.Button(receiver).Label("RETURN")
            .Style("position", "fixed").Style("right", "1.5rem").Style("bottom", "1.25rem")
            .Style("z-index", "90").Style("padding", ".8rem 1.1rem")
            .Style("background", "rgba(2,8,15,.94)")
            .Style("border", $"1px solid {Muted}88")
            .Style("color", White).Style("font-family", "inherit")
            .Style("font-size", "1rem").Style("cursor", "pointer")
            .OnClick(returnToCampus));

        root.Content(Header(receiver, vipRoute, checkpoint));

        return root;
    }

    private static ElementBuilder SecurityPlan(
        object receiver,
        SpatialSecurityCheckpointModel checkpoint,
        string userName,
        Action<Microsoft.AspNetCore.Components.Web.MouseEventArgs> worldClick,
        Action<int> moveQueue,
        Action placeTray,
        Action loadBelongings,
        Action scanner,
        Action collectTray,
        Action enterFacility)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 120 72")
            .Attribute("preserveAspectRatio", "xMidYMid meet")
            .Style("position", "absolute").Style("inset", "0")
            .Style("width", "100vw").Style("height", "100vh")
            .OnClick(worldClick);

        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("width", "120").Attribute("height", "72").Attribute("fill", Ink));

        // Left: belongings are physically deposited here first.
        Box(svg, receiver, 5, 22, 23, 28, Yellow, "BELONGINGS / TRAY");
        Label(svg, receiver, 16.5, 27, "PUT YOUR STUFF", Yellow, 1.15, true);
        Label(svg, receiver, 16.5, 30.5, "IN THE TRAY", White, .95, true);

        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", "10").Attribute("y", "34").Attribute("width", "13").Attribute("height", "7")
            .Attribute("rx", "1").Attribute("fill", "#10242d")
            .Attribute("stroke", Yellow).Attribute("stroke-width", ".5")
            .OnClick(_ =>
            {
                if (checkpoint.Stage == SpatialSecurityCheckpointStage.TrayReady)
                    placeTray();
                else
                    loadBelongings();
            }));

        if (checkpoint.BelongingsOnTray)
            svg.Child(WorkshopGui.Element(receiver, "rect")
                .Attribute("x", "13").Attribute("y", "36").Attribute("width", "7").Attribute("height", "3")
                .Attribute("fill", $"{Cyan}55").Attribute("stroke", Cyan).Attribute("stroke-width", ".35"));

        // Center: the line is people, not UI slots.
        Label(svg, receiver, 54, 17, "WAIT HERE", Red, 1.55, true);
        Label(svg, receiver, 54, 20, "SECURITY WILL WAVE YOU FORWARD", Muted, .72, false);

        const double queueX = 52;
        const double step = 5.2;

        foreach (var agent in checkpoint.Agents.OrderBy(a => a.QueueSlot))
        {
            var y = 26 + agent.QueueSlot * step;
            var speech = checkpoint.GetAgentSpeech(agent.Id);
            var isFront = agent.QueueSlot == 0;
            var agitated = checkpoint.Agitated && agent.QueueSlot > checkpoint.PlayerQueueSlot;

            var group = WorkshopGui.Element(receiver, "g")
                .Style("cursor", "pointer")
                .Style("animation", agitated ? "security-plan-agitated .28s ease-in-out infinite" : "none")
                .OnClick(_ => moveQueue(agent.QueueSlot));

            group.Child(WorkshopGui.Element(receiver, "circle")
                .Attribute("cx", queueX.ToString("0.##"))
                .Attribute("cy", (y - 1.5).ToString("0.##"))
                .Attribute("r", isFront ? "1.7" : "1.35")
                .Attribute("fill", isFront ? Yellow : Cyan));

            group.Child(WorkshopGui.Element(receiver, "path")
                .Attribute("d", $"M {queueX - 2.1} {y + 2} Q {queueX} {y - 1} {queueX + 2.1} {y + 2} L {queueX + 2.3} {y + 5} H {queueX - 2.3} Z")
                .Attribute("fill", isFront ? $"{Yellow}33" : $"{Cyan}18")
                .Attribute("stroke", isFront ? Yellow : Cyan)
                .Attribute("stroke-width", isFront ? ".5" : ".3"));

            if (!string.IsNullOrWhiteSpace(speech))
                group.Child(Speech(receiver, queueX + 4, y, speech, agitated ? Red : Yellow));

            svg.Child(group);

            if (isFront)
            {
                Label(svg, receiver, 61, y - 1, "NEXT", Yellow, .8, true);
                Label(svg, receiver, 61, y + 1.6, "WAVE FORWARD", Muted, .6, false);
            }
        }

        var playerY = 26 + checkpoint.PlayerQueueSlot * step;
        DrawPerson(receiver, svg, checkpoint.Stage == SpatialSecurityCheckpointStage.Queue ? 43 : 39, playerY, Magenta, true);
        Label(svg, receiver, 36, playerY + 1, userName.ToUpperInvariant(), Magenta, .68, true);

        // Right: person screening, then security-controlled waiting, then collection.
        Box(svg, receiver, 75, 20, 16, 30, Magenta, "SCANNER");
        Label(svg, receiver, 83, 27, "WALK", Magenta, 1.15, true);
        Label(svg, receiver, 83, 30, "THROUGH", White, 1.0, true);
        svg.Child(WorkshopGui.Element(receiver, "path")
            .Attribute("d", "M 79 44 V 28 Q 79 24 83 24 Q 87 24 87 28 V 44")
            .Attribute("fill", "none").Attribute("stroke", Magenta).Attribute("stroke-width", "1.1")
            .OnClick(_ => scanner()));

        Label(svg, receiver, 91, 34, "SECURITY", Yellow, .72, true);
        Label(svg, receiver, 91, 37, "WAIT FOR", Yellow, .72, true);
        Label(svg, receiver, 91, 40, "CLEARANCE", Yellow, .72, true);

        Box(svg, receiver, 91, 20, 22, 30, Green, "COLLECT");
        Label(svg, receiver, 102, 29, "YOUR", Green, 1.05, true);
        Label(svg, receiver, 102, 33, "TRAY", Green, 1.05, true);
        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", "96").Attribute("y", "37").Attribute("width", "12").Attribute("height", "6")
            .Attribute("fill", "#10242d").Attribute("stroke", Green).Attribute("stroke-width", ".5")
            .OnClick(_ => collectTray()));

        if (checkpoint.HasClearedCheckpoint)
        {
            svg.Child(WorkshopGui.Element(receiver, "path")
                .Attribute("d", "M 101 20 V 13 H 116 V 20")
                .Attribute("fill", $"{Green}15").Attribute("stroke", Green).Attribute("stroke-width", ".6")
                .OnClick(_ => enterFacility()));
            Label(svg, receiver, 108.5, 11, "PASSAGE OPEN", Green, .8, true);
        }

        var instruction = checkpoint.Stage switch
        {
            SpatialSecurityCheckpointStage.Approach => "ENTER THE SECURITY AREA",
            SpatialSecurityCheckpointStage.Queue => "WAIT HERE // MOVE ONLY INTO THE SPACE THAT OPENS",
            SpatialSecurityCheckpointStage.TrayReady => "TAKE THE TRAY",
            SpatialSecurityCheckpointStage.TrayLoaded => "PUT YOUR BELONGINGS ON THE TRAY",
            SpatialSecurityCheckpointStage.Scanner => "WALK THROUGH THE SCANNER",
            SpatialSecurityCheckpointStage.Collection => "WAIT FOR CLEARANCE // THEN COLLECT YOUR TRAY",
            SpatialSecurityCheckpointStage.Cleared => "SECURITY CLEAR // PROCEED",
            _ => "FOLLOW THE PHYSICAL SEQUENCE"
        };

        Label(svg, receiver, 60, 64, instruction, White, .95, true);

        if (checkpoint.ServicePulse > 0 && checkpoint.Stage == SpatialSecurityCheckpointStage.Queue)
        {
            svg.Child(WorkshopGui.Element(receiver, "circle")
                .Attribute("cx", "52").Attribute("cy", "24")
                .Attribute("r", "1.1").Attribute("fill", Cyan)
                .Style("animation", "security-service-pulse .55s ease-out"));
        }

        svg.Child(WorkshopGui.Element(receiver, "style")
            .Text("@keyframes security-plan-agitated{0%,100%{filter:brightness(1)}50%{filter:brightness(2.1)}}@keyframes security-service-pulse{0%{transform:translate(0,0);opacity:1}100%{transform:translate(30px,8px);opacity:0}}"));

        return svg;
    }

    private static ElementBuilder VipElevation(
        object receiver,
        SpatialSecurityCheckpointModel checkpoint,
        string userName,
        Action clearVipRoute)
    {
        var root = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("inset", "0")
            .Style("display", "flex").Style("align-items", "center").Style("justify-content", "center")
            .Style("background", Ink);

        var scene = WorkshopGui.Panel(receiver)
            .Style("position", "relative").Style("width", "min(1100px,88vw)")
            .Style("height", "min(650px,78vh)")
            .Style("border", $"1px solid {Magenta}88")
            .Style("background", "#07131b")
            .Style("box-shadow", $"0 0 60px {Magenta}16");

        scene.Content(WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "6%").Style("right", "6%").Style("bottom", "12%")
            .Style("height", "18%").Style("border-top", $"2px solid {Cyan}")
            .Style("background", "#0b2029")
            .Text("GROUND FLOOR // VIP BYPASS"));

        // Elevated card reader.
        scene.Content(WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "48%").Style("top", "28%")
            .Style("width", "12%").Style("height", "24%")
            .Style("border", $"2px solid {Yellow}").Style("background", "#08151d")
            .Style("cursor", "pointer").Style("display", "flex").Style("flex-direction", "column")
            .Style("align-items", "center").Style("justify-content", "center")
             .OnClick(_ => clearVipRoute())
            .Content(WorkshopGui.Element(receiver, "div").Style("color", Yellow).Style("font-size", "1.2rem").Style("font-weight", "700").Text("CARD"))
            .Content(WorkshopGui.Element(receiver, "div").Style("color", White).Style("font-size", "1rem").Text("READER"))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".5rem").Style("color", Muted).Style("font-size", ".75rem").Text("TAP VIP PASS")));

        Guard(scene, receiver, 22, 39, "GUARD 01");
        Guard(scene, receiver, 74, 39, "GUARD 02");

        scene.Content(WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "39%").Style("top", "12%")
            .Style("width", "22%").Style("text-align", "center")
            .Style("color", Magenta).Style("font-size", "1.4rem").Style("font-weight", "700")
            .Style("letter-spacing", ".14em").Text("VIP SECURITY"));

        scene.Content(WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "8%").Style("top", "10%")
            .Style("color", Cyan).Style("font-size", "1rem").Style("letter-spacing", ".12em")
            .Text($"PASS HOLDER // {userName.ToUpperInvariant()}"));

        scene.Content(WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "20%").Style("right", "20%").Style("bottom", "4%")
            .Style("text-align", "center").Style("color", White).Style("font-size", "1rem")
            .Text(checkpoint.Stage == SpatialSecurityCheckpointStage.Cleared
                ? "BYPASS OPEN // SECURITY HAS CLEARED THE PASS"
                : "TAP THE CARD READER // TWO SECURITY GUARDS STAFF THE BYPASS"));

        root.Content(scene);
        return root;
    }

    private static void Guard(ElementBuilder scene, object receiver, double x, double y, string name)
        => scene.Content(WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", $"{x}%").Style("top", $"{y}%")
            .Style("width", "12%").Style("height", "28%")
            .Style("border", $"1px solid {Cyan}77").Style("background", "#0a1d25")
            .Style("display", "flex").Style("flex-direction", "column")
            .Style("align-items", "center").Style("justify-content", "center")
            .Content(WorkshopGui.Element(receiver, "div").Style("width", "42px").Style("height", "42px").Style("border-radius", "50%").Style("background", Cyan))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".6rem").Style("color", White).Style("font-size", ".8rem").Text(name))
            .Content(WorkshopGui.Element(receiver, "div").Style("color", Muted).Style("font-size", ".65rem").Text("SECURITY")));

    private static ElementBuilder Header(object receiver, bool vipRoute, SpatialSecurityCheckpointModel checkpoint)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "fixed").Style("left", "1.2rem").Style("top", "1rem")
            .Style("z-index", "100").Style("padding", ".55rem .8rem")
            .Style("background", Ink).Style("border", $"1px solid {(vipRoute ? Magenta : Cyan)}77")
            .Style("color", vipRoute ? Magenta : Yellow)
            .Style("font-size", "clamp(1rem,1.3vw,1.3rem)").Style("font-weight", "700")
            .Style("letter-spacing", ".12em")
            .Text(vipRoute ? "SINGULARITY LAB // VIP SECURITY BYPASS" : "SINGULARITY LAB // SECURITY CHECKPOINT");

    private static void Box(ElementBuilder svg, object receiver, double x, double y, double w, double h, string stroke, string label)
    {
        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", x).Attribute("y", y).Attribute("width", w).Attribute("height", h)
            .Attribute("fill", "#071a22").Attribute("stroke", stroke).Attribute("stroke-width", ".55"));
        Label(svg, receiver, x + w / 2, y + 3.5, label, stroke, .72, true);
    }

    private static void Label(ElementBuilder svg, object receiver, double x, double y, string text, string color, double size, bool bold)
        => svg.Child(WorkshopGui.Element(receiver, "text")
            .Attribute("x", x).Attribute("y", y)
            .Attribute("fill", color).Attribute("font-size", size)
            .Attribute("font-family", "Consolas,'Courier New',monospace")
            .Attribute("font-weight", bold ? "700" : "400")
            .Attribute("text-anchor", "middle").Text(text));

    private static ElementBuilder Speech(object receiver, double x, double y, string text, string color)
        => WorkshopGui.Element(receiver, "text")
            .Attribute("x", x).Attribute("y", y)
            .Attribute("fill", color).Attribute("font-size", ".62")
            .Attribute("font-family", "Consolas,'Courier New',monospace")
            .Attribute("font-weight", "700").Text(text);

    private static void DrawPerson(object receiver, ElementBuilder svg, double x, double y, string color, bool highlight)
    {
        svg.Child(WorkshopGui.Element(receiver, "circle")
            .Attribute("cx", x).Attribute("cy", y - 1.5)
            .Attribute("r", highlight ? "1.7" : "1.3").Attribute("fill", color));
        svg.Child(WorkshopGui.Element(receiver, "path")
            .Attribute("d", $"M {x - 2} {y + 2} Q {x} {y - 1} {x + 2} {y + 2} L {x + 2.2} {y + 5} H {x - 2.2} Z")
            .Attribute("fill", $"{color}33").Attribute("stroke", color).Attribute("stroke-width", ".4"));
    }
}
