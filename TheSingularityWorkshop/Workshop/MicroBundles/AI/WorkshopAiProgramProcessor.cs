using System;
using System.Collections.Generic;
using System.Linq;

namespace TheSingularityWorkshop.Workshop.MicroBundles.AI;

public enum WorkshopAiLedgerStatus
{
    AutoExecuted,
    AwaitingHumanApproval,
    Rejected
}

public sealed record WorkshopAiLedgerEntry(
    int Sequence,
    int OperationId,
    WorkshopAiLedgerStatus Status,
    string Description);

public sealed record WorkshopAiProgramProcessingResult(
    ProgramAi Program,
    IReadOnlyList<WorkshopAiLedgerEntry> Entries,
    string Summary);

public static class WorkshopAiProgramProcessor
{
    public static WorkshopAiProgramProcessingResult Process(
        string transport,
        AiContextSnapshot snapshot,
        GrammarAiContract grammar,
        Func<int, bool> automaticExecutor)
    {
        ArgumentNullException.ThrowIfNull(transport);
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(grammar);
        ArgumentNullException.ThrowIfNull(automaticExecutor);

        var program = ProgramAiParser.Parse(transport);
        var entries = new List<WorkshopAiLedgerEntry>();
        var sequence = 1;

        foreach (var operationId in program.Instructions)
        {
            var rule = grammar.RuleFor(operationId);
            if (rule is null || rule.RequiredContextIds.Any(id => !snapshot.ContextIds.Contains(id)))
            {
                entries.Add(new WorkshopAiLedgerEntry(
                    sequence++,
                    operationId,
                    WorkshopAiLedgerStatus.Rejected,
                    "GRAMMAR REJECTED // operation is not legal in the current context."));
                continue;
            }

            if (!snapshot.AllowedProgramTokens.Contains(operationId))
            {
                entries.Add(new WorkshopAiLedgerEntry(
                    sequence++,
                    operationId,
                    WorkshopAiLedgerStatus.Rejected,
                    "PROTOCOL REJECTED // operation is not exposed by the current context."));
                continue;
            }

            if (automaticExecutor(operationId))
            {
                entries.Add(new WorkshopAiLedgerEntry(
                    sequence++,
                    operationId,
                    WorkshopAiLedgerStatus.AutoExecuted,
                    Describe(operationId)));
                continue;
            }

            entries.Add(new WorkshopAiLedgerEntry(
                sequence++,
                operationId,
                WorkshopAiLedgerStatus.AwaitingHumanApproval,
                $"{Describe(operationId)} // HUMAN APPROVAL REQUIRED"));
        }

        var autoCount = entries.Count(entry => entry.Status == WorkshopAiLedgerStatus.AutoExecuted);
        var approvalCount = entries.Count(entry => entry.Status == WorkshopAiLedgerStatus.AwaitingHumanApproval);
        var rejectedCount = entries.Count(entry => entry.Status == WorkshopAiLedgerStatus.Rejected);

        var summary = $"PROGRAM PROCESSED // AUTO={autoCount} // APPROVAL={approvalCount} // REJECTED={rejectedCount}";
        return new WorkshopAiProgramProcessingResult(program, entries, summary);
    }

    private static string Describe(int operationId) => operationId switch
    {
        WorkshopAiProtocol.Select => "SELECT",
        WorkshopAiProtocol.MoveNorth => "MOVE NORTH",
        WorkshopAiProtocol.MoveSouth => "MOVE SOUTH",
        WorkshopAiProtocol.MoveWest => "MOVE WEST",
        WorkshopAiProtocol.MoveEast => "MOVE EAST",
        WorkshopAiProtocol.Open => "OPEN",
        WorkshopAiProtocol.RequestAecReview => "REQUEST AEC REVIEW",
        WorkshopAiProtocol.CopyContext => "COPY CONTEXT",
        WorkshopAiProtocol.ApplyProgram => "APPLY PROGRAM",
        _ => "UNKNOWN OPERATION"
    };
}
