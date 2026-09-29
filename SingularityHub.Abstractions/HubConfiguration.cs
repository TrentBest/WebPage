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
    /// <summary>Gets whether this acquisition identifies a valid provider source.</summary>
    public bool IsValid => Kind == HubTabAcquisitionKind.Provider && SourceId != 0;
}

/// <summary>
/// One tab at one Hub level. Placement is configuration data; content ownership is
/// expressed by TargetKind/TargetId or by Acquisition.
/// </summary>
public sealed record HubTabDefinition
{
    /// <summary>Creates a Hub tab definition.</summary>
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

    /// <summary>Gets the stable tab identity.</summary>
    public ulong Id { get; }
    /// <summary>Gets the human-readable tab label.</summary>
    public string Label { get; }
    /// <summary>Gets the tab placement order within its Hub level.</summary>
    public int Order { get; }
    /// <summary>Gets the kind of target exposed by the tab.</summary>
    public HubTabTargetKind TargetKind { get; }
    /// <summary>Gets the route used to enter the tab.</summary>
    public string Route { get; }
    /// <summary>Gets the target identity when the tab declares one.</summary>
    public ulong? TargetId { get; }
    /// <summary>Gets the provider acquisition information when the tab is acquired.</summary>
    public HubTabAcquisition? Acquisition { get; }
    /// <summary>Gets whether this tab opens another Hub view.</summary>
    public bool IsNestedHub => TargetKind == HubTabTargetKind.Hub;
    /// <summary>Gets whether this tab obtains its content from an acquisition source.</summary>
    public bool IsAcquired => Acquisition.HasValue;
}

/// <summary>
/// Configuration for one rendered Hub level. The renderer should render these tabs
/// at this level; it should not reconstruct them from unrelated application state.
/// </summary>
public sealed class SingularityHubDefinition
{
    /// <summary>Creates a Hub definition with deterministically ordered tabs.</summary>
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

    /// <summary>Gets the stable Hub identity.</summary>
    public ulong Id { get; }
    /// <summary>Gets the canonical path of this Hub level.</summary>
    public string Path { get; }
    /// <summary>Gets the display title of this Hub level.</summary>
    public string Title { get; }
    /// <summary>Gets the deterministically ordered tabs for this Hub level.</summary>
    public IReadOnlyList<HubTabDefinition> Tabs { get; }
}

/// <summary>
/// Hub-owned configuration registry. A host can render the current level from the
/// returned definition and route into nested definitions without creating a second
/// kind of Hub.
/// </summary>
public interface ISingularityHubConfiguration
{
    /// <summary>Gets all registered Hub definitions.</summary>
    IReadOnlyCollection<SingularityHubDefinition> Hubs { get; }
    /// <summary>Registers a Hub definition.</summary>
    bool Register(SingularityHubDefinition definition);
    /// <summary>Removes a Hub definition by identity.</summary>
    bool Remove(ulong hubId);
    /// <summary>Finds a Hub definition by identity.</summary>
    bool TryGet(ulong hubId, out SingularityHubDefinition? definition);
    /// <summary>Finds a Hub definition by canonical path.</summary>
    bool TryResolve(string path, out SingularityHubDefinition? definition);
}
