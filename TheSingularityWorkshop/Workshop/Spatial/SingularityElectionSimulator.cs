namespace TheSingularityWorkshop.Gui;

using System;
using System.Collections.Generic;
using System.Linq;

public sealed record SingularityCandidate(
    string CitizenId,
    string DisplayName,
    string OfficeId);

/// <summary>
/// In-memory election simulation for the city-builder. Persistence and
/// authentication belong to the future civic service; this type defines the
/// neutral game rules that service will enforce.
/// </summary>
public sealed class SingularityElectionSimulator
{
    private readonly HashSet<string> _candidateIds = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _votesByCitizen = new(StringComparer.OrdinalIgnoreCase);

    public SingularityElectionSimulator(SingularityElection election)
        => Election = election ?? throw new ArgumentNullException(nameof(election));

    public SingularityElection Election { get; }

    public IReadOnlyCollection<string> CandidateIds => _candidateIds;
    public int VoteCount => _votesByCitizen.Count;

    public void RegisterCandidate(SingularityCandidate candidate)
    {
        ArgumentNullException.ThrowIfNull(candidate);

        if (!string.Equals(candidate.OfficeId, Election.OfficeId, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException($"Candidate '{candidate.CitizenId}' is registered for a different office.");

        if (string.IsNullOrWhiteSpace(candidate.CitizenId))
            throw new ArgumentException("A candidate requires a citizen ID.", nameof(candidate));

        _candidateIds.Add(candidate.CitizenId);
    }

    public void CastVote(string citizenId, string candidateId)
    {
        if (!_candidateIds.Contains(candidateId))
            throw new InvalidOperationException($"Candidate '{candidateId}' is not registered in this election.");

        if (string.IsNullOrWhiteSpace(citizenId))
            throw new ArgumentException("A vote requires a citizen ID.", nameof(citizenId));

        _votesByCitizen[citizenId] = candidateId;
    }

    public string? DetermineWinner()
        => _candidateIds
            .Select(candidateId => new
            {
                CandidateId = candidateId,
                Votes = _votesByCitizen.Values.Count(vote => string.Equals(vote, candidateId, StringComparison.OrdinalIgnoreCase))
            })
            .OrderByDescending(x => x.Votes)
            .ThenBy(x => x.CandidateId, StringComparer.OrdinalIgnoreCase)
            .Select(x => x.CandidateId)
            .FirstOrDefault();
}
