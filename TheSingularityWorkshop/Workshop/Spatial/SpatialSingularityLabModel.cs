using System;
using System.Collections.Generic;
using System.Linq;
using TheSingularityWorkshop.FSM_API;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Spatial state for the Singularity Laboratory entrance and elevator experience.
/// FSM_API owns the visitor progression while the model remains the semantic context
/// consumed by the Workshop GUI builder.
/// </summary>
public sealed class SpatialSingularityLabModel : IStateContext, IDisposable
{
    private readonly FSMHandle _handle;
    private readonly string _processingGroup;
    private SpatialSingularityLabCommand _pendingCommand;
    private bool _disposed;

    public SpatialSingularityLabModel()
    {
        _processingGroup = $"SpatialSingularityLab:{Guid.NewGuid():N}";
        FSM_API.Create.CreateProcessingGroup(_processingGroup);

        var builder = FSM_API.Create.CreateFiniteStateMachine(
            "SpatialSingularityLab",
            processRate: -1,
            processingGroup: _processingGroup);

        foreach (var stage in Enum.GetValues<SpatialSingularityLabStage>())
        {
            var capturedStage = stage;
            builder.State(
                capturedStage.ToString(),
                onEnter: _ => EnterStage(capturedStage),
                onUpdate: _ => { },
                onExit: _ => { });
        }

        builder
            .WithInitialState(SpatialSingularityLabStage.ApproachSecurity.ToString())
            .Transition(
                SpatialSingularityLabStage.ApproachSecurity.ToString(),
                SpatialSingularityLabStage.SecurityQueue.ToString(),
                _ => _pendingCommand == SpatialSingularityLabCommand.EnterSecurityLine)
            .Transition(
                SpatialSingularityLabStage.SecurityQueue.ToString(),
                SpatialSingularityLabStage.SecurityScreening.ToString(),
                _ => _pendingCommand == SpatialSingularityLabCommand.PlaceBelongings)
            .Transition(
                SpatialSingularityLabStage.SecurityScreening.ToString(),
                SpatialSingularityLabStage.SecurityComplete.ToString(),
                _ => _pendingCommand == SpatialSingularityLabCommand.CompleteScreening)
            .Transition(
                SpatialSingularityLabStage.SecurityComplete.ToString(),
                SpatialSingularityLabStage.ElevatorElevation.ToString(),
                _ => _pendingCommand == SpatialSingularityLabCommand.EnterElevator)
            .Transition(
                SpatialSingularityLabStage.ElevatorElevation.ToString(),
                SpatialSingularityLabStage.Fingerprint.ToString(),
                _ => _pendingCommand == SpatialSingularityLabCommand.ScanId)
            .Transition(
                SpatialSingularityLabStage.Fingerprint.ToString(),
                SpatialSingularityLabStage.RetinalScan.ToString(),
                _ => _pendingCommand == SpatialSingularityLabCommand.ScanFingerprint)
            .Transition(
                SpatialSingularityLabStage.RetinalScan.ToString(),
                SpatialSingularityLabStage.FloorSelection.ToString(),
                _ => _pendingCommand == SpatialSingularityLabCommand.ScanRetina)
            .Transition(
                SpatialSingularityLabStage.FloorSelection.ToString(),
                SpatialSingularityLabStage.FloorChallenge.ToString(),
                _ => _pendingCommand == SpatialSingularityLabCommand.SelectFloor)
            .Transition(
                SpatialSingularityLabStage.FloorChallenge.ToString(),
                SpatialSingularityLabStage.FloorOpen.ToString(),
                _ => _pendingCommand == SpatialSingularityLabCommand.ResolveFloorChallenge &&
                     IsSelectedFloorAuthorized())
            .Transition(
                SpatialSingularityLabStage.FloorChallenge.ToString(),
                SpatialSingularityLabStage.AccessDenied.ToString(),
                _ => _pendingCommand == SpatialSingularityLabCommand.ResolveFloorChallenge &&
                     !IsSelectedFloorAuthorized())
            .Transition(
                SpatialSingularityLabStage.AccessDenied.ToString(),
                SpatialSingularityLabStage.ElevatorElevation.ToString(),
                _ => _pendingCommand == SpatialSingularityLabCommand.ReturnToElevator)
            .Transition(
                SpatialSingularityLabStage.FloorOpen.ToString(),
                SpatialSingularityLabStage.ElevatorElevation.ToString(),
                _ => _pendingCommand == SpatialSingularityLabCommand.ReturnToElevator)
            .BuildDefinition();

        _handle = FSM_API.Create.CreateInstance(
            "SpatialSingularityLab",
            this,
            _processingGroup);

        _handle.Update();
    }

    public string Name { get; set; } = "Singularity Laboratory";

    public bool IsValid => !_disposed;

    public SpatialSingularityLabStage Stage
        => Enum.Parse<SpatialSingularityLabStage>(_handle.CurrentState);

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
        ObjectDisposedException.ThrowIf(_disposed, this);

        SelectedFloor = null;
        AccessCard = "VISITOR";
        _pendingCommand = SpatialSingularityLabCommand.None;

