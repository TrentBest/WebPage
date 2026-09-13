using System;
using TheSingularityWorkshop.Workshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Complete public-surface tests for <see cref="CoordinateSystemUtilities"/>.</summary>
public sealed class CoordinateSystemUtilitiesTests
{
    [Theory]
    [InlineData(-10, 0, 100, 0)]
    [InlineData(50, 0, 100, 50)]
    [InlineData(150, 0, 100, 100)]
    public void Clamp_Constrains_To_Bounds(double value, double minimum, double maximum, double expected)
        => Assert.Equal(expected, CoordinateSystemUtilities.Clamp(value, minimum, maximum));

    [Fact]
    public void Clamp_Rejects_Reversed_Bounds()
        => Assert.Throws<ArgumentException>(() => CoordinateSystemUtilities.Clamp(50, 100, 0));

    [Fact]
    public void Clamp_Rejects_NonFinite_Value()
        => Assert.Throws<ArgumentOutOfRangeException>(() => CoordinateSystemUtilities.Clamp(double.NaN));

    [Theory]
    [InlineData(0, 0, 100, 0)]
    [InlineData(50, 0, 100, 50)]
    [InlineData(100, 0, 100, 100)]
    [InlineData(25, -50, 50, 75)]
    public void Normalize_Maps_Source_Range_To_Zero_To_OneHundred(double value, double minimum, double maximum, double expected)
        => Assert.Equal(expected, CoordinateSystemUtilities.Normalize(value, minimum, maximum));

    [Theory]
    [InlineData(0, 0, 100, 0)]
    [InlineData(50, 0, 100, 50)]
    [InlineData(100, 0, 100, 100)]
    [InlineData(25, -50, 50, -25)]
    public void Denormalize_Maps_Zero_To_OneHundred_Back_To_Source_Range(double normalized, double minimum, double maximum, double expected)
        => Assert.Equal(expected, CoordinateSystemUtilities.Denormalize(normalized, minimum, maximum));

    [Fact]
    public void Normalize_And_Denormalize_Are_Inverses()
    {
        var value = 37.5;
        var normalized = CoordinateSystemUtilities.Normalize(value, -100, 200);
        var restored = CoordinateSystemUtilities.Denormalize(normalized, -100, 200);

        Assert.Equal(value, restored, 10);
    }

    [Fact]
    public void Range_Validation_Rejects_Zero_Or_Reversed_Range()
    {
        Assert.Throws<ArgumentException>(() => CoordinateSystemUtilities.Normalize(1, 10, 10));
        Assert.Throws<ArgumentException>(() => CoordinateSystemUtilities.Denormalize(50, 10, 0));
    }

    [Fact]
    public void Distance_Uses_Euclidean_Plane_Distance()
        => Assert.Equal(5, CoordinateSystemUtilities.Distance(0, 0, 3, 4));
}
