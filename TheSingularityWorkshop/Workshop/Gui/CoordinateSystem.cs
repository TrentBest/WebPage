using System;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>
/// Defines a normalized Cartesian coordinate space for GUI composition.
/// The origin is the visual center of the panel: (50, 50).
/// Positive X points right and positive Y points down, matching browser layout.
/// </summary>
public readonly record struct CoordinateSystem(
    double Minimum = 0,
    double Maximum = 100,
    double OriginX = 50,
    double OriginY = 50)
{
    public double Range => Maximum - Minimum;

    public bool Contains(double x, double y)
        => x >= Minimum && x <= Maximum && y >= Minimum && y <= Maximum;

    public double ToPanelX(double x)
        => ValidateAndProject(x, OriginX);

    public double ToPanelY(double y)
        => ValidateAndProject(y, OriginY);

    public (double X, double Y) ToPanel(double x, double y)
        => (ToPanelX(x), ToPanelY(y));

    public (double X, double Y) FromPanel(double x, double y)
        => (FromPanelX(x), FromPanelY(y));

    public double FromPanelX(double x)
        => ValidateAndProject(x, OriginX);

    public double FromPanelY(double y)
        => ValidateAndProject(y, OriginY);

    public double DeltaX(double fromX, double toX) => toX - fromX;

    public double DeltaY(double fromY, double toY) => toY - fromY;

    /// <summary>
    /// Gets the canonical normalized GUI coordinate system.
    /// Explicit constructor arguments are required because <c>new()</c> on a
    /// value type uses the CLR zero-initialized representation rather than the
    /// optional parameter defaults declared on the primary constructor.
    /// </summary>
    public static CoordinateSystem Normalized { get; } = new(0, 100, 50, 50);

    private double ValidateAndProject(double value, double origin)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
            throw new ArgumentOutOfRangeException(nameof(value), "Coordinate must be finite.");

        if (value < Minimum || value > Maximum)
            throw new ArgumentOutOfRangeException(nameof(value), value,
                $"Coordinate must be between {Minimum} and {Maximum}.");

        // For the normalized panel space the numerical coordinate is already
        // the panel percentage. Keeping this operation explicit establishes the
        // contract that renderers consume panel coordinates, not viewport coordinates.
        return value;
    }
}
