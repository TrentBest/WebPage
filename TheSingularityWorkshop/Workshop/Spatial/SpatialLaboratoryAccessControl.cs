namespace TheSingularityWorkshop.Gui;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Diegetic multi-factor access sequence for the Singularity Laboratory.
/// Each action is represented as a physical interaction rather than a page-level dialog.
/// </summary>
public sealed class SpatialLaboratoryAccessControl
{
    private readonly Dictionary<string, int> _clearanceByFloor;

    public SpatialLaboratoryAccessControl()
    {
        _clearanceByFloor = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            ["security"] = 0,
            ["director"] = 1,
            ["gravity"] = 2,
            ["energy"] = 2,
            ["materials"] = 2,
            ["atomics"] = 3,
            ["quantum"] = 4,
            ["plasma"] = 4,
            ["fluids"] = 3,
            ["cosmic"] = 5,
            ["simulation"] = 5
        };

        Guards =
        [
            new("guard-left", "Guard #1", "LEFT ELEVATOR POST"),
            new("guard-right", "Guard #2", "RIGHT ELEVATOR POST"),
            new("guard-floor-a", "Guard #3", "LOWER FLOOR CHECKPOINT"),
            new("guard-floor-b", "Guard #4", "LOWER FLOOR CHECKPOINT")
        ];
    }

    public SpatialLaboratoryAccessStage Stage { get; private set; } = SpatialLaboratoryAccessStage.BadgeRequired;
    public int VisitorClearance { get; private set; }
    public IReadOnlyList<SpatialLaboratoryAccessGuard> Guards { get; }

    public bool ScanBadge()
    {
        if (Stage != SpatialLaboratoryAccessStage.BadgeRequired)
            return false;

        Stage = SpatialLaboratoryAccessStage.FingerprintRequired;
        return true;
    }

    public bool ScanFingerprint()
    {
        if (Stage != SpatialLaboratoryAccessStage.FingerprintRequired)
            return false;

        Stage = SpatialLaboratoryAccessStage.RetinalScanRequired;
        return true;
    }

    public bool ScanRetina()
    {
        if (Stage != SpatialLaboratoryAccessStage.RetinalScanRequired)
            return false;

        Stage = SpatialLaboratoryAccessStage.Authorized;
        VisitorClearance = 1;
        return true;
    }

    public SpatialAccessDecision RequestFloor(string floorId)
    {
        if (Stage != SpatialLaboratoryAccessStage.Authorized)
            return SpatialAccessDecision.Denied("Please complete the laboratory identity sequence first.");

        if (!_clearanceByFloor.TryGetValue(floorId, out var required))
            return SpatialAccessDecision.Denied("That floor does not exist.");

        if (VisitorClearance < required)
        {
            Stage = SpatialLaboratoryAccessStage.Challenged;
            return SpatialAccessDecision.Denied("Sorry sir, you are not cleared for this floor. I'm going to have to ask you to leave.");
        }

        Stage = SpatialLaboratoryAccessStage.Authorized;
        return SpatialAccessDecision.Allowed(floorId);
    }

    public void ReturnToElevator()
    {
        Stage = SpatialLaboratoryAccessStage.Authorized;
    }
}

public enum SpatialLaboratoryAccessStage
{
    BadgeRequired,
    FingerprintRequired,
    RetinalScanRequired,
    Authorized,
    Challenged
}

public readonly record struct SpatialLaboratoryAccessGuard(string Id, string Name, string Post);

public readonly record struct SpatialAccessDecision(bool Allowed, string Message, string? FloorId)
{
    public static SpatialAccessDecision Allowed(string floorId)
        => new(true, $"Access granted. Proceed to {floorId}.", floorId);

    public static SpatialAccessDecision Denied(string message)
        => new(false, message, null);
}
