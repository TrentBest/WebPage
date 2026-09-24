using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialConstructionAndMapTests
{
    [Fact(DisplayName = "Incremental Unit Test 51 — Construction real estate encloses every work vehicle")]
    public void ConstructionSiteEnclosesVehicles()
    {
        var scene = SpatialWorkshopScene.CreateDefault();

        Assert.All(scene.ConstructionVehicles, vehicle => Assert.True(scene.ConstructionSite.Bounds.Contains(vehicle.Position)));
        Assert.True(scene.ConstructionSite.Bounds.Width > scene.ConstructionSite.FoundationBounds.Width);
        Assert.True(scene.ConstructionSite.Bounds.Height > scene.ConstructionSite.FoundationBounds.Height);
    }



    [Fact(DisplayName = "Workshop map presents an architectural directory rather than bare rectangles")]
    public void WorkshopMapUsesArchitecturalDirectoryPresentation()
    {
        var map = File.ReadAllText(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "TheSingularityWorkshop", "Gui", "SpatialWorkshopMapGuiBuilder.Rendering.cs"));

        Assert.Contains("MapBuildingGeometry(receiver, item, b, accent)", map);
        Assert.Contains("workshop-map-building-info", map);
        Assert.Contains("SingularityCampusCatalog.Buildings.FirstOrDefault", map);
        Assert.Contains("Attribute(\"points\", \"5,13 13,5 87,5 95,13 95,87 87,95 13,95 5,87\")", map);
        Assert.Contains("workshop-map-directory-item", map);
        Assert.Contains("HOVER FOR LIVE DETAILS", map);
    }

    [Fact(DisplayName = "Workshop map docks You and AI below its navigation controls")]
    public void WorkshopMapOwnsHubIdentityDock()
    {
        var map = File.ReadAllText(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "TheSingularityWorkshop", "Gui", "SpatialWorkshopMapGuiBuilder.Rendering.cs"));

        var ai = File.ReadAllText(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "TheSingularityWorkshop", "Gui", "WorkshopAiTerminalGuiBuilder.cs"));

        Assert.Contains("ElementBuilder? hubIdentityDock = null", map);
        Assert.Contains("panel.Content(scopeBar);", map);
        Assert.Contains("panel.Content(hubIdentityDock);", map);
        Assert.Contains("bool docked = false", ai);
        Assert.Contains(".Style(\"position\", docked ? \"relative\" : \"fixed\")", ai);
        Assert.Contains("SESSION LINK", ai);
    }

    [Fact(DisplayName = "Incremental Unit Test 52 — Campus spacing separates the construction site from the ontology mall")]
    public void ConstructionSiteAndMallAreSeparate()
    {
        var scene = SpatialWorkshopScene.CreateDefault();
        var lab = scene.Interactables.Single(item => item.Id == "singularity-lab");
        var site = scene.ConstructionSite.Bounds;

        Assert.True(site.X > lab.Bounds.X + lab.Bounds.Width);
    }
}
