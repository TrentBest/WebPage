namespace TheSingularityWorkshop.SingularityHub;

/// <summary>Identifies what a Hub tab exposes at its current level.</summary>
public enum HubTabTargetKind
{
    /// <summary>The tab opens an Experience.</summary>
    Experience,
    /// <summary>The tab opens another Hub view. The same Hub implementation may therefore be nested recursively.</summary>
    Hub,
    /// <summary>The tab exposes a tool or other host-owned operational surface.</summary>
    Tool
}

/// <summary>Describes how a Hub tab obtains its content.</summary>
public enum HubTabAcquisitionKind
{
    /// <summary>The tab content is explicitly declared by the Hub configuration.</summary>
    Declared,
    /// <summary>The tab content is supplied by a registered provider/capability.</summary>
    Provider
}

/// <summary>Explicit source information for a tab whose content is acquired rather than declared.</summary>
public readonly record struct HubTabAcquisition(
    HubTabAcquisitionKind Kind,
    ulong SourceId,
    string? Selector = null)
{
    public bool IsValid => Kind == HubTabAcquisitionKind.Provider && SourceId != 0;
}

/// <summary>
/// One tab at one Hub level. Placement is configuration data; content ownership is
/// expressed by TargetKind/TargetId or by Acquisition.
/// </summary>
public sealed record HubTabDefinition
{
    public HubTabDefinition(
        ulong id,
        string label,
        int order,
        HubTabTargetKind targetKind,
        string route,
        ulong? targetId = null,
        HubTabAcquisition? acquisition = null)
    {
        if (id == 0) throw new ArgumentException("A Hub tab requires a stable identity.", nameof(id));
        if (string.IsNullOrWhiteSpace(label)) throw new ArgumentException("A Hub tab requires a label.", nameof(label));
        if (order < 0) throw new ArgumentOutOfRangeException(nameof(order));
        if (string.IsNullOrWhiteSpace(route)) throw new ArgumentException("A Hub tab requires a route.", nameof(route));

        var hasTarget = targetId.GetValueOrDefault() != 0;
        var hasAcquisition = acquisition.HasValue;

        if (hasTarget == hasAcquisition)
            throw new ArgumentException("A Hub tab must have exactly one declared target or acquisition source.");

        if (hasAcquisition && !acquisition!.Value.IsValid)
            throw new ArgumentException("The tab acquisition source is invalid.", nameof(acquisition));

        Id = id;
        Label = label;
        Order = order;
        TargetKind = targetKind;
        Route = route;
        TargetId = targetId;
        Acquisition = acquisition;
    }

    public ulong Id { get; }
    public string Label { get; }
    public int Order { get; }
    public HubTabTargetKind TargetKind { get; }
    public string Route { get; }
    public ulong? TargetId { get; }
    public HubTabAcquisition? Acquisition { get; }

    public bool IsNestedHub => TargetKind == HubTabTargetKind.Hub;
    public bool IsAcquired => Acquisition.HasValue;
}

/// <summary>
/// Configuration for one rendered Hub level. The renderer should render these tabs
/// at this level; it should not reconstruct them from unrelated application state.
/// </summary>
public sealed class SingularityHubDefinition
{
    public SingularityHubDefinition(
        ulong id,
        string path,
        string title,
        IEnumerable<HubTabDefinition> tabs)
    {
        if (id == 0) throw new ArgumentException("A Hub requires a stable identity.", nameof(id));
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("A Hub requires a path.", nameof(path));
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("A Hub requires a title.", nameof(title));
        ArgumentNullException.ThrowIfNull(tabs);

        Id = id;
        Path = path;
        Title = title;
        Tabs = tabs.OrderBy(tab => tab.Order).ThenBy(tab => tab.Id).ToArray();

        if (Tabs.Select(tab => tab.Id).Distinct().Count() != Tabs.Count)
            throw new ArgumentException("Hub tab identities must be unique.", nameof(tabs));

        if (Tabs.Select(tab => tab.Order).Distinct().Count() != Tabs.Count)
            throw new ArgumentException("Hub tab placement order must be unique.", nameof(tabs));
    }

    public ulong Id { get; }
    public string Path { get; }
    public string Title { get; }
    public IReadOnlyList<HubTabDefinition> Tabs { get; }
}

/// <summary>
/// Hub-owned configuration registry. A host can render the current level from the
/// returned definition and route into nested definitions without creating a second
/// kind of Hub.
/// </summary>
public interface ISingularityHubConfiguration
{
    IReadOnlyCollection<SingularityHubDefinition> Hubs { get; }
    bool Register(SingularityHubDefinition definition);
    bool Remove(ulong hubId);
    bool TryGet(ulong hubId, out SingularityHubDefinition? definition);
    bool TryResolve(string path, out SingularityHubDefinition? definition);
}
