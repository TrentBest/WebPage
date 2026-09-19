namespace TheSingularityWorkshop.Gui;

public static partial class SpatialWorkshopMapGuiBuilder
{
    public static ElementBuilder Build(
        object receiver,
        SpatialWorkshopScene scene,
        double avatarX,
        double avatarY,
        string moniker,
        Func<string, Task> selectDestination,
        Action close,
        SpatialMapScope scope = SpatialMapScope.Workshop,
        double zoom = 1d,
        Action<double>? setZoom = null,
        Action<SpatialMapScope>? setScope = null)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0").Style("z-index", "200")
            .Style("display", "flex").Style("align-items", "center").Style("justify-content", "center")
            .Style("padding", "1rem").Style("box-sizing", "border-box")
            .Style("background", "rgba(1,4,10,.96)").Style("color", White)
            .Style("font-family", "Consolas, 'Courier New', monospace");

        root.Content(WorkshopGui.Element(receiver, "style").Text(
            "@keyframes workshop-map-pulse{0%,100%{opacity:.62;transform:scale(1)}50%{opacity:1;transform:scale(1.15)}}"));

        var panel = WorkshopGui.Panel(receiver)
            .Style("width", "min(1280px,98vw)").Style("height", "min(92vh,900px)")
            .Style("display", "flex").Style("flex-direction", "column")
            .Style("box-sizing", "border-box")
            .Style("border", $"{Cyan}99 solid 1px")
            .Style("background", "rgba(2,9,18,.99)")
            .Style("box-shadow", $"0 0 90px {Cyan}18,inset 0 0 50px {Cyan}08")
            .Style("padding", "1rem");

        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("display", "flex").Style("justify-content", "space-between").Style("align-items", "center")
            .Style("gap", "1rem").Style("flex-wrap", "wrap")
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("color", Cyan).Style("font-size", "clamp(.8rem,2vw,1.25rem)")
                .Style("letter-spacing", ".22em").Text(moniker.ToUpperInvariant()))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("color", Yellow).Style("font-size", ".5rem").Style("letter-spacing", ".18em")
                .Text(scope == SpatialMapScope.Workshop ? "WORKSHOP CAMPUS // COMPLETE MAP" : "SINGULARITY CITY // DEVELOPMENT MAP")));

        var scopeBar = WorkshopGui.Element(receiver, "div")
            .Style("display", "flex").Style("gap", ".35rem").Style("flex-wrap", "wrap")
            .Style("align-items", "center").Style("margin", ".65rem 0")
            .Content(ScopeButton(receiver, "WORKSHOP", SpatialMapScope.Workshop, scope, setScope))
            .Content(ScopeButton(receiver, "SINGULARITY CITY", SpatialMapScope.City, scope, setScope))
            .Content(ScopeButton(receiver, "WORLD // UNDER CONSTRUCTION", SpatialMapScope.World, scope, setScope))
            .Content(WorkshopGui.Element(receiver, "span").Style("margin-left", "auto").Style("color", "#8fa7b2").Style("font-size", ".43rem")
                .Text("SCROLL THE MAP LIST • SCALE WITH + / −"));

        panel.Content(scopeBar);

        var layout = WorkshopGui.Element(receiver, "div")
            .Style("display", "grid")
            .Style("grid-template-columns", "minmax(0,1fr) minmax(260px,330px)")
            .Style("gap", ".8rem").Style("min-height", "0").Style("flex", "1");

        layout.Content(scope == SpatialMapScope.Workshop
            ? Map(receiver, scene, avatarX, avatarY, selectDestination, zoom, setZoom)
            : scope == SpatialMapScope.City
                ? CityMap(receiver, avatarX, avatarY, selectDestination, zoom, setZoom)
                : WorldMap(receiver, avatarX, avatarY, zoom, setZoom));

        layout.Content(scope == SpatialMapScope.Workshop
            ? Directory(receiver, scene, selectDestination)
            : scope == SpatialMapScope.City
                ? CityDirectory(receiver, selectDestination)
                : WorldDirectory(receiver));

        panel.Content(layout);
        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("display", "flex").Style("justify-content", "space-between").Style("align-items", "center")
            .Style("gap", "1rem").Style("margin-top", ".7rem")
            .Content(WorkshopGui.Element(receiver, "span").Style("color", "#a8bbc4").Style("font-size", ".46rem")
                .Text(scope == SpatialMapScope.Workshop
                    ? "THE MAP SHOWS BUILDING FOOTPRINTS AND ONLY ONE MARKER: YOU ARE HERE."
                    : "UNDER CONSTRUCTION: APPROXIMATE MASSING ONLY. PROXIMITY WILL EXPLAIN EACH FUTURE FUNCTION."))
            .Content(WorkshopGui.Button(receiver).Label("CLOSE MAP")
                .Style("padding", ".45rem .7rem").Style("border", $"1px solid {Magenta}66")
                .Style("background", "rgba(20,4,18,.8)").Style("color", Magenta)
                .Style("font-family", "inherit").Style("font-size", ".45rem").Style("cursor", "pointer").OnClick(close)));

        root.Content(panel);
        return root;
    }

    private static ElementBuilder ScopeButton(object receiver, string label, SpatialMapScope value, SpatialMapScope current, Action<SpatialMapScope>? setScope)
        => WorkshopGui.Button(receiver).Label(label)
            .Style("padding", ".4rem .55rem").Style("border", $"1px solid {(value == current ? Yellow : Cyan)}88")
            .Style("background", value == current ? $"{Yellow}18" : "rgba(255,255,255,.025)")
            .Style("color", value == current ? Yellow : White).Style("font-family", "inherit")
            .Style("font-size", ".43rem").Style("cursor", "pointer")
            .OnClick(() => setScope?.Invoke(value));

    private static ElementBuilder Map(object receiver, SpatialWorkshopScene scene, double avatarX, double avatarY, Func<string, Task> selectDestination, double zoom, Action<double>? setZoom)
    {
        var map = MapFrame(receiver, zoom, setZoom);
        map.Content(WorkshopGui.Element(receiver, "svg").Attribute("viewBox", "0 0 100 100")
            .Style("position", "absolute").Style("inset", "0").Style("width", "100%").Style("height", "100%")
            .Style("pointer-events", "none").Content(Grid(receiver)).Content(Compass(receiver)).Content(ScaleBar(receiver)));

        foreach (var item in scene.Interactables)
        {
            var b = item.Bounds;
            var accent = Accent(item.ExperienceId);
            map.Content(WorkshopGui.Button(receiver)
                .Style("position", "absolute").Style("left", $"{b.X:0.##}%").Style("top", $"{b.Y:0.##}%")
                .Style("width", $"{Math.Max(b.Width, 1.5d):0.##}%").Style("height", $"{Math.Max(b.Height, 1.5d):0.##}%")
                .Style("box-sizing", "border-box").Style("padding", "0")
                .Style("border", $"2px solid {accent}cc").Style("background", Fill(accent))
                .Style("color", White).Style("font-family", "inherit").Style("cursor", "pointer").Style("z-index", "3")
                .AriaLabel($"Navigate to {item.Name}").Title($"Select {item.Name}")
                .OnClick(() => selectDestination(item.Id)).StopPropagation("onclick")
                .Content(WorkshopGui.Element(receiver, "span").Style("display", "flex").Style("width", "100%").Style("height", "100%")
                    .Style("align-items", "center").Style("justify-content", "center").Style("padding", ".15rem")
                    .Style("box-sizing", "border-box").Style("font-size", "clamp(.4rem,.52vw,.62rem)")
                    .Style("font-weight", "700").Style("line-height", "1.05").Style("text-align", "center")
                    .Style("text-shadow", "0 1px 2px #000").Text(item.Name)));
        }

        return AddYouAreHere(receiver, map, avatarX, avatarY);
    }

    private static ElementBuilder CityMap(object receiver, double avatarX, double avatarY, Func<string, Task> selectDestination, double zoom, Action<double>? setZoom)
    {
        var map = MapFrame(receiver, zoom, setZoom);
        map.Content(WorkshopGui.Element(receiver, "svg").Attribute("viewBox", "0 0 100 100")
            .Style("position", "absolute").Style("inset", "0").Style("width", "100%").Style("height", "100%")
            .Style("pointer-events", "none").Content(Grid(receiver)).Content(Compass(receiver)).Content(CityRoads(receiver)));

        foreach (var structure in SingularityCityCatalog.Structures)
        {
            var b = structure.Bounds;
            var accent = CityAccent(structure.Kind);
            map.Content(WorkshopGui.Element(receiver, "div")
                .Style("position", "absolute").Style("left", $"{b.X:0.##}%").Style("top", $"{b.Y:0.##}%")
                .Style("width", $"{b.Width:0.##}%").Style("height", $"{b.Height:0.##}%")
                .Style("box-sizing", "border-box").Style("border", $"1px dashed {accent}aa")
                .Style("background", $"{accent}0d").Style("z-index", "2")
                .Title($"{structure.Name}: UNDER CONSTRUCTION — {structure.FutureFunction}")
                .Content(WorkshopGui.Element(receiver, "span").Style("display", "block").Style("padding", ".2rem")
                    .Style("font-size", "clamp(.35rem,.46vw,.55rem)").Style("color", accent)
                    .Style("font-weight", "700").Text(structure.Name)));
        }

        var workshop = SingularityCityCatalog.WorkshopDistrict;
        map.Content(WorkshopGui.Button(receiver)
            .Style("position", "absolute").Style("left", $"{workshop.X}%").Style("top", $"{workshop.Y}%")
            .Style("width", $"{workshop.Width}%").Style("height", $"{workshop.Height}%")
            .Style("box-sizing", "border-box").Style("border", $"2px solid {Yellow}dd")
            .Style("background", $"{Yellow}14").Style("color", Yellow).Style("z-index", "4")
            .Style("font-family", "inherit").Style("font-size", ".58rem").Style("font-weight", "700")
            .Style("cursor", "pointer").Label("WORKSHOP // CENTER OF SINGULARITY CITY")
            .OnClick(() => selectDestination("workshop.forge")));

        return AddYouAreHere(receiver, map, avatarX, avatarY);
    }

    private static ElementBuilder WorldMap(object receiver, double avatarX, double avatarY, double zoom, Action<double>? setZoom)
    {
        var map = MapFrame(receiver, zoom, setZoom);
        map.Content(WorkshopGui.Element(receiver, "svg").Attribute("viewBox", "0 0 100 60")
            .Style("position", "absolute").Style("inset", "0").Style("width", "100%").Style("height", "100%")
            .Style("pointer-events", "none")
            .Content(WorkshopGui.Element(receiver, "rect").Attribute("x", 1).Attribute("y", 1).Attribute("width", 98).Attribute("height", 58)
                .Attribute("fill", "#06101a").Attribute("stroke", Cyan).Attribute("stroke-opacity", ".25"))
            .Content(WorkshopGui.Element(receiver, "path").Attribute("d", "M5 38 C18 18 27 28 39 20 S62 10 72 24 S87 18 96 34 L96 58 L5 58 Z")
                .Attribute("fill", $"{Green}09").Attribute("stroke", Green).Attribute("stroke-opacity", ".35").Attribute("stroke-width", ".5")));

        foreach (var region in SpatialWorldRegionCatalog.Regions)
        {
            map.Content(WorkshopGui.Element(receiver, "div")
                .Style("position", "absolute").Style("left", $"{region.Bounds.X}%").Style("top", $"{region.Bounds.Y}%")
                .Style("width", $"{region.Bounds.Width}%").Style("height", $"{region.Bounds.Height}%")
                .Style("border", $"1px dashed {Cyan}55").Style("background", $"{Cyan}06")
                .Style("box-sizing", "border-box").Style("z-index", "2")
                .Title($"{region.Name}: UNDER CONSTRUCTION — {region.FutureFunction}")
                .Content(WorkshopGui.Element(receiver, "span").Style("display", "block").Style("padding", ".2rem")
                    .Style("font-size", ".42rem").Style("color", Cyan).Text(region.Name)));
        }

        return AddYouAreHere(receiver, map, 50, 30);
    }

    private static ElementBuilder MapFrame(object receiver, double zoom, Action<double>? setZoom)
    {
        var frame = WorkshopGui.Panel(receiver).Style("position", "relative").Style("min-width", "0")
            .Style("min-height", "0").Style("overflow", "hidden")
            .Style("border", $"1px solid {Cyan}55").Style("background", "#01060d");

        var transform = $"scale({Math.Clamp(zoom, .75, 2.5):0.###})";
        frame.Content(WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("inset", "0")
            .Style("transform", transform).Style("transform-origin", "center center"));

        frame.Content(WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("right", ".5rem").Style("bottom", ".5rem")
            .Style("z-index", "10").Style("display", "flex").Style("gap", ".25rem")
            .Content(WorkshopGui.Button(receiver).Label("−").Style("width", "2rem").Style("height", "2rem").Style("cursor", "pointer").OnClick(() => setZoom?.Invoke(Math.Max(.75, zoom - .15))))
            .Content(WorkshopGui.Button(receiver).Label("+").Style("width", "2rem").Style("height", "2rem").Style("cursor", "pointer").OnClick(() => setZoom?.Invoke(Math.Min(2.5, zoom + .15)))));
        return frame;
    }

    private static ElementBuilder AddYouAreHere(object receiver, ElementBuilder map, double avatarX, double avatarY)
    {
        map.Content(WorkshopGui.Element(receiver, "div").Style("position", "absolute")
            .Style("left", $"{avatarX:0.##}%").Style("top", $"{avatarY:0.##}%")
            .Style("width", "13px").Style("height", "13px").Style("transform", "translate(-50%,-50%)")
            .Style("border", $"2px solid {White}").Style("border-radius", "50%")
            .Style("box-shadow", $"0 0 18px {White}aa").Style("animation", "workshop-map-pulse 1.6s ease-in-out infinite")
            .Style("z-index", "6").Style("pointer-events", "none"));
        map.Content(WorkshopGui.Element(receiver, "div").Style("position", "absolute")
            .Style("left", $"{avatarX:0.##}%").Style("top", $"{avatarY + 2.4:0.##}%")
            .Style("transform", "translateX(-50%)").Style("z-index", "6").Style("pointer-events", "none")
            .Style("color", White).Style("font-size", ".42rem").Style("font-weight", "700").Text("YOU ARE HERE"));
        return map;
    }

    private static ElementBuilder Directory(object receiver, SpatialWorkshopScene scene, Func<string, Task> selectDestination)
    {
        var directory = DirectoryFrame(receiver, "WORKSHOP DESTINATIONS");
        foreach (var group in scene.Interactables.GroupBy(item => item.Place?.Domain ?? SpatialDomain.Workshop))
        {
            directory.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".55rem").Style("color", Yellow)
                .Style("font-size", ".5rem").Style("font-weight", "700").Style("letter-spacing", ".12em")
                .Text(group.Key == SpatialDomain.Workshop ? "WORKSHOP TOOLS & STUDIOS" : "CITY PLACES"));
            foreach (var item in group)
            {
                var accent = Accent(item.ExperienceId);
                directory.Content(WorkshopGui.Button(receiver).Label(item.Name.ToUpperInvariant())
                    .Style("display", "block").Style("width", "100%").Style("margin", ".2rem 0").Style("padding", ".42rem")
                    .Style("text-align", "left").Style("border", $"1px solid {accent}66").Style("border-left", $"4px solid {accent}")
                    .Style("background", "rgba(255,255,255,.025)").Style("color", White).Style("font-family", "inherit")
                    .Style("font-size", ".44rem").Style("font-weight", "700").Style("cursor", "pointer")
                    .OnClick(() => selectDestination(item.Id)));
            }
        }
        return directory;
    }

    private static ElementBuilder DirectoryFrame(object receiver, string title)
        => WorkshopGui.Element(receiver, "div").Style("border", $"{Cyan}33 solid 1px").Style("background", "rgba(0,10,18,.72)")
            .Style("padding", ".65rem").Style("overflow", "auto").Style("min-height", "0")
            .Content(WorkshopGui.Element(receiver, "div").Style("color", Yellow).Style("font-size", ".55rem")
                .Style("font-weight", "700").Style("letter-spacing", ".16em").Text(title))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin", ".35rem 0 .6rem").Style("color", "#8fa7b2")
                .Style("font-size", ".42rem").Style("line-height", "1.5")
                .Text("The list is the readable index. The map is the spatial truth. Only the map changes scale."));

    private static ElementBuilder CityDirectory(object receiver, Func<string, Task> selectDestination)
    {
        var directory = DirectoryFrame(receiver, "SINGULARITY CITY // PLANNED MASSING");
        directory.Content(WorkshopGui.Element(receiver, "div").Style("padding", ".45rem").Style("border", $"1px solid {Yellow}44")
            .Style("color", Yellow).Style("font-size", ".43rem").Style("line-height", "1.5")
            .Text("The Workshop is the completed center point. The surrounding city is deliberately incomplete. Move near a site to learn what will eventually occupy it."));
        directory.Content(WorkshopGui.Button(receiver).Label("ENTER WORKSHOP DISTRICT")
            .Style("width", "100%").Style("margin", ".5rem 0").Style("padding", ".5rem").Style("cursor", "pointer")
            .Style("border", $"1px solid {Yellow}88").Style("background", $"{Yellow}12").Style("color", Yellow)
            .Style("font-family", "inherit").Style("font-size", ".45rem").OnClick(() => selectDestination("workshop.forge")));
        foreach (var structure in SingularityCityCatalog.Structures)
            directory.Content(WorkshopGui.Element(receiver, "div").Style("margin", ".2rem 0").Style("padding", ".35rem")
                .Style("border-left", $"3px solid {CityAccent(structure.Kind)}")
                .Style("background", "rgba(255,255,255,.02)")
                .Content(WorkshopGui.Element(receiver, "div").Style("color", White).Style("font-size", ".43rem").Text(structure.Name))
                .Content(WorkshopGui.Element(receiver, "div").Style("color", "#8299a4").Style("font-size", ".36rem").Text("UNDER CONSTRUCTION")));
        return directory;
    }

    private static ElementBuilder WorldDirectory(object receiver)
    {
        var directory = DirectoryFrame(receiver, "WORLD // UNDER CONSTRUCTION");
        directory.Content(WorkshopGui.Element(receiver, "div").Style("padding", ".45rem").Style("border", $"1px solid {Cyan}44")
            .Style("color", Cyan).Style("font-size", ".43rem").Style("line-height", "1.5")
            .Text("A future world layer will connect the city to regional and global infrastructure. This view is intentionally incomplete rather than pretending those places already exist."));
        foreach (var region in SpatialWorldRegionCatalog.Regions)
            directory.Content(WorkshopGui.Element(receiver, "div").Style("margin", ".25rem 0").Style("padding", ".4rem")
                .Style("border-left", $"3px solid {Cyan}")
                .Content(WorkshopGui.Element(receiver, "div").Style("color", White).Style("font-size", ".43rem").Text(region.Name))
                .Content(WorkshopGui.Element(receiver, "div").Style("color", "#8299a4").Style("font-size", ".36rem").Text("UNDER CONSTRUCTION")));
        return directory;
    }

    private static ElementBuilder Grid(object receiver)
    {
        var svg = WorkshopGui.Element(receiver, "g");
        for (var i = 0; i <= 20; i++)
        {
            var p = i * 5;
            svg.Child(WorkshopGui.Element(receiver, "line").Attribute("x1", p).Attribute("y1", 0).Attribute("x2", p).Attribute("y2", 100).Attribute("stroke", Cyan).Attribute("stroke-opacity", ".1").Attribute("stroke-width", ".12"));
            svg.Child(WorkshopGui.Element(receiver, "line").Attribute("x1", 0).Attribute("y1", p).Attribute("x2", 100).Attribute("y2", p).Attribute("stroke", Cyan).Attribute("stroke-opacity", ".1").Attribute("stroke-width", ".12"));
        }
        return svg;
    }

    private static ElementBuilder CityRoads(object receiver)
        => WorkshopGui.Element(receiver, "svg").Content(
            WorkshopGui.Element(receiver, "path").Attribute("d", "M0 40 H100 M0 60 H100 M40 0 V100 M60 0 V100")
                .Attribute("stroke", White).Attribute("stroke-opacity", ".08").Attribute("stroke-width", ".7"))
            .Content(WorkshopGui.Element(receiver, "path").Attribute("d", "M15 0 C20 30 28 65 35 100 M85 0 C78 28 72 70 68 100")
                .Attribute("stroke", Cyan).Attribute("stroke-opacity", ".08").Attribute("stroke-width", ".5"));

    private static ElementBuilder Compass(object receiver)
        => WorkshopGui.Element(receiver, "g")
            .Content(WorkshopGui.Element(receiver, "text").Attribute("x", 94).Attribute("y", 6).Attribute("fill", White).Attribute("font-size", "3").Attribute("font-weight", "700").Attribute("text-anchor", "middle").Text("N"))
            .Content(WorkshopGui.Element(receiver, "line").Attribute("x1", 94).Attribute("y1", 7).Attribute("x2", 94).Attribute("y2", 11).Attribute("stroke", White).Attribute("stroke-width", ".35"));

    private static ElementBuilder ScaleBar(object receiver)
        => WorkshopGui.Element(receiver, "g")
            .Content(WorkshopGui.Element(receiver, "line").Attribute("x1", 6).Attribute("y1", 94).Attribute("x2", 26).Attribute("y2", 94).Attribute("stroke", Yellow).Attribute("stroke-width", ".45"))
            .Content(WorkshopGui.Element(receiver, "text").Attribute("x", 16).Attribute("y", 92).Attribute("fill", Yellow).Attribute("font-size", "2.1").Attribute("text-anchor", "middle").Text("20 WORLD UNITS"));

    private static string Fill(string accent) => accent switch
    {
        Cyan => "rgba(0,234,255,.10)",
        Yellow => "rgba(255,211,77,.10)",
        Magenta => "rgba(255,56,209,.10)",
        _ => "rgba(82,224,90,.10)"
    };

    private static string Accent(string experienceId) => experienceId switch
    {
        "mansion" => Magenta,
        "forge" => Cyan,
        "singularity-transit" => Cyan,
        "singularity-ontology-mall" => Yellow,
        "image-workshop" => Magenta,
        _ => Green
    };

    private static string CityAccent(SpatialCityStructureKind kind) => kind switch
    {
        SpatialCityStructureKind.Civic => Magenta,
        SpatialCityStructureKind.Transport or SpatialCityStructureKind.Port => Cyan,
        SpatialCityStructureKind.Education or SpatialCityStructureKind.Science or SpatialCityStructureKind.Technology => Yellow,
        SpatialCityStructureKind.Park => Green,
        _ => White
    };
}
