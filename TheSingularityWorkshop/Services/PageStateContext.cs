using System;
using System.Collections.Generic;
using TheSingularityWorkshop.FSM_API;
using TheSingularityWorkshop.Workshop.Randomness;

namespace TheSingularityWorkshop.Services
{
    /// <summary>
    /// Runtime data carried by the page-level FSM.
    /// The Living GUI and each organism's FSM operate on this context; Razor owns no timing.
    /// </summary>
    public sealed class PageStateContext : IStateContext
    {
        public static int PopulationObservationThreshold => WorkshopPresentationProfile.Current.PopulationThreshold;
        private const double RootSize = 50;
        private const double SeedSize = 10;
        private const double DefaultNodeSize = 50;
        private const double MaximumTravelDistance = 0.35;
        private const double GrowthLerp = 0.22;
        private const double TravelLerp = 0.16;
        private const double RotationStep = 24;
        private const double RootGrowthStep = 20;
        private double MaximumNodeSize => _maximumNodeSize;
        private double GravityAcceleration => _gravityAcceleration;
        private readonly SquirrelRng _random = new(0x50414745u);
        private readonly double _maximumNodeSize;
        private readonly double _gravityAcceleration;
        private readonly List<LivingNodeState> _livingNodes = new();
        private bool _enterRequested;

        public string Name { get; set; }
        public bool IsValid { get; set; }
        public object? SingularityHub { get; }
        public bool EnterRequested { get => _enterRequested; set => _enterRequested = value; }
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

        public PageStateContext(object? singularityHub = null)
        {
            Name = "PageFSMContext";
            IsValid = true;
            SingularityHub = singularityHub;
            var profile = WorkshopPresentationProfile.Current;
            _maximumNodeSize = profile.MaximumNodeSize;
            _gravityAcceleration = profile.GravityAcceleration;
        }

        public void ResetStateClock() => StateTicks = 0;

        public void BeginLivingGui()
        {
            if (_livingNodes.Count != 0) return;

            _livingNodes.Add(new LivingNodeState(
                "G:0", 0, 50, 50, RootSize, true, false, true,
                LivingNodePhase.Initialization));

            LivingGuiFrozen = false;
            LivingGuiPopulated = false;
            MonikerReady = false;
            GravityReleased = false;
            LivingGuiFallen = false;
            NavigationReady = false;
            PopulationCompletedTick = -1;
            MonikerPresentationStartTick = -1;
        }

        public void InitializeOrganism(LivingNodeState node)
        {
            if (node.IsRoot)
            {
                node.Phase = LivingNodePhase.Existing;
                return;
            }

            node.TargetX = MinX + _random.NextDouble() * (MaxX - MinX);
            node.TargetY = MinY + _random.NextDouble() * (MaxY - MinY);
            node.Rotation = (_random.NextDouble() * 360.0) - 180.0;
            node.Phase = LivingNodePhase.Traveling;
        }

        public void AdvanceTravel(LivingNodeState node)
        {
            if (LivingGuiFrozen || node.Phase != LivingNodePhase.Traveling) return;

            var dx = node.TargetX - node.X;
            var dy = node.TargetY - node.Y;
            var distance = Math.Sqrt(dx * dx + dy * dy);

            if (distance <= MaximumTravelDistance)
            {
                node.X = node.TargetX;
                node.Y = node.TargetY;
                node.Phase = LivingNodePhase.Planting;
                return;
            }

            node.X = Lerp(node.X, node.TargetX, TravelLerp);
            node.Y = Lerp(node.Y, node.TargetY, TravelLerp);
            node.Rotation += RotationStep;
        }

        public bool IsTravelComplete(LivingNodeState node)
            => Distance(node.X, node.Y, node.TargetX, node.TargetY) <= MaximumTravelDistance;

        public void AdvancePlanting(LivingNodeState node)
        {
            if (LivingGuiFrozen || node.Phase != LivingNodePhase.Planting) return;

            node.Size = Lerp(node.Size, DefaultNodeSize, GrowthLerp);
            if (Math.Abs(node.Size - DefaultNodeSize) <= 0.5)
            {
                node.Size = DefaultNodeSize;
                node.GrowthReady = true;
                node.Phase = LivingNodePhase.Existing;
            }
        }

