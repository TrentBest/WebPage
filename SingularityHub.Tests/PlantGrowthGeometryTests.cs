using System.Linq;
using TheSingularityWorkshop.Workshop.Plant;
using Xunit;

namespace SingularityHub.Tests;

public sealed class PlantGrowthGeometryTests
{
    [Fact(DisplayName = "Hub growth is authored from the final invisible navigation mesh")]
    public void HubGrowth_UsesFinalMeshAsSourceOfTruth()
    {
        using var plant = new PlantGrowthMicroBundle();

        Assert.Equal(6, plant.Panels.Count);
        Assert.Equal(plant.Panels.Count, PlantGrowthMicroBundle.FinalHubGeometry.Count);

        foreach (var panel in PlantGrowthMicroBundle.FinalHubGeometry)
        {
            Assert.True(PlantGrowthMicroBundle.HubGeometry.MeshBounds.Contains(
                new PlantGrowthMicroBundle.SpatialPoint(panel.X, panel.Y)));

            Assert.Contains(plant.Panels, x =>
                x.Id == panel.Id &&
                x.X == panel.X &&
                x.Y == panel.Y &&
                x.Width == panel.Width &&
                x.Height == panel.Height);
        }
    }

    [Fact(DisplayName = "Every vine starts from the common upper-left mesh root")]
    public void HubGrowth_VinesShareCommonRoot()
    {
        using var plant = new PlantGrowthMicroBundle();
        var root = PlantGrowthMicroBundle.HubGeometry.Root;

        foreach (var vine in plant.Vines)
        {
            Assert.StartsWith($"M {root.X:0.##} {root.Y:0.##}", vine.Path);
            Assert.Equal(0, vine.Progress);
            Assert.Equal(0, vine.BloomProgress);
        }
    }

    [Fact(DisplayName = "Vine FSMs use staggered phase delays")]
    public void HubGrowth_VinesDoNotBloomAsOneSynchronousBatch()
    {
        using var plant = new PlantGrowthMicroBundle();
        var delays = plant.Vines.Select(v => v.PhaseDelayTicks).ToArray();

        Assert.Equal(delays.Length, delays.Distinct().Count());
        Assert.True(delays.SequenceEqual(delays.OrderBy(x => x)));
    }

    [Fact(DisplayName = "Vine FSM clocks produce staggered bloom progress")]
    public void HubGrowth_BloomsArriveAtDifferentTimes()
    {
        using var plant = new PlantGrowthMicroBundle();
        plant.Start();

        for (var i = 0; i < 60; i++)
            plant.Update();

        var progress = plant.Panels.Select(x => x.BloomProgress).ToArray();
        Assert.Contains(progress, value => value > 0);
        Assert.Contains(progress, value => value < 1);
        Assert.True(progress.Distinct().Count() > 1);
    }

    [Fact(DisplayName = "Growth paths terminate at their final mesh slots")]
    public void HubGrowth_VinesTerminateAtPanelAnchors()
    {
        using var plant = new PlantGrowthMicroBundle();

        foreach (var vine in plant.Vines)
        {
            var panel = PlantGrowthMicroBundle.FinalHubGeometry.Single(x => x.Id == vine.TargetId);
            Assert.Equal(panel.AnchorX, vine.TargetX);
            Assert.Equal(panel.AnchorY, vine.TargetY);
            Assert.Contains(vine.TargetX.ToString("0.##"), vine.Path);
            Assert.Contains(vine.TargetY.ToString("0.##"), vine.Path);
        }
    }

    [Fact(DisplayName = "The final bloom leaves every GUI panel at its authored mesh position")]
    public void HubGrowth_BloomPlacesPanelsDirectlyIntoFinalPositions()
    {
        using var plant = new PlantGrowthMicroBundle();
        plant.Start();

        for (var i = 0; i < 500 && !plant.IsSettled; i++)
            plant.Update();

        Assert.True(plant.IsSettled);

        foreach (var panel in plant.Panels)
        {
            Assert.Equal(1, panel.BloomProgress);
            var authored = PlantGrowthMicroBundle.FinalHubGeometry.Single(x => x.Id == panel.Id);
            Assert.Equal(authored.X, panel.X);
            Assert.Equal(authored.Y, panel.Y);
        }
    }
}