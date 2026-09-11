using System.Reflection;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Layer 0 test infrastructure: X.Y.Z addresses are discoverable metadata.</summary>
public sealed class ArchitectureCoordinateTests
{
    [ArchitectureTest(0, 2, 1)]
    [Fact(DisplayName = "0.02.001 — Coordinate_Formats_As_X_Y_Z")]
    public void Coordinate_Formats_As_X_Y_Z()
    {
        var attribute = new ArchitectureTestAttribute(2, 1, 43);
        Assert.Equal("2.01.043", attribute.Address);
    }

    [ArchitectureTest(0, 2, 2)]
    [Fact(DisplayName = "0.02.002 — Coordinate_Preserves_Layer_Group_Test")]
    public void Coordinate_Preserves_Layer_Group_Test()
    {
        var attribute = new ArchitectureTestAttribute(7, 43, 9);
        Assert.Equal(7, attribute.Layer);
        Assert.Equal(43, attribute.Group);
        Assert.Equal(9, attribute.Test);
    }

    [ArchitectureTest(0, 2, 3)]
    [Fact(DisplayName = "0.02.003 — Coordinate_Can_Be_Read_From_Method")]
    public void Coordinate_Can_Be_Read_From_Method()
    {
        var method = typeof(ArchitectureCoordinateTests).GetMethod(nameof(Coordinate_Can_Be_Read_From_Method));
        Assert.NotNull(method);
        Assert.Equal("0.02.003", ArchitectureTestAttribute.GetAddress(method!));
    }

    [ArchitectureTest(0, 2, 4)]
    [Fact(DisplayName = "0.02.004 — Negative_Layer_Is_Rejected")]
    public void Negative_Layer_Is_Rejected()
        => Assert.Throws<ArgumentOutOfRangeException>(() => new ArchitectureTestAttribute(-1, 1, 1));

    [ArchitectureTest(0, 2, 5)]
    [Fact(DisplayName = "0.02.005 — Missing_Metadata_Is_Detectable")]
    public void Missing_Metadata_Is_Detectable()
    {
        var method = typeof(ArchitectureCoordinateTests).GetMethod(nameof(Coordinate_Without_Metadata));
        Assert.NotNull(method);
        Assert.Throws<InvalidOperationException>(() => ArchitectureTestAttribute.GetAddress(method!));
    }

    private void Coordinate_Without_Metadata() { }
}
