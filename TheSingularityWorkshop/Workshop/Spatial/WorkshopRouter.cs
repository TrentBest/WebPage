namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Routes traversal through nested spatial scenes. Root-world destinations are
/// entered through <see cref="SpatialPlace"/> so presentation code cannot silently
/// substitute a different scene ID or interior entry point.
/// </summary>
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

    public bool HasVisited(string sceneId) => _visitedScenes.Contains(sceneId);

    public void RegisterEntryPoint(string sceneId, SpatialPoint entryPoint)
    {
        if (string.IsNullOrWhiteSpace(sceneId)) throw new ArgumentException("A scene ID is required.", nameof(sceneId));
        _entryPoints[sceneId] = entryPoint;
    }

    /// <summary>Registers a complete place boundary: identity plus its interior materialization point.</summary>
    public void RegisterPlace(SpatialPlace place)
    {
        ArgumentNullException.ThrowIfNull(place);
        RegisterEntryPoint(place.SceneId, place.EntryPoint);
    }

    public SpatialPoint GetEntryPoint(string sceneId)
        => _entryPoints.TryGetValue(sceneId, out var point) ? point : new SpatialPoint(50, 78);

    public bool Enter(SpatialPlace place, SpatialPoint returnPosition)
    {
        ArgumentNullException.ThrowIfNull(place);
        return Enter(place.SceneId, returnPosition);
    }

    /// <summary>Enters a child scene by scene ID. Use the place overload for root-world destinations.</summary>
    public bool Enter(string sceneId, SpatialPoint returnPosition)
    {
        var firstVisit = _visitedScenes.Add(sceneId);
        _sceneStack.Push(sceneId, returnPosition);
        return firstVisit;
    }

    public bool TryExit(out SpatialSceneFrame frame) => _sceneStack.TryPop(out frame);
}
