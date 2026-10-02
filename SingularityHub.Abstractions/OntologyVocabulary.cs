using TheSingularityWorkshop.ProtocolAi;

namespace TheSingularityWorkshop.SingularityHub;

/// <summary>
/// Adapts ProtocolAI's immutable semantic vocabulary to the nine-layer ontology.
/// Each layer owns a ProtocolAI definition; the ontology runtime carries only the
/// resulting integer token. The human-readable entry remains available at the
/// semantic/authoring boundary.
/// </summary>
public sealed class OntologyVocabulary
{
    private const ulong ProtocolIdBase = 0x4F4E540000000000UL;

    private readonly ProtocolBuilder[] _builders =
        Enumerable.Range(0, OntologySignature.LayerCount)
            .Select(layer => new ProtocolBuilder(
                ProtocolIdBase + (ulong)layer + 1,
                $"Ontology.Layer{layer}"))
            .ToArray();

    private readonly ProtocolDefinition?[] _definitions =
        new ProtocolDefinition?[OntologySignature.LayerCount];

    public int GetOrAdd(int layer, string entry)
    {
        ValidateLayer(layer);
        ArgumentException.ThrowIfNullOrWhiteSpace(entry);

        var definition = _definitions[layer];
        if (definition is not null)
        {
            try
            {
                return checked((int)definition.ReferenceByValue(entry).SymbolId);
            }
            catch (KeyNotFoundException)
            {
            }
        }

        var token = StableToken(entry);

        if (TryGetEntry(layer, token, out var occupied))
        {
            throw new InvalidOperationException(
                $"Ontology token collision in layer {layer}: '{entry}' and '{occupied}' both map to {token}.");
        }

        _builders[layer].Define((ulong)token, entry, entry);
        _definitions[layer] = _builders[layer].Build();
        return token;
    }

    public bool TryGetToken(int layer, string entry, out int token)
    {
        ValidateLayer(layer);
        ArgumentException.ThrowIfNullOrWhiteSpace(entry);

        var definition = _definitions[layer];
        if (definition is null)
        {
            token = 0;
            return false;
        }

        try
        {
            token = checked((int)definition.ReferenceByValue(entry).SymbolId);
            return true;
        }
        catch (KeyNotFoundException)
        {
            token = 0;
            return false;
        }
    }

    public bool TryGetEntry(int layer, int token, out string? entry)
    {
        ValidateLayer(layer);

        var definition = _definitions[layer];
        if (definition is null || token <= 0)
        {
            entry = null;
            return false;
        }

        try
        {
            entry = definition.Decode((ulong)token);
            return true;
        }
        catch (KeyNotFoundException)
        {
            entry = null;
            return false;
        }
    }

    public IReadOnlyDictionary<string, int> GetEntries(int layer)
    {
        ValidateLayer(layer);

        var definition = _definitions[layer];
        if (definition is null)
            return new Dictionary<string, int>(StringComparer.Ordinal);

        return definition.Symbols.ToDictionary(
            symbol => symbol.Value,
            symbol => checked((int)symbol.Id),
            StringComparer.Ordinal);
    }

    public string DescribeLayer(int layer)
    {
        ValidateLayer(layer);
        return (_definitions[layer] ?? _builders[layer].Build()).Describe();
    }

    public static int StableToken(string entry)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entry);

        unchecked
        {
            uint hash = 2166136261u;
            foreach (var character in entry)
            {
                hash ^= character;
                hash *= 16777619u;
            }

            var token = (int)(hash & 0x7FFFFFFFu);
            return token == 0 ? 1 : token;
        }
    }

    private static void ValidateLayer(int layer)
    {
        if ((uint)layer >= OntologySignature.LayerCount)
            throw new ArgumentOutOfRangeException(nameof(layer));
    }
}
