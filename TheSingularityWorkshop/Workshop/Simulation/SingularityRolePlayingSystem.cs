namespace TheSingularityWorkshop.Gui;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Foundational rules model for MicroBundle-authored simulations.
/// SRPS deliberately separates deterministic rules from presentation and Experience code.
/// </summary>
public sealed record SingularityRolePlayingSystem(
    string Id,
    string Name,
    string Version,
    IReadOnlyList<SrpsAttributeDefinition> Attributes,
    IReadOnlyList<SrpsMaterialDefinition> Materials,
    IReadOnlyList<SrpsPhysicsRuleSet> PhysicsRuleSets)
{
    public static SingularityRolePlayingSystem CreateDefault()
        => new(
            "srps",
            "Singularity Role Playing System",
            "0.1",
            [
                new("mass", "Mass", "kg"),
                new("momentum", "Momentum", "kg·m/s"),
                new("energy", "Energy", "J"),
                new("strength", "Strength", "N"),
                new("durability", "Durability", "J"),
                new("temperature", "Temperature", "K")
            ],
            [
                new("bronze", "Bronze", 8800, 2.8e8),
                new("iron", "Iron", 7874, 2.5e8),
                new("steel", "Steel", 7850, 2.5e8)
            ],
            [
                new("material-impact", "Material Impact", "impact.v1", "Physics LUT"),
                new("projectile-impact", "Projectile Impact", "projectile.v1", "Physics LUT"),
                new("thermal-transfer", "Thermal Transfer", "thermal.v1", "Physics LUT")
            ]);

    public SrpsPhysicsRuleSet ResolveRuleSet(string id)
    {
        foreach (var ruleSet in PhysicsRuleSets)
        {
            if (string.Equals(ruleSet.Id, id, StringComparison.OrdinalIgnoreCase))
                return ruleSet;
        }

        throw new KeyNotFoundException($"No SRPS rule set '{id}' is registered.");
    }
}

/// <summary>Named physical quantity exposed to MicroBundles.</summary>
public readonly record struct SrpsAttributeDefinition(string Id, string Name, string Unit);

/// <summary>Material constants used by the baked simulation pipeline.</summary>
public readonly record struct SrpsMaterialDefinition(string Id, string Name, double DensityKgPerM3, double CharacteristicStrengthPa);

/// <summary>
/// Declares a deterministic physics calculation whose expensive Monte Carlo or numerical
/// baking occurs offline. Runtime MicroBundles consume the resulting LUT rather than guessing.
/// </summary>
public readonly record struct SrpsPhysicsRuleSet(string Id, string Name, string Version, string LookupKind);

/// <summary>Address of a baked lookup table produced from a physics simulation.</summary>
public readonly record struct SrpsPhysicsLutAddress(
    string RuleSetId,
    string Version,
    string TextureId,
    int SampleCount,
    string CoordinateSchema)
{
    public bool IsBaked => SampleCount >= 1_000_000;
}

/// <summary>
/// Runtime lookup contract. The Experience supplies normalized physical coordinates;
/// the baked table supplies the deterministic outcome.
/// </summary>
public interface ISrpsPhysicsLut
{
    SrpsPhysicsLutAddress Address { get; }
    double Sample(ReadOnlySpan<double> coordinates);
}

/// <summary>Deterministic material-vs-material impact result.</summary>
public readonly record struct SrpsImpactResult(
    string AttackerMaterialId,
    string DefenderMaterialId,
    double EnergyTransferJoules,
    double PenetrationDepthMeters,
    double StructuralDamage,
    double AttackerResidualEnergy);

/// <summary>
/// A MicroBundle-facing physics service. No Experience needs to invent material outcomes.
/// </summary>
public interface ISrpsPhysicsEngine
{
    SrpsImpactResult ResolveMaterialImpact(
        string attackerMaterialId,
        string defenderMaterialId,
        double massKg,
        double velocityMetersPerSecond);
}

/// <summary>
/// Registry metadata for a baked physics result. The actual texture can live in an asset
/// bundle or remote content store; this manifest remains small and portable.
/// </summary>
public sealed record SrpsPhysicsBakeManifest(
    string Id,
    string RuleSetId,
    string Version,
    SrpsPhysicsLutAddress Lut,
    string GeneratorDescription,
    string ValidationDescription)
{
    public static SrpsPhysicsBakeManifest BronzeVsIronImpact
        => new(
            "physics.bronze-vs-iron",
            "material-impact",
            "1.0",
            new SrpsPhysicsLutAddress(
                "material-impact",
                "1.0",
                "lut.physics.material-impact.v1",
                1_000_000,
                "mass,velocity,angle,geometry,material-pair"),
            "Offline numerical collision simulation; Monte Carlo sampling is permitted during baking.",
            "Runtime values are interpolation/lookups into the baked result; no authored combat guess is required.");
}
