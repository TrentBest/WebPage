using Microsoft.AspNetCore.Components.Web;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Manifests the current Singularity City catalog through the Workshop GUI primitives.
/// This is a WebPage renderer over existing spatial data; publication/provider composition
/// is intentionally kept separate until the city renderer receives its canonical bundle contract.
/// </summary>
public static class SingularityCityGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Magenta = "#ff2cff";
    private const string Gold = "#ffd34d";
    private const string Mint = "#58e8c1";
    private const string White = "#f1fbff";

    public static ElementBuilder Build(
        object receiver,
        IReadOnlyList<SpatialCityStructure> structures,
        IReadOnlyList<SpatialDigiten> digitens,
        double avatarX,
        double avatarY,
        double zoom,
        string avatarLabel,
        string? selectedStructureId,
        string status,
        Action<KeyboardEventArgs> onKeyDown,
        Func<MouseEventArgs, Task> onWorldClick,
        Func<string, Task> selectStructure,
        Action<double, double> moveAvatarBy,
        Action<double, double> moveSelectedStructure,
        Action changeSelectedKind,
        Action resetDraft,
        Action<double> setZoom)
    {
        var camera = new SpatialCamera(avatarX, avatarY, zoom).Clamped();
        var selected = structures.FirstOrDefault(x => x.Id == selectedStructureId);

        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0")
            .Style("width", "100vw").Style("height", "100vh")
            .Style("overflow", "hidden").Style("background", "#030812")
            .Style("color", White).Style("font-family", "Consolas,'Courier New',monospace")
            .Attribute("id", "singularity-city-experience");

        root.Content(Styles(receiver));
        root.Content(World(receiver, structures, digitens, camera, onKeyDown, onWorldClick, selectStructure, selectedStructureId));
        root.Content(AvatarMarker(receiver, avatarLabel));
        root.Content(Header(receiver, status, structures.Count, digitens.Count));
        root.Content(SelectedPanel(receiver, selected, selectedStructureId, moveSelectedStructure, changeSelectedKind, resetDraft));
        root.Content(NavigationControls(receiver, moveAvatarBy, setZoom, camera.Zoom));
        return root;
    }

    private static ElementBuilder Styles(object receiver)
        => WorkshopGui.Element(receiver, "style").Text(
            ".city-world{outline:none;cursor:crosshair}" +
            ".city-structure{cursor:pointer;transition:filter .18s ease}" +
            ".city-structure:hover,.city-structure:focus-visible{filter:drop-shadow(0 0 1.15px #00eaff);outline:none}" +
            ".city-structure.selected{filter:drop-shadow(0 0 1.5px #ff2cff)}" +
            ".city-structure rect{vector-effect:non-scaling-stroke;transition:stroke-width .18s ease}" +
            ".city-world-title,.city-ui-panel{border:1px solid #23475b;background:rgba(3,10,20,.91);box-shadow:0 8px 30px #0005;backdrop-filter:blur(8px)}" +
            ".city-ui-panel{padding:.8rem .9rem;max-width:min(22rem,calc(100vw - 1.2rem))}" +
            ".city-kicker{font-size:.58rem;letter-spacing:.17em;color:#00eaff;font-weight:800}" +
            ".city-muted{font:400 .72rem/1.5 system-ui,sans-serif;color:#9bb5c2}" +
            ".city-action-row{display:flex;gap:.35rem;flex-wrap:wrap;margin-top:.6rem}" +
            ".city-action{font:700 .58rem Consolas,monospace;letter-spacing:.07em;color:#eaf8fd;background:#0a1b2b;border:1px solid #286078;padding:.55rem .6rem;cursor:pointer}" +
            ".city-action:hover,.city-action:focus-visible{border-color:#ff2cff;outline:2px solid #ff2cff44}" +
            ".city-status{font-size:.6rem;letter-spacing:.08em;color:#58e8c1}" +
            ".city-pulse{animation:cityPulse 1.8s ease-in-out infinite}" +
            "@keyframes cityPulse{0%,100%{opacity:.35;transform:scale(.75)}50%{opacity:.85;transform:scale(1.2)}}" +
            "@media(max-width:720px){.city-world-title{max-width:calc(100vw - 1rem)}.city-right-panel{top:auto!important;bottom:7rem!important;max-height:26vh;overflow:auto}.city-controls{bottom:.5rem!important;left:.5rem!important}.city-muted{font-size:.66rem}}" +
            "@media(prefers-reduced-motion:reduce){.city-structure,.city-structure rect,.city-pulse{animation:none!important;transition:none!important}}" );

    private static ElementBuilder World(
        object receiver,
        IReadOnlyList<SpatialCityStructure> structures,
        IReadOnlyList<SpatialDigiten> digitens,
        SpatialCamera camera,
        Action<KeyboardEventArgs> onKeyDown,
        Func<MouseEventArgs, Task> onWorldClick,
        Func<string, Task> selectStructure,
        string? selectedStructureId)
    {
        var world = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("inset", "0")
            .Style("overflow", "hidden").Style("outline", "none")
            .Attribute("tabindex", "0").Attribute("autofocus", "autofocus")
            .Attribute("class", "city-world")
            .AriaLabel("Singularity City world map. Use arrow keys or WASD to walk; click open ground to travel and select a building to approach it.")
            .OnKeyDown(onKeyDown).OnClick(args => _ = onWorldClick(args))
            .PreventDefault("onkeydown");

        var plane = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("left", "0").Style("top", "0")
            .Style("width", $"{SpatialCamera.WorldWidthVw * camera.Zoom:0.###}vw")
            .Style("height", $"{SpatialCamera.WorldHeightVh * camera.Zoom:0.###}vh")
            .Style("transform", $"translate({camera.OffsetVw:0.###}vw,{camera.OffsetVh:0.###}vh)")
            .Style("transform-origin", "0 0").Style("transition", "transform .12s linear");

        plane.Content(Grid(receiver, camera.Zoom));
        plane.Content(WorkshopBoundary(receiver));
        foreach (var structure in structures)
            plane.Content(Structure(receiver, structure, camera.Zoom, selectedStructureId, selectStructure));
        foreach (var digiten in digitens)
            plane.Content(DigitenMarker(receiver, digiten, camera.Zoom));
        world.Content(plane);
        return world;
    }

    private static ElementBuilder Grid(object receiver, double zoom)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 100").Attribute("preserveAspectRatio", "none")
            .Style("position", "absolute").Style("inset", "0")
            .Style("width", "100%").Style("height", "100%")
            .Style("pointer-events", "none").Style("opacity", ".8");
        for (var p = 0; p <= 100; p += 5)
        {
            svg.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", p).Attribute("y1", 0).Attribute("x2", p).Attribute("y2", 100)
                .Attribute("stroke", Cyan).Attribute("stroke-opacity", p % 10 == 0 ? ".16" : ".07").Attribute("stroke-width", ".12"));
            svg.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", 0).Attribute("y1", p).Attribute("x2", 100).Attribute("y2", p)
                .Attribute("stroke", Cyan).Attribute("stroke-opacity", p % 10 == 0 ? ".16" : ".07").Attribute("stroke-width", ".12"));
        }
        return svg;
    }

    private static ElementBuilder WorkshopBoundary(object receiver)
    {
        var b = SingularityCityCatalog.WorkshopDistrict;
        return WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 100").Attribute("preserveAspectRatio", "none")
            .Style("position", "absolute").Style("inset", "0")
            .Style("width", "100%").Style("height", "100%")
            .Style("pointer-events", "none")
            .Child(WorkshopGui.Element(receiver, "rect")
                .Attribute("x", b.X).Attribute("y", b.Y).Attribute("width", b.Width).Attribute("height", b.Height)
                .Attribute("fill", "#00eaff").Attribute("fill-opacity", ".035")
                .Attribute("stroke", Cyan).Attribute("stroke-width", ".45").Attribute("stroke-dasharray", "1.2 .6"))
            .Child(WorkshopGui.Element(receiver, "text")
                .Attribute("x", b.X + b.Width / 2).Attribute("y", b.Y + b.Height / 2)
                .Attribute("text-anchor", "middle").Attribute("font-size", "2.2")
                .Attribute("font-family", "monospace").Attribute("font-weight", "800").Attribute("fill", White)
                .Text("SINGULARITY WORKSHOP"))
            .Child(WorkshopGui.Element(receiver, "text")
                .Attribute("x", b.X + b.Width / 2).Attribute("y", b.Y + b.Height / 2 + 3)
                .Attribute("text-anchor", "middle").Attribute("font-size", "1.25")
                .Attribute("font-family", "monospace").Attribute("fill", Mint)
                .Text("FIXED CITY ANCHOR"));
    }

    private static ElementBuilder Structure(
        object receiver, SpatialCityStructure structure, double zoom, string? selectedId, Func<string, Task> selectStructure)
    {
        var b = structure.Bounds;
        var accent = Accent(structure.Kind);
        var selected = string.Equals(structure.Id, selectedId, StringComparison.Ordinal);
        var label = Abbreviate(structure.Name, b.Width);
        var group = WorkshopGui.Element(receiver, "g")
            .Attribute("class", selected ? "city-structure selected" : "city-structure")
            .Attribute("data-structure-id", structure.Id)
            .Attribute("tabindex", "0").Attribute("role", "button")
            .Attribute("aria-label", $"Approach {structure.Name}. {structure.Kind}. {structure.FutureFunction}")
            .Attribute("transform", $"translate({b.X:0.##} {b.Y:0.##})")
            .Title($"{structure.Name} — {structure.FutureFunction}")
            .OnClick(() => _ = selectStructure(structure.Id))
            .OnKeyDown(args => { if (args.Key is "Enter" or " ") _ = selectStructure(structure.Id); })
            .StopPropagation("onclick").StopPropagation("onkeydown");
        group.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("width", b.Width).Attribute("height", b.Height).Attribute("rx", ".65")
            .Attribute("fill", accent).Attribute("fill-opacity", selected ? ".30" : ".16")
            .Attribute("stroke", selected ? Magenta : accent).Attribute("stroke-width", selected ? ".65" : ".3"));
        group.Child(WorkshopGui.Element(receiver, "path")
            .Attribute("d", $"M1 1H{Math.Max(1, b.Width - 1):0.##}V{Math.Max(1, b.Height - 1):0.##}H1Z")
            .Attribute("fill", "none").Attribute("stroke", accent).Attribute("stroke-opacity", ".48").Attribute("stroke-width", ".16"));
        group.Child(WorkshopGui.Element(receiver, "text")
            .Attribute("x", b.Width / 2).Attribute("y", b.Height / 2 - .5)
            .Attribute("text-anchor", "middle").Attribute("font-size", Math.Clamp(b.Width / 8.5, .85, 1.6))
            .Attribute("font-weight", "800").Attribute("font-family", "monospace").Attribute("fill", White)
            .Attribute("pointer-events", "none").Text(label));
        group.Child(WorkshopGui.Element(receiver, "text")
            .Attribute("x", b.Width / 2).Attribute("y", b.Height / 2 + 1.8)
            .Attribute("text-anchor", "middle").Attribute("font-size", Math.Clamp(b.Width / 13, .65, 1.0))
            .Attribute("font-family", "monospace").Attribute("fill", accent)
            .Attribute("pointer-events", "none").Text(structure.Kind.ToString().ToUpperInvariant()));
        return group;
    }

    private static ElementBuilder DigitenMarker(object receiver, SpatialDigiten digiten, double zoom)
    {
        var p = digiten.Position;
        var colour = digiten.Id switch { "digiten-01" => Gold, "digiten-02" => Mint, "digiten-03" => Magenta, _ => Cyan };
        return WorkshopGui.Element(receiver, "g")
            .Attribute("class", "city-digiten")
            .Attribute("transform", $"translate({p.X:0.##} {p.Y:0.##})")
            .Attribute("aria-label", $"Digiten {digiten.Name}. Goal: {digiten.Goal.Description}")
            .Attribute("pointer-events", "none")
            .Child(WorkshopGui.Element(receiver, "circle").Attribute("r", ".52").Attribute("fill", colour).Attribute("stroke", White).Attribute("stroke-width", ".18"))
            .Child(WorkshopGui.Element(receiver, "circle").Attribute("r", ".95").Attribute("fill", "none").Attribute("stroke", colour).Attribute("stroke-opacity", ".6").Attribute("stroke-width", ".14"))
            .Child(WorkshopGui.Element(receiver, "text").Attribute("x", ".9").Attribute("y", ".2")
                .Attribute("font-size", "1.25").Attribute("font-weight", "800").Attribute("font-family", "monospace")
                .Attribute("fill", White).Text(digiten.Name));
    }

    private static ElementBuilder AvatarMarker(object receiver, string avatarLabel)
        => WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("left", "50%").Style("top", "50%")
            .Style("transform", "translate(-50%,-50%)").Style("z-index", "30")
            .Style("pointer-events", "none").Style("display", "flex")
            .Style("flex-direction", "column").Style("align-items", "center").Style("gap", ".2rem")
            .Attribute("aria-label", "You are here")
            .Content(WorkshopGui.Element(receiver, "div").Style("width", "1.1rem").Style("height", "1.1rem")
                .Style("border-radius", "50%").Style("border", $"2px solid {White}").Style("background", Magenta)
                .Style("box-shadow", $"0 0 16px {Magenta}").Style("position", "relative")
                .Content(WorkshopGui.Element(receiver, "span").Attribute("class", "city-pulse")
                    .Style("position", "absolute").Style("inset", "-6px").Style("border", $"1px solid {Magenta}")
                    .Style("border-radius", "50%")))
            .Content(WorkshopGui.Element(receiver, "div").Style("padding", ".15rem .35rem")
                .Style("border", $"1px solid {Magenta}88").Style("background", "rgba(3,10,20,.92)")
                .Style("color", White).Style("font-size", ".6rem").Style("font-weight", "800")
                .Style("letter-spacing", ".08em").Style("white-space", "nowrap")
                .Text($"YOU ARE HERE · {avatarLabel.ToUpperInvariant()}"));

    private static ElementBuilder Header(object receiver, string status, int buildingCount, int digitenCount)
        => WorkshopGui.Panel(receiver).Attribute("class", "city-world-title")
            .Style("position", "fixed").Style("left", ".8rem").Style("top", ".8rem")
            .Style("z-index", "20").Style("padding", ".7rem .9rem").Style("max-width", "min(28rem,calc(100vw - 1.6rem))")
            .Content(WorkshopGui.Element(receiver, "div").Attribute("class", "city-kicker").Text("THE SINGULARITY WORKSHOP · CITY EXPERIENCE"))
            .Content(WorkshopGui.Element(receiver, "h1").Style("font-size", "clamp(1.1rem,2vw,1.6rem)").Style("margin", ".2rem 0")
                .Style("letter-spacing", ".06em").Text("SINGULARITY CITY"))
            .Content(WorkshopGui.Element(receiver, "div").Attribute("class", "city-muted")
                .Text($"100 × 100 catalog world · {buildingCount} catalog structures · {digitenCount} source-catalog Digitens"))
            .Content(WorkshopGui.Element(receiver, "div").Attribute("class", "city-status").Style("margin-top", ".35rem").Text(status));

    private static ElementBuilder SelectedPanel(
        object receiver,
        SpatialCityStructure selected,
        string? selectedId,
        Action<double, double> moveSelectedStructure,
        Action changeSelectedKind,
        Action resetDraft)
    {
        var hasSelected = !string.IsNullOrWhiteSpace(selectedId) && structuresContain(selected.Id);
        // The default record is not a real selected structure; the ID guard keeps its copy out of the UI.
        if (!hasSelected)
            return WorkshopGui.Panel(receiver).Style("display", "none");

        return WorkshopGui.Panel(receiver).Attribute("class", "city-ui-panel city-right-panel")
            .Style("position", "fixed").Style("right", ".8rem").Style("top", ".8rem")
            .Style("z-index", "20").Style("width", "min(22rem,calc(100vw - 1.6rem))")
            .Content(WorkshopGui.Element(receiver, "div").Attribute("class", "city-kicker").Text("SELECTED STRUCTURE · LOCAL DRAFT"))
            .Content(WorkshopGui.Element(receiver, "h2").Style("margin", ".3rem 0").Style("font-size", "1rem").Text(selected.Name))
            .Content(WorkshopGui.Element(receiver, "div").Style("font-size", ".62rem").Style("letter-spacing", ".1em").Style("color", Accent(selected.Kind))
                .Text($"{selected.Kind.ToString().ToUpperInvariant()} · FOOTPRINT {selected.Bounds.Width:0.#} × {selected.Bounds.Height:0.#}"))
            .Content(WorkshopGui.Element(receiver, "p").Attribute("class", "city-muted").Text(selected.FutureFunction))
            .Content(WorkshopGui.Element(receiver, "div").Attribute("class", "city-action-row")
                .Content(Action(receiver, "TYPE NEXT", changeSelectedKind))
                .Content(Action(receiver, "RESET LAYOUT", resetDraft)))
            .Content(WorkshopGui.Element(receiver, "div").Attribute("class", "city-kicker").Style("margin-top", ".8rem").Text("MOVE SELECTED FOOTPRINT"))
            .Content(WorkshopGui.Element(receiver, "div").Attribute("class", "city-action-row")
                .Content(Action(receiver, "↑ NORTH", () => moveSelectedStructure(0, -2)))
                .Content(Action(receiver, "← WEST", () => moveSelectedStructure(-2, 0)))
                .Content(Action(receiver, "EAST →", () => moveSelectedStructure(2, 0)))
                .Content(Action(receiver, "↓ SOUTH", () => moveSelectedStructure(0, 2))))
            .Content(WorkshopGui.Element(receiver, "p").Attribute("class", "city-muted").Style("margin-bottom", "0")
                .Text("Changes are browser-local proposals. The source catalog, published bundles, and live world are unchanged."));

        bool structuresContain(string id) => !string.IsNullOrWhiteSpace(id) && id == selected.Id;
    }

    private static ElementBuilder NavigationControls(
        object receiver, Action<double, double> moveAvatarBy, Action<double> setZoom, double zoom)
        => WorkshopGui.Panel(receiver).Attribute("class", "city-ui-panel city-controls")
            .Style("position", "fixed").Style("left", ".8rem").Style("bottom", ".8rem")
            .Style("z-index", "20").Style("width", "14rem")
            .Content(WorkshopGui.Element(receiver, "div").Attribute("class", "city-kicker").Text("MOVE · CLICK OR WASD"))
            .Content(WorkshopGui.Element(receiver, "div").Attribute("class", "city-action-row")
                .Style("justify-content", "center")
                .Content(Action(receiver, "↑", () => moveAvatarBy(0, -1)))
                .Content(Action(receiver, "←", () => moveAvatarBy(-1, 0)))
                .Content(Action(receiver, "→", () => moveAvatarBy(1, 0)))
                .Content(Action(receiver, "↓", () => moveAvatarBy(0, 1))))
            .Content(WorkshopGui.Element(receiver, "div").Attribute("class", "city-action-row")
                .Content(Action(receiver, "ZOOM −", () => setZoom(Math.Max(SpatialCamera.MinZoom, zoom / 1.15))))
                .Content(Action(receiver, "ZOOM +", () => setZoom(Math.Min(SpatialCamera.MaxZoom, zoom * 1.15)))));

    private static ElementBuilder Action(object receiver, string label, Action onClick)
        => WorkshopGui.Button(receiver).Label(label).Style("font-family", "inherit")
            .Style("padding", ".55rem .6rem").Style("font-size", ".58rem")
            .Style("letter-spacing", ".06em").Style("border", "1px solid #286078")
            .Style("background", "#081827").Style("color", White).Style("cursor", "pointer")
            .OnClick(onClick);

    private static string Accent(SpatialCityStructureKind kind) => kind switch
    {
        SpatialCityStructureKind.Civic => Gold,
        SpatialCityStructureKind.Medical => "#ff8d8d",
        SpatialCityStructureKind.Education => Cyan,
        SpatialCityStructureKind.Commerce => Magenta,
        SpatialCityStructureKind.Residential => Mint,
        SpatialCityStructureKind.Culture => "#d0a0ff",
        SpatialCityStructureKind.Hospitality => Gold,
        SpatialCityStructureKind.Transport => Gold,
        SpatialCityStructureKind.Industry => "#e39a63",
        SpatialCityStructureKind.Technology => Cyan,
        SpatialCityStructureKind.Science => Cyan,
        SpatialCityStructureKind.Port => Cyan,
        SpatialCityStructureKind.Utility => "#e39a63",
        SpatialCityStructureKind.Park => Mint,
        _ => White
    };

    private static string Abbreviate(string name, double width)
    {
        var maxChars = Math.Max(4, (int)Math.Floor(width * 1.2));
        return name.Length <= maxChars ? name.ToUpperInvariant() : name[..Math.Max(2, maxChars - 1)].ToUpperInvariant() + "…";
    }
}