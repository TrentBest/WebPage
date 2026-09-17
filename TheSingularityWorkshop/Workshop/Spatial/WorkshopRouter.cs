namespace TheSingularityWorkshop.Gui;

/// <summary>Routes traversal through the Workshop's nested spatial scenes.</summary>
public sealed class WorkshopRouter
{
    private readonly SpatialSceneStack _sceneStack;
    private readonly Dictionary<string, SpatialPoint> _entryPoints = new(StringComparer.OrdinalIgnoreCase)
    {
        ["forge"] = new SpatialPoint(50, 78),
        ["line-lab"] = new SpatialPoint(50, 78)
    };
    private readonly HashSet<string> _visitedScenes = new(StringComparer.OrdinalIgnoreCase);

    public WorkshopRouter(string rootSceneId = "workshop")
        => _sceneStack = new SpatialSceneStack(rootSceneId);

    public string CurrentSceneId => _sceneStack.CurrentSceneId;
    public int Depth => _sceneStack.Depth;
    public IReadOnlyList<SpatialSceneFrame> Frames => _sceneStack.Frames;
    public SpatialSceneStack SceneStack => _sceneStack;

    /// <summary>Returns true after the visitor has entered a scene at least once.</summary>
    public bool HasVisited(string sceneId) => _visitedScenes.Contains(sceneId);

    /// <summary>Registers the point at which a visitor materializes inside a child scene.</summary>
    public void RegisterEntryPoint(string sceneId, SpatialPoint entryPoint)
    {
        if (string.IsNullOrWhiteSpace(sceneId)) throw new ArgumentException("A scene ID is required.", nameof(sceneId));
        _entryPoints[sceneId] = entryPoint;
    }

    public SpatialPoint GetEntryPoint(string sceneId)
        => _entryPoints.TryGetValue(sceneId, out var point) ? point : new SpatialPoint(50, 78);

    /// <summary>Enters a child scene and reports whether this is the visitor's first visit.</summary>
    public bool Enter(string sceneId, SpatialPoint returnPosition)
    {
        var firstVisit = _visitedScenes.Add(sceneId);
        _sceneStack.Push(sceneId, returnPosition);
        return firstVisit;
    }

    public bool TryExit(out SpatialSceneFrame frame) => _sceneStack.TryPop(out frame);
}
