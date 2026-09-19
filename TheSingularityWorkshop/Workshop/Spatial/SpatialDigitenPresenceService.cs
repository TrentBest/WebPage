namespace TheSingularityWorkshop.Gui;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Live population boundary for a persistent Singularity world.
/// </summary>
/// <remarks>
/// This service intentionally owns only ephemeral presence. A connected user gets
/// a Digiten avatar in the live population; disconnect removes that avatar. Durable
/// identity, civic office, buildings, economy, and world time belong to persistent
/// services and are not deleted when a visitor leaves.
/// </remarks>
public sealed class SpatialDigitenPresenceService
{
    private readonly Dictionary<string, SpatialDigitenPresence> _connected =
        new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyList<SpatialDigitenPresence> Connected
        => _connected.Values.OrderBy(x => x.DigitenId, StringComparer.OrdinalIgnoreCase).ToArray();

    public SpatialDigitenPresence Connect(
        string userId,
        string displayName,
        SpatialPoint position)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("A user ID is required.", nameof(userId));

        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("A display name is required.", nameof(displayName));

        var presence = new SpatialDigitenPresence(
            $"avatar:{userId}",
            userId,
            displayName,
            position,
            DateTimeOffset.UtcNow);

        _connected[userId] = presence;
        return presence;
    }

    public bool Disconnect(string userId)
        => !string.IsNullOrWhiteSpace(userId) && _connected.Remove(userId);

    public bool TryGet(string userId, out SpatialDigitenPresence presence)
        => _connected.TryGetValue(userId, out presence!);
}

public sealed record SpatialDigitenPresence(
    string DigitenId,
    string UserId,
    string DisplayName,
    SpatialPoint Position,
    DateTimeOffset ConnectedAtUtc);
