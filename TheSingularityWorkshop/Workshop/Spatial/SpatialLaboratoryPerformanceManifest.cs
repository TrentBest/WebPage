namespace TheSingularityWorkshop.Gui;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Defines the laboratory's diegetic performance instrumentation.
/// These are budgets and instrument definitions, not fabricated runtime measurements.
/// A live renderer can later feed the same snapshot contract.
/// </summary>
public sealed record SpatialLaboratoryPerformanceManifest(
    string Id,
    string Name,
    IReadOnlyList<SpatialLaboratoryPerformanceInstrument> Instruments,
    IReadOnlyList<SpatialLaboratoryPerformanceBudget> Budgets)
{
    public static SpatialLaboratoryPerformanceManifest CreateDefault()
        => new(
            "lab-performance",
            "PERFORMANCE OBSERVATORY",
            [
                new("frame", "FRAME PIPELINE", "frame-time", "ms", "Browser frame budget and render cadence."),
                new("geometry", "GEOMETRY", "line-count", "lines", "Source, projected, and rendered geometry."),
                new("payload", "GPU PAYLOAD", "payload-bytes", "bytes", "Bytes prepared for a future GPU manifestation."),
                new("simulation", "SIMULATION", "tick-rate", "ticks/s", "Physics and FSM work performed per second."),
                new("agents", "DIGITENS", "active-agents", "agents", "Robots, drones, and other active spatial actors."),
                new("collider", "COLLIDER", "collision-rate", "events/s", "Collision events processed by the experiment."),
                new("memory", "MEMORY", "working-set", "MB", "Runtime memory attributed to the active experiment.")
            ],
            [
                new("frame", "FRAME TIME", 16.67, "ms", "60 Hz presentation budget."),
                new("lines", "RENDERED LINES", 20_000, "lines", "Initial line-rendering investigation ceiling."),
                new("payload", "GPU PAYLOAD", 4_000_000, "bytes", "Initial browser payload investigation ceiling."),
                new("simulation", "SIMULATION TICKS", 60, "ticks/s", "Target cadence for interactive experiments.")
            ]);

    public SpatialLaboratoryPerformanceInstrument? FindInstrument(string id)
        => Instruments.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase));

    public SpatialLaboratoryPerformanceBudget? FindBudget(string id)
        => Budgets.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase));
}

/// <summary>One named performance signal exposed by the laboratory.</summary>
public readonly record struct SpatialLaboratoryPerformanceInstrument(
    string Id,
    string Name,
    string Signal,
    string Unit,
    string Description);

/// <summary>Explicit performance budget used by a laboratory instrument.</summary>
public readonly record struct SpatialLaboratoryPerformanceBudget(
    string Id,
    string Name,
    double Limit,
    string Unit,
    string Description);

/// <summary>
/// A renderer/simulation snapshot. The dashboard deliberately distinguishes measured values
/// from absent telemetry; zero is never used to mean "not measured."
/// </summary>
public readonly record struct SpatialLaboratoryPerformanceSnapshot(
    double? FrameTimeMilliseconds,
    double? FramesPerSecond,
    int? SourceLines,
    int? ProjectedLines,
    int? RenderedLines,
    long? GpuPayloadBytes,
    double? SimulationTicksPerSecond,
    int? ActiveAgents,
    double? CollisionEventsPerSecond,
    double? WorkingSetMegabytes)
{
    public static SpatialLaboratoryPerformanceSnapshot Empty
        => new(null, null, null, null, null, null, null, null, null, null);

    public bool HasTelemetry
        => FrameTimeMilliseconds.HasValue
            || FramesPerSecond.HasValue
            || SourceLines.HasValue
            || ProjectedLines.HasValue
            || RenderedLines.HasValue
            || GpuPayloadBytes.HasValue
            || SimulationTicksPerSecond.HasValue
            || ActiveAgents.HasValue
            || CollisionEventsPerSecond.HasValue
            || WorkingSetMegabytes.HasValue;
}
