using System.Linq;
using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialAECModelTests
{
    [Fact]
    public void Workshop_Model_Separates_Core_From_Extensions()
    {
        var model = CreateModel();

        Assert.Equal(["forge"], model.Core.Select(x => x.Id));
        Assert.Equal(["external-tool"], model.Extensions.Select(x => x.Id));
    }

    [Fact]
    public void Moving_Workshop_Carries_Visitor_When_Visitor_Is_Inside()
    {
        var model = CreateModel();
        var session = new SpatialAECModelSession(model);

        session.MoveWorkshop(5, -3, visitorIsInside: true);

        Assert.Equal(new SpatialPoint(5, -3), session.BuildingTransform);
        Assert.Equal(new SpatialPoint(5, -3), session.VisitorTransform);
        Assert.Equal(new SpatialPoint(15, 7), session.VisitorWorldPosition(new SpatialPoint(10, 10)));
    }

    [Fact]
    public void Moving_Workshop_Does_Not_Move_External_Visitor()
    {
        var model = CreateModel();
        var session = new SpatialAECModelSession(model);

        session.MoveWorkshop(5, -3, visitorIsInside: false);

        Assert.Equal(new SpatialPoint(5, -3), session.BuildingTransform);
        Assert.Equal(new SpatialPoint(0, 0), session.VisitorTransform);
    }

    [Fact]
    public void Vr_God_Interaction_Places_Hand_On_Target_And_Avatar_Above()
    {
        var interaction = SpatialVrGodInteraction.Create(
            "forge",
            new SpatialPoint(20, 30),
            new SpatialPoint(20, 30),
            new SpatialPoint(20, 10));

        Assert.Equal("forge", interaction.TargetId);
        Assert.Equal(new SpatialPoint(20, 30), interaction.HandPoint);
        Assert.True(interaction.AvatarAboveTarget);
    }

    private static SpatialWorkshopModel CreateModel()
        => new(
            new SpatialBounds(0, 0, 100, 100),
            [
                new SpatialWorkshopModelStructure(
                    "forge",
                    "The Forge",
                    new SpatialBounds(10, 10, 20, 20),
                    true,
                    "forge"),
                new SpatialWorkshopModelStructure(
                    "external-tool",
                    "External Tool",
                    new SpatialBounds(50, 50, 20, 20),
                    false,
                    "external-tool")
            ]);
}
