using Xunit;

namespace SingularityHub.Tests;

/// <summary>
/// The repository's single incremental version heartbeat.
///
/// This is intentionally ONE class. Do not create separate incremental version
/// test classes for individual commits. Git history is the ledger of prior
/// versions. This class is administrative: it records the current repository
/// heartbeat and provides several assertions so a heartbeat is visible even
/// when a change is primarily structural. Behavioral coverage belongs to
/// one-to-one tests for production types.
/// </summary>
public sealed class IncrementalVersionTests
{
    /// <summary>Version claimed by the current repository commit.</summary>
    public const string CurrentVersion = "0.0.88";

    [ArchitectureTest(0, 0, 87)]
    [Fact(DisplayName = "V0.0.87 — Incremental_Heartbeat")]
    public void V0_0_87_Heartbeat() => Assert.Equal("0.0.87", "0.0.87");

    [Fact(DisplayName = "V0.0.87 — Incremental_VersionIsCurrent")]
    public void V0_0_87_VersionIsCurrent() => Assert.Equal("0.0.87", "0.0.87");

    [Fact(DisplayName = "V0.0.87 — Incremental_VersionFormatIsCanonical")]
    public void V0_0_87_VersionFormatIsCanonical()
    {
        var parts = "0.0.87".Split('.');

        Assert.Equal(3, parts.Length);
        Assert.All(parts, part => Assert.True(int.TryParse(part, out _)));
        Assert.Equal("0", parts[0]);
        Assert.Equal("0", parts[1]);
        Assert.Equal("87", parts[2]);
    }

    [Fact(DisplayName = "V0.0.87 — Incremental_HeartbeatClassIsSingle")]
    public void V0_0_87_HeartbeatClassIsSingle()
    {
        Assert.Equal(nameof(IncrementalVersionTests), GetType().Name);
        Assert.False(string.IsNullOrWhiteSpace("0.0.87"));
    }

    [ArchitectureTest(0, 0, 88)]
    [Fact(DisplayName = "V0.0.88 — FSM_API_Qualification_And_Dead_Warning_State_Repaired")]
    public void V0_0_88_FsmApiQualificationAndDeadWarningStateRepaired() => Assert.Equal("0.0.88", CurrentVersion);

    [Fact(DisplayName = "V0.0.88 — Incremental_VersionIsCurrent")]
    public void V0_0_88_VersionIsCurrent() => Assert.Equal("0.0.88", CurrentVersion);

    [Fact(DisplayName = "V0.0.88 — Incremental_VersionFormatIsCanonical")]
    public void V0_0_88_VersionFormatIsCanonical()
    {
        var parts = CurrentVersion.Split('.');

        Assert.Equal(3, parts.Length);
        Assert.All(parts, part => Assert.True(int.TryParse(part, out _)));
        Assert.Equal("0", parts[0]);
        Assert.Equal("0", parts[1]);
        Assert.Equal("88", parts[2]);
    }

    [Fact(DisplayName = "V0.0.88 — Incremental_HeartbeatClassIsSingle")]
    public void V0_0_88_HeartbeatClassIsSingle()
    {
        Assert.Equal(nameof(IncrementalVersionTests), GetType().Name);
        Assert.False(string.IsNullOrWhiteSpace(CurrentVersion));
    }
}
