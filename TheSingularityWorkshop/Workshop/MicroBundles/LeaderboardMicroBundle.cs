using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// Reusable leaderboard capability exposed as a MicroBundle.
/// The capability owns leaderboard data and mutation; an Experience decides where
/// and how that capability is discovered and presented.
/// </summary>
public sealed class LeaderboardMicroBundle : IDisposable
{
    public const int BundleId = 2201;

    private readonly MicroBundle _lifecycle;
    private readonly Dictionary<string, LeaderboardEntry> _entries =
        new(StringComparer.OrdinalIgnoreCase);
    private bool _disposed;

    public LeaderboardMicroBundle(
        string name,
        IEnumerable<LeaderboardEntry>? initialEntries = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Leaderboard name is required.", nameof(name));

        _lifecycle = new MicroBundle(BundleId, name, new WebMicroBundleProvider());

        Presentation = new MicroBundlePresentation(
            name,
            "LEADERBOARD",
            "A reusable, live capability for recording and inspecting progress inside an Experience.",
            ["RANKING", "LIVE DATA", "RESULTS", "INSPECTION"]);
        
        if (initialEntries is not null)
            foreach (var entry in initialEntries)
                Set(entry);
    }

    public ulong Id => (ulong)_lifecycle.Id;
    public MicroBundleManifestation? Manifestation => _lifecycle.Manifestation;
    public string Phase => ((MicroBundleContext)_lifecycle.Context).Phase;
    public MicroBundlePresentation Presentation { get; }

    /// <summary>Current entries ordered by steps, then elapsed time.</summary>
    public IReadOnlyList<LeaderboardEntry> Entries
        => new ReadOnlyCollection<LeaderboardEntry>(
            _entries.Values
                .OrderBy(x => x.Steps)
                .ThenBy(x => x.TimeMilliseconds)
                .ThenBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
                .ToList());

    /// <summary>
    /// Records a result. Existing players are replaced only when the new result
    /// improves the recorded step count or, at equal steps, the elapsed time.
    /// </summary>
    public bool Record(string name, long timeMilliseconds, int steps)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Leaderboard player name is required.", nameof(name));
        if (timeMilliseconds < 0)
            throw new ArgumentOutOfRangeException(nameof(timeMilliseconds));
        if (steps < 0)
            throw new ArgumentOutOfRangeException(nameof(steps));

        var candidate = new LeaderboardEntry(name, timeMilliseconds, steps);

        if (_entries.TryGetValue(name, out var existing) &&
            !IsBetter(candidate, existing))
            return false;

        _entries[name] = candidate;
        return true;
    }

    /// <summary>Returns the current entry for a named player, if one exists.</summary>
    public bool TryGet(string name, out LeaderboardEntry entry)
        => _entries.TryGetValue(name, out entry!);

    /// <summary>Advances the capability lifecycle and refreshes its manifestation.</summary>
    public void Update() => _lifecycle.Update();

    public void Dispose()
    {
        if (_disposed) return;
        _lifecycle.Dispose();
        _disposed = true;
    }

    private void Set(LeaderboardEntry entry)
        => _entries[entry.Name] = entry;

    private static bool IsBetter(LeaderboardEntry candidate, LeaderboardEntry existing)
        => candidate.Steps < existing.Steps ||
           candidate.Steps == existing.Steps && candidate.TimeMilliseconds < existing.TimeMilliseconds;
}

/// <summary>Durable semantic result data for one leaderboard participant.</summary>
public sealed record LeaderboardEntry(
    string Name,
    long TimeMilliseconds,
    int Steps)
{
    public string DisplayTime
    {
        get
        {
            var time = TimeSpan.FromMilliseconds(TimeMilliseconds);
            return $"{(int)time.TotalMinutes:00}:{time.Seconds:00}.{time.Milliseconds / 10:00}";
        }
    }
}
