using TheSingularityWorkshop.FSM_COS;
using DomainMicroBundle = TheSingularityWorkshop.MicroBundleDomain.IMicroBundle;
using TheSingularityWorkshop.Workshop.MicroBundles;
using TheSingularityWorkshop.Workshop.Rendering;

namespace TheSingularityWorkshop.Workshop.Composition;

/// <summary>
/// Creates fresh MicroBundle instances for each composition request.
/// A runtime catalog must not hand a bundle instance already owned by another RuntimeAssembly
/// to a later composition; runtime switching requires isolated bundle lifetimes.
/// </summary>
public sealed class WorkshopCompositionCatalog : IMicroBundleCatalog
{
    private readonly IReadOnlyDictionary<ulong, Func<DomainMicroBundle>> _factories;

    public WorkshopCompositionCatalog()
    {
        var factories = new Dictionary<ulong, Func<DomainMicroBundle>>();
        Register(factories, (ulong)MonikerMicroBundle.BundleId, () => new MonikerMicroBundle());
        Register(factories, (ulong)MonikerSwayBehaviorMicroBundle.BundleId, () => new MonikerSwayBehaviorMicroBundle());
        Register(factories, (ulong)MonikerExperienceMicroBundle.BundleId, () => new MonikerExperienceMicroBundle());
        Register(factories, LivingGuiExperienceMicroBundle.BundleId, () => new LivingGuiExperienceMicroBundle());
        Register(factories, RenderingMicroBundle.BundleId, () => new RenderingMicroBundle());
        Register(factories, ProtocolAiCompositionBundle.BundleId, () => new ProtocolAiCompositionBundle());
        Register(factories, GrammarAiCompositionBundle.BundleId, () => new GrammarAiCompositionBundle());
        Register(factories, AiExchangeCompositionBundle.BundleId, () => new AiExchangeCompositionBundle());
        _factories = factories;
    }

    public bool TryResolve(ulong bundleId, out DomainMicroBundle? bundle)
    {
        if (!_factories.TryGetValue(bundleId, out var factory))
        {
            bundle = null;
            return false;
        }

        bundle = factory();
        if (bundle.Descriptor.Id == bundleId)
            return true;

        if (bundle is IDisposable disposable)
            disposable.Dispose();
        throw new InvalidOperationException(
            $"Catalog factory for MicroBundle {bundleId} created identity {bundle.Descriptor.Id}.");
    }

    public bool TryResolve(ulong bundleId, string version, out DomainMicroBundle? bundle)
    {
        if (!TryResolve(bundleId, out bundle) || bundle is null)
            return false;

        if (string.Equals(bundle.Descriptor.Version, version, StringComparison.Ordinal))
            return true;

        if (bundle is IDisposable disposable)
            disposable.Dispose();

        bundle = null;
        return false;
    }

    private static void Register(
        IDictionary<ulong, Func<DomainMicroBundle>> factories,
        ulong bundleId,
        Func<DomainMicroBundle> factory)
    {
        if (!factories.TryAdd(bundleId, factory))
            throw new InvalidOperationException($"Duplicate MicroBundle ID {bundleId} in the WebPage composition catalog.");
    }
}
