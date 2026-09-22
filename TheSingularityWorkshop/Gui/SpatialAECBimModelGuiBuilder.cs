namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Conference-room BIM workbench. The physical metaphor is deliberate:
/// a scale model sits on the table, floors are removable slices, and the
/// ontology catalog supplies atomic rooms that can be placed into the active level.
/// </summary>
public static class SpatialAECBimModelGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Yellow = "#ffd34d";
    private const string Magenta = "#ff38d1";
    private const string Green = "#52e05a";
    private const string White = "#ffffff";
    private const string Muted = "#7895a1";
    private const string Ink = "#02080f";

    public static ElementBuilder Build(
        object receiver,
        SpatialAECBimModel model,
        string avatarName,
        Action<int> setFloor,
        Action<string> search,
        Action<string> addRoom,
        Action<string> duplicateRoom,
        Action<string> removeRoom,
        Action exit)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0")
            .Style("overflow", "hidden")
            .Style("background", Ink).Style("color", White)
            .Style("font-family", "Consolas, 'Courier New', monospace");

        root.Content(ConferenceRoom(receiver, model, avatarName));
        root.Content(TableModel(receiver, model, duplicateRoom, removeRoom));
        root.Content(OntologyPalette(receiver, model, search, addRoom));
        root.Content(FloorControls(receiver, model, setFloor));
        root.Content(DataPanel(receiver, model));
        root.Content(WorkshopGui.Button(receiver).Label("← RETURN TO AEC LOBBY")
            .Style("position", "fixed").Style("right", "1rem").Style("bottom", "1rem")
            .Style("z-index", "60").Style("padding", ".45rem .7rem")
            .Style("border", $"1px solid {Magenta}88").Style("background", $"{Magenta}0a")
            .Style("color", Magenta).Style("font-family", "inherit").Style("cursor", "pointer")
            .OnClick(exit));

        return root;
    }

    private static ElementBuilder ConferenceRoom(object receiver, SpatialAECBimModel model, string avatarName)
        => WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 100")
            .Style("position", "absolute").Style("inset", "0").Style("width", "100vw").Style("height", "100vh")
            .Style("background", "radial-gradient(circle at 50% 45%,#112936 0,#02080f 62%)")
            .Content(WorkshopGui.Element(receiver, "rect")
                .Attribute("x", "5").Attribute("y", "8").Attribute("width", "90").Attribute("height", "84")
                .Attribute("fill", "none").Attribute("stroke", Cyan).Attribute("stroke-opacity", ".22").Attribute("stroke-width", ".3"))
            .Content(WorkshopGui.Element(receiver, "rect")
                .Attribute("x", "28").Attribute("y", "34").Attribute("width", "44").Attribute("height", "30")
                .Attribute("rx", "3").Attribute("fill", "rgba(255,211,77,.035)")
                .Attribute("stroke", Yellow).Attribute("stroke-opacity", ".55").Attribute("stroke-width", ".5"))
            .Content(WorkshopGui.Element(receiver, "text").Attribute("x", "50").Attribute("y", "50")
                .Attribute("fill", Yellow).Attribute("font-size", "1.5").Attribute("text-anchor", "middle").Text("CONFERENCE TABLE"))
            .Content(WorkshopGui.Element(receiver, "text").Attribute("x", "50").Attribute("y", "53")
                .Attribute("fill", Muted).Attribute("font-size", ".8").Attribute("text-anchor", "middle")
                .Text($"{model.BuildingName.ToUpperInvariant()} // BIM REVIEW"))
            .Content(WorkshopGui.Element(receiver, "text").Attribute("x", "50").Attribute("y", "12")
                .Attribute("fill", Magenta).Attribute("font-size", "1.7").Attribute("text-anchor", "middle")
                .Text("AEC CONFERENCE ROOM // SCALE MODEL WORKBENCH"))
            .Content(WorkshopGui.Element(receiver, "text").Attribute("x", "50").Attribute("y", "15")
                .Attribute("fill", Muted).Attribute("font-size", ".8").Attribute("text-anchor", "middle")
                .Text($"VISITOR // {avatarName.ToUpperInvariant()} // ACTIVE FLOOR {model.ActiveFloor}"));

    private static ElementBuilder TableModel(object receiver, SpatialAECBimModel model, Action<string> duplicateRoom, Action<string> removeRoom)
    {
        var panel = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("left", "2%").Style("top", "20%").Style("z-index", "20")
            .Style("width", "30%").Style("max-height", "58%").Style("overflow", "auto")
            .Style("padding", ".7rem").Style("border", $"1px solid {Yellow}55")
            .Style("background", "rgba(1,6,12,.94)");

        panel.Content(WorkshopGui.Element(receiver, "div").Style("color", Yellow).Style("font-size", ".45rem").Style("letter-spacing", ".15em")
            .Text($"SCALE MODEL // FLOOR {model.ActiveFloor} + BELOW"));

        foreach (var room in model.RoomsOnOrBelowActiveFloor)
        {
            var accent = room.Floor == model.ActiveFloor ? Cyan : Muted;
            panel.Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".35rem").Style("padding", ".35rem")
                .Style("border-left", $"3px solid {accent}").Style("background", $"{accent}08")
                .Content(WorkshopGui.Element(receiver, "div").Style("color", White).Style("font-size", ".4rem")
                    .Text($"F{room.Floor} // {room.Name}"))
                .Content(WorkshopGui.Element(receiver, "div").Style("color", accent).Style("font-size", ".32rem")
                    .Text($"{room.Category} // {string.Join(" / ", room.Tags)}"))
                .Content(WorkshopGui.Button(receiver).Label("DUPLICATE")
                    .Style("margin-top", ".25rem").Style("margin-right", ".2rem").Style("font-size", ".3rem").Style("cursor", "pointer")
                    .OnClick(() => duplicateRoom(room.Id)))
                .Content(WorkshopGui.Button(receiver).Label("REMOVE")
                    .Style("font-size", ".3rem").Style("cursor", "pointer").Style("color", Magenta)
                    .OnClick(() => removeRoom(room.Id))));
        }

        return panel;
    }

    private static ElementBuilder OntologyPalette(object receiver, SpatialAECBimModel model, Action<string> search, Action<string> addRoom)
    {
        var panel = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("right", "2%").Style("top", "20%").Style("z-index", "20")
            .Style("width", "30%").Style("max-height", "58%").Style("overflow", "auto")
            .Style("padding", ".7rem").Style("border", $"1px solid {Cyan}55")
            .Style("background", "rgba(1,6,12,.94)");

        panel.Content(WorkshopGui.Element(receiver, "div").Style("color", Cyan).Style("font-size", ".45rem").Style("letter-spacing", ".15em")
            .Text("ONTOLOGY MALL // CONTENT PALETTE"));

        var input = WorkshopGui.Element(receiver, "input")
            .Attribute("value", model.SearchQuery)
            .Attribute("placeholder", "Search ontology // Idler // Flex // MEP ...")
            .Style("width", "100%").Style("box-sizing", "border-box").Style("padding", ".45rem")
            .Style("background", "rgba(0,234,255,.04)").Style("border", $"1px solid {Cyan}44")
            .Style("color", White).Style("font-family", "inherit").Style("font-size", ".38rem")
            .OnInput((Microsoft.AspNetCore.Components.ChangeEventArgs args) => search(args.Value?.ToString() ?? ""));
        panel.Content(input);

        foreach (var content in model.SearchResults)
        {
            panel.Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".35rem").Style("padding", ".4rem")
                .Style("border", $"1px solid {Green}44").Style("background", $"{Green}07")
                .Content(WorkshopGui.Element(receiver, "div").Style("color", White).Style("font-size", ".4rem").Text(content.Name))
                .Content(WorkshopGui.Element(receiver, "div").Style("color", Muted).Style("font-size", ".31rem").Text($"{content.Category} // {string.Join(" / ", content.Tags)}"))
                .Content(WorkshopGui.Button(receiver).Label("ADD TO ACTIVE FLOOR")
                    .Style("margin-top", ".25rem").Style("padding", ".28rem .4rem")
                    .Style("border", $"1px solid {Green}66").Style("color", Green)
                    .Style("font-family", "inherit").Style("font-size", ".3rem").Style("cursor", "pointer")
                    .OnClick(() => addRoom(content.Id))));
        }

        return panel;
    }

    private static ElementBuilder FloorControls(object receiver, SpatialAECBimModel model, Action<int> setFloor)
    {
        var controls = WorkshopGui.Element(receiver, "div")
            .Style("position", "fixed").Style("left", "50%").Style("bottom", "3rem")
            .Style("transform", "translateX(-50%)").Style("z-index", "40")
            .Style("display", "flex").Style("gap", ".3rem").Style("padding", ".45rem")
            .Style("border", $"1px solid {Yellow}55").Style("background", "rgba(1,6,12,.92)");

        foreach (var floor in model.Floors)
            controls.Content(WorkshopGui.Button(receiver).Label(floor == model.ActiveFloor ? $"▶ FLOOR {floor}" : $"FLOOR {floor}")
                .Style("padding", ".3rem .45rem").Style("font-family", "inherit").Style("font-size", ".33rem")
                .Style("cursor", "pointer").Style("color", floor == model.ActiveFloor ? Cyan : Muted)
                .OnClick(() => setFloor(floor)));

        return controls;
    }

    private static ElementBuilder DataPanel(object receiver, SpatialAECBimModel model)
        => WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("left", "50%").Style("top", "19%")
            .Style("transform", "translateX(-50%)").Style("z-index", "25")
            .Style("width", "min(24rem,28vw)").Style("padding", ".55rem")
            .Style("border", $"1px solid {Magenta}44").Style("background", "rgba(1,6,12,.8)")
            .Content(WorkshopGui.Element(receiver, "div").Style("color", Magenta).Style("font-size", ".38rem").Style("letter-spacing", ".14em")
                .Text("WALL / PROJECTOR // DATA ELEVATION"))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".3rem").Style("color", White).Style("font-size", ".35rem")
                .Text($"MODEL {model.BuildingId.ToUpperInvariant()} // ROOMS {model.Rooms.Count} // ACTIVE {model.ActiveFloor}"))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".25rem").Style("color", Muted).Style("font-size", ".31rem")
                .Text("OWNERSHIP // GEOMETRY / PROGRAM / SYSTEMS / BEHAVIOR / SOURCE"));

}
