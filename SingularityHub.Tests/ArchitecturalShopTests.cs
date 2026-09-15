using TheSingularityWorkshop.Workshop.Architecture;
using Xunit;

namespace SingularityHub.Tests;

public sealed class ArchitecturalShopTests
{
    [Fact]
    public void Editor_CapturesFloorPlanAsArchitecturalStructure()
    {
        var editor = new ArchitecturalShopEditor();

        editor.SelectTool(ArchitecturalCellKind.Room);
        editor.Apply(6, 6);
        editor.SelectTool(ArchitecturalCellKind.Stair);
        editor.Apply(7, 6);

        var structure = editor.Capture();

        Assert.Single(structure.Floors);
        Assert.Contains(structure.Floors[0].Cells, cell => cell is { X: 6, Y: 6, Kind: ArchitecturalCellKind.Room });
        Assert.Contains(structure.Floors[0].Cells, cell => cell is { X: 7, Y: 6, Kind: ArchitecturalCellKind.Stair });
        Assert.Equal(4, structure.VerticalConnectorCount);
        Assert.Equal(2, structure.TotalCellCount);
    }

    [Fact]
    public void Templates_ProvideVerticalCirculationAndDifferentStartingPlans()
    {
        var plans = new[]
        {
            ArchitecturalFloorPlanTemplates.OpenWorkshop,
            ArchitecturalFloorPlanTemplates.Gallery,
            ArchitecturalFloorPlanTemplates.TowerCore
        };

        Assert.Equal(3, plans.Select(plan => plan.Id).Distinct().Count());
        Assert.All(plans, plan => Assert.NotEmpty(plan.VerticalConnectors));
        Assert.Contains(plans[0].VerticalConnectors, connector => connector.Kind == ArchitecturalVerticalConnectorKind.Ladder);
        Assert.Contains(plans[1].VerticalConnectors, connector => connector.Kind == ArchitecturalVerticalConnectorKind.Elevator);
        Assert.True(plans[2].VerticalConnectors.Any(connector => connector.ToFloor > 1));
    }
}
