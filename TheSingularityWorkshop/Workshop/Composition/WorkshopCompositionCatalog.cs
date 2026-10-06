using TheSingularityWorkshop.FSM_COS;
using DomainMicroBundle = TheSingularityWorkshop.MicroBundleDomain.IMicroBundle;
using TheSingularityWorkshop.Workshop.MicroBundles;


namespace TheSingularityWorkshop.Workshop.Composition;

public sealed class WorkshopCompositionCatalog : IMicroBundleCatalog
{
    private readonly IReadOnlyDictionary<ulong, DomainMicroBundle> _bundles;

    public WorkshopCompositionCatalog()
    {
        var protocol = new ProtocolAiCompositionBundle();
        var grammar = new GrammarAiCompositionBundle();
        var aiExchange = new AiExchangeCompositionBundle();
        var moniker = new MonikerMicroBundle();
        var livingGui = new LivingGuiExperienceMicroBundle();

        _bundles = new Dictionary<ulong, DomainMicroBundle>
        {
            [moniker.Descriptor.Id] = moniker,
            [livingGui.Id] = livingGui,
            [protocol.Id] = protocol,
            [grammar.Id] = grammar,
            [aiExchange.Id] = aiExchange
        };
    }

    public bool TryResolve(ulong bundleId, out DomainMicroBundle? bundle) =>
        _bundles.TryGetValue(bundleId, out bundle);
}
