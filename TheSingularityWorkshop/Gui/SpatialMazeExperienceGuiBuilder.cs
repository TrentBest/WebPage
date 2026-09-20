namespace TheSingularityWorkshop.Gui;

using Microsoft.AspNetCore.Components.Web;
using TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>Recursive spatial presentation for the Workshop maze Experience.</summary>
public static class SpatialMazeExperienceGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Yellow = "#ffd34d";
    private const string White = "#ffffff";
    private const string Muted = "#9fb2bd";

    public static ElementBuilder Build(
        object receiver,
        SpatialHolodeckMazeModel holodeck,
        string userName,
        LeaderboardMicroBundle leaderboard,
        Action<string?> selectLeaderboardEntry,
        string? selectedLeaderboardEntry,
        Action<KeyboardEventArgs> onKeyDown,
        Action exit,
        Action toggleMode,
        double zoom,
        Action<double> setZoom)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0").Style("overflow", "hidden")
            .Style("background", "#02070d").Style("color", White)
            .Attribute("tabindex", "0").Attribute("autofocus", "autofocus")
            .OnKeyDown(onKeyDown)
            .OnWheel(args => setZoom(Math.Clamp(zoom * (args.DeltaY < 0 ? 1.12 : 1 / 1.12), .6, 2.2)))
            .PreventDefault("onwheel");

        if (holodeck.Mode == SpatialHolodeckMazeMode.FirstPerson)
            root.Content(FirstPerson(receiver, holodeck));
        else
            root.Content(TwoDimensional(receiver, holodeck, zoom));

        root.Content(Hud(receiver, holodeck, userName, zoom, setZoom, toggleMode, exit, leaderboard, selectLeaderboardEntry, selectedLeaderboardEntry));
        return root;
    }

    private static ElementBuilder TwoDimensional(object receiver, SpatialHolodeckMazeModel holodeck, double zoom)
    {
        var maze = holodeck.Maze;
        var avatar = holodeck.PlayerPosition;
        var cell = 34d * zoom;
        var offsetX = 50d - (avatar.X * cell / 10d);
        var offsetY = 50d - (avatar.Y * cell / 10d);
        var map = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", $"\{offsetX:0.###\}vw").Style("top", $"\{offsetY:0.###\}vh")
            .Style("transform-origin", "0 0");

        for (var y = 0; y < maze.Height; y++)
            for (var x = 0; x < maze.Width; x++)
            {
                if (x >= maze.Rows[y].Length) continue;
                var ch = maze.Rows[y][x];
                map.Content(WorkshopGui.Element(receiver, "div")
                    .Style("position", "absolute")
                    .Style("left", $"\{x * cell:0.###\}px").Style("top", $"\{y * cell:0.###\}px")
                    .Style("width", $"\{cell:0.###\}px").Style("height", $"\{cell:0.###\}px")
                    .Style("box-sizing", "border-box")
                    .Style("border", ch == '#' ? $"1px solid \{Cyan\}55" : $"1px solid \{Cyan\}0d")
                    .Style("background", ch == '#' ? $"\{Cyan\}12" : "transparent")
                    .Style("pointer-events", "none"));
            }

        map.Content(Dot(receiver, "YOU", avatar, Yellow, cell));
        foreach (var agent in holodeck.Agents)
            map.Content(Dot(receiver, agent.Name, agent.Position, agent.Rule == SpatialMazeWallRule.LeftHand ? Magenta : Cyan, cell));

        return map;
    }

    private static ElementBuilder FirstPerson(object receiver, SpatialHolodeckMazeModel holodeck)
    {
        var root = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("inset", "0")
            .Style("perspective", "900px")
            .Style("background", "radial-gradient(circle at 50% 48%, #163444 0, #02070d 62%)");

        for (var depth = 1; depth <= 6; depth++)
        {
            var inset = 8 + depth * 7;
            root.Content(WorkshopGui.Element(receiver, "div")
                .Style("position", "absolute").Style("left", $"{inset}%").Style("right", $"{inset}%")
                .Style("top", $"{inset * .65}%").Style("bottom", $"{inset * .45}%")
                .Style("border", $"1px solid \{Cyan\}{Math.Max(12, 72 - depth * 9):X2}")
                .Style("transform", $"translateZ(-{depth * 70}px)")
                .Style("pointer-events", "none"));
        }

        root.Content(WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "50%").Style("top", "52%")
            .Style("transform", "translate(-50%,-50%)")
            .Style("color", Yellow).Style("font-size", "2rem")
            .Text(HeadingGlyph(holodeck.PlayerHeading)));

        root.Content(WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "50%").Style("bottom", "12%")
            .Style("transform", "translateX(-50%)")
            .Style("color", White).Style("font-family", "Consolas,'Courier New',monospace")
            .Style("font-size", ".65rem")
            .Text($"FIRST-PERSON // {holodeck.PlayerHeading.ToString().ToUpperInvariant()} // CELL {Math.Floor(holodeck.PlayerPosition.X):0},{Math.Floor(holodeck.PlayerPosition.Y):0}"));

        return root;
    }

    private static string HeadingGlyph(SpatialMazeHeading heading)
        => heading switch
        {
            SpatialMazeHeading.North => "▲",
            SpatialMazeHeading.East => "▶",
            SpatialMazeHeading.South => "▼",
            _ => "◀"
        };

    private static ElementBuilder Dot(object receiver, string name, SpatialPoint position, string glyph, double cell)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute")
            .Style("left", $"\{position.X * cell:0.###\}px").Style("top", $"\{position.Y * cell:0.###\}px")
            .Style("transform", "translate(-50%,-50%)")
            .Style("width", "11px").Style("height", "11px").Style("border-radius", "50%")
            .Style("background", glyph).Style("box-shadow", $"0 0 16px \{glyph\}aa")
            .Style("z-index", "50")
            .Content(WorkshopGui.Element(receiver, "span")
                .Style("position", "absolute").Style("left", "50%").Style("bottom", "14px")
                .Style("transform", "translateX(-50%)").Style("white-space", "nowrap")
                .Style("padding", ".12rem .28rem").Style("border", $"1px solid \{glyph\}55")
                .Style("background", "rgba(1,4,10,.9)").Style("color", glyph)
                .Style("font-family", "Consolas,'Courier New',monospace").Style("font-size", ".42rem")
                .Style("letter-spacing", ".08em").Text(name));

    private static ElementBuilder Hud(
        object receiver,
        SpatialHolodeckMazeModel holodeck,
        string userName,
        double zoom,
        Action<double> setZoom,
        Action toggleMode,
        Action exit,
        LeaderboardMicroBundle leaderboard,
        Action<string?> selectLeaderboardEntry,
        string? selectedLeaderboardEntry)
    {
        var hud = WorkshopGui.Element(receiver, "div").Style("position", "fixed").Style("inset", "0").Style("pointer-events", "none");

        var status = holodeck.Captured
            ? "CAPTURED // SIMULATION RESET REQUIRED"
            : holodeck.Finished
                ? "EXIT FOUND // HOLODECK RUN COMPLETE"
                : holodeck.AgentsReleased
                    ? "HUNTERS RELEASED // WALL-FOLLOWING PURSUIT ACTIVE"
                    : $"HEAD START // {Math.Max(0, (int)Math.Ceiling((SpatialHolodeckMazeModel.AgentDelaySeconds - (DateTime.UtcNow - holodeck.StartedUtc).TotalSeconds))):00}s";

        hud.Content(WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("left", "1rem").Style("top", "1rem")
            .Style("padding", ".5rem .65rem").Style("background", "rgba(1,4,10,.9)").Style("border", $"1px solid \{Cyan\}55")
            .Style("font-family", "Consolas,'Courier New',monospace").Style("font-size", ".5rem")
            .Text($"HOLODECK // {holodeck.Maze.Title} // {userName} // {holodeck.Mode}"));

        hud.Content(WorkshopGui.Button(receiver).Label(holodeck.Mode == SpatialHolodeckMazeMode.TwoDimensional ? "SWITCH TO FPS NAVIGATION" : "SWITCH TO 2D MAZE")
            .Style("position", "absolute").Style("left", "1rem").Style("top", "4.2rem")
            .Style("pointer-events", "auto").Style("padding", ".45rem .6rem")
            .Style("border", $"1px solid \{Magenta\}66").Style("background", "rgba(1,4,10,.92)")
            .Style("color", Magenta).Style("font-family", "inherit").Style("font-size", ".38rem")
            .Style("cursor", "pointer").OnClick(toggleMode));

        hud.Content(WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("left", "1rem").Style("bottom", "1rem")
            .Style("padding", ".45rem .6rem").Style("background", "rgba(1,4,10,.9)").Style("border", $"1px solid \{Cyan\}44")
            .Style("color", holodeck.Captured ? Magenta : holodeck.AgentsReleased ? Yellow : Muted)
            .Style("font-family", "system-ui,sans-serif").Style("font-size", ".58rem")
            .Text(holodeck.Started ? status : "MAZE READY // YOU HAVE 30 SECONDS BEFORE THE HUNTERS ENTER"));

        hud.Content(WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("right", "1rem").Style("bottom", "1rem")
            .Style("padding", ".4rem .55rem").Style("background", "rgba(1,4,10,.9)")
            .Style("border", $"1px solid \{Cyan\}55").Style("color", White).Style("font-size", ".38rem")
            .Text("2D: WASD / ARROWS // FPS: W/S MOVE, A/D TURN"));

        hud.Content(WorkshopGui.Button(receiver).Label("← EXIT HOLODECK")
            .Style("position", "absolute").Style("right", "1rem").Style("top", "1rem")
            .Style("pointer-events", "auto").Style("padding", ".4rem .55rem")
            .Style("background", "rgba(1,4,10,.9)").Style("border", $"1px solid \{Cyan\}55")
            .Style("color", White).Style("cursor", "pointer").OnClick(exit));

        hud.Content(LeaderboardPanel(receiver, leaderboard, selectLeaderboardEntry, selectedLeaderboardEntry));
        return hud;
    }

    private static ElementBuilder LeaderboardPanel(
        object receiver,
        LeaderboardMicroBundle leaderboard,
        Action<string?> selectLeaderboardEntry,
        string? selectedLeaderboardEntry)
    {
        var panel = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("right", "1rem").Style("top", "1rem")
            .Style("pointer-events", "auto").Style("padding", ".6rem").Style("width", "210px")
            .Style("background", "rgba(1,4,10,.94)").Style("border", $"1px solid {Yellow}55");

        panel.Content(WorkshopGui.Element(receiver, "button")
            .Style("width", "100%").Style("text-align", "left").Style("padding", "0")
            .Style("border", "0").Style("background", "transparent").Style("color", Yellow)
            .Style("font-size", ".52rem").Style("letter-spacing", ".15em").Style("cursor", "pointer")
            .OnClick(() => selectLeaderboardEntry(null))
            .Text($"MICROBUNDLE // {leaderboard.Presentation.Surface}"));

        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("margin-top", ".35rem").Style("color", Muted).Style("font-size", ".42rem")
            .Text("Click a result to inspect its current data."));

        if (selectedLeaderboardEntry is not null && leaderboard.TryGet(selectedLeaderboardEntry, out var selected))
        {
            panel.Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".5rem").Style("padding", ".4rem")
                .Style("border", $"1px solid {Cyan}33").Style("background", $"{Cyan}08")
                .Style("color", White).Style("font-family", "Consolas,'Courier New',monospace").Style("font-size", ".44rem")
                .Text($"{selected.Name} // LIVE RESULT\\nTIME {selected.DisplayTime}\\nSTEPS {selected.Steps}"));
        }

        foreach (var (entry, index) in leaderboard.Entries.Select((value, index) => (value, index)))
        {
            var capturedName = entry.Name;
            panel.Content(WorkshopGui.Element(receiver, "button")
                .Style("display", "block").Style("width", "100%").Style("margin-top", ".25rem")
                .Style("padding", ".2rem .1rem").Style("border", "0").Style("border-bottom", $"1px solid {Cyan}18")
                .Style("background", "transparent").Style("color", White).Style("text-align", "left")
                .Style("font-family", "Consolas,'Courier New',monospace").Style("font-size", ".46rem").Style("cursor", "pointer")
                .OnClick(() => selectLeaderboardEntry(capturedName))
                .Text($"{index + 1}. {entry.Name,-8} {entry.DisplayTime}  {entry.Steps}"));
        }

        return panel;
    }
}
