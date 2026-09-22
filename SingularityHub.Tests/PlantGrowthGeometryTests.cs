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
