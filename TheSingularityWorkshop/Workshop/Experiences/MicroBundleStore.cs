using TheSingularityWorkshop.SingularityHub;

namespace TheSingularityWorkshop.Workshop.Experiences;

/// <summary>
/// Storage boundary between the Hub runtime and durable MicroBundle providers.
/// A provider may be local, Singularity Warehouse, Azure Blob, or another repository.
/// </summary>
public interface IMicroBundleStore
{
    bool TryGet(ulong id, out IMicroBundle? bundle);
    IReadOnlyList<IMicroBundle> FindByOntologyLayer(int layer, int token);
}

/// <summary>Adapts the in-memory registry to the durable-store boundary.</summary>
public sealed class RegistryMicroBundleStore : IMicroBundleStore
{
    private readonly MicroBundleRegistry _registry;

    public RegistryMicroBundleStore(MicroBundleRegistry registry)
        => _registry = registry ?? throw new ArgumentNullException(nameof(registry));

    public bool TryGet(ulong id, out IMicroBundle? bundle) => _registry.TryGet(id, out bundle);

    public IReadOnlyList<IMicroBundle> FindByOntologyLayer(int layer, int token)
        => _registry.FindByOntologyLayer(layer, token);
}

/// <summary>
/// Provider adapter for the Singularity Warehouse boundary. The Warehouse remains the
/// durable broker; the Hub consumes this narrow capability without referencing Warehouse UI
/// or transport types. This keeps Azure and local providers interchangeable.
/// </summary>
public sealed class WarehouseMicroBundleStoreAdapter : IMicroBundleStore
{
    private readonly Func<ulong, IMicroBundle?> _get;
    private readonly Func<int, int, IReadOnlyList<IMicroBundle>> _query;

    public WarehouseMicroBundleStoreAdapter(
        Func<ulong, IMicroBundle?> get,
        Func<int, int, IReadOnlyList<IMicroBundle>> query)
    {
        _get = get ?? throw new ArgumentNullException(nameof(get));
        _query = query ?? throw new ArgumentNullException(nameof(query));
    }

    public bool TryGet(ulong id, out IMicroBundle? bundle)
    {
        bundle = _get(id);
        return bundle is not null;
    }

    public IReadOnlyList<IMicroBundle> FindByOntologyLayer(int layer, int token)
        => _query(layer, token);
}
