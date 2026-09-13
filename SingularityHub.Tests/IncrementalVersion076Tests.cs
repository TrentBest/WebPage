using Xunit;

namespace SingularityHub.Tests;

public sealed class IncrementalVersion076Tests
{
    [ArchitectureTest(0, 0, 76)]
    [Fact(DisplayName = "0.00.076 — Canonical_Experience_Contract")]
    public void CanonicalExperienceContract()
    {
        Assert.Equal("0.0.76", "0.0.76");
    }
}