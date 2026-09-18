using Microsoft.AspNetCore.Components.Web;
using TheSingularityWorkshop.Workshop.MicroBundles;

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
        Func<string, Task> selectPlatform,
        Action<string> boardTrain,
        Action exit,
        bool microGameActive,
        int microGameScore,
        int microGameRound,
        string? microGameTargetId,
        Action startMicroGame,
        Action<string> selectMicroGamePlatform,
        double avatarX,
        double avatarY,
        Action<KeyboardEventArgs> onKeyDown,
        Func<MouseEventArgs, Task> onWorldClick,
        TransitTrainSpecification trainSpecification)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0")
            .Style("overflow", "hidden")
            .Style("background", "#01040a")
            .Style("color", White)
            .Style("font-family", "Consolas, 'Courier New', monospace");

        root.Content(Styles(receiver));
        root.Content(Header(receiver, userName, manifest));
        root.Content(Station(receiver, manifest, selectedDestinationId, trainVisible, selectPlatform, boardTrain, avatarX, avatarY, onKeyDown, onWorldClick, trainSpecification));
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
        Func<string, Task> selectPlatform,
        Action<string> boardTrain,
        double avatarX,
        double avatarY,
        Action<KeyboardEventArgs> onKeyDown,
        Func<MouseEventArgs, Task> onWorldClick,
        TransitTrainSpecification trainSpecification)
    {
        var station = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("inset", "0")
            .Style("width", "100vw").Style("height", "100vh")
            .Style("box-sizing", "border-box").Style("overflow", "hidden")
            .Attribute("tabindex", "0")
            .AriaLabel("Singularity Grand Central. Click to walk. Use WASD or arrow keys.")
            .OnKeyDown(onKeyDown)
            .PreventDefault("onkeydown");

        var shell = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("inset", "0")
            .Style("width", "100%").Style("height", "100%")
            .Style("border", $"1px solid {Cyan}44")
            .Style("background", "radial-gradient(circle at 50% 44%, rgba(0,234,255,.09), transparent 42%), linear-gradient(180deg,#07131d,#02060c)")
            .Style("box-shadow", $"0 0 120px {Cyan}12,inset 0 0 100px {Cyan}08")
            .Style("overflow", "hidden");

        var worldPlane = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "0").Style("top", "0")
            .Style("width", "100%").Style("height", "100%")
            .Style("transform", $"translate({50d - avatarX:0.###}%, {50d - avatarY:0.###}%)")
            .Style("transition", "transform .18s ease-out")
            .Style("transform-origin", "50% 50%");

        worldPlane.Content(Roof(receiver));
        worldPlane.Content(FloorPlan(receiver, manifest));
        worldPlane.Content(GrandHall(receiver));
        worldPlane.Content(TrackField(receiver, manifest));
        worldPlane.Content(TrainTraffic(receiver, trainSpecification));
        worldPlane.Content(Platforms(receiver, manifest, selectedDestinationId, trainVisible, selectPlatform, boardTrain, trainSpecification));
        worldPlane.Content(VerticalWalkways(receiver, manifest));
        worldPlane.Content(StationLabels(receiver, manifest));
        worldPlane.Content(ArchitecturalSymbols(receiver));
        worldPlane.Content(PassengerActors(receiver, manifest));
        worldPlane.Content(StaffActors(receiver));
        worldPlane.Content(WalkSurface(receiver, onWorldClick));
        shell.Content(worldPlane);
        shell.Content(Avatar(receiver));
        shell.Content(StationFocus(receiver, onKeyDown));

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

    /// <summary>
    /// Draws the station as a deliberately crude architectural floor plan first: a massive
    /// outer envelope, public concourse, parallel rail corridors, vertical circulation cores,
    /// and long boarding platforms. Detail can be added later without losing the building.
    /// </summary>
    private static ElementBuilder FloorPlan(object receiver, SpatialTransitManifest manifest)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 180 112")
            .Attribute("preserveAspectRatio", "none")
            .Style("position", "absolute").Style("inset", "0")
            .Style("width", "100%").Style("height", "100%")
            .Style("pointer-events", "none")
            .Style("opacity", ".9");

        // Massive exterior envelope: this is the building, not a collection of cards.
        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", "3").Attribute("y", "3")
            .Attribute("width", "174").Attribute("height", "106")
            .Attribute("fill", "rgba(0,234,255,.025)").Attribute("stroke", Cyan)
            .Attribute("stroke-opacity", ".48").Attribute("stroke-width", ".45"));
        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", "5").Attribute("y", "5")
            .Attribute("width", "170").Attribute("height", "102")
            .Attribute("fill", "rgba(255,255,255,.012)").Attribute("stroke", White)
            .Attribute("stroke-opacity", ".13").Attribute("stroke-width", ".2"));

        // Main public concourse: deliberately oversized so the station has a civic interior.
        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", "11").Attribute("y", "10")
            .Attribute("width", "158").Attribute("height", "28")
            .Attribute("fill", "none").Attribute("stroke", White)
            .Attribute("stroke-opacity", ".28").Attribute("stroke-width", ".3"));
        svg.Child(WorkshopGui.Element(receiver, "line")
            .Attribute("x1", "11").Attribute("y1", "32")
            .Attribute("x2", "169").Attribute("y2", "32")
            .Attribute("stroke", Cyan).Attribute("stroke-opacity", ".16").Attribute("stroke-width", ".2"));

        // A central pedestrian spine connects the public hall to every boarding zone.
        svg.Child(WorkshopGui.Element(receiver, "line")
            .Attribute("x1", "90").Attribute("y1", "10")
            .Attribute("x2", "90").Attribute("y2", "107")
            .Attribute("stroke", Yellow).Attribute("stroke-opacity", ".22").Attribute("stroke-width", ".35"));
        svg.Child(WorkshopGui.Element(receiver, "line")
            .Attribute("x1", "84").Attribute("y1", "10")
            .Attribute("x2", "84").Attribute("y2", "107")
            .Attribute("stroke", White).Attribute("stroke-opacity", ".08").Attribute("stroke-width", ".18"));
        svg.Child(WorkshopGui.Element(receiver, "line")
            .Attribute("x1", "96").Attribute("y1", "10")
            .Attribute("x2", "96").Attribute("y2", "107")
            .Attribute("stroke", White).Attribute("stroke-opacity", ".08").Attribute("stroke-width", ".18"));

        // Rail corridors: several parallel tracks, with platforms between/along them.
        for (var y = 45; y <= 103; y += 4.5)
        {
            svg.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", "8").Attribute("y1", y)
                .Attribute("x2", "172").Attribute("y2", y)
                .Attribute("stroke", Cyan).Attribute("stroke-opacity", ".20").Attribute("stroke-width", ".34"));
            svg.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", "8").Attribute("y1", y + 1.7)
                .Attribute("x2", "172").Attribute("y2", y + 1.7)
                .Attribute("stroke", White).Attribute("stroke-opacity", ".07").Attribute("stroke-width", ".16"));
        }

        // Vertical circulation cores: stairs/lifts/escalators to the stacked concourses.
        foreach (var x in new[] { 20, 55, 90, 125, 160 })
        {
            svg.Child(WorkshopGui.Element(receiver, "rect")
                .Attribute("x", x - 2.2).Attribute("y", "34")
                .Attribute("width", "4.4").Attribute("height", "10")
                .Attribute("fill", "none").Attribute("stroke", Magenta)
                .Attribute("stroke-opacity", ".42").Attribute("stroke-width", ".25"));
            svg.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", x).Attribute("y1", "44")
                .Attribute("x2", x).Attribute("y2", "105")
                .Attribute("stroke", Magenta).Attribute("stroke-opacity", ".15").Attribute("stroke-width", ".22")
                .Attribute("stroke-dasharray", "1.5 1.5"));
        }

        // Platform footprints come from the manifest; the floor plan remains data-driven.
        foreach (var platform in manifest.Platforms)
        {
            var x = 8 + platform.X * 1.64;
            var y = platform.Level == 1 ? 44 + platform.Y * .45 : 47 + platform.Y * .55;
            var width = Math.Max(18, platform.Width * 2.1);
            svg.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", x).Attribute("y1", y)
                .Attribute("x2", Math.Min(172, x + width)).Attribute("y2", y)
                .Attribute("stroke", platform.Level == 1 ? Cyan : Magenta)
                .Attribute("stroke-opacity", ".42").Attribute("stroke-width", ".9"));
        }

        // Main entrance / arrival plaza.
        svg.Child(WorkshopGui.Element(receiver, "line")
            .Attribute("x1", "70").Attribute("y1", "109")
            .Attribute("x2", "110").Attribute("y2", "109")
            .Attribute("stroke", Yellow).Attribute("stroke-opacity", ".65").Attribute("stroke-width", "1.2"));
        svg.Child(WorkshopGui.Element(receiver, "line")
            .Attribute("x1", "78").Attribute("y1", "107").Attribute("x2", "102").Attribute("y2", "107")
            .Attribute("stroke", Yellow).Attribute("stroke-opacity", ".25").Attribute("stroke-width", ".35"));

        return svg;
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

    private static ElementBuilder ArchitecturalSymbols(object receiver)
    {
        var layer = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("inset", "0")
            .Style("pointer-events", "none").Style("color", White);

        layer.Content(Symbol(receiver, 12, 12, 20, 6, "VANDERBILT HALL", Cyan));
        layer.Content(Symbol(receiver, 35, 12, 22, 6, "TICKET COUNTERS", Yellow));
        layer.Content(Symbol(receiver, 60, 12, 18, 6, "INFORMATION", Yellow));
        layer.Content(Symbol(receiver, 80, 12, 18, 6, "WAITING / MEET", Cyan));
        layer.Content(Symbol(receiver, 82, 30, 8, 5, "CLOCK", Yellow));
        layer.Content(Symbol(receiver, 11, 39, 15, 7, "BILTMORE", Magenta));
        layer.Content(Symbol(receiver, 74, 39, 15, 7, "DINING", Green));
        layer.Content(Symbol(receiver, 90, 39, 8, 7, "STAFF", Green));
        layer.Content(Symbol(receiver, 2, 48, 8, 45, "NORTH / SERVICE", Dim));
        layer.Content(Symbol(receiver, 90, 48, 8, 45, "SOUTH / SERVICE", Dim));
        return layer;
    }

    private static ElementBuilder Symbol(object receiver, double left, double top, double width, double height, string label, string accent)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", $"{left}%").Style("top", $"{top}%")
            .Style("width", $"{width}%").Style("height", $"{height}%")
            .Style("box-sizing", "border-box")
            .Style("border", $"1px solid {accent}55")
            .Style("background", $"{accent}08")
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("position", "absolute").Style("left", ".3rem").Style("top", ".25rem")
                .Style("font-size", ".34rem").Style("letter-spacing", ".11em")
                .Style("color", accent).Text(label));

    private static ElementBuilder WalkSurface(object receiver, double avatarX, double avatarY, Func<MouseEventArgs, Task> onWorldClick)
    {
        const int cells = 18;
        var surface = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("inset", "0").Style("z-index", "2")
            .Style("display", "grid")
            .Style("grid-template-columns", $"repeat({cells},1fr)")
            .Style("grid-template-rows", $"repeat({cells},1fr)")
            .Style("pointer-events", "none");

        for (var row = 0; row < cells; row++)
        for (var column = 0; column < cells; column++)
        {
            var x = (column + .5) / cells * 100;
            var y = (row + .5) / cells * 100;
            surface.Content(WorkshopGui.Button(receiver)
                .Style("width", "100%").Style("height", "100%").Style("padding", "0")
                .Style("border", "0").Style("background", "transparent")
                .Style("pointer-events", "auto").Style("cursor", "crosshair")
                .AriaLabel($"Walk to {x:0.#}, {y:0.#}")
                .OnClick(() => moveAvatarTo(Math.Clamp(x, 3, 97), Math.Clamp(y, 5, 95))));
        }
        return surface;
    }

    private static ElementBuilder StationFocus(object receiver, Action<KeyboardEventArgs> onKeyDown)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("inset", "0").Style("z-index", "17")
            .Style("pointer-events", "none")
            .Attribute("tabindex", "0")
            .OnKeyDown(onKeyDown);

    private static ElementBuilder Avatar(object receiver)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "fixed").Style("left", "50%").Style("top", "50%")
            .Style("transform", "translate(-50%,-50%)").Style("z-index", "22")
            .Style("width", "18px").Style("height", "18px").Style("border-radius", "50%")
            .Style("border", $"2px solid {White}").Style("background", "#01040a")
            .Style("box-shadow", $"0 0 18px {White}88")
            .Attribute("title", "YOUR POSITION");

    private static ElementBuilder PassengerActors(object receiver, SpatialTransitManifest manifest)
    {
        var layer = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("inset", "0").Style("z-index", "12")
            .Style("pointer-events", "none");
        var index = 0;
        foreach (var passenger in manifest.Passengers)
        {
            var left = Math.Clamp(passenger.WaitingPosition.X, 5, 95);
            var top = Math.Clamp(passenger.WaitingPosition.Y, 5, 95);
            var duration = 7 + (index % 5);
            layer.Content(WorkshopGui.Element(receiver, "div")
                .Style("position", "absolute").Style("left", $"{left}%").Style("top", $"{top}%")
                .Style("width", "7px").Style("height", "7px").Style("border-radius", "50%")
                .Style("background", index % 2 == 0 ? Cyan : Yellow)
                .Style("box-shadow", $"0 0 10px {(index % 2 == 0 ? Cyan : Yellow)}")
                .Style("animation", $"workshop-digiten-flow-{index % 4} {duration}s ease-in-out infinite")
                .Attribute("title", passenger.Id));
            index++;
        }
        return layer;
    }

    private static ElementBuilder StaffActors(object receiver)
    {
        var layer = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("inset", "0").Style("z-index", "13")
            .Style("pointer-events", "none");
        var staff = new[]
        {
            ("STATION MASTER", 91d, 15d), ("TICKET AGENT", 46d, 15d),
            ("INFORMATION", 68d, 15d), ("CONDUCTOR", 32d, 56d),
            ("CONDUCTOR", 63d, 74d), ("SECURITY", 12d, 43d),
            ("CLEANING", 84d, 43d), ("ENGINEER", 50d, 91d)
        };
        foreach (var person in staff)
            layer.Content(WorkshopGui.Element(receiver, "div")
                .Style("position", "absolute").Style("left", $"{person.Item2}%").Style("top", $"{person.Item3}%")
                .Style("padding", ".12rem .2rem").Style("border", $"1px solid {Green}55")
                .Style("background", "rgba(1,4,10,.7)").Style("color", Green)
                .Style("font-size", ".3rem").Style("letter-spacing", ".06em")
                .Text("● " + person.Item1));
        return layer;
    }

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

    private static ElementBuilder TrainTraffic(object receiver, TransitTrainSpecification specification)
    {
        var layer = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("inset", "0").Style("z-index", "6")
            .Style("pointer-events", "none");

        var lanes = new[] { 55d, 66d, 78d, 89d };
        for (var i = 0; i < lanes.Length; i++)
        {
            var direction = i % 2 == 0 ? "normal" : "reverse";
            layer.Content(WorkshopGui.Element(receiver, "div")
                .Style("position", "absolute").Style("left", "-18%").Style("top", $"{lanes[i]}%")
                .Style("width", $"{Math.Clamp(specification.CarCount * 1.4, 8, 16)}%")
                .Style("height", "1.2rem")
                .Style("border", $"1px solid {(i % 2 == 0 ? Cyan : Magenta)}99")
                .Style("background", "rgba(4,12,18,.88)")
                .Style("box-shadow", $"0 0 14px {(i % 2 == 0 ? Cyan : Magenta)}22")
                .Style("animation", $"workshop-train-traffic-{direction} {14 + i * 2}s linear infinite")
                .Content(WorkshopGui.Element(receiver, "span")
                    .Style("display", "block").Style("padding", ".25rem .35rem")
                    .Style("font-size", ".3rem").Style("letter-spacing", ".08em")
                    .Style("color", i % 2 == 0 ? Cyan : Magenta)
                    .Text($"TRAIN {i + 1:00} // {specification.CarCount}-CAR")));
        }
        return layer;
    }

    private static ElementBuilder Platforms(
        object receiver,
        SpatialTransitManifest manifest,
        string? selectedDestinationId,
        bool trainVisible,
        Func<string, Task> selectPlatform,
        Action<string> boardTrain,
        TransitTrainSpecification trainSpecification)
    {
        var area = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("inset", "0")
            .Style("pointer-events", "none");

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
                .Style("box-sizing", "border-box").Style("z-index", "8").Style("cursor", "pointer").Style("pointer-events", "auto")
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

            card.OnClick(() => selectPlatform(platform.Id)).StopPropagation("onclick");

            if (selected && trainVisible)
                card.Content(Train(receiver, destination, trainSpecification, () => boardTrain(destination.Id)));

            area.Content(card);
        }

        return area;
    }

    private static ElementBuilder Train(object receiver, SpatialTransitDestination destination, TransitTrainSpecification specification, Action board)
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
                .Style("margin-top", ".3rem").Style("font-size", ".32rem").Style("color", Dim)
                .Text($"HO SANDBOX // {specification.CarCount} CARS // 1:{TransitTrainSpecification.HoScaleRatio:0.0} // MODEL CAR {specification.ModelCarLengthMillimeters:0.0} MM"))
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
            .Text("SCHEMATIC REFERENCE // 44 PLATFORM CAPACITY // 67 TRACK INFRASTRUCTURE // WALKWAYS ABOVE TRACKS // VERTICAL STACKING"));

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
        Func<string, Task> selectPlatform)
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
            "@keyframes workshop-platform-breathe{0%,100%{opacity:.7}50%{opacity:1}}" +
            "@keyframes workshop-digiten-flow-0{0%,100%{transform:translate(0,0)}50%{transform:translate(8vw,-1.5vh)}}" +
            "@keyframes workshop-digiten-flow-1{0%,100%{transform:translate(0,0)}50%{transform:translate(-7vw,2vh)}}" +
            "@keyframes workshop-digiten-flow-2{0%,100%{transform:translate(0,0)}50%{transform:translate(4vw,3vh)}}" +
            "@keyframes workshop-digiten-flow-3{0%,100%{transform:translate(0,0)}50%{transform:translate(-5vw,-2vh)}}" +
            "@keyframes workshop-train-traffic-normal{0%{transform:translateX(0)}100%{transform:translateX(850%)}}" +
            "@keyframes workshop-train-traffic-reverse{0%{transform:translateX(850%)}100%{transform:translateX(0)}}");
}
