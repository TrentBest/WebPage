using System;
using System.Collections.Generic;

namespace TheSingularityWorkshop.Workshop.Agents;

/// <summary>
/// Compact waypoint storage designed to map directly to a GPU-friendly texture.
/// Each waypoint is represented by two unsigned 16-bit normalized coordinates.
/// The texture is storage; locomotion semantics remain in the simulation.
/// </summary>
public sealed class DigitenWaypointTexture
{
    private readonly ushort[] _coordinates;

    public DigitenWaypointTexture(IEnumerable<(double X, double Y)> waypoints)
    {
        ArgumentNullException.ThrowIfNull(waypoints);

        var source = new List<(double X, double Y)>(waypoints);
        if (source.Count == 0)
            throw new ArgumentException("At least one waypoint is required.", nameof(waypoints));

        _coordinates = new ushort[source.Count * 2];
        for (var i = 0; i < source.Count; i++)
        {
            _coordinates[i * 2] = Encode(source[i].X);
            _coordinates[i * 2 + 1] = Encode(source[i].Y);
        }
    }

    /// <summary>Number of waypoint texels represented by this storage.</summary>
    public int Count => _coordinates.Length / 2;

    /// <summary>Reads one normalized waypoint without allocating a new object.</summary>
    public (double X, double Y) Get(int index)
    {
        if ((uint)index >= (uint)Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        return (Decode(_coordinates[index * 2]), Decode(_coordinates[index * 2 + 1]));
    }

    private static ushort Encode(double value) => (ushort)Math.Round(Math.Clamp(value, 0d, 1d) * ushort.MaxValue);
    private static double Decode(ushort value) => value / (double)ushort.MaxValue;
}

/// <summary>
/// Deterministic to-and-fro cursor over compact waypoint storage.
/// A Digiten only needs its current waypoint and travel direction; it does not
/// need a conventional pathfinding graph to move along a prepared route.
/// </summary>
public struct DigitenWaypointCursor
{
    private int _index;
    private int _direction;

    public DigitenWaypointCursor(int count)
    {
        if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count));
        _index = 0;
        _direction = count == 1 ? 0 : 1;
        Count = count;
    }

    public int Count { get; }
    public int Index => _index;
    public int Direction => _direction;

    /// <summary>Returns the next waypoint index and reverses at either endpoint.</summary>
    public int Advance()
    {
        if (Count == 1)
            return _index;

        _index += _direction;
        if (_index == Count - 1 || _index == 0)
            _direction = -_direction;

        return _index;
    }
}
