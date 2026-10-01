using TheSingularityWorkshop.Gui;
using TheSingularityWorkshop.Workshop.MicroBundles.AI;
using Xunit;

namespace SingularityHub.Tests;

public sealed class WorkshopAiProgramProcessorTests
{
    [Fact]
    public void ProtocolAndGrammar_AutomaticallyExecuteSafeMovement_AndQueueApprovalWork()
    {
        var scene = SpatialWorkshopScene.CreateDefault();
        var snapshot = WorkshopAiTerminalContext.BuildMapSnapshot(SpatialMapScope.Workshop, scene);
        var grammar = WorkshopAiTerminalContext.BuildMapGrammar(snapshot);
        var executed = new List<int>();

        var result = WorkshopAiProgramProcessor.Process(
            $"{ProtocolAi.MoveNorth}\n{ProtocolAi.Open}\n{ProtocolAi.RequestAecReview}",
            snapshot,
            grammar,
            operation =>
            {
                if (operation is ProtocolAi.MoveNorth)
                {
                    executed.Add(operation);
                    return true;
                }

                return false;
            });

        Assert.Single(executed);
        Assert.Equal(ProtocolAi.MoveNorth, executed[0]);
        Assert.Equal(3, result.Entries.Count);
        Assert.Equal(WorkshopAiLedgerStatus.AutoExecuted, result.Entries[0].Status);
        Assert.Equal(WorkshopAiLedgerStatus.AwaitingHumanApproval, result.Entries[1].Status);
        Assert.Equal(WorkshopAiLedgerStatus.AwaitingHumanApproval, result.Entries[2].Status);
        Assert.Contains("AUTO=1", result.Summary);
        Assert.Contains("APPROVAL=2", result.Summary);
    }

    [Fact]
    public void Grammar_Rejects_Operation_Outside_Current_Context()
    {
        var snapshot = new AiContextSnapshot(
            42,
            [1],
            new Dictionary<int, string> { [1] = "TEST" },
            [ProtocolAi.MoveNorth],
            [ProtocolAi.MoveNorth]);
        var grammar = new GrammarAiContract(
            42,
            [new GrammarAiRule(ProtocolAi.MoveSouth, [1], [])]);

        var result = WorkshopAiProgramProcessor.Process(
            ProtocolAi.MoveSouth.ToString(),
            snapshot,
            grammar,
            _ => true);

        Assert.Single(result.Entries);
        Assert.Equal(WorkshopAiLedgerStatus.Rejected, result.Entries[0].Status);
        Assert.Contains("GRAMMAR REJECTED", result.Entries[0].Description);
    }

    [Fact]
    public void Parser_Rejects_NonInteger_Protocol_Data()
    {
        var scene = SpatialWorkshopScene.CreateDefault();
        var snapshot = WorkshopAiTerminalContext.BuildMapSnapshot(SpatialMapScope.Workshop, scene);
        var grammar = WorkshopAiTerminalContext.BuildMapGrammar(snapshot);

        Assert.Throws<FormatException>(() =>
            WorkshopAiProgramProcessor.Process(
                "MOVE NORTH",
                snapshot,
                grammar,
                _ => false));
    }
}
