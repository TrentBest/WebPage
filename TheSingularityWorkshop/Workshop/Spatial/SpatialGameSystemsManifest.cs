namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Declarative catalog of generalized game-system capabilities that can be composed
/// into Experiences without depending on a particular commercial game, trademark,
/// setting, or presentation.
/// </summary>
/// <remarks>
/// The intent is not to recreate any one title. The reusable unit is the rule family:
/// combat, logistics, diplomacy, exploration, progression, economy, and so forth.
/// A future Experience can select the capabilities it needs and let normal
/// MicroBundle arbitration compose them with the rest of its environment.
/// </remarks>
public sealed record SpatialGameSystemsManifest(
    IReadOnlyList<SpatialGameSystem> Systems,
    IReadOnlyList<SpatialGameExperience> Experiences,
    IReadOnlyList<SpatialGameShop> Shops)
{
    /// <summary>Creates the initial generalized game capability catalog.</summary>
    public static SpatialGameSystemsManifest CreateDefault()
        => new(
            [
                new("combat", "COMBAT", "Resolve conflict through units, attributes, actions, range, damage, and outcomes."),
                new("warfare", "WARFARE", "Compose armies, fronts, logistics, objectives, territory, and command."),
                new("tactical-command", "TACTICAL COMMAND", "Control units and formations at encounter scale."),
                new("rts", "REAL-TIME COMMAND", "Manage simultaneous units, resources, production, and objectives."),
                new("strategy-4x", "STRATEGIC EXPANSION", "Explore, expand, exploit, and develop through persistent turns or ticks."),
                new("role-progression", "ROLE PROGRESSION", "Represent characters through attributes, skills, equipment, advancement, and consequence."),
                new("quest", "QUESTS", "Compose goals, prerequisites, actors, rewards, branching outcomes, and provenance."),
                new("diplomacy", "DIPLOMACY", "Represent relationships, agreements, reputation, negotiation, and changing alliances."),
                new("exploration", "EXPLORATION", "Discover places, resources, entities, events, and incomplete knowledge."),
                new("logistics", "LOGISTICS", "Move people, goods, units, energy, and information through constrained networks."),
                new("production", "PRODUCTION", "Transform resources into manufactured outputs through facilities and processes."),
                new("economy", "ECONOMY", "Model ownership, prices, markets, contracts, currency, and resource flows."),
                new("simulation-time", "SIMULATION TIME", "Advance deterministic world state through turns, ticks, schedules, and events.")
            ],
            [
                new("war-room", "WAR ROOM", "Command conflict from a strategic map.", ["warfare", "logistics", "diplomacy", "simulation-time"]),
                new("tactical-arena", "TACTICAL ARENA", "Resolve bounded encounters with composable units.", ["combat", "tactical-command", "role-progression"]),
                new("grand-strategy", "GRAND STRATEGY", "Develop territory and institutions across a persistent world.", ["strategy-4x", "economy", "diplomacy", "production", "simulation-time"]),
                new("adventure-hall", "ADVENTURE HALL", "Build role-driven journeys from reusable quests and encounters.", ["role-progression", "quest", "exploration", "combat"]),
                new("merchant-exchange", "MERCHANT EXCHANGE", "Operate a market and logistics network.", ["economy", "production", "logistics"])
            ],
            [
                new("strategy-shop", "STRATEGY & WAR GAMES", "strategy-4x", "Turn-based and persistent strategic Experiences."),
                new("tactics-shop", "TACTICS & ADVENTURE", "tactical-command", "Encounter-scale tactical and role-driven Experiences."),
                new("simulation-shop", "SIMULATION & MANAGEMENT", "economy", "Management, production, transportation, and civic simulations.")
            ]);

    /// <summary>Finds a reusable system by stable identifier.</summary>
    public SpatialGameSystem? FindSystem(string id)
        => Systems.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase));

    /// <summary>Finds an Experience composition by stable identifier.</summary>
    public SpatialGameExperience? FindExperience(string id)
        => Experiences.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase));

    /// <summary>Returns the systems required by a composed Experience.</summary>
    public IReadOnlyList<SpatialGameSystem> SystemsFor(SpatialGameExperience experience)
        => [.. experience.SystemIds
            .Select(FindSystem)
             .Where(x => x.HasValue)
            .Select(x => x!.Value)];
}

/// <summary>A reusable game-system capability rather than a complete game.</summary>
public readonly record struct SpatialGameSystem(string Id, string Name, string Purpose);

/// <summary>A composed game Experience assembled from reusable systems.</summary>
public sealed record SpatialGameExperience(
    string Id,
    string Name,
    string Purpose,
    IReadOnlyList<string> SystemIds);

/// <summary>A physical/social shop that exposes families of game Experiences.</summary>
public readonly record struct SpatialGameShop(string Id, string Name, string PrimarySystemId, string Purpose);
