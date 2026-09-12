namespace TheSingularityWorkshop.SingularityHub;

/// <summary>
/// Runtime catalog for globally addressable MicroBundles.
/// A slot is the pair (nine-layer ontology, VariantId). A slot may have one
/// occupant; a different variant may coexist under the same ontology.
/// </summary>
public sealed class MicroBundleRegistry
{
    private readonly Dictionary<MicroBundleAddress, IMicroBundle> _bundles = new();

    public IReadOnlyCollection<IMicroBundle> Bundles => _bundles.Values;

    public bool TryPublish(MicroBundleAddress address, IMicroBundle bundle)
    {
        ArgumentNullException.ThrowIfNull(bundle);

        if (!address.IsValid)
            return false;

        return _bundles.TryAdd(address, bundle);
    }

    public bool TryGet(MicroBundleAddress address, out IMicroBundle? bundle)
        => _bundles.TryGetValue(address, out bundle);

    public bool Contains(MicroBundleAddress address) => _bundles.ContainsKey(address);

    public bool TryWithdraw(MicroBundleAddress address, out IMicroBundle? bundle)
        => _bundles.Remove(address, out bundle);
}
