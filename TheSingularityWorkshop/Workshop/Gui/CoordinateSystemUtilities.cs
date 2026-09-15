using System;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>
/// Stateless helpers for converting and constraining values in a normalized
/// Cartesian GUI coordinate space.
/// </summary>
public static class CoordinateSystemUtilities
{
    public static double Clamp(double value, double minimum = 0, double maximum = 100)
    {
        if (minimum > maximum)
            throw new ArgumentException("Minimum cannot exceed maximum.", nameof(minimum));

        if (double.IsNaN(value) || double.IsInfinity(value))
            throw new ArgumentOutOfRangeException(nameof(value), "Coordinate must be finite.");

        return Math.Clamp(value, minimum, maximum);
    }

    public static double Normalize(double value, double minimum, double maximum)
    {
        if (minimum >= maximum)
            throw new ArgumentException("Minimum must be less than maximum.", nameof(minimum));

        if (double.IsNaN(value) || double.IsInfinity(value))
            throw new ArgumentOutOfRangeException(nameof(value), "Coordinate must be finite.");

        return (value - minimum) / (maximum - minimum) * 100d;
    }

    public static double Denormalize(double normalized, double minimum, double maximum)
    {
        if (minimum >= maximum)
            throw new ArgumentException("Minimum must be less than maximum.", nameof(minimum));

        if (double.IsNaN(normalized) || double.IsInfinity(normalized))
            throw new ArgumentOutOfRangeException(nameof(normalized), "Coordinate must be finite.");

        return minimum + normalized / 100d * (maximum - minimum);
    }

    public static double Distance(double x1, double y1, double x2, double y2)
        => Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
}
