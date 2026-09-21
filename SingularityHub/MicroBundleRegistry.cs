namespace TheSingularityWorkshop.SingularityHub;

/// <summary>
/// Runtime catalog for globally addressable MicroBundles.
/// A slot is the pair (nine-layer ontology, VariantId). A slot may have one
/// occupant; a different variant may coexist under the same ontology.
/// </summary>
public sealed class MicroBundleRegistry
{
    private readonly Dictionary<MicroBundleAddress, IMicroBundle> _bundles = new();

    /// <summary>Gets all bundles currently published in the registry.</summary>
    public IReadOnlyCollection<IMicroBundle> Bundles => _bundles.Values;

    /// <summary>Publishes a bundle into an unused address slot.</summary>
    public bool TryPublish(MicroBundleAddress address, IMicroBundle bundle)
    {
        ArgumentNullException.ThrowIfNull(bundle);

        if (!address.IsValid)
            return false;

        return _bundles.TryAdd(address, bundle);
    }

    /// <summary>Attempts to retrieve a bundle by address.</summary>
    public bool TryGet(MicroBundleAddress address, out IMicroBundle? bundle)
        => _bundles.TryGetValue(address, out bundle);

    /// <summary>Gets whether an address is currently occupied.</summary>
    public bool Contains(MicroBundleAddress address) => _bundles.ContainsKey(address);

    /// <summary>Removes a published bundle and returns it when present.</summary>
    public bool TryWithdraw(MicroBundleAddress address, out IMicroBundle? bundle)
        => _bundles.Remove(address, out bundle);
}
