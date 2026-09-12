using System;
using System.Collections.Generic;
using TheSingularityWorkshop.FSM_API;

namespace TheSingularityWorkshop.Services
{
    /// <summary>
    /// Runtime data carried by the page-level FSM.
    /// The living GUI swarm and its presentation gates are state, not Razor-owned timing state.
    /// </summary>
    public sealed class PageStateContext : IStateContext
    {
        public const int CriticalMass = 100;

        private const double SeedSize = 6;
        private const double DefaultNodeSize = 48;
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

            // The first object is deliberately a literal G:0 seed. It is the root
            // from which every later lineage can be read without an external index.
            _livingNodes.Add(new LivingNodeState(
                "G:0", 0, 50, 50, SeedSize, 50, 50, 0, 0, 50, 50,
                growthReady: false, seedDoubled: false));

            LivingGuiFrozen = false;
            LivingGuiPopulated = false;
            MonikerReady = false;
            GravityReleased = false;
            LivingGuiFallen = false;
        }

        /// <summary>
        /// Advances one living seed at a time through the visual lifecycle:
        /// seed -> double -> fly to a random point -> expand to default size -> grow
        /// -> reproduce. The exact 100-node boundary freezes the field.
        /// </summary>
        public void AdvanceLivingGui()
        {
            if (LivingGuiFrozen || _livingNodes.Count >= CriticalMass)
                return;

            var newborns = new List<LivingNodeState>();

            foreach (var node in _livingNodes)
            {
                // A new seed first visibly doubles before it is launched.
                if (!node.SeedDoubled)
                {
                    node.Size = SeedSize * 2;
                    node.SeedDoubled = true;
                    continue;
                }

                // The doubled seed then travels like the Pong ball to its chosen point.
                if (node.SeedTicks < node.SeedFlightDuration)
                {
                    node.SeedTicks++;
                    var progress = (double)node.SeedTicks / node.SeedFlightDuration;
                    node.X = node.SeedX + ((node.TargetX - node.SeedX) * progress);
                    node.Y = node.SeedY + ((node.TargetY - node.SeedY) * progress);
                    continue;
                }

                // On arrival it expands to its normal/default GUI size.
                if (!node.GrowthReady)
                {
                    node.Size = DefaultNodeSize;
                    node.GrowthReady = true;
                    continue;
                }

                // Then it grows. A completed node produces another seed carrying
                // its own readable lineage: G:1 -> G:1-0, G:1-1; G:0 -> G:1, G:2...
                node.Size *= 2;

                if (node.Size >= MaximumNodeSize)
                {
                    node.Size = DefaultNodeSize;
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
                MonikerReady = true;
            }
        }

        private LivingNodeState CreateSeed(LivingNodeState parent)
        {
            var (targetX, targetY) = NextChaoticPosition();
            var flightDuration = _random.Next(MinimumSeedFlightTicks, MaximumSeedFlightTicks + 1);

            var lineage = parent.Generation == 0
                ? $"G:{parent.OffspringCount}"
                : $"{parent.Lineage}-{parent.OffspringCount - 1}";

            return new LivingNodeState(
                lineage,
                parent.Generation + 1,
                parent.X,
                parent.Y,
                SeedSize,
                parent.X,
                parent.Y,
                0,
                flightDuration,
                targetX,
                targetY,
                growthReady: false,
                seedDoubled: false);
        }

        private (double X, double Y) NextChaoticPosition()
        {
            return (
                MinX + _random.NextDouble() * (MaxX - MinX),
                MinY + _random.NextDouble() * (MaxY - MinY));
        }

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
                double targetY,
                bool growthReady,
                bool seedDoubled)
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
                GrowthReady = growthReady;
                SeedDoubled = seedDoubled;
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
            public bool GrowthReady { get; internal set; }
            public bool SeedDoubled { get; internal set; }
            public double GravityVelocity { get; internal set; }
            public double Rotation { get; internal set; }
        }
    }
}
