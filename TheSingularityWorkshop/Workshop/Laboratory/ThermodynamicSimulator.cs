using System;

namespace TheSingularityWorkshop.Workshop.Laboratory;

public readonly record struct ThermalState(
    double MassKg,
    double SpecificHeatJPerKgK,
    double TemperatureK);

public readonly record struct ThermalStepResult(
    ThermalState Before,
    ThermalState After,
    double EnergyAddedJoules);

/// <summary>
/// Deterministic calorimetry state integrator. The material properties are inputs,
/// allowing sourced elemental/material data to drive the experiment later.
/// </summary>
public sealed class ThermodynamicSimulator
{
    public ThermalState State { get; private set; }

    public ThermodynamicSimulator(ThermalState initialState)
    {
        if (initialState.MassKg <= 0 || initialState.SpecificHeatJPerKgK <= 0 || initialState.TemperatureK <= 0)
            throw new ArgumentOutOfRangeException(nameof(initialState));
        State = initialState;
    }

    public ThermalStepResult AddHeat(double energyJoules)
    {
        if (energyJoules < 0)
            throw new ArgumentOutOfRangeException(nameof(energyJoules));

        var before = State;
        var deltaTemperature = energyJoules / (before.MassKg * before.SpecificHeatJPerKgK);
        State = before with { TemperatureK = before.TemperatureK + deltaTemperature };
        return new ThermalStepResult(before, State, energyJoules);
    }
}
