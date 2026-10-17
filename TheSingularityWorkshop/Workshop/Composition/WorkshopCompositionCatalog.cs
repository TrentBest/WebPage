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
        Register(factories, () => new MonikerMicroBundle());
        Register(factories, () => new MonikerSwayBehaviorMicroBundle());
        Register(factories, () => new MonikerExperienceMicroBundle());
        Register(factories, () => new LivingGuiExperienceMicroBundle());
        Register(factories, () => new RenderingMicroBundle());
        Register(factories, () => new ProtocolAiCompositionBundle());
        Register(factories, () => new GrammarAiCompositionBundle());
        Register(factories, () => new AiExchangeCompositionBundle());
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
        return true;
    }

    public bool TryResolve(ulong bundleId, string version, out DomainMicroBundle? bundle)
    {
        if (!TryResolve(bundleId, out bundle) || bundle is null)
            return false;

        if (bundle.Descriptor is not null &&
            string.Equals(bundle.Descriptor.Version, version, StringComparison.Ordinal))
            return true;

        if (bundle is IDisposable disposable)
            disposable.Dispose();

        bundle = null;
        return false;
    }

    private static void Register(
        IDictionary<ulong, Func<DomainMicroBundle>> factories,
        Func<DomainMicroBundle> factory)
    {
        // Inspect a prototype only to discover the canonical domain identity. Dispose it
        // immediately so the catalog itself does not retain a runtime-owned instance.
        var prototype = factory();
        var id = prototype.Descriptor.Id;
        if (!factories.TryAdd(id, factory))
        {
            if (prototype is IDisposable disposable)
                disposable.Dispose();
            throw new InvalidOperationException($"Duplicate MicroBundle ID {id} in the WebPage composition catalog.");
        }

        if (prototype is IDisposable owned)
            owned.Dispose();
    }
}
