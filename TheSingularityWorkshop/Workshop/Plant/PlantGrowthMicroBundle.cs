using System;
using System.Collections.Generic;
using System.Linq;
using TheSingularityWorkshop.FSM_API;

namespace TheSingularityWorkshop.Workshop.Plant;

/// <summary>
/// A plant-shaped presentation micro-bundle. FSM_API owns its lifecycle and advances
/// the geometry that the renderer displays.
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

    public sealed class PlantPanel
    {
        internal PlantPanel(string id, string title, double x, double y, double width, double height)
        { Id = id; Title = title; X = x; Y = y; Width = width; Height = height; }

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

    public sealed class PlantVine
    {
        internal PlantVine(string id, string targetId, string path, double speed)
        { Id = id; TargetId = targetId; Path = path; Speed = speed; }

        public string Id { get; }
        public string TargetId { get; }
        public string Path { get; }
        public double Progress { get; internal set; }
        internal double Speed { get; }
        internal int MatureTicks { get; set; }
    }

    private sealed class PlantContext : IStateContext
    {
        public PlantContext()
        {
            Vines = new List<PlantVine>
            {
                new("root-header", "header", "M 2 4 C 10 7 15 4 24 7 S 39 8 50 7", .018),
                new("root-identity", "identity", "M 3 5 C 10 10 14 18 21 18 S 27 15 31 19", .021),
                new("root-context", "context", "M 4 5 C 14 12 22 24 35 25 S 46 19 49 18", .022),
                new("root-navigation", "navigation", "M 3 6 C 13 17 24 34 41 34 S 56 30 63 32", .020),
                new("root-ai", "ai", "M 3 6 C 16 20 30 39 48 44 S 66 42 78 43", .023),
                new("root-content", "content", "M 4 6 C 18 18 32 22 46 20 S 61 19 78 22", .019)
            };

            Panels = new List<PlantPanel>
            {
                new("header", "HUB", 50, 7, 46, 9),
                new("identity", "IDENTITY", 14, 18, 22, 15),
                new("context", "CONTEXT", 38, 18, 22, 15),
                new("navigation", "NAVIGATION", 63, 32, 25, 14),
                new("ai", "AI", 66, 43, 21, 14),
                new("content", "EXPERIENCES", 47, 21, 31, 27)
            };
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