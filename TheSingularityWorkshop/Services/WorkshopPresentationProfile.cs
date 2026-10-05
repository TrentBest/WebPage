namespace TheSingularityWorkshop.Services;

public sealed class WorkshopPresentationProfile
{
    public const int DefaultPopulationThreshold = 64;
    public const double DefaultGrowthStep = 20;
    public const double DefaultMaximumNodeSize = 100;
    public const double DefaultSeedFlightStep = 4.5;
    public const double DefaultSeedScalingStep = 40;
    public const double DefaultParentRecoveryStep = 80;
    public const double DefaultGravityAcceleration = 1.15;
    public const double DefaultMonikerDurationSeconds = 3;

    public int PopulationThreshold { get; set; } = DefaultPopulationThreshold;
    public double GrowthStep { get; set; } = DefaultGrowthStep;
    public double MaximumNodeSize { get; set; } = DefaultMaximumNodeSize;
    public double SeedFlightStep { get; set; } = DefaultSeedFlightStep;
    public double SeedScalingStep { get; set; } = DefaultSeedScalingStep;
    public double ParentRecoveryStep { get; set; } = DefaultParentRecoveryStep;
    public double GravityAcceleration { get; set; } = DefaultGravityAcceleration;
    public double MonikerDurationSeconds { get; set; } = DefaultMonikerDurationSeconds;

    public static WorkshopPresentationProfile Current { get; } = new();

    public void Reset()
    {
        PopulationThreshold = DefaultPopulationThreshold;
        GrowthStep = DefaultGrowthStep;
        MaximumNodeSize = DefaultMaximumNodeSize;
        SeedFlightStep = DefaultSeedFlightStep;
        SeedScalingStep = DefaultSeedScalingStep;
        ParentRecoveryStep = DefaultParentRecoveryStep;
        GravityAcceleration = DefaultGravityAcceleration;
        MonikerDurationSeconds = DefaultMonikerDurationSeconds;
    }
}
