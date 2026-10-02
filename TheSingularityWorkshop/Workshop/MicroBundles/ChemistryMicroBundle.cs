using System;
using System.Collections.Generic;
using System.Linq;
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
    private readonly FictionalElementRegistry _fictionalElements = new();
    private bool _disposed;
    private bool _arbitrated;

    public ChemistryMicroBundle()
    {
        _runtime = new MicroBundle(BundleId, "Chemistry", new Provider());
        _fictionalElements.RegisterRange(ElementalCatalog.Fictional.Values);
    }

    public int Id => BundleId;
    public IStateContext Context => _runtime.Context;

    /// <summary>All canonical elements owned by the domain catalog.</summary>
    public IReadOnlyDictionary<string, Atom> Elements => ElementalCatalog.Core;

    /// <summary>Fictional/experience-specific elements available to this bundle.</summary>
    public IReadOnlyCollection<Atom> FictionalElements => _fictionalElements.Elements;

    /// <summary>
    /// Registers an additional fictional element without modifying the canonical catalog.
    /// </summary>
    public bool RegisterFictionalElement(Atom element) => _fictionalElements.Register(element);

    /// <summary>
    /// Resolves either a canonical or registered fictional element by symbol.
    /// </summary>
    public bool TryGetElement(string symbol, out Atom? element)
    {
        if (ElementalCatalog.Core.TryGetValue(symbol, out element))
            return true;

        return _fictionalElements.TryGetBySymbol(symbol, out element);
    }

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
