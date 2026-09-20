namespace TheSingularityWorkshop.Gui;

using System;
using System.Linq;

/// <summary>
/// Presents the Singularity Lab as a Dragon-Warrior-style top-down spatial experience,
/// with deliberate elevation views for security and the secure elevator.
/// </summary>
public static class SpatialSingularityLabGuiBuilder
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
        SpatialSingularityLabModel lab,
        string userName,
        Action exit,
        Action enterSecurity,
        Action placeBelongings,
        Action completeScreening,
        Action enterElevator,
        Action scanId,
        Action scanFingerprint,
        Action scanRetina,
        Action<int> selectFloor,
        Action resolveFloorChallenge,
        Action returnToElevator)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0")
            .Style("width", "100vw").Style("height", "100vh")
            .Style("overflow", "hidden")
            .Style("background", Ink)
            .Style("color", White)
            .Style("font-family", "Consolas, 'Courier New', monospace");

        if (lab.IsElevation)
            root.Content(ElevatorElevation(receiver, lab, userName, scanId, scanFingerprint, scanRetina, selectFloor, returnToElevator));
        else
            root.Content(TopDown(receiver, lab, userName, enterSecurity, placeBelongings, completeScreening, enterElevator, resolveFloorChallenge, returnToElevator));

        root.Content(Status(receiver, lab));
        root.Content(Exit(receiver, exit));
        return root;
    }

    private static ElementBuilder TopDown(
        object receiver,
        SpatialSingularityLabModel lab,
        string userName,
        Action enterSecurity,
        Action placeBelongings,
        Action completeScreening,
        Action enterElevator,
        Action resolveFloorChallenge,
        Action returnToElevator)
    {
        var canvas = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("inset", "0")
            .Style("background", "radial-gradient(circle at 50% 50%, #102633 0, #02080f 62%)")
            .Style("overflow", "hidden");

        canvas.Content(BuildingPlan(receiver, lab));
        canvas.Content(Avatar(receiver, lab, userName));

        var panel = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("left", "2rem").Style("top", "2rem")
            .Style("width", "min(27rem,38vw)").Style("padding", ".85rem")
            .Style("border", $"1px solid {Cyan}55")
            .Style("background", "rgba(1,6,12,.91)")
            .Style("z-index", "30");

        panel.Content(WorkshopGui.Element(receiver, "div").Style("font-size", ".46rem").Style("letter-spacing", ".16em").Style("color", Cyan)
            .Text("SINGULARITY LAB // SPATIAL EXPERIENCE"));
        panel.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".25rem").Style("font-size", ".34rem").Style("color", Muted)
            .Text("DRAGON-WARRIOR MODE // TOP VIEW // CAMERA FOLLOWS YOU"));

        var action = lab.Stage switch
        {
            SpatialSingularityLabStage.ApproachSecurity => WorkshopGui.Button(receiver).Label("CLICK SECURITY LINE → BEGIN SCREENING")
                .OnClick(enterSecurity),
            SpatialSingularityLabStage.SecurityQueue => WorkshopGui.Button(receiver).Label("PUT BELONGINGS IN BUCKET →")
                .OnClick(placeBelongings),
            SpatialSingularityLabStage.SecurityScreening => WorkshopGui.Button(receiver).Label("WALK THROUGH SCREENING MACHINE →")
                .OnClick(completeScreening),
            SpatialSingularityLabStage.SecurityComplete => WorkshopGui.Button(receiver).Label("APPROACH ELEVATOR → ENTER ELEVATION")
                .OnClick(enterElevator),
            SpatialSingularityLabStage.FloorChallenge => WorkshopGui.Button(receiver).Label("FACE THE GUARD POST →")
                .OnClick(resolveFloorChallenge),
            SpatialSingularityLabStage.AccessDenied => WorkshopGui.Button(receiver).Label("RETURN TO ELEVATOR")
                .OnClick(returnToElevator),
            SpatialSingularityLabStage.FloorOpen => WorkshopGui.Button(receiver).Label("RETURN TO ELEVATOR")
                .OnClick(returnToElevator),
            _ => WorkshopGui.Button(receiver).Label("CONTINUE")
        };

        action.Style("margin-top", ".7rem").Style("width", "100%").Style("padding", ".65rem")
            .Style("border", $"1px solid {Yellow}88").Style("background", "rgba(255,211,77,.05)")
            .Style("color", Yellow).Style("font-family", "inherit").Style("font-size", ".38rem")
            .Style("cursor", "pointer");
        panel.Content(action);

        if (lab.Stage == SpatialSingularityLabStage.FloorOpen && lab.SelectedFloor is int floor)
            panel.Content(FloorPreview(receiver, lab, floor));

        if (lab.Stage == SpatialSingularityLabStage.AccessDenied)
            panel.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".6rem").Style("padding", ".55rem")
                .Style("border", $"1px solid {Magenta}66").Style("color", Magenta).Style("font-family", "system-ui,sans-serif")
                .Style("font-size", ".75rem").Text(lab.LastMessage));

        canvas.Content(panel);
        return canvas;
    }

    private static ElementBuilder BuildingPlan(object receiver, SpatialSingularityLabModel lab)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 100")
            .Style("position", "absolute").Style("inset", "0")
            .Style("width", "100vw").Style("height", "100vh");

        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", "20").Attribute("y", "14").Attribute("width", "60").Attribute("height", "72")
            .Attribute("fill", "rgba(0,234,255,.018)").Attribute("stroke", Cyan).Attribute("stroke-width", ".5"));

        Room(svg, receiver, 24, 20, 17, 15, "RECEPTION", Cyan);
        Room(svg, receiver, 43, 20, 18, 15, "DIRECTOR", Yellow);
        Room(svg, receiver, 63, 20, 13, 15, "CONFERENCE", Magenta);
        Room(svg, receiver, 24, 38, 25, 22, "SECURITY", Yellow);
        Room(svg, receiver, 52, 38, 24, 22, "SCREENING", Cyan);
        Room(svg, receiver, 24, 63, 52, 17, "ELEVATOR / VERTICAL CORE", Magenta);

        foreach (var guard in lab.Guards.Where(g => g.Post.StartsWith("SECURITY", StringComparison.OrdinalIgnoreCase)))
            GuardDot(svg, receiver, guard, guard.Post.Contains("LEFT", StringComparison.OrdinalIgnoreCase) ? 28 : 70, 48);

        if (lab.HasPassedSecurity)
        {
            foreach (var guard in lab.Guards.Where(g => g.Post.StartsWith("ELEVATOR", StringComparison.OrdinalIgnoreCase)))
                GuardDot(svg, receiver, guard, guard.Post.Contains("LEFT", StringComparison.OrdinalIgnoreCase) ? 39 : 61, 70);
        }

        svg.Child(WorkshopGui.Element(receiver, "text").Attribute("x", "50").Attribute("y", "9")
            .Attribute("fill", Yellow).Attribute("font-size", "2.1").Attribute("text-anchor", "middle")
            .Text("SINGULARITY LAB // MAIN ENTRANCE"));

        return svg;
    }

    private static void Room(ElementBuilder svg, object receiver, double x, double y, double w, double h, string label, string stroke)
    {
        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", x).Attribute("y", y).Attribute("width", w).Attribute("height", h)
            .Attribute("fill", "rgba(0,0,0,.12)").Attribute("stroke", stroke).Attribute("stroke-opacity", ".55").Attribute("stroke-width", ".3"));
        svg.Child(WorkshopGui.Element(receiver, "text")
            .Attribute("x", x + w / 2).Attribute("y", y + h / 2)
            .Attribute("fill", White).Attribute("font-size", "1.05").Attribute("text-anchor", "middle")
            .Text(label));
    }

    private static void GuardDot(ElementBuilder svg, object receiver, SpatialLabGuard guard, double x, double y)
    {
        svg.Child(WorkshopGui.Element(receiver, "circle")
            .Attribute("cx", x).Attribute("cy", y).Attribute("r", "1.4")
            .Attribute("fill", Magenta).Attribute("stroke", White).Attribute("stroke-width", ".25"));
        svg.Child(WorkshopGui.Element(receiver, "text")
            .Attribute("x", x).Attribute("y", y - 2)
            .Attribute("fill", White).Attribute("font-size", ".7").Attribute("text-anchor", "middle")
            .Text(guard.Name));
    }

    private static ElementBuilder Avatar(object receiver, SpatialSingularityLabModel lab, string userName)
    {
        var (x, y) = lab.Stage switch
        {
            SpatialSingularityLabStage.ApproachSecurity => (34d, 67d),
            SpatialSingularityLabStage.SecurityQueue => (34d, 48d),
            SpatialSingularityLabStage.SecurityScreening => (61d, 48d),
            SpatialSingularityLabStage.SecurityComplete => (50d, 68d),
            SpatialSingularityLabStage.FloorChallenge => (50d, 72d),
            SpatialSingularityLabStage.AccessDenied => (50d, 72d),
            SpatialSingularityLabStage.FloorOpen => (50d, 76d),
            _ => (50d, 68d)
        };

        return WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", $"{x}%").Style("top", $"{y}%")
            .Style("transform", "translate(-50%,-50%)").Style("z-index", "40")
            .Style("text-align", "center").Style("pointer-events", "none")
            .Content(WorkshopGui.Element(receiver, "div").Style("font-size", "1.2rem").Style("color", Cyan).Style("text-shadow", $"0 0 14px {Cyan}").Text("◉"))
            .Content(WorkshopGui.Element(receiver, "div").Style("font-size", ".32rem").Style("color", Cyan).Text($"YOU ARE HERE // {userName}"));
    }

    private static ElementBuilder ElevatorElevation(
        object receiver,
        SpatialSingularityLabModel lab,
        string userName,
        Action scanId,
        Action scanFingerprint,
        Action scanRetina,
        Action<int> selectFloor,
        Action returnToElevator)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 100")
            .Style("position", "absolute").Style("inset", "0")
            .Style("width", "100vw").Style("height", "100vh")
            .Style("background", "linear-gradient(#07131c,#02080f) ");

        svg.Child(WorkshopGui.Element(receiver, "rect").Attribute("x", "30").Attribute("y", "15").Attribute("width", "40").Attribute("height", "70")
            .Attribute("fill", "rgba(0,234,255,.025)").Attribute("stroke", Cyan).Attribute("stroke-width", ".45"));
        svg.Child(WorkshopGui.Element(receiver, "rect").Attribute("x", "40").Attribute("y", "24").Attribute("width", "20").Attribute("height", "48")
            .Attribute("fill", "rgba(255,255,255,.015)").Attribute("stroke", White).Attribute("stroke-width", ".35"));
        svg.Child(WorkshopGui.Element(receiver, "line").Attribute("x1", "50").Attribute("y1", "24").Attribute("x2", "50").Attribute("y2", "72")
            .Attribute("stroke", Muted).Attribute("stroke-width", ".2"));

        GuardElevation(svg, receiver, lab.Guards.First(g => g.Id == "guard-01"), 25, 57);
        GuardElevation(svg, receiver, lab.Guards.First(g => g.Id == "guard-02"), 75, 57);

        svg.Child(WorkshopGui.Element(receiver, "rect").Attribute("x", "74").Attribute("y", "28").Attribute("width", "15").Attribute("height", "22")
            .Attribute("fill", "rgba(255,211,77,.035)").Attribute("stroke", Yellow).Attribute("stroke-width", ".3"));
        svg.Child(WorkshopGui.Element(receiver, "text").Attribute("x", "81.5").Attribute("y", "33").Attribute("fill", Yellow).Attribute("font-size", ".85").Attribute("text-anchor", "middle").Text("SECURITY"));
        svg.Child(WorkshopGui.Element(receiver, "text").Attribute("x", "81.5").Attribute("y", "36").Attribute("fill", White).Attribute("font-size", ".7").Attribute("text-anchor", "middle").Text("CONSOLE"));

        svg.Child(WorkshopGui.Element(receiver, "rect").Attribute("x", "77").Attribute("y", "39").Attribute("width", "9").Attribute("height", "4")
            .Attribute("fill", "rgba(0,234,255,.08)").Attribute("stroke", Cyan).Attribute("stroke-width", ".25"));
        svg.Child(WorkshopGui.Element(receiver, "text").Attribute("x", "81.5").Attribute("y", "41.8").Attribute("fill", Cyan).Attribute("font-size", ".55").Attribute("text-anchor", "middle").Text("ID READER"));

        var panel = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("left", "50%").Style("bottom", "4%").Style("transform", "translateX(-50%)")
            .Style("width", "min(42rem,88vw)").Style("padding", ".8rem")
            .Style("border", $"1px solid {Yellow}66").Style("background", "rgba(1,6,12,.95)")
            .Style("text-align", "center").Style("z-index", "50");

        panel.Content(WorkshopGui.Element(receiver, "div").Style("font-size", ".42rem").Style("letter-spacing", ".15em").Style("color", Yellow)
            .Text($"ELEVATOR SECURITY // {lab.Stage.ToString().ToUpperInvariant()}"));
        panel.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".35rem").Style("font-family", "system-ui,sans-serif").Style("font-size", ".82rem").Style("color", White)
            .Text(lab.LastMessage));

        switch (lab.Stage)
        {
            case SpatialSingularityLabStage.ElevatorElevation:
                panel.Content(ActionButton(receiver, "SCAN ID CARD", scanId, Cyan));
                break;
            case SpatialSingularityLabStage.Fingerprint:
                panel.Content(ActionButton(receiver, "TOUCH TERMINAL // FINGERPRINT", scanFingerprint, Cyan));
                break;
            case SpatialSingularityLabStage.RetinalScan:
                panel.Content(ActionButton(receiver, "LOOK INTO RETINAL SCANNER", scanRetina, Magenta));
                break;
            case SpatialSingularityLabStage.FloorSelection:
                var floors = WorkshopGui.Element(receiver, "div").Style("display", "flex").Style("gap", ".3rem").Style("flex-wrap", "wrap").Style("justify-content", "center").Style("margin-top", ".55rem");
                foreach (var floor in lab.Floors)
                {
                    var locked = !floor.Authorized;
                    floors.Content(WorkshopGui.Button(receiver).Label($"{(locked ? "🔒 " : "")}FLOOR {floor.Level}")
                        .Style("padding", ".45rem .55rem").Style("border", $"1px solid {(locked ? Muted : Yellow)}66")
                        .Style("background", locked ? "rgba(82,97,106,.06)" : "rgba(255,211,77,.05)")
                        .Style("color", locked ? Muted : Yellow).Style("font-family", "inherit").Style("font-size", ".35rem")
                        .Style("cursor", "pointer").OnClick(() => selectFloor(floor.Level)));
                }
                panel.Content(floors);
                break;
            case SpatialSingularityLabStage.FloorChallenge:
                panel.Content(ActionButton(receiver, "CHALLENGE THE GUARD POST", () => { }, Yellow));
                break;
            case SpatialSingularityLabStage.AccessDenied:
                panel.Content(ActionButton(receiver, "RETURN TO ELEVATOR CONTROLS", returnToElevator, Magenta));
                break;
        }

        return WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("inset", "0")
            .Content(svg)
            .Content(panel);
    }

    private static void GuardElevation(ElementBuilder svg, object receiver, SpatialLabGuard guard, double x, double y)
    {
        svg.Child(WorkshopGui.Element(receiver, "rect").Attribute("x", x - 5).Attribute("y", y - 11).Attribute("width", "10").Attribute("height", "22")
            .Attribute("fill", "rgba(255,56,209,.025)").Attribute("stroke", Magenta).Attribute("stroke-opacity", ".3").Attribute("stroke-width", ".25"));
        svg.Child(WorkshopGui.Element(receiver, "text").Attribute("x", x).Attribute("y", y - 7).Attribute("fill", White).Attribute("font-size", ".8").Attribute("text-anchor", "middle").Text("GUARD"));
        svg.Child(WorkshopGui.Element(receiver, "text").Attribute("x", x).Attribute("y", y - 5).Attribute("fill", Yellow).Attribute("font-size", ".7").Attribute("text-anchor", "middle").Text(guard.Name));
        svg.Child(WorkshopGui.Element(receiver, "circle").Attribute("cx", x).Attribute("cy", y + 2).Attribute("r", "2").Attribute("fill", Magenta).Attribute("stroke", White).Attribute("stroke-width", ".3"));
    }

    private static ElementBuilder FloorPreview(object receiver, SpatialSingularityLabModel lab, int floor)
    {
        var info = lab.Floors.First(x => x.Level == floor);
        return WorkshopGui.Panel(receiver)
            .Style("margin-top", ".65rem").Style("padding", ".55rem")
            .Style("border", $"1px solid {Green}55").Style("background", "rgba(82,224,90,.025)")
            .Content(WorkshopGui.Element(receiver, "div").Style("font-size", ".45rem").Style("color", Green).Text(info.Name))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".25rem").Style("font-size", ".35rem").Style("color", Muted).Text(info.Description));
    }

    private static ElementBuilder Status(object receiver, SpatialSingularityLabModel lab)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("right", "2rem").Style("top", "2rem").Style("z-index", "80")
            .Style("max-width", "25rem").Style("padding", ".55rem .7rem")
            .Style("border", $"1px solid {Cyan}44").Style("background", "rgba(1,6,12,.82)")
            .Content(WorkshopGui.Element(receiver, "div").Style("font-size", ".32rem").Style("color", Cyan).Text($"ACCESS CARD // {lab.AccessCard}"))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".25rem").Style("font-size", ".38rem").Style("color", White).Text(lab.LastMessage));

    private static ElementBuilder ActionButton(object receiver, string label, Action action, string accent)
        => WorkshopGui.Button(receiver).Label(label)
            .Style("margin-top", ".55rem").Style("padding", ".55rem .8rem")
            .Style($"border", $"1px solid {accent}88").Style("background", "rgba(0,234,255,.04)")
            .Style("color", accent).Style("font-family", "inherit").Style("font-size", ".38rem")
            .Style("cursor", "pointer").OnClick(action);

    private static ElementBuilder Exit(object receiver, Action exit)
        => WorkshopGui.Button(receiver).Label("← RETURN TO CAMPUS")
            .Style("position", "absolute").Style("left", "2rem").Style("bottom", "1.5rem").Style("z-index", "100")
            .Style("padding", ".5rem .7rem").Style("border", $"1px solid {Magenta}66")
            .Style("background", "rgba(1,4,10,.9)").Style("color", Magenta)
            .Style("font-family", "inherit").Style("font-size", ".38rem").Style("cursor", "pointer")
            .OnClick(exit);
}
