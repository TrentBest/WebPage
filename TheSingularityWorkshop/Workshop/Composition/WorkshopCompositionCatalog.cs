using TheSingularityWorkshop.FSM_COS;
using TheSingularityWorkshop.Workshop.MicroBundles;
using CosMicroBundle = TheSingularityWorkshop.FSM_COS.IMicroBundle;

namespace TheSingularityWorkshop.Workshop.Composition;

public sealed class WorkshopCompositionCatalog : IMicroBundleCatalog
{
    private readonly IReadOnlyDictionary<ulong, CosMicroBundle> _bundles;

    public WorkshopCompositionCatalog()
    {
        var protocol = new ProtocolAiCompositionBundle();
        var grammar = new GrammarAiCompositionBundle();
        var aiExchange = new AiExchangeCompositionBundle();
        var moniker = new MonikerMicroBundle();

        _bundles = new Dictionary<ulong, CosMicroBundle>
        {
            [moniker.CosDescriptor.Id] = moniker,
            [protocol.Id] = protocol,
            [grammar.Id] = grammar,
            [aiExchange.Id] = aiExchange
        };
    }

    public bool TryResolve(ulong bundleId, out CosMicroBundle? bundle) =>
        _bundles.TryGetValue(bundleId, out bundle);
}
