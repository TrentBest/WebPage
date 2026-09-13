using System;
using TheSingularityWorkshop.Services;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Complete focused coverage of the Living GUI positional state contract.</summary>
public sealed class PageStateContextTests
{
    [Fact] public void ConstructorStartsValid() { var context = new PageStateContext(); Assert.True(context.IsValid); }
    [Fact] public void ConstructorUsesCanonicalName() { var context = new PageStateContext(); Assert.Equal("PageFSMContext", context.Name); }
    [Fact] public void ConstructorStartsWithoutLivingNodes() { var context = new PageStateContext(); Assert.Empty(context.LivingNodes); }
    [Fact] public void BeginLivingGuiCreatesExactlyOneRoot() { var context = new PageStateContext(); context.BeginLivingGui(); Assert.Single(context.LivingNodes); }
    [Fact] public void RootStartsAtHorizontalCenter() { var context = new PageStateContext(); context.BeginLivingGui(); Assert.Equal(50d, context.LivingNodes[0].X); }
    [Fact] public void RootStartsAtVerticalCenter() { var context = new PageStateContext(); context.BeginLivingGui(); Assert.Equal(50d, context.LivingNodes[0].Y); }
    [Fact] public void RootIsMarkedAsRoot() { var context = new PageStateContext(); context.BeginLivingGui(); Assert.True(context.LivingNodes[0].IsRoot); }
    [Fact] public void RootStartsAtGenerationZero() { var context = new PageStateContext(); context.BeginLivingGui(); Assert.Equal(0, context.LivingNodes[0].Generation); }
    [Fact] public void RootStartsWithCanonicalLineage() { var context = new PageStateContext(); context.BeginLivingGui(); Assert.Equal("G:0", context.LivingNodes[0].Lineage); }
    [Fact] public void RootStartsInRootGrowthPhase() { var context = new PageStateContext(); context.BeginLivingGui(); Assert.Equal(PageStateContext.LivingNodePhase.RootGrowth, context.LivingNodes[0].Phase); }
    [Fact] public void RootGrowthPreservesCenterCoordinates() { var context = new PageStateContext(); context.BeginLivingGui(); context.AdvanceRootGrowth(); var root = context.LivingNodes[0]; Assert.Equal(50d, root.X); Assert.Equal(50d, root.Y); }
    [Fact] public void RootReproductionPreservesParentCenterCoordinates() { var context = new PageStateContext(); context.BeginLivingGui(); context.AdvanceRootGrowth(); context.AdvanceReproduction(); Assert.True(context.LivingNodes.Count > 1); var child = context.LivingNodes[1]; Assert.Equal(50d, child.X); Assert.Equal(50d, child.Y); }
    [Fact] public void ChildTargetsAreInsideSafeHorizontalBounds() { var context = new PageStateContext(); context.BeginLivingGui(); context.AdvanceRootGrowth(); context.AdvanceReproduction(); foreach (var node in context.LivingNodes) Assert.InRange(node.TargetX, 10d, 90d); }
    [Fact] public void ChildTargetsAreInsideSafeVerticalBounds() { var context = new PageStateContext(); context.BeginLivingGui(); context.AdvanceRootGrowth(); context.AdvanceReproduction(); foreach (var node in context.LivingNodes) Assert.InRange(node.TargetY, 10d, 90d); }
    [Fact] public void SeedFlightMovesChildTowardItsTarget() { var context = new PageStateContext(); context.BeginLivingGui(); context.AdvanceRootGrowth(); context.AdvanceReproduction(); var child = context.LivingNodes[1]; var before = Distance(child.X, child.Y, child.TargetX, child.TargetY); context.AdvanceSeedFlight(); var after = Distance(child.X, child.Y, child.TargetX, child.TargetY); Assert.True(after <= before); }
    [Fact] public void ResetStateClockClearsStateTicksOnly() { var context = new PageStateContext(); context.StateTicks = 37; context.TotalTicks = 91; context.ResetStateClock(); Assert.Equal(0, context.StateTicks); Assert.Equal(91, context.TotalTicks); }

    private static double Distance(double x1, double y1, double x2, double y2) => Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
}
