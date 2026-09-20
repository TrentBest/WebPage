using Microsoft.AspNetCore.Components.Web;

namespace TheSingularityWorkshop.Gui;

/// <summary>Semantic AEC element bundles exposed by the remote office drafting table.</summary>
public enum SpatialAECElementKind
{
    Wall,
    Door,
    Window,
    Column,
    Beam,
    Equipment,
    Duct,
    Pipe,
    Conduit,
    Electrical,
    Annotation
}

/// <summary>A semantic element captured by the AEC drafting surface.</summary>
public readonly record struct SpatialAECDraftElement(
    int Index,
    SpatialAECElementKind Kind,
    SpatialLine Line,
    double Length,
    double AngleDegrees);

/// <summary>
/// Mutable drawing state for the remote AEC office. Geometry remains platform-neutral;
/// the GUI is only a perception and authoring surface over the semantic substrate.
/// </summary>
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
            throw new InvalidOperationException("The AEC drafting table is not currently drawing.");

        var end = SnapPoint(point);
        if (Orthogonal)
            end = Orthogonalize(StartPoint.Value, end);

        _linework.ContinueLine(end);
        var line = _linework.EndLine();
        var dx = line.End.X - line.Start.X;
        var dy = line.End.Y - line.Start.Y;
        var length = Math.Sqrt(dx * dx + dy * dy);
        var angle = Math.Atan2(dy, dx) * 180d / Math.PI;
        var element = new SpatialAECDraftElement(_elements.Count + 1, ElementKind, line, length, angle);
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
        for (var i = 0; i < _elements.Count; i++)
        {
            var line = _elements[i].Line;
            _linework.BeginLine(line.Start);
            _linework.ContinueLine(line.End);
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
    {
        var dx = Math.Abs(end.X - start.X);
        var dy = Math.Abs(end.Y - start.Y);
        return dx >= dy
            ? new SpatialPoint(end.X, start.Y)
            : new SpatialPoint(start.X, end.Y);
    }

    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(_disposed, this);

    public void Dispose()
    {
        if (_disposed) return;
        _linework.Dispose();
        _disposed = true;
        _elements.Clear();
    }
}

