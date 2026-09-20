namespace TheSingularityWorkshop.Gui;

using SingularityHub.Abstractions;

/// <summary>
/// Reusable physical access-control substrate. A card reader is a capability,
/// not laboratory-specific UI, and can therefore be embedded in other Experiences.
/// </summary>
public sealed class SpatialLaboratoryAccessModel
{
    public SpatialLaboratoryAccessModel()
    {
        Badge = new SpatialSecurityBadge("VISITOR", SpatialSecurityClearance.Visitor);
        CardReader = new SpatialCardReader(
            "lab-main-card-reader",
            "SINGULARITY LABORATORY MAIN ENTRY",
            new OntologySignature(1, 4, 2, 7, 3, 6, 5, 8, 101),
            SpatialSecurityClearance.Employee);
        Intercom = new SpatialIntercom(
            "lab-main-intercom",
            "SECURITY INTERCOM",
            new OntologySignature(1, 4, 2, 7, 3, 6, 5, 8, 102));
    }

    public SpatialSecurityBadge Badge { get; private set; }
    public SpatialCardReader CardReader { get; }
    public SpatialIntercom Intercom { get; }
    public bool VoiceAccessGranted { get; private set; }
    public bool DoorOpen { get; private set; }
    public string LastMessage { get; private set; } = "Please use the intercom to the left.";

    public void CallIntercom()
    {
        VoiceAccessGranted = true;
        LastMessage = "VOICE ACCESS GRANTED. Please use your security badge.";
    }

    public bool TryBadge()
    {
        if (!VoiceAccessGranted)
        {
            LastMessage = "BADGE ACCESS HELD. Please use the intercom to the left.";
            return false;
        }

        if (!CardReader.Accepts(Badge))
        {
            LastMessage = $"ACCESS DENIED. REQUIRED CLEARANCE: {CardReader.RequiredClearance}.";
            return false;
        }

        DoorOpen = true;
        LastMessage = "ACCESS GRANTED. Welcome to the Singularity Laboratory.";
        return true;
    }

    public void UpgradeBadge(string badgeId, SpatialSecurityClearance clearance)
    {
        Badge = new SpatialSecurityBadge(badgeId, clearance);
        LastMessage = $"BADGE UPDATED // {badgeId} // {clearance}";
    }

    public void Reset()
    {
        VoiceAccessGranted = false;
        DoorOpen = false;
        LastMessage = "Please use the intercom to the left.";
    }
}

public readonly record struct SpatialSecurityBadge(
    string Id,
    SpatialSecurityClearance Clearance);

public readonly record struct SpatialCardReader(
    string Id,
    string Name,
    OntologySignature Ontology,
    SpatialSecurityClearance RequiredClearance)
{
    public bool Accepts(SpatialSecurityBadge badge)
        => badge.Clearance >= RequiredClearance;
}

public readonly record struct SpatialIntercom(
    string Id,
    string Name,
    OntologySignature Ontology);

public enum SpatialSecurityClearance
{
    Visitor,
    Employee,
    Research,
    Administrator,
    FacilityHead
}
