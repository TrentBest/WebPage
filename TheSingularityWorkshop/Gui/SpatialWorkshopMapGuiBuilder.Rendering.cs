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
        Action<SpatialMapScope>? setScope = null,
        SpatialWorkshopArrangement? arrangement = null,
        ElementBuilder? hubIdentityDock = null)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0").Style("z-index", "200")
            .Style("display", "flex").Style("align-items", "center").Style("justify-content", "center")
            .Style("padding", "1rem").Style("box-sizing", "border-box")
            .Style("background", "rgba(1,4,10,.96)").Style("color", White)
            .Style("font-family", "Consolas, 'Courier New', monospace");

        root.Content(WorkshopGui.Element(receiver, "style").Text(
            "@keyframes workshop-map-pulse{0%,100%{opacity:.62;transform:scale(1)}50%{opacity:1;transform:scale(1.15)}}@keyframes workshop-map-breathe{0%,100%{filter:brightness(1);transform:scale(1)}50%{filter:brightness(1.42);transform:scale(1.035)}}.workshop-map-interactable:hover,.workshop-map-structure:focus-within{animation:workshop-map-breathe 1.35s ease-in-out infinite;transform-origin:center center}.workshop-map-building-controls{opacity:0;pointer-events:none;transition:opacity .15s ease}.workshop-map-structure:hover .workshop-map-building-controls,.workshop-map-structure:focus-within .workshop-map-building-controls{opacity:1;pointer-events:auto}.workshop-map-interactable:focus-visible,.workshop-map-structure:focus-visible{outline:1px solid #ffd34d;outline-offset:2px}@media(prefers-reduced-motion:reduce){.workshop-map-interactable:hover,.workshop-map-structure:hover{animation:none;filter:brightness(1.2);transform:scale(1.01)}}.workshop-map-structure{transition:filter .18s ease,box-shadow .18s ease,transform .18s ease,border-color .18s ease}.workshop-map-structure.is-breathing{box-shadow:0 0 24px rgba(255,211,77,.16),inset 0 0 22px rgba(0,234,255,.08)}.workshop-map-structure:hover,.workshop-map-structure:focus-within{filter:brightness(1.16)}.workshop-map-building-info{position:absolute;left:50%;bottom:calc(100% + .45rem);transform:translate(-50%,.35rem);width:min(240px,28vw);padding:.5rem .6rem;border:1px solid rgba(0,234,255,.42);background:rgba(1,6,12,.96);box-shadow:0 0 24px rgba(0,234,255,.12),inset 0 0 18px rgba(0,234,255,.035);opacity:0;pointer-events:none;transition:opacity .16s ease,transform .16s ease;z-index:30}.workshop-map-structure:hover .workshop-map-building-info,.workshop-map-structure:focus-within .workshop-map-building-info{opacity:1;transform:translate(-50%,0)}.workshop-map-directory-item{transition:transform .16s ease,border-color .16s ease,background .16s ease,box-shadow .16s ease}.workshop-map-directory-item:hover,.workshop-map-directory-item:focus-visible{transform:translateX(3px);border-color:rgba(255,255,255,.52)!important;background:rgba(255,255,255,.055)!important;box-shadow:0 0 18px rgba(0,234,255,.08)}}"));

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
                .Style("display", "flex").Style("flex-direction", "column").Style("gap", ".2rem")
                .Content(WorkshopGui.Element(receiver, "div")
                    .Style("color", Cyan).Style("font-size", "clamp(.85rem,2.2vw,1.35rem)")
                    .Style("font-weight", "800").Style("letter-spacing", ".16em")
                    .Text(scope switch
                    {
                        SpatialMapScope.Workshop => "MAP: WORKSHOP",
                        SpatialMapScope.City => "MAP: CITY // UNDER CONSTRUCTION",
                        SpatialMapScope.World => "MAP: WORLD // UNDER CONSTRUCTION",
                        SpatialMapScope.SolarSystem => "MAP: SOLAR SYSTEM // UNDER CONSTRUCTION",
                        _ => "SPATIAL MASTER MAP"
                    }))
                .Content(WorkshopGui.Element(receiver, "div")
                    .Style("color", "#8fa7b2").Style("font-size", ".42rem").Style("letter-spacing", ".14em")
                    .Text("PRIMARY NAVIGATION // SPATIAL TRUTH // INTERACTABLE FACILITIES")))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("display", "flex").Style("flex-direction", "column").Style("align-items", "flex-end").Style("gap", ".2rem")
                .Content(WorkshopGui.Element(receiver, "div")
                    .Style("color", Yellow).Style("font-size", ".45rem").Style("letter-spacing", ".14em")
                    .Text($"SESSION // {moniker.ToUpperInvariant()}"))
                .Content(WorkshopGui.Element(receiver, "div")
                    .Style("color", "#8fa7b2").Style("font-size", ".36rem").Style("letter-spacing", ".12em")
                    .Text("MAP STATUS // LIVE ARRANGEMENT"))
                .Content(WorkshopGui.Element(receiver, "div")
                    .Style("color", "#8fa7b2").Style("font-size", ".36rem").Style("letter-spacing", ".12em")
                    .Text(scope switch
                    {
                        SpatialMapScope.Workshop => "WORKSHOP CAMPUS // COMPLETE MAP",
                        SpatialMapScope.City => "SINGULARITY CITY // UNDER CONSTRUCTION",
                        SpatialMapScope.World => "WORLD // DEVELOPMENT MAP",
                        SpatialMapScope.SolarSystem => "SOLAR SYSTEM // UNDER CONSTRUCTION",
                        _ => "SPATIAL MAP"
                    }))));

        var scopeBar = WorkshopGui.Element(receiver, "div")
            .Style("display", "flex").Style("gap", ".35rem").Style("flex-wrap", "wrap")
            .Style("align-items", "center").Style("margin", ".65rem 0")
            .Content(ScopeButton(receiver, "WORKSHOP", SpatialMapScope.Workshop, scope, setScope))
            .Content(ScopeButton(receiver, "CITY // UNDER CONSTRUCTION", SpatialMapScope.City, scope, setScope))
            .Content(ScopeButton(receiver, "WORLD // UNDER CONSTRUCTION", SpatialMapScope.World, scope, setScope))
            .Content(ScopeButton(receiver, "SOLAR SYSTEM // UNDER CONSTRUCTION", SpatialMapScope.SolarSystem, scope, setScope))
            .Content(WorkshopGui.Element(receiver, "span").Style("margin-left", "auto").Style("color", "#8fa7b2").Style("font-size", ".43rem")
                .Text("SCROLL THE MAP LIST • SCALE WITH + / −"));

        panel.Content(scopeBar);
        if (hubIdentityDock is not null)
            panel.Content(hubIdentityDock);

        var layout = WorkshopGui.Element(receiver, "div")
            .Style("display", "grid")
            .Style("grid-template-columns", "minmax(0,1fr) minmax(260px,330px)")
            .Style("gap", ".8rem").Style("min-height", "0").Style("flex", "1");

        layout.Content(scope == SpatialMapScope.Workshop
            ? Map(receiver, scene, avatarX, avatarY, selectDestination, zoom, setZoom, arrangement)
            : scope == SpatialMapScope.City
                ? CityMap(receiver, avatarX, avatarY, selectDestination, zoom, setZoom)
                : scope == SpatialMapScope.World
                    ? WorldMap(receiver, avatarX, avatarY, zoom, setZoom)
                    : SolarSystemMap(receiver, zoom, setZoom));

        layout.Content(scope == SpatialMapScope.Workshop
            ? Directory(receiver, scene, selectDestination)
            : scope == SpatialMapScope.City
                ? CityDirectory(receiver, selectDestination)
                : scope == SpatialMapScope.World
                    ? WorldDirectory(receiver)
                    : SolarSystemDirectory(receiver));

        panel.Content(layout);
        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("display", "flex").Style("justify-content", "space-between").Style("align-items", "center")
            .Style("gap", "1rem").Style("margin-top", ".7rem")
            .Content(WorkshopGui.Element(receiver, "span").Style("color", "#a8bbc4").Style("font-size", ".46rem")
                .Text(scope == SpatialMapScope.Workshop
                    ? "SELECT A BUILDING. THE MAP CLOSES, YOUR AVATAR WALKS TO ITS DOOR, AND A SECOND CLICK ENTERS."
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

    private static ElementBuilder Map(object receiver, SpatialWorkshopScene scene, double avatarX, double avatarY, Func<string, Task> selectDestination, double zoom, Action<double>? setZoom, SpatialWorkshopArrangement? arrangement)
    {
        var map = MapFrame(receiver, zoom, setZoom);
        map.Content(WorkshopGui.Element(receiver, "svg").Attribute("viewBox", "0 0 100 100")
            .Style("position", "absolute").Style("inset", "0").Style("width", "100%").Style("height", "100%")
            .Style("pointer-events", "none").Content(Grid(receiver)).Content(Compass(receiver)).Content(ScaleBar(receiver)));

        foreach (var item in scene.Interactables)
        {
            var b = arrangement?.GetBounds(item) ?? item.Bounds;
            var accent = Accent(item.ExperienceId);

            var container = WorkshopGui.Element(receiver, "div")
                .Class($"workshop-map-structure{(item.IsBreathing ? " is-breathing" : string.Empty)}")
                .Style("position", "absolute")
                .Style("left", $"{b.X:0.##}%")
                .Style("top", $"{b.Y:0.##}%")
                .Style("width", $"{Math.Max(b.Width, 1.5d):0.##}%")
                .Style("height", $"{Math.Max(b.Height, 1.5d):0.##}%")
                .Style("box-sizing", "border-box")
                .Style("border", $"1.5px solid {accent}dd")
                .Style("border-radius", "2px")
                .Style("background", BuildingFill(accent))
                .Style("box-shadow", $"0 0 14px {accent}18,inset 0 0 18px {accent}12")
                .Style("color", White)
                .Style("z-index", "3")
                .Style("outline", "none")
                .Style("overflow", "visible")
                .Attribute("tabindex", "0")
                .AriaLabel($"Select {item.Name}")
                .Title($"Select {item.Name} // hover for facility details")
                .OnMouseEnter(() => item.OnHover(new NormalizedPointer(.5d, .5d)))
                .OnMouseLeave(item.OnHoverExit);

            container.Content(WorkshopGui.Element(receiver, "span")
                .Style("position", "absolute")
                .Style("left", "50%").Style("top", "50%")
                .Style("transform", "translate(-50%,-50%)")
                .Style("font-size", "clamp(.4rem,.52vw,.62rem)")
                .Style("font-weight", "800").Style("line-height", "1.05")
                .Style("text-align", "center").Style("text-shadow", "0 1px 3px #000")
                .Style("pointer-events", "none")
                .Style("z-index", "8")
                .Text(item.Name));

            var placeDescription = item.Place?.Description ?? "Spatial Workshop facility.";
            container.Content(WorkshopGui.Element(receiver, "div")
                .Class("workshop-map-building-info")
                .Content(WorkshopGui.Element(receiver, "div")
                    .Style("color", accent).Style("font-size", ".48rem").Style("font-weight", "800")
                    .Style("letter-spacing", ".12em").Text(item.Name.ToUpperInvariant()))
                .Content(WorkshopGui.Element(receiver, "div")
                    .Style("margin-top", ".25rem").Style("color", "#a8bbc4").Style("font-size", ".37rem")
                    .Style("line-height", "1.45").Text(placeDescription))
                .Content(WorkshopGui.Element(receiver, "div")
                    .Style("margin-top", ".35rem").Style("color", Yellow).Style("font-size", ".34rem")
                    .Style("letter-spacing", ".1em").Text(item.IsBreathing ? "LIVE FACILITY // CLICK TO NAVIGATE" : "HOVER FOR LIVE DETAILS")));

            container.Content(MapBuildingGeometry(receiver, item, b, accent));
            container.OnClick(() => _ = selectDestination(item.Id)).StopPropagation("onclick");


            map.Content(container);
        }

        return AddYouAreHere(receiver, map, avatarX, avatarY);
    }


    private static ElementBuilder MapBuildingGeometry(object receiver, SpatialInteractable item, SpatialBounds b, string accent)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 100")
            .Style("position", "absolute").Style("inset", "0").Style("width", "100%").Style("height", "100%")
            .Style("pointer-events", "none").Style("overflow", "visible");

        svg.Content(WorkshopGui.Element(receiver, "polygon")
            .Attribute("points", "5,13 13,5 87,5 95,13 95,87 87,95 13,95 5,87")
            .Attribute("fill", accent).Attribute("fill-opacity", ".035")
            .Attribute("stroke", accent).Attribute("stroke-opacity", ".32").Attribute("stroke-width", ".8"));

        svg.Content(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", 1).Attribute("y", 1).Attribute("width", 98).Attribute("height", 98)
            .Attribute("fill", "none").Attribute("stroke", accent).Attribute("stroke-opacity", ".72").Attribute("stroke-width", "1.1"));

        var spec = SingularityCampusCatalog.Buildings.FirstOrDefault(building => building.Id == item.Id);
        var columns = Math.Clamp(spec?.Columns ?? 0, 0, 8);
        var rows = Math.Clamp(spec?.Rows ?? 0, 0, 8);

        for (var column = 1; column < columns; column++)
        {
            var x = 5d + 90d * column / columns;
            svg.Content(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", x).Attribute("y1", 7).Attribute("x2", x).Attribute("y2", 93)
                .Attribute("stroke", accent).Attribute("stroke-opacity", ".13").Attribute("stroke-width", ".55"));
        }

        for (var row = 1; row < rows; row++)
        {
            var y = 5d + 90d * row / rows;
            svg.Content(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", 7).Attribute("y1", y).Attribute("x2", 93).Attribute("y2", y)
                .Attribute("stroke", accent).Attribute("stroke-opacity", ".11").Attribute("stroke-width", ".55"));
        }

        svg.Content(WorkshopGui.Element(receiver, "path")
            .Attribute("d", "M 14 8 H 86")
            .Attribute("stroke", accent).Attribute("stroke-opacity", ".55").Attribute("stroke-width", "1.4"));

        foreach (var opening in item.Openings)
        {
            var startX = opening.Edge is SpatialGeometryEdge.Top or SpatialGeometryEdge.Bottom
                ? (opening.Start / Math.Max(1d, b.Width)) * 100d : 0d;
            var startY = opening.Edge is SpatialGeometryEdge.Left or SpatialGeometryEdge.Right
                ? (opening.Start / Math.Max(1d, b.Height)) * 100d : 0d;
            var lengthX = opening.Edge is SpatialGeometryEdge.Top or SpatialGeometryEdge.Bottom
                ? (opening.Length / Math.Max(1d, b.Width)) * 100d : 0d;
            var lengthY = opening.Edge is SpatialGeometryEdge.Left or SpatialGeometryEdge.Right
                ? (opening.Length / Math.Max(1d, b.Height)) * 100d : 0d;

            switch (opening.Edge)
            {
                case SpatialGeometryEdge.Top:
                    svg.Content(WorkshopGui.Element(receiver, "line").Attribute("x1", startX).Attribute("y1", 0).Attribute("x2", startX + lengthX).Attribute("y2", 0).Attribute("stroke", "#02060b").Attribute("stroke-width", "4"));
                    if (opening.Kind == SpatialOpeningKind.Door)
                        svg.Content(WorkshopGui.Element(receiver, "path").Attribute("d", $"M {startX + lengthX:0.##} 0 A {lengthX:0.##} {lengthX:0.##} 0 0 1 {startX:0.##} {lengthX:0.##}").Attribute("fill", "none").Attribute("stroke", accent).Attribute("stroke-width", ".9"));
                    break;
                case SpatialGeometryEdge.Bottom:
                    svg.Content(WorkshopGui.Element(receiver, "line").Attribute("x1", startX).Attribute("y1", 100).Attribute("x2", startX + lengthX).Attribute("y2", 100).Attribute("stroke", "#02060b").Attribute("stroke-width", "4"));
                    if (opening.Kind == SpatialOpeningKind.Door)
                        svg.Content(WorkshopGui.Element(receiver, "path").Attribute("d", $"M {startX + lengthX:0.##} 100 A {lengthX:0.##} {lengthX:0.##} 0 0 0 {startX:0.##} {100 - lengthX:0.##}").Attribute("fill", "none").Attribute("stroke", accent).Attribute("stroke-width", ".9"));
                    break;
                case SpatialGeometryEdge.Right:
                    svg.Content(WorkshopGui.Element(receiver, "line").Attribute("x1", 100).Attribute("y1", startY).Attribute("x2", 100).Attribute("y2", startY + lengthY).Attribute("stroke", "#02060b").Attribute("stroke-width", "4"));
                    if (opening.Kind == SpatialOpeningKind.Door)
                        svg.Content(WorkshopGui.Element(receiver, "path").Attribute("d", $"M 100 {startY + lengthY:0.##} A {lengthY:0.##} {lengthY:0.##} 0 0 0 {100 - lengthY:0.##} {startY:0.##}").Attribute("fill", "none").Attribute("stroke", accent).Attribute("stroke-width", ".9"));
                    break;
                case SpatialGeometryEdge.Left:
                    svg.Content(WorkshopGui.Element(receiver, "line").Attribute("x1", 0).Attribute("y1", startY).Attribute("x2", 0).Attribute("y2", startY + lengthY).Attribute("stroke", "#02060b").Attribute("stroke-width", "4"));
                    if (opening.Kind == SpatialOpeningKind.Door)
                        svg.Content(WorkshopGui.Element(receiver, "path").Attribute("d", $"M 0 {startY:0.##} A {lengthY:0.##} {lengthY:0.##} 0 0 0 {lengthY:0.##} {startY + lengthY:0.##}").Attribute("fill", "none").Attribute("stroke", accent).Attribute("stroke-width", ".9"));
                    break;
            }
        }

        return svg;
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
                .Class("workshop-map-structure")
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
            .Class("workshop-map-interactable")
            .Style("position", "absolute").Style("left", $"{workshop.X}%").Style("top", $"{workshop.Y}%")
            .Style("width", $"{workshop.Width}%").Style("height", $"{workshop.Height}%")
            .Style("box-sizing", "border-box").Style("border", $"2px solid {Yellow}dd")
            .Style("background", $"{Yellow}14").Style("color", Yellow).Style("z-index", "4")
            .Style("font-family", "inherit").Style("font-size", ".58rem").Style("font-weight", "700")
            .Style("cursor", "pointer").Text("WORKSHOP // CENTER OF SINGULARITY CITY")
            .OnClick(() => selectDestination("forge")));

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
                .Class("workshop-map-structure")
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

    private static ElementBuilder SolarSystemMap(object receiver, double zoom, Action<double>? setZoom)
    {
        var manifest = SpatialSolarSystemManifest.CreateDefault();
        var data = SpatialSolarSystemDataManifest.Nasa;
        var map = MapFrame(receiver, zoom, setZoom);

        map.Content(WorkshopGui.Element(receiver, "svg").Attribute("viewBox", "0 0 100 70")
            .Style("position", "absolute").Style("inset", "0").Style("width", "100%").Style("height", "100%")
            .Style("pointer-events", "none").Content(Grid(receiver)));

        // Schematic orbit bands: distances are represented for legibility, while the
        // NASA/JPL values remain attached to the semantic objects in the data panel.
        foreach (var body in manifest.Bodies.Where(x => x.Kind is SpatialCelestialBodyKind.Planet))
        {
            var orbitRadius = body.Id switch
            {
                "mercury" => 9d,
                "venus" => 15d,
                "earth" => 22d,
                "mars" => 29d,
                "jupiter" => 39d,
                "saturn" => 49d,
                "uranus" => 58d,
                "neptune" => 67d,
                _ => 0d
            };

            if (orbitRadius <= 0) continue;

            map.Content(WorkshopGui.Element(receiver, "div")
                .Style("position", "absolute").Style("left", "50%").Style("top", "50%")
                .Style("width", $"{orbitRadius * 2}%").Style("height", $"{orbitRadius * 2}%")
                .Style("transform", "translate(-50%,-50%)")
                .Style("border", "1px dashed rgba(0,234,255,.22)")
                .Style("border-radius", "50%").Style("pointer-events", "none"));
        }

        foreach (var body in manifest.Bodies)
        {
            var point = body.Position;
            var reference = data.Find(body.Id);
            var accent = body.Kind == SpatialCelestialBodyKind.Star ? Yellow : Cyan;
            var size = body.Kind == SpatialCelestialBodyKind.Star ? 8d : Math.Clamp(body.DisplayRadius * 1.7d, 2d, 8d);

            map.Content(WorkshopGui.Element(receiver, "div")
                .Style("position", "absolute")
                .Style("left", $"{point.X}%").Style("top", $"{point.Y * .70}%")
                .Style("width", $"{size}%").Style("aspect-ratio", "1")
                .Style("transform", "translate(-50%,-50%)")
                .Style("border", $"1px solid {accent}")
                .Style("border-radius", "50%")
                .Style("background", $"{accent}22")
                .Style("box-shadow", $"0 0 18px {accent}55")
                .Style("z-index", "3")
                .Title(reference is null
                    ? body.Name
                    : $"{body.Name} // {reference.Value.DiameterKm:N0} km // {reference.Value.MassKg:E2} kg")
                .Content(WorkshopGui.Element(receiver, "span")
                    .Style("position", "absolute").Style("left", "50%").Style("top", "50%")
                    .Style("transform", "translate(-50%,-50%)")
                    .Style("font-size", ".36rem").Style("font-weight", "700")
                    .Style("color", White).Style("white-space", "nowrap")
                    .Text(body.Name)));
        }

        return map;
    }

    private static ElementBuilder ControlButton(object receiver, string label, Action action)
        => WorkshopGui.Button(receiver).Label(label)
            .Style("width", "1.35rem").Style("height", "1.25rem")
            .Style("padding", "0")
            .Style("border", "1px solid rgba(0,234,255,.35)")
            .Style("background", "rgba(0,234,255,.06)")
            .Style("color", White)
            .Style("font-family", "inherit").Style("font-size", ".55rem")
            .Style("cursor", "pointer")
            .OnClick(action)
            .StopPropagation("onclick");

    private static ElementBuilder SolarSystemDirectory(object receiver)
    {
        var directory = DirectoryFrame(receiver, "SOLAR SYSTEM // REPRESENTATIONAL DATA");
        var data = SpatialSolarSystemDataManifest.Nasa;

        directory.Content(WorkshopGui.Element(receiver, "div")
            .Style("padding", ".45rem").Style("margin-bottom", ".45rem")
            .Style("border", $"1px solid {Yellow}44").Style("color", Yellow)
            .Style("font-size", ".42rem").Style("line-height", "1.5")
            .Text("UNDER CONSTRUCTION • SCHEMATIC RESEARCH DATA ONLY • DYNAMIC EPHEMERIDES RESERVED FOR FUTURE JPL INTEGRATION"));

        foreach (var body in data.Bodies)
        {
            directory.Content(WorkshopGui.Element(receiver, "div")
                .Style("margin", ".2rem 0").Style("padding", ".35rem")
                .Style("border-left", $"3px solid {Cyan}")
                .Style("background", "rgba(255,255,255,.02)")
                .Content(WorkshopGui.Element(receiver, "div")
                    .Style("color", White).Style("font-size", ".43rem").Text(body.Name))
                .Content(WorkshopGui.Element(receiver, "div")
                    .Style("color", "#8299a4").Style("font-size", ".36rem")
                    .Text($"{body.DiameterKm:N0} km • {body.MassKg:E2} kg • {body.SurfaceGravityMS2:0.0} m/s² • {body.MeanDistanceFromSunMillionKm:0.0} Mkm")));
        }

        directory.Content(WorkshopGui.Element(receiver, "div")
            .Style("margin-top", ".5rem").Style("padding", ".4rem")
            .Style("border", $"1px solid {Cyan}33").Style("color", "#8fa7b2")
            .Style("font-size", ".35rem").Style("line-height", "1.5")
            .Text("NASA SOURCE: nssdc.gsfc.nasa.gov/planetary/factsheet/ • JPL HORIZONS: ssd.jpl.nasa.gov/horizons/"));

        return directory;
    }

    private static ElementBuilder MapFrame(object receiver, double zoom, Action<double>? setZoom)
    {
        var frame = WorkshopGui.Panel(receiver).Style("position", "relative").Style("min-width", "0")
            .Style("min-height", "0").Style("overflow", "hidden")
            .Style("border", $"1px solid {Cyan}55").Style("background", "#01060d")
            .Style("transform", $"scale({Math.Clamp(zoom, .75, 2.5):0.###})").Style("transform-origin", "center center");

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
            var index = 0;
            foreach (var item in group)
            {
                index++;
                var accent = Accent(item.ExperienceId);
                var category = group.Key == SpatialDomain.Workshop ? "WORKSHOP FACILITY" : "CITY DESTINATION";
                directory.Content(WorkshopGui.Button(receiver)
                    .Class("workshop-map-directory-item")
                    .Style("display", "grid").Style("grid-template-columns", "2rem minmax(0,1fr) auto")
                    .Style("align-items", "center").Style("gap", ".45rem")
                    .Style("width", "100%").Style("margin", ".22rem 0").Style("padding", ".42rem .45rem")
                    .Style("text-align", "left").Style("border", $"1px solid {accent}55").Style("border-left", $"4px solid {accent}")
                    .Style("background", "linear-gradient(90deg,rgba(255,255,255,.045),rgba(255,255,255,.012))")
                    .Style("color", White).Style("font-family", "inherit").Style("cursor", "pointer")
                    .Style("font-size", ".44rem").Style("font-weight", "700")
                    .AriaLabel($"Navigate to {item.Name}")
                    .Title(item.Place?.Description ?? item.Name)
                    .Content(WorkshopGui.Element(receiver, "span")
                        .Style("color", accent).Style("font-size", ".36rem").Style("letter-spacing", ".08em")
                        .Text($"{index:00}"))
                    .Content(WorkshopGui.Element(receiver, "span")
                        .Style("min-width", "0").Style("display", "flex").Style("flex-direction", "column").Style("gap", ".12rem")
                        .Content(WorkshopGui.Element(receiver, "span").Style("color", White).Text(item.Name.ToUpperInvariant()))
                        .Content(WorkshopGui.Element(receiver, "span").Style("color", "#708892").Style("font-size", ".32rem").Style("font-weight", "400")
                            .Text(category)))
                    .Content(WorkshopGui.Element(receiver, "span")
                        .Style("color", accent).Style("font-size", ".8rem").Text("↗"))
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

    private static string BuildingFill(string accent) => accent switch
    {
        Cyan => "linear-gradient(145deg,rgba(0,234,255,.14),rgba(0,234,255,.035) 58%,rgba(1,8,14,.72))",
        Yellow => "linear-gradient(145deg,rgba(255,211,77,.14),rgba(255,211,77,.035) 58%,rgba(10,8,2,.72))",
        Magenta => "linear-gradient(145deg,rgba(255,56,209,.14),rgba(255,56,209,.035) 58%,rgba(10,2,9,.72))",
        _ => "linear-gradient(145deg,rgba(82,224,90,.13),rgba(82,224,90,.035) 58%,rgba(2,10,5,.72))"
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
