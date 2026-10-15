namespace TheSingularityWorkshop.Services;

/// <summary>
/// Produces the human-mediated ProtocolAi clipboard exchange without contacting an LLM.
/// Kept independent of component rendering and browser clipboard APIs so the exchange
/// contract can be verified with the existing unit-test project.
/// </summary>
public static class AiExchangeTextBuilder
{
    public static string Build(
        string? protocolDescription,
        string? grammarDescription,
        string? contextPayload)
    {
        var parts = new List<string>
        {
            "WORKSHOP PROTOCOLAI TERMINAL EXCHANGE",
            "The response is a proposal only. Do not invent IDs or claim validation.",
            "PROTOCOL",
            protocolDescription ?? "Protocol vocabulary is not composed.",
            "GRAMMAR",
            grammarDescription ?? "Grammar definition is not composed."
        };

        if (!string.IsNullOrWhiteSpace(contextPayload))
        {
            parts.Add("HOST CONTEXT / TASK");
            parts.Add(contextPayload.Trim());
        }

        parts.Add("RESPONSE CONTRACT");
        parts.Add("Return a structured proposal matching the host task's requested format. Preserve stable IDs where supplied. Explain conflicts instead of silently guessing. The host will validate and review the response before applying changes.");
        return string.Join(Environment.NewLine + Environment.NewLine, parts);
    }
}
