using TheSingularityWorkshop.Workshop.Chemistry;
using Xunit;

namespace SingularityHub.Tests;

public sealed class HadronColliderSimulatorTests
{
    [Fact]
    public void Collider_RunsInjectionAccelerationCollisionAndAnalysis()
    {
        var collider = new HadronColliderSimulator();
        collider.Configure(HadronKind.Proton, HadronKind.Proton, 7, 2.5);

        collider.Inject();
        Assert.Equal(HadronColliderSimulationState.Injecting, collider.State);
        Assert.True(collider.ClockwiseBeam!.Value.Active);

        collider.Accelerate();
        Assert.Equal(HadronColliderSimulationState.Accelerating, collider.State);

        var collision = collider.Collide("HADRON DETECTOR");

        Assert.Equal(HadronColliderSimulationState.Analyzing, collider.State);
        Assert.Equal(1, collision.EventId);
        Assert.Equal(14, collision.CenterOfMassEnergyTeV);
        Assert.Equal("HADRON DETECTOR", collision.DetectorChannel);
        Assert.Equal(collision, collider.LastCollision);
    }

    [Fact]
    public void Collider_RequiresThePhysicalControlSequence()
    {
        var collider = new HadronColliderSimulator();
        collider.Configure(HadronKind.Proton, HadronKind.Antiproton, 3.5, 1);

        Assert.Throws<InvalidOperationException>(() => collider.Collide());
        Assert.Throws<InvalidOperationException>(() => collider.Accelerate());
    }
}
