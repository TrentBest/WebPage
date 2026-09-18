using System.Collections.ObjectModel;

namespace TheSingularityWorkshop.Workshop.Advertising;

/// <summary>
/// A user-created advertising agency inside the Workshop ecosystem.
/// An agency can represent virtual companies, procure available placements,
/// and create new advertising inventory for later rental.
/// </summary>
public sealed class MadmenAgency
{
    private readonly List<AdvertisingClient> _clients = [];
    private readonly List<AdvertisingSpace> _inventory = [];
    private readonly List<AdvertisingCampaign> _campaigns = [];

    public MadmenAgency(int id, string name)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Agency name is required.", nameof(name));

        Id = id;
        Name = name.Trim();
    }

    public int Id { get; }
    public string Name { get; }

    public ReadOnlyCollection<AdvertisingClient> Clients => _clients.AsReadOnly();
    public ReadOnlyCollection<AdvertisingSpace> Inventory => _inventory.AsReadOnly();
    public ReadOnlyCollection<AdvertisingCampaign> Campaigns => _campaigns.AsReadOnly();

    public AdvertisingClient AddClient(int id, string name)
    {
        var client = new AdvertisingClient(id, name);
        if (_clients.Any(x => x.Id == client.Id))
            throw new InvalidOperationException($"Client {client.Id} already exists.");

        _clients.Add(client);
        return client;
    }

    /// <summary>Creates inventory that this agency may later offer to clients.</summary>
    public AdvertisingSpace CreateSpace(
        int id,
        string location,
        AdvertisingFormat format,
        string audience,
        int attentionCost,
        bool isInterruptive = false)
    {
        var space = new AdvertisingSpace(
            id, location, format, audience, attentionCost, isInterruptive);

        if (_inventory.Any(x => x.Id == id))
            throw new InvalidOperationException($"Advertising space {id} already exists.");

        _inventory.Add(space);
        return space;
    }

    /// <summary>
    /// Procures an available placement for a client. The model deliberately
    /// keeps procurement separate from presentation so future markets can plug in.
    /// </summary>
    public AdvertisingCampaign Procure(
        AdvertisingClient client,
        AdvertisingSpace space,
        string message)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(space);
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Campaign message is required.", nameof(message));

        if (!_clients.Contains(client))
            throw new InvalidOperationException("The client does not belong to this agency.");
        if (!_inventory.Contains(space))
            throw new InvalidOperationException("The advertising space is not agency inventory.");
        if (!space.IsAvailable)
            throw new InvalidOperationException("The advertising space is already procured.");

        space.Procure();
        var campaign = new AdvertisingCampaign(
            _campaigns.Count + 1, client, space, message.Trim());
        _campaigns.Add(campaign);
        return campaign;
    }
}

public enum AdvertisingFormat
{
    Sign,
    Kiosk,
    EnvironmentObject,
    LessonSponsor,
    SearchResult,
    SponsoredExperience
}

public sealed record AdvertisingClient(int Id, string Name)
{
    public AdvertisingClient(int id, string name) : this(
        id,
        string.IsNullOrWhiteSpace(name)
            ? throw new ArgumentException("Client name is required.", nameof(name))
            : name.Trim())
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
    }
}

public sealed class AdvertisingSpace
{
    internal AdvertisingSpace(
        int id,
        string location,
        AdvertisingFormat format,
        string audience,
        int attentionCost,
        bool isInterruptive)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
        if (string.IsNullOrWhiteSpace(location)) throw new ArgumentException("Location is required.", nameof(location));
        if (string.IsNullOrWhiteSpace(audience)) throw new ArgumentException("Audience is required.", nameof(audience));
        if (attentionCost < 0) throw new ArgumentOutOfRangeException(nameof(attentionCost));

        Id = id;
        Location = location.Trim();
        Format = format;
        Audience = audience.Trim();
        AttentionCost = attentionCost;
        IsInterruptive = isInterruptive;
    }

    public int Id { get; }
    public string Location { get; }
    public AdvertisingFormat Format { get; }
    public string Audience { get; }
    public int AttentionCost { get; }
    public bool IsInterruptive { get; }
    public bool IsAvailable { get; private set; } = true;

    internal void Procure() => IsAvailable = false;
}

/// <summary>A concrete procurement connecting a client to one advertising placement.</summary>
public sealed record AdvertisingCampaign(
    int Id,
    AdvertisingClient Client,
    AdvertisingSpace Space,
    string Message);
