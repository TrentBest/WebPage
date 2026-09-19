using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialLaboratory3DManifestTests
{
    [Fact]
    public void ThreeDimensionalLab_PreservesRendererEscalationBoundary()
    {
        var lab = SpatialLaboratory3DManifest.CreateDefault();

        Assert.Equal(SpatialLaboratoryRenderPath.SpatialLineRendererThenGpu, lab.RenderPath);
        Assert.Contains(lab.Experiments, x => x.Id == "tower" && x.Dimensions == 3);
        Assert.Contains(lab.Experiments, x => x.Id == "gravity-bodies");
        Assert.Contains(lab.Experiments, x => x.Id == "launcher-track" && x.Dimensions == 3);
    }
}