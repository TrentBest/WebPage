namespace TheSingularityWorkshop.SingularityHub;

/// <summary>
/// Retains the human-readable vocabulary behind the nine integer ontology layers.
/// Runtime routing uses only the integer token; strings remain available at the
/// authoring/debugging boundary. Tokens are deterministic FNV-1a values so the
/// same canonical entry resolves to the same integer across processes.
/// </summary>
public sealed class OntologyVocabulary
{
    private readonly Dictionary<string, int>[] _nameToToken =
        Enumerable.Range(0, OntologySignature.LayerCount)
            .Select(_ => new Dictionary<string, int>(StringComparer.Ordinal))
            .ToArray();

    private readonly Dictionary<int, string>[] _tokenToName =
        Enumerable.Range(0, OntologySignature.LayerCount)
            .Select(_ => new Dictionary<int, string>())
            .ToArray();

    /// <summary>Registers or resolves one human-readable entry in one ontology layer.</summary>
    public int GetOrAdd(int layer, string entry)
    {
        ValidateLayer(layer);
        ArgumentException.ThrowIfNullOrWhiteSpace(entry);

        if (_nameToToken[layer].TryGetValue(entry, out var existing))
            return existing;

        var token = StableToken(entry);

        if (_tokenToName[layer].TryGetValue(token, out var occupied) &&
            !StringComparer.Ordinal.Equals(occupied, entry))
        {
            throw new InvalidOperationException(
                $"Ontology token collision in layer {layer}: '{entry}' and '{occupied}' both map to {token}.");
        }

        _nameToToken[layer][entry] = token;
        _tokenToName[layer][token] = entry;
        return token;
    }

    public bool TryGetToken(int layer, string entry, out int token)
    {
        ValidateLayer(layer);
        ArgumentException.ThrowIfNullOrWhiteSpace(entry);
        return _nameToToken[layer].TryGetValue(entry, out token);
    }

    public bool TryGetEntry(int layer, int token, out string? entry)
    {
        ValidateLayer(layer);
        return _tokenToName[layer].TryGetValue(token, out entry);
    }

    public IReadOnlyDictionary<string, int> GetEntries(int layer)
    {
        ValidateLayer(layer);
        return _nameToToken[layer];
    }

    /// <summary>
    /// Stable 31-bit FNV-1a token. The string is hashed once at vocabulary
    /// registration; runtime ontology operations never need the string.
    /// </summary>
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
