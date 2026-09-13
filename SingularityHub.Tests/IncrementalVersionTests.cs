using Xunit;

namespace SingularityHub.Tests;

/// <summary>
/// The repository's single incremental version heartbeat.
///
/// AI INSTRUCTION: This is intentionally ONE class and ONE test. Do not create
/// IncrementalVersion###Tests.cs files for individual commits.
/// Git history is the ledger of prior versions. This test records only the
/// current repository heartbeat and does not claim behavioral coverage.
/// Behavioral coverage belongs to one-to-one tests for production types.
/// </summary>
public sealed class IncrementalVersionTests
{
    /// <summary>Version claimed by the current repository commit.</summary>
    public const string CurrentVersion = "0.0.80";

    [ArchitectureTest(0, 0, 80)]
    [Fact(DisplayName = "V0.0.80 — Incremental_Heartbeat")]
    public void V0_0_80() => Assert.Equal(CurrentVersion, "0.0.80");
}
