using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialCompositionAnd3DTests
{
    [Fact(DisplayName = "Incremental Unit Test 39 — Building recipe composes semantic modules")]
    public void BuildingRecipeComposesModules()
    {
        var recipe = new SpatialBuildingRecipe(
            "example-hq",
            "Example Headquarters",
            new SpatialBounds(0, 0, 40, 30))
            .AddModule(new SpatialBuildingModule("lobby", SpatialBuildingModuleKind.Room, new SpatialBounds(2, 2, 8, 8)))
            .AddModule(new SpatialBuildingModule("stairs", SpatialBuildingModuleKind.Stair, new SpatialBounds(12, 2, 6, 12), Detail: 2))
            .AddModule(new SpatialBuildingModule("arcade", SpatialBuildingModuleKind.Arcade, new SpatialBounds(20, 2, 16, 12), Detail: 2));

        using var linework = SpatialBuildingRecipeGenerator.CreateLinework(recipe);

        Assert.Equal(3, recipe.Modules.Count);
        Assert.True(linework.Lines.Count > 4);
    }

    [Fact(DisplayName = "Incremental Unit Test 40 — 3D line buffer preserves six coordinates")]
    public void ThreeDimensionalLineBufferPreservesCoordinates()
    {
        var lines = new[]
        {
            new SpatialLine3(0, new SpatialPoint3(1, 2, 3), new SpatialPoint3(4, 5, 6))
        };

        var buffer = SpatialLine3TextureBuffer.FromLines(lines);

        Assert.Equal(2, buffer.Width);
        Assert.Equal(1, SpatialLine3TextureBuffer.Height);
        Assert.Equal(1f, buffer.Records[0].Start.R);
        Assert.Equal(2f, buffer.Records[0].Start.G);
        Assert.Equal(3f, buffer.Records[0].Start.B);
        Assert.Equal(4f, buffer.Records[0].End.R);
        Assert.Equal(5f, buffer.Records[0].End.G);
        Assert.Equal(6f, buffer.Records[0].End.B);
    }

    [Fact(DisplayName = "Incremental Unit Test 41 — Orthographic camera projects vertical depth")]
    public void OrthographicCameraProjectsDepthIntoPerceptionSpace()
    {
        var camera = new SpatialOrthographicCamera();
        var origin = camera.Project(new SpatialPoint3(0, 0, 0));
        var depth = camera.Project(new SpatialPoint3(0, 0, 10));

        Assert.NotEqual(origin, depth);
    }

    [Fact(DisplayName = "Incremental Unit Test 42 — 3D projector feeds the existing line perception contract")]
    public void ThreeDimensionalProjectorProducesRenderedLines()
    {
        var style = SpatialLineStyleCatalog.Blueprint;
        var lines = new[]
        {
            new SpatialLine3(0, new SpatialPoint3(0, 0, 0), new SpatialPoint3(10, 5, 8))
        };

        var rendered = new SpatialLine3DProjector().Project(lines, new SpatialOrthographicCamera(), style);

        Assert.Single(rendered);
        Assert.Equal(lines[0], rendered[0].Source);
        Assert.Equal(2, rendered[0].Points.Count);
    }
}
