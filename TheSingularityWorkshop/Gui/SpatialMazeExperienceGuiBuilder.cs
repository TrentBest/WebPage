namespace TheSingularityWorkshop.Gui;

using Microsoft.AspNetCore.Components.Web;

/// <summary>Recursive spatial presentation for the Workshop maze Experience.</summary>
public static class SpatialMazeExperienceGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Yellow = "#ffd34d";
    private const string White = "#ffffff";
    private const string Muted = "#9fb2bd";

    public static ElementBuilder Build(
        object receiver,
        SpatialMazeExperience maze,
        string userName,
        SpatialPoint avatar,
        IReadOnlyList<SpatialMazeParticipant> participants,
        Action<KeyboardEventArgs> onKeyDown,
        Action exit,
        double zoom,
        Action<double> setZoom,
        bool finished)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0").Style("overflow", "hidden")
            .Style("background", "#02070d").Style("color", White)
            .Attribute("tabindex", "0").Attribute("autofocus", "autofocus")
            .OnKeyDown(onKeyDown)
            .OnWheel(args => setZoom(Math.Clamp(zoom * (args.DeltaY < 0 ? 1.12 : 1 / 1.12), .6, 2.2)))
            .PreventDefault("onwheel");

        var cell = 34d * zoom;
        var offsetX = 50d - (avatar.X * cell / 10d);
        var offsetY = 50d - (avatar.Y * cell / 10d);
        var map = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", $"{offsetX:0.###}vw").Style("top", $"{offsetY:0.###}vh")
            .Style("transform-origin", "0 0");

        for (var y = 0; y < maze.Height; y++)
            for (var x = 0; x < maze.Width; x++)
            {
                if (x >= maze.Rows[y].Length) continue;
                var ch = maze.Rows[y][x];
                var tile = WorkshopGui.Element(receiver, "div")
                    .Style("position", "absolute").Style("left", $"{x * cell:0.###}px").Style("top", $"{y * cell:0.###}px")
                    .Style("width", $"{cell:0.###}px").Style("height", $"{cell:0.###}px")
                    .Style("box-sizing", "border-box")
                    .Style("border", ch == '#' ? $"1px solid {Cyan}55" : $"1px solid {Cyan}0d")
                    .Style("background", ch == '#' ? $"{Cyan}12" : "transparent")
                    .Style("pointer-events", "none");
                map.Content(tile);
            }

        map.Content(Dot(receiver, "YOU", avatar, Yellow, cell));
        foreach (var participant in participants)
            map.Content(Dot(receiver, participant.Name, participant.Position, Cyan, cell));

        root.Content(map);
        root.Content(Hud(receiver, maze, userName, zoom, setZoom, exit, finished));
        return root;
    }

    private static ElementBuilder Dot(object receiver, string name, SpatialPoint position, string glyph, double cell)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute")
            .Style("left", $"{position.X * cell:0.###}px").Style("top", $"{position.Y * cell:0.###}px")
            .Style("transform", "translate(-50%,-50%)")
            .Style("width", "11px").Style("height", "11px").Style("border-radius", "50%")
            .Style("background", glyph).Style("box-shadow", $"0 0 16px {glyph}aa")
            .Style("z-index", "50")
            .Content(WorkshopGui.Element(receiver, "span")
                .Style("position", "absolute").Style("left", "50%").Style("bottom", "14px")
                .Style("transform", "translateX(-50%)").Style("white-space", "nowrap")
                .Style("padding", ".12rem .28rem").Style("border", $"1px solid {glyph}55")
                .Style("background", "rgba(1,4,10,.9)").Style("color", glyph)
                .Style("font-family", "Consolas,'Courier New',monospace").Style("font-size", ".42rem")
                .Style("letter-spacing", ".08em").Text(name));

    private static ElementBuilder Hud(object receiver, SpatialMazeExperience maze, string userName, double zoom, Action<double> setZoom, Action exit, bool finished)
    {
        var hud = WorkshopGui.Element(receiver, "div").Style("position", "fixed").Style("inset", "0").Style("pointer-events", "none");
        hud.Content(WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("left", "1rem").Style("top", "1rem")
            .Style("padding", ".5rem .65rem").Style("background", "rgba(1,4,10,.9)").Style("border", $"1px solid {Cyan}55")
            .Style("font-family", "Consolas,'Courier New',monospace").Style("font-size", ".5rem").Text($"{maze.Title} // {userName} // ZOOM {zoom:0.00}X"));
        hud.Content(WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("right", "1rem").Style("top", "1rem")
            .Style("pointer-events", "auto").Style("padding", ".6rem").Style("width", "190px")
            .Style("background", "rgba(1,4,10,.94)").Style("border", $"1px solid {Yellow}55")
            .Content(WorkshopGui.Element(receiver, "div").Style("color", Yellow).Style("font-size", ".52rem").Style("letter-spacing", ".15em").Text("LEADERBOARD"))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".45rem").Style("font-family", "Consolas,'Courier New',monospace").Style("font-size", ".46rem").Style("line-height", "1.65")
                .Text(string.Join("\n", SpatialMazeCatalog.Leaderboard.Select((x, i) => $"{i + 1}. {x.Name,-8} {x.Time}  {x.Steps}")))));
        hud.Content(WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("left", "1rem").Style("bottom", "1rem")
            .Style("padding", ".45rem .6rem").Style("background", "rgba(1,4,10,.9)").Style("border", $"1px solid {Cyan}44")
            .Style("color", Muted).Style("font-family", "system-ui,sans-serif").Style("font-size", ".58rem")
            .Text(finished ? "EXIT FOUND. You solved the authored maze." : "MAZE MODE: WASD / ARROWS. Clicks far from your avatar are ignored. Find the exit."));
        hud.Content(WorkshopGui.Element(receiver, "button").Style("position", "absolute").Style("right", "1rem").Style("bottom", "1rem")
            .Style("pointer-events", "auto").Style("padding", ".4rem .55rem").Style("background", "rgba(1,4,10,.9)")
            .Style("border", $"1px solid {Cyan}55").Style("color", White).Style("cursor", "pointer").OnClick(exit).Text("← EXIT"));
        return hud;
    }
}
