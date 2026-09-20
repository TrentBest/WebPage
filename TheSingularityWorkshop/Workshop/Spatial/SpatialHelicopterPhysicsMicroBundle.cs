namespace TheSingularityWorkshop.Workshop.Spatial;

/// <summary>
/// LUT-driven cinematic helicopter flight and tower sway substrate.
/// This is deliberately deterministic presentation physics: the browser renders the
/// sampled result rather than inventing motion with ad-hoc CSS timing.
/// </summary>
public sealed class SpatialHelicopterPhysicsMicroBundle
{
    public SpatialHelicopterPhysicsConstants Constants { get; } = new();

    public SpatialHelicopterPhysicsSample Takeoff(double progress) => Sample(TakeoffLut, progress);
    public SpatialHelicopterPhysicsSample Landing(double progress) => Sample(LandingLut, progress);
    public SpatialTowerSwaySample TowerSway(double progress) => Sample(TowerSwayLut, progress);

    private static readonly SpatialHelicopterPhysicsSample[] TakeoffLut =
    [
        new(0.00, 0.00, 0.00, 0.00, 0.00, 0.72, 0.18, 0.00),
        new(0.05, 0.01, 0.02, 0.01, 0.42, 0.78, 0.27, 0.01),
        new(0.10, 0.03, 0.05, 0.02, 0.61, 0.83, 0.35, 0.02),
        new(0.15, 0.05, 0.09, 0.03, 0.78, 0.88, 0.42, 0.03),
        new(0.20, 0.08, 0.14, 0.05, 0.92, 0.92, 0.49, 0.04),
        new(0.25, 0.12, 0.20, 0.07, 1.04, 0.96, 0.56, 0.05),
        new(0.30, 0.16, 0.27, 0.09, 1.13, 1.00, 0.63, 0.06),
        new(0.35, 0.21, 0.35, 0.12, 1.19, 1.03, 0.69, 0.07),
        new(0.40, 0.27, 0.44, 0.15, 1.23, 1.05, 0.74, 0.08),
        new(0.45, 0.33, 0.54, 0.18, 1.25, 1.06, 0.78, 0.09),
        new(0.50, 0.40, 0.65, 0.22, 1.25, 1.07, 0.81, 0.10),
        new(0.55, 0.47, 0.76, 0.26, 1.22, 1.07, 0.83, 0.09),
        new(0.60, 0.54, 0.87, 0.30, 1.17, 1.06, 0.84, 0.08),
        new(0.65, 0.61, 0.98, 0.34, 1.10, 1.05, 0.84, 0.07),
        new(0.70, 0.68, 1.09, 0.38, 1.01, 1.04, 0.82, 0.06),
        new(0.75, 0.75, 1.20, 0.42, 0.90, 1.02, 0.79, 0.05),
        new(0.80, 0.82, 1.30, 0.46, 0.77, 1.00, 0.75, 0.04),
        new(0.85, 0.88, 1.39, 0.49, 0.62, 0.98, 0.69, 0.03),
        new(0.90, 0.94, 1.47, 0.52, 0.46, 0.96, 0.62, 0.02),
        new(0.95, 0.98, 1.54, 0.55, 0.25, 0.94, 0.54, 0.01),
        new(1.00, 1.00, 1.60, 0.58, 0.08, 0.92, 0.48, 0.00)
    ];

    private static readonly SpatialHelicopterPhysicsSample[] LandingLut =
    [
        new(0.00, 0.00, 1.60, 0.58, 0.08, 0.92, 0.48, 0.00),
        new(0.05, 0.03, 1.54, 0.55, -0.21, 0.90, 0.43, -0.01),
        new(0.10, 0.06, 1.47, 0.52, -0.38, 0.89, 0.39, -0.02),
        new(0.15, 0.10, 1.39, 0.49, -0.52, 0.88, 0.35, -0.03),
        new(0.20, 0.14, 1.31, 0.46, -0.63, 0.87, 0.32, -0.04),
        new(0.25, 0.19, 1.22, 0.43, -0.71, 0.86, 0.30, -0.05),
        new(0.30, 0.24, 1.13, 0.40, -0.77, 0.85, 0.29, -0.06),
        new(0.35, 0.29, 1.04, 0.37, -0.80, 0.84, 0.29, -0.07),
        new(0.40, 0.35, 0.95, 0.34, -0.81, 0.83, 0.30, -0.08),
        new(0.45, 0.41, 0.86, 0.31, -0.80, 0.82, 0.31, -0.09),
        new(0.50, 0.47, 0.78, 0.28, -0.77, 0.81, 0.33, -0.10),
        new(0.55, 0.53, 0.70, 0.25, -0.72, 0.80, 0.35, -0.09),
        new(0.60, 0.59, 0.62, 0.22, -0.64, 0.79, 0.37, -0.08),
        new(0.65, 0.65, 0.54, 0.19, -0.55, 0.78, 0.39, -0.07),
        new(0.70, 0.71, 0.46, 0.16, -0.44, 0.77, 0.41, -0.06),
        new(0.75, 0.77, 0.39, 0.13, -0.32, 0.76, 0.43, -0.05),
        new(0.80, 0.83, 0.32, 0.10, -0.21, 0.75, 0.45, -0.04),
        new(0.85, 0.88, 0.25, 0.08, -0.13, 0.74, 0.46, -0.03),
        new(0.90, 0.93, 0.18, 0.06, -0.08, 0.73, 0.47, -0.02),
        new(0.95, 0.97, 0.10, 0.03, -0.03, 0.72, 0.48, -0.01),
        new(1.00, 1.00, 0.00, 0.00, 0.00, 0.70, 0.50, 0.00)
    ];