        public bool IsPlantingComplete(LivingNodeState node)
            => node.Size >= DefaultNodeSize - 0.5;

        public void AdvanceExisting(LivingNodeState node)
        {
            if (LivingGuiFrozen || node.Phase != LivingNodePhase.Existing) return;

            node.Size = Lerp(node.Size, MaximumNodeSize, GrowthLerp);
            if (Math.Abs(node.Size - MaximumNodeSize) <= 0.5)
            {
                node.Size = MaximumNodeSize;
                node.SeedDoubled = true;
                node.Phase = LivingNodePhase.Reproducing;
            }
        }

        public bool IsExistingComplete(LivingNodeState node)
            => node.Size >= MaximumNodeSize - 0.5;

        public LivingNodeState CreateOffspring(LivingNodeState parent)
        {
            parent.OffspringCount++;
            parent.ReproductionComplete = false;

            var child = new LivingNodeState(
                parent.Generation == 0
                    ? $"G:{parent.OffspringCount}"
                    : $"{parent.Lineage}-{parent.OffspringCount - 1}",
                parent.Generation + 1,
                parent.X,
                parent.Y,
                SeedSize,
                false,
                false,
                false,
                LivingNodePhase.Initialization);

            _livingNodes.Add(child);
            return child;
        }

        public void MarkReproductionComplete(LivingNodeState node)
        {
            node.ReproductionComplete = true;
            node.Phase = LivingNodePhase.Reducing;

            if (_livingNodes.Count >= PopulationObservationThreshold && !LivingGuiPopulated)
            {
                LivingGuiPopulated = true;
                PopulationCompletedTick = TotalTicks;
            }
        }

        public bool IsReproductionComplete(LivingNodeState node) => node.ReproductionComplete;

        public void AdvanceReducing(LivingNodeState node)
        {
            if (LivingGuiFrozen || node.Phase != LivingNodePhase.Reducing) return;

            node.Size = Lerp(node.Size, DefaultNodeSize, GrowthLerp);
            if (Math.Abs(node.Size - DefaultNodeSize) <= 0.5)
            {
                node.Size = DefaultNodeSize;
                node.SeedDoubled = false;
                node.Phase = LivingNodePhase.Existing;
            }
        }

        public bool IsReducingComplete(LivingNodeState node)
            => node.Size <= DefaultNodeSize + 0.5;

        public void FreezeLivingGui() => LivingGuiFrozen = true;

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

        private static double Lerp(double current, double target, double amount)
            => current + ((target - current) * amount);

        private static double Distance(double x1, double y1, double x2, double y2)
        {
            var dx = x2 - x1;
            var dy = y2 - y1;
            return Math.Sqrt((dx * dx) + (dy * dy));
        }

        private const double MinX = 10;
        private const double MaxX = 90;
        private const double MinY = 10;
        private const double MaxY = 90;

        public enum LivingNodePhase
        {
            Initialization,
            Traveling,
            Planting,
            Existing,
            Reproducing,
            Reducing
        }

        public sealed class LivingNodeState
        {
            internal LivingNodeState(
                string lineage,
                int generation,
                double x,
                double y,
                double size,
                bool growthReady,
                bool seedDoubled,
                bool isRoot,
                LivingNodePhase phase)
            {
                Lineage = lineage;
                Generation = generation;
                X = x;
                Y = y;
                Size = size;
                GrowthReady = growthReady;
                SeedDoubled = seedDoubled;
                IsRoot = isRoot;
                Phase = phase;
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
            public bool ReproductionComplete { get; internal set; }
            public double GravityVelocity { get; internal set; }
            public double Rotation { get; internal set; }
            public LivingNodePhase Phase { get; internal set; }
            public double TargetX { get; internal set; }
            public double TargetY { get; internal set; }
        }
    }
}