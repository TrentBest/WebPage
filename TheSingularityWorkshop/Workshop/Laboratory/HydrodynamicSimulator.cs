using System;

namespace TheSingularityWorkshop.Workshop.Laboratory;

public readonly record struct FluidCell(
    double DensityKgPerM3,
    double VelocityMPerS,
    double PressurePa,
    double TemperatureK);

public readonly record struct HydrodynamicStepResult(
    FluidCell Before,
    FluidCell After,
    double DynamicPressurePa,
    double ReynoldsNumber);

/// <summary>
/// Small deterministic finite-volume-style teaching model. It exposes the state
/// transition surface needed by future numerical solvers without pretending that
/// one cell reproduces a full CFD solution.
/// </summary>
public sealed class HydrodynamicSimulator
{
    public FluidCell State { get; private set; }

    public HydrodynamicSimulator(FluidCell initialState)
    {
        Validate(initialState);
        State = initialState;
    }

    public HydrodynamicStepResult Step(
        double accelerationMPerS2,
        double timeStepS,
        double characteristicLengthM,
        double dynamicViscosityPaS)
    {
        if (timeStepS <= 0) throw new ArgumentOutOfRangeException(nameof(timeStepS));
        if (characteristicLengthM <= 0) throw new ArgumentOutOfRangeException(nameof(characteristicLengthM));
        if (dynamicViscosityPaS <= 0) throw new ArgumentOutOfRangeException(nameof(dynamicViscosityPaS));

        var before = State;
        var velocity = before.VelocityMPerS + accelerationMPerS2 * timeStepS;
        var pressure = before.PressurePa +
                       before.DensityKgPerM3 * accelerationMPerS2 * characteristicLengthM;

        State = before with
        {
            VelocityMPerS = velocity,
            PressurePa = pressure
        };

        var dynamicPressure = 0.5 * before.DensityKgPerM3 * velocity * velocity;
        var reynolds = before.DensityKgPerM3 * Math.Abs(velocity) * characteristicLengthM / dynamicViscosityPaS;
        return new HydrodynamicStepResult(before, State, dynamicPressure, reynolds);
    }

    private static void Validate(FluidCell state)
    {
        if (state.DensityKgPerM3 <= 0) throw new ArgumentOutOfRangeException(nameof(state));
        if (state.PressurePa < 0) throw new ArgumentOutOfRangeException(nameof(state));
        if (state.TemperatureK <= 0) throw new ArgumentOutOfRangeException(nameof(state));
    }
}
