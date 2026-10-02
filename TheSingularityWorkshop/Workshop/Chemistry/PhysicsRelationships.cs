using System;

namespace TheSingularityWorkshop.Workshop.Chemistry;

public static class PhysicsRelationships
{
    public static double ElectricalPowerWatts(double currentAmps, double voltageVolts) => currentAmps * voltageVolts;
    public static double VoltageVolts(double currentAmps, double resistanceOhms) => currentAmps * resistanceOhms;
    public static double ResistanceOhms(double resistivityOhmM, double lengthM, double areaM2) => resistivityOhmM * lengthM / RequirePositive(areaM2, nameof(areaM2));
    public static double ChargeCoulombs(double currentAmps, double timeS) => currentAmps * timeS;
    public static double ElectricalEnergyJoules(double powerWatts, double timeS) => powerWatts * timeS;

    public static double ForceNewtons(double massKg, double accelerationMPerS2) => massKg * accelerationMPerS2;
    public static double MechanicalPowerWatts(double forceNewtons, double velocityMPerS) => forceNewtons * velocityMPerS;
    public static double WorkJoules(double forceNewtons, double displacementM) => forceNewtons * displacementM;
    public static double KineticEnergyJoules(double massKg, double velocityMPerS) => 0.5 * massKg * velocityMPerS * velocityMPerS;
    public static double MomentumKgMPerS(double massKg, double velocityMPerS) => massKg * velocityMPerS;

    public static double DensityKgPerM3(double massKg, double volumeM3) => massKg / RequirePositive(volumeM3, nameof(volumeM3));
    public static double PressurePa(double forceNewtons, double areaM2) => forceNewtons / RequirePositive(areaM2, nameof(areaM2));
    public static double StressPa(double forceNewtons, double areaM2) => PressurePa(forceNewtons, areaM2);
    public static double Strain(double changeInLengthM, double originalLengthM) => changeInLengthM / RequirePositive(originalLengthM, nameof(originalLengthM));
    public static double ElasticStressPa(double youngsModulusPa, double strain) => youngsModulusPa * strain;

    public static double HeatEnergyJoules(double massKg, double specificHeatJPerKgK, double temperatureChangeK) =>
        massKg * specificHeatJPerKgK * temperatureChangeK;

    public static double ConductiveHeatRateWatts(double thermalConductivityWPerMK, double areaM2, double temperatureDifferenceK, double thicknessM) =>
        thermalConductivityWPerMK * RequirePositive(areaM2, nameof(areaM2)) * temperatureDifferenceK / RequirePositive(thicknessM, nameof(thicknessM));

    public static double ThermalExpansionM(double coefficientPerK, double originalLengthM, double temperatureChangeK) =>
        coefficientPerK * originalLengthM * temperatureChangeK;

    public static double HydrostaticPressurePa(double densityKgPerM3, double gravitationalAccelerationMPerS2, double depthM) =>
        densityKgPerM3 * gravitationalAccelerationMPerS2 * RequireNonNegative(depthM, nameof(depthM));

    public static double DynamicPressurePa(double densityKgPerM3, double velocityMPerS) =>
        0.5 * densityKgPerM3 * velocityMPerS * velocityMPerS;

    public static double VolumetricFlowRateM3PerS(double areaM2, double velocityMPerS) =>
        RequirePositive(areaM2, nameof(areaM2)) * velocityMPerS;

    public static double ReynoldsNumber(double densityKgPerM3, double velocityMPerS, double characteristicLengthM, double dynamicViscosityPaS) =>
        densityKgPerM3 * velocityMPerS * RequirePositive(characteristicLengthM, nameof(characteristicLengthM)) /
        RequirePositive(dynamicViscosityPaS, nameof(dynamicViscosityPaS));

    public static double BuoyantForceNewtons(double displacedFluidDensityKgPerM3, double displacedVolumeM3, double gravitationalAccelerationMPerS2) =>
        displacedFluidDensityKgPerM3 * RequirePositive(displacedVolumeM3, nameof(displacedVolumeM3)) * gravitationalAccelerationMPerS2;

    public static double VolumetricStrain(double volumeChangeM3, double originalVolumeM3) =>
        volumeChangeM3 / RequirePositive(originalVolumeM3, nameof(originalVolumeM3));

    public static double BulkModulusPa(double pressureChangePa, double volumetricStrain) =>
        -pressureChangePa / RequireNonZero(volumetricStrain, nameof(volumetricStrain));

    public static double ModeIStressIntensityPaSqrtM(double geometryFactor, double normalStressPa, double crackLengthM) =>
        geometryFactor * normalStressPa * Math.Sqrt(Math.PI * RequirePositive(crackLengthM, nameof(crackLengthM)));

    public static double CriticalCrackLengthM(double fractureToughnessPaSqrtM, double geometryFactor, double normalStressPa) =>
        Math.Pow(
            fractureToughnessPaSqrtM /
            (RequireNonZero(geometryFactor, nameof(geometryFactor)) * RequirePositive(normalStressPa, nameof(normalStressPa))),
            2) / Math.PI;

    public static double ParisCrackGrowthRateMPerCycle(double parisCoefficient, double stressIntensityRangePaSqrtM, double parisExponent) =>
        parisCoefficient * Math.Pow(RequirePositive(stressIntensityRangePaSqrtM, nameof(stressIntensityRangePaSqrtM)), parisExponent);

    public static double DecayConstantFromHalfLifeSeconds(double halfLifeS) => Math.Log(2) / RequirePositive(halfLifeS, nameof(halfLifeS));

    public static double DecayedQuantity(double initialQuantity, double decayConstantPerS, double timeS) =>
        initialQuantity * Math.Exp(-decayConstantPerS * RequireNonNegative(timeS, nameof(timeS)));

    public static double ActivityBecquerels(double quantity, double decayConstantPerS) =>
        quantity * RequireNonNegative(decayConstantPerS, nameof(decayConstantPerS));

    public static double MassEnergyJoules(double massKg)
    {
        const double SpeedOfLightMPerS = 299_792_458;
        return massKg * SpeedOfLightMPerS * SpeedOfLightMPerS;
    }

    private static double RequirePositive(double value, string parameterName)
    {
        if (value <= 0) throw new ArgumentOutOfRangeException(parameterName, value, "Value must be greater than zero.");
        return value;
    }

    private static double RequireNonZero(double value, string parameterName)
    {
        if (value == 0) throw new ArgumentOutOfRangeException(parameterName, value, "Value must not be zero.");
        return value;
    }

    private static double RequireNonNegative(double value, string parameterName)
    {
        if (value < 0) throw new ArgumentOutOfRangeException(parameterName, value, "Value must not be negative.");
        return value;
    }
}
