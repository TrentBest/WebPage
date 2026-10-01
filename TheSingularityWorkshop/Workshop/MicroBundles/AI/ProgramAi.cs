using System;
using System.Collections.Generic;
using System.Linq;

namespace TheSingularityWorkshop.Workshop.MicroBundles.AI;

/// <summary>
/// Program-level representation returned by an LLM. The executable portion is an
/// integer sequence; explicit creation names are kept as a separate authoring field.
/// </summary>
public sealed record ProgramAi(
    IReadOnlyList<int> Instructions,
    IReadOnlyList<string> CreationNames)
{
    public static ProgramAi Parse(IEnumerable<int> instructions, IEnumerable<string>? creationNames = null)
        => new(
            WorkshopAiProtocol.Normalize(instructions),
            creationNames?.Where(name => !string.IsNullOrWhiteSpace(name)).ToArray() ?? Array.Empty<string>());
}

/// <summary>
/// The complete AI-facing contract for a semantic interactable.
/// </summary>
public interface IAiInteractable : IGrammarAiInteractable
{
    ProgramAi ApplyAiProgram(ProgramAi program);
}
