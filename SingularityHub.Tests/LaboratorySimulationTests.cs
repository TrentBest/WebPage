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
        Assert.Equal(101_500, step.After.PressurePa);
        Assert.Equal(32_000, step.DynamicPressurePa);
        Assert.Equal(400_000, step.ReynoldsNumber);
        Assert.Equal(step.After, simulator.State);
    }

    [Fact]
    public void LandscapeHydrodynamics_Conserves_Water_When_Flowing_Between_Terrain_Cells()
    {
        var simulator = new LandscapeHydrodynamicsSimulator(2, 1, [0d, -1d]);
        simulator.ApplyWave(new WaveForcing(0, 0, 2, 0, 0));

        var before = simulator.GetCell(0, 0).WaterDepthM + simulator.GetCell(1, 0).WaterDepthM;
        simulator.Step(0.1);
        var after = simulator.GetCell(0, 0).WaterDepthM + simulator.GetCell(1, 0).WaterDepthM;

        Assert.Equal(2, before);
        Assert.Equal(before, after, 10);
        Assert.True(simulator.GetCell(1, 0).WaterDepthM > 0);
    }

    [Fact]
    public void LandscapeHydrodynamics_Supports_Custom_Boundaries_And_Water_Fill()
    {
        var simulator = new LandscapeHydrodynamicsSimulator(3, 1, [0d, 0d, 0d]);
        simulator.SetBoundary(2, 0, false);
        simulator.FillWater(2);

        Assert.Equal(2, simulator.GetCell(0, 0).WaterDepthM);
        Assert.Equal(2, simulator.GetCell(1, 0).WaterDepthM);
        Assert.Equal(0, simulator.GetCell(2, 0).WaterDepthM);
        Assert.False(simulator.ActiveCells[2]);
    }

    [Fact]
    public void LandscapeHydrodynamics_Applies_Weather_Tectonics_Waves_And_Boat_Forcing()
    {
        var simulator = new LandscapeHydrodynamicsSimulator(3, 3, new double[9]);

        simulator.ApplyWeather(new WeatherForcing(0.1, 4, 2, 10));
        simulator.ApplyTectonics(new TectonicForcing([0, 0, 0, 0, 1, 0, 0, 0, 0]));
        simulator.ApplyWave(new WaveForcing(1, 1, 3, 2, 0));
        simulator.LaunchBoat(new BoatState("toy-boat", 1, 1, 10, 0.02));

        var step = simulator.Step(0.1);

        Assert.Equal(1, step.Step);
        Assert.Equal(1, simulator.GetCell(1, 1).TerrainElevationM, 10);
        Assert.True(step.MaximumWaterDepthM > 0);
        Assert.Single(step.Boats);
        Assert.Equal("toy-boat", step.Boats[0].Id);
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
