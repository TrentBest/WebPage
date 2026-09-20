namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Renders an AEC plan experience as an explorable semantic floor plan.
/// Interactables breathe as data-driven capabilities; the renderer does not decide
/// what the building means.
/// </summary>
public static class SpatialAECPlanExperienceGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Yellow = "#ffd34d";
    private const string Magenta = "#ff38d1";
    private const string White = "#ffffff";
    private const string Ink = "#02080f";

    public static ElementBuilder Build(
        object receiver,
        SpatialAECPlanExperience experience,
        string avatarName,
        int floor,
        string? selectedInteractableId,
        double avatarX,
        double avatarY,
        Action<int> selectFloor,
        Action<string> approachInteractable,
        Action<double, double> walkTo,
        Action enterAuthoring,
        Action exit)
    {
        var view = experience.GetFloor(Math.Clamp(floor, 1, experience.Floors.Count));

        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0")
            .Style("overflow", "hidden")
            .Style("background", Ink).Style("color", White)
            .Style("font-family", "Consolas, 'Courier New', monospace");

        root.Content(Plan(receiver, experience, view, selectedInteractableId, avatarX, avatarY, approachInteractable, walkTo));
        root.Content(Header(receiver, experience, avatarName, view));
        root.Content(FloorRail(receiver, experience, view.Floor, selectFloor));
        root.Content(Inspector(receiver, experience, view, selectedInteractableId, approachInteractable, enterAuthoring));
        root.Content(Exit(receiver, exit));

        return root;
    }

    private static ElementBuilder Plan(
        object receiver,
        SpatialAECPlanExperience experience,
        SpatialAECPlanView view,
        string? selectedInteractableId,
        double avatarX,
        double avatarY,
        Action<string> approachInteractable,
        Action<double, double> walkTo)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 100")
            .Attribute("preserveAspectRatio", "xMidYMid meet")
            .Style("position", "absolute").Style("left", "0").Style("top", "10%")
            .Style("width", "68%").Style("height", "82%")
            .Style("background", "rgba(0,234,255,.012)");

        svg.Child(WorkshopGui.Element(receiver, "text")
            .Attribute("x", "50").Attribute("y", "5")
            .Attribute("fill", Yellow).Attribute("font-size", "2")
            .Attribute("text-anchor", "middle")
            .Text(view.Name));

        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", "10").Attribute("y", "10")
            .Attribute("width", "80").Attribute("height", "72")
            .Attribute("fill", "none").Attribute("stroke", Cyan)
            .Attribute("stroke-opacity", ".45").Attribute("stroke-width", ".45"));

        var scaleX = 80d / Math.Max(1, view.Width);
        var scaleY = 72d / Math.Max(1, view.Depth);

        foreach (var room in view.Rooms)
        {
            var x = 10 + room.X * scaleX;
            var y = 10 + room.Y * scaleY;
            var width = room.Width * scaleX;
            var height = room.Depth * scaleY;

            svg.Child(WorkshopGui.Element(receiver, "rect")
                .Attribute("x", x).Attribute("y", y)
                .Attribute("width", width).Attribute("height", height)
                .Attribute("fill", "rgba(0,234,255,.025)")
                .Attribute("stroke", "rgba(0,234,255,.35)").Attribute("stroke-width", ".25"));

            svg.Child(WorkshopGui.Element(receiver, "text")
                .Attribute("x", x + width / 2).Attribute("y", y + height / 2)
                .Attribute("fill", "#9ab0b8").Attribute("font-size", "1")
                .Attribute("text-anchor", "middle")
                .Text(room.Name));
        }

        var avatarScreenX = 10 + avatarX * scaleX;
        var avatarScreenY = 10 + avatarY * scaleY;

        svg.Child(WorkshopGui.Element(receiver, "circle")
            .Attribute("cx", avatarScreenX).Attribute("cy", avatarScreenY)
            .Attribute("r", "1.6")
            .Attribute("fill", "rgba(255,211,77,.28)")
            .Attribute("stroke", Yellow).Attribute("stroke-width", ".55"));

        svg.Child(WorkshopGui.Element(receiver, "circle")
            .Attribute("cx", avatarScreenX).Attribute("cy", avatarScreenY)
            .Attribute("r", "3.1")
            .Attribute("fill", "none")
            .Attribute("stroke", Yellow).Attribute("stroke-opacity", ".45")
            .Attribute("stroke-width", ".22"));

        foreach (var interactable in view.Interactables)
        {
            var x = 10 + interactable.X * scaleX;
            var y = 10 + interactable.Y * scaleY;
            var selected = string.Equals(interactable.Id, selectedInteractableId, StringComparison.OrdinalIgnoreCase);
            var button = WorkshopGui.Element(receiver, "g")
                .Attribute("role", "button")
                .Attribute("tabindex", "0")
                .Style("cursor", "pointer");

            button.Child(WorkshopGui.Element(receiver, "circle")
                .Attribute("cx", x).Attribute("cy", y)
                .Attribute("r", selected ? "2.5" : "1.8")
                .Attribute("fill", selected ? "rgba(255,211,77,.24)" : "rgba(0,234,255,.16)")
                .Attribute("stroke", selected ? Yellow : Cyan)
                .Attribute("stroke-width", selected ? ".7" : ".45"));

            button.Child(WorkshopGui.Element(receiver, "circle")
                .Attribute("cx", x).Attribute("cy", y).Attribute("r", "3.4")
                .Attribute("fill", "none").Attribute("stroke", selected ? Yellow : Cyan)
                .Attribute("stroke-opacity", ".55").Attribute("stroke-width", ".25"))
                .Child(WorkshopGui.Element(receiver, "animate")
                    .Attribute("attributeName", "r")
                    .Attribute("values", "2.5;4;2.5")
                    .Attribute("dur", "2.2s")
                    .Attribute("repeatCount", "indefinite"));

            button.Child(WorkshopGui.Element(receiver, "text")
                .Attribute("x", x + 3).Attribute("y", y - 2)
                .Attribute("fill", selected ? Yellow : White)
                .Attribute("font-size", ".9")
                .Text(interactable.Name));

            // WorkshopGui's element builder is intentionally event-oriented; attach the
            // interaction to the semantic group rather than inventing a renderer-level
            // object type.
            button.OnClick(() =>
            {
                walkTo(interactable.X, interactable.Y);
                approachInteractable(interactable.Id);
            });
            svg.Child(button);
        }

        return svg;
    }

    private static ElementBuilder Header(
        object receiver,
        SpatialAECPlanExperience experience,
        string avatarName,
        SpatialAECPlanView view)
        => WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("left", "2%").Style("top", "2%")
            .Style("padding", ".6rem .8rem")
            .Style("border", $"1px solid {Cyan}55")
            .Style("background", "rgba(1,6,12,.9)")
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("color", Cyan).Style("font-size", ".45rem")
                .Style("letter-spacing", ".15em")
                .Text("AEC PLAN WALK // SELECTED DEFAULT"))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".25rem").Style("color", White)
                .Style("font-family", "system-ui,sans-serif").Style("font-size", "1rem")
                .Text(experience.BuildingType.Name))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".2rem").Style("color", "#7895a1")
                .Style("font-size", ".32rem")
                .Text($"{avatarName} // {view.Name} // CODE BASIS: {experience.CodeProfile.Edition}"));

    private static ElementBuilder FloorRail(
        object receiver,
        SpatialAECPlanExperience experience,
        int currentFloor,
        Action<int> selectFloor)
    {
        var rail = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("left", "2%").Style("bottom", "3%")
            .Style("display", "flex").Style("gap", ".3rem")
            .Style("padding", ".45rem")
            .Style("border", "1px solid rgba(0,234,255,.28)")
            .Style("background", "rgba(1,6,12,.92)");

        foreach (var floor in experience.Floors)
        {
            rail.Content(WorkshopGui.Button(receiver)
                .Label($"L{floor.Floor:00}")
                .Style("padding", ".35rem .5rem")
                .Style("border", $"1px solid {(floor.Floor == currentFloor ? Yellow : Cyan)}66")
                .Style("background", floor.Floor == currentFloor ? "rgba(255,211,77,.08)" : "transparent")
                .Style("color", floor.Floor == currentFloor ? Yellow : Cyan)
                .Style("font-family", "inherit")
                .Style("font-size", ".35rem")
                .OnClick(() => selectFloor(floor.Floor)));
        }

        return rail;
    }

    private static ElementBuilder Inspector(
        object receiver,
        SpatialAECPlanExperience experience,
        SpatialAECPlanView view,
        string? selectedInteractableId,
        Action<string> approachInteractable,
        Action enterAuthoring)
    {
        var selected = view.Interactables.FirstOrDefault(x =>
            string.Equals(x.Id, selectedInteractableId, StringComparison.OrdinalIgnoreCase));

        var panel = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("right", "2%").Style("top", "13%")
            .Style("width", "min(27rem,28vw)")
            .Style("padding", ".85rem")
            .Style("border", $"1px solid {Cyan}55")
            .Style("background", "rgba(1,6,12,.94)");

        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("color", Yellow).Style("font-size", ".38rem")
            .Style("letter-spacing", ".14em")
            .Text("BUILDING CAPABILITY // BREATHING INTERACTABLES"));

        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("margin-top", ".55rem").Style("color", "#9ab0b8")
            .Style("font-size", ".34rem")
            .Text($"CODE BASIS // {experience.CodeProfile.Basis}"));

        if (selected is null)
        {
            panel.Content(WorkshopGui.Element(receiver, "p")
                .Style("font-family", "system-ui,sans-serif")
                .Style("font-size", ".8rem").Style("color", White)
                .Text("Walk the plan. The building will reveal capabilities as you approach the breathing markers."));
        }
        else
        {
            panel.Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".7rem").Style("color", Cyan)
                .Style("font-size", ".8rem")
                .Text(selected.Prompt));

            panel.Content(WorkshopGui.Element(receiver, "p")
                .Style("font-family", "system-ui,sans-serif")
                .Style("font-size", ".82rem").Style("line-height", "1.45")
                .Style("color", White)
                .Text(selected.Capability));

            panel.Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".5rem").Style("color", "#7895a1")
                .Style("font-size", ".34rem")
                .Text(selected.Detail));

            panel.Content(WorkshopGui.Button(receiver)
                .Label("APPROACH INTERACTABLE")
                .Style("margin-top", ".7rem").Style("width", "100%")
                .Style("padding", ".55rem")
                .Style("border", $"1px solid {Cyan}88")
                .Style("background", "rgba(0,234,255,.05)")
                .Style("color", Cyan).Style("font-family", "inherit")
                .Style("cursor", "pointer")
                .OnClick(() => approachInteractable(selected.Id)));
        }

        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("margin-top", ".7rem").Style("padding-top", ".55rem")
            .Style("border-top", "1px solid rgba(0,234,255,.12)")
            .Style("font-size", ".32rem").Style("color", "#7895a1")
            .Text(experience.CodeProfile.SafetyNote));

        panel.Content(WorkshopGui.Button(receiver)
            .Label("OPEN AUTHORING SURFACE →")
            .Style("margin-top", ".65rem").Style("width", "100%")
            .Style("padding", ".55rem")
            .Style("border", $"1px solid {Magenta}77")
            .Style("background", "rgba(255,56,209,.04)")
            .Style("color", Magenta).Style("font-family", "inherit")
            .Style("cursor", "pointer")
            .OnClick(enterAuthoring));

        return panel;
    }

    private static ElementBuilder Exit(object receiver, Action exit)
        => WorkshopGui.Button(receiver).Label("← RETURN TO CONSULTATION")
            .Style("position", "absolute").Style("right", "2%").Style("bottom", "3%")
            .Style("padding", ".5rem .7rem")
            .Style("border", $"1px solid {Magenta}66")
            .Style("background", "rgba(1,4,10,.9)")
            .Style("color", Magenta)
            .Style("font-family", "inherit").Style("font-size", ".36rem")
            .Style("cursor", "pointer").OnClick(exit);
}
