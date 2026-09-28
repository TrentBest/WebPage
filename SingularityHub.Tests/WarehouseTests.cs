using HubKernel = TheSingularityWorkshop.SingularityHub.SingularityHub;
using TheSingularityWorkshop.SingularityHub;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Layer 5: Data Warehouse liaison. Identity resolution is direct and explicit.</summary>
public sealed class WarehouseTests
{
    [ArchitectureTest(5, 2, 1)]
    [Fact(DisplayName = "5.02.001 — WarehouseLiaison_Implements_Contract")]
    public void WarehouseLiaison_Implements_Contract()
        => Assert.IsAssignableFrom<IDataWarehouseLiaison>(new HubKernel());

    [ArchitectureTest(5, 2, 2)]
    [Fact(DisplayName = "5.02.002 — Unknown_Identity_Is_Not_Resolved")]
    public void Unknown_Identity_Is_Not_Resolved()
        => Assert.False(new HubKernel().TryResolve(5202, out _));

    [ArchitectureTest(5, 2, 3)]
    [Fact(DisplayName = "5.02.003 — Known_Identity_Resolves_To_Exact_Address")]
    public void Known_Identity_Resolves_To_Exact_Address()
    {
        var hub = new HubKernel();
        var expected = new WarehouseAddress(0x1234);
        hub.MapWarehouseIdentity(5203, expected);
        Assert.True(hub.TryResolve(5203, out var actual));
        Assert.Equal(expected, actual);
    }

    [ArchitectureTest(5, 2, 4)]
    [Fact(DisplayName = "5.02.004 — Identity_Mapping_Is_Deterministic")]
    public void Identity_Mapping_Is_Deterministic()
    {
        var hub = new HubKernel();
        var address = new WarehouseAddress(5204);
        hub.MapWarehouseIdentity(5204, address);
        Assert.True(hub.TryResolve(5204, out var first));
        Assert.True(hub.TryResolve(5204, out var second));
        Assert.Equal(first, second);
    }

    [ArchitectureTest(5, 2, 5)]
    [Fact(DisplayName = "5.02.005 — Remapping_Identity_Replaces_Address")]
    public void Remapping_Identity_Replaces_Address()
    {
        var hub = new HubKernel();
        hub.MapWarehouseIdentity(5205, new WarehouseAddress(1));
        hub.MapWarehouseIdentity(5205, new WarehouseAddress(2));
        Assert.True(hub.TryResolve(5205, out var resolved));
        Assert.Equal(new WarehouseAddress(2), resolved);
    }

    [ArchitectureTest(5, 2, 6)]
    [Fact(DisplayName = "5.02.006 — Liaison_Exposes_No_Scan_Operation")]
    public void Liaison_Exposes_No_Scan_Operation()
    {
        var methods = typeof(IDataWarehouseLiaison).GetMethods();
        Assert.Single(methods);
        Assert.Equal(nameof(IDataWarehouseLiaison.TryResolve), methods[0].Name);
    }
}
