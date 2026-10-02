namespace TheSingularityWorkshop.SingularityHub;

/// <summary>
/// Runtime catalog for globally addressable MicroBundles.
/// A slot is the pair (nine-layer ontology, VariantId). A slot may have one
/// occupant; a different variant may coexist under the same ontology.
///
/// The registry maintains two complementary indexes:
/// 1. exact address -> bundle for identity/uniqueness;
/// 2. nine layer/value indexes -> bundles for semantic ontology discovery.
///
/// Publishing a bundle touches exactly nine ontology buckets, so the ontology
/// indexing cost is bounded by the fixed ontology depth rather than the number
/// of registered bundles.
/// </summary>
public sealed class MicroBundleRegistry
{
    private readonly Dictionary<MicroBundleAddress, IMicroBundle> _bundles = new();
    private readonly Dictionary<int, HashSet<MicroBundleAddress>>[] _layerIndexes =
        Enumerable.Range(0, OntologySignature.LayerCount)
            .Select(_ => new Dictionary<int, HashSet<MicroBundleAddress>>())
            .ToArray();

    public IReadOnlyCollection<IMicroBundle> Bundles => _bundles.Values;

    public bool TryPublish(MicroBundleAddress address, IMicroBundle bundle)
    {
        ArgumentNullException.ThrowIfNull(bundle);

        if (!address.IsValid || bundle.Ontology != address.Ontology)
            return false;

        if (!_bundles.TryAdd(address, bundle))
            return false;

        for (var layer = 0; layer < OntologySignature.LayerCount; layer++)
        {
            var value = address.Ontology[layer];

            if (!_layerIndexes[layer].TryGetValue(value, out var addresses))
            {
                addresses = [];
                _layerIndexes[layer][value] = addresses;
            }

            addresses.Add(address);
        }

        return true;
    }

    public bool TryGet(MicroBundleAddress address, out IMicroBundle? bundle)
        => _bundles.TryGetValue(address, out bundle);

    /// <summary>
    /// Returns every bundle whose ontology contains the requested value at the
    /// requested layer. Lookup is one dictionary access plus the matching bucket.
    /// </summary>
    public IReadOnlyCollection<IMicroBundle> GetByLayer(int layer, int value)
    {
        ValidateLayer(layer);

        if (!_layerIndexes[layer].TryGetValue(value, out var addresses))
            return Array.Empty<IMicroBundle>();

        return addresses
            .Select(address => _bundles[address])
            .ToArray();
    }

    /// <summary>
    /// Returns the address index for callers that need compact integer routing
    /// without materializing the bundle objects.
    /// </summary>
    public IReadOnlyCollection<MicroBundleAddress> GetAddressesByLayer(int layer, int value)
    {
        ValidateLayer(layer);

        return _layerIndexes[layer].TryGetValue(value, out var addresses)
            ? addresses
            : Array.Empty<MicroBundleAddress>();
    }

    public bool Contains(MicroBundleAddress address) => _bundles.ContainsKey(address);

    public bool TryWithdraw(MicroBundleAddress address, out IMicroBundle? bundle)
    {
        if (!_bundles.Remove(address, out bundle))
            return false;

        for (var layer = 0; layer < OntologySignature.LayerCount; layer++)
        {
            var value = address.Ontology[layer];

            if (!_layerIndexes[layer].TryGetValue(value, out var addresses))
                continue;

            addresses.Remove(address);

            if (addresses.Count == 0)
                _layerIndexes[layer].Remove(value);
        }

        return true;
    }

    private static void ValidateLayer(int layer)
    {
        if ((uint)layer >= OntologySignature.LayerCount)
            throw new ArgumentOutOfRangeException(nameof(layer));
    }
}
