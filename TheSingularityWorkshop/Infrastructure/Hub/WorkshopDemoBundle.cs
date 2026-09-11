using TheSingularityWorkshop.SingularityHub;

namespace TheSingularityWorkshop.Infrastructure.Hub;

/// <summary>A deliberately tiny MicroBundle proving the WebPage can host the Hub kernel.</summary>
public sealed class WorkshopDemoBundle : IMicroBundle
{
    private bool _arbitrated;
    public ulong Id => 1;
    public OntologySignature Ontology => new(1, 0, 0, 0, 0, 0, 0, 0, 0);
    public BundleVersion Version => new(0, 1, 0);
    public IReadOnlyList<ulong> Dependencies => Array.Empty<ulong>();

    public bool Arbitrate(IArbitrator arbitrator, int roundIndex)
    {
        if (_arbitrated) return false;
        _arbitrated = true;
        return true;
    }
}
