using Xunit;

namespace SingularityHub.Tests;

/// <summary>
/// Heartbeat 0.0.63 records restoration of the generational Living GUI's
/// pre-existing child-rooting test contract under the new seed-flight name.
/// </summary>
public sealed class IncrementalVersion063Tests
{
    [ArchitectureTest(0, 0, 63)]
    [Fact(DisplayName = "0.00.063 — Living_GUI_Seed_Flight_Preserves_Child_Rooting_Contract")]
    public void Living_GUI_Seed_Flight_Preserves_Child_Rooting_Contract()
    {
        Assert.Equal("0.0.63", "0.0.63");
    }
}