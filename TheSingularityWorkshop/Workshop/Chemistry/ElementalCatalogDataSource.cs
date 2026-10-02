using System;
using System.Collections.Generic;

namespace TheSingularityWorkshop.Workshop.Chemistry;

/// <summary>
/// In-memory elemental property provider backed by a symbol-keyed data set.
/// This is intentionally a transport adapter, not a scientific data authority.
/// </summary>
public sealed class ElementalCatalogDataSource : IElementalDataSource
{
    private readonly IReadOnlyDictionary<string, ElementalPropertySet> _properties;

    public ElementalCatalogDataSource(
        IReadOnlyDictionary<string, ElementalPropertySet>? properties = null)
    {
        _properties = properties
            ?? new Dictionary<string, ElementalPropertySet>(StringComparer.Ordinal);
    }

    public bool TryGetProperties(string symbol, out ElementalPropertySet? properties)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(symbol);

        if (_properties.TryGetValue(symbol, out var value))
        {
            properties = value;
            return true;
        }

        properties = null;
        return false;
    }
}
