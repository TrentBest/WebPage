namespace TheSingularityWorkshop.Gui;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

/// <summary>A nested Workshop Experience that demonstrates authored spatial traversal.</summary>
public sealed class SpatialMazeExperience
{
    public SpatialMazeExperience(string id, string title, IReadOnlyList<string> rows)
    {
        Id = string.IsNullOrWhiteSpace(id) ? throw new ArgumentException("Maze id is required.", nameof(id)) : id;
        Title = string.IsNullOrWhiteSpace(title) ? throw new ArgumentException("Maze title is required.", nameof(title)) : title;
        Rows = new ReadOnlyCollection<string>((rows ?? throw new ArgumentNullException(nameof(rows))).ToList());
        if (Rows.Count == 0) throw new ArgumentException("Maze requires at least one row.", nameof(rows));
        Width = Rows.Max(x => x.Length);
        Height = Rows.Count;
        Start = Find('S');
        Exit = Find('E');
    }

    public string Id { get; }
    public string Title { get; }
    public IReadOnlyList<string> Rows { get; }
    public int Width { get; }
    public int Height { get; }
    public SpatialPoint Start { get; }
    public SpatialPoint Exit { get; }

    public bool IsWalkable(int x, int y)
        => y >= 0 && y < Height && x >= 0 && x < Rows[y].Length && Rows[y][x] != '#';

    private SpatialPoint Find(char marker)
    {
        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Rows[y].Length; x++)
                if (Rows[y][x] == marker) return new SpatialPoint(x + .5, y + .5);
        throw new InvalidOperationException($"Maze marker '{marker}' was not found.");
    }
}

public sealed record SpatialMazeParticipant(string Name, SpatialPoint Position, bool IsCurrentUser = false);

public sealed record SpatialMazeScore(string Name, string Time, int Steps);

public static class SpatialMazeCatalog
{
    public static SpatialMazeExperience Showcase { get; } = new(
        "maze",
        "WORKSHOP MAZE // NAVIGATION TEST",
        [
            "#########################",
            "#S....#........#........#",
            "###.#.#.######.#.######.#",
            "#...#.#......#.#......#.#",
            "#.###.######.#.######.#.#",
            "#.....#......#......#...#",
            "#.#####.###########.###.#",
            "#.......#.........#.....#",
            "#######.#.#######.#.#####",
            "#.......#.#.....#.#.....#",
            "#.#######.#.###.#.#####.#",
            "#.........#.#...#.......#",
            "#########.#.#.#########.#",
            "#.........#.#...........#",
            "#.#########.#############",
            "#.................#......#",
            "#################.#.######",
            "#.................#......#",
            "###################.######",
            "#.......................E#",
            "#########################"
        ]);

    public static IReadOnlyList<SpatialMazeParticipant> Participants { get; } =
    [
        new("Avery", new SpatialPoint(2.5, 1.5)),
        new("Morgan", new SpatialPoint(18.5, 3.5)),
        new("Riley", new SpatialPoint(8.5, 17.5))
    ];

    public static IReadOnlyList<SpatialMazeScore> Leaderboard { get; } =
    [
        new("Avery", "01:42.18", 118),
        new("Morgan", "02:07.51", 143),
        new("Riley", "02:31.04", 169),
        new("Casey", "03:04.77", 201),
        new("Jordan", "03:26.12", 227)
    ];
}
