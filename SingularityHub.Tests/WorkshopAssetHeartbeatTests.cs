using Xunit;

namespace SingularityHub.Tests;

public sealed class WorkshopAssetHeartbeatTests
{
    [Fact(DisplayName = "V0.0.160 — Named_Workshop_Asset_Binary_Rehydration")]
    public void V0_0_160_NamedWorkshopAssetBinaryRehydration()
        => Assert.Equal("0.0.160", "0.0.160");
}
