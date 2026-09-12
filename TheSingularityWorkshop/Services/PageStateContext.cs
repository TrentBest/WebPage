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
        private const double SeedSize = 6;
        private const double MaximumNodeSize = 96;
        private const double MinX = 8;
        private const double MaxX = 92;
        private const double MinY = 10;
        private const double MaxY = 88;
        private const int MinimumSeedFlightTicks = 2;
        private const int MaximumSeedFlightTicks = 6;
        private const double GravityAcceleration = 1.15;

        private readonly Random _random = new();
        private readonly List<LivingNodeState> _livingNodes = new();

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

        public bool LivingGuiFrozen { get; private set; }
        public IReadOnlyList<LivingNodeState> LivingNodes => _livingNodes;

        public PageStateContext(object? singularityHub = null)
        {
            SingularityHub = singularityHub;
        }

        public void ResetStateClock() => StateTicks = 0;

        public void BeginLivingGui()
        {
            if (_livingNodes.Count != 0) return;

            // The first living GUI is deliberately dead-center. Chaos begins only
            // after the user has entered the experience and the first seed exists.
            _livingNodes.Add(new LivingNodeState(
                "1", 1, 50, 50, SeedSize, 50, 50, 0, 4, 50, 50));

            LivingGuiFrozen = false;
            LivingGuiPopulated = false;
            MonikerReady = false;
            GravityReleased = false;
            LivingGuiFallen = false;
        }

        /// <summary>
        /// Advances the swarm by one FSM heartbeat.
        /// Newborn GUIs are thrown from their parent's location toward independent
        /// destinations while remaining at seed size. Their throw duration is varied,
        /// so the swarm never becomes a synchronized tree layout. Only after the throw
        /// completes does the GUI enter its growth cycle. At maximum size it resets to
        /// seed size and launches a new child. At 100 nodes, all living-GUI updates stop.
        /// </summary>
        public void AdvanceLivingGui()
        {
            if (LivingGuiFrozen || _livingNodes.Count >= CriticalMass)
                return;

            var newborns = new List<LivingNodeState>();

            foreach (var node in _livingNodes)
            {
                if (node.SeedTicks < node.SeedFlightDuration)
                {
                    node.SeedTicks++;
                    var progress = (double)node.SeedTicks / node.SeedFlightDuration;
                    node.X = node.SeedX + ((node.TargetX - node.SeedX) * progress);
                    node.Y = node.SeedY + ((node.TargetY - node.SeedY) * progress);
                    continue;
                }

                // A living GUI doubles before it reproduces. The newborn is tiny and
                // independent, so parent growth and child growth are intentionally out
                // of phase with one another.
                node.Size *= 2;

                if (node.Size >= MaximumNodeSize)
                {
                    node.Size = SeedSize;
                    node.OffspringCount++;
                    newborns.Add(CreateSeed(node));
                }
            }

            _livingNodes.AddRange(newborns);

            if (_livingNodes.Count >= CriticalMass)
            {
                if (_livingNodes.Count > CriticalMass)
                    _livingNodes.RemoveRange(CriticalMass, _livingNodes.Count - CriticalMass);

                LivingGuiFrozen = true;
                LivingGuiPopulated = true;
            }
        }

        private LivingNodeState CreateSeed(LivingNodeState parent)
        {
            var (targetX, targetY) = NextChaoticPosition();
            var flightDuration = _random.Next(MinimumSeedFlightTicks, MaximumSeedFlightTicks + 1);

            return new LivingNodeState(
                $"{parent.Lineage}.{parent.OffspringCount}",
                parent.Generation + 1,
                parent.X,
                parent.Y,
                SeedSize,
                parent.X,
                parent.Y,
                0,
                flightDuration,
                targetX,
                targetY);
        }

        private (double X, double Y) NextChaoticPosition()
        {
            // Randomized, bounded destinations deliberately replace quadrant balancing.
            // The result should feel alive rather than like a tree being laid out.
            return (
                MinX + _random.NextDouble() * (MaxX - MinX),
                MinY + _random.NextDouble() * (MaxY - MinY));
        }

        /// <summary>
        /// Starts physics only after the moniker has been instantiated by the FSM.
        /// </summary>
        public void AdvanceGravity()
        {
            if (!LivingGuiFrozen)
                return;

            foreach (var node in _livingNodes)
            {
                node.GravityVelocity += GravityAcceleration;
                node.Y += node.GravityVelocity;
                node.Rotation += 2.4;
            }

            GravityReleased = true;
            LivingGuiFallen = _livingNodes.TrueForAll(node => node.Y > 125);
        }

        public sealed class LivingNodeState
        {
            internal LivingNodeState(
                string lineage,
                int generation,
                double x,
                double y,
                double size,
                double seedX,
                double seedY,
                int seedTicks,
                int seedFlightDuration,
                double targetX,
                double targetY)
            {
                Lineage = lineage;
                Generation = generation;
                X = x;
                Y = y;
                Size = size;
                SeedX = seedX;
                SeedY = seedY;
                SeedTicks = seedTicks;
                SeedFlightDuration = seedFlightDuration;
                TargetX = targetX;
                TargetY = targetY;
            }

            public string Lineage { get; }
            public int Generation { get; }
            public double X { get; internal set; }
            public double Y { get; internal set; }
            public double Size { get; internal set; }
            public int OffspringCount { get; internal set; }
            public double SeedX { get; }
            public double SeedY { get; }
            public int SeedTicks { get; internal set; }
            public int SeedFlightDuration { get; }
            public double TargetX { get; }
            public double TargetY { get; }
            public double GravityVelocity { get; internal set; }
            public double Rotation { get; internal set; }
        }
    }
}