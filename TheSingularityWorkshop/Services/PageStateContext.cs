using System;
using System.Collections.Generic;
using TheSingularityWorkshop.FSM_API;

namespace TheSingularityWorkshop.Services
{
    /// <summary>
    /// Runtime data carried by the page-level FSM.
    /// The Living GUI and its phase FSMs operate on this context; Razor owns no timing.
    /// </summary>
    public sealed class PageStateContext : IStateContext
    {
        public const int CriticalMass = 100;

        private const double RootSize = 200;
        private const double SeedSize = 40;
        private const double DefaultNodeSize = 200;
        private const double GrowthStep = 50;
        private const double MaximumNodeSize = 400;
        private const double MinX = 10;
        private const double MaxX = 90;
        private const double MinY = 10;
        private const double MaxY = 90;
        private const double RootGrowthStep = 200;
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

        public long PopulationCompletedTick { get; internal set; } = -1;
        public long StateTicks { get; set; }
        public long TotalTicks { get; set; }

        public bool LivingGuiFrozen { get; private set; }
        public IReadOnlyList<LivingNodeState> LivingNodes => _livingNodes;

        public bool NeedsRootGrowth => _livingNodes.Exists(node => node.IsRoot && !node.SeedDoubled);
        public bool NeedsChildRooting => !NeedsRootGrowth && _livingNodes.Exists(node => !node.IsRoot && !node.GrowthReady);
        public bool NeedsMatureGrowth => !NeedsRootGrowth && !NeedsChildRooting && _livingNodes.Exists(node => node.Size < MaximumNodeSize);

        public PageStateContext(object? singularityHub = null) => SingularityHub = singularityHub;

        public void ResetStateClock() => StateTicks = 0;

        public void BeginLivingGui()
        {
            if (_livingNodes.Count != 0) return;

            _livingNodes.Add(new LivingNodeState(
                "G:0", 0, 50, 50, RootSize,
                growthReady: true, seedDoubled: false, isRoot: true));

            LivingGuiFrozen = false;
            LivingGuiPopulated = false;
            PopulationCompletedTick = -1;
            MonikerReady = false;
            GravityReleased = false;
            LivingGuiFallen = false;
        }

        public void AdvanceRootGrowth()
        {
            if (LivingGuiFrozen) return;

            foreach (var node in _livingNodes)
            {
                if (!node.IsRoot || node.SeedDoubled) continue;

                node.Size = Math.Min(MaximumNodeSize, node.Size + RootGrowthStep);
                node.SeedDoubled = node.Size >= MaximumNodeSize;
                break;
            }
        }

        public void AdvanceChildRooting()
        {
            if (LivingGuiFrozen) return;

            foreach (var node in _livingNodes)
            {
                if (node.IsRoot || node.GrowthReady) continue;

                node.Size = Math.Min(DefaultNodeSize, node.Size + RootGrowthStep);
                node.GrowthReady = node.Size >= DefaultNodeSize;
                break;
            }
        }

        public void AdvanceMatureGrowth()
        {
            if (LivingGuiFrozen) return;

            foreach (var node in _livingNodes)
            {
                if (!node.GrowthReady || node.Size >= MaximumNodeSize) continue;

                node.Size = Math.Min(MaximumNodeSize, node.Size + GrowthStep);
                break;
            }
        }

        public void AdvanceReproduction()
        {
            if (LivingGuiFrozen || _livingNodes.Count >= CriticalMass)
                return;

            var newborns = new List<LivingNodeState>();

            foreach (var node in _livingNodes)
            {
                if (node.Size < MaximumNodeSize) continue;

                node.OffspringCount++;
                newborns.Add(CreateSeed(node));
            }

            _livingNodes.AddRange(newborns);

            if (_livingNodes.Count >= CriticalMass)
            {
                if (_livingNodes.Count > CriticalMass)
                    _livingNodes.RemoveRange(CriticalMass, _livingNodes.Count - CriticalMass);

                LivingGuiFrozen = true;
                LivingGuiPopulated = true;
                MonikerReady = true;
                PopulationCompletedTick = TotalTicks;
            }
        }

        private LivingNodeState CreateSeed(LivingNodeState parent)
        {
            var (x, y) = NextSafePosition();
            var lineage = parent.Generation == 0
                ? $"G:{parent.OffspringCount}"
                : $"{parent.Lineage}-{parent.OffspringCount - 1}";

            return new LivingNodeState(
                lineage, parent.Generation + 1, x, y, SeedSize,
                growthReady: false, seedDoubled: false, isRoot: false);
        }

        private (double X, double Y) NextSafePosition() =>
            (MinX + _random.NextDouble() * (MaxX - MinX),
             MinY + _random.NextDouble() * (MaxY - MinY));

        public void AdvanceGravity()
        {
            if (!LivingGuiFrozen) return;

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
                string lineage, int generation, double x, double y, double size,
                bool growthReady, bool seedDoubled, bool isRoot)
            {
                Lineage = lineage;
                Generation = generation;
                X = x;
                Y = y;
                Size = size;
                GrowthReady = growthReady;
                SeedDoubled = seedDoubled;
                IsRoot = isRoot;
            }

            public string Lineage { get; }
            public int Generation { get; }
            public double X { get; internal set; }
            public double Y { get; internal set; }
            public double Size { get; internal set; }
            public int OffspringCount { get; internal set; }
            public bool GrowthReady { get; internal set; }
            public bool SeedDoubled { get; internal set; }
            public bool IsRoot { get; }
            public double GravityVelocity { get; internal set; }
            public double Rotation { get; internal set; }
        }
    }
}
