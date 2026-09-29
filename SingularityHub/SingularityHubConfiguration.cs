using System.Collections.ObjectModel;

namespace TheSingularityWorkshop.SingularityHub;

/// <summary>Default in-memory registry for the configuration that defines each Hub level.</summary>
public sealed class SingularityHubConfiguration : ISingularityHubConfiguration
{
    private readonly Dictionary<ulong, SingularityHubDefinition> _hubs = new();
    private readonly Dictionary<string, ulong> _paths = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyCollection<SingularityHubDefinition> Hubs =>
        new ReadOnlyCollection<SingularityHubDefinition>(_hubs.Values.ToList());

    public bool Register(SingularityHubDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        if (_hubs.ContainsKey(definition.Id) || _paths.ContainsKey(definition.Path))
            return false;

        _hubs.Add(definition.Id, definition);
        _paths.Add(definition.Path, definition.Id);
        return true;
    }

    public bool Remove(ulong hubId)
    {
        if (!_hubs.Remove(hubId, out var definition))
            return false;

        _paths.Remove(definition.Path);
        return true;
    }

    public bool TryGet(ulong hubId, out SingularityHubDefinition? definition) =>
        _hubs.TryGetValue(hubId, out definition);

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
