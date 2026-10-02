using System;

namespace TheSingularityWorkshop.Workshop.Chemistry;

/// <summary>
/// Fundamental physical relationships used to derive behavior from sourced measurements.
/// These methods do not store derived measurements on elemental identity.
/// </summary>
public static class PhysicsRelationships
{
    /// <summary>Electrical power: P = I × V.</summary>
    public static double ElectricalPowerWatts(double currentAmps, double voltageVolts)
        => currentAmps * voltageVolts;

    /// <summary>Ohm's law: V = I × R.</summary>
    public static double VoltageVolts(double currentAmps, double resistanceOhms)
        => currentAmps * resistanceOhms;

    /// <summary>Electrical resistance from resistivity: R = ρL/A.</summary>
    public static double ResistanceOhms(double resistivityOhmM, double lengthM, double areaM2)
        => resistivityOhmM * lengthM / RequirePositive(areaM2, nameof(areaM2));

    /// <summary>Charge transferred by current over time: Q = It.</summary>
    public static double ChargeCoulombs(double currentAmps, double timeS)
        => currentAmps * timeS;

    /// <summary>Electrical energy from power over time: E = Pt.</summary>
    public static double ElectricalEnergyJoules(double powerWatts, double timeS)
        => powerWatts * timeS;

    /// <summary>Newton's second law: F = ma.</summary>
    public static double ForceNewtons(double massKg, double accelerationMPerS2)
        => massKg * accelerationMPerS2;

    /// <summary>Mechanical power from force and velocity: P = Fv.</summary>
    public static double MechanicalPowerWatts(double forceNewtons, double velocityMPerS)
        => forceNewtons * velocityMPerS;

    /// <summary>Work for force parallel to displacement: W = Fd.</summary>
    public static double WorkJoules(double forceNewtons, double displacementM)
        => forceNewtons * displacementM;

    /// <summary>Kinetic energy: E = 1/2 mv².</summary>
    public static double KineticEnergyJoules(double massKg, double velocityMPerS)
        => 0.5 * massKg * velocityMPerS * velocityMPerS;

    /// <summary>Linear momentum: p = mv.</summary>
    public static double MomentumKgMPerS(double massKg, double velocityMPerS)
        => massKg * velocityMPerS;

    /// <summary>Density: ρ = m/V.</summary>
    public static double DensityKgPerM3(double massKg, double volumeM3)
        => massKg / RequirePositive(volumeM3, nameof(volumeM3));

    /// <summary>Pressure: p = F/A.</summary>
    public static double PressurePa(double forceNewtons, double areaM2)
        => forceNewtons / RequirePositive(areaM2, nameof(areaM2));

    /// <summary>Normal stress: σ = F/A.</summary>
    public static double StressPa(double forceNewtons, double areaM2)
        => PressurePa(forceNewtons, areaM2);

    /// <summary>Engineering strain: ε = ΔL/L.</summary>
    public static double Strain(double changeInLengthM, double originalLengthM)
        => changeInLengthM / RequirePositive(originalLengthM, nameof(originalLengthM));

    /// <summary>Linear elastic Hooke relationship: σ = Eε.</summary>
    public static double ElasticStressPa(double youngsModulusPa, double strain)
        => youngsModulusPa * strain;

    /// <summary>Mode-I stress intensity: K = Yσ√(πa).</summary>
    /// <remarks>
    /// This is the generic linear-elastic fracture-mechanics form; Y captures crack geometry.
    /// </remarks>
    public static double ModeIStressIntensityPaSqrtM(
        double geometryFactor,
        double normalStressPa,
        double crackLengthM)
        => geometryFactor * normalStressPa * Math.Sqrt(Math.PI * RequirePositive(crackLengthM, nameof(crackLengthM)));

    private static double RequirePositive(double value, string parameterName)
    {
        if (value <= 0)
            throw new ArgumentOutOfRangeException(parameterName, value, "Value must be greater than zero.");

        return value;
    }
}
