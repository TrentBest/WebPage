using System;
using System.Collections.Generic;
using System.Linq;
using TheSingularityWorkshop.FSM_API;

namespace TheSingularityWorkshop.Workshop.Plant;

/// <summary>
/// A plant-shaped presentation micro-bundle whose growth is constrained by the
/// final Hub geometry. The animation is not a random layout generator: the Hub
/// geometry is the destination and the vines are the construction paths toward it.
/// </summary>
public sealed class PlantGrowthMicroBundle : IDisposable
{
    private readonly string _processingGroup = $"FirstContact.Plant:{Guid.NewGuid():N}";
    private readonly string _definitionName = $"FirstContact.PlantGrowth:{Guid.NewGuid():N}";
    private readonly PlantContext _context = new();
    private readonly FSMHandle _fsm;
    private bool _disposed;

    public PlantGrowthMicroBundle()
    {
        FSM_API.FSM_API.Create.CreateProcessingGroup(_processingGroup);
        FSM_API.FSM_API.Create.CreateFiniteStateMachine(_definitionName, -1, _processingGroup)
            .State("Rooting", null, UpdateRooting, null)
            .State("Crawling", null, UpdateCrawling, null)
            .State("Blooming", null, UpdateBlooming, null)
            .State("Opening", null, UpdateOpening, null)
            .State("Settled", null, null, null)
            .Transition("Rooting", "Crawling", c => ((PlantContext)c).RootProgress >= 1)
            .Transition("Crawling", "Blooming", c => ((PlantContext)c).AllBranchesMature)
            .Transition("Blooming", "Opening", c => ((PlantContext)c).BloomThresholdReached)
            .Transition("Opening", "Settled", c => ((PlantContext)c).Panels.All(p => p.OpenProgress >= 1))
            .WithInitialState("Rooting")
            .BuildDefinition();

        _fsm = FSM_API.FSM_API.Create.CreateInstance(_definitionName, _context, _processingGroup);
    }

    /// <summary>The geometry the growth animation is ultimately constructing.</summary>
    public static IReadOnlyList<HubPanelGeometry> FinalHubGeometry => HubGeometry.Panels;

    public string CurrentState => _fsm.CurrentState;
    public IReadOnlyList<PlantVine> Vines => _context.Vines;
    public IReadOnlyList<PlantPanel> Panels => _context.Panels;
    public bool IsSettled => CurrentState == "Settled";
    public PlantSoundCue LastSoundCue { get; private set; }
    public event Action<PlantSoundCue>? SoundCueRequested;

    public void Start() => _context.Started = true;

    public void Update()
    {
        if (_disposed || !_context.Started) return;

        var before = _fsm.CurrentState;
        FSM_API.FSM_API.Interaction.Update(_processingGroup);

        if (before != _fsm.CurrentState)
        {
            LastSoundCue = _fsm.CurrentState switch
            {
                "Crawling" => PlantSoundCue.VineGrowth,
                "Blooming" => PlantSoundCue.Bloom,
                "Opening" => PlantSoundCue.PanelRip,
                _ => PlantSoundCue.None
            };
            SoundCueRequested?.Invoke(LastSoundCue);
        }
    }

    private void UpdateRooting(IStateContext _) =>
        _context.RootProgress = Math.Min(1, _context.RootProgress + .055);

    private void UpdateCrawling(IStateContext _)
    {
        foreach (var vine in _context.Vines)
        {
            if (vine.Progress < 1)
                vine.Progress = Math.Min(1, vine.Progress + vine.Speed);
            else
                vine.MatureTicks++;
        }
    }

    private void UpdateBlooming(IStateContext _)
    {
        foreach (var panel in _context.Panels)
        {
            panel.BloomProgress = Math.Min(1, panel.BloomProgress + .055);
            if (panel.BloomProgress >= 1) panel.BloomTicks++;
        }
    }

    private void UpdateOpening(IStateContext _)
    {
        foreach (var panel in _context.Panels)
            panel.OpenProgress = Math.Min(1, panel.OpenProgress + .09);
    }

    public void Dispose()
    {
        if (_disposed) return;
        FSM_API.FSM_API.Interaction.DestroyFiniteStateMachine(_definitionName, _processingGroup);
        _disposed = true;
    }

    public enum PlantSoundCue { None, VineGrowth, Bloom, PanelRip }

