using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialCivilizationExpansionTests
{
    [Fact(DisplayName = "Incremental Unit Test 55 — civilization expansion has space, city, ocean, and island domains")]
    public void CivilizationExpansionIsComposable()
    {
        var civilization = SpatialCivilizationManifest.CreateDefault();

        Assert.Contains(civilization.Destinations, x => x.Id == "singularity-shipyard");
        Assert.Contains(civilization.Destinations, x => x.Id == "luna-colony");
        Assert.Contains(civilization.Destinations, x => x.Id == "mars-colony");
        Assert.Contains(civilization.Destinations, x => x.Id == "singularity-capitol");
        Assert.Contains(civilization.Destinations, x => x.Id == "ocean-shipyard");
        Assert.Contains(civilization.SimulationDomains, x => x.Id == "city-builder");
        Assert.Contains(civilization.SimulationDomains, x => x.Id == "aec-simulation");
    }

    [Fact(DisplayName = "Incremental Unit Test 56 — shipyard separates construction from station transit")]
    public void ShipyardIsIndependentAndCapabilityDriven()
    {
        var shipyard = SpatialShipyardManifest.CreateDefault();

        Assert.Contains("SHUTTLE REQUIRED", shipyard.RelationshipToStation);
        Assert.Contains(shipyard.BuildRules, x => x.Id == "time-gated");
        Assert.Contains(shipyard.BuildRules, x => x.Id == "resource-gated");
        Assert.Contains(shipyard.ComponentFamilies, x => x.Id == "science-fiction");
        Assert.Contains(shipyard.ComponentFamilies.Single(x => x.Id == "science-fiction").Components, x => x == "warp engine");
        Assert.Contains(shipyard.ComponentFamilies.Single(x => x.Id == "science-fiction").Components, x => x == "hyperspace engine");
    }

    [Fact(DisplayName = "Incremental Unit Test 57 — AEC behaviors are separable from building models")]
    public void AecBehaviorPackagesAreComposable()
    {
        var aec = SpatialAECExperienceManifest.CreateDefault();

        Assert.Contains(aec.BehaviorDomains, x => x.Id == "hvac");
        Assert.Contains(aec.BehaviorDomains.Single(x => x.Id == "hvac").Behaviors, x => x == "replacement tenancy");
        Assert.Contains(aec.BehaviorDomains, x => x.Id == "healthcare");
        Assert.Contains(aec.UseCases, x => x.Id == "behavior-package");
        Assert.Contains(aec.MonetizationHooks, x => x == "behavior-package-sale");
    }
}
