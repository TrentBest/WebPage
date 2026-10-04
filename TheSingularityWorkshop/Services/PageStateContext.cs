using System;
using System.Collections.Generic;
using TheSingularityWorkshop.FSM_API;
using TheSingularityWorkshop.Workshop.Randomness;

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
        private const double SeedFlightStep = 4.5;
        private const double SeedScalingStep = 40;
        private const double ParentRecoveryStep = 200;
        private const double MinX = 10;
        private const double MaxX = 90;
        private const double MinY = 10;
        private const double MaxY = 90;
        private const double RootGrowthStep = 50;
        private const double GravityAcceleration = 1.15;
        private readonly SquirrelRng _random = new(0x50414745u);
        private readonly List<LivingNodeState> _livingNodes = new();
        private int _matureGrowthCursor;
        private bool _enterRequested;

        public string Name { get; set; }
        public bool IsValid { get; set; }
        public object? SingularityHub { get; }
        public bool EnterRequested
        {
            get => _enterRequested;
            set => _enterRequested = value;
        }
        public bool LivingGuiPopulated { get; internal set; }
        public bool MonikerReady { get; internal set; }
        public bool GravityReleased { get; internal set; }
        public bool LivingGuiFallen { get; internal set; }
        public bool NavigationReady { get; internal set; }
        public long PopulationCompletedTick { get; internal set; } = -1;
        public long MonikerPresentationStartTick { get; internal set; } = -1;
        public long StateTicks { get; set; }
        public long TotalTicks { get; set; }
        public bool LivingGuiFrozen { get; private set; }
        public IReadOnlyList<LivingNodeState> LivingNodes => _livingNodes;
        public bool NeedsRootGrowth => _livingNodes.Exists(node => node.Phase == LivingNodePhase.RootGrowth);
        public bool NeedsSeedFlight => _livingNodes.Exists(node => node.Phase == LivingNodePhase.SeedFlight);
        public bool NeedsSeedScaling => _livingNodes.Exists(node => node.Phase == LivingNodePhase.SeedScaling);
        public bool NeedsMatureGrowth => _livingNodes.Exists(node => node.Phase == LivingNodePhase.MatureGrowth && node.Size < MaximumNodeSize);
        public bool NeedsReproduction => _livingNodes.Exists(node => node.Phase == LivingNodePhase.ReproductionPending);
        public bool NeedsParentRecovery => _livingNodes.Exists(node => node.Phase == LivingNodePhase.ParentRecovery);

        public PageStateContext(object? singularityHub = null)
        {
            Name = "PageFSMContext";
            IsValid = true;
            SingularityHub = singularityHub;
        }

        public void ResetStateClock() => StateTicks = 0;

        public void BeginLivingGui()
        {
            if (_livingNodes.Count != 0) return;
            _livingNodes.Add(new LivingNodeState("G:0", 0, 50, 50, RootSize, growthReady: true, seedDoubled: false, isRoot: true, phase: LivingNodePhase.RootGrowth));
            _matureGrowthCursor = 0;
            LivingGuiFrozen = false;
            LivingGuiPopulated = false;
            MonikerReady = false;
            GravityReleased = false;
            LivingGuiFallen = false;
            NavigationReady = false;
            PopulationCompletedTick = -1;
            MonikerPresentationStartTick = -1;
        }

        public void AdvanceRootGrowth()
        {
            if (LivingGuiFrozen) return;
            foreach (var node in _livingNodes)
            {
                if (node.Phase != LivingNodePhase.RootGrowth) continue;
                node.Size = Math.Min(MaximumNodeSize, node.Size + RootGrowthStep);
                node.SeedDoubled = node.Size >= MaximumNodeSize;
                if (node.SeedDoubled) node.Phase = LivingNodePhase.ReproductionPending;
                break;
            }
        }

        public void AdvanceSeedFlight()
        {
            if (LivingGuiFrozen) return;
            foreach (var node in _livingNodes)
            {
                if (node.Phase != LivingNodePhase.SeedFlight) continue;
                var dx = node.TargetX - node.X;
                var dy = node.TargetY - node.Y;
                var distance = Math.Sqrt(dx * dx + dy * dy);
                if (distance <= SeedFlightStep)
                {
                    node.X = node.TargetX;
                    node.Y = node.TargetY;
                    node.Phase = LivingNodePhase.SeedScaling;
                    continue;
                }
                node.X += dx / distance * SeedFlightStep;
                node.Y += dy / distance * SeedFlightStep;
                node.Rotation += 4.0;
            }
        }

        public void AdvanceSeedScaling()
        {
            if (LivingGuiFrozen) return;
            foreach (var node in _livingNodes)
            {
                if (node.Phase != LivingNodePhase.SeedScaling) continue;
                node.Size = Math.Min(DefaultNodeSize, node.Size + SeedScalingStep);
                node.GrowthReady = node.Size >= DefaultNodeSize;
                if (node.GrowthReady) node.Phase = LivingNodePhase.MatureGrowth;
            }
        }

        public void AdvanceMatureGrowth()
        {
            if (LivingGuiFrozen || _livingNodes.Count == 0) return;
            for (var offset = 0; offset < _livingNodes.Count; offset++)
            {
                var index = (_matureGrowthCursor + offset) % _livingNodes.Count;
                var node = _livingNodes[index];
                if (node.Phase != LivingNodePhase.MatureGrowth || node.Size >= MaximumNodeSize) continue;
                node.Size = Math.Min(MaximumNodeSize, node.Size + GrowthStep);
                if (node.Size >= MaximumNodeSize)
                {
                    node.SeedDoubled = true;
                    node.Phase = LivingNodePhase.ReproductionPending;
                }
                _matureGrowthCursor = (index + 1) % _livingNodes.Count;
                break;
            }
        }

        public void AdvanceReproduction()
        {
            if (LivingGuiFrozen || _livingNodes.Count >= CriticalMass) return;
            var newborns = new List<LivingNodeState>();
            foreach (var node in _livingNodes)
            {
                if (node.Phase != LivingNodePhase.ReproductionPending) continue;
                node.OffspringCount++;
                newborns.Add(CreateSeed(node));
                node.Phase = LivingNodePhase.ParentRecovery;
            }
            _livingNodes.AddRange(newborns);

            // The first offspring marks the end of the opening population phase.
            // It is a handoff signal, not a population cap: the living GUI continues
            // reproducing while the moniker is presented.
            if (newborns.Count > 0 && !LivingGuiPopulated)
            {
                LivingGuiPopulated = true;
                MonikerReady = true;
                PopulationCompletedTick = TotalTicks;
            }
        }

        public void FreezeLivingGui()
        {
            LivingGuiFrozen = true;
        }

        public void AdvanceParentRecovery()
        {
            if (LivingGuiFrozen) return;
            foreach (var node in _livingNodes)
            {
                if (node.Phase != LivingNodePhase.ParentRecovery) continue;
                node.Size = Math.Max(DefaultNodeSize, node.Size - ParentRecoveryStep);
                if (node.Size <= DefaultNodeSize)
                {
                    node.Size = DefaultNodeSize;
                    node.SeedDoubled = false;
                    node.Phase = LivingNodePhase.MatureGrowth;
                }
            }
        }

        private LivingNodeState CreateSeed(LivingNodeState parent)
        {
            var (x, y) = NextSafePosition();
            var lineage = parent.Generation == 0 ? $"G:{parent.OffspringCount}" : $"{parent.Lineage}-{parent.OffspringCount - 1}";
            var rotation = (_random.NextDouble() * 360.0) - 180.0;
            return new LivingNodeState(lineage, parent.Generation + 1, parent.X, parent.Y, SeedSize, growthReady: false, seedDoubled: false, isRoot: false, phase: LivingNodePhase.SeedFlight, targetX: x, targetY: y, rotation: rotation);
        }

        private (double X, double Y) NextSafePosition() => (MinX + _random.NextDouble() * (MaxX - MinX), MinY + _random.NextDouble() * (MaxY - MinY));

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

        public enum LivingNodePhase { RootGrowth, SeedFlight, SeedScaling, MatureGrowth, ReproductionPending, ParentRecovery }

        public sealed class LivingNodeState
        {
            internal LivingNodeState(string lineage, int generation, double x, double y, double size, bool growthReady, bool seedDoubled, bool isRoot, LivingNodePhase phase, double targetX = 0, double targetY = 0, double rotation = 0)
            {
                Lineage = lineage; Generation = generation; X = x; Y = y; Size = size; GrowthReady = growthReady; SeedDoubled = seedDoubled; IsRoot = isRoot; Phase = phase; TargetX = targetX; TargetY = targetY; Rotation = rotation;
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
            public LivingNodePhase Phase { get; internal set; }
            public double TargetX { get; }
            public double TargetY { get; }
        }
    }
}