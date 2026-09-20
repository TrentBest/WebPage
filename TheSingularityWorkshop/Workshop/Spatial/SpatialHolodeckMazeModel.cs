namespace TheSingularityWorkshop.Gui;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Runtime model for the laboratory's first holodeck experience.
/// The authored maze remains data; presentation may be top-down or first-person.
/// </summary>
public sealed class SpatialHolodeckMazeModel
{
    public const int AgentDelaySeconds = 30;
    public const double AgentStepIntervalSeconds = .55d;

    private readonly SpatialMazeExperience _maze;

    public SpatialHolodeckMazeModel(SpatialMazeExperience? maze = null)
    {
        _maze = maze ?? SpatialMazeCatalog.Showcase;
        Reset();
    }

    public SpatialMazeExperience Maze => _maze;
    public SpatialHolodeckMazeMode Mode { get; private set; }
    public SpatialPoint PlayerPosition { get; private set; }
    public SpatialMazeHeading PlayerHeading { get; private set; }
    public bool Started { get; private set; }
    public bool AgentsReleased { get; private set; }
    public bool Captured { get; private set; }
    public bool Finished { get; private set; }
    public int Steps { get; private set; }
    public DateTime StartedUtc { get; private set; }

    public TimeSpan HeadStartRemaining
        => !Started
            ? TimeSpan.FromSeconds(AgentDelaySeconds)
            : TimeSpan.FromSeconds(Math.Max(0, AgentDelaySeconds - (DateTime.UtcNow - StartedUtc).TotalSeconds));

    public IReadOnlyList<SpatialHolodeckAgent> Agents => _agents;

    private readonly List<SpatialHolodeckAgent> _agents =
    [
        new("left-wall", "LEFT-WALL", SpatialMazeWallRule.LeftHand, SpatialMazeCatalog.Participants[1].Position, SpatialMazeHeading.South),
        new("right-wall", "RIGHT-WALL", SpatialMazeWallRule.RightHand, SpatialMazeCatalog.Participants[2].Position, SpatialMazeHeading.North)
    ];

    public void Reset()
    {
        PlayerPosition = _maze.Start;
        PlayerHeading = SpatialMazeHeading.East;
        Mode = SpatialHolodeckMazeMode.TwoDimensional;
        Started = false;
        AgentsReleased = false;
        Captured = false;
        Finished = false;
        Steps = 0;
        StartedUtc = default;
        _agents[0] = _agents[0] with { Position = SpatialMazeCatalog.Participants[1].Position, Heading = SpatialMazeHeading.South, LastMoveUtc = default };
        _agents[1] = _agents[1] with { Position = SpatialMazeCatalog.Participants[2].Position, Heading = SpatialMazeHeading.North, LastMoveUtc = default };
    }

    public void Start(DateTime utcNow)
    {
        if (Started || Captured || Finished) return;
        Started = true;
        StartedUtc = utcNow;
    }

    public void ToggleMode()
        => Mode = Mode == SpatialHolodeckMazeMode.TwoDimensional
            ? SpatialHolodeckMazeMode.FirstPerson
            : SpatialHolodeckMazeMode.TwoDimensional;

    public bool TryMovePlayer(int dx, int dy, DateTime utcNow)
    {
        Start(utcNow);
        if (Captured || Finished) return false;

        var nextX = (int)Math.Floor(PlayerPosition.X) + Math.Sign(dx);
        var nextY = (int)Math.Floor(PlayerPosition.Y) + Math.Sign(dy);
        if (!_maze.IsWalkable(nextX, nextY)) return false;

        PlayerPosition = new SpatialPoint(nextX + .5, nextY + .5);
        if (dx != 0) PlayerHeading = dx > 0 ? SpatialMazeHeading.East : SpatialMazeHeading.West;
        if (dy != 0) PlayerHeading = dy > 0 ? SpatialMazeHeading.South : SpatialMazeHeading.North;
        Steps++;
        CheckTerminalState();
        return true;
    }

    public bool TurnLeft(DateTime utcNow)
    {
        Start(utcNow);
        if (Captured || Finished) return false;
        PlayerHeading = Rotate(PlayerHeading, -1);
        return true;
    }

    public bool TurnRight(DateTime utcNow)
    {
        Start(utcNow);
        if (Captured || Finished) return false;
        PlayerHeading = Rotate(PlayerHeading, 1);
        return true;
    }

    public void Advance(DateTime utcNow)
    {
        if (!Started || Captured || Finished) return;

        if (!AgentsReleased)
        {
            AgentsReleased = (utcNow - StartedUtc).TotalSeconds >= AgentDelaySeconds;
            if (!AgentsReleased) return;
        }

        for (var index = 0; index < _agents.Count; index++)
        {
            var agent = _agents[index];
            if (agent.LastMoveUtc != default &&
                (utcNow - agent.LastMoveUtc).TotalSeconds < AgentStepIntervalSeconds)
                continue;

            var moved = StepWallFollower(agent);
            if (moved is not null)
                _agents[index] = moved.Value with { LastMoveUtc = utcNow };
        }

        CheckTerminalState();
    }

    private SpatialHolodeckAgent? StepWallFollower(SpatialHolodeckAgent agent)
    {
        var candidates = agent.Rule == SpatialMazeWallRule.LeftHand
            ? new[] { Rotate(agent.Heading, -1), agent.Heading, Rotate(agent.Heading, 1), Rotate(agent.Heading, 2) }
            : new[] { Rotate(agent.Heading, 1), agent.Heading, Rotate(agent.Heading, -1), Rotate(agent.Heading, 2) };

        foreach (var heading in candidates)
        {
            var (dx, dy) = Delta(heading);
            var x = (int)Math.Floor(agent.Position.X) + dx;
            var y = (int)Math.Floor(agent.Position.Y) + dy;
            if (!_maze.IsWalkable(x, y)) continue;

            return agent with
            {
                Position = new SpatialPoint(x + .5, y + .5),
                Heading = heading
            };
        }

        return null;
    }

    private void CheckTerminalState()
    {
        if (Cell(PlayerPosition) == Cell(_maze.Exit))
            Finished = true;

        if (_agents.Any(agent => Cell(agent.Position) == Cell(PlayerPosition)))
            Captured = true;
    }

    private static (int X, int Y) Cell(SpatialPoint point)
        => ((int)Math.Floor(point.X), (int)Math.Floor(point.Y));

    private static SpatialMazeHeading Rotate(SpatialMazeHeading heading, int quarterTurns)
    {
        var value = ((int)heading + quarterTurns) % 4;
        if (value < 0) value += 4;
        return (SpatialMazeHeading)value;
    }

    private static (int X, int Y) Delta(SpatialMazeHeading heading)
        => heading switch
        {
            SpatialMazeHeading.North => (0, -1),
            SpatialMazeHeading.East => (1, 0),
            SpatialMazeHeading.South => (0, 1),
            _ => (-1, 0)
        };
}

public enum SpatialHolodeckMazeMode
{
    TwoDimensional,
    FirstPerson
}

public enum SpatialMazeWallRule
{
    LeftHand,
    RightHand
}

public enum SpatialMazeHeading
{
    North,
    East,
    South,
    West
}

public readonly record struct SpatialHolodeckAgent(
    string Id,
    string Name,
    SpatialMazeWallRule Rule,
    SpatialPoint Position,
    SpatialMazeHeading Heading,
    DateTime LastMoveUtc);
