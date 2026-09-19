namespace TheSingularityWorkshop.Gui;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// The laboratory's explicit third-dimensional instrument. It is intentionally a lab experiment:
/// the renderer is measured here before the technique becomes a general campus primitive.
/// </summary>
public sealed record SpatialLaboratory3DManifest(
    string Id,
    string Name,
    string Description,
    IReadOnlyList<SpatialLaboratory3DExperiment> Experiments,
    SpatialLaboratoryRenderPath RenderPath)
{
    public static SpatialLaboratory3DManifest CreateDefault()
        => new(
            "lab-3d",
            "THIRD DIMENSION LAB",
            "Enter the third dimension, inspect the projected geometry, and measure the line-rendering path before escalating to browser GPU drawing.",
            [
                new("tower", "3D TEST TOWER", 3, 1_000, "Projection and line density."),
                new("gravity-bodies", "3D GRAVITY BODIES", 3, 5_000, "Orbital paths and field volumes."),
                new("launcher-track", "3D LAUNCHER TRACK", 3, 12_000, "A movable launcher traverses a spatial track and fires measured projectiles through a three-dimensional volume."),
                new("laboratory", "3D LAB MODEL", 3, 20_000, "Building-scale geometry and camera movement.")
            ],
            SpatialLaboratoryRenderPath.SpatialLineRendererThenGpu);

    public SpatialLaboratory3DExperiment? Find(string id)
        => Experiments.Count == 0 ? null : Experiments.FirstOrDefault(x => x.Id == id);
}

public readonly record struct SpatialLaboratory3DExperiment(
    string Id,
    string Name,
    int Dimensions,
    int ExpectedLineCount,
    string Purpose);

public enum SpatialLaboratoryRenderPath
{
    SpatialLineRendererThenGpu
}
