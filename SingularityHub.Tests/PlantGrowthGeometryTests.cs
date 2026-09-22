using System.Linq;
using TheSingularityWorkshop.Workshop.Plant;
using Xunit;

namespace SingularityHub.Tests;

public sealed class PlantGrowthGeometryTests
{
    [Fact(DisplayName = "Hub growth is authored from the final Hub geometry")]
    public void HubGrowth_UsesFinalGeometryAsSourceOfTruth()
    {
        using var plant = new PlantGrowthMicroBundle();

        Assert.Equal(6, plant.Panels.Count);
        Assert.Equal(
            PlantGrowthMicroBundle.FinalHubGeometry.Select(x => x.Id),
            plant.Panels.Select(x => x.Id));

        foreach (var panel in PlantGrowthMicroBundle.FinalHubGeometry)
        {
            Assert.Contains(plant.Panels, x =>
                x.Id == panel.Id &&
                x.X == panel.X &&
                x.Y == panel.Y &&
                x.Width == panel.Width &&
                x.Height == panel.Height);
        }
    }

    [Fact(DisplayName = "Panels begin their opening sequence at the shared main-panel staging point")]
    public void HubGrowth_PanelsStageAtMainPanelBeforeRelocation()
    {
        using var plant = new PlantGrowthMicroBundle();

        foreach (var panel in plant.Panels)
        {
            Assert.Equal(PlantGrowthMicroBundle.HubGeometry.StagingX, panel.RenderX);
            Assert.Equal(PlantGrowthMicroBundle.HubGeometry.StagingY, panel.RenderY);
            Assert.Equal(0, panel.PositionProgress);
        }
    }

    [Fact(DisplayName = "Panel relocation converges on each panel's final navigation position")]
    public void HubGrowth_PanelsRelocateIntoFinalGeometry()
    {
        using var plant = new PlantGrowthMicroBundle();
        plant.Start();

        for (var i = 0; i < 500 && !plant.IsSettled; i++)
            plant.Update();

        Assert.True(plant.IsSettled);

        foreach (var panel in plant.Panels)
        {
            Assert.Equal(panel.X, panel.RenderX, 6);
            Assert.Equal(panel.Y, panel.RenderY, 6);
            Assert.Equal(1, panel.PositionProgress);
        }
    }

    [Fact(DisplayName = "Every growth branch terminates on its destination panel perimeter")]
    public void HubGrowth_VinesTerminateAtPanelAnchors()
    {
        using var plant = new PlantGrowthMicroBundle();

        Assert.Equal(plant.Panels.Count, plant.Vines.Count);

        foreach (var vine in plant.Vines)
        {
            var panel = PlantGrowthMicroBundle.FinalHubGeometry.Single(x => x.Id == vine.TargetId);

            Assert.Equal(panel.AnchorX, vine.TargetX);
            Assert.Equal(panel.AnchorY, vine.TargetY);
            Assert.Contains(vine.TargetX.ToString("0.##"), vine.Path);
            Assert.Contains(vine.TargetY.ToString("0.##"), vine.Path);
        }
    }
}
