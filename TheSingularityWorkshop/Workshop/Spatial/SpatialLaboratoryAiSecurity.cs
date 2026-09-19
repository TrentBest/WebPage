namespace TheSingularityWorkshop.Gui;

using System.Collections.Generic;

/// <summary>
/// Defines the diegetic AI research boundary. Visitor-accessible laboratory floors stop
/// before the restricted AI floors; this is an access model, not merely a UI convention.
/// </summary>
public sealed record SpatialLaboratoryAiSecurity(
    int LowestRestrictedLevel,
    string RestrictedAreaId,
    IReadOnlyList<string> RestrictedResearchDomains)
{
    public static SpatialLaboratoryAiSecurity CreateDefault()
        => new(
            -2,
            "ai-core",
            ["artificial-intelligence", "agent-architecture", "model-training", "grammar", "protocol"]);

    public bool IsRestricted(int level) => level <= LowestRestrictedLevel;
}
