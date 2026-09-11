using System;
using System.Collections.Generic;
using TheSingularityWorkshop.FSM_API;

namespace TheSingularityWorkshop.Services
{
    /// <summary>
    /// Runtime data carried by the page-level FSM.
    /// The living GUI swarm is state, not Razor-owned timing state.
    /// </summary>
    public sealed class PageStateContext : IStateContext
    {
        private const int CriticalMass = 100;
        private const double MinimumNodeSize = 60;
        private const double MaximumNodeSize = 120;
        private const double GrowthPerTick = 2;
        private const double MinX = 8;
        private const double MaxX = 92;
        private const double MinY = 10;
        private const double MaxY = 88;

        private readonly Random _random = new();

        public string Name { get; set; } = "PageFSMContext";
        public bool IsValid { get; set; } = true;
        public object? SingularityHub { get; }

        public bool EnterRequested { get; set; }
        public bool LivingGuiPopulated { get; internal set; }
        public bool MonikerReady { get; internal set; }
        public bool GravityReleased { get; internal set; }
        public bool LivingGuiFallen { get; internal set; }
        public bool NavigationReady { get; internal set; }

        public long StateTicks { get; set; }
        public long TotalTicks { get; set; }
        public long GravityTicks { get; private set; }

        public bool LivingGuiFrozen { get; private set; }
        public bool GravityActive { get; private set; }
        public IReadOnlyList<LivingNodeState> LivingNodes => _livingNodes;

        private readonly List<LivingNodeState> _livingNodes = new();

        public PageStateContext(object? singularityHub = null)
        {
            SingularityHub = singularityHub;
        }

        public void ResetStateClock() => StateTicks = 0;

        public void BeginLivingGui()
        {
            if (_livingNodes.Count != 0) return;

            _livingNodes.Add(CreateNode("1", 1));
            LivingGuiFrozen = false;
            LivingGuiPopulated = false;
            MonikerReady = false;
            GravityReleased = false;
            LivingGuiFallen = false;
            NavigationReady = false;
            GravityActive = false;
            GravityTicks = 0;
        }

        /// <summary>
        /// Advances the living GUI swarm by one heartbeat while its dedicated
        /// process group is active. At exactly 100 nodes the swarm freezes.
        /// No further living-GUI process-group work is required after this point.
        /// </summary>
        public void AdvanceLivingGui()
        {
            if (LivingGuiFrozen || _livingNodes.Count >= CriticalMass)
                return;

            var newborns = new List<LivingNodeState>();

            foreach (var node in _livingNodes)
            {
                node.Size += GrowthPerTick;

                if (node.Size >= MaximumNodeSize)
                {
                    node.Size = MinimumNodeSize;
                    node.OffspringCount++;

                    newborns.Add(CreateNode(
                        $"{node.Lineage}.{node.OffspringCount}",
                        node.Generation + 1));
                }
            }

            var remaining = CriticalMass - _livingNodes.Count;
            if (newborns.Count > remaining)
                newborns.RemoveRange(remaining, newborns.Count - remaining);

            _livingNodes.AddRange(newborns);

            if (_livingNodes.Count == CriticalMass)
            {
                LivingGuiFrozen = true;
                LivingGuiPopulated = true;
            }
        }

        /// <summary>
        /// Creates the visual moniker's runtime phase and resets the independent
        /// gravity clock. The parent FSM enters this state only after critical mass.
        /// </summary>
        public void BeginGravity()
        {
            if (!LivingGuiPopulated)
                return;

            LivingGuiFrozen = true;
            GravityActive = true;
            GravityReleased = true;
            MonikerReady = true;
            GravityTicks = 0;
            NavigationReady = false;
        }

        /// <summary>
        /// Advances the independent gravity process group. The visual acceleration
        /// is rendered by the LivingGui CSS animation; this heartbeat owns the
        /// deterministic three-second handoff to the navigation chrome.
        /// </summary>
        public void AdvanceGravity()
        {
            if (!GravityActive)
                return;

            GravityTicks++;

            // PageFSM runs at 33 ms. 91 ticks is just over three seconds.
            if (GravityTicks >= 91)
            {
                LivingGuiFallen = true;
                NavigationReady = true;
            }
        }

        private LivingNodeState CreateNode(string lineage, int generation)
        {
            var (x, y) = NextBalancedPosition();
            return new LivingNodeState(lineage, generation, x, y, MinimumNodeSize);
        }

        /// <summary>
        /// Prefer the least-populated quadrant, then choose a random point inside it.
        /// Coordinates are local 0-100 percentages of the living panel.
        /// </summary>
        private (double X, double Y) NextBalancedPosition()
        {
            var counts = new int[4];

            foreach (var node in _livingNodes)
            {
                var nodeQuadrant = (node.X >= 50 ? 1 : 0) + (node.Y >= 50 ? 2 : 0);
                counts[nodeQuadrant]++;
            }

            var least = counts[0];
            for (var i = 1; i < counts.Length; i++)
                least = Math.Min(least, counts[i]);

            var candidates = new List<int>(4);
            for (var i = 0; i < counts.Length; i++)
            {
                if (counts[i] == least)
                    candidates.Add(i);
            }

            var selectedQuadrant = candidates[_random.Next(candidates.Count)];

            var xMin = selectedQuadrant is 0 or 2 ? MinX : 50;
            var xMax = selectedQuadrant is 0 or 2 ? 50 : MaxX;
            var yMin = selectedQuadrant is 0 or 1 ? MinY : 50;
            var yMax = selectedQuadrant is 0 or 1 ? 50 : MaxY;

            return (
                xMin + _random.NextDouble() * (xMax - xMin),
                yMin + _random.NextDouble() * (yMax - yMin));
        }

        public sealed class LivingNodeState
        {
            internal LivingNodeState(string lineage, int generation, double x, double y, double size)
            {
                Lineage = lineage;
                Generation = generation;
                X = x;
                Y = y;
                Size = size;
            }

            public string Lineage { get; }
            public int Generation { get; }
            public double X { get; }
            public double Y { get; }
            public double Size { get; internal set; }
            public int OffspringCount { get; internal set; }
        }
    }
}