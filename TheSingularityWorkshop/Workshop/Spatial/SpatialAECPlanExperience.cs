using System;
using System.Collections.Generic;
using System.Linq;

namespace TheSingularityWorkshop.Gui;

/// <summary>Describes one semantic room or zone in a floor plan.</summary>
public sealed record SpatialAECPlanRoom(
    string Id,
    string Name,
    double X,
    double Y,
    double Width,
    double Depth,
    string Function,
    bool IsInteractable = true);

/// <summary>Describes an interaction that becomes active as the avatar approaches it.</summary>
public sealed record SpatialAECPlanInteractable(
    string Id,
    string Name,
    string RoomId,
    string Prompt,
    string Capability,
    string Detail,
    double X,
    double Y);

/// <summary>A semantic floor plan that can be rendered by any GUI builder.</summary>
public sealed record SpatialAECPlanView(
    int Floor,
    string Name,
    double Width,
    double Depth,
    IReadOnlyList<SpatialAECPlanRoom> Rooms,
    IReadOnlyList<SpatialAECPlanInteractable> Interactables);

/// <summary>
/// The default walkable experience for a selected building type.
/// Plan geometry, capabilities and interaction cues remain data so a renderer can
/// breathe/highlight them without owning the building semantics.
/// </summary>
public sealed record SpatialAECPlanExperience(
    SpatialAECBuildingType BuildingType,
    SpatialBuildingSpecification Specification,
    SpatialAECCodeProfile CodeProfile,
    IReadOnlyList<SpatialAECPlanView> Floors)
{
    /// <summary>Returns the floor plan currently selected by the user.</summary>
    public SpatialAECPlanView GetFloor(int floor)
        => Floors.FirstOrDefault(x => x.Floor == floor)
           ?? throw new KeyNotFoundException($"Floor {floor} is not present in this experience.");

    /// <summary>Returns all interactables across the selected building.</summary>
    public IReadOnlyList<SpatialAECPlanInteractable> Interactables
        => Floors.SelectMany(x => x.Interactables).ToArray();
}

/// <summary>
/// Creates a conservative default plan from the selected catalog building.
/// Visual detail can grow as the user explores without changing the semantic model.
/// </summary>
public static class SpatialAECPlanExperienceFactory
{
    /// <summary>Creates the pre-established starting experience for a catalog building type.</summary>
    public static SpatialAECPlanExperience Create(SpatialAECBuildingType buildingType)
    {
        ArgumentNullException.ThrowIfNull(buildingType);

        var specification = SpatialBuildingSpecificationCatalog.Resolve(buildingType.Id);
        var code = SpatialAECCodeCatalog.For(specification);

        var floorCount = Math.Clamp(
            specification.MinimumFloors,
            1,
            Math.Min(specification.MaximumFloors, 12));

        var floors = Enumerable.Range(1, floorCount)
            .Select(floor => CreateFloor(buildingType, specification, code, floor))
            .ToArray();

        return new SpatialAECPlanExperience(buildingType, specification, code, floors);
    }

    private static SpatialAECPlanView CreateFloor(
        SpatialAECBuildingType buildingType,
        SpatialBuildingSpecification specification,
        SpatialAECCodeProfile code,
        int floor)
    {
        var width = specification.MinimumWidth;
        var depth = specification.MinimumDepth;

        var rooms = new List<SpatialAECPlanRoom>
        {
            new("arrival", "Arrival / Reception", 0, 0, width * .20, depth * .25, "controlled arrival"),
            new("circulation", "Protected Core", width * .20, 0, width * .15, depth, "egress / vertical circulation"),
            new("program", buildingType.Name, width * .35, 0, width * .65, depth * .60, specification.Description),
            new("systems", "Building Systems", width * .35, depth * .60, width * .65, depth * .40, "mechanical / electrical / service"),
        };

        var interactables = new List<SpatialAECPlanInteractable>
        {
            new("arrival-door", "Main Entry", "arrival",
                "APPROACH: ENTRY SYSTEM",
                "Secure arrival, visitor control and building access.",
                $"Default egress and access basis: {code.Name}.",
                width * .10, depth * .12),

            new("protected-core", "Protected Core", "circulation",
                "APPROACH: LIFE SAFETY",
                "Explore protected circulation, stairs, elevator and emergency egress.",
                $"Structural basis: {code.StructuralSystem}.",
                width * .275, depth * .50),

            new("program-capability", "Program Capability", "program",
                $"APPROACH: {buildingType.Name.ToUpperInvariant()}",
                $"This building is configured for {specification.Purpose}.",
                $"Code basis: {code.Basis}.",
                width * .68, depth * .28),

            new("systems-wall", "Building Systems", "systems",
                "APPROACH: BUILDING SYSTEMS",
                "Reveal the services that make the building operational.",
                $"Floor-to-floor basis: {code.DefaultFloorToFloor} units; live-load basis: {code.DefaultLiveLoad}.",
                width * .68, depth * .78)
        };

        return new SpatialAECPlanView(
            floor,
            $"LEVEL {floor:00} // {buildingType.Name}",
            width,
            depth,
            rooms,
            interactables);
    }
}
