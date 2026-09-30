using System.Collections.ObjectModel;

namespace TheSingularityWorkshop.SingularityHub;

/// <summary>Default in-memory registry for the configuration that defines each Hub level.</summary>
public sealed class SingularityHubConfiguration : ISingularityHubConfiguration
{
    private readonly Dictionary<ulong, SingularityHubDefinition> _hubs = new();
    private readonly Dictionary<string, ulong> _paths = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Gets the currently registered Hub definitions.</summary>
    public IReadOnlyCollection<SingularityHubDefinition> Hubs =>
        new ReadOnlyCollection<SingularityHubDefinition>(_hubs.Values.ToList());

    /// <summary>Registers a Hub definition when its ID and path are both unused.</summary>
    /// <param name="definition">The Hub definition to register.</param>
    /// <returns><see langword="true"/> when the definition was registered; otherwise, <see langword="false"/>.</returns>
    public bool Register(SingularityHubDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        if (_hubs.ContainsKey(definition.Id) || _paths.ContainsKey(definition.Path))
            return false;

        _hubs.Add(definition.Id, definition);
        _paths.Add(definition.Path, definition.Id);
        return true;
    }

    /// <summary>Removes a registered Hub definition by ID.</summary>
    /// <param name="hubId">The ID of the Hub to remove.</param>
    /// <returns><see langword="true"/> when a definition was removed; otherwise, <see langword="false"/>.</returns>
    public bool Remove(ulong hubId)
    {
        if (!_hubs.Remove(hubId, out var definition))
            return false;

        _paths.Remove(definition.Path);
        return true;
    }

    /// <summary>Gets a registered Hub definition by ID.</summary>
    /// <param name="hubId">The ID of the Hub to find.</param>
    /// <param name="definition">Receives the matching definition when found; otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> when the Hub exists; otherwise, <see langword="false"/>.</returns>
    public bool TryGet(ulong hubId, out SingularityHubDefinition? definition) =>
        _hubs.TryGetValue(hubId, out definition);

    /// <summary>Resolves a registered Hub definition by its path.</summary>
    /// <param name="path">The path to resolve.</param>
    /// <param name="definition">Receives the matching definition when found; otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> when the path resolves to a Hub; otherwise, <see langword="false"/>.</returns>
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
