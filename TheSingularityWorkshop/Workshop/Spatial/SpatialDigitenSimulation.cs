using System;
using System.Collections.Generic;

namespace TheSingularityWorkshop.Gui
{

    /// <summary>Small deterministic simulation substrate for living non-player citizens.</summary>
    /// <remarks>
    /// Digitens are deliberately separated from the map renderer. A Digiten owns a goal,
    /// a world position, and a path through a supplied walkability field. Rendering can consume
    /// this state later without changing the simulation model.
    ///</remarks>
    public sealed class SpatialDigiten
    {
        private IReadOnlyList<SpatialGridPoint> _path = [];
        private int _pathIndex;

        public SpatialDigiten(string id, string name, SpatialPoint position, SpatialDigitenGoal goal)
        {
            Id = id;
            Name = name;
            Position = position;
            Goal = goal;
        }

        public string Id { get; }
        public string Name { get; }
        public SpatialPoint Position { get; private set; }
        public SpatialDigitenGoal Goal { get; private set; }
        public bool HasRoute => _path.Count > 0 && _pathIndex < _path.Count;

        public void SetGoal(SpatialDigitenGoal goal)
        {
            Goal = goal;
            _path = [];
            _pathIndex = 0;
        }

        public bool PlanRoute(int width, int height, Func<int, int, bool> isWalkable)
        {
            var start = new SpatialGridPoint((int)Math.Floor(Position.X), (int)Math.Floor(Position.Y));
            var goal = new SpatialGridPoint((int)Math.Floor(Goal.Position.X), (int)Math.Floor(Goal.Position.Y));
            _path = DiabloPathfinder.FindPath(width, height, isWalkable, start, goal);
            _pathIndex = _path.Count > 0 && _path[0] == start ? 1 : 0;
            return HasRoute || start == goal;
        }

        public bool Step()
        {
            if (!HasRoute) return false;
            var next = _path[_pathIndex++];
            Position = new SpatialPoint(next.X + .5, next.Y + .5);
            return true;
        }
    }

    public readonly record struct SpatialDigitenGoal(
        string Id,
        string Description,
        SpatialPoint Position);

    /// <summary>Creates a small population used by future living-world Experiences.</summary>
    public static class SpatialDigitenCatalog
    {
        public static IReadOnlyList<SpatialDigiten> CreateStarterPopulation()
            => [
            new("digiten-01", "Mara", new SpatialPoint(45.5, 45.5), new SpatialDigitenGoal("work", "Go to the Workshop", new SpatialPoint(50.5, 50.5))),
        new("digiten-02", "Ivo", new SpatialPoint(55.5, 48.5), new SpatialDigitenGoal("transit", "Catch the transit network", new SpatialPoint(90.5, 47.5))),
        new("digiten-03", "Nia", new SpatialPoint(47.5, 54.5), new SpatialDigitenGoal("market", "Visit the future public market", new SpatialPoint(34.5, 12.5))),
        new("digiten-04", "Sol", new SpatialPoint(53.5, 53.5), new SpatialDigitenGoal("research", "Reach the science district", new SpatialPoint(14.5, 74.5)))
        ];

    }
}