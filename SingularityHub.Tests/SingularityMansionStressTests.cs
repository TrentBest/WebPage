using System.Linq;
using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SingularityMansionStressTests
{
    [Fact(DisplayName = "Incremental Unit Test 29 — Campus catalog contains the Singularity Mansion")]
    public void CampusCatalogContainsMansion()
    {
        var mansion = Assert.Single(SingularityCampusCatalog.Buildings, item => item.Id == "singularity-mansion");

        Assert.Equal("Singularity Mansion", mansion.Name);
        Assert.True(mansion.Bounds.Width >= 30);
        Assert.True(mansion.Bounds.Height >= 30);
    }

    [Fact(DisplayName = "Incremental Unit Test 30 — Mansion exposes a defined entrance")]
    public void MansionExposesAnEntrance()
    {
        var mansion = Assert.Single(SingularityCampusCatalog.Buildings, item => item.Id == "singularity-mansion");

        var openings = mansion.Openings ?? throw new InvalidOperationException("Mansion openings must be defined.");
        Assert.Contains(openings, opening => opening.Kind == SpatialOpeningKind.Door);
    }

    [Fact(DisplayName = "Incremental Unit Test 31 — Mansion density increases line count")]
    public void MansionDensityScalesLineCount()
    {
        using var baseline = SingularityMansionLineworkFactory.Create(1);
        using var dense = SingularityMansionLineworkFactory.Create(2);

        Assert.True(dense.Lines.Count > baseline.Lines.Count);
        Assert.Equal(baseline.Lines.Count, baseline.TextureBuffer.Width);
        Assert.Equal(dense.Lines.Count, dense.TextureBuffer.Width);
    }

    [Fact(DisplayName = "Incremental Unit Test 32 — Renderer consumes the Mansion line substrate")]
    public void RendererProducesOneRenderedLinePerMansionLine()
    {
        using var mansion = SingularityMansionLineworkFactory.Create(1);
        var rendered = new SpatialLineRenderer().Render(mansion, SpatialLineStyleCatalog.Blueprint);

        Assert.Equal(mansion.Lines.Count, rendered.Count);
        Assert.Equal(mansion.Lines[0], rendered[0].Source);
    }
}
