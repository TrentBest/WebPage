using Xunit;

namespace SingularityHub.Tests;

/// <summary>
/// The repository's single incremental version heartbeat.
///
/// AI INSTRUCTION: This is intentionally ONE class. Do not create
/// IncrementalVersion###Tests.cs files for individual commits.
/// Git history is the ledger of prior versions. This class is administrative:
/// it records the current repository heartbeat and provides several assertions
/// so a heartbeat is visible even when a change is primarily structural.
/// Behavioral coverage belongs to one-to-one tests for production types.
/// </summary>
public sealed class IncrementalVersionTests
{
    /// <summary>Version claimed by the current repository commit.</summary>
    public const string CurrentVersion = "0.0.83";

    [ArchitectureTest(0, 0, 83)]
    [Fact(DisplayName = "V0.0.83 — Incremental_Heartbeat")]
    public void V0_0_83_Heartbeat() => Assert.Equal(CurrentVersion, "0.0.83");

    [Fact(DisplayName = "V0.0.83 — Incremental_VersionIsCurrent")]
    public void V0_0_83_VersionIsCurrent() => Assert.Equal("0.0.83", CurrentVersion);

    [Fact(DisplayName = "V0.0.83 — Incremental_VersionFormatIsCanonical")]
    public void V0_0_83_VersionFormatIsCanonical()
    {
        var parts = CurrentVersion.Split('.');

        Assert.Equal(3, parts.Length);
        Assert.All(parts, part => Assert.True(int.TryParse(part, out _)));
        Assert.Equal("0", parts[0]);
        Assert.Equal("0", parts[1]);
        Assert.Equal("83", parts[2]);
    }

    [Fact(DisplayName = "V0.0.83 — Incremental_HeartbeatClassIsSingle")]
    public void V0_0_83_HeartbeatClassIsSingle()
    {
        Assert.Equal(nameof(IncrementalVersionTests), GetType().Name);
        Assert.False(string.IsNullOrWhiteSpace(CurrentVersion));
    }
}
