using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialTransitLaboratoryHeartbeatTests
{
    [ArchitectureTest(0, 0, 153)]
    [Fact(DisplayName = "V0.0.153 — Spatial_Transit_And_Laboratory_Manifests")]
    public void V0_0_153_SpatialTransitAndLaboratoryManifests()
        => Assert.Equal("0.0.153", "0.0.153");
}
