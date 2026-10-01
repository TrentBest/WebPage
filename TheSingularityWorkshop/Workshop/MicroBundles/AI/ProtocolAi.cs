using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TheSingularityWorkshop.Workshop.MicroBundles.AI;

/// <summary>
/// Lowest-level AI protocol vocabulary. Runtime-facing concepts are represented by
/// deterministic integers; human-readable names remain authoring/debugging metadata.
/// </summary>
public static class ProtocolAi
{
    public const int Select = 1001;
    public const int MoveNorth = 1002;
    public const int MoveSouth = 1003;
    public const int MoveWest = 1004;
    public const int MoveEast = 1005;
    public const int Open = 1006;
    public const int RequestAecReview = 1007;
    public const int CopyContext = 1008;
    public const int ApplyProgram = 1009;

    public static int Identity(string canonicalName)
    {
        if (string.IsNullOrWhiteSpace(canonicalName))
            throw new ArgumentException("A canonical protocol name is required.", nameof(canonicalName));

        unchecked
        {
            uint hash = 2166136261;
            foreach (var character in canonicalName.Trim().ToLowerInvariant())
            {
                hash ^= character;
                hash *= 16777619;
            }

            return (int)(hash & 0x7fffffff);
        }
    }

    public static IReadOnlyList<int> Normalize(IEnumerable<int> program)
        => program?.ToArray() ?? throw new ArgumentNullException(nameof(program));
}

/// <summary>Integer-only operation vocabulary exposed by an AI-interactable.</summary>
public sealed record ProtocolAiContract(
    int IdentityId,
    IReadOnlyList<int> OperationIds,
    IReadOnlyList<int> ContextIds,
    IReadOnlyDictionary<int, string> HumanDescriptions)
{
    public static ProtocolAiContract Create(
        string canonicalIdentity,
        IEnumerable<int> operations,
        IEnumerable<int> contexts,
        IReadOnlyDictionary<int, string>? descriptions = null)
        => new(
            ProtocolAi.Identity(canonicalIdentity),
            operations.ToArray(),
            contexts.ToArray(),
            descriptions ?? new Dictionary<int, string>());
}
