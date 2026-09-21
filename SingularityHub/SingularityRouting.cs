using System;
using System.Collections.Generic;

namespace TheSingularityWorkshop.SingularityHub;

/// <summary>
/// Default in-memory implementation of the Hub routing boundary.
/// </summary>
public sealed class SingularityRouting : ISingularityRouting
{
    private readonly Dictionary<string, SingularityRoute> _routes = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Gets all routes currently registered with the router.</summary>
    public IReadOnlyCollection<SingularityRoute> Routes => _routes.Values;

    /// <summary>Registers a route when its path is not already occupied.</summary>
    public bool Register(SingularityRoute route)
    {
        if (route.RouteId == 0 || route.ExperienceId == 0 || string.IsNullOrWhiteSpace(route.Path))
            throw new ArgumentException("A route requires stable identities and a path.", nameof(route));

        if (_routes.ContainsKey(route.Path))
            return false;

        _routes.Add(route.Path, route);
        return true;
    }

    /// <summary>Removes a route by path.</summary>
    public bool Remove(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;
        return _routes.Remove(path);
    }

    /// <summary>Attempts to resolve a route by path.</summary>
    public bool TryResolve(string path, out SingularityRoute route)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            route = default;
            return false;
        }

        return _routes.TryGetValue(path, out route);
    }
}
