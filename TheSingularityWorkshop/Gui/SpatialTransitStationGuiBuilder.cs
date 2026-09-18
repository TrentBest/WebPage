using Microsoft.AspNetCore.Components.Web;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Builds the spatial Grand Central Experience. The station is intentionally modeled as
/// infrastructure rather than a destination list: levels, platforms, tracks, passengers,
/// and boarding points are rendered from the transit manifest.
/// </summary>
public static class SpatialTransitStationGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Yellow = "#ffd34d";
    private const string Magenta = "#ff38d1";
    private const string Green = "#52e05a";
    private const string White = "#ffffff";
    private const string Dim = "#78909c";

    public static ElementBuilder Build(
        object receiver,
        SpatialTransitManifest manifest,
        string userName,
        string? selectedDestinationId,
        bool trainVisible,
        Action<string> selectPlatform,
        Action<string> boardTrain,
        Action exit,
        bool microGameActive,
        int microGameScore,
        int microGameRound,
        string? microGameTargetId,
        Action startMicroGame,
        Action<string> selectMicroGamePlatform)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0")
            .Style("overflow", "hidden")
            .Style("background", "#01040a")
            .Style("color", White)
            .Style("font-family", "Consolas, 'Courier New', monospace");

        root.Content(Styles(receiver));
        root.Content(Header(receiver, userName, manifest));
        root.Content(Station(receiver, manifest, selectedDestinationId, trainVisible, selectPlatform, boardTrain));
        root.Content(PassengerFlow(receiver, manifest));
        root.Content(OperationsKiosk(receiver, microGameActive, microGameScore, microGameRound, microGameTargetId, manifest, startMicroGame, selectMicroGamePlatform));
        root.Content(ExitButton(receiver, exit));
        return root;
    }

    private static ElementBuilder Header(object receiver, string userName, SpatialTransitManifest manifest)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "fixed").Style("left", "1.4rem").Style("top", "1rem")
            .Style("z-index", "30").Style("pointer-events", "none")
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("color", Cyan).Style("font-size", "clamp(.9rem,2vw,1.5rem)")
                .Style("font-weight", "700").Style("letter-spacing", ".24em")
                .Text("SINGULARITY GRAND CENTRAL"))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".35rem").Style("color", Yellow)
                .Style("font-size", ".48rem").Style("letter-spacing", ".18em")
                .Text($"CENTRAL TRANSIT HUB // {manifest.Platforms.Count} PLATFORMS // {manifest.Levels.Count} LEVELS // WELCOME {userName.ToUpperInvariant()}"));

    private static ElementBuilder Station(
        object receiver,
        SpatialTransitManifest manifest,
        string? selectedDestinationId,
        bool trainVisible,
        Action<string> selectPlatform,
        Action<string> boardTrain)
    {
        var station = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("inset", "0")
            .Style("padding", "6rem 2rem 2.5rem")
            .Style("box-sizing", "border-box").Style("overflow", "auto");

        var shell = WorkshopGui.Element(receiver, "div")
            .Style("position", "relative")
            .Style("min-height", "calc(100vh - 8.5rem)")
            .Style("min-width", "920px")
            .Style("max-width", "1700px").Style("margin", "0 auto")
            .Style("border", $"1px solid {Cyan}44")
            .Style("background", "radial-gradient(circle at 50% 44%, rgba(0,234,255,.09), transparent 42%), linear-gradient(180deg,#07131d,#02060c)")
            .Style("box-shadow", $"0 0 120px {Cyan}12,inset 0 0 100px {Cyan}08")
            .Style("overflow", "hidden");

        shell.Content(Roof(receiver));
        shell.Content(GrandHall(receiver));
        shell.Content(TrackField(receiver, manifest));
        shell.Content(Platforms(receiver, manifest, selectedDestinationId, trainVisible, selectPlatform, boardTrain));
        shell.Content(VerticalWalkways(receiver, manifest));
        shell.Content(StationLabels(receiver, manifest));

        station.Content(shell);
        return station;
    }

    private static ElementBuilder Roof(object receiver)
        => WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 100")
            .Attribute("preserveAspectRatio", "none")
            .Style("position", "absolute").Style("inset", "0")
            .Style("width", "100%").Style("height", "100%")
            .Style("pointer-events", "none")
            .Content(RoofLines(receiver));

    private static ElementBuilder RoofLines(object receiver)
    {
        var group = WorkshopGui.Element(receiver, "g");
        for (var i = 0; i < 11; i++)
        {
            var x = 4 + i * 9.2;
            group.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", x).Attribute("y1", "3")
                .Attribute("x2", "50").Attribute("y2", "48")
                .Attribute("stroke", Cyan).Attribute("stroke-opacity", ".10")
                .Attribute("stroke-width", ".16"));
        }

        for (var y = 8; y <= 92; y += 8)
            group.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", "2").Attribute("y1", y)
                .Attribute("x2", "98").Attribute("y2", y)
                .Attribute("stroke", White).Attribute("stroke-opacity", ".035")
                .Attribute("stroke-width", ".14"));

        return group;
    }

    private static ElementBuilder GrandHall(object receiver)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "8%").Style("right", "8%")
            .Style("top", "7%").Style("height", "29%")
            .Style("border", $"1px solid {White}20")
            .Style("background", "linear-gradient(180deg,rgba(255,255,255,.028),rgba(0,234,255,.018))")
            .Style("box-shadow", $"inset 0 0 45px {Cyan}08")
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("position", "absolute").Style("inset", "0")
                .Style("display", "flex").Style("flex-direction", "column")
                .Style("align-items", "center").Style("justify-content", "center")
                .Style("pointer-events", "none")
                .Content(WorkshopGui.Element(receiver, "div")
                    .Style("font-size", "clamp(1.3rem,4vw,3.4rem)")
                    .Style("letter-spacing", ".38em").Style("color", White)
                    .Style("opacity", ".82").Text("GRAND CENTRAL"))
                .Content(WorkshopGui.Element(receiver, "div")
                    .Style("margin-top", ".55rem").Style("font-size", ".5rem")
                    .Style("letter-spacing", ".32em").Style("color", Cyan)
                    .Text("ARRIVALS // DEPARTURES // CONNECTIONS // PEOPLE IN MOTION"))
                .Content(WorkshopGui.Element(receiver, "div")
                    .Style("margin-top", "1rem").Style("font-size", ".42rem")
                    .Style("letter-spacing", ".18em").Style("color", Yellow)
                    .Text("THE STATION IS A LIVING EXPERIENCE — NOT A MENU")));

    private static ElementBuilder TrackField(object receiver, SpatialTransitManifest manifest)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 100")
            .Attribute("preserveAspectRatio", "none")
            .Style("position", "absolute").Style("inset", "0")
            .Style("width", "100%").Style("height", "100%")
            .Style("pointer-events", "none");

        foreach (var level in manifest.Levels)
        {
            var y = level.Number == 1 ? 58 : 78;
            svg.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", "4").Attribute("y1", y)
                .Attribute("x2", "96").Attribute("y2", y)
                .Attribute("stroke", level.Number == 1 ? Cyan : Magenta)
                .Attribute("stroke-opacity", ".22")
                .Attribute("stroke-width", ".35"));

            for (var x = 8; x <= 92; x += 8)
                svg.Child(WorkshopGui.Element(receiver, "line")
                    .Attribute("x1", x).Attribute("y1", y - 2)
                    .Attribute("x2", x).Attribute("y2", y + 2)
                    .Attribute("stroke", White).Attribute("stroke-opacity", ".16")
                    .Attribute("stroke-width", ".18"));
        }

        return svg;
    }

    private static ElementBuilder Platforms(
        object receiver,
        SpatialTransitManifest manifest,
        string? selectedDestinationId,
        bool trainVisible,
        Action<string> selectPlatform,
        Action<string> boardTrain)
    {
        var area = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "5%").Style("right", "5%")
            .Style("top", "45%").Style("bottom", "5%");

        foreach (var platform in manifest.Platforms)
        {
            var destination = manifest.Destinations.FirstOrDefault(x => x.Id == platform.DestinationId);
            var selected = destination.Id == selectedDestinationId;
            var left = platform.X;
            var top = platform.Y;

            var card = WorkshopGui.Panel(receiver)
                .Style("position", "absolute")
                .Style("left", $"{left}%").Style("top", $"{top}%")
                .Style("width", $"{platform.Width}%").Style("height", "15%")
                .Style("box-sizing", "border-box")
                .Style("border", $"1px solid {(selected ? Yellow : platform.Level == 1 ? Cyan : Magenta)}{(selected ? "cc" : "55")}")
                .Style("background", selected ? $"{Yellow}12" : "rgba(2,8,14,.88)")
                .Style("box-shadow", selected ? $"0 0 28px {Yellow}18,inset 0 0 22px {Yellow}08" : "none")
                .Style("overflow", "hidden");

            card.Content(WorkshopGui.Element(receiver, "div")
                .Style("font-size", ".42rem").Style("letter-spacing", ".14em")
                .Style("color", selected ? Yellow : platform.Level == 1 ? Cyan : Magenta)
                .Text($"PLATFORM {PlatformNumber(manifest, platform.Id):00} // LEVEL {platform.Level} // TRACK {platform.TrackIndex + 1:00}"));

            card.Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".38rem").Style("font-size", ".62rem")
                .Style("font-weight", "700").Style("color", White)
                .Text(destination.Label));

            card.Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".22rem").Style("font-size", ".38rem")
                .Style("color", Dim)
                .Text("WALKWAY CONNECTION // BOARDING EDGE"));

            card.Content(WorkshopGui.Button(receiver)
                .Label(selected ? "REQUEST TRAIN" : "GO TO PLATFORM")
                .Style("position", "absolute").Style("left", ".5rem").Style("right", ".5rem")
                .Style("bottom", ".45rem").Style("padding", ".34rem .45rem")
                .Style("border", $"1px solid {(selected ? Yellow : Cyan)}66")
                .Style("background", "rgba(1,4,10,.92)")
                .Style("color", selected ? Yellow : White)
                .Style("font-family", "inherit").Style("font-size", ".38rem")
                .Style("letter-spacing", ".08em").Style("cursor", "pointer")
                .OnClick(() => selectPlatform(destination.Id)));

            if (selected && trainVisible)
                card.Content(Train(receiver, destination, () => boardTrain(destination.Id)));

            area.Content(card);
        }

        return area;
    }

    private static ElementBuilder Train(object receiver, SpatialTransitDestination destination, Action board)
        => WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("left", "4%").Style("right", "4%")
            .Style("top", "-5%").Style("height", "48%")
            .Style("z-index", "5")
            .Style("border", $"1px solid {Magenta}cc")
            .Style("background", "linear-gradient(90deg,rgba(255,56,209,.10),rgba(0,234,255,.16),rgba(255,56,209,.10))")
            .Style("box-shadow", $"0 0 35px {Magenta}20")
            .Style("animation", "workshop-train-arrive .8s ease-out both")
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("font-size", ".36rem").Style("letter-spacing", ".12em").Style("color", Yellow)
                .Text($"ARRIVAL // {destination.Label}"))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("position", "absolute").Style("left", ".5rem").Style("right", ".5rem")
                .Style("bottom", ".4rem").Style("display", "flex")
                .Style("justify-content", "space-between").Style("align-items", "center")
                .Content(WorkshopGui.Element(receiver, "span")
                    .Style("font-size", ".34rem").Style("color", White)
                    .Text("DOORS OPEN // BOARDING FLOW ACTIVE"))
                .Content(WorkshopGui.Button(receiver).Label("BOARD")
                    .Style("padding", ".28rem .42rem").Style("border", $"1px solid {Green}88")
                    .Style("background", $"{Green}10").Style("color", Green)
                    .Style("font-family", "inherit").Style("font-size", ".34rem")
                    .Style("cursor", "pointer").OnClick(board)));

    private static ElementBuilder VerticalWalkways(object receiver, SpatialTransitManifest manifest)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 100")
            .Attribute("preserveAspectRatio", "none")
            .Style("position", "absolute").Style("inset", "0")
            .Style("width", "100%").Style("height", "100%")
            .Style("pointer-events", "none");

        foreach (var platform in manifest.Platforms)
        {
            var centerX = platform.X + platform.Width / 2;
            var concourseY = platform.Level == 1 ? 44 : 44;
            var platformY = platform.Y;

            svg.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", centerX).Attribute("y1", concourseY)
                .Attribute("x2", centerX).Attribute("y2", platformY)
                .Attribute("stroke", platform.Level == 1 ? Cyan : Magenta)
                .Attribute("stroke-opacity", ".32")
                .Attribute("stroke-width", ".22")
                .Attribute("stroke-dasharray", "1 1"));
        }

        return svg;
    }

    private static ElementBuilder StationLabels(object receiver, SpatialTransitManifest manifest)
    {
        var panel = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "1.5%").Style("right", "1.5%")
            .Style("bottom", "1.2%").Style("display", "flex")
            .Style("justify-content", "space-between").Style("pointer-events", "none");

        foreach (var level in manifest.Levels)
            panel.Content(WorkshopGui.Element(receiver, "div")
                .Style("font-size", ".38rem").Style("letter-spacing", ".13em")
                .Style("color", level.Number == 1 ? Cyan : Magenta)
                .Text($"LEVEL {level.Number:00} // {level.Name}"));

        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("font-size", ".36rem").Style("letter-spacing", ".1em").Style("color", Dim)
            .Text("EXPANDABLE PLATFORM ARCHITECTURE // WALKWAYS ABOVE TRACKS // VERTICAL STACKING"));

        return panel;
    }

    private static ElementBuilder PassengerFlow(object receiver, SpatialTransitManifest manifest)
    {
        var panel = WorkshopGui.Element(receiver, "div")
            .Style("position", "fixed").Style("left", "1.4rem").Style("bottom", "4.1rem")
            .Style("z-index", "25").Style("padding", ".55rem .7rem")
            .Style("border", $"1px solid {Cyan}44")
            .Style("background", "rgba(1,4,10,.9)")
            .Style("pointer-events", "none");

        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("font-size", ".4rem").Style("letter-spacing", ".15em").Style("color", Cyan)
            .Text($"DIGITEN FLOW // {manifest.Passengers.Count:00} IN MOTION"));

        foreach (var passenger in manifest.Passengers.Take(8))
            panel.Content(WorkshopGui.Element(receiver, "div")
                .Style("display", "inline-block").Style("margin", ".35rem .3rem 0 0")
                .Style("width", "8px").Style("height", "8px").Style("border-radius", "50%")
                .Style("background", passenger.BoardingOrder % 2 == 0 ? Cyan : Yellow)
                .Style("box-shadow", $"0 0 9px {(passenger.BoardingOrder % 2 == 0 ? Cyan : Yellow)}")
                .Attribute("title", passenger.Id));

        return panel;
    }

    private static ElementBuilder OperationsKiosk(
        object receiver,
        bool active,
        int score,
        int round,
        string? targetId,
        SpatialTransitManifest manifest,
        Action start,
        Action<string> selectPlatform)
    {
        var target = manifest.Destinations.FirstOrDefault(x => x.Id == targetId);
        var targetLabel = targetId is null ? "STATION OBSERVER" : target.Label;

        var kiosk = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("right", "1.4rem").Style("top", "5.2rem")
            .Style("z-index", "35").Style("width", "245px")
            .Style("padding", ".7rem").Style("border", $"1px solid {Yellow}55")
            .Style("background", "rgba(1,4,10,.93)")
            .Style("box-shadow", $"0 0 25px {Yellow}08");

        kiosk.Content(WorkshopGui.Element(receiver, "div")
            .Style("font-size", ".42rem").Style("letter-spacing", ".16em").Style("color", Yellow)
            .Text("STATION OPERATIONS"));

        if (!active)
        {
            kiosk.Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".45rem").Style("font-size", ".43rem").Style("line-height", "1.5").Style("color", Dim)
                .Text("Something small is hidden in the station. Can you route Digitens to their platforms before the next departure?"));

            kiosk.Content(WorkshopGui.Button(receiver).Label("OPEN THE OPERATIONS CHALLENGE")
                .Style("margin-top", ".6rem").Style("width", "100%").Style("padding", ".45rem")
                .Style("border", $"1px solid {Yellow}88").Style("background", $"{Yellow}0c")
                .Style("color", Yellow).Style("font-family", "inherit").Style("font-size", ".38rem")
                .Style("letter-spacing", ".08em").Style("cursor", "pointer").OnClick(start));

            return kiosk;
        }

        kiosk.Content(WorkshopGui.Element(receiver, "div")
            .Style("margin-top", ".45rem").Style("font-size", ".4rem").Style("color", Green)
            .Text($"ROUND {round:00} // SCORE {score:000}"));

        kiosk.Content(WorkshopGui.Element(receiver, "div")
            .Style("margin-top", ".45rem").Style("font-size", ".43rem").Style("line-height", "1.5").Style("color", White)
            .Text($"ROUTE THIS PASSENGER TO: {targetLabel}"));

        kiosk.Content(WorkshopGui.Element(receiver, "div")
            .Style("margin-top", ".45rem").Style("font-size", ".34rem").Style("line-height", "1.45").Style("color", Dim)
            .Text("Click the matching physical platform. A correct route earns 10 points; a wrong platform costs 2."));

        foreach (var platform in manifest.Platforms)
        {
            var destination = manifest.Destinations.FirstOrDefault(x => x.Id == platform.DestinationId);
            kiosk.Content(WorkshopGui.Button(receiver).Label(destination.Label)
                .Style("display", "block").Style("width", "100%").Style("margin-top", ".25rem")
                .Style("padding", ".28rem .35rem").Style("border", $"1px solid {Cyan}33")
                .Style("background", "rgba(0,234,255,.025)").Style("color", White)
                .Style("font-family", "inherit").Style("font-size", ".34rem")
                .Style("text-align", "left").Style("cursor", "pointer")
                .OnClick(() => selectPlatform(platform.Id)));
        }

        return kiosk;
    }

    private static ElementBuilder ExitButton(object receiver, Action exit)
        => WorkshopGui.Button(receiver).Label("← RETURN TO GARDEN")
            .Style("position", "fixed").Style("left", "1.4rem").Style("bottom", "1rem")
            .Style("z-index", "40").Style("padding", ".5rem .7rem")
            .Style("border", $"1px solid {Magenta}66").Style("background", "rgba(20,4,18,.9)")
            .Style("color", Magenta).Style("font-family", "inherit")
            .Style("font-size", ".42rem").Style("letter-spacing", ".1em").Style("cursor", "pointer")
            .OnClick(exit);

    private static int PlatformNumber(SpatialTransitManifest manifest, string id)
    {
        for (var i = 0; i < manifest.Platforms.Count; i++)
            if (manifest.Platforms[i].Id == id) return i + 1;
        return 0;
    }

    private static ElementBuilder Styles(object receiver)
        => WorkshopGui.Element(receiver, "style").Text(
            "@keyframes workshop-train-arrive{0%{transform:translateX(45%);opacity:0}100%{transform:translateX(0);opacity:1}}" +
            "@keyframes workshop-passenger-breathe{0%,100%{opacity:.55}50%{opacity:1}}" +
            "@keyframes workshop-platform-breathe{0%,100%{opacity:.7}50%{opacity:1}}");
}
