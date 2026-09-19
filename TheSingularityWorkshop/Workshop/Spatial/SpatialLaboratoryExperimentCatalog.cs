namespace TheSingularityWorkshop.Gui;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Inventory-first laboratory preparation. A visitor composes an experiment from physical
/// specimens rather than transcribing a worksheet. The resulting recipe remains semantic
/// and can later be handed to SRPS for execution.
/// </summary>
public sealed record SpatialLaboratoryExperimentCatalog(
    IReadOnlyList<SpatialLaboratoryInventoryItem> Items,
    IReadOnlyList<SpatialLaboratoryExperimentTemplate> Templates)
{
    public static SpatialLaboratoryExperimentCatalog CreateDefault()
        => new(
            SpatialLaboratoryInventoryCatalog.CreateDefault(),
            [
                new("ramp-and-mass", "RAMP + MASS", "Mechanics", ["ramp", "steel-ball", "wood-ball"], "Compare acceleration and momentum across masses and surfaces."),
                new("thermal-flask", "BUNSEN + FLASK", "Thermodynamics", ["bunsen-burner", "thermometer", "flask"], "Observe heat transfer and temperature change."),
                new("hydrogen-balloon", "HYDROGEN BALLOON", "Fluids", ["hydrogen-spigot", "balloon", "scale"], "Explore buoyancy, mass, volume, and lift."),
                new("gravity-bodies", "GRAVITY BODIES", "Gravity", ["gravity-tray", "planet", "moon", "asteroid", "comet"], "Compose bodies and observe orbital and field interactions."),
                new("ballistics", "BALLISTICS BENCH", "Materials", ["ballistic-launcher", "projectile", "target-plate", "chronograph"], "Measure projectile motion and material impact through SRPS.")
            ]);

    public SpatialLaboratoryInventoryItem? FindItem(string id)
        => Items.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase));

    public SpatialLaboratoryExperimentTemplate? FindTemplate(string id)
        => Templates.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase));
}

/// <summary>A physical object, instrument, or service connection available on the lab floor.</summary>
public readonly record struct SpatialLaboratoryInventoryItem(
    string Id,
    string Name,
    string Kind,
    string Color,
    string Description);

public static class SpatialLaboratoryInventoryCatalog
{
    public static IReadOnlyList<SpatialLaboratoryInventoryItem> CreateDefault()
        =>
        [
            new("gravity-tray", "PRIMARY GRAVITY TRAY", "apparatus", "#ffd34d", "The common tray into which gravity-body specimens are dropped."),
            new("ramp", "ADJUSTABLE RAMP", "apparatus", "#00eaff", "Variable incline with a known surface profile."),
            new("steel-ball", "STEEL MASS BALL", "specimen", "#b9c5cc", "Known mass and material specimen."),
            new("wood-ball", "WOOD MASS BALL", "specimen", "#b47b4f", "Low-density comparison specimen."),
            new("bunsen-burner", "BUNSEN BURNER", "energy-source", "#ff6b35", "Controlled thermal energy source."),
            new("hydrogen-spigot", "HYDROGEN SPIGOT", "gas-source", "#ff38d1", "Controlled hydrogen supply for buoyancy experiments."),
            new("balloon", "LARGE BALLOON", "apparatus", "#ff8adf", "Large-volume flexible gas vessel."),
            new("thermometer", "THERMOMETER", "instrument", "#ffffff", "Temperature measurement."),
            new("flask", "LAB FLASK", "apparatus", "#8de8ff", "Heat-transfer vessel."),
            new("scale", "PRECISION SCALE", "instrument", "#d9eef4", "Mass measurement."),
            new("planet", "PLANET SPECIMEN", "celestial", "#4da6ff", "Massive gravity-body specimen."),
            new("moon", "MOON SPECIMEN", "celestial", "#d8d8d8", "Satellite-scale gravity-body specimen."),
            new("asteroid", "ASTEROID SPECIMEN", "celestial", "#9b8269", "Irregular low-mass gravity body."),
            new("comet", "COMET SPECIMEN", "celestial", "#a7d7e8", "Small body with a visible activity trail."),
            new("ballistic-launcher", "BALLISTIC LAUNCHER", "apparatus", "#ff3b30", "Controlled projectile launch apparatus."),
            new("catapult-arm", "CATAPULT ARM", "apparatus", "#ff38d1", "Lever geometry becomes an explicit energy-transfer input."),
            new("launch-stone", "LAUNCH STONE", "specimen", "#a8a8a8", "Mass and geometry become SRPS inputs."),
            new("stone-block", "STONE BLOCK", "structure", "#c7b8a6", "Structural block used to expose impact and collapse."),
            new("wood-block", "WOOD BLOCK", "structure", "#b47b4f", "Structural block used for comparative failure behavior."),
            new("target-wall", "TARGET WALL", "structure", "#d65a3a", "Composable wall made from physical blocks."),
            new("impact-plate", "IMPACT PLATE", "specimen", "#d65a3a", "Instrumented surface for impact measurements."),
            new("adjustable-ramp", "ADJUSTABLE RAMP", "apparatus", "#00eaff", "Three-dimensional ramp with measurable incline."),
            new("track", "3D LAUNCH TRACK", "apparatus", "#00eaff", "Spatial rail around which a launcher carriage can move."),
            new("launcher-carriage", "LAUNCHER CARRIAGE", "apparatus", "#ff3b30", "Moves the launcher to a position in the three-dimensional track."),
            new("trajectory-volume", "TRAJECTORY VOLUME", "instrument", "#ff38d1", "Records the projectile path through 3D space."),
            new("projectile", "PROJECTILE", "specimen", "#f2f2f2", "Mass and geometry become SRPS inputs."),
            new("target-plate", "TARGET PLATE", "specimen", "#d65a3a", "Material target for impact experiments."),
            new("chronograph", "CHRONOGRAPH", "instrument", "#ffd34d", "Measures projectile transit time.")
        ];
}

/// <summary>A reusable experiment recipe that can be prepared by selecting inventory items.</summary>
public readonly record struct SpatialLaboratoryExperimentTemplate(
    string Id,
    string Name,
    string Domain,
    IReadOnlyList<string> RequiredInventoryIds,
    string Purpose);
