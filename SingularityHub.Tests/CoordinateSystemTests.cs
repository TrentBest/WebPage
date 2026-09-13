using System;
using TheSingularityWorkshop.Workshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Complete public-surface tests for <see cref="CoordinateSystem"/>.</summary>
public sealed class CoordinateSystemTests
{
    [Fact]
    public void Normalized_Uses_Zero_To_OneHundred_With_Centered_Origin()
    {
        var coordinates = CoordinateSystem.Normalized;

        Assert.Equal(0, coordinates.Minimum);
        Assert.Equal(100, coordinates.Maximum);
        Assert.Equal(50, coordinates.OriginX);
        Assert.Equal(50, coordinates.OriginY);
        Assert.Equal(100, coordinates.Range);
    }

    [Theory]
    [InlineData(0, 0, true)]
    [InlineData(50, 50, true)]
    [InlineData(100, 100, true)]
    [InlineData(-1, 50, false)]
    [InlineData(50, 101, false)]
    public void Contains_Reports_Normalized_Bounds(double x, double y, bool expected)
        => Assert.Equal(expected, CoordinateSystem.Normalized.Contains(x, y));

    [Theory]
    [InlineData(0, 0)]
    [InlineData(25, 25)]
    [InlineData(50, 50)]
    [InlineData(75, 75)]
    [InlineData(100, 100)]
    public void ToPanel_Preserves_Normalized_Coordinates(double x, double y)
    {
        var result = CoordinateSystem.Normalized.ToPanel(x, y);
        Assert.Equal(x, result.X);
        Assert.Equal(y, result.Y);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(25, 25)]
    [InlineData(50, 50)]
    [InlineData(75, 75)]
    [InlineData(100, 100)]
    public void FromPanel_Preserves_Normalized_Coordinates(double x, double y)
    {
        var result = CoordinateSystem.Normalized.FromPanel(x, y);
        Assert.Equal(x, result.X);
        Assert.Equal(y, result.Y);
    }

    [Fact]
    public void Delta_Uses_Standard_Cartesian_Differences()
    {
        var coordinates = CoordinateSystem.Normalized;

        Assert.Equal(30, coordinates.DeltaX(20, 50));
        Assert.Equal(-30, coordinates.DeltaX(50, 20));
        Assert.Equal(40, coordinates.DeltaY(10, 50));
        Assert.Equal(-40, coordinates.DeltaY(50, 10));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void OutOfRange_Coordinates_Are_Rejected(double value)
        => Assert.Throws<ArgumentOutOfRangeException>(() => CoordinateSystem.Normalized.ToPanelX(value));

    [Fact]
    public void NonFinite_Coordinates_Are_Rejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CoordinateSystem.Normalized.ToPanelX(double.NaN));
        Assert.Throws<ArgumentOutOfRangeException>(() => CoordinateSystem.Normalized.ToPanelY(double.PositiveInfinity));
    }
}
