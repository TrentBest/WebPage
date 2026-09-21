namespace TheSingularityWorkshop.Gui;

using System;
using System.Linq;
using Microsoft.AspNetCore.Components.Web;

/// <summary>
/// Plan-view security checkpoint. Digitens physically define the queue;
/// the visitor learns the line by watching and responding to those people.
/// </summary>
public static class SpatialSecurityCheckpointGuiBuilder
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
        SpatialLaboratoryTrafficModel traffic,
        string userName,
        Action<MouseEventArgs> worldClick,
        Action<int> moveQueue,
        Action placeTray,
        Action loadBelongings,
        Action scanner,
        Action collectTray,
        Action returnToCampus,
        Action enterFacility)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0").Style("overflow", "hidden")
            .Style("background", Ink).Style("color", White)
            .Style("font-family", "Consolas,'Courier New',monospace");

        root.Content(Environment(receiver, checkpoint, userName, worldClick, moveQueue, placeTray, loadBelongings, scanner, collectTray, enterFacility));
        root.Content(Frame(receiver, checkpoint, traffic));
        root.Content(WorkshopGui.Button(receiver).Label("RETURN")
            .Style("position", "fixed").Style("right", "2rem").Style("bottom", "2rem")
            .Style("z-index", "90").Style("padding", ".85rem 1.25rem")
            .Style("background", "rgba(2,8,15,.9)").Style("border", $"1px solid {Muted}88")
            .Style("color", White).Style("font-family", "inherit").Style("font-size", "1rem")
            .Style("letter-spacing", ".12em").Style("cursor", "pointer").OnClick(returnToCampus));

        return root;
    }

    private static ElementBuilder Environment(
        object receiver,
        SpatialSecurityCheckpointModel checkpoint,
        string userName,
        Action<MouseEventArgs> worldClick,
        Action<int> moveQueue,
        Action placeTray,
        Action loadBelongings,
        Action scanner,
        Action collectTray,
        Action enterFacility)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", ViewBox(checkpoint))
            .Attribute("preserveAspectRatio", "xMidYMid meet")
            .Style("position", "absolute").Style("inset", "0")
            .Style("width", "100vw").Style("height", "100vh")
            .Style("cursor", "pointer")
            .OnClick(worldClick);

        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("width", "120").Attribute("height", "72").Attribute("fill", Ink));

        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", "3").Attribute("y", "4").Attribute("width", "114").Attribute("height", "64")
            .Attribute("fill", "#06131b").Attribute("stroke", "#23404b").Attribute("stroke-width", ".35"));

        // Plan-view floor. Geometry, people and equipment are the interface.
        svg.Child(WorkshopGui.Element(receiver, "path")
            .Attribute("d", "M 5 25 H 115 V 65 H 5 Z")
            .Attribute("fill", "#071820").Attribute("stroke", "#31515f").Attribute("stroke-width", ".25"));

        for (var x = 7; x <= 113; x += 7)
            svg.Child(WorkshopGui.Element(receiver, "path")
                .Attribute("d", $"M {x} 25 L {x + 5} 65")
                .Attribute("stroke", "#17313b").Attribute("stroke-width", ".16"));

        for (var y = 31; y <= 61; y += 6)
            svg.Child(WorkshopGui.Element(receiver, "path")
                .Attribute("d", $"M 5 {y} H 115")
                .Attribute("stroke", "#17313b").Attribute("stroke-width", ".16"));

        // Threshold and approach.
        svg.Child(WorkshopGui.Element(receiver, "path")
            .Attribute("d", "M 6 25 V 12 H 43 V 25")
            .Attribute("fill", "#0b2029").Attribute("stroke", Cyan).Attribute("stroke-width", ".45"));
        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", "9").Attribute("y", "15").Attribute("width", "31").Attribute("height", "7")
            .Attribute("fill", Ink).Attribute("stroke", "#31515f").Attribute("stroke-width", ".3"));
        svg.Child(Label(receiver, 11, 19.8, "SECURITY THRESHOLD", Yellow, 1.25));
        svg.Child(Label(receiver, 11, 23.5, "CONTROLLED ACCESS", Muted, .7));
        svg.Child(Label(receiver, 17, 34, "WALK THE PLAN", Cyan, .95));

        // Digitens are the queue. There are no GUI queue slots or queue cards.
        const double queueX = 42;
        const double queueStep = 4.6;

        foreach (var agent in checkpoint.Agents.OrderBy(agent => agent.QueueSlot))
        {
            var y = 29 + agent.QueueSlot * queueStep;
            var highlighted = agent.QueueSlot == checkpoint.PlayerQueueSlot;
            var speech = checkpoint.GetAgentSpeech(agent.Id);
            var agitated = checkpoint.Agitated && !string.IsNullOrWhiteSpace(speech);

            var person = WorkshopGui.Element(receiver, "g")
                .Style("cursor", "pointer")
                .Style("transform-origin", $"{queueX}px {y}px")
                .Style("animation", agitated ? "security-digitens-agitated .28s ease-in-out infinite" : "none")
                .OnClick(_ => moveQueue(agent.QueueSlot));

            person.Child(WorkshopGui.Element(receiver, "circle")
                .Attribute("cx", queueX.ToString("0.##"))
                .Attribute("cy", (y - 2).ToString("0.##"))
                .Attribute("r", highlighted ? "1.7" : "1.35")
                .Attribute("fill", highlighted ? Magenta : Cyan));

            person.Child(WorkshopGui.Element(receiver, "path")
                .Attribute("d", $"M {queueX - 2} {y + 2} Q {queueX} {y - 1} {queueX + 2} {y + 2} L {queueX + 2.3} {y + 5} H {queueX - 2.3} Z")
                .Attribute("fill", highlighted ? $"{Magenta}44" : $"{Cyan}1c")
                .Attribute("stroke", highlighted ? Magenta : Cyan)
                .Attribute("stroke-width", highlighted ? ".5" : ".3"));

            if (!string.IsNullOrWhiteSpace(speech))
                person.Child(Speech(receiver, queueX + 4.5, y - 1, speech,
                    agitated ? Red : agent.QueueSlot < checkpoint.PlayerQueueSlot ? Yellow : Red));

            svg.Child(person);

            if (agent.QueueSlot == 0)
                svg.Child(Label(receiver, 48, y - 1, ServiceText(checkpoint.GetAgentStage(agent.Id)), Yellow, .72));
        }

        // The user's position changes from queue -> tray -> scanner -> collection.
        // The camera follows that physical position instead of fitting the whole
        // building onto the screen.
        var (userX, userY) = PlayerPosition(checkpoint);
        DrawPerson(receiver, svg, userX, userY, Magenta, true);
        svg.Child(Label(receiver, userX + 5, userY + 1, userName.ToUpperInvariant(), Magenta, .72));

        // Empty spaces are physical opportunities to move forward.
        for (var slot = 0; slot < SpatialSecurityCheckpointModel.QueueCapacity; slot++)
        {
            if (slot == checkpoint.PlayerQueueSlot || checkpoint.Agents.Any(agent => agent.QueueSlot == slot))
                continue;

            var y = 29 + slot * queueStep;
            svg.Child(WorkshopGui.Element(receiver, "rect")
                .Attribute("x", (queueX - 3.2).ToString("0.##"))
                .Attribute("y", (y - 2.4).ToString("0.##"))
                .Attribute("width", "6.4").Attribute("height", "5")
                .Attribute("fill", "transparent").Attribute("stroke", "none")
                .OnClick(_ => moveQueue(slot)));
        }

        // Physical conveyor / tray.
        svg.Child(WorkshopGui.Element(receiver, "path")
            .Attribute("d", "M 45 39 H 78 L 82 57 H 49 Z")
            .Attribute("fill", "#0d222c").Attribute("stroke", Yellow).Attribute("stroke-width", ".45"));

        for (var x = 51; x <= 76; x += 5)
            svg.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", x.ToString()).Attribute("y1", "40")
                .Attribute("x2", (x + 3).ToString()).Attribute("y2", "55")
                .Attribute("stroke", "#45616b").Attribute("stroke-width", ".65"));

        var trayX = checkpoint.TrayOnRollers ? "54" : "48";
        var trayY = checkpoint.TrayOnRollers ? "43" : "45";
        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", trayX).Attribute("y", trayY).Attribute("width", "12").Attribute("height", "5")
            .Attribute("rx", "1")
            .Attribute("fill", checkpoint.TrayOnRollers ? $"{Yellow}32" : "#10232c")
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
                .Attribute("x", "57").Attribute("y", "44").Attribute("width", "6").Attribute("height", "3")
                .Attribute("fill", $"{Cyan}55").Attribute("stroke", Cyan).Attribute("stroke-width", ".3"));

        svg.Child(Label(receiver, 48, 38, "DIVEST", Yellow, 1.05));

        // X-ray machine.
        svg.Child(WorkshopGui.Element(receiver, "path")
            .Attribute("d", "M 65 34 H 80 V 55 H 65 Z")
            .Attribute("fill", "#091a23").Attribute("stroke", Cyan).Attribute("stroke-width", ".7"));

        svg.Child(WorkshopGui.Element(receiver, "path")
            .Attribute("d", "M 68 37 H 77 V 52 H 68 Z")
            .Attribute("fill", Ink).Attribute("stroke", "#39717e").Attribute("stroke-width", ".35")
            .OnClick(_ => loadBelongings()));

        svg.Child(Label(receiver, 67, 32, "SCREENING CORE", Cyan, .85));

        // Person scanner.
        svg.Child(WorkshopGui.Element(receiver, "path")
            .Attribute("d", "M 84 57 V 38 Q 84 34 88 34 H 96 Q 100 34 100 38 V 57")
            .Attribute("fill", "none").Attribute("stroke", Magenta).Attribute("stroke-width", "1.2")
            .OnClick(_ => scanner()));

        svg.Child(Label(receiver, 84, 31.8, "WALK THROUGH", Magenta, .9));

        // Collection area.
        svg.Child(WorkshopGui.Element(receiver, "path")
            .Attribute("d", "M 101 40 H 112 V 54 H 101 Z")
            .Attribute("fill", "#0a1d25").Attribute("stroke", Green).Attribute("stroke-width", ".55")
            .OnClick(_ => collectTray()));

        svg.Child(WorkshopGui.Element(receiver, "path")
            .Attribute("d", "M 103 43 H 110 M 103 47 H 110 M 103 51 H 110")
            .Attribute("stroke", "#4c7760").Attribute("stroke-width", ".5"));

        svg.Child(Label(receiver, 101, 37.5, "COLLECTION", Green, .9));

        if (checkpoint.HasClearedCheckpoint)
        {
            svg.Child(WorkshopGui.Element(receiver, "path")
                .Attribute("d", "M 106 25 V 17 H 115 V 25")
                .Attribute("fill", $"{Green}18").Attribute("stroke", Green).Attribute("stroke-width", ".65")
                .OnClick(_ => enterFacility()));
            svg.Child(Label(receiver, 106.5, 15, "CLEARED", Green, .9));
            svg.Child(Label(receiver, 105, 28.5, "FACILITY ACCESS", Green, .65));
        }

        var instruction = checkpoint.Stage switch
        {
            SpatialSecurityCheckpointStage.Approach => "ENTER THE THRESHOLD",
            SpatialSecurityCheckpointStage.Queue => "MOVE WITH THE LINE",
            SpatialSecurityCheckpointStage.TrayReady => "TAKE THE TRAY",
            SpatialSecurityCheckpointStage.TrayLoaded => "LOAD YOUR BELONGINGS",
            SpatialSecurityCheckpointStage.Scanner => "STEP THROUGH",
            SpatialSecurityCheckpointStage.Collection => "COLLECT YOUR BELONGINGS",
            SpatialSecurityCheckpointStage.Cleared => "THE THRESHOLD IS OPEN",
            _ => "WALK THE PLAN"
        };

        svg.Child(Label(receiver, 47, 63, instruction, White, 1.05));

        // Every cleared Digitens physically traverses the checkpoint. The model
        // records a pulse for each person; the keyed SVG node makes each pulse a
        // fresh short animation rather than a dashboard counter.
        if (checkpoint.ServicePulse > 0 && checkpoint.Stage == SpatialSecurityCheckpointStage.Queue)
        {
            var service = WorkshopGui.Element(receiver, "g")
                .Attribute("id", $"security-service-{checkpoint.ServicePulse}")
                .Style("pointer-events", "none");
            service.Child(WorkshopGui.Element(receiver, "circle")
                .Attribute("cx", "42").Attribute("cy", "25")
                .Attribute("r", "1.15").Attribute("fill", Cyan));
            service.Child(WorkshopGui.Element(receiver, "path")
                .Attribute("d", "M 40.4 30 Q 42 27 43.6 30 L 44 34 H 40 Z")
                .Attribute("fill", $"{Cyan}33").Attribute("stroke", Cyan).Attribute("stroke-width", ".3"));
            service.Child(WorkshopGui.Element(receiver, "animateTransform")
                .Attribute("attributeName", "transform")
                .Attribute("type", "translate")
                .Attribute("values", "0 0; 12 16; 28 16; 49 20; 64 22")
                .Attribute("dur", ".82s")
                .Attribute("repeatCount", "1"));
            svg.Child(service);
        }

        return svg;
    }

    private static string ViewBox(SpatialSecurityCheckpointModel checkpoint)
    {
        const double zoom = 1.55;
        const double width = 120d / zoom;
        const double height = 72d / zoom;
        var (centerX, centerY) = CameraCenter(checkpoint);

        return $"{centerX - width / 2d:0.###} {centerY - height / 2d:0.###} {width:0.###} {height:0.###}";
    }

    private static (double X, double Y) CameraCenter(SpatialSecurityCheckpointModel checkpoint)
        => checkpoint.Stage switch
        {
            SpatialSecurityCheckpointStage.Approach => (30, 24),
            SpatialSecurityCheckpointStage.Queue => (48, 45),
            SpatialSecurityCheckpointStage.TrayReady or SpatialSecurityCheckpointStage.TrayLoaded => (58, 45),
            SpatialSecurityCheckpointStage.Scanner => (91, 45),
            SpatialSecurityCheckpointStage.Collection => (106, 47),
            SpatialSecurityCheckpointStage.Cleared => (106, 22),
            _ => (48, 45)
        };

    private static (double X, double Y) PlayerPosition(SpatialSecurityCheckpointModel checkpoint)
        => checkpoint.Stage switch
        {
            SpatialSecurityCheckpointStage.Approach => (28, 25),
            SpatialSecurityCheckpointStage.Queue => (42, 29 + checkpoint.PlayerQueueSlot * 4.6),
            SpatialSecurityCheckpointStage.TrayReady or SpatialSecurityCheckpointStage.TrayLoaded => (54, 46),
            SpatialSecurityCheckpointStage.Scanner => (91, 45),
            SpatialSecurityCheckpointStage.Collection => (106, 47),
            SpatialSecurityCheckpointStage.Cleared => (106, 21),
            _ => (42, 29 + checkpoint.PlayerQueueSlot * 4.6)
        };

    private static ElementBuilder Frame(
        object receiver,
        SpatialSecurityCheckpointModel checkpoint,
        SpatialLaboratoryTrafficModel traffic)
    {
        var now = TimeOnly.FromDateTime(DateTime.Now);
        var accent = checkpoint.Agitated ? Red : checkpoint.HasClearedCheckpoint ? Green : Cyan;

        var frame = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "1.1rem")
            .Style("pointer-events", "none")
            .Style("border", $"1px solid {Cyan}66")
            .Style("box-sizing", "border-box").Style("z-index", "80");

        frame.Content(WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "1.4rem").Style("top", "1.25rem")
            .Style("padding", ".35rem .75rem").Style("background", Ink)
            .Style("font-size", "clamp(1rem,1.35vw,1.35rem)").Style("font-weight", "700")
            .Style("letter-spacing", ".13em").Style("color", Yellow)
            .Text("SINGULARITY LABORATORY // SECURITY"));

        frame.Content(WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("right", "1.4rem").Style("top", "1.3rem")
            .Style("padding", ".35rem .65rem").Style("background", Ink)
            .Style("font-size", "clamp(.8rem,1vw,1rem)").Style("letter-spacing", ".1em")
            .Style("color", accent)
            .Text(checkpoint.HasClearedCheckpoint ? "THRESHOLD OPEN" : "ACTIVE"));

        frame.Content(WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "1.4rem").Style("bottom", "1.25rem")
            .Style("padding", ".3rem .55rem").Style("background", Ink)
            .Style("font-size", "clamp(.72rem,.9vw,.9rem)").Style("letter-spacing", ".08em")
            .Style("color", Muted)
            .Text(checkpoint.HasClearedCheckpoint
                ? "THE MAP IS THE SPATIAL TRUTH"
                : $"GUARDS // {traffic.OnDutyGuards(now).Count} ON DUTY"));

        if (!checkpoint.HasClearedCheckpoint)
            frame.Content(WorkshopGui.Element(receiver, "div")
                .Style("position", "absolute").Style("left", "50%")
                .Style("transform", "translateX(-50%)").Style("bottom", "1.25rem")
                .Style("max-width", "45vw").Style("padding", ".3rem .65rem")
                .Style("background", Ink).Style("font-family", "system-ui,sans-serif")
                .Style("font-size", "clamp(.8rem,1vw,1rem)")
                .Style("text-align", "center").Style("color", White)
                .Text(checkpoint.LastMessage));

        frame.Content(WorkshopGui.Element(receiver, "style")
            .Text("@keyframes security-digitens-agitated{0%,100%{filter:brightness(1)}50%{filter:brightness(2.2)}}"));

        return frame;
    }

    private static ElementBuilder Speech(object receiver, double x, double y, string text, string color)
        => WorkshopGui.Element(receiver, "g")
            .Child(WorkshopGui.Element(receiver, "rect")
                .Attribute("x", x.ToString("0.##")).Attribute("y", (y - 2.2).ToString("0.##"))
                .Attribute("width", Math.Max(12, text.Length * .42).ToString("0.##"))
                .Attribute("height", "3.5").Attribute("rx", "1")
                .Attribute("fill", Ink).Attribute("stroke", color).Attribute("stroke-width", ".3"))
            .Child(Label(receiver, x + .8, y, text, color, .62));

    private static string ServiceText(SpatialSecurityAgentStage stage)
        => stage switch
        {
            SpatialSecurityAgentStage.Tray => "TRAY",
            SpatialSecurityAgentStage.XRay => "X-RAY",
            SpatialSecurityAgentStage.Scanner => "SCAN",
            SpatialSecurityAgentStage.Cleared => "CLEAR",
            _ => "NEXT"
        };

    private static void DrawPerson(object receiver, ElementBuilder svg, double x, double y, string color, bool highlighted)
    {
        var scale = highlighted ? 1.35 : 1;
        svg.Child(WorkshopGui.Element(receiver, "circle")
            .Attribute("cx", x.ToString("0.##")).Attribute("cy", (y - 4 * scale).ToString("0.##"))
            .Attribute("r", (1.2 * scale).ToString("0.##")).Attribute("fill", color));

        svg.Child(WorkshopGui.Element(receiver, "path")
            .Attribute("d",
                $"M {x - 1.6 * scale} {y + 1.5 * scale} " +
                $"Q {x} {y - 1.5 * scale} {x + 1.6 * scale} {y + 1.5 * scale} " +
                $"L {x + 2 * scale} {y + 5 * scale} H {x - 2 * scale} Z")
            .Attribute("fill", highlighted ? $"{color}55" : $"{color}22")
            .Attribute("stroke", color).Attribute("stroke-width", highlighted ? ".45" : ".3"));
    }

    private static ElementBuilder Label(object receiver, double x, double y, string text, string color, double size)
        => WorkshopGui.Element(receiver, "text")
            .Attribute("x", x.ToString("0.##")).Attribute("y", y.ToString("0.##"))
            .Attribute("fill", color).Attribute("font-size", size.ToString("0.##"))
            .Attribute("font-family", "Consolas,'Courier New',monospace")
            .Attribute("font-weight", size >= 1 ? "700" : "400")
            .Attribute("letter-spacing", size >= 1 ? ".06em" : ".03em")
            .Text(text);
}