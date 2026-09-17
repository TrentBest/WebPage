namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Routes traversal through the Workshop's nested spatial scenes.
/// The router owns the hierarchy and each scene's interior entry point so the
/// presentation page does not need to know how a building is entered.
/// </summary>
public sealed class WorkshopRouter
{
    private readonly SpatialSceneStack _sceneStack;
    private readonly Dictionary<string, SpatialPoint> _entryPoints = new(StringComparer.OrdinalIgnoreCase)
    {
        ["forge"] = new SpatialPoint(50, 78),
        ["line-lab"] = new SpatialPoint(50, 78)
    };

    public WorkshopRouter(string rootSceneId = "workshop")
        => _sceneStack = new SpatialSceneStack(rootSceneId);

    public string CurrentSceneId => _sceneStack.CurrentSceneId;
    public int Depth => _sceneStack.Depth;
    public IReadOnlyList<SpatialSceneFrame> Frames => _sceneStack.Frames;
    public SpatialSceneStack SceneStack => _sceneStack;

    /// <summary>Registers the point at which a visitor materializes inside a child scene.</summary>
    public void RegisterEntryPoint(string sceneId, SpatialPoint entryPoint)
    {
        if (string.IsNullOrWhiteSpace(sceneId))
            throw new ArgumentException("A scene ID is required.", nameof(sceneId));

        _entryPoints[sceneId] = entryPoint;
    }

    /// <summary>Returns the diegetic entry point for a child scene.</summary>
    public SpatialPoint GetEntryPoint(string sceneId)
        => _entryPoints.TryGetValue(sceneId, out var point) ? point : new SpatialPoint(50, 78);

    /// <summary>Enters a child scene and remembers the parent's physical return position.</summary>
    public void Enter(string sceneId, SpatialPoint returnPosition)
        => _sceneStack.Push(sceneId, returnPosition);

    public bool TryExit(out SpatialSceneFrame frame)
        => _sceneStack.TryPop(out frame);
}
