using System;
using System.Threading;

namespace TheSingularityWorkshop.Workshop.Randomness;

/// <summary>
/// Fast, deterministic Squirrel Noise 5 based random stream.
/// </summary>
/// <remarks>
/// This implementation is based on Squirrel Noise 5 by Squirrel Eiserloh.
/// The original Squirrel Noise 5 source is released under CC-BY 3.0; attribution is retained here.
/// Squirrel Noise is a counter-based noise function: each position can be evaluated
/// independently, which makes it useful for procedural generation and concurrent work.
/// This wrapper adds an atomic position counter so callers can also consume it as a
/// sequential random stream without sharing <see cref="System.Random"/> state.
/// It is not a cryptographic random source.
/// </remarks>
public sealed class SquirrelRng
{
    private const uint BitNoise1 = 0x68E31DA4u;
    private const uint BitNoise2 = 0xB5297A4Du;
    private const uint BitNoise3 = 0x1B56C4E9u;

    private int _position = -1;

    /// <summary>Initializes a deterministic stream with the supplied seed.</summary>
    public SquirrelRng(uint seed = 0)
    {
        Seed = seed;
    }

    /// <summary>Gets the seed used by this stream.</summary>
    public uint Seed { get; }

    /// <summary>Gets the most recently consumed position.</summary>
    public int Position => Volatile.Read(ref _position);

    /// <summary>Evaluates Squirrel Noise 5 at an arbitrary position without changing stream state.</summary>
    public static uint Noise(int position, uint seed = 0)
    {
        unchecked
        {
            var mangledBits = (uint)position;
            mangledBits *= BitNoise1;
            mangledBits += seed;
            mangledBits ^= mangledBits >> 8;
            mangledBits += BitNoise2;
            mangledBits ^= mangledBits << 8;
            mangledBits *= BitNoise3;
            mangledBits ^= mangledBits >> 8;
            return mangledBits;
        }
    }

    /// <summary>Consumes and returns the next 32-bit value.</summary>
    public uint NextUInt()
        => Noise(Interlocked.Increment(ref _position), Seed);

    /// <summary>Returns a value in the half-open integer range [0, exclusiveMax).</summary>
    public int Next(int exclusiveMax)
    {
        if (exclusiveMax <= 0)
            throw new ArgumentOutOfRangeException(nameof(exclusiveMax));

        return (int)(NextUInt() % (uint)exclusiveMax);
    }

    /// <summary>Returns a value in the half-open integer range [minimum, maximum).</summary>
    public int Next(int minimum, int maximum)
    {
        if (maximum <= minimum)
            throw new ArgumentOutOfRangeException(nameof(maximum));

        return minimum + Next(maximum - minimum);
    }

    /// <summary>Returns a deterministic value in the half-open floating range [0, 1).</summary>
    public double NextDouble()
        => NextUInt() / 4294967296d;

    /// <summary>Returns a deterministic floating-point value in the half-open range [minimum, maximum).</summary>
    public double NextDouble(double minimum, double maximum)
    {
        if (maximum < minimum)
            throw new ArgumentOutOfRangeException(nameof(maximum));

        return minimum + NextDouble() * (maximum - minimum);
    }

    /// <summary>Returns true or false from the next stream value.</summary>
    public bool NextBool()
        => (NextUInt() & 1u) == 0u;
}
