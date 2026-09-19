using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialLaboratoryPerformanceManifestTests
{
    [Fact]
    public void DefaultPerformanceObservatory_DeclaresCoreSignalsAndBudgets()
    {
        var performance = SpatialLaboratoryPerformanceManifest.CreateDefault();

        Assert.Contains(performance.Instruments, x => x.Id == "frame");
        Assert.Contains(performance.Instruments, x => x.Id == "geometry");
        Assert.Contains(performance.Instruments, x => x.Id == "payload");
        Assert.Contains(performance.Instruments, x => x.Id == "simulation");
        Assert.Contains(performance.Instruments, x => x.Id == "agents");

        Assert.Equal(16.67, performance.FindBudget("frame")!.Value.Limit, 2);
        Assert.Equal(20_000, performance.FindBudget("lines")!.Value.Limit);
        Assert.Equal(4_000_000, performance.FindBudget("payload")!.Value.Limit);
    }

    [Fact]
    public void EmptySnapshot_DoesNotPretendToContainRuntimeMeasurements()
    {
        var snapshot = SpatialLaboratoryPerformanceSnapshot.Empty;

        Assert.False(snapshot.HasTelemetry);
        Assert.Null(snapshot.FrameTimeMilliseconds);
        Assert.Null(snapshot.FramesPerSecond);
        Assert.Null(snapshot.RenderedLines);
    }

    [Fact]
    public void PopulatedSnapshot_ReportsTelemetry()
    {
        var snapshot = new SpatialLaboratoryPerformanceSnapshot(
            8.4,
            118.2,
            20_000,
            12_000,
            10_000,
            1_200_000,
            60,
            7,
            14,
            84);

        Assert.True(snapshot.HasTelemetry);
        Assert.Equal(8.4, snapshot.FrameTimeMilliseconds);
        Assert.Equal(118.2, snapshot.FramesPerSecond);
        Assert.Equal(1_200_000, snapshot.GpuPayloadBytes);
    }
}
