using Microsoft.AspNetCore.Components.Web;

namespace TheSingularityWorkshop.Gui;

public enum SpatialAECElementKind
{
    Wall, Door, Window, Column, Beam, Equipment, Duct, Pipe, Conduit, Electrical, Annotation
}

public readonly record struct SpatialAECDraftElement(
    int Index,
    SpatialAECElementKind Kind,
    SpatialLine Line,
    double Length,
    double AngleDegrees,
    int Floor,
    double StartZ,
    double EndZ);

public sealed class SpatialAECDraftModel : IDisposable
{
    private readonly SpatialLinework _linework = new();
    private readonly List<SpatialAECDraftElement> _elements = [];
    private bool _disposed;

    public IReadOnlyList<SpatialAECDraftElement> Elements => _elements;
    public SpatialLinework Linework => _linework;
    public SpatialAECElementKind ElementKind { get; set; } = SpatialAECElementKind.Wall;
    public double Snap { get; set; } = 1d;
    public bool Orthogonal { get; set; } = true;
    public bool Drawing { get; private set; }
    public int SelectedFloor { get; set; } = 1;
    public string SelectedTrade { get; set; } = "architecture";
    public double CurrentElevation => SelectedFloor * 12d;
    public SpatialPoint? StartPoint { get; private set; }

    public void Begin(SpatialPoint point)
    {
        ThrowIfDisposed();
        var snapped = SnapPoint(point);
        _linework.BeginLine(snapped);
        StartPoint = snapped;
        Drawing = true;
    }

    public SpatialAECDraftElement End(SpatialPoint point)
    {
        ThrowIfDisposed();
        if (!Drawing || StartPoint is null)
            throw new InvalidOperationException("The AEC spatial console is not currently drawing.");

        var end = SnapPoint(point);
        if (Orthogonal) end = Orthogonalize(StartPoint.Value, end);

        _linework.ContinueLine(end);
        var line = _linework.EndLine();
        var dx = line.End.X - line.Start.X;
        var dy = line.End.Y - line.Start.Y;
        var length = Math.Sqrt(dx * dx + dy * dy);
        var angle = Math.Atan2(dy, dx) * 180d / Math.PI;

        var element = new SpatialAECDraftElement(
            _elements.Count + 1, ElementKind, line, length, angle,
            SelectedFloor, CurrentElevation, CurrentElevation);

        _elements.Add(element);
        Drawing = false;
        StartPoint = null;
        return element;
    }

    public void Clear()
    {
        ThrowIfDisposed();
        _elements.Clear();
        _linework.Clear();
        Drawing = false;
        StartPoint = null;
    }

    public void Undo()
    {
        ThrowIfDisposed();
        if (_elements.Count == 0) return;

        _elements.RemoveAt(_elements.Count - 1);
        _linework.Clear();
        foreach (var element in _elements)
        {
            _linework.BeginLine(element.Line.Start);
            _linework.ContinueLine(element.Line.End);
            _linework.EndLine();
        }

        var renumbered = _elements.Select((element, index) => element with { Index = index + 1 }).ToArray();
        _elements.Clear();
        _elements.AddRange(renumbered);
    }

    public SpatialPoint SnapPoint(SpatialPoint point)
    {
        var snap = Math.Max(.25d, Snap);
        return new SpatialPoint(
            Math.Round(point.X / snap) * snap,
            Math.Round(point.Y / snap) * snap);
    }

    private static SpatialPoint Orthogonalize(SpatialPoint start, SpatialPoint end)
        => Math.Abs(end.X - start.X) >= Math.Abs(end.Y - start.Y)
            ? new SpatialPoint(end.X, start.Y)
            : new SpatialPoint(start.X, end.Y);

    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(_disposed, this);

    public void Dispose()
    {
        if (_disposed) return;
        _linework.Dispose();
        _disposed = true;
        _elements.Clear();
    }
}

public static class SpatialAECRemoteOfficeGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Yellow = "#ffd34d";
    private const string Magenta = "#ff38d1";
    private const string White = "#ffffff";
    private const string Muted = "#7895a1";
    private const string Void = "#061018";

    public static ElementBuilder Build(
        object receiver,
        SpatialAECRemoteOfficeManifest manifest,
        SpatialAECDraftModel draft,
        string avatarName,
        Action<MouseEventArgs> drawPoint,
        Action clear,
        Action undo,
        Action<SpatialAECElementKind> setElementKind,
        Action<double> setSnap,
        Action<bool> setOrthogonal,
        Action<int> setFloor,
        Action<string> setTrade,
        Action exitOffice)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0")
            .Style("width", "100vw").Style("height", "100vh")
            .Style("overflow", "hidden")
            .Style("background", Void).Style("color", White)
            .Style("font-family", "Consolas, 'Courier New', monospace");

        root.Content(WorkshopGui.Element(receiver, "style").Text(
            "@keyframes spatial-breathe{0%,100%{opacity:.55}50%{opacity:1}}" +
            "@keyframes spatial-pulse{0%,100%{transform:scale(.9);opacity:.55}50%{transform:scale(1.15);opacity:1}}" +
            ".aec-floor{transition:all .2s ease}.aec-floor:hover{filter:brightness(1.7)}"));

        var canvas = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 100")
            .Attribute("preserveAspectRatio", "none")
            .Style("position", "absolute").Style("inset", "0")
            .Style("width", "100vw").Style("height", "100vh")
            .Style("cursor", draft.Drawing ? "crosshair" : "crosshair")
            .Style("background", "#061018")
            .AriaLabel("Three dimensional axonometric AEC authoring workspace. Click two points to place an element on the selected floor.")
            .OnClick(drawPoint).StopPropagation("onclick");

        DrawAxonometricGrid(canvas, receiver);

        foreach (var element in draft.Elements)
        {
            var a = Project(element.Line.Start.X, element.Line.Start.Y, element.StartZ);
            var b = Project(element.Line.End.X, element.Line.End.Y, element.EndZ);
            var stroke = ColorFor(element.Kind);

            canvas.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", a.X).Attribute("y1", a.Y)
                .Attribute("x2", b.X).Attribute("y2", b.Y)
                .Attribute("stroke", stroke)
                .Attribute("stroke-width", element.Kind == SpatialAECElementKind.Wall ? "0.72" : "0.38")
                .Attribute("stroke-opacity", ".95"));

            AddDimension(canvas, receiver, element, a, b);
        }

        foreach (var point in IntersectionPoints(draft.Elements))
        {
            var p = Project(point.X, point.Y, draft.CurrentElevation);
            canvas.Child(WorkshopGui.Element(receiver, "circle")
                .Attribute("cx", p.X).Attribute("cy", p.Y).Attribute("r", ".34")
                .Attribute("fill", Cyan).Attribute("stroke", Void).Attribute("stroke-width", ".12"));
        }

        if (draft.Drawing && draft.StartPoint is { } start)
        {
            var p = Project(start.X, start.Y, draft.CurrentElevation);
            canvas.Child(WorkshopGui.Element(receiver, "circle")
                .Attribute("cx", p.X).Attribute("cy", p.Y).Attribute("r", ".35")
                .Attribute("fill", Cyan).Attribute("fill-opacity", ".85"));
            canvas.Child(WorkshopGui.Element(receiver, "circle")
                .Attribute("cx", p.X).Attribute("cy", p.Y).Attribute("r", ".85")
                .Attribute("fill", "none").Attribute("stroke", Yellow)
                .Attribute("stroke-width", ".18").Attribute("stroke-dasharray", ".3 .3"));
        }

        root.Content(canvas);
        root.Content(Avatar(receiver, avatarName, draft));
        root.Content(TradeContext(receiver, manifest, draft, setElementKind, setSnap, setOrthogonal, clear, undo, exitOffice));
        root.Content(FloorNavigator(receiver, draft, setFloor, setTrade, manifest));
        root.Content(Status(receiver, draft));

        return root;
    }

    private static void DrawAxonometricGrid(ElementBuilder canvas, object receiver)
    {
        for (var i = 0d; i <= 100d; i += 5d)
        {
            var x0 = Project(i, 0, 0);
            var x1 = Project(i, 100, 0);
            var y0 = Project(0, i, 0);
            var y1 = Project(100, i, 0);

            canvas.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", x0.X).Attribute("y1", x0.Y)
                .Attribute("x2", x1.X).Attribute("y2", x1.Y)
                .Attribute("stroke", Cyan).Attribute("stroke-opacity", i % 10 == 0 ? ".16" : ".055").Attribute("stroke-width", ".12"));
            canvas.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", y0.X).Attribute("y1", y0.Y)
                .Attribute("x2", y1.X).Attribute("y2", y1.Y)
                .Attribute("stroke", Cyan).Attribute("stroke-opacity", i % 10 == 0 ? ".16" : ".055").Attribute("stroke-width", ".12"));
        }

        for (var floor = 1; floor <= 5; floor++)
        {
            var left = Project(0, 0, floor * 12);
            var right = Project(100, 0, floor * 12);
            canvas.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", left.X).Attribute("y1", left.Y)
                .Attribute("x2", right.X).Attribute("y2", right.Y)
                .Attribute("stroke", Yellow).Attribute("stroke-opacity", ".06").Attribute("stroke-width", ".18"));
        }
    }

    private static ElementBuilder TradeContext(
        object receiver,
        SpatialAECRemoteOfficeManifest manifest,
        SpatialAECDraftModel draft,
        Action<SpatialAECElementKind> setElementKind,
        Action<double> setSnap,
        Action<bool> setOrthogonal,
        Action clear,
        Action undo,
        Action exitOffice)
    {
        var trade = manifest.FindDiscipline(draft.SelectedTrade);
        var panel = WorkshopGui.Element(receiver, "div")
            .Style("position", "fixed").Style("left", "1rem").Style("top", "1rem").Style("z-index", "30")
            .Style("width", "min(25rem,calc(100vw - 2rem))")
            .Style("padding", ".75rem")
            .Style("border", $"1px solid {Cyan}66")
            .Style("background", "rgba(2,10,17,.86)")
            .Style("backdrop-filter", "blur(8px)")
            .Style("box-shadow", $"0 0 28px {Cyan}12");

        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("color", Cyan).Style("font-size", ".62rem").Style("letter-spacing", ".18em")
            .Text($"{draft.SelectedTrade.ToUpperInvariant()} // FLOOR {draft.SelectedFloor}"));

        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("margin-top", ".22rem").Style("color", Muted).Style("font-size", ".38rem")
            .Text($"ACTIVE CONSTRUCTION ZONE // ELEVATION {draft.CurrentElevation:0.#}"));

        if (trade is not null)
            panel.Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".42rem").Style("color", White).Style("font-size", ".39rem")
                .Text(string.Join("  /  ", trade.Value.Concepts.Select(x => x.ToUpperInvariant()))));

        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("margin-top", ".62rem").Style("color", Yellow).Style("font-size", ".4rem")
            .Style("letter-spacing", ".12em").Text("ACTIVE ELEMENT BUNDLES"));

        var tools = WorkshopGui.Element(receiver, "div")
            .Style("display", "flex").Style("flex-wrap", "wrap").Style("gap", ".28rem").Style("margin-top", ".3rem");

        foreach (var kind in TradeKinds(draft.SelectedTrade))
        {
            var active = draft.ElementKind == kind;
            tools.Content(WorkshopGui.Button(receiver).Label(kind.ToString().ToUpperInvariant())
                .Style("padding", ".32rem .42rem")
                .Style("border", $"1px solid {(active ? Cyan : "#ffffff")}55")
                .Style("background", active ? $"{Cyan}18" : "transparent")
                .Style("color", active ? Cyan : White).Style("font-family", "inherit").Style("font-size", ".36rem")
                .Style("cursor", "pointer").OnClick(() => setElementKind(kind)));
        }

        panel.Content(tools);

        var controls = WorkshopGui.Element(receiver, "div").Style("display", "flex").Style("gap", ".25rem").Style("margin-top", ".5rem");
        foreach (var value in new[] { .5d, 1d, 2d, 5d })
            controls.Content(WorkshopGui.Button(receiver).Label($"SNAP {value:0.0}")
                .Style("padding", ".25rem .35rem").Style("font-size", ".34rem").Style("cursor", "pointer")
                .OnClick(() => setSnap(value)));

        controls.Content(WorkshopGui.Button(receiver).Label(draft.Orthogonal ? "ORTHO" : "FREE")
            .Style("padding", ".25rem .35rem").Style("font-size", ".34rem").Style("cursor", "pointer")
            .OnClick(() => setOrthogonal(!draft.Orthogonal)));

        panel.Content(controls);

        panel.Content(WorkshopGui.Button(receiver).Label("UNDO")
            .Style("margin-top", ".4rem").Style("padding", ".32rem .45rem").Style("font-size", ".35rem").Style("cursor", "pointer").OnClick(undo));
        panel.Content(WorkshopGui.Button(receiver).Label("CLEAR")
            .Style("margin-left", ".25rem").Style("padding", ".32rem .45rem").Style("font-size", ".35rem").Style("cursor", "pointer").OnClick(clear));
        panel.Content(WorkshopGui.Button(receiver).Label("← LEAVE AEC OFFICE")
            .Style("margin-left", ".25rem").Style("padding", ".32rem .45rem").Style("font-size", ".35rem")
            .Style("color", Magenta).Style("cursor", "pointer").OnClick(exitOffice));

        return panel;
    }

    private static IReadOnlyList<SpatialAECElementKind> TradeKinds(string trade)
        => trade.ToLowerInvariant() switch
        {
            "structural" => [SpatialAECElementKind.Column, SpatialAECElementKind.Beam, SpatialAECElementKind.Wall],
            "mechanical" => [SpatialAECElementKind.Duct, SpatialAECElementKind.Equipment, SpatialAECElementKind.Pipe],
            "electrical" => [SpatialAECElementKind.Conduit, SpatialAECElementKind.Electrical, SpatialAECElementKind.Equipment],
            "plumbing" => [SpatialAECElementKind.Pipe, SpatialAECElementKind.Equipment, SpatialAECElementKind.Wall],
            "procurement" => [SpatialAECElementKind.Equipment, SpatialAECElementKind.Annotation],
            _ => [SpatialAECElementKind.Wall, SpatialAECElementKind.Door, SpatialAECElementKind.Window, SpatialAECElementKind.Column]
        };

    private static ElementBuilder FloorNavigator(
        object receiver,
        SpatialAECDraftModel draft,
        Action<int> setFloor,
        Action<string> setTrade,
        SpatialAECRemoteOfficeManifest manifest)
    {
        var nav = WorkshopGui.Element(receiver, "div")
            .Style("position", "fixed").Style("right", "1rem").Style("top", "1rem").Style("z-index", "31")
            .Style("width", "min(24rem,34vw)").Style("min-width", "15rem")
            .Style("padding", ".65rem")
            .Style("border", $"1px solid {Yellow}55")
            .Style("background", "rgba(2,10,17,.55)")
            .Style("backdrop-filter", "blur(6px)")
            .Style("perspective", "800px");

        nav.Content(WorkshopGui.Element(receiver, "div").Style("color", Yellow).Style("font-size", ".42rem").Style("letter-spacing", ".15em")
            .Text("SPATIAL NAVIGATOR // BUILDING SECTION"));

        var building = WorkshopGui.Element(receiver, "div")
            .Style("position", "relative").Style("height", "16rem").Style("margin-top", ".25rem")
            .Style("transform", "rotateX(8deg) rotateY(-18deg)")
            .Style("transform-style", "preserve-3d");

        for (var floor = 5; floor >= 1; floor--)
        {
            var y = 5 + (5 - floor) * 18;
            var selected = draft.SelectedFloor == floor;
            var level = WorkshopGui.Button(receiver).Label($"FLOOR {floor}")
                .Style("position", "absolute").Style("left", "8%").Style("top", $"{y}%")
                .Style("width", "84%").Style("height", "15%")
                .Style("border", $"1px solid {(selected ? Cyan : "#ffffff")}88")
                .Style("background", selected ? $"{Cyan}18" : "rgba(0,234,255,.025)")
                .Style("color", selected ? Cyan : Muted)
                .Style("font-family", "inherit").Style("font-size", ".4rem").Style("text-align", "left")
                .Style("padding-left", ".55rem").Style("cursor", "pointer")
                .Style("transform", $"translateZ({(5 - floor) * 3}px)")
                .Style("box-shadow", selected ? $"0 0 16px {Cyan}22" : "none")
                .OnClick(() => setFloor(floor));
            building.Content(level);
        }

        nav.Content(building);

        var trades = WorkshopGui.Element(receiver, "div").Style("display", "flex").Style("flex-wrap", "wrap").Style("gap", ".25rem");
        foreach (var discipline in manifest.Disciplines)
        {
            var selected = string.Equals(draft.SelectedTrade, discipline.Id, StringComparison.OrdinalIgnoreCase);
            trades.Content(WorkshopGui.Button(receiver).Label(discipline.Name)
                .Style("padding", ".25rem .35rem").Style("font-size", ".32rem")
                .Style("border", $"1px solid {(selected ? Yellow : "#ffffff")}44")
                .Style("color", selected ? Yellow : Muted).Style("cursor", "pointer")
                .OnClick(() => setTrade(discipline.Id)));
        }
        nav.Content(trades);

        nav.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".25rem").Style("color", Muted).Style("font-size", ".32rem")
            .Text($"SELECTED FLOOR {draft.SelectedFloor} // {draft.SelectedTrade.ToUpperInvariant()} CONTEXT"));
        return nav;
    }

    private static ElementBuilder Status(object receiver, SpatialAECDraftModel draft)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "fixed").Style("left", "50%").Style("bottom", "1rem").Style("z-index", "25")
            .Style("transform", "translateX(-50%)")
            .Style("padding", ".38rem .65rem").Style("border", $"1px solid {Cyan}33")
            .Style("background", "rgba(2,10,17,.72)").Style("color", Muted)
            .Style("font-size", ".34rem").Style("letter-spacing", ".1em").Style("pointer-events", "none")
            .Text(draft.Drawing
                ? $"PLACING {draft.ElementKind.ToString().ToUpperInvariant()} // CLICK ENDPOINT // X Y Z"
                : $"FLOOR {draft.SelectedFloor} // Z {draft.CurrentElevation:0.#} // CLICK TWO POINTS TO AUTHOR");

    private static void AddDimension(ElementBuilder canvas, object receiver, SpatialAECDraftElement element, (double X, double Y) a, (double X, double Y) b)
    {
        var x = (a.X + b.X) / 2d;
        var y = (a.Y + b.Y) / 2d - 1.1d;
        canvas.Child(WorkshopGui.Element(receiver, "text")
            .Attribute("x", x).Attribute("y", y)
            .Attribute("fill", Yellow).Attribute("font-size", "1.4")
            .Attribute("text-anchor", "middle")
            .Attribute("paint-order", "stroke").Attribute("stroke", Void).Attribute("stroke-width", ".45")
            .Text($"{element.Length:0.##}"));
    }

    private static IEnumerable<SpatialPoint> IntersectionPoints(IReadOnlyList<SpatialAECDraftElement> elements)
    {
        var points = new List<SpatialPoint>();
        for (var i = 0; i < elements.Count; i++)
        for (var j = i + 1; j < elements.Count; j++)
            if (elements[i].Floor == elements[j].Floor && TryIntersect(elements[i].Line, elements[j].Line, out var point))
                points.Add(point);

        return points.GroupBy(p => $"{p.X:R}|{p.Y:R}").Select(g => g.First());
    }

    private static bool TryIntersect(SpatialLine first, SpatialLine second, out SpatialPoint point)
    {
        var x1 = first.Start.X; var y1 = first.Start.Y;
        var x2 = first.End.X; var y2 = first.End.Y;
        var x3 = second.Start.X; var y3 = second.Start.Y;
        var x4 = second.End.X; var y4 = second.End.Y;
        var denominator = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);

        if (Math.Abs(denominator) < 1e-9)
        {
            point = default;
            return false;
        }

        var t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / denominator;
        var u = -((x1 - x2) * (y1 - y3) - (y1 - y2) * (x1 - x3)) / denominator;

        if (t is < 0d or > 1d || u is < 0d or > 1d)
        {
            point = default;
            return false;
        }

        point = new SpatialPoint(x1 + t * (x2 - x1), y1 + t * (y2 - y1));
        return true;
    }

    private static (double X, double Y) Project(double x, double y, double z)
        => (50d + (x - y) * .55d, 50d + (x + y) * .24d - z * .08d);

    private static string ColorFor(SpatialAECElementKind kind)
        => kind switch
        {
            SpatialAECElementKind.Wall => Cyan,
            SpatialAECElementKind.Door => Yellow,
            SpatialAECElementKind.Window => White,
            SpatialAECElementKind.Column => Magenta,
            SpatialAECElementKind.Beam => Magenta,
            SpatialAECElementKind.Equipment => "#ff8a3d",
            SpatialAECElementKind.Duct => "#7df9ff",
            SpatialAECElementKind.Pipe => "#4dff88",
            SpatialAECElementKind.Conduit => "#c084fc",
            SpatialAECElementKind.Electrical => Yellow,
            SpatialAECElementKind.Annotation => White,
            _ => White
        };

    private static ElementBuilder Avatar(object receiver, string name, SpatialAECDraftModel draft)
    {
        var p = Project(50, 50, draft.CurrentElevation);
        return WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", $"{p.X}%").Style("top", $"{p.Y}%").Style("z-index", "20")
            .Style("transform", "translate(-50%,-50%)").Style("pointer-events", "none")
            .Style("text-align", "center").Style("color", White).Style("font-size", ".38rem")
            .Content(WorkshopGui.Element(receiver, "div").Style("color", Cyan).Style("font-size", "1rem").Style("text-shadow", $"0 0 12px {Cyan}").Text("◉"))
            .Content(WorkshopGui.Element(receiver, "div").Style("color", Cyan).Text($"YOU ARE HERE // {name}"))
            .Content(WorkshopGui.Element(receiver, "div").Style("color", Muted).Style("font-size", ".3rem").Text($"X 50.0  Y 50.0  Z {draft.CurrentElevation:0.#}"));
    }
}
