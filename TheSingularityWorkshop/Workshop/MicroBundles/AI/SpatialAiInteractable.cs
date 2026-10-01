using System;
using System.Collections.Generic;

namespace TheSingularityWorkshop.Workshop.MicroBundles.AI;

/// <summary>
/// Adapts a spatial object to the AI protocol without making the spatial model
/// depend on a particular LLM provider.
/// </summary>
public sealed class SpatialAiInteractable : IAiInteractable
{
    private readonly Func<ProgramAi, ProgramAi> _apply;

    public SpatialAiInteractable(
        string canonicalIdentity,
        IEnumerable<int> operationIds,
        IEnumerable<int> contextIds,
        IEnumerable<int> allowedProgramTokens,
        Func<ProgramAi, ProgramAi> apply)
    {
        Protocol = WorkshopAiProtocolContract.Create(canonicalIdentity, operationIds, contextIds);
        Grammar = new GrammarAiContract(
            Protocol.IdentityId,
            [
                .. Protocol.OperationIds.Select(id => new GrammarAiRule(
                    id,
                    Protocol.ContextIds,
                    allowedProgramTokens is null ? Array.Empty<int>() : [.. allowedProgramTokens]))
            ]);
        _apply = apply ?? throw new ArgumentNullException(nameof(apply));
    }

    public WorkshopAiProtocolContract Protocol { get; }
    public GrammarAiContract Grammar { get; }

    public AiContextSnapshot BuildAiContext()
        => new(
            Protocol.IdentityId,
            Protocol.ContextIds,
            Protocol.HumanDescriptions,
            Protocol.OperationIds,
            Grammar.Rules.SelectMany(rule => rule.AllowedArgumentIds).Distinct().ToArray());

    public ProgramAi ApplyAiProgram(ProgramAi program)
        => _apply(program);
}