/// <summary>Builds the remote AEC office as a lightweight RPG-like blueprint workspace.</summary>
public static class SpatialAECRemoteOfficeGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Yellow = "#ffd34d";
    private const string Magenta = "#ff38d1";
    private const string White = "#ffffff";
    private const string Muted = "#7895a1";

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
        Action exitOffice)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0")
            .Style("width", "100vw").Style("height", "100vh")
            .Style("overflow", "hidden")
            .Style("background", "#061018")
            .Style("color", White)
            .Style("font-family", "Consolas, 'Courier New', monospace");

        root.Content(WorkshopGui.Element(receiver, "style").Text(
            "@keyframes blueprint-breathe{0%,100%{opacity:.72}50%{opacity:1}}" +
            "@keyframes forge-glow{0%,100%{filter:brightness(.9)}50%{filter:brightness(1.25)}}"));

        var canvas = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 100")
            .Attribute("preserveAspectRatio", "none")
            .Style("position", "absolute").Style("inset", "0")
            .Style("width", "100vw").Style("height", "100vh")
            .Style("cursor", draft.Drawing ? "crosshair" : "cell")
            .Style("background", "linear-gradient(rgba(0,234,255,.035) 1px,transparent 1px),linear-gradient(90deg,rgba(0,234,255,.035) 1px,transparent 1px)")
            .Style("background-size", "5% 5%")
            .AriaLabel("AEC blueprint drafting surface. Click two points to create a semantic element.")
            .OnClick(drawPoint)
            .StopPropagation("onclick");

        for (var i = 0; i <= 100; i += 10)
        {
            canvas.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", i).Attribute("y1", 0).Attribute("x2", i).Attribute("y2", 100)
                .Attribute("stroke", Cyan).Attribute("stroke-opacity", ".09").Attribute("stroke-width", ".12"));
            canvas.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", 0).Attribute("y1", i).Attribute("x2", 100).Attribute("y2", i)
                .Attribute("stroke", Cyan).Attribute("stroke-opacity", ".09").Attribute("stroke-width", ".12"));
        }

        foreach (var element in draft.Elements)
        {
            var stroke = ColorFor(element.Kind);
            canvas.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", element.Line.Start.X).Attribute("y1", element.Line.Start.Y)
                .Attribute("x2", element.Line.End.X).Attribute("y2", element.Line.End.Y)
                .Attribute("stroke", stroke).Attribute("stroke-width", element.Kind == SpatialAECElementKind.Wall ? ".7" : ".38")
                .Attribute("stroke-opacity", ".95"));
            AddDimension(canvas, receiver, element, stroke);
        }

        if (draft.Drawing && draft.StartPoint is { } start)
        {
            canvas.Child(WorkshopGui.Element(receiver, "circle")
                .Attribute("cx", start.X).Attribute("cy", start.Y).Attribute("r", ".65")
                .Attribute("fill", "none").Attribute("stroke", Yellow).Attribute("stroke-width", ".25"));
        }

        root.Content(canvas);
        root.Content(Avatar(receiver, avatarName));
        root.Content(LeftPanel(receiver, manifest, draft, setElementKind, setSnap, setOrthogonal, clear, undo, exitOffice));
        root.Content(RightPanel(receiver, manifest, draft));
        return root;
    }

    private static ElementBuilder LeftPanel(
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
        var panel = WorkshopGui.Element(receiver, "div")
            .Style("position", "fixed").Style("left", "1rem").Style("top", "1rem").Style("z-index", "30")
            .Style("width", "min(23rem,calc(100vw - 2rem))").Style("max-height", "calc(100vh - 2rem)")
            .Style("overflow", "auto").Style("padding", ".8rem")
            .Style("border", $"1px solid {Cyan}66")
            .Style("background", "rgba(2,10,17,.93)")
            .Style("box-shadow", $"0 0 30px {Cyan}10,inset 0 0 22px {Cyan}07");

        panel.Content(WorkshopGui.Element(receiver, "div").Style("color", Cyan).Style("font-size", ".58rem").Style("letter-spacing", ".2em").Text(manifest.Name));
        panel.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".35rem").Style("color", Muted).Style("font-size", ".4rem").Text("SINGULARITYBIM // AUTHORING SURFACE"));

        var disciplines = WorkshopGui.Element(receiver, "div").Style("display", "flex").Style("flex-wrap", "wrap").Style("gap", ".3rem").Style("margin-top", ".7rem");
        foreach (var discipline in manifest.Disciplines)
            disciplines.Content(WorkshopGui.Button(receiver).Label(discipline.Name).Style("padding", ".3rem .42rem").Style("font-family", "inherit").Style("font-size", ".38rem").Style("cursor", "pointer").OnClick(() => { }));
        panel.Content(disciplines);

        panel.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".75rem").Style("color", Yellow).Style("font-size", ".45rem").Style("letter-spacing", ".15em").Text("ELEMENT BUNDLES"));

        var tools = WorkshopGui.Element(receiver, "div").Style("display", "grid").Style("grid-template-columns", "repeat(3,1fr)").Style("gap", ".3rem").Style("margin-top", ".35rem");
        foreach (var kind in Enum.GetValues<SpatialAECElementKind>())
        {
            var active = draft.ElementKind == kind;
            tools.Content(WorkshopGui.Button(receiver).Label(kind.ToString().ToUpperInvariant())
                .Style("padding", ".38rem .2rem").Style("border", $"1px solid {(active ? Cyan : "#ffffff")}55")
                .Style("background", active ? $"{Cyan}16" : "transparent")
                .Style("color", active ? Cyan : White).Style("font-family", "inherit").Style("font-size", ".36rem")
                .Style("cursor", "pointer").OnClick(() => setElementKind(kind)));
        }
        panel.Content(tools);

        panel.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".75rem").Style("color", Yellow).Style("font-size", ".45rem").Text("DRAFTING CONTROLS"));
        var snap = WorkshopGui.Element(receiver, "div").Style("display", "flex").Style("gap", ".3rem").Style("margin-top", ".3rem");
        foreach (var value in new[] { .5d, 1d, 2d, 5d })
            snap.Content(WorkshopGui.Button(receiver).Label($"SNAP {value:0.0}")
                .Style("padding", ".3rem .4rem").Style("font-family", "inherit").Style("font-size", ".36rem").Style("cursor", "pointer")
                .OnClick(() => setSnap(value)));
        panel.Content(snap);

        panel.Content(WorkshopGui.Button(receiver).Label(draft.Orthogonal ? "ORTHO ON" : "ORTHO OFF")
            .Style("margin-top", ".35rem").Style("padding", ".35rem .5rem")
            .Style("border", $"1px solid {Cyan}55").Style("background", draft.Orthogonal ? $"{Cyan}12" : "transparent")
            .Style("color", draft.Orthogonal ? Cyan : White).Style("font-family", "inherit").Style("font-size", ".38rem")
            .Style("cursor", "pointer").OnClick(() => setOrthogonal(!draft.Orthogonal)));

        panel.Content(WorkshopGui.Button(receiver).Label("UNDO LAST")
            .Style("margin-top", ".55rem").Style("padding", ".4rem .55rem").Style("cursor", "pointer")
            .OnClick(undo));
        panel.Content(WorkshopGui.Button(receiver).Label("CLEAR PLAN")
            .Style("margin-left", ".35rem").Style("padding", ".4rem .55rem").Style("cursor", "pointer")
            .OnClick(clear));

        panel.Content(WorkshopGui.Button(receiver).Label("← LEAVE AEC OFFICE")
            .Style("display", "block").Style("margin-top", ".75rem").Style("padding", ".45rem .6rem")
            .Style("border", $"1px solid {Magenta}66").Style("background", "transparent")
            .Style("color", Magenta).Style("font-family", "inherit").Style("font-size", ".4rem")
            .Style("cursor", "pointer").OnClick(exitOffice));

        return panel;
    }

    private static ElementBuilder RightPanel(object receiver, SpatialAECRemoteOfficeManifest manifest, SpatialAECDraftModel draft)
    {
        var panel = WorkshopGui.Element(receiver, "div")
            .Style("position", "fixed").Style("right", "1rem").Style("top", "1rem").Style("z-index", "30")
            .Style("width", "min(20rem,calc(100vw - 2rem))").Style("max-height", "calc(100vh - 2rem)")
            .Style("overflow", "auto").Style("padding", ".7rem")
            .Style("border", $"1px solid {Yellow}55").Style("background", "rgba(2,10,17,.9)");

        panel.Content(WorkshopGui.Element(receiver, "div").Style("color", Yellow).Style("font-size", ".5rem").Style("letter-spacing", ".15em").Text("AUTO DIMENSIONS"));
        panel.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".3rem").Style("color", Muted).Style("font-size", ".37rem").Text($"{draft.Elements.Count:00} SEMANTIC ELEMENTS"));

        if (draft.Elements.Count == 0)
            panel.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".7rem").Style("color", Muted).Style("font-size", ".42rem").Text("Draw two points. The office will snap, classify, and dimension the element."));
        else
            foreach (var element in draft.Elements.TakeLast(12))
                panel.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".45rem").Style("padding", ".4rem").Style("border-left", $"2px solid {ColorFor(element.Kind)}")
                    .Content(WorkshopGui.Element(receiver, "div").Style("color", White).Style("font-size", ".42rem").Text($"#{element.Index:00} {element.Kind.ToString().ToUpperInvariant()}"))
                    .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".2rem").Style("color", Yellow).Style("font-size", ".38rem").Text($"{element.Length:0.##}u @ {element.AngleDegrees:0.#}°"))
                    .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".15rem").Style("color", Muted).Style("font-size", ".34rem").Text($"{element.Line.Start.X:0.##},{element.Line.Start.Y:0.##} → {element.Line.End.X:0.##},{element.Line.End.Y:0.##}")));

        panel.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".7rem").Style("color", Cyan).Style("font-size", ".42rem").Style("letter-spacing", ".12em").Text("OUTPUT BOUNDARIES"));
        panel.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".25rem").Style("color", Muted).Style("font-size", ".35rem").Text(string.Join(" // ", manifest.ExternalTargets.Select(x => x.Name))));
        return panel;
    }

    private static void AddDimension(ElementBuilder canvas, object receiver, SpatialAECDraftElement element, string color)
    {
        var midpointX = (element.Line.Start.X + element.Line.End.X) / 2d;
        var midpointY = (element.Line.Start.Y + element.Line.End.Y) / 2d;
        canvas.Child(WorkshopGui.Element(receiver, "text")
            .Attribute("x", midpointX).Attribute("y", midpointY - 1.1)
            .Attribute("fill", Yellow).Attribute("font-size", "1.5")
            .Attribute("text-anchor", "middle")
            .Attribute("paint-order", "stroke").Attribute("stroke", "#061018").Attribute("stroke-width", ".45")
            .Text($"{element.Length:0.##}"));
    }

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
            SpatialAECElementKind.Electrical => "#ffd34d",
            SpatialAECElementKind.Annotation => White,
            _ => White
        };

    private static ElementBuilder Avatar(object receiver, string name)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "fixed").Style("left", "50%").Style("bottom", "1rem").Style("z-index", "25")
            .Style("pointer-events", "none").Style("color", White).Style("font-size", ".4rem")
            .Content(WorkshopGui.Element(receiver, "span").Style("margin-right", ".35rem").Text(name))
            .Content(WorkshopGui.Element(receiver, "span").Style("color", Cyan).Style("animation", "blueprint-breathe 2s infinite").Text("●"));
}
