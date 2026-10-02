using TheSingularityWorkshop.Workshop.Laboratory;
using Xunit;

namespace SingularityHub.Tests;

public sealed class LaboratorySimulationTests
{
    [Fact]
    public void ParticleCollider_Conserves_Configured_Input_Energy()
    {
        var collider = new ParticleColliderSimulator();
        collider.Configure(
            new ColliderParticle("proton-a", 1.67262192369e-27, 1.602176634e-19, 8),
            new ColliderParticle("proton-b", 1.67262192369e-27, 1.602176634e-19, 8));

        var collision = collider.Collide(16);

        Assert.Equal(1, collision.EventId);
        Assert.Equal(16, collision.TotalInputEnergyJoules);
        Assert.True(collision.ConservesEnergy);
        Assert.Equal(0, collision.EnergyResidualJoules);
    }

    [Fact]
    public void HydrodynamicSimulator_Advances_Fluid_State_And_Measures_Regime()
    {
        var simulator = new HydrodynamicSimulator(
            new FluidCell(1000, 2, 100_000, 300));

        var step = simulator.Step(3, 2, 0.5, 0.01);

        Assert.Equal(8, step.After.VelocityMPerS);
        Assert.Equal(104_000, step.After.PressurePa);
        Assert.Equal(32_000, step.DynamicPressurePa);
        Assert.Equal(400_000, step.ReynoldsNumber);
        Assert.Equal(step.After, simulator.State);
    }

    [Fact]
    public void ThermodynamicSimulator_Conserves_Added_Heat_Through_Temperature_Change()
    {
        var simulator = new ThermodynamicSimulator(new ThermalState(2, 900, 300));

        var step = simulator.AddHeat(18_000);

        Assert.Equal(310, step.After.TemperatureK);
        Assert.Equal(18_000, step.EnergyAddedJoules);
        Assert.Equal(step.After, simulator.State);
    }

    [Fact]
    public void ElectromagneticCircuitSimulator_Derives_Current_Power_And_Energy()
    {
        var simulator = new ElectromagneticCircuitSimulator(12, 6);

        var measurement = simulator.Measure(10);

        Assert.Equal(2, measurement.State.CurrentAmps);
        Assert.Equal(24, measurement.PowerWatts);
        Assert.Equal(240, measurement.EnergyJoules);
    }
}
