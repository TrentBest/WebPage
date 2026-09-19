namespace TheSingularityWorkshop.Gui;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Grammar is treated as a compositional runtime structure rather than a bag of strings.
/// Protocols, programming languages, natural languages, and vocalization can share the same
/// structural tooling while retaining domain-specific grammars.
/// </summary>
public sealed record LanguageGrammarManifest(IReadOnlyList<LanguageGrammarDefinition> Grammars)
{
    public static LanguageGrammarManifest CreateDefault()
        => new(
        [
            new("en", "English Grammar", ["subject", "verb", "object", "modifier"]),
            new("de", "German Grammar", ["subject", "verb", "object", "case"]),
            new("ja", "Japanese Grammar", ["topic", "object", "verb", "particle"]),
            new("tlh", "Klingon Grammar", ["object", "verb", "subject", "aspect"]),
            new("csharp", "C# Grammar", ["namespace", "type", "member", "statement", "expression"]),
            new("http", "HTTP Grammar", ["method", "target", "headers", "body"]),
            new("tcp", "TCP/IP Grammar", ["endpoint", "segment", "sequence", "acknowledgement"])
        ]);

    public LanguageGrammarDefinition? Find(string id)
        => Grammars.FirstOrDefault(x => x.Id == id);
}

public readonly record struct LanguageGrammarDefinition(
    string Id,
    string Name,
    IReadOnlyList<string> ProductionRoles);
