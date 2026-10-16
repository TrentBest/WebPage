namespace TheSingularityWorkshop.Gui;

using System;
using System.Collections.Generic;

/// <summary>
/// Source-aligned WebPage reference fixture for the draft Gestures starport departure scenario.
/// It does not evaluate live crew, provider, equipment, Physics, or ATC state.
/// </summary>
public sealed record SpatialStarportDepartureExperience(
    IReadOnlyList<SpatialStarportZone> Zones,
    IReadOnlyList<SpatialStarportTaskDefinition> Tasks,
    SpatialStarportDepartureSnapshot ReadinessHold,
    SpatialStarportDepartureSnapshot LandingGearFailure)
{
    public const string ScenarioId = "starport.departure.v1";
    public SpatialStarportDepartureSnapshot GetSnapshot(bool failureInjected)
        => failureInjected ? LandingGearFailure : ReadinessHold;
}

public enum SpatialStarportCrewRole { AllCrew, FlightCrew, GroundCrew, FlightControl }
public enum SpatialStarportTaskStatus { Ready, InProgress, Completed, Failed, Blocked, Waiting }

public sealed record SpatialStarportZone(string Id, string Label, double X, double Y, double Width, double Height);
public sealed record SpatialStarportTaskDefinition(
    string Id, SpatialStarportCrewRole Role, string Action,
    IReadOnlyList<string> Dependencies, IReadOnlyList<string> ProviderAnchorIds);
public sealed record SpatialStarportTaskObservation(string TaskId, SpatialStarportTaskStatus Status, string? Reason);
public sealed record SpatialStarportEvidenceObservation(string Id, string Label, bool Confirmed, string Detail);
public sealed record SpatialStarportDepartureSnapshot(
    string Id, string Title, string Summary, bool FailureInjected,
    IReadOnlyList<SpatialStarportTaskObservation> Observations,
    IReadOnlyList<SpatialStarportEvidenceObservation> Evidence);
