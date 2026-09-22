using System;
using System.Collections.Generic;
using System.Linq;
using TheSingularityWorkshop.FSM_API;
using TheSingularityWorkshop.Workshop.Motion;

namespace TheSingularityWorkshop.Workshop.Plant;

/// <summary>
/// Builds the Workshop Hub as a living GUI mesh.
///
/// The final navigation geometry is authored first and is intentionally invisible
/// during presentation. A common root grows into that mesh through deterministic,
/// independently clocked vine FSMs. Each vine reaches its own slot, flowers, and
/// leaves its panel in its final position. The result is a construction animation,
/// not a second layout that later has to be pushed into place.
/// </summary>
public sealed class PlantGrowthMicroBundle : IDisposable
{
    private readonly string _processingGroup = $"FirstContact.Plant:{Guid.NewGuid():N}";
    private readonly string _definitionName = $"FirstContact.PlantGrowth:{Guid.NewGuid():N}";
    private readonly string _vineDefinitionName = $"FirstContact.PlantVine:{Guid.NewGuid():N}";
    private readonly PlantContext _context = new();
    private readonly List<FSMHandle> _vineFsms = new();
    private readonly FSMHandle _fsm;
    private bool _disposed;

    public PlantGrowthMicroBundle()
    {
        FSM_API.FSM_API.Create.CreateProcessingGroup(_processingGroup);

        FSM_API.FSM_API.Create.CreateFiniteStateMachine(_vineDefinitionName, -1, _processingGroup)
            .State("Waiting", null, UpdateVineWaiting, null)
            .State("Growing", null, UpdateVineGrowing, null)
            .State("Blooming", null, UpdateVineBlooming, null)
            .State("Settled", null, null, null)
            .Transition("Waiting", "Growing", c => ((VineContext)c).PhaseTicks >= ((VineContext)c).PhaseDelayTicks)
            .Transition("Growing", "Blooming", c => ((VineContext)c).PathProgress >= 1)
            .Transition("Blooming", "Settled", c => ((VineContext)c).BloomClock >= 1)
            .WithInitialState("Waiting")
            .BuildDefinition();

        FSM_API.FSM_API.Create.CreateFiniteStateMachine(_definitionName, -1, _processingGroup)
            .State("Rooting", null, UpdateRooting, null)
            .State("Growing", null, UpdateGrowing, null)
            .State("Blooming", null, UpdateBlooming, null)
            .State("Settled", null, null, null)
            .Transition("Rooting", "Growing", c => ((PlantContext)c).RootProgress >= 1)
            .Transition("Growing", "Blooming", c => ((PlantContext)c).AllVinesAtDestinations)
            .Transition("Blooming", "Settled", c => ((PlantContext)c).AllPanelsSettled)
            .WithInitialState("Rooting")
            .BuildDefinition();

        foreach (var vine in _context.Vines)
        {
            var vineContext = new VineContext(vine);
            vine.Runtime = vineContext;
            _vineFsms.Add(FSM_API.FSM_API.Create.CreateInstance(_vineDefinitionName, vineContext, _processingGroup));
        }

        _fsm = FSM_API.FSM_API.Create.CreateInstance(_definitionName, _context, _processingGroup);
    }

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
                "Growing" => PlantSoundCue.VineGrowth,
                "Blooming" => PlantSoundCue.Bloom,
                "Settled" => PlantSoundCue.Settle,
                _ => PlantSoundCue.None
            };
            SoundCueRequested?.Invoke(LastSoundCue);
        }
    }

    private void UpdateRooting(IStateContext _) =>
        _context.RootProgress = Math.Min(1, _context.RootProgress + .08);

    private void UpdateGrowing(IStateContext _) { }

    private void UpdateBlooming(IStateContext _)
    {
        foreach (var vine in _context.Vines)
        {
            var panel = _context.Panels.First(p => p.Id == vine.TargetId);
            panel.BloomProgress = vine.BloomProgress;
            if (panel.BloomProgress >= 1)
                panel.BloomTicks++;
        }
    }

    private static void UpdateVineWaiting(IStateContext context)
    {
        var state = (VineContext)context;
        state.PhaseTicks++;
        state.Vine.BloomProgress = 0;
    }

    private static void UpdateVineGrowing(IStateContext context)
    {
        var state = (VineContext)context;
        var vine = state.Vine;

        state.PathProgress = Math.Min(1, state.PathProgress + vine.Speed);
        vine.Progress = MotionLutMicroBundle.SmoothStep(state.PathProgress);

        if (state.PathProgress >= 1)
            vine.Progress = 1;
    }

    private static void UpdateVineBlooming(IStateContext context)
    {
        var state = (VineContext)context;
        var vine = state.Vine;

        state.BloomClock = Math.Min(1, state.BloomClock + .075);
        vine.BloomProgress = state.BloomClock >= 1
            ? 1
            : MotionLutMicroBundle.BounceOut(state.BloomClock);

        if (state.BloomClock >= 1)
            vine.BloomProgress = 1;
    }

    public void Dispose()
    {
        if (_disposed) return;
        FSM_API.FSM_API.Interaction.DestroyFiniteStateMachine(_definitionName, _processingGroup);
        FSM_API.FSM_API.Interaction.DestroyFiniteStateMachine(_vineDefinitionName, _processingGroup);
        _disposed = true;
    }

    public enum PlantSoundCue { None, VineGrowth, Bloom, Settle }

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
            MeshSlot = geometry.MeshSlot;
        }

        public string Id { get; }
        public string Title { get; }
        public double X { get; }
        public double Y { get; }
        public double Width { get; }
        public double Height { get; }
        public int MeshSlot { get; }
        public double BloomProgress { get; internal set; }
        public int BloomTicks { get; internal set; }
    }

    public sealed class PlantVine
    {
        internal PlantVine(string id, string targetId, string path, double targetX, double targetY, double speed, int phaseDelayTicks)
        {
            Id = id;
            TargetId = targetId;
            Path = path;
            TargetX = targetX;
            TargetY = targetY;
            Speed = speed;
            PhaseDelayTicks = phaseDelayTicks;
        }

        public string Id { get; }
        public string TargetId { get; }
        public string Path { get; }
        public double TargetX { get; }
        public double TargetY { get; }
        public double Speed { get; }
        public int PhaseDelayTicks { get; }
        public double Progress { get; internal set; }
        public double BloomProgress { get; internal set; }
        internal VineContext? Runtime { get; set; }
    }

    public static class HubGeometry
    {
        public static SpatialMeshBounds MeshBounds { get; } = new(4, 4, 92, 52);
        public static SpatialPoint Root => new(MeshBounds.X, MeshBounds.Y);

        public static IReadOnlyList<HubPanelGeometry> Panels { get; } = new[]
        {
            new HubPanelGeometry("header", "HUB", 11, 7, 16, 6, 0),
            new HubPanelGeometry("identity", "IDENTITY", 11, 16, 16, 6, 1),
            new HubPanelGeometry("context", "CONTEXT", 11, 25, 16, 6, 2),
            new HubPanelGeometry("navigation", "NAVIGATION", 11, 34, 16, 6, 3),
            new HubPanelGeometry("ai", "AI", 11, 43, 16, 6, 4),
            new HubPanelGeometry("content", "EXPERIENCES", 11, 52, 16, 6, 5)
        };

        internal static string BuildGrowthPath(HubPanelGeometry panel, int index)
        {
            var root = Root;
            var targetX = panel.AnchorX;
            var targetY = panel.AnchorY;
            var dx = targetX - root.X;
            var dy = targetY - root.Y;
            var bend = 7 + index * 1.25;
            var sign = Math.Sign(dy == 0 ? 1 : dy);

            var p1x = Clamp(root.X + dx * .18 + index * 2.2, MeshBounds.X, MeshBounds.Right);
            var p1y = Clamp(root.Y + sign * bend, MeshBounds.Y, MeshBounds.Bottom);
            var p2x = Clamp(root.X + dx * .42 - index * 1.1, MeshBounds.X, MeshBounds.Right);
            var p2y = Clamp(root.Y + dy * .28 - sign * bend, MeshBounds.Y, MeshBounds.Bottom);
            var p3x = Clamp(root.X + dx * .66 + Math.Sin(index * 1.7) * 4.5, MeshBounds.X, MeshBounds.Right);
            var p3y = Clamp(root.Y + dy * .62 + Math.Cos(index * 1.3) * 3.5, MeshBounds.Y, MeshBounds.Bottom);
            var p4x = Clamp(targetX - dx * .16, MeshBounds.X, MeshBounds.Right);
            var p4y = Clamp(targetY - dy * .12, MeshBounds.Y, MeshBounds.Bottom);

            return $"M {root.X:0.##} {root.Y:0.##} C {p1x:0.##} {p1y:0.##} {p2x:0.##} {p2y:0.##} {p3x:0.##} {p3y:0.##} S {p4x:0.##} {p4y:0.##} {targetX:0.##} {targetY:0.##}";
        }

        private static double Clamp(double value, double min, double max) => Math.Clamp(value, min, max);
    }

    public readonly record struct SpatialPoint(double X, double Y);

    public readonly record struct SpatialMeshBounds(double X, double Y, double Width, double Height)
    {
        public double Right => X + Width;
        public double Bottom => Y + Height;
        public bool Contains(SpatialPoint point) => point.X >= X && point.X <= Right && point.Y >= Y && point.Y <= Bottom;
    }

    public sealed record HubPanelGeometry(
        string Id,
        string Title,
        double X,
        double Y,
        double Width,
        double Height,
        int MeshSlot)
    {
        public double Left => X - Width / 2;
        public double Right => X + Width / 2;
        public double Top => Y - Height / 2;
        public double Bottom => Y + Height / 2;
        public double AnchorX => X + Width / 2;
        public double AnchorY => Y;
    }

    private sealed class PlantContext : IStateContext
    {
        public PlantContext()
        {
            Panels = HubGeometry.Panels.Select(panel => new PlantPanel(panel)).ToList();
            Vines = HubGeometry.Panels
                .Select((panel, index) => new PlantVine(
                    $"root-{panel.Id}",
                    panel.Id,
                    HubGeometry.BuildGrowthPath(panel, index),
                    panel.AnchorX,
                    panel.AnchorY,
                    .026 + index * .002,
                    5 + index * 7))
                .ToList();
        }

        public string Name { get; set; } = "FirstContact.PlantGrowth";
        public bool IsValid { get; set; } = true;
        public bool Started { get; set; }
        public double RootProgress { get; set; }
        public List<PlantVine> Vines { get; }
        public List<PlantPanel> Panels { get; }
        public bool AllVinesAtDestinations => Vines.All(v => v.Progress >= 1);
        public bool AllPanelsSettled => Vines.All(v => v.BloomProgress >= 1);
    }

    private sealed class VineContext : IStateContext
    {
        public VineContext(PlantVine vine) => Vine = vine;
        public string Name { get; set; } = "FirstContact.PlantVine";
        public bool IsValid { get; set; } = true;
        public PlantVine Vine { get; }
        public int PhaseTicks { get; set; }
        public double PathProgress { get; set; }
        public double BloomClock { get; set; }
        public int PhaseDelayTicks => Vine.PhaseDelayTicks;
    }
}