using TheSingularityWorkshop.SingularityHub;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Layer 8: execution boundary. The Hub describes eligible work; providers execute it.</summary>
public sealed class ExecutionDomainTests
{
    [ArchitectureTest(8, 1, 1)]
    [Fact(DisplayName = "8.01.001 — ExecutionProvider_Implements_Contract")]
    public void ExecutionProvider_Implements_Contract()
        => Assert.IsAssignableFrom<IExecutionProvider>(new TestExecutionProvider());

    [ArchitectureTest(8, 1, 2)]
    [Fact(DisplayName = "8.01.002 — WorkDescriptor_Is_Explicit")]
    public void WorkDescriptor_Is_Explicit()
    {
        var work = new WorkDescriptor(8002, 42, 3);
        Assert.Equal((ulong)8002, work.ProcessGroupId);
        Assert.Equal((ulong)42, work.WorkId);
        Assert.Equal(3, work.PathIndex);
    }

    [ArchitectureTest(8, 1, 3)]
    [Fact(DisplayName = "8.01.003 — Provider_Returns_CompletionToken")]
    public void Provider_Returns_CompletionToken()
    {
        var provider = new TestExecutionProvider();
        var result = provider.Execute(new WorkDescriptor(8003, 1, 0));
        Assert.True(result.Completed);
        Assert.Equal((ulong)1, result.Value);
    }

    [ArchitectureTest(8, 1, 4)]
    [Fact(DisplayName = "8.01.004 — Execution_Does_Not_Mutate_WorkDescriptor")]
    public void Execution_Does_Not_Mutate_WorkDescriptor()
    {
        var provider = new TestExecutionProvider();
        var work = new WorkDescriptor(8004, 7, 2);
        _ = provider.Execute(work);
        Assert.Equal(new WorkDescriptor(8004, 7, 2), work);
    }

    [ArchitectureTest(8, 1, 5)]
    [Fact(DisplayName = "8.01.005 — ExecutionProvider_Can_Report_Incomplete")]
    public void ExecutionProvider_Can_Report_Incomplete()
    {
        var provider = new TestExecutionProvider { Complete = false };
        var result = provider.Execute(new WorkDescriptor(8005, 8, 0));
        Assert.False(result.Completed);
    }

    private sealed class TestExecutionProvider : IExecutionProvider
    {
        public bool Complete { get; init; } = true;
        public CompletionToken Execute(WorkDescriptor work) => new(work.WorkId, Complete);
    }
}
