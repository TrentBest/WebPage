namespace TheSingularityWorkshop.Workshop;

/// <summary>
/// The four primary public surfaces of the Workshop.
/// The order is intentional: understand, experience, create, publish.
/// Other routes may expose instruments or destinations without becoming primary surfaces.
/// </summary>
public enum WorkshopSurface
{
    Understand = 0,
    Experiences = 1,
    Create = 2,
    Publish = 3
}

/// <summary>Describes a primary Workshop surface without coupling the UI to routing details.</summary>
public readonly record struct WorkshopSurfaceDescriptor(
    WorkshopSurface Surface,
    string Title,
    string Purpose)
{
    public static IReadOnlyList<WorkshopSurfaceDescriptor> PublicSurfaces { get; } =
        new[]
        {
            new WorkshopSurfaceDescriptor(WorkshopSurface.Understand, "Understand", "See the Workshop explain itself through executable evidence."),
            new WorkshopSurfaceDescriptor(WorkshopSurface.Experiences, "Experiences", "Enter working environments built from Workshop capabilities."),
            new WorkshopSurfaceDescriptor(WorkshopSurface.Create, "Create", "Compose something of your own from the Workshop vocabulary."),
            new WorkshopSurfaceDescriptor(WorkshopSurface.Publish, "Publish", "Prepare reusable Workshop creations for sharing and distribution.")
        };
}
