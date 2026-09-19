using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialLineRenderingStressTests
{
    [Fact]
    public void MansionBuildsAConstructionDrawingWithSubstantialDetail()
    {
        using var mansion = SingularityMansionLineworkFactory.Create(1);

        Assert.True(mansion.Lines.Count > 1000);
        Assert.Equal(mansion.Lines.Count, mansion.TextureBuffer.Width);
        Assert.Equal(1, SpatialLineTextureBuffer.Height);
    }

    [Fact]
    public void MansionDensityIncreasesTheRendererWorkloadWithoutChangingFootprint()
    {
        using var baseline = SingularityMansionLineworkFactory.Create(1);
        using var dense = SingularityMansionLineworkFactory.Create(4);

        Assert.True(dense.Lines.Count > baseline.Lines.Count);
        Assert.Equal(baseline.Lines.Count, baseline.TextureBuffer.Width);
        Assert.Equal(dense.Lines.Count, dense.TextureBuffer.Width);
    }

    [Fact]
    public void ThreeDimensionalExperimentUsesTheSameRenderingLifecycle()
    {
        var experiment = SpatialLineRenderingExperiment.CreateDefault();

        Assert.True(experiment.Stats.Source3DLines > 0);
        Assert.Equal(experiment.Stats.Source3DLines, experiment.Stats.ProjectedLines);
        Assert.Equal(experiment.Stats.ProjectedLines, experiment.Stats.RenderedLines);
        Assert.Equal(experiment.Stats.RenderedLines, experiment.Stats.TextureWidth);
        Assert.Equal(experiment.Stats.TextureWidth * 4 * sizeof(float), experiment.Stats.GpuPayloadBytes);
    }
}
