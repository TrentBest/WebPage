using System;
using System.Collections.Generic;
using System.Linq;

namespace TheSingularityWorkshop.Workshop.MicroBundles.AI;

/// <summary>
/// Parses the deliberately boring transport returned by an LLM: integer
/// instructions separated by whitespace or commas, with optional explicit
/// CREATE_NAME lines for operations that genuinely require a human-readable name.
/// </summary>
public static class ProgramAiParser
{
    public static ProgramAi Parse(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new ProgramAi(Array.Empty<int>(), Array.Empty<string>());

        var instructions = new List<int>();
        var names = new List<string>();

        foreach (var rawLine in text.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (rawLine.StartsWith("CREATE_NAME=", StringComparison.OrdinalIgnoreCase))
            {
                var name = rawLine["CREATE_NAME=".Length..].Trim();
                if (name.Length > 0)
                    names.Add(name);
                continue;
            }

            foreach (var token in rawLine.Split([',', ' ', '\t'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (!int.TryParse(token, out var instruction))
                    throw new FormatException($"AI program token '{token}' is not an integer protocol value.");

                instructions.Add(instruction);
            }
        }

        return ProgramAi.Parse(instructions, names);
    }
}
