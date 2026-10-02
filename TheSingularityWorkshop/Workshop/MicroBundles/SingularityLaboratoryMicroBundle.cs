using System;
using System.Collections.Generic;
using TheSingularityWorkshop.FSM_API;
using TheSingularityWorkshop.SingularityHub;
using TheSingularityWorkshop.Workshop.Laboratory;
using HubBundle = TheSingularityWorkshop.SingularityHub.IMicroBundle;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// Diegetic scientific research facility for the Workshop. Its instruments can
/// later be manifested through the Hub while experiments remain renderer-neutral. The
/// simulation engines are exposed as diegetic instruments so the same state transitions
/// can drive interactive benches and executable verification tests.
/// </summary>
public sealed class SingularityLaboratoryMicroBundle : IMicroBundle, HubBundle, IDisposable
{
    public const int BundleId = 2216;

    private sealed class Provider : IMicroBundleProvider
    {
        public MicroBundleManifestation Manifest(IStateContext context) =>
            new(BundleId, "Singularity Laboratory", ((MicroBundleContext)context).Phase);
    }

    private readonly MicroBundle _runtime;
    private bool _disposed;
    private bool _arbitrated;

    public SingularityLaboratoryMicroBundle()
    {
        _runtime = new MicroBundle(BundleId, "Singularity Laboratory", new Provider());
    }

    public int Id => BundleId;
    public IStateContext Context => _runtime.Context;
    public IReadOnlyList<LaboratoryExperiment> Experiments => SingularityLaboratory.Experiments;
    public IReadOnlyList<LaboratoryRoom> Rooms => LaboratoryRoomCatalog.Rooms;
    public IReadOnlyList<ConfigurableLaboratoryRoom> ConfigurableRooms { get; } =
    [
        ConfigurableLaboratory.CreateEmptyRoom("configurable-lab-01"),
        ConfigurableLaboratory.CreateEmptyRoom("configurable-lab-02"),
        ConfigurableLaboratory.CreateEmptyRoom("configurable-lab-03")
    ];
    public ParticleColliderSimulator ParticleCollider { get; } = new();
    public HydrodynamicSimulator Hydrodynamics { get; } =
        new(new FluidCell(1000, 0, 101_325, 293.15));
    public LandscapeHydrodynamicsSimulator LandscapeHydrodynamics { get; } =
        new(8, 8, new double[64]);
    public ThermodynamicSimulator Thermodynamics { get; } =
        new(new ThermalState(1, 4186, 293.15));
    public ElectromagneticCircuitSimulator Electromagnetism { get; } =
        new(12, 6);
    public IReadOnlyList<LaboratoryResult> LastResults { get; private set; } = Array.Empty<LaboratoryResult>();

    public LaboratoryResult RunExperiment(string experimentId)
    {
        ThrowIfDisposed();
        var result = SingularityLaboratory.Run(experimentId);
        LastResults = [result];
        return result;
    }

    public IReadOnlyList<LaboratoryResult> RunAllExperiments()
    {
        ThrowIfDisposed();
        LastResults = SingularityLaboratory.RunAll();
        return LastResults;
    }

    public bool Arbitrate(IArbitrator arbitrator, int roundIndex)
    {
        ArgumentNullException.ThrowIfNull(arbitrator);
        if (roundIndex < 0 || _arbitrated)
            return false;

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
    OntologySignature HubBundle.Ontology => WorkshopOntology.Laboratory(BundleId);
    BundleVersion HubBundle.Version => new(1, 0, 0);
    IReadOnlyList<ulong> HubBundle.Dependencies => Array.Empty<ulong>();
    bool HubBundle.Arbitrate(IArbitrator arbitrator, int roundIndex) => Arbitrate(arbitrator, roundIndex);

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(SingularityLaboratoryMicroBundle));
    }
}
