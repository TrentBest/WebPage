using Microsoft.AspNetCore.Components.Web;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Builds the Workshop Grand Central Station Experience.
/// Platforms are destinations, the station map is interactive, and a requested
/// train manifests at the selected platform rather than being a decorative sprite.
/// </summary>
public static class SpatialTransitStationGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Yellow = "#ffd34d";
    private const string Magenta = "#ff38d1";
    private const string Green = "#52e05a";
    private const string White = "#ffffff";

    public static ElementBuilder Build(
        object receiver,
        SpatialTransitManifest manifest,
        string userName,
        string? selectedDestinationId,
        bool trainVisible,
        Action<string> selectPlatform,
        Action<string> boardTrain,
        Action exit)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0")
            .Style("overflow", "hidden")
            .Style("background", "#01040a")
            .Style("color", White)
            .Style("font-family", "Consolas, 'Courier New', monospace");

        root.Content(Styles(receiver));
        root.Content(Header(receiver, userName));
        root.Content(Station(receiver, manifest, selectedDestinationId, trainVisible, selectPlatform, boardTrain));
        root.Content(ExitButton(receiver, exit));
        return root;
    }

    private static ElementBuilder Header(object receiver, string userName)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "fixed").Style("left", "1.4rem").Style("top", "1rem")
            .Style("z-index", "20").Style("pointer-events", "none")
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("color", Cyan).Style("font-size", "clamp(.8rem,1.7vw,1.25rem)")
                .Style("font-weight", "700").Style("letter-spacing", ".28em")
                .Text("SINGULARITY GRAND CENTRAL"))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".35rem").Style("color", Yellow)
                .Style("font-size", ".48rem").Style("letter-spacing", ".2em")
                .Text($"CENTRAL TRANSIT HUB // WELCOME {userName.ToUpperInvariant()}"));

    private static ElementBuilder Station(
        object receiver,
        SpatialTransitManifest manifest,
        string? selectedDestinationId,
        bool trainVisible,
        Action<string> selectPlatform,
        Action<string> boardTrain)
    {
        var station = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("inset", "0")
            .Style("padding", "5.5rem 2rem 3rem")
            .Style("box-sizing", "border-box").Style("overflow", "auto");

        var shell = WorkshopGui.Element(receiver, "div")
            .Style("position", "relative")
            .Style("min-height", "calc(100vh - 9rem)")
            .Style("max-width", "1500px").Style("margin", "0 auto")
            .Style("border", $"1px solid {Cyan}44")
            .Style("background", "radial-gradient(circle at 50% 45%, rgba(0,234,255,.08), transparent 52%), #020812")
            .Style("box-shadow", $"0 0 100px {Cyan}10,inset 0 0 80px {Cyan}08")
            .Style("overflow", "hidden");

        shell.Content(Roof(receiver));
        shell.Content(TrackDiagram(receiver, manifest));
        shell.Content(Concourse(receiver));
        shell.Content(PlatformRing(receiver, manifest, selectedDestinationId, trainVisible, selectPlatform, boardTrain));
        shell.Content(StatusBoard(receiver, manifest, selectedDestinationId, trainVisible));

        station.Content(shell);
        return station;
    }

    private static ElementBuilder Roof(object receiver)
    {
        var roof = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 60")
            .Attribute("preserveAspectRatio", "none")
            .Style("position", "absolute").Style("inset", "0")
            .Style("width", "100%").Style("height", "100%")
            .Style("pointer-events", "none").Style("opacity", ".65");

        for (var i = 0; i < 9; i++)
        {
            var x = 5 + i * 11.25;
            roof.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", x).Attribute("y1", "4")
                .Attribute("x2", "50").Attribute("y2", "48")
                .Attribute("stroke", Cyan).Attribute("stroke-opacity", ".12")
                .Attribute("stroke-width", ".18"));
        }

        for (var y = 8; y <= 48; y += 8)
            roof.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", "2").Attribute("y1", y)
                .Attribute("x2", "98").Attribute("y2", y)
                .Attribute("stroke", Cyan).Attribute("stroke-opacity", ".08")
                .Attribute("stroke-width", ".18"));

        return roof;
    }

    private static ElementBuilder TrackDiagram(object receiver, SpatialTransitManifest manifest)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 100")
            .Attribute("preserveAspectRatio", "none")
            .Style("position", "absolute").Style("inset", "0")
            .Style("width", "100%").Style("height", "100%")
            .Style("pointer-events", "none");

        for (var i = 0; i < manifest.TrackCenterline.Count - 1; i++)
        {
            var a = manifest.TrackCenterline[i];
            var b = manifest.TrackCenterline[i + 1];
            svg.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", a.X).Attribute("y1", a.Y)
                .Attribute("x2", b.X).Attribute("y2", b.Y)
                .Attribute("stroke", Cyan).Attribute("stroke-opacity", ".34")
                .Attribute("stroke-width", ".45"));
            svg.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", a.X).Attribute("y1", a.Y + .9)
                .Attribute("x2", b.X).Attribute("y2", b.Y + .9)
                .Attribute("stroke", White).Attribute("stroke-opacity", ".12")
                .Attribute("stroke-width", ".18"));
        }

        for (var i = 0; i < manifest.TrackCenterline.Count; i++)
        {
            var p = manifest.TrackCenterline[i];
            svg.Child(WorkshopGui.Element(receiver, "circle")
                .Attribute("cx", p.X).Attribute("cy", p.Y)
                .Attribute("r", "1.1").Attribute("fill", "#01040a")
                .Attribute("stroke", Cyan).Attribute("stroke-width", ".28"));
        }

        return svg;
    }

    private static ElementBuilder Concourse(object receiver)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "7%").Style("right", "7%")
            .Style("top", "15%").Style("height", "25%")
            .Style("border", $"1px solid {White}18")
            .Style("background", "rgba(255,255,255,.018)")
            .Style("display", "flex").Style("align-items", "center")
            .Style("justify-content", "center").Style("pointer-events", "none")
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("text-align", "center")
                .Content(WorkshopGui.Element(receiver, "div")
                    .Style("font-size", "clamp(1.1rem,3vw,2.6rem)")
                    .Style("letter-spacing", ".35em").Style("color", White)
                    .Style("opacity", ".72").Text("GRAND CENTRAL"))
                .Content(WorkshopGui.Element(receiver, "div")
                    .Style("margin-top", ".6rem").Style("font-size", ".46rem")
                    .Style("letter-spacing", ".32em").Style("color", Cyan)
                    .Text("ARRIVALS // DEPARTURES // DESTINATIONS // EXPERIENCE NETWORK")));

    private static ElementBuilder PlatformRing(
        object receiver,
        SpatialTransitManifest manifest,
        string? selectedDestinationId,
        bool trainVisible,
        Action<string> selectPlatform,
        Action<string> boardTrain)
    {
        var area = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "4%").Style("right", "4%")
            .Style("top", "43%").Style("bottom", "5%")
            .Style("display", "grid").Style("grid-template-columns", "repeat(auto-fit,minmax(180px,1fr))")
            .Style("gap", ".7rem").Style("align-items", "stretch").Style("overflow", "auto").Style("padding-bottom", ".5rem");

        foreach (var destination in manifest.Destinations)
        {
            var selected = destination.Id == selectedDestinationId;
            var platform = WorkshopGui.Panel(receiver)
                .Style("position", "relative")
                .Style("border", $"1px solid {(selected ? Yellow : Cyan)}{(selected ? "bb" : "44")}")
                .Style("background", selected ? $"{Yellow}0d" : "rgba(0,12,20,.65)")
                .Style("box-shadow", selected ? $"0 0 35px {Yellow}14,inset 0 0 25px {Yellow}08" : "none")
                .Style("padding", ".7rem").Style("box-sizing", "border-box")
                .Style("overflow", "hidden");

            platform.Content(WorkshopGui.Element(receiver, "div")
                .Style("font-size", ".48rem").Style("letter-spacing", ".16em")
                .Style("color", selected ? Yellow : Cyan)
                .Text($"PLATFORM {PlatformNumber(manifest, destination.Id):00}"));

            platform.Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".55rem").Style("font-size", ".7rem")
                .Style("font-weight", "700").Style("color", White)
                .Text(destination.Label));

            platform.Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".3rem").Style("font-size", ".42rem")
                .Style("line-height", "1.45").Style("color", "#9bb0b9")
                .Text($"DESTINATION // {destination.SceneId.ToUpperInvariant()}"));

            platform.Content(PlatformLine(receiver, selected));
            platform.Content(WorkshopGui.Button(receiver)
                .Label(selected ? "REQUEST TRAIN" : "SELECT PLATFORM")
                .Style("position", "absolute").Style("left", ".7rem").Style("right", ".7rem")
                .Style("bottom", ".7rem").Style("padding", ".45rem .5rem")
                .Style("border", $"1px solid {(selected ? Yellow : Cyan)}66")
                .Style("background", "rgba(1,4,10,.88)")
                .Style("color", selected ? Yellow : White)
                .Style("font-family", "inherit").Style("font-size", ".42rem")
                .Style("letter-spacing", ".1em").Style("cursor", "pointer")
                .OnClick(() => selectPlatform(destination.Id)));

            if (selected && trainVisible)
                platform.Content(Train(receiver, destination, () => boardTrain(destination.Id)));

            area.Content(platform);
        }

        return area;
    }

    private static ElementBuilder PlatformLine(object receiver, bool selected)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "0").Style("right", "0")
            .Style("top", "45%").Style("height", "3px")
            .Style("background", selected ? Yellow : Cyan)
            .Style("opacity", selected ? ".7" : ".25")
            .Style("box-shadow", selected ? $"0 0 16px {Yellow}" : "none")
            .Style("pointer-events", "none");

    private static ElementBuilder Train(object receiver, SpatialTransitDestination destination, Action board)
        => WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("left", "8%").Style("right", "8%")
            .Style("top", "31%").Style("height", "17%")
            .Style("z-index", "5")
            .Style("border", $"1px solid {Magenta}aa")
            .Style("background", "linear-gradient(90deg, rgba(255,56,209,.08), rgba(0,234,255,.12), rgba(255,56,209,.08))")
            .Style("box-shadow", $"0 0 32px {Magenta}18")
            .Style("animation", "workshop-train-arrive .75s ease-out both")
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("position", "absolute").Style("left", "1rem").Style("top", ".45rem")
                .Style("font-size", ".42rem").Style("letter-spacing", ".14em").Style("color", Yellow)
                .Text($"ARRIVING // {destination.Label}"))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("position", "absolute").Style("left", "1rem").Style("right", "1rem")
                .Style("bottom", ".55rem").Style("display", "flex")
                .Style("align-items", "center").Style("justify-content", "space-between")
                .Content(WorkshopGui.Element(receiver, "span")
                    .Style("font-size", ".48rem").Style("color", White)
                    .Text("TRAIN MANIFESTED // DOORS OPEN"))
                .Content(WorkshopGui.Button(receiver).Label("BOARD TRAIN →")
                    .Style("padding", ".4rem .55rem").Style("border", $"1px solid {Green}88")
                    .Style("background", $"{Green}10").Style("color", Green)
                    .Style("font-family", "inherit").Style("font-size", ".4rem")
                    .Style("letter-spacing", ".1em").Style("cursor", "pointer").OnClick(board)));

    private static ElementBuilder StatusBoard(object receiver, SpatialTransitManifest manifest, string? selectedDestinationId, bool trainVisible)
    {
        var destination = manifest.Destinations.FirstOrDefault(x => x.Id == selectedDestinationId);
        var selectedLabel = selectedDestinationId is null ? "SELECT A PLATFORM" : destination.Label;
        var state = trainVisible ? "TRAIN ARRIVED // BOARDING OPEN" : "AWAITING DESTINATION REQUEST";

        return WorkshopGui.Element(receiver, "div")
            .Style("position", "fixed").Style("right", "1.4rem").Style("top", "1rem")
            .Style("z-index", "21").Style("padding", ".55rem .7rem")
            .Style("border", $"1px solid {Yellow}55").Style("background", "rgba(1,4,10,.88)")
            .Style("min-width", "220px").Style("pointer-events", "none")
            .Content(WorkshopGui.Element(receiver, "div").Style("font-size", ".4rem").Style("letter-spacing", ".16em").Style("color", Yellow).Text("CENTRAL DEPARTURES"))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".35rem").Style("font-size", ".52rem").Style("color", White).Text(selectedLabel))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".25rem").Style("font-size", ".38rem").Style("color", trainVisible ? Green : "#8296a0").Text(state));
    }

    private static ElementBuilder ExitButton(object receiver, Action exit)
        => WorkshopGui.Button(receiver).Label("← RETURN TO WORKSHOP")
            .Style("position", "fixed").Style("left", "1.4rem").Style("bottom", "1rem")
            .Style("z-index", "30").Style("padding", ".5rem .7rem")
            .Style("border", $"1px solid {Magenta}66").Style("background", "rgba(20,4,18,.88)")
            .Style("color", Magenta).Style("font-family", "inherit")
            .Style("font-size", ".42rem").Style("letter-spacing", ".1em").Style("cursor", "pointer")
            .OnClick(exit);

    private static int PlatformNumber(SpatialTransitManifest manifest, string id)
    {
        for (var i = 0; i < manifest.Destinations.Count; i++)
            if (manifest.Destinations[i].Id == id) return i + 1;
        return 0;
    }

    private static ElementBuilder Styles(object receiver)
        => WorkshopGui.Element(receiver, "style").Text(
            "@keyframes workshop-train-arrive{0%{transform:translateX(45%);opacity:0}100%{transform:translateX(0);opacity:1}}" +
            "@keyframes workshop-platform-breathe{0%,100%{opacity:.65}50%{opacity:1}}");
}
