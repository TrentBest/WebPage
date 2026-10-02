using System;
using System.Collections.Generic;
using System.Linq;
using HubBundle = TheSingularityWorkshop.SingularityHub.IMicroBundle;
using TheSingularityWorkshop.SingularityHub;

namespace TheSingularityWorkshop.Workshop.Chemistry;

/// <summary>
/// Resolves elemental consequences for material-bearing MicroBundles.
/// It does not know whether the source represents a weapon, armor, vehicle,
/// building, or something fictional; it evaluates the material contract only.
/// </summary>
public sealed class ElementalArbitrator
{
    public IReadOnlyList<MaterialResolution> Evaluate(IEnumerable<HubBundle> installedBundles)
    {
        ArgumentNullException.ThrowIfNull(installedBundles);

        return installedBundles
            .OfType<IElementalMaterialSource>()
            .SelectMany(source => source.Materials)
            .Select(Resolve)
            .ToArray();
    }

    private static MaterialResolution Resolve(MaterialComposition material)
    {
        var elements = material.Elements
            .OrderByDescending(x => x.Fraction)
            // Atomic number is not a unique identity for fictional elements:
            // multiple fictional elements may intentionally have an unassigned Z=0.
            .Select(x => x.Element)
            .DistinctBy(x => x.Symbol)
            .ToArray();

        return new MaterialResolution(
            material.Id,
            material.Name,
            material.Application,
            elements,
            InferPhysics(material.Application, elements));
    }

    /// <summary>
    /// Produces normalized default behavior (0..1), not laboratory measurements.
    /// A later data-domain bundle can replace these heuristics with authoritative
    /// material-property data without changing the arbitration contract.
    /// </summary>
    private static MaterialPhysicsDefaults InferPhysics(
        MaterialApplication application,
        IReadOnlyList<Atom> elements)
    {
        var hasMetal = elements.Any(x => x.Category.Contains("Metal", StringComparison.OrdinalIgnoreCase));
        var hasCarbon = elements.Any(x => x.Symbol.Equals("C", StringComparison.Ordinal));
        var hasSilicon = elements.Any(x => x.Symbol.Equals("Si", StringComparison.Ordinal));

        return application switch
        {
            MaterialApplication.Structural => new(0.75, hasMetal ? 0.85 : 0.65, 0.20, hasMetal ? 0.70 : 0.35, hasMetal ? 0.80 : 0.20),
            MaterialApplication.Grip => new(0.45, 0.40, 0.70, 0.25, 0.15),
            MaterialApplication.Flexible => new(0.35, hasCarbon ? 0.55 : 0.30, 0.85, 0.20, 0.10),
            MaterialApplication.Armor => new(0.70, hasCarbon || hasSilicon ? 0.80 : 0.60, 0.45, 0.25, 0.20),
            MaterialApplication.Decorative => new(0.40, 0.35, 0.35, 0.30, 0.25),
            MaterialApplication.Conductive => new(0.55, 0.45, 0.25, 0.65, hasMetal ? 0.95 : 0.55),
            MaterialApplication.Insulating => new(0.30, 0.35, 0.45, 0.10, 0.05),
            _ => throw new ArgumentOutOfRangeException(nameof(application))
        };
    }
}
