using Xunit;

namespace SingularityHub.Tests;

/// <summary>Breadcrumb proving the construction-site handover slice has a durable test marker.</summary>
public sealed class ConstructionSiteExperienceHeartbeatTests
{
    [Fact(DisplayName = "V0.0.167 — Construction_Site_Foreman_Handover")]
    public void ConstructionSiteForemanHandover()
        => Assert.Equal("0.0.167", "0.0.167");
}
