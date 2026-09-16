using System;

namespace TheSingularityWorkshop.Workshop.Agents;

/// <summary>
/// A compact 2D occupancy surface for one explorable city level.
///
/// Each cell answers a deliberately small question: can a Digiten occupy this
/// cell right now, and if so, which persistent actor owns it? The resulting
/// texture-like representation is simultaneously an occupancy map and the
/// substrate from which a crowd vector field can be derived.
///
/// This is a CPU-side simulation representation for now. A renderer/GPU layer
/// may mirror the same data into an actual texture later without changing the
/// world semantics.
/// </summary>
public sealed class DigitenCityTexture
{
    public const int Empty = 0;
    public const int Blocked = -1;

    private readonly int[] _cells;

    public DigitenCityTexture(int width, int height)
    {
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

        Width = width;
        Height = height;
        _cells = new int[width * height];
    }

    public int Width { get; }
    public int Height { get; }

    public bool IsWalkable(int x, int y) => InBounds(x, y) && _cells[Offset(x, y)] != Blocked;

    public bool IsOccupied(int x, int y) => InBounds(x, y) && _cells[Offset(x, y)] > Empty;

    public int GetOccupant(int x, int y)
    {
        ValidateCoordinates(x, y);
        return _cells[Offset(x, y)];
    }

    public void SetBlocked(int x, int y, bool blocked = true)
    {
        ValidateCoordinates(x, y);
        var offset = Offset(x, y);
        if (blocked)
        {
            if (_cells[offset] > Empty)
                throw new InvalidOperationException("A cell occupied by a Digiten cannot be blocked.");
            _cells[offset] = Blocked;
            return;
        }

        if (_cells[offset] == Blocked)
            _cells[offset] = Empty;
    }

    public bool TryOccupy(int x, int y, int digitenId)
    {
        if (digitenId <= Empty) throw new ArgumentOutOfRangeException(nameof(digitenId));
        if (!IsWalkable(x, y)) return false;

        var offset = Offset(x, y);
        if (_cells[offset] != Empty) return false;

        _cells[offset] = digitenId;
        return true;
    }

    public bool TryMove(int digitenId, int fromX, int fromY, int toX, int toY)
    {
        if (digitenId <= Empty) throw new ArgumentOutOfRangeException(nameof(digitenId));
        ValidateCoordinates(fromX, fromY);
        ValidateCoordinates(toX, toY);

        var from = Offset(fromX, fromY);
        var to = Offset(toX, toY);
        if (_cells[from] != digitenId || !IsWalkable(toX, toY) || _cells[to] != Empty)
            return false;

        _cells[from] = Empty;
        _cells[to] = digitenId;
        return true;
    }

    public void Release(int digitenId)
    {
        if (digitenId <= Empty) return;
        for (var i = 0; i < _cells.Length; i++)
        {
            if (_cells[i] == digitenId)
                _cells[i] = Empty;
        }
    }

    private bool InBounds(int x, int y) => (uint)x < (uint)Width && (uint)y < (uint)Height;
    private int Offset(int x, int y) => y * Width + x;

    private void ValidateCoordinates(int x, int y)
    {
        if (!InBounds(x, y))
            throw new ArgumentOutOfRangeException($"({x},{y})");
    }
}

/// <summary>A discrete local direction in the city occupancy surface.</summary>
public readonly record struct DigitenVector(int X, int Y)
{
    public static readonly DigitenVector Zero = new(0, 0);
}

/// <summary>
/// Deterministic vector-field heuristic for a city texture.
/// It deliberately does not solve a global pathfinding problem: it evaluates
/// local alternatives and lets accumulated directional frustration change the
/// preferred direction when a Digiten remains displaced from its desired line.
/// </summary>
public sealed class DigitenCrowdField
{
    private static readonly DigitenVector[] Directions =
    {
        new(0, -1), new(1, -1), new(1, 0), new(1, 1),
        new(0, 1), new(-1, 1), new(-1, 0), new(-1, -1)
    };

    private readonly DigitenCityTexture _city;

    public DigitenCrowdField(DigitenCityTexture city)
    {
        _city = city ?? throw new ArgumentNullException(nameof(city));
    }

    /// <summary>
    /// Chooses a locally useful direction. DesiredDirection is the bird-flight
    /// direction. When frustration reaches the reversal threshold, the actor
    /// first prefers the exact opposite direction if that adjacent cell is
    /// available. Otherwise the normal local alternatives are scored by their
    /// alignment with the desired direction.
    /// </summary>
    public DigitenVector ChooseDirection(
        int x,
        int y,
        DigitenVector desiredDirection,
        double frustration,
        double reversalThreshold = 1.0)
    {
        if (!_city.IsWalkable(x, y))
            return DigitenVector.Zero;

        var desired = Normalize(desiredDirection);
        var forceReversal = frustration >= reversalThreshold;
        var reverse = new DigitenVector(-Math.Sign(desiredDirection.X), -Math.Sign(desiredDirection.Y));

        if (forceReversal && reverse != DigitenVector.Zero && IsAvailable(x, y, reverse))
            return reverse;

        var best = DigitenVector.Zero;
        var bestScore = double.NegativeInfinity;

        foreach (var candidate in Directions)
        {
            if (!IsAvailable(x, y, candidate))
                continue;

            var alignment = candidate.X * desired.X + candidate.Y * desired.Y;
            var score = forceReversal ? -alignment : alignment;
            if (score > bestScore)
            {
                bestScore = score;
                best = candidate;
            }
        }

        return best;
    }

    private bool IsAvailable(int x, int y, DigitenVector direction)
    {
        var nx = x + direction.X;
        var ny = y + direction.Y;
        return _city.IsWalkable(nx, ny) && !_city.IsOccupied(nx, ny);
    }

    private static (double X, double Y) Normalize(DigitenVector direction)
    {
        var length = Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);
        return length == 0 ? (0, 0) : (direction.X / length, direction.Y / length);
    }
}
