namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Routes traversal through the Workshop's nested spatial scenes.
/// The router owns the hierarchy so presentation pages do not need to know how
/// parent/child traversal history is stored or restored.
/// </summary>
public sealed class WorkshopRouter
{
    private readonly SpatialSceneStack _sceneStack;

    /// <summary>Creates a router rooted at the supplied Workshop scene.</summary>
    public WorkshopRouter(string rootSceneId = "workshop")
        => _sceneStack = new SpatialSceneStack(rootSceneId);

    /// <summary>The scene currently occupied by the visitor.</summary>
    public string CurrentSceneId => _sceneStack.CurrentSceneId;

    /// <summary>Current depth in the virtual spatial hierarchy.</summary>
    public int Depth => _sceneStack.Depth;

    /// <summary>Traversal frames from the root scene to the current scene.</summary>
    public IReadOnlyList<SpatialSceneFrame> Frames => _sceneStack.Frames;

    /// <summary>The underlying traversal stack for diagnostics and tests.</summary>
    public SpatialSceneStack SceneStack => _sceneStack;

    /// <summary>
    /// Routes into a child scene and records the parent's return position.
    /// </summary>
    public void Enter(string sceneId, SpatialPoint returnPosition)
        => _sceneStack.Push(sceneId, returnPosition);

    /// <summary>
    /// Routes back to the parent scene and returns the frame that describes
    /// where the visitor should resume in that parent.
    /// </summary>
    public bool TryExit(out SpatialSceneFrame frame)
        => _sceneStack.TryPop(out frame);
}