        if (Stage != SpatialSingularityLabStage.ApproachSecurity)
            _handle.TransitionTo(SpatialSingularityLabStage.ApproachSecurity.ToString());
        else
            EnterStage(SpatialSingularityLabStage.ApproachSecurity);
    }

    public void EnterSecurityLine()
        => Command(SpatialSingularityLabCommand.EnterSecurityLine);

    public void PlaceBelongings()
        => Command(SpatialSingularityLabCommand.PlaceBelongings);

    public void CompleteScreening()
        => Command(SpatialSingularityLabCommand.CompleteScreening);

    public void EnterElevator()
        => Command(SpatialSingularityLabCommand.EnterElevator);

    public void ScanId(string userName = "USER")
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (Stage != SpatialSingularityLabStage.ElevatorElevation)
            return;

        LastMessage = $"Welcome, {userName}. Please touch the terminal for a fingerprint scan.";
        Command(SpatialSingularityLabCommand.ScanId);
    }

    public void ScanFingerprint()
        => Command(SpatialSingularityLabCommand.ScanFingerprint);

    public void ScanRetina()
        => Command(SpatialSingularityLabCommand.ScanRetina);

    public void SelectFloor(int floor)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (Stage != SpatialSingularityLabStage.FloorSelection)
            return;

        if (Floors.All(x => x.Level != floor))
            return;

        SelectedFloor = floor;
        Command(SpatialSingularityLabCommand.SelectFloor);
    }

    public void ResolveFloorChallenge()
        => Command(SpatialSingularityLabCommand.ResolveFloorChallenge);

    public void ReturnToElevator()
        => Command(SpatialSingularityLabCommand.ReturnToElevator);

    public void UpgradeCard(string cardName)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (string.IsNullOrWhiteSpace(cardName))
            return;

        AccessCard = cardName.Trim().ToUpperInvariant();
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        FSM_API.Interaction.DestroyFiniteStateMachine(
            "SpatialSingularityLab",
            _processingGroup);

        _disposed = true;
    }

    private void Command(SpatialSingularityLabCommand command)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (!CanAccept(command))
            return;

        _pendingCommand = command;
        _handle.Update();
        _pendingCommand = SpatialSingularityLabCommand.None;
    }

    private bool CanAccept(SpatialSingularityLabCommand command)
        => command switch
        {
            SpatialSingularityLabCommand.EnterSecurityLine
                => Stage == SpatialSingularityLabStage.ApproachSecurity,
            SpatialSingularityLabCommand.PlaceBelongings
                => Stage == SpatialSingularityLabStage.SecurityQueue,
            SpatialSingularityLabCommand.CompleteScreening
                => Stage == SpatialSingularityLabStage.SecurityScreening,
            SpatialSingularityLabCommand.EnterElevator
                => Stage == SpatialSingularityLabStage.SecurityComplete,
            SpatialSingularityLabCommand.ScanId
                => Stage == SpatialSingularityLabStage.ElevatorElevation,
            SpatialSingularityLabCommand.ScanFingerprint
                => Stage == SpatialSingularityLabStage.Fingerprint,
            SpatialSingularityLabCommand.ScanRetina
                => Stage == SpatialSingularityLabStage.RetinalScan,
            SpatialSingularityLabCommand.SelectFloor
                => Stage == SpatialSingularityLabStage.FloorSelection && SelectedFloor.HasValue,
            SpatialSingularityLabCommand.ResolveFloorChallenge
                => Stage == SpatialSingularityLabStage.FloorChallenge && SelectedFloor.HasValue,
            SpatialSingularityLabCommand.ReturnToElevator
                => Stage is SpatialSingularityLabStage.AccessDenied or SpatialSingularityLabStage.FloorOpen,
            _ => false
        };

    private bool IsSelectedFloorAuthorized()
        => SelectedFloor is int floor &&
           Floors.FirstOrDefault(x => x.Level == floor) is { Authorized: true };

    private void EnterStage(SpatialSingularityLabStage stage)
    {
        _pendingCommand = SpatialSingularityLabCommand.None;

        LastMessage = stage switch
        {
            SpatialSingularityLabStage.ApproachSecurity
                => "Proceed to security. Click the security line to begin screening.",
            SpatialSingularityLabStage.SecurityQueue
                => "Please place your belongings in the bucket, then walk through.",
            SpatialSingularityLabStage.SecurityScreening
                => "Please walk through the screening machine.",
            SpatialSingularityLabStage.SecurityComplete
                => "All clear. Retrieve your belongings and proceed to the elevator.",
            SpatialSingularityLabStage.ElevatorElevation
                => "Scan your ID card at the security console.",
            SpatialSingularityLabStage.Fingerprint
                => LastMessage,
            SpatialSingularityLabStage.RetinalScan
                => "Fingerprint accepted. Please complete the retinal scan.",
            SpatialSingularityLabStage.FloorSelection
                => "Identity confirmed. Select an authorized destination.",
            SpatialSingularityLabStage.FloorChallenge
                => SelectedFloor is int floor && Floors.FirstOrDefault(x => x.Level == floor) is { } destination
                    ? destination.Authorized
                        ? $"Floor {floor} selected. Proceeding to {destination.Name}."
                        : $"Clearance required for Floor {floor}. Proceed to the guard post."
                    : "Destination not selected.",
            SpatialSingularityLabStage.FloorOpen
                => SelectedFloor is int openFloor && Floors.FirstOrDefault(x => x.Level == openFloor) is { } openDestination
                    ? $"Welcome. Floor {openDestination.Level}: {openDestination.Name}."
                    : "Floor access granted.",
            SpatialSingularityLabStage.AccessDenied
                => "Sorry sir, you are not cleared for this floor. I'm going to have to ask you to leave.",
            _ => LastMessage
        };
    }

    private enum SpatialSingularityLabCommand
    {
        None,
        EnterSecurityLine,
        PlaceBelongings,
        CompleteScreening,
        EnterElevator,
        ScanId,
        ScanFingerprint,
        ScanRetina,
        SelectFloor,
        ResolveFloorChallenge,
        ReturnToElevator
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

public readonly record struct SpatialLabFloorAccess(
    int Level,
    string Name,
    bool Authorized,
    string Description);
