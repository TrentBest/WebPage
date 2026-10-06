using TheSingularityWorkshop.MicroBundleDomain;

namespace TheSingularityWorkshop.Infrastructure.Atomic;

public static class AtomicBundleIds
{
    public const ulong Core = 0x41544F4D00000001;
    public const ulong Thermal = 0x41544F4D00000002;
    public const ulong MaterialPhysics = 0x41544F4D00000003;
}

public sealed record AtomicElement(int AtomicNumber, string Symbol, string Name, double AtomicMass);

public sealed class AtomicCoreBundle : TheSingularityWorkshop.MicroBundleDomain.IMicroBundle
{
    public MicroBundleDescriptor Descriptor { get; } = new(AtomicBundleIds.Core, "0.1.0");
    public ulong Id => AtomicBundleIds.Core;
    public IReadOnlyList<MicroBundleDependencyRequest> Dependencies => Array.Empty<MicroBundleDependencyRequest>();
    public AtomicElement Element { get; }
    public AtomicCoreBundle(AtomicElement element) => Element = element ?? throw new ArgumentNullException(nameof(element));
    public void Load(IMicroBundleLoadContext context) { }
    public bool Arbitrate(IMicroBundleArbitrationContext context, int roundIndex) => false;
}

public sealed class AtomicThermalBundle : TheSingularityWorkshop.MicroBundleDomain.IMicroBundle
{
    public MicroBundleDescriptor Descriptor { get; } = new(
        AtomicBundleIds.Thermal,
        "0.1.0",
        new[] { new MicroBundleDependency(AtomicBundleIds.Core) });
    public ulong Id => AtomicBundleIds.Thermal;
    public IReadOnlyList<MicroBundleDependencyRequest> Dependencies => new[] { MicroBundleDependencyRequest.Unconfigured(AtomicBundleIds.Core) };
    public double MeltingPointKelvin { get; }
    public double BoilingPointKelvin { get; }
    public AtomicThermalBundle(double meltingPointKelvin, double boilingPointKelvin)
    { MeltingPointKelvin = meltingPointKelvin; BoilingPointKelvin = boilingPointKelvin; }
    public void Load(IMicroBundleLoadContext context) { }
    public bool Arbitrate(IMicroBundleArbitrationContext context, int roundIndex) => false;
}

public sealed class AtomicMaterialPhysicsBundle : TheSingularityWorkshop.MicroBundleDomain.IMicroBundle
{
    public MicroBundleDescriptor Descriptor { get; } = new(
        AtomicBundleIds.MaterialPhysics,
        "0.1.0",
        new[]
        {
            new MicroBundleDependency(AtomicBundleIds.Core),
            new MicroBundleDependency(AtomicBundleIds.Thermal)
        });
    public ulong Id => AtomicBundleIds.MaterialPhysics;
    public IReadOnlyList<MicroBundleDependencyRequest> Dependencies => new[]
    {
        MicroBundleDependencyRequest.Unconfigured(AtomicBundleIds.Core),
        MicroBundleDependencyRequest.Unconfigured(AtomicBundleIds.Thermal)
    };
    public bool SupportsFracture { get; }
    public bool SupportsPhaseChange { get; }
    public AtomicMaterialPhysicsBundle(bool supportsFracture = true, bool supportsPhaseChange = true)
    { SupportsFracture = supportsFracture; SupportsPhaseChange = supportsPhaseChange; }
    public void Load(IMicroBundleLoadContext context) { }
    public bool Arbitrate(IMicroBundleArbitrationContext context, int roundIndex) => false;
}