    /// <summary>
    /// A panel in the final Hub layout. These coordinates are the source of truth
    /// for both the destination UI and the growth animation.
    /// </summary>
    public sealed class PlantPanel
    {
        internal PlantPanel(HubPanelGeometry geometry)
        {
            Id = geometry.Id;
            Title = geometry.Title;
            X = geometry.X;
            Y = geometry.Y;
            Width = geometry.Width;
            Height = geometry.Height;
        }

        public string Id { get; }
        public string Title { get; }
        public double X { get; }
        public double Y { get; }
        public double Width { get; }
        public double Height { get; }
        public double BloomProgress { get; internal set; }
        public int BloomTicks { get; internal set; }
        public double OpenProgress { get; internal set; }
    }

    /// <summary>A growth branch whose destination is a concrete point on a final Hub panel.</summary>
    public sealed class PlantVine
    {
        internal PlantVine(string id, string targetId, string path, double targetX, double targetY, double speed)
        {
            Id = id;
            TargetId = targetId;
            Path = path;
            TargetX = targetX;
            TargetY = targetY;
            Speed = speed;
        }

        public string Id { get; }
        public string TargetId { get; }
        public string Path { get; }
        public double TargetX { get; }
        public double TargetY { get; }
        public double Progress { get; internal set; }
        internal double Speed { get; }
        internal int MatureTicks { get; set; }
    }

    /// <summary>
    /// Final Hub geometry. Growth is derived from this table rather than carrying
    /// a second, hand-tuned set of unrelated coordinates.
    /// </summary>
    public static class HubGeometry
    {
        public static IReadOnlyList<HubPanelGeometry> Panels { get; } = new[]
        {
            new HubPanelGeometry("header", "HUB", 50, 7, 46, 9),
            new HubPanelGeometry("identity", "IDENTITY", 14, 18, 22, 15),
            new HubPanelGeometry("context", "CONTEXT", 38, 18, 22, 15),
            new HubPanelGeometry("navigation", "NAVIGATION", 63, 32, 25, 14),
            new HubPanelGeometry("ai", "AI", 66, 43, 21, 14),
            new HubPanelGeometry("content", "EXPERIENCES", 47, 21, 31, 27)
        };

        internal static string BuildGrowthPath(HubPanelGeometry panel, int index)
        {
            const double rootX = 2;
            const double rootY = 4;

            var targetX = panel.AnchorX;
            var targetY = panel.AnchorY;
            var bend = 7 + index * 1.4;
            var control1X = rootX + (targetX - rootX) * .28;
            var control1Y = rootY + Math.Sign(targetY - rootY) * bend;
            var control2X = targetX - (targetX - rootX) * .22;
            var control2Y = targetY - Math.Sign(targetY - rootY) * bend;

            return $"M {rootX:0.##} {rootY:0.##} C {control1X:0.##} {control1Y:0.##} {control2X:0.##} {control2Y:0.##} {targetX:0.##} {targetY:0.##}";
        }
    }

    public sealed record HubPanelGeometry(
        string Id,
        string Title,
        double X,
        double Y,
        double Width,
        double Height)
    {
        public double Left => X - Width / 2;
        public double Right => X + Width / 2;
        public double Top => Y - Height / 2;
        public double Bottom => Y + Height / 2;

        /// <summary>
        /// Selects a deterministic perimeter point facing the root. The growth branch
        /// therefore appears to enter the panel from its nearest edge.
        /// </summary>
        public double AnchorX => X <= 50 ? Right : Left;

        public double AnchorY => Y <= 12 ? Bottom : Top;
    }

    private sealed class PlantContext : IStateContext
    {
        public PlantContext()
        {
            Vines = HubGeometry.Panels
                .Select((panel, index) => new PlantVine(
                    $"root-{panel.Id}",
                    panel.Id,
                    HubGeometry.BuildGrowthPath(panel, index),
                    panel.AnchorX,
                    panel.AnchorY,
                    .019 + index * .001))
                .ToList();

            Panels = HubGeometry.Panels
                .Select(panel => new PlantPanel(panel))
                .ToList();
        }

        public string Name { get; set; } = "FirstContact.PlantGrowth";
        public bool IsValid { get; set; } = true;
        public bool Started { get; set; }
        public double RootProgress { get; set; }
        public List<PlantVine> Vines { get; }
        public List<PlantPanel> Panels { get; }
        public bool AllBranchesMature => Vines.All(v => v.Progress >= 1 && v.MatureTicks >= 10);
        public bool BloomThresholdReached => Panels.All(p => p.BloomProgress >= 1 && p.BloomTicks >= 8);
    }
}
