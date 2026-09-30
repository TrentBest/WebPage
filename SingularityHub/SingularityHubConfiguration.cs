using System.Collections.ObjectModel;

namespace TheSingularityWorkshop.SingularityHub;

/// <summary>Default in-memory registry for the configuration that defines each Hub level.</summary>
public sealed class SingularityHubConfiguration : ISingularityHubConfiguration
{
    private readonly Dictionary<ulong, SingularityHubDefinition> _hubs = new();
    private readonly Dictionary<string, ulong> _paths = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Gets all currently registered Hub definitions.</summary>
    public IReadOnlyCollection<SingularityHubDefinition> Hubs =>
        new ReadOnlyCollection<SingularityHubDefinition>(_hubs.Values.ToList());

    /// <summary>Registers a Hub definition unless its identity or path is already registered.</summary>
    public bool Register(SingularityHubDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        if (_hubs.ContainsKey(definition.Id) || _paths.ContainsKey(definition.Path))
            return false;

        _hubs.Add(definition.Id, definition);
        _paths.Add(definition.Path, definition.Id);
        return true;
    }

    /// <summary>Removes a registered Hub definition by stable identity.</summary>
    public bool Remove(ulong hubId)
    {
        if (!_hubs.Remove(hubId, out var definition))
            return false;

        _paths.Remove(definition.Path);
        return true;
    }

    /// <summary>Attempts to retrieve a Hub definition by stable identity.</summary>
    public bool TryGet(ulong hubId, out SingularityHubDefinition? definition) =>
        _hubs.TryGetValue(hubId, out definition);

    /// <summary>Attempts to resolve a Hub definition by canonical path.</summary>
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
