namespace TheSingularityWorkshop.Workshop.Agents;

/// <summary>
/// A capability exposed by a physical or simulated vehicle to an operating agent.
/// The vehicle owns the vocabulary; an agent discovers it rather than hard-coding machine knowledge.
/// </summary>
public sealed record VehicleCapability(
    string Id,
    string Description,
    IReadOnlyList<VehicleAction> Actions);

/// <summary>One operation an agent may request from a vehicle capability.</summary>
public sealed record VehicleAction(
    string Id,
    string Description,
    IReadOnlyList<string> RequiredCapabilities);

/// <summary>
/// Contract implemented by things that can expose an executable capability manifest.
/// </summary>
public interface IVehicleCapabilitySource
{
    IReadOnlyList<VehicleCapability> Capabilities { get; }
    bool CanPerform(string actionId);
}
