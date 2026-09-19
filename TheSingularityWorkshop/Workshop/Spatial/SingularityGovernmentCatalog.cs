namespace TheSingularityWorkshop.Gui;

using System;
using System.Collections.Generic;
using System.Linq;

public enum SingularityGovernmentBranch
{
    Executive,
    Legislative,
    Administrative,
    PublicWorks
}

public enum SingularityOfficeType
{
    Mayor,
    Council,
    CityClerk,
    Treasury,
    Planning,
    PublicWorks,
    Housing,
    Transit,
    DigitalServices
}

public sealed record SingularityGovernmentOffice(
    string Id,
    string Title,
    SingularityGovernmentBranch Branch,
    SingularityOfficeType Type,
    string BuildingSpecificationId,
    string Purpose,
    bool Elected);

/// <summary>
/// The initial civic authority model for Singularity City. It describes game
/// institutions and responsibilities; it does not prescribe real-world politics.
/// </summary>
public static class SingularityGovernmentCatalog
{
    public static IReadOnlyList<SingularityGovernmentOffice> Offices =>
    [
        new("government.mayor", "Singularity Mayor", SingularityGovernmentBranch.Executive, SingularityOfficeType.Mayor,
            "civic.government", "City executive; proposes civic priorities and administers the city builder.", true),
        new("government.council", "Singularity City Council", SingularityGovernmentBranch.Legislative, SingularityOfficeType.Council,
            "civic.government", "Represents districts and adopts city rules, budgets, and district changes.", true),
        new("government.clerk", "City Clerk", SingularityGovernmentBranch.Administrative, SingularityOfficeType.CityClerk,
            "civic.government", "Maintains civic records, registrations, elections, and public decisions.", false),
        new("government.treasury", "City Treasury", SingularityGovernmentBranch.Administrative, SingularityOfficeType.Treasury,
            "civic.government", "Maintains the city's digital treasury and public budget ledger.", false),
        new("government.planning", "Planning Commission", SingularityGovernmentBranch.Administrative, SingularityOfficeType.Planning,
            "civic.government", "Maintains district plans and evaluates building specifications against city rules.", false),
        new("government.public-works", "Public Works Authority", SingularityGovernmentBranch.PublicWorks, SingularityOfficeType.PublicWorks,
            "civic.government", "Coordinates roads, utilities, public spaces, and city construction.", false),
        new("government.housing", "Housing Authority", SingularityGovernmentBranch.Administrative, SingularityOfficeType.Housing,
            "civic.government", "Maintains housing policy, residential supply, and digital-real-estate rules.", false),
        new("government.transit", "Transit Authority", SingularityGovernmentBranch.PublicWorks, SingularityOfficeType.Transit,
            "civic.government", "Plans and operates city transportation networks.", false),
        new("government.digital-services", "Digital Services Office", SingularityGovernmentBranch.Administrative, SingularityOfficeType.DigitalServices,
            "civic.government", "Operates civic identity, voting, currency, and city-builder services.", false)
    ];

    public static SingularityGovernmentOffice Resolve(string id)
        => Offices.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase))
           ?? throw new KeyNotFoundException($"No government office is registered for '{id}'.");

    public static IReadOnlyList<SingularityGovernmentOffice> ElectedOffices
        => Offices.Where(x => x.Elected).ToArray();
}

/// <summary>Rules for the fictional city's recurring player election system.</summary>
public sealed record SingularityElectionRules(
    TimeSpan ElectionInterval,
    TimeSpan CampaignWindow,
    int MinimumCandidateAge,
    bool CitizensMayRun,
    bool CitizensMayVote);

public sealed record SingularityElection(
    string Id,
    string OfficeId,
    DateTimeOffset OpensAt,
    DateTimeOffset ClosesAt,
    IReadOnlyList<string> CandidateIds);

public static class SingularityElectionCatalog
{
    public static SingularityElectionRules DefaultRules =>
        new(
            TimeSpan.FromDays(14),
            TimeSpan.FromDays(5),
            18,
            true,
            true);

    public static SingularityElection Create(
        string id,
        string officeId,
        DateTimeOffset opensAt,
        IEnumerable<string> candidateIds)
    {
        var office = SingularityGovernmentCatalog.Resolve(officeId);
        if (!office.Elected)
            throw new InvalidOperationException($"Government office '{officeId}' is not elected.");

        var candidates = candidateIds.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        if (candidates.Length == 0)
            throw new ArgumentException("An election requires at least one candidate.", nameof(candidateIds));

        return new SingularityElection(
            id,
            officeId,
            opensAt,
            opensAt.Add(DefaultRules.ElectionInterval),
            candidates);
    }
}
