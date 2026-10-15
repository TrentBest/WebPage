using System;
using TheSingularityWorkshop.Workshop.MicroBundles;
using Xunit;

namespace SingularityHub.Tests;

public sealed class BuildingPlanTests
{
    [Fact]
    public void ExamplePlanContainsSemanticOpenings()
    {
        var plan = PlanSketchExamples.CompactApartment;

        Assert.Equal("example.compact-apartment", plan.Id);
        Assert.Equal(4, plan.Rooms.Count);
        Assert.Contains(plan.Openings, x => x.Kind == PlanOpeningKind.HingedDoor);
        Assert.Contains(plan.Openings, x => x.Kind == PlanOpeningKind.SlidingDoor);
        Assert.Contains(plan.Openings, x => x.Kind == PlanOpeningKind.PocketDoor);
        Assert.Contains(plan.Openings, x => x.Kind == PlanOpeningKind.RevolvingDoor);
        Assert.Contains(plan.Openings, x => x.Kind == PlanOpeningKind.Window);
    }

    [Fact]
    public void OpeningMustReferToExistingWall()
    {
        var error = Assert.Throws<ArgumentException>(() => new BuildingPlan(
            "bad", "Bad plan", 10, 10, Array.Empty<PlanRoom>(),
            [new("wall", 0, 0, 10, 0)],
            [new("door", "missing-wall", 1, 1, PlanOpeningKind.HingedDoor)]));

        Assert.Contains("missing wall", error.Message);
    }

    [Fact]
    public void RoomsMustRemainInsidePlanExtents()
    {
        Assert.Throws<ArgumentException>(() => new BuildingPlan(
            "bad", "Bad plan", 10, 10,
            [new("room", "Outside", 9, 0, 2, 2, "Other")],
            Array.Empty<PlanWall>(), Array.Empty<PlanOpening>()));
    }

    [Fact]
    public void OpeningFractionMustBeNormalized()
    {
        Assert.Throws<ArgumentException>(() => new BuildingPlan(
            "bad", "Bad plan", 10, 10, Array.Empty<PlanRoom>(),
            [new("wall", 0, 0, 10, 0)],
            [new("door", "wall", 1, 1, PlanOpeningKind.SlidingDoor, OpenFraction: 1.5)]));
    }

    [Fact]
    public void OpeningRejectsUnknownSemanticEnumValues()
    {
        Assert.Throws<ArgumentException>(() => new BuildingPlan(
            "bad", "Bad plan", 10, 10, Array.Empty<PlanRoom>(),
            [new("wall", 0, 0, 10, 0)],
            [new("door", "wall", 1, 1, (PlanOpeningKind)99)]));
    }

    [Fact]
    public void ProviderExposesPlanCapabilityWithoutGuiDependency()
    {
        var provider = new PlanViewMicroBundleProvider();
        var manifestation = provider.Manifest(new PlanContext());

        Assert.Equal(PlanViewMicroBundleProvider.BundleId, manifestation.Id);
        Assert.Equal("building.plan", manifestation.Target);
        Assert.Equal("PlanView", manifestation.Effect);
        Assert.Equal(4, manifestation.Parameters["rooms"]);
    }

    private sealed class PlanContext : TheSingularityWorkshop.FSM_API.IStateContext
    {
        public string Name { get; set; } = "test-plan";
        public bool IsValid { get; set; } = true;
    }
}
