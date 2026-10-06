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
        private const double MinX = 10;
        private const double MaxX = 90;
        private const double MinY = 10;
        private const double MaxY = 90;
        private readonly double _maximumNodeSize;
        private readonly double _gravityAcceleration;
        private readonly List<LivingNodeState> _livingNodes = new();
        private readonly List<LivingNodeState> _preallocatedNodes;
        private readonly Stack<LivingNodeState> _availableNodes = new();
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
        public IReadOnlyList<LivingNodeState> PreallocatedLivingNodes => _preallocatedNodes;
        public int AvailablePopulationSlots => _availableNodes.Count;

        public PageStateContext(object? singularityHub = null)
        {
            Name = "PageFSMContext";
            IsValid = true;
            SingularityHub = singularityHub;

            var profile = WorkshopPresentationProfile.Current;
            _maximumNodeSize = profile.MaximumNodeSize;
            _gravityAcceleration = profile.GravityAcceleration;

            var poolSize = Math.Max(profile.PopulationPoolSize, profile.PopulationThreshold);
            _preallocatedNodes = new List<LivingNodeState>(poolSize);
            for (var index = 0; index < poolSize; index++)
            {
                var node = new LivingNodeState(
                    $"POOL:{index}",
                    0,
                    50,
                    50,
                    SeedSize,
                    false,
                    false,
                    index == 0,
                    LivingNodePhase.Dormant);

                _preallocatedNodes.Add(node);
            }

            AssignDistributionTargets(_preallocatedNodes);
        }

        public void ResetStateClock() => StateTicks = 0;

        public void BeginLivingGui()
        {
            if (_livingNodes.Count != 0) return;

            _livingNodes.Clear();
            _availableNodes.Clear();

            for (var index = _preallocatedNodes.Count - 1; index >= 0; index--)
            {
                var node = _preallocatedNodes[index];
                node.Generation = 0;
                node.OffspringCount = 0;
                node.GrowthReady = false;
                node.SeedDoubled = false;
                node.ReproductionComplete = false;
                node.GravityVelocity = 0;
                node.Rotation = 0;
                node.X = 50;
                node.Y = 50;
                node.Size = SeedSize;
                node.Lineage = string.Empty;
                node.IsRoot = false;
                node.Phase = LivingNodePhase.Dormant;
                _availableNodes.Push(node);
            }

            var root = _availableNodes.Pop();
            root.Lineage = "G:0";
            root.Generation = 0;
            root.X = 50;
            root.Y = 50;
            root.Size = RootSize;
            root.GrowthReady = true;
            root.IsRoot = true;
            root.Phase = LivingNodePhase.Initialization;
            _livingNodes.Add(root);

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

            // Targets are precomputed when the 100-slot population is allocated.
            // No RNG object or random stream is created during reproduction.
            node.X = 50;
            node.Y = 50;
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

            node.Size = Lerp(node.Size, _maximumNodeSize, GrowthLerp);
            if (Math.Abs(node.Size - _maximumNodeSize) <= 0.5)
            {
                node.Size = _maximumNodeSize;
                node.SeedDoubled = true;
                node.Phase = LivingNodePhase.Reproducing;
            }
        }

        public bool IsExistingComplete(LivingNodeState node)
            => node.Size >= _maximumNodeSize - 0.5;

        public LivingNodeState CreateOffspring(LivingNodeState parent)
        {
            if (_availableNodes.Count == 0)
                throw new InvalidOperationException("The Living GUI population pool is exhausted.");

            parent.OffspringCount++;
            parent.ReproductionComplete = false;

            var child = _availableNodes.Pop();
            child.Lineage = parent.Generation == 0
                ? $"G:{parent.OffspringCount}"
                : $"{parent.Lineage}-{parent.OffspringCount - 1}";
            child.Generation = parent.Generation + 1;
            child.X = parent.X;
            child.Y = parent.Y;
            child.Size = SeedSize;
            child.OffspringCount = 0;
            child.GrowthReady = false;
            child.SeedDoubled = false;
            child.IsRoot = false;
            child.ReproductionComplete = false;
            child.GravityVelocity = 0;
            child.Rotation = child.InitialRotation;
            child.Phase = LivingNodePhase.Initialization;

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
                node.GravityVelocity += _gravityAcceleration;
                node.Y += node.GravityVelocity;
                node.Rotation += 2.4;
            }

            GravityReleased = true;
            LivingGuiFallen = _livingNodes.TrueForAll(node => node.Y > 125);
        }

        private static void AssignDistributionTargets(IList<LivingNodeState> nodes)
        {
            if (nodes.Count == 0)
                return;

            nodes[0].TargetX = 50;
            nodes[0].TargetY = 50;

            var targetCount = nodes.Count - 1;
            if (targetCount == 0)
                return;

            // Generate random-looking destinations across the whole viewport, then
            // randomize their activation order. The spatial cells prevent a perfectly
            // valid pseudo-random sequence from accidentally launching its first wave
            // from one side of the screen.
            var columns = Math.Max(1, (int)Math.Ceiling(Math.Sqrt(targetCount)));
            var rows = Math.Max(1, (int)Math.Ceiling(targetCount / (double)columns));
            var cellWidth = (MaxX - MinX) / columns;
            var cellHeight = (MaxY - MinY) / rows;
            var targets = new List<(double X, double Y)>(targetCount);

            for (var index = 0; index < targetCount; index++)
            {
                var column = index % columns;
                var row = index / columns;
                var jitterX = (ToUnit(SquirrelRng.Noise((index * 2) + 0, 0x50414745u)) - 0.5) * 0.60;
                var jitterY = (ToUnit(SquirrelRng.Noise((index * 2) + 1, 0x50414745u)) - 0.5) * 0.60;

                targets.Add((
                    MinX + ((column + 0.5 + jitterX) * cellWidth),
                    MinY + ((row + 0.5 + jitterY) * cellHeight)));
            }

            // Fisher-Yates with Squirrel Noise: deterministic for tests, but the
            // population's activation order is no longer tied to row-major cells.
            for (var index = targets.Count - 1; index > 0; index--)
            {
                var swapIndex = (int)(SquirrelRng.Noise(index, 0x53485546u) % (uint)(index + 1));
                (targets[index], targets[swapIndex]) = (targets[swapIndex], targets[index]);
            }

            for (var index = 1; index < nodes.Count; index++)
            {
                var target = targets[index - 1];
                nodes[index].TargetX = target.X;
                nodes[index].TargetY = target.Y;

                var angle = ToUnit(SquirrelRng.Noise((index * 3) + 2, 0x524F5445u)) * (Math.PI * 2.0);
                nodes[index].InitialRotation = (angle * (180.0 / Math.PI)) - 180.0;
                nodes[index].Rotation = nodes[index].InitialRotation;
            }
        }

        private static double ToUnit(uint value) => value / 4294967296d;

        private static double Lerp(double current, double target, double amount)
            => current + ((target - current) * amount);

        private static double Distance(double x1, double y1, double x2, double y2)
        {
            var dx = x2 - x1;
            var dy = y2 - y1;
            return Math.Sqrt((dx * dx) + (dy * dy));
        }

        public enum LivingNodePhase
        {
            Dormant,
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

            public string Lineage { get; internal set; }
            public int Generation { get; internal set; }
            public double X { get; internal set; }
            public double Y { get; internal set; }
            public double Size { get; internal set; }
            public int OffspringCount { get; internal set; }
            public bool GrowthReady { get; internal set; }
            public bool SeedDoubled { get; internal set; }
            public bool IsRoot { get; internal set; }
            public bool ReproductionComplete { get; internal set; }
            public double GravityVelocity { get; internal set; }
            public double Rotation { get; internal set; }
            public double InitialRotation { get; internal set; }
            public LivingNodePhase Phase { get; internal set; }
            public double TargetX { get; internal set; }
            public double TargetY { get; internal set; }
        }
    }
}