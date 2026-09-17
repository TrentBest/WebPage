namespace TheSingularityWorkshop.Gui;

/// <summary>
/// A single traversal frame in the virtual spatial hierarchy.
/// The return position belongs to the parent scene and lets reverse traversal
/// restore the visitor to the place from which the child scene was entered.
/// </summary>
public readonly record struct SpatialSceneFrame(
    string SceneId,
    string ParentSceneId,
    SpatialPoint ReturnPosition);

/// <summary>
/// Stack-based traversal state for nested spatial scenes.
/// Entering an interactable pushes a child scene; leaving pops back to its parent.
/// </summary>
public sealed class SpatialSceneStack
{
    private readonly List<SpatialSceneFrame> _frames = [];

    /// <summary>Creates a traversal stack rooted at the supplied scene.</summary>
    public SpatialSceneStack(string rootSceneId)
    {
        if (string.IsNullOrWhiteSpace(rootSceneId))
            throw new ArgumentException("A root scene ID is required.", nameof(rootSceneId));

        _frames.Add(new SpatialSceneFrame(rootSceneId, string.Empty, default));
    }

    /// <summary>The scene currently occupied by the visitor.</summary>
    public string CurrentSceneId => _frames[^1].SceneId;

    /// <summary>Number of scene levels currently occupied.</summary>
    public int Depth => _frames.Count;

    /// <summary>Immutable traversal history from root to current scene.</summary>
    public IReadOnlyList<SpatialSceneFrame> Frames => _frames;

    /// <summary>Enters a child scene and remembers where the visitor came from.</summary>
    public void Push(string sceneId, SpatialPoint returnPosition)
    {
        if (string.IsNullOrWhiteSpace(sceneId))
            throw new ArgumentException("A scene ID is required.", nameof(sceneId));

        _frames.Add(new SpatialSceneFrame(sceneId, CurrentSceneId, returnPosition));
    }

    /// <summary>
    /// Leaves the current child scene. The root scene cannot be popped.
    /// The returned frame contains the parent-scene position to restore.
    /// </summary>
    public bool TryPop(out SpatialSceneFrame frame)
    {
        if (_frames.Count <= 1)
        {
            frame = default;
            return false;
        }

        var index = _frames.Count - 1;
        frame = _frames[index];
        _frames.RemoveAt(index);
        return true;
    }
}
