namespace TheSingularityWorkshop.Gui;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Data-driven Workshop Acoustic Studio Lab. Fixture observations are authored examples,
/// not output from the Physics solver; WebPage must not duplicate acoustic calculations.
/// </summary>
public sealed record SpatialAcousticStudioManifest(
    string Id,
    string Name,
    string OwnershipLabel,
    string StatusLabel,
    double WidthMeters,
    double HeightMeters,
    IReadOnlyList<SpatialAcousticStudioZone> Zones,
    IReadOnlyList<SpatialAcousticStudioSource> Sources,
    IReadOnlyList<SpatialAcousticStudioListener> Listeners,
    IReadOnlyList<SpatialAcousticStudioFixtureObservation> FixtureObservations)
{
    public static SpatialAcousticStudioManifest CreateDefault() => new(
        "workshop.acoustic-studio",
        "WORKSHOP ACOUSTIC STUDIO LAB",
        "Owned by The Singularity Workshop",
        "PROTOTYPE // FIXTURE OBSERVATIONS",
        12,
        8,
        [
            new("live-room", "LIVE ROOM", 0.5, 0.5, 6.5, 5.5),
            new("vocal-booth", "VOCAL BOOTH", 7.5, 0.5, 4, 2.5),
            new("control-room", "CONTROL ROOM", 7.5, 3.5, 4, 4)
        ],
        [
            new("source-vocal", "Vocal Source", new(3, 2.5, 1.6), "Microphone position in the live room."),
            new("source-monitor", "Studio Monitor", new(8.5, 5.2, 1.2), "Near-field monitor in the control room."),
            new("source-instrument", "Instrument Amp", new(5.5, 4.5, 0.8), "Instrument amplifier in the live room.")
        ],
        [
            new("listener-control", "Control Listener", new(9.5, 5.5, 1.6), "Primary listening position."),
            new("listener-booth", "Booth Listener", new(9.2, 1.5, 1.6), "Vocal booth monitoring position.")
        ],
        [
            new("source-vocal", "listener-control", 7.12, 20.8, "Toward control room", -16.0),
            new("source-monitor", "listener-control", 1.41, 4.1, "Near-field / slightly left", -3.0),
            new("source-instrument", "listener-booth", 5.91, 17.2, "Toward live room", -14.0)
        ]);

    public SpatialAcousticStudioSource ResolveSource(string id)
        => Sources.Where(x => string.Equals(x.Id, id, StringComparison.Ordinal)).Select(x => (SpatialAcousticStudioSource?)x).FirstOrDefault()
           ?? throw new KeyNotFoundException($"Unknown acoustic studio source '{id}'.");

    public SpatialAcousticStudioListener ResolveListener(string id)
        => Listeners.Where(x => string.Equals(x.Id, id, StringComparison.Ordinal)).Select(x => (SpatialAcousticStudioListener?)x).FirstOrDefault()
           ?? throw new KeyNotFoundException($"Unknown acoustic studio listener '{id}'.");

    public SpatialAcousticStudioFixtureObservation? FindFixtureObservation(string sourceId, string listenerId)
        => FixtureObservations.FirstOrDefault(x => x.SourceId == sourceId && x.ListenerId == listenerId);
}

public readonly record struct SpatialAcousticStudioPoint(double X, double Y, double Z);
public readonly record struct SpatialAcousticStudioZone(string Id, string Name, double X, double Y, double Width, double Height);
public readonly record struct SpatialAcousticStudioSource(string Id, string Name, SpatialAcousticStudioPoint PositionMeters, string Description);
public readonly record struct SpatialAcousticStudioListener(string Id, string Name, SpatialAcousticStudioPoint PositionMeters, string Description);

/// <summary>Clearly illustrative values for UI wiring; never label these as Physics output.</summary>
public readonly record struct SpatialAcousticStudioFixtureObservation(
    string SourceId,
    string ListenerId,
    double DistanceMeters,
    double PropagationDelayMilliseconds,
    string DirectionDescription,
    double RelativeLevelDb);
