using System;
using System.Collections.Generic;
using System.Linq;

namespace TheSingularityWorkshop.Workshop.MicroBundles.AI;

/// <summary>
/// Workshop-facing contract that binds the generic ProtocolAi package to the
/// Workshop's domain-specific integer vocabulary.
/// </summary>
public sealed record WorkshopAiProtocolContract(
    int IdentityId,
    IReadOnlyList<int> OperationIds,
    IReadOnlyList<int> ContextIds,
    IReadOnlyDictionary<int, string> HumanDescriptions)
{
    public static WorkshopAiProtocolContract Create(
        string canonicalIdentity,
        IEnumerable<int> operations,
        IEnumerable<int> contexts,
        IReadOnlyDictionary<int, string>? descriptions = null)
        => new(
            WorkshopAiProtocol.Identity(canonicalIdentity),
            operations.ToArray(),
            contexts.ToArray(),
            descriptions ?? new Dictionary<int, string>());
}
