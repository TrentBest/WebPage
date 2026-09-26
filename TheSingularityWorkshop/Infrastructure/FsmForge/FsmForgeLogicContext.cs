using TheSingularityWorkshop.FSM_COS;

namespace TheSingularityWorkshop.Infrastructure.FsmForge;

/// <summary>
/// Runtime context shared by Forge conditions and the composed MicroBundles.
/// The FSM_API-facing identity remains IStateContext; this type carries the
/// optional experience data alongside it.
/// </summary>
public sealed class FsmForgeLogicContext : FsmForgePreviewContext
{
    private readonly Dictionary<ulong, object> _bundles = new();

    public IReadOnlyDictionary<ulong, object> Bundles => _bundles;

    public void AttachBundle(IMicroBundle bundle)
    {
        ArgumentNullException.ThrowIfNull(bundle);
        _bundles[bundle.Id] = bundle;
    }

    public bool TryGetBundle<TBundle>(ulong bundleId, out TBundle? bundle)
        where TBundle : class
    {
        if (_bundles.TryGetValue(bundleId, out var value) && value is TBundle typed)
        {
            bundle = typed;
            return true;
        }

        bundle = null;
        return false;
    }
}
