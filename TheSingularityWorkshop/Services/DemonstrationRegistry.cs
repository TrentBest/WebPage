using System.Collections.ObjectModel;

namespace TheSingularityWorkshop.Services;

/// <summary>
/// Descriptive metadata for a proof that can be discovered by the WebPage host.
/// This metadata never contains executable behavior; launch behavior remains with its owning host or repository.
/// </summary>
public sealed record DemonstrationDefinition(
    string Id,
    string Title,
    string Summary,
    string WhyItMatters,
    string Status,
    string WhatItProves,
    string WhatItDoesNotProve,
    string LaunchMode,
    string RunInstructions,
    string SourceUrl,
    string Requirements,
    IReadOnlyList<string>? RequiredHostCapabilities = null,
    string? DiagramKind = null,
    string? EmbeddedExperienceUrl = null);

/// <summary>
/// Registry of demonstration descriptions loaded from the host manifest.
/// Entries remain discoverable when they cannot execute inside this host so the UI can explain the limitation.
/// </summary>
public sealed class DemonstrationRegistry
{
    private readonly IReadOnlyList<DemonstrationDefinition> _entries;
    private readonly IReadOnlySet<string> _hostCapabilities;

    public DemonstrationRegistry(
        IEnumerable<DemonstrationDefinition>? entries,
        IEnumerable<string>? hostCapabilities = null)
    {
        var materialized = (entries ?? Array.Empty<DemonstrationDefinition>()).ToArray();
        if (materialized.Any(entry => string.IsNullOrWhiteSpace(entry.Id)))
            throw new ArgumentException("Every demonstration must have a stable ID.", nameof(entries));

        var duplicateId = materialized
            .GroupBy(entry => entry.Id, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault(group => group.Count() > 1);
        if (duplicateId is not null)
            throw new ArgumentException($"Demonstration ID '{duplicateId.Key}' is duplicated.", nameof(entries));

        _entries = new ReadOnlyCollection<DemonstrationDefinition>(materialized);
        _hostCapabilities = new HashSet<string>(
            hostCapabilities ?? Array.Empty<string>(),
            StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>All declared entries, including source-only proofs and entries unavailable in this host.</summary>
    public IReadOnlyList<DemonstrationDefinition> Entries => _entries;

    /// <summary>Looks up an entry without requiring gallery code to enumerate demonstrations by name.</summary>
    public bool TryGet(string id, out DemonstrationDefinition? demonstration)
    {
        demonstration = _entries.FirstOrDefault(entry =>
            string.Equals(entry.Id, id, StringComparison.OrdinalIgnoreCase));
        return demonstration is not null;
    }

    /// <summary>Whether the current host satisfies all explicitly declared host capabilities for an entry.</summary>
    public bool HasRequiredHostCapabilities(DemonstrationDefinition demonstration) =>
        (demonstration.RequiredHostCapabilities ?? Array.Empty<string>())
            .All(_hostCapabilities.Contains);
}

/// <summary>URL conventions for selecting a demonstration inside the existing Home/Hub route.</summary>
public static class DemonstrationRoutes
{
    public const string Gallery = "/?tab=demonstrations";

    public static string Detail(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        return $"{Gallery}&demo={Uri.EscapeDataString(id)}";
    }

    public static string? ReadSelectedId(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return null;

        return query.TrimStart('?')
            .Split('&', StringSplitOptions.RemoveEmptyEntries)
            .Select(part => part.Split('=', 2))
            .Where(parts => parts.Length == 2 &&
                string.Equals(Uri.UnescapeDataString(parts[0].Replace('+', ' ')), "demo", StringComparison.OrdinalIgnoreCase))
            .Select(parts => Uri.UnescapeDataString(parts[1].Replace('+', ' ')))
            .FirstOrDefault();
    }
}
