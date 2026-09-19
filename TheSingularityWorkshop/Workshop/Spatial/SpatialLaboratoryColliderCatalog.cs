namespace TheSingularityWorkshop.Gui;

using System.Collections.Generic;
using System.Linq;

/// <summary>Particle-collider demonstrations that scale from tabletop concepts to hadron-scale facilities.</summary>
public sealed record SpatialLaboratoryColliderCatalog(IReadOnlyList<SpatialColliderConfiguration> Configurations)
{
    public static SpatialLaboratoryColliderCatalog CreateDefault()
        => new(
        [
            new("bench", "BENCH COLLIDER", "Small charged-particle demonstration.", 0.5, "charge,field,collision"),
            new("room", "ROOM-SCALE COLLIDER", "Extended beam path with larger instrumentation.", 8, "charge,field,collision,detector"),
            new("ring", "RING COLLIDER", "Closed-loop beam experiment.", 100, "beam,current,field,collision"),
            new("hadron", "HADRON COLLIDER", "Large-scale hadron collision demonstration.", 1000, "hadron,field,collision,detector"),
            new("super-ring", "EXTRA-LARGE COLLIDER", "Story-scale collider for extreme experimental scenarios.", 10000, "beam,field,collision,energy")
        ]);
    
    public SpatialColliderConfiguration? Find(string id)
        => Configurations.FirstOrDefault(x => x.Id == id);
}

public readonly record struct SpatialColliderConfiguration(
    string Id,
    string Name,
    string Description,
    double ScaleMeters,
    string MeasurementDomains);
