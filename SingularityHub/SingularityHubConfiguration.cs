using System.Collections.ObjectModel;

namespace TheSingularityWorkshop.SingularityHub;

/// <summary>Default in-memory registry for the configuration that defines each Hub level.</summary>
public sealed class SingularityHubConfiguration : ISingularityHubConfiguration
{
    private readonly Dictionary<ulong, SingularityHubDefinition> _hubs = new();
    private readonly Dictionary<string, ulong> _paths = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Gets all registered Hub definitions.</summary>
    public IReadOnlyCollection<SingularityHubDefinition> Hubs =>
        new ReadOnlyCollection<SingularityHubDefinition>(_hubs.Values.ToList());

    /// <summary>Registers a Hub definition when its identity and path are unused.</summary>
    /// <param name="definition">Definition to register.</param>
    /// <returns><see langword="true"/> when registration succeeds.</returns>
    public bool Register(SingularityHubDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        if (_hubs.ContainsKey(definition.Id) || _paths.ContainsKey(definition.Path))
            return false;

        _hubs.Add(definition.Id, definition);
        _paths.Add(definition.Path, definition.Id);
        return true;
    }

    /// <summary>Removes a Hub definition by identity.</summary>
    /// <param name="hubId">Stable Hub identity.</param>
    /// <returns><see langword="true"/> when a definition was removed.</returns>
    public bool Remove(ulong hubId)
    {
        if (!_hubs.Remove(hubId, out var definition))
            return false;

        _paths.Remove(definition.Path);
        return true;
    }

    /// <summary>Gets a Hub definition by identity.</summary>
    /// <param name="hubId">Stable Hub identity.</param>
    /// <param name="definition">Resolved definition when present.</param>
    /// <returns><see langword="true"/> when the Hub was found.</returns>
    public bool TryGet(ulong hubId, out SingularityHubDefinition? definition) =>
        _hubs.TryGetValue(hubId, out definition);

    /// <summary>Gets a Hub definition by routing path.</summary>
    /// <param name="path">Hub path to resolve.</param>
    /// <param name="definition">Resolved definition when present.</param>
    /// <returns><see langword="true"/> when the Hub was found.</returns>
    public bool TryResolve(string path, out SingularityHubDefinition? definition)
    {
        if (string.IsNullOrWhiteSpace(path) ||
            !_paths.TryGetValue(path, out var hubId))
        {
            definition = null;
            return false;
        }

        return _hubs.TryGetValue(hubId, out definition);
    }
}
