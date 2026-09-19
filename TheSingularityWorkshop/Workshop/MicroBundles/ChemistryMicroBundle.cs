using System;
using System.Collections.Generic;
using TheSingularityWorkshop.FSM_API;
using TheSingularityWorkshop.Workshop.Chemistry;
using HubBundle = TheSingularityWorkshop.SingularityHub.IMicroBundle;
using TheSingularityWorkshop.SingularityHub;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// Chemistry capability for the Workshop. It owns elemental data and the
/// arbitration service that evaluates material-bearing bundles after install.
/// Presentation is intentionally delegated to a future ChemistryLabGuiBuilder.
/// </summary>
public sealed class ChemistryMicroBundle : IMicroBundle, HubBundle, IDisposable
{
    public const int BundleId = 2215;

    private sealed class Provider : IMicroBundleProvider
    {
        public MicroBundleManifestation Manifest(IStateContext context) =>
            new(BundleId, "Chemistry", ((MicroBundleContext)context).Phase);
    }

    private readonly MicroBundle _runtime;
    private readonly ElementalArbitrator _arbitrator = new();
    private bool _disposed;
    private bool _arbitrated;

    public ChemistryMicroBundle()
    {
        _runtime = new MicroBundle(BundleId, "Chemistry", new Provider());
    }

    public int Id => BundleId;
    public IStateContext Context => _runtime.Context;
    public IReadOnlyDictionary<string, Atom> Elements => ElementalCatalog.Core;
    public IReadOnlyList<MaterialResolution> ResolvedMaterials { get; private set; } = Array.Empty<MaterialResolution>();

    /// <summary>
    /// Evaluates the content exposed by every installed bundle through
    /// <see cref="IElementalMaterialSource"/>. The source's identity is irrelevant
    /// to the chemistry engine; material application and elemental composition drive resolution.
    /// </summary>
    public bool Arbitrate(IArbitrator arbitrator, int roundIndex)
    {
        ArgumentNullException.ThrowIfNull(arbitrator);
        if (roundIndex < 0 || _arbitrated)
            return false;

        ResolvedMaterials = _arbitrator.Evaluate(arbitrator.LoadedBundles);
        _arbitrated = true;
        return true;
    }

    public void Update() => _runtime.Update();

    public void Dispose()
    {
        if (_disposed) return;
        _runtime.Dispose();
        _disposed = true;
    }

    ulong HubBundle.Id => BundleId;
    OntologySignature HubBundle.Ontology => new(0, 0, 0, 0, 0, 0, 0, 0, BundleId);
    BundleVersion HubBundle.Version => new(1, 0, 0);
    IReadOnlyList<ulong> HubBundle.Dependencies => Array.Empty<ulong>();
    bool HubBundle.Arbitrate(IArbitrator arbitrator, int roundIndex) => Arbitrate(arbitrator, roundIndex);
}
