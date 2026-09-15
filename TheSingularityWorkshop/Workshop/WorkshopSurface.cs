namespace TheSingularityWorkshop.Workshop;

/// <summary>
/// The four public entry surfaces of the Workshop experience.
/// The order is intentional: understand, experience, create, publish.
/// </summary>
public enum WorkshopSurface
{
    WhatIsThis = 0,
    Experiences = 1,
    Create = 2,
    Shop = 3
}

/// <summary>Describes a public Workshop surface without coupling the UI to routing details.</summary>
public readonly record struct WorkshopSurfaceDescriptor(
    WorkshopSurface Surface,
    string Title,
    string Purpose)
{
    public static IReadOnlyList<WorkshopSurfaceDescriptor> PublicSurfaces { get; } =
        new[]
        {
            new WorkshopSurfaceDescriptor(WorkshopSurface.WhatIsThis, "What Is This?", "Understand the Hub, Workshop, and composition model."),
            new WorkshopSurfaceDescriptor(WorkshopSurface.Experiences, "Experiences", "Explore working things built with the Workshop."),
            new WorkshopSurfaceDescriptor(WorkshopSurface.Create, "Create", "Compose something of your own."),
            new WorkshopSurfaceDescriptor(WorkshopSurface.Shop, "Singularity Shop", "Publish and discover reusable Workshop creations.")
        };
}
