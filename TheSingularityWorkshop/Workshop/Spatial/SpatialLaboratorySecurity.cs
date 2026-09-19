namespace TheSingularityWorkshop.Gui;

using System.Collections.Generic;

/// <summary>Diegetic laboratory security and the visitor's first interaction.</summary>
public sealed class SpatialLaboratorySecurity
{
    private SpatialLaboratorySecurity(string scannerId, IReadOnlyList<SpatialLaboratoryGuard> guards)
    {
        ScannerId = scannerId;
        Guards = guards;
    }

    public string ScannerId { get; }
    public IReadOnlyList<SpatialLaboratoryGuard> Guards { get; }
    public bool BadgeAccepted { get; private set; }

    public static SpatialLaboratorySecurity CreateDefault()
        => new(
            "lab-badge-scanner",
            [
                new("guard-1", "Guard #1", "Welcome to the Singularity Lab. Please scan your badge."),
                new("guard-2", "Guard #2", "Badge accepted. The laboratory is expecting you.")
            ]);

    /// <summary>Accepts the visitor badge through the physical scanner interaction.</summary>
    public bool ScanBadge()
    {
        BadgeAccepted = true;
        return true;
    }
}

public readonly record struct SpatialLaboratoryGuard(string Id, string Name, string Greeting);

/// <summary>Administrative services available after the visitor reaches the director's office.</summary>
public sealed record SpatialLaboratoryAdministration(
    string DigitenId,
    string Name,
    IReadOnlyList<SpatialLaboratoryAdministrativeService> Services)
{
    public static SpatialLaboratoryAdministration CreateDefault()
        => new(
            "lab-administrator",
            "LAB ADMINISTRATOR",
            [
                new("guided-tour", "REQUEST A GUIDED TOUR", "The administrator routes the visitor through active floors and instruments."),
                new("visitor-pass", "REQUEST A VISITOR PASS", "Creates a temporary visitor identity and access scope."),
                new("lab-space", "APPLY FOR LAB SPACE", "Submits a research-space request with domain, equipment, and project requirements.")
            ]);
}

public readonly record struct SpatialLaboratoryAdministrativeService(
    string Id,
    string Name,
    string Description);
