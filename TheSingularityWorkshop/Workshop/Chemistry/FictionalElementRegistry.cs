using System;
using System.Collections.Generic;
using System.Linq;

namespace TheSingularityWorkshop.Workshop.Chemistry;

/// <summary>
/// Runtime registry for fictional or experience-specific elemental identities.
/// Canonical real-world elements remain owned by <see cref="ElementalCatalog"/>.
/// </summary>
public sealed class FictionalElementRegistry
{
    private readonly Dictionary<string, Atom> _elements =
        new(StringComparer.Ordinal);

    public IReadOnlyCollection<Atom> Elements => _elements.Values.ToArray();

    public int Count => _elements.Count;

    public bool Register(Atom element)
    {
        ArgumentNullException.ThrowIfNull(element);

        if (!element.IsFictional)
            throw new ArgumentException("Only fictional elements may be registered.", nameof(element));

        if (ElementalCatalog.Core.ContainsKey(element.Symbol))
            throw new ArgumentException(
                $"The symbol '{element.Symbol}' is already assigned to a canonical element.",
                nameof(element));

        return _elements.TryAdd(element.Symbol, element);
    }

    public bool RegisterRange(IEnumerable<Atom> elements)
    {
        ArgumentNullException.ThrowIfNull(elements);

        var added = false;
        foreach (var element in elements)
            added |= Register(element);

        return added;
    }

    public bool TryGetBySymbol(string symbol, out Atom? element) =>
        _elements.TryGetValue(symbol, out element);

    public Atom GetBySymbol(string symbol) => _elements[symbol];
}
