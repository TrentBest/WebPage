using TheSingularityWorkshop.Workshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Complete public-surface tests for <see cref="CoordinateSystemGuiBuilder"/>.</summary>
public sealed class CoordinateSystemGuiBuilderTests
{
    [Fact]
    public void Create_Uses_Cartesian_Normalized_Defaults()
    {
        var node = CoordinateSystemGuiBuilder.Create("plane").Build();

        Assert.Equal("coordinate-system", node.Kind);
        Assert.Equal("plane", node.Id);
        Assert.Equal("cartesian", node.Properties["coordinate-space"]);
        Assert.Equal("0", node.Properties["minimum"]);
        Assert.Equal("100", node.Properties["maximum"]);
        Assert.Equal("50", node.Properties["origin-x"]);
        Assert.Equal("50", node.Properties["origin-y"]);
    }

    [Fact]
    public void Create_Accepts_Custom_Coordinate_System()
    {
        var coordinates = new CoordinateSystem(-10, 10, 0, 0);
        var node = CoordinateSystemGuiBuilder.Create("plane", coordinates).Build();

        Assert.Equal("-10", node.Properties["minimum"]);
        Assert.Equal("10", node.Properties["maximum"]);
        Assert.Equal("0", node.Properties["origin-x"]);
        Assert.Equal("0", node.Properties["origin-y"]);
    }

    [Fact]
    public void Fluent_Visual_Settings_Are_Stored()
    {
        var node = CoordinateSystemGuiBuilder.Create("plane")
            .Text("Cartesian Plane")
            .Axes()
            .Grid()
            .Origin()
            .Bounds(false)
            .Build();

        Assert.Equal("Cartesian Plane", node.Text);
        Assert.Equal("visible", node.Properties["axes"]);
        Assert.Equal("visible", node.Properties["grid"]);
        Assert.Equal("visible", node.Properties["origin"]);
        Assert.Equal("hidden", node.Properties["bounds"]);
    }

    [Fact]
    public void Point_Projects_Into_The_Coordinate_Plane()
    {
        var node = CoordinateSystemGuiBuilder.Create("plane")
            .Point("origin", 50, 50)
            .Point("upper-right", 90, 10)
            .Build();

        var origin = node.Find("origin");
        var upperRight = node.Find("upper-right");

        Assert.Equal("50", origin.Properties["x"]);
        Assert.Equal("50", origin.Properties["y"]);
        Assert.Equal("90", upperRight.Properties["x"]);
        Assert.Equal("10", upperRight.Properties["y"]);
    }

    [Fact]
    public void Multiple_Points_Remain_Children_Of_The_Plane()
    {
        var node = CoordinateSystemGuiBuilder.Create("plane")
            .Point("a", 10, 90)
            .Point("b", 90, 10)
            .Build();

        Assert.Equal(2, node.Children.Count);
        Assert.Equal("a", node.Children[0].Id);
        Assert.Equal("b", node.Children[1].Id);
    }
}
