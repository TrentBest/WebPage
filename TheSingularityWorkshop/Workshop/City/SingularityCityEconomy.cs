using System;
using System.Collections.Generic;
using System.Linq;

namespace TheSingularityWorkshop.Workshop.City;

/// <summary>A persistent monetary balance owned by a world actor.</summary>
public sealed class SingularityWallet
{
    public SingularityWallet(long startingBalance = 0)
    {
        if (startingBalance < 0)
            throw new ArgumentOutOfRangeException(nameof(startingBalance));

        Balance = startingBalance;
    }

    public long Balance { get; private set; }

    public void Credit(long amount)
    {
        if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
        Balance = checked(Balance + amount);
    }

    public bool TryDebit(long amount)
    {
        if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
        if (Balance < amount) return false;
        Balance -= amount;
        return true;
    }
}

/// <summary>A persistent residence assignment in Singularity City.</summary>
public sealed record SingularityHome(string Id, int X, int Y, int Capacity);

/// <summary>
/// A business that turns population presence into economic activity. The
/// simulation intentionally models attendance rather than inventing a global
/// economy formula; providers can later define richer pricing and services.
/// </summary>
public sealed class SingularityBusiness
{
    public SingularityBusiness(string id, string name, int x, int y, long attendancePrice)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("A business id is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("A business name is required.", nameof(name));
        if (attendancePrice < 0) throw new ArgumentOutOfRangeException(nameof(attendancePrice));

        Id = id;
        Name = name;
        X = x;
        Y = y;
        AttendancePrice = attendancePrice;
    }

    public string Id { get; }
    public string Name { get; }
    public int X { get; }
    public int Y { get; }
    public long AttendancePrice { get; }
    public int Attendance { get; private set; }
    public long Revenue { get; private set; }

    public bool Attend(SingularityWallet wallet)
    {
        ArgumentNullException.ThrowIfNull(wallet);
        if (!wallet.TryDebit(AttendancePrice)) return false;

        Attendance++;
        Revenue = checked(Revenue + AttendancePrice);
        return true;
    }
}

/// <summary>A persistent city inhabitant with a home, wallet, and current activity.</summary>
public sealed class SingularityCitizen
{
    public SingularityCitizen(int id, string name, SingularityHome home, long startingBalance)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("A citizen name is required.", nameof(name));

        Id = id;
        Name = name;
        Home = home ?? throw new ArgumentNullException(nameof(home));
        Wallet = new SingularityWallet(startingBalance);
    }

    public int Id { get; }
    public string Name { get; }
    public SingularityHome Home { get; }
    public SingularityWallet Wallet { get; }
    public string CurrentActivity { get; private set; } = "Home";

    public bool Visit(SingularityBusiness business)
    {
        ArgumentNullException.ThrowIfNull(business);
        if (!business.Attend(Wallet)) return false;
        CurrentActivity = $"Attending:{business.Id}";
        return true;
    }

    public void ReturnHome() => CurrentActivity = "Home";
}

/// <summary>
/// Small deterministic economic substrate for the persistent city. It is not
/// the final economy: it is the heartbeat on which richer providers can build.
/// </summary>
public sealed class SingularityCityEconomy
{
    private readonly Dictionary<int, SingularityCitizen> _citizens = new();
    private readonly Dictionary<string, SingularityBusiness> _businesses = new(StringComparer.Ordinal);

    public IReadOnlyCollection<SingularityCitizen> Citizens => _citizens.Values;
    public IReadOnlyCollection<SingularityBusiness> Businesses => _businesses.Values;

    public void Register(SingularityCitizen citizen)
    {
        ArgumentNullException.ThrowIfNull(citizen);
        _citizens.Add(citizen.Id, citizen);
    }

    public void Register(SingularityBusiness business)
    {
        ArgumentNullException.ThrowIfNull(business);
        _businesses.Add(business.Id, business);
    }

    public bool TryGetCitizen(int id, out SingularityCitizen? citizen) => _citizens.TryGetValue(id, out citizen);

    public bool TryGetBusiness(string id, out SingularityBusiness? business) => _businesses.TryGetValue(id, out business);

    public long TotalCurrency => _citizens.Sum(c => c.Wallet.Balance) + _businesses.Sum(b => b.Revenue);
}
