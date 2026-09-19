using System;
using System.Collections.Generic;
using System.Linq;

namespace TheSingularityWorkshop.Workshop.Chemistry;

/// <summary>How a material is being applied to a manufactured object.</summary>
public enum MaterialApplication
{
    Structural,
    Grip,
    Flexible,
    Armor,
    Decorative,
    Conductive,
    Insulating
}

/// <summary>Elemental contribution of an atom to a material.</summary>
public readonly record struct ElementalFraction(Atom Element, double Fraction)
{
    public ElementalFraction
    {
        if (Fraction <= 0 || Fraction > 1)
            throw new ArgumentOutOfRangeException(nameof(Fraction), "Elemental fraction must be greater than zero and no greater than one.");
    }
}

/// <summary>
/// Domain-only material description. A MicroBundle may expose these records so
/// the chemistry arbitrator can reason about newly installed content without
/// knowing what the content is pretending to be.
/// </summary>
public sealed record MaterialComposition(
    string Id,
    string Name,
    MaterialApplication Application,
    IReadOnlyList<ElementalFraction> Elements)
{
    public MaterialComposition(string id, string name, MaterialApplication application, params ElementalFraction[] elements)
        : this(id, name, application, (IReadOnlyList<ElementalFraction>)elements)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Material id is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Material name is required.", nameof(name));
        if (elements is null || elements.Length == 0) throw new ArgumentException("At least one elemental contribution is required.", nameof(elements));
        if (elements.Sum(x => x.Fraction) > 1.000001) throw new ArgumentException("Elemental fractions cannot exceed one.", nameof(elements));
    }
}

/// <summary>
/// Capability contract for an installed MicroBundle that can describe materials.
/// The chemistry arbitrator consumes only this contract during arbitration.
/// </summary>
public interface IElementalMaterialSource
{
    IReadOnlyList<MaterialComposition> Materials { get; }
}

/// <summary>Default physical behavior inferred from elemental composition and application.</summary>
public readonly record struct MaterialPhysicsDefaults(
    double DensityKgPerM3,
    double Hardness,
    double Flexibility,
    double ThermalConductivity,
    double ElectricalConductivity);

/// <summary>Resolved material contribution produced by chemistry arbitration.</summary>
public sealed record MaterialResolution(
    string MaterialId,
    string MaterialName,
    MaterialApplication Application,
    IReadOnlyList<Atom> Elements,
    MaterialPhysicsDefaults Physics);
