using System;

namespace TheSingularityWorkshop.Workshop.Agents;

/// <summary>
/// Samples a deliberately imperfect destination around a waypoint.
/// The waypoint remains the route's semantic anchor; the sampled point makes
/// repeated trips feel like movement through a living environment rather than
/// a character tracing the same mathematical line forever.
/// </summary>
public static class DigitenWaypointSampler
{
    /// <summary>
    /// Chooses a deterministic candidate inside a radius, shrinking the radius
    /// until the candidate is walkable and unoccupied.
    /// </summary>
    public static bool TrySample(
        DigitenCityTexture city,
        int waypointX,
        int waypointY,
        int radius,
        Random random,
        out (int X, int Y) point)
    {
        ArgumentNullException.ThrowIfNull(city);
        ArgumentNullException.ThrowIfNull(random);

        if (radius < 0)
            throw new ArgumentOutOfRangeException(nameof(radius));

        for (var currentRadius = radius; currentRadius >= 0; currentRadius--)
        {
            var attempts = currentRadius == 0 ? 1 : Math.Max(8, currentRadius * currentRadius * 2);
            for (var attempt = 0; attempt < attempts; attempt++)
            {
                var x = waypointX + random.Next(-currentRadius, currentRadius + 1);
                var y = waypointY + random.Next(-currentRadius, currentRadius + 1);
                if (city.IsWalkable(x, y) && !city.IsOccupied(x, y))
                {
                    point = (x, y);
                    return true;
                }
            }
        }

        point = default;
        return false;
    }
}

/// <summary>
/// Accumulates the discomfort of repeatedly failing to travel in the desired
/// direction. The simulation may eventually decide that the Digiten has had
/// enough and should reverse rather than remain obediently trapped.
/// </summary>
public sealed class DigitenTravelFrustration
{
    public DigitenTravelFrustration(double reversalThreshold = 1.0)
    {
        if (reversalThreshold <= 0)
            throw new ArgumentOutOfRangeException(nameof(reversalThreshold));

        ReversalThreshold = reversalThreshold;
    }

    public double Value { get; private set; }
    public double ReversalThreshold { get; }
    public bool HasHadEnough => Value >= ReversalThreshold;

    /// <summary>
    /// Adds frustration according to how far the chosen direction diverges
    /// from the desired direction. Aligned travel relieves the frustration.
    /// </summary>
    public void Observe(DigitenVector desiredDirection, DigitenVector actualDirection, double amount = 0.25)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        var desired = Normalize(desiredDirection);
        var actual = Normalize(actualDirection);
        var alignment = desired.X * actual.X + desired.Y * actual.Y;

        if (alignment >= 0.99)
            Value = Math.Max(0, Value - amount);
        else
            Value = Math.Clamp(Value + ((1 - alignment) * amount), 0, ReversalThreshold);
    }

    public void Reset() => Value = 0;

    private static (double X, double Y) Normalize(DigitenVector direction)
    {
        var length = Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);
        return length == 0 ? (0, 0) : (direction.X / length, direction.Y / length);
    }
}
