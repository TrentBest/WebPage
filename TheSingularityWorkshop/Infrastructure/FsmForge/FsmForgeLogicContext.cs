using TheSingularityWorkshop.MicroBundleDomain;
namespace TheSingularityWorkshop.Infrastructure.FsmForge;
public sealed class FsmForgeLogicContext : FsmForgePreviewContext
{
    private readonly Dictionary<ulong, object> _bundles = new();
    public IReadOnlyDictionary<ulong, object> Bundles => _bundles;
    public void AttachBundle(IMicroBundle bundle){ArgumentNullException.ThrowIfNull(bundle);_bundles[bundle.Id]=bundle;}
    public bool TryGetBundle<TBundle>(ulong bundleId,out TBundle? bundle) where TBundle:class
    {
        if(_bundles.TryGetValue(bundleId,out var value)&&value is TBundle typed){bundle=typed;return true;}
        bundle=null;return false;
    }
}
