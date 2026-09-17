using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialBuildingGeneratorTests
{
    [Fact(DisplayName = "Incremental Unit Test 33 — Building specification becomes renderer-neutral geometry")]
    public void BuildingSpecCreatesGeometry()
    {
        var spec = new SpatialBuildingSpec(
            "arcade",
            "Singularity Arcade",
            new SpatialBounds(10, 20, 30, 24),
            Columns: 5,
            Rows: 4,
            Detail: 2,
            Accent: "#ffe04a",
            Fill: "#17120a");

        var geometry = SpatialBuildingGenerator.CreateGeometry(spec);

        Assert.Equal(spec.Id, geometry.Id);
        Assert.Equal(spec.Bounds, geometry.Bounds);
        Assert.Equal(spec.Accent, geometry.Accent);
        Assert.Equal(spec.Fill, geometry.Fill);
    }

    [Fact(DisplayName = "Incremental Unit Test 34 — Building detail controls line composition without changing footprint")]
    public void BuildingDetailScalesLinework()
    {
        var baseline = new SpatialBuildingSpec("mall", "Ontology Mall", new SpatialBounds(10, 10, 60, 40), Columns: 4, Rows: 3, Detail: 1);
        var detailed = baseline with { Detail = 4 };

        using var low = SpatialBuildingGenerator.CreateLinework(baseline);
        using var high = SpatialBuildingGenerator.CreateLinework(detailed);

        Assert.True(high.Lines.Count > low.Lines.Count);
        Assert.Equal(baseline.Bounds, SpatialBuildingGenerator.CreateGeometry(detailed).Bounds);
        Assert.Equal(high.Lines.Count, high.TextureBuffer.Width);
    }

    [Fact(DisplayName = "Incremental Unit Test 35 — Building generator preserves declared door metadata")]
    public void BuildingGeneratorPreservesOpenings()
    {
        var door = new SpatialOpening(SpatialOpeningKind.Door, SpatialGeometryEdge.Bottom, 8, 14);
        var spec = new SpatialBuildingSpec("transit", "Singularity Transit", new SpatialBounds(40, 40, 20, 12), Openings: [door]);

        var geometry = SpatialBuildingGenerator.CreateGeometry(spec);

        Assert.Single(geometry.Openings);
        Assert.Equal(SpatialOpeningKind.Door, geometry.Openings[0].Kind);
        Assert.Equal(SpatialGeometryEdge.Bottom, geometry.Openings[0].Edge);
        Assert.Equal(6, geometry.Openings[0].Length);
    }
}
