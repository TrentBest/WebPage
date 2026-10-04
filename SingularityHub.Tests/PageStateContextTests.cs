using System;
using TheSingularityWorkshop.Services;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>
/// Complete focused coverage of the Living GUI positional state contract.
///
/// Ontology identity is intentionally represented with L1-L9 placeholders until
/// the production ontology values are authored. Do not invent ontology values.
/// </summary>
public sealed class PageStateContextTests
{
    [Fact] public void L1_L2_L3_L4_L5_L6_L7_L8_L9_ConstructorStartsValid() { var context = new PageStateContext(); Assert.True(context.IsValid); }
    [Fact] public void L1_L2_L3_L4_L5_L6_L7_L8_L9_ConstructorUsesCanonicalName() { var context = new PageStateContext(); Assert.Equal("PageFSMContext", context.Name); }
    [Fact] public void L1_L2_L3_L4_L5_L6_L7_L8_L9_ConstructorStartsWithoutLivingNodes() { var context = new PageStateContext(); Assert.Empty(context.LivingNodes); }
    [Fact] public void L1_L2_L3_L4_L5_L6_L7_L8_L9_BeginLivingGuiCreatesExactlyOneRoot() { var context = new PageStateContext(); context.BeginLivingGui(); Assert.Single(context.LivingNodes); }
    [Fact] public void L1_L2_L3_L4_L5_L6_L7_L8_L9_RootStartsAtHorizontalCenter() { var context = new PageStateContext(); context.BeginLivingGui(); Assert.Equal(50d, context.LivingNodes[0].X); }
    [Fact] public void L1_L2_L3_L4_L5_L6_L7_L8_L9_RootStartsAtVerticalCenter() { var context = new PageStateContext(); context.BeginLivingGui(); Assert.Equal(50d, context.LivingNodes[0].Y); }
    [Fact] public void L1_L2_L3_L4_L5_L6_L7_L8_L9_RootIsMarkedAsRoot() { var context = new PageStateContext(); context.BeginLivingGui(); Assert.True(context.LivingNodes[0].IsRoot); }
    [Fact] public void L1_L2_L3_L4_L5_L6_L7_L8_L9_RootStartsAtGenerationZero() { var context = new PageStateContext(); context.BeginLivingGui(); Assert.Equal(0, context.LivingNodes[0].Generation); }
    [Fact] public void L1_L2_L3_L4_L5_L6_L7_L8_L9_RootStartsWithCanonicalLineage() { var context = new PageStateContext(); context.BeginLivingGui(); Assert.Equal("G:0", context.LivingNodes[0].Lineage); }
    [Fact] public void L1_L2_L3_L4_L5_L6_L7_L8_L9_RootStartsInRootGrowthPhase() { var context = new PageStateContext(); context.BeginLivingGui(); Assert.Equal(PageStateContext.LivingNodePhase.RootGrowth, context.LivingNodes[0].Phase); }
    [Fact] public void L1_L2_L3_L4_L5_L6_L7_L8_L9_RootGrowthPreservesCenterCoordinates() { var context = new PageStateContext(); context.BeginLivingGui(); context.AdvanceRootGrowth(); var root = context.LivingNodes[0]; Assert.Equal(50d, root.X); Assert.Equal(50d, root.Y); }
    [Fact] public void L1_L2_L3_L4_L5_L6_L7_L8_L9_RootReproductionPreservesParentCenterCoordinates() { var context = new PageStateContext(); context.BeginLivingGui(); AdvanceRootToReproduction(context); context.AdvanceReproduction(); Assert.True(context.LivingNodes.Count > 1); var child = context.LivingNodes[1]; Assert.Equal(50d, child.X); Assert.Equal(50d, child.Y); }
    [Fact] public void L1_L2_L3_L4_L5_L6_L7_L8_L9_ChildTargetsAreInsideSafeHorizontalBounds() { var context = new PageStateContext(); context.BeginLivingGui(); AdvanceRootToReproduction(context); context.AdvanceReproduction(); foreach (var node in context.LivingNodes) { if (node.IsRoot) continue; Assert.InRange(node.TargetX, 10d, 90d); } }
    [Fact] public void L1_L2_L3_L4_L5_L6_L7_L8_L9_ChildTargetsAreInsideSafeVerticalBounds() { var context = new PageStateContext(); context.BeginLivingGui(); AdvanceRootToReproduction(context); context.AdvanceReproduction(); foreach (var node in context.LivingNodes) { if (node.IsRoot) continue; Assert.InRange(node.TargetY, 10d, 90d); } }
    [Fact] public void L1_L2_L3_L4_L5_L6_L7_L8_L9_SeedFlightMovesChildTowardItsTarget() { var context = new PageStateContext(); context.BeginLivingGui(); AdvanceRootToReproduction(context); context.AdvanceReproduction(); var child = context.LivingNodes[1]; var before = Distance(child.X, child.Y, child.TargetX, child.TargetY); context.AdvanceSeedFlight(); var after = Distance(child.X, child.Y, child.TargetX, child.TargetY); Assert.True(after <= before); }
    [Fact] public void L1_L2_L3_L4_L5_L6_L7_L8_L9_PopulationPreservesRootAtPanelCenter() { var context = new PageStateContext(); context.BeginLivingGui(); const int guard = 10000; var ticks = 0; while (context.LivingNodes.Count < PageStateContext.PopulationObservationThreshold && ticks++ < guard) { context.AdvanceRootGrowth(); context.AdvanceSeedFlight(); context.AdvanceSeedScaling(); context.AdvanceMatureGrowth(); context.AdvanceReproduction(); context.AdvanceParentRecovery(); } Assert.True(context.LivingNodes.Count >= PageStateContext.PopulationObservationThreshold); var root = context.LivingNodes[0]; Assert.True(root.IsRoot); Assert.Equal(50d, root.X); Assert.Equal(50d, root.Y); }
    [Fact] public void L1_L2_L3_L4_L5_L6_L7_L8_L9_PopulationChildrenRemainWithinPanelCoordinateBounds() { var context = new PageStateContext(); context.BeginLivingGui(); const int guard = 10000; var ticks = 0; while (context.LivingNodes.Count < PageStateContext.PopulationObservationThreshold && ticks++ < guard) { context.AdvanceRootGrowth(); context.AdvanceSeedFlight(); context.AdvanceSeedScaling(); context.AdvanceMatureGrowth(); context.AdvanceReproduction(); context.AdvanceParentRecovery(); } Assert.True(context.LivingNodes.Count >= PageStateContext.PopulationObservationThreshold); foreach (var node in context.LivingNodes) { if (node.IsRoot) continue; Assert.InRange(node.X, 10d, 90d); Assert.InRange(node.Y, 10d, 90d); } }
    [Fact] public void L1_L2_L3_L4_L5_L6_L7_L8_L9_ResetStateClockClearsStateTicksOnly() { var context = new PageStateContext(); context.StateTicks = 37; context.TotalTicks = 91; context.ResetStateClock(); Assert.Equal(0, context.StateTicks); Assert.Equal(91, context.TotalTicks); }

    private static void AdvanceRootToReproduction(PageStateContext context)
    {
        for (var i = 0; i < 4; i++)
            context.AdvanceRootGrowth();
    }

    private static double Distance(double x1, double y1, double x2, double y2) => Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
}