    private static readonly SpatialTowerSwaySample[] TowerSwayLut =
    [
        new(0.00, -2.2,  1.0, -0.8),
        new(0.05, -1.1,  1.7,  0.3),
        new(0.10,  0.8,  0.9,  1.1),
        new(0.15,  2.4, -0.2,  0.4),
        new(0.20,  1.3, -1.4, -0.9),
        new(0.25, -0.7, -2.0, -1.5),
        new(0.30, -2.6, -0.8, -0.4),
        new(0.35, -1.5,  1.1,  1.0),
        new(0.40,  0.4,  2.1,  1.7),
        new(0.45,  2.7,  1.0,  0.6),
        new(0.50,  1.8, -0.7, -0.7),
        new(0.55, -0.2, -1.8, -1.4),
        new(0.60, -2.5, -1.2, -0.2),
        new(0.65, -1.7,  0.8,  0.9),
        new(0.70,  0.2,  1.9,  1.5),
        new(0.75,  2.5,  1.2,  0.4),
        new(0.80,  1.9, -0.5, -0.8),
        new(0.85,  0.0, -1.7, -1.3),
        new(0.90, -2.3, -1.0, -0.3),
        new(0.95, -1.4,  0.9,  0.8),
        new(1.00,  0.5,  1.6,  1.2)
    ];

    private static SpatialHelicopterPhysicsSample Sample(
        IReadOnlyList<SpatialHelicopterPhysicsSample> lut,
        double progress)
    {
        progress = Math.Clamp(progress, 0d, 1d);
        var scaled = progress * (lut.Count - 1);
        var index = Math.Min((int)Math.Floor(scaled), lut.Count - 2);
        var t = scaled - index;
        var a = lut[index];
        var b = lut[index + 1];

        return new(
            progress,
            Lerp(a.X, b.X, t),
            Lerp(a.Y, b.Y, t),
            Lerp(a.Z, b.Z, t),
            Lerp(a.VerticalVelocity, b.VerticalVelocity, t),
            Lerp(a.RotorRpm, b.RotorRpm, t),
            Lerp(a.Collective, b.Collective, t),
            Lerp(a.Pitch, b.Pitch, t));
    }

    private static SpatialTowerSwaySample Sample(
        IReadOnlyList<SpatialTowerSwaySample> lut,
        double progress)
    {
        progress = Math.Clamp(progress, 0d, 1d);
        var scaled = progress * (lut.Count - 1);
        var index = Math.Min((int)Math.Floor(scaled), lut.Count - 2);
        var t = scaled - index;
        var a = lut[index];
        var b = lut[index + 1];

        return new(
            progress,
            Lerp(a.Roll, b.Roll, t),
            Lerp(a.Pitch, b.Pitch, t),
            Lerp(a.Yaw, b.Yaw, t));
    }

    private static double Lerp(double a, double b, double t) => a + (b - a) * t;
}

public sealed record SpatialHelicopterPhysicsConstants
{
    public double MassKg { get; init; } = 2500d;
    public double RotorDiameterMeters { get; init; } = 13.4d;
    public double CruiseSpeedMetersPerSecond { get; init; } = 62d;
    public double TakeoffDurationSeconds { get; init; } = 3.4d;
    public double LandingDurationSeconds { get; init; } = 3.1d;
    public double TowerRevealDurationSeconds { get; init; } = 3.8d;
    public double MaximumTowerRollDegrees { get; init; } = 2.7d;
    public double MaximumTowerPitchDegrees { get; init; } = 2.1d;
    public double MaximumTowerYawDegrees { get; init; } = 1.7d;
}

public readonly record struct SpatialHelicopterPhysicsSample(
    double Progress,
    double X,
    double Y,
    double Z,
    double VerticalVelocity,
    double RotorRpm,
    double Collective,
    double Pitch);

public readonly record struct SpatialTowerSwaySample(
    double Progress,
    double Roll,
    double Pitch,
    double Yaw);
