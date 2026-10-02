using System;
using TheSingularityWorkshop.Workshop.Chemistry;
using Xunit;

namespace SingularityHub.Tests;

public sealed class PhysicsRelationshipsTests
{
    [Fact]
    public void ElectricalRelationships_AreDerived()
    {
        Assert.Equal(60, PhysicsRelationships.ElectricalPowerWatts(5, 12));
        Assert.Equal(24, PhysicsRelationships.VoltageVolts(2, 12));
        Assert.Equal(6, PhysicsRelationships.ResistanceOhms(2, 3, 1));
        Assert.Equal(10, PhysicsRelationships.ChargeCoulombs(2, 5));
        Assert.Equal(300, PhysicsRelationships.ElectricalEnergyJoules(60, 5));
    }

    [Fact]
    public void MechanicsRelationships_AreDerived()
    {
        Assert.Equal(20, PhysicsRelationships.ForceNewtons(5, 4));
        Assert.Equal(60, PhysicsRelationships.MechanicalPowerWatts(20, 3));
        Assert.Equal(100, PhysicsRelationships.WorkJoules(20, 5));
        Assert.Equal(50, PhysicsRelationships.KineticEnergyJoules(4, 5));
        Assert.Equal(20, PhysicsRelationships.MomentumKgMPerS(4, 5));
    }

    [Fact]
    public void ContinuumAndFractureRelationships_AreDerived()
    {
        Assert.Equal(1000, PhysicsRelationships.DensityKgPerM3(10, 0.01));
        Assert.Equal(100, PhysicsRelationships.PressurePa(200, 2));
        Assert.Equal(100, PhysicsRelationships.StressPa(200, 2));
        Assert.Equal(0.02, PhysicsRelationships.Strain(0.01, 0.5));
        Assert.Equal(200, PhysicsRelationships.ElasticStressPa(10000, 0.02));

        var expected = 2 * 100 * Math.Sqrt(Math.PI * 0.25);
        Assert.Equal(expected, PhysicsRelationships.ModeIStressIntensityPaSqrtM(2, 100, 0.25), 10);
    }

    [Fact]
    public void GeometryRelationships_Reject_NonPositive_Denominators()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => PhysicsRelationships.DensityKgPerM3(1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => PhysicsRelationships.PressurePa(1, -1));
        Assert.Throws<ArgumentOutOfRangeException>(() => PhysicsRelationships.ResistanceOhms(1, 1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => PhysicsRelationships.Strain(1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => PhysicsRelationships.ModeIStressIntensityPaSqrtM(1, 1, 0));
    }
}
