namespace TheSingularityWorkshop.World;

public sealed record HiddenWorldSwitch(
    string Id,
    string LocationId,
    string Name,
    string DiscoveryTitle,
    string DiscoveryDescription,
    string EventType,
    WorkshopPresentationMode ResultingPresentationMode)
{
    public static HiddenWorldSwitch ResearchLaboratoryThreeDimensionality { get; } =
        new(
            "switch.research-laboratory.3d",
            "singularity-lab",
            "RESEARCH SYSTEM // 07",
            "A CAPABILITY HAS BEEN DISCOVERED",
            "A visitor found something the Workshop was not advertising.",
            "world.presentation.3d.unlock",
            WorkshopPresentationMode.ThreeDimensional);
}
