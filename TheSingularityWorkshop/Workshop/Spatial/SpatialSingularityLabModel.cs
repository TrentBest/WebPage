namespace TheSingularityWorkshop.Gui;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Spatial state for the Singularity Laboratory entrance and elevator experience.
/// The building remains a Dragon-Warrior-style top-down world until an interaction
/// deliberately changes the camera into an elevation view.
/// </summary>
public sealed class SpatialSingularityLabModel
{
    public SpatialSingularityLabModel()
    {
        Reset();
    }

    public SpatialSingularityLabStage Stage { get; private set; }
    public int? SelectedFloor { get; private set; }
    public string AccessCard { get; private set; } = "VISITOR";
    public string LastMessage { get; private set; } = "Proceed to security.";
    public IReadOnlyList<SpatialLabGuard> Guards { get; } =
    [
        new("guard-01", "Guard 01", "ELEVATOR LEFT"),
        new("guard-02", "Guard 02", "ELEVATOR RIGHT"),
        new("guard-03", "Guard 03", "SECURITY LEFT"),
        new("guard-04", "Guard 04", "SECURITY RIGHT")
    ];

    public IReadOnlyList<SpatialLabEntranceSpace> EntranceSpaces { get; } =
    [
        new("reception", "RECEPTIONIST DESK", "Reception / visitor processing"),
        new("director", "FACILITY DIRECTOR", "Director's office"),
        new("conference", "CONFERENCE ROOM", "Briefing / consultation"),
        new("security", "SECURITY", "Identity, screening, and controlled access"),
        new("elevator", "ELEVATOR", "Secure vertical circulation")
    ];

    public IReadOnlyList<SpatialLabFloorAccess> Floors { get; } =
    [
        new(1, "PHYSICS // MOTION", true, "Ramp, gravity, acceleration, rolling-body experiments."),
        new(2, "PHYSICS // FIELDS", true, "Gravity, field, energy, and measurement experiments."),
        new(3, "CHEMISTRY // REACTIONS", true, "Controlled reactions, mixtures, and visual demonstrations."),
        new(4, "MATERIALS // STRUCTURE", true, "Stress, materials, structures, and failure studies."),
        new(5, "ADVANCED SCIENCE", false, "Advanced and speculative research."),
        new(6, "EXPERIMENTORIUM", false, "Researcher-authored experiments and open-ended laboratory work.")
    ];

    public bool HasPassedSecurity => Stage >= SpatialSingularityLabStage.SecurityComplete;
    public bool IsElevation => Stage == SpatialSingularityLabStage.ElevatorElevation;
    public bool IsFloorSelected => SelectedFloor.HasValue;

    public void Reset()
    {
        Stage = SpatialSingularityLabStage.ApproachSecurity;
        SelectedFloor = null;
        AccessCard = "VISITOR";
        LastMessage = "Proceed to security. Click the security line to begin screening.";
    }

    public void EnterSecurityLine()
    {
        if (Stage != SpatialSingularityLabStage.ApproachSecurity) return;
        Stage = SpatialSingularityLabStage.SecurityQueue;
        LastMessage = "Please place your belongings in the bucket, then walk through.";
    }

    public void PlaceBelongings()
    {
        if (Stage != SpatialSingularityLabStage.SecurityQueue) return;
        Stage = SpatialSingularityLabStage.SecurityScreening;
        LastMessage = "Please walk through the screening machine.";
    }

    public void CompleteScreening()
    {
        if (Stage != SpatialSingularityLabStage.SecurityScreening) return;
        Stage = SpatialSingularityLabStage.SecurityComplete;
        LastMessage = "All clear. Retrieve your belongings and proceed to the elevator.";
    }

    public void EnterElevator()
    {
        if (!HasPassedSecurity) return;
        Stage = SpatialSingularityLabStage.ElevatorElevation;
        LastMessage = "Scan your ID card at the security console.";
    }

    public void ScanId()
    {
        if (Stage != SpatialSingularityLabStage.ElevatorElevation) return;
        Stage = SpatialSingularityLabStage.Fingerprint;
        LastMessage = $"Welcome, Trent. Please touch the terminal for a fingerprint scan.";
    }

    public void ScanFingerprint()
    {
        if (Stage != SpatialSingularityLabStage.Fingerprint) return;
        Stage = SpatialSingularityLabStage.RetinalScan;
        LastMessage = "Fingerprint accepted. Please complete the retinal scan.";
    }

    public void ScanRetina()
    {
        if (Stage != SpatialSingularityLabStage.RetinalScan) return;
        Stage = SpatialSingularityLabStage.FloorSelection;
        LastMessage = "Identity confirmed. Select an authorized destination.";
    }

    public void SelectFloor(int floor)
    {
        if (Stage != SpatialSingularityLabStage.FloorSelection) return;
        var destination = Floors.FirstOrDefault(x => x.Level == floor);
        if (destination == default) return;

        SelectedFloor = floor;
        Stage = SpatialSingularityLabStage.FloorChallenge;
        LastMessage = destination.Authorized
            ? $"Floor {floor} selected. Proceeding to {destination.Name}."
            : $"Clearance required for Floor {floor}. Proceed to the guard post.";
    }

    public void ResolveFloorChallenge()
    {
        if (Stage != SpatialSingularityLabStage.FloorChallenge || !SelectedFloor.HasValue) return;

        var destination = Floors.First(x => x.Level == SelectedFloor.Value);
        if (destination.Authorized)
        {
            Stage = SpatialSingularityLabStage.FloorOpen;
            LastMessage = $"Welcome. Floor {destination.Level}: {destination.Name}.";
        }
        else
        {
            Stage = SpatialSingularityLabStage.AccessDenied;
            LastMessage = "Sorry sir, you are not cleared for this floor. I'm going to have to ask you to leave.";
        }
    }

    public void ReturnToElevator()
    {
        if (Stage != SpatialSingularityLabStage.AccessDenied && Stage != SpatialSingularityLabStage.FloorOpen) return;
        Stage = SpatialSingularityLabStage.ElevatorElevation;
        LastMessage = "Please select another destination.";
        SelectedFloor = null;
    }

    public void UpgradeCard(string cardName)
    {
        if (string.IsNullOrWhiteSpace(cardName)) return;
        AccessCard = cardName.Trim().ToUpperInvariant();
    }
}

public enum SpatialSingularityLabStage
{
    ApproachSecurity,
    SecurityQueue,
    SecurityScreening,
    SecurityComplete,
    ElevatorElevation,
    Fingerprint,
    RetinalScan,
    FloorSelection,
    FloorChallenge,
    FloorOpen,
    AccessDenied
}

public readonly record struct SpatialLabGuard(string Id, string Name, string Post);
public readonly record struct SpatialLabEntranceSpace(string Id, string Name, string Purpose);
public readonly record struct SpatialLabFloorAccess(int Level, string Name, bool Authorized, string Description);
