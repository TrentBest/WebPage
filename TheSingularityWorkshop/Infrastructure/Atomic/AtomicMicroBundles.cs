using TheSingularityWorkshop.FSM_COS;

namespace TheSingularityWorkshop.Infrastructure.Atomic;

public static class AtomicBundleIds
{
    public const ulong Core = 0x41544F4D00000001;
    public const ulong Thermal = 0x41544F4D00000002;
    public const ulong MaterialPhysics = 0x41544F4D00000003;
}

public sealed record AtomicElement(int AtomicNumber, string Symbol, string Name, double AtomicMass);

public sealed class AtomicCoreBundle : IMicroBundle
{
    public ulong Id => AtomicBundleIds.Core;
    public IReadOnlyList<BundleRequest> Dependencies => Array.Empty<BundleRequest>();
    public AtomicElement Element { get; }
    public AtomicCoreBundle(AtomicElement element) => Element = element ?? throw new ArgumentNullException(nameof(element));
    public void Load(MicroBundleLoadContext context) { }
    public bool Arbitrate(ArbitrationContext context, int roundIndex) => false;
}

public sealed class AtomicThermalBundle : IMicroBundle
{
    public ulong Id => AtomicBundleIds.Thermal;
    public IReadOnlyList<BundleRequest> Dependencies => new[] { BundleRequest.Unconfigured(AtomicBundleIds.Core) };
    public double MeltingPointKelvin { get; }
    public double BoilingPointKelvin { get; }
    public AtomicThermalBundle(double meltingPointKelvin, double boilingPointKelvin)
    { MeltingPointKelvin = meltingPointKelvin; BoilingPointKelvin = boilingPointKelvin; }
    public void Load(MicroBundleLoadContext context) { }
    public bool Arbitrate(ArbitrationContext context, int roundIndex) => false;
}

public sealed class AtomicMaterialPhysicsBundle : IMicroBundle
{
    public ulong Id => AtomicBundleIds.MaterialPhysics;
    public IReadOnlyList<BundleRequest> Dependencies => new[]
    {
        BundleRequest.Unconfigured(AtomicBundleIds.Core),
        BundleRequest.Unconfigured(AtomicBundleIds.Thermal)
    };
    public bool SupportsFracture { get; }
    public bool SupportsPhaseChange { get; }
    public AtomicMaterialPhysicsBundle(bool supportsFracture = true, bool supportsPhaseChange = true)
    { SupportsFracture = supportsFracture; SupportsPhaseChange = supportsPhaseChange; }
    public void Load(MicroBundleLoadContext context) { }
    public bool Arbitrate(ArbitrationContext context, int roundIndex) => false;
}
