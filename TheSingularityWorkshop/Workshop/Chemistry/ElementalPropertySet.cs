using System;

namespace TheSingularityWorkshop.Workshop.Chemistry;

/// <summary>
/// Structured elemental/material properties that can be populated by an authoritative
/// data source. Nullable values mean the source does not provide that property.
/// Values are expressed in SI units unless the field explicitly says otherwise.
/// </summary>
public sealed record ElementalPropertySet
{
    public double? DensityKgPerM3 { get; init; }
    public double? MeltingPointK { get; init; }
    public double? BoilingPointK { get; init; }
    public double? TriplePointPressurePa { get; init; }
    public double? CriticalTemperatureK { get; init; }
    public double? CriticalPressurePa { get; init; }

    public double? SpecificHeatJPerKgK { get; init; }
    public double? ThermalConductivityWPerMK { get; init; }
    public double? ThermalExpansionPerK { get; init; }

    public double? ElectricalResistivityOhmM { get; init; }
    public double? MagneticSusceptibility { get; init; }
    public double? RelativePermittivity { get; init; }

    public double? YoungsModulusPa { get; init; }
    public double? ShearModulusPa { get; init; }
    public double? BulkModulusPa { get; init; }
    public double? PoissonsRatio { get; init; }
    public double? YieldStrengthPa { get; init; }
    public double? UltimateTensileStrengthPa { get; init; }
    public double? FractureToughnessPaSqrtM { get; init; }
    public double? HardnessVickers { get; init; }
    public double? FatigueStrengthPa { get; init; }

    public double? VaporPressurePa { get; init; }
    public double? DynamicViscosityPaS { get; init; }
    public double? SurfaceTensionNPerM { get; init; }

    public double? SpecificActivityBqPerKg { get; init; }
    public double? HalfLifeS { get; init; }
    public double? NeutronCrossSectionBarn { get; init; }

    /// <summary>
    /// Optional provenance identifier for the dataset or record that supplied these values.
    /// </summary>
    public string? SourceId { get; init; }

    /// <summary>
    /// Optional version/revision of the source dataset.
    /// </summary>
    public string? SourceVersion { get; init; }

    /// <summary>
    /// Optional uncertainty note or machine-readable uncertainty reference.
    /// </summary>
    public string? Uncertainty { get; init; }
}

/// <summary>
/// Supplies elemental properties independently of presentation and arbitration.
/// Implementations may be backed by static data, a serialized warehouse page,
/// or another authoritative provider.
/// </summary>
public interface IElementalDataSource
{
    bool TryGetProperties(string symbol, out ElementalPropertySet? properties);
}

/// <summary>
/// Fundamental relationships exposed by the chemistry/physics domain.
/// These are equations, not additional stored measurements.
/// </summary>
public static class PhysicsRelationships
{
    /// <summary>Electrical power: P = I × V.</summary>
    public static double ElectricalPowerWatts(double currentAmps, double voltageVolts)
        => currentAmps * voltageVolts;

    /// <summary>Newton's second law: F = m × a.</summary>
    public static double ForceNewtons(double massKg, double accelerationMPerS2)
        => massKg * accelerationMPerS2;

    /// <summary>Mechanical power from force and velocity: P = F × v.</summary>
    public static double MechanicalPowerWatts(double forceNewtons, double velocityMPerS)
        => forceNewtons * velocityMPerS;

    /// <summary>Work: W = F × d for force parallel to displacement.</summary>
    public static double WorkJoules(double forceNewtons, double displacementM)
        => forceNewtons * displacementM;

    /// <summary>Density: ρ = m / V.</summary>
    public static double DensityKgPerM3(double massKg, double volumeM3)
        => massKg / volumeM3;

    /// <summary>Pressure: p = F / A.</summary>
    public static double PressurePa(double forceNewtons, double areaM2)
        => forceNewtons / areaM2;
}
