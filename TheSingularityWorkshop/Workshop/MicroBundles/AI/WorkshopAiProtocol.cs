using System;
using System.Collections.Generic;
using System.Linq;

namespace TheSingularityWorkshop.Workshop.MicroBundles.AI;

/// <summary>
/// Workshop-specific integer operation vocabulary built on top of the generic ProtocolAi
/// package contract. This type is deliberately named for the Workshop domain so it
/// cannot collide with the generic <c>TheSingularityWorkshop.ProtocolAi</c> package.
/// </summary>
public static class WorkshopAiProtocol
{
    public const string ProtocolVersion = "ProtocolAi/1";

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
