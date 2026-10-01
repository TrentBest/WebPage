using System;
using System.Collections.Generic;
using System.Linq;

namespace TheSingularityWorkshop.Workshop.MicroBundles.AI;

/// <summary>
/// Grammar layer above ProtocolAi. It describes legal composition without carrying
/// executable implementation details into the language model exchange.
/// </summary>
public static class GrammarAi
{
    public const string GrammarVersion = "GrammarAi/1";
}

public sealed record GrammarAiRule(
    int OperationId,
    IReadOnlyList<int> RequiredContextIds,
    IReadOnlyList<int> AllowedArgumentIds);

public sealed record GrammarAiContract(
    int SubjectId,
    IReadOnlyList<GrammarAiRule> Rules)
{
    public GrammarAiRule? RuleFor(int operationId)
        => Rules.FirstOrDefault(rule => rule.OperationId == operationId);
}

/// <summary>
/// Builds the focused grammar for a single AI-interactable in its current context.
/// </summary>
public interface IGrammarAiInteractable
{
    WorkshopAiProtocolContract Protocol { get; }
    GrammarAiContract Grammar { get; }
    AiContextSnapshot BuildAiContext();
}

/// <summary>Focused context copied to an LLM instead of exposing the entire application.</summary>
public sealed record AiContextSnapshot(
    int SubjectId,
    IReadOnlyList<int> ContextIds,
    IReadOnlyDictionary<int, string> HumanContext,
    IReadOnlyList<int> AvailableOperations,
    IReadOnlyList<int> AllowedProgramTokens)
{
    public string ToClipboardText()
    {
        var operations = string.Join(",", AvailableOperations);
        var tokens = string.Join(",", AllowedProgramTokens);
        return $"SUBJECT={SubjectId}\nCONTEXT={string.Join(",", ContextIds)}\nOPERATIONS={operations}\nTOKENS={tokens}";
    }
}
