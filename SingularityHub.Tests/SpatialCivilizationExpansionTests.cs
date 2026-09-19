using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialCivilizationExpansionTests
{
    [Fact(DisplayName = "Incremental Unit Test 58 — ship drives change interaction modes without changing the environment")]
    public void ShipDriveSelectionChangesInteractionModes()
    {
        var design = SpatialShipDesignManifest.CreateDefault();

        Assert.Contains(design.ModesForDrive("warp"), x => x.Id == "warp-flight");
        Assert.Contains(design.ModesForDrive("hyperspace"), x => x.Id == "hyperspace-flight");
        Assert.Contains(design.ModesForDrive("chemical"), x => x.Id == "orbital-flight");
        Assert.DoesNotContain(design.ModesForDrive("warp"), x => x.Id == "hyperspace-flight");
    }

    [Fact(DisplayName = "Incremental Unit Test 59 — orbital shipyard requires shuttle transit")]
    public void OrbitalShipyardIsIndependentFromStation()
    {
        var orbital = SpatialOrbitalShipyardManifest.CreateDefault();
        var route = Assert.Single(orbital.ShuttleRoutes);

        Assert.Equal("singularity-station", route.OriginId);
        Assert.Equal("singularity-shipyard", route.DestinationId);
        Assert.Equal("SHUTTLE", route.VehicleType);
        Assert.NotEqual(orbital.StationPosition, orbital.ShipyardPosition);
    }

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

    [Fact(DisplayName = "Incremental Unit Test 60 — intelligent AEC drafting table exposes semantic line intent and external targets")]
    public void IntelligentDraftingTableIsPlatformNeutral()
    {
        var office = SpatialAECRemoteOfficeManifest.CreateDefault();

        Assert.Equal("AEC SECTOR PORTAL", office.SectorPortalName);
        Assert.True(office.SupportsLineType("wall"));
        Assert.True(office.SupportsLineType("equipment"));
        Assert.Contains(office.DraftingTable.Controls, x => x == "angle");
        Assert.Contains(office.ExternalTargets, x => x.Id == "revit");
        Assert.Contains(office.ExternalTargets, x => x.Id == "inventor");
        Assert.Contains(office.ExternalTargets, x => x.Id == "singularity-warehouse");
    }

    [Fact(DisplayName = "Incremental Unit Test 61 — Singularity Taxi makes the ride optional without removing the Experience")]
    public void SingularityTaxiSupportsRideOrInstantArrival()
    {
        var taxi = SpatialTaxiManifest.CreateDefault();

        Assert.True(taxi.Rules.RideIsOptional);
        Assert.True(taxi.Rules.InstantDestinationAvailable);
        Assert.True(taxi.Rules.PassengerMaySkipRide);
        Assert.True(taxi.Rules.RideCountsAsExperience);
        Assert.Contains(taxi.Fleet, x => x.Id == "hover-cab");
        Assert.Contains(taxi.Fleet, x => x.Id == "orbital-shuttle");
        Assert.NotNull(taxi.FindDestination("station"));
    }

    [Fact(DisplayName = "Incremental Unit Test 62 — Explore exposes the AEC office while taxi remains a city destination")]
    public void ExploreExposesAecOfficeAndTaxi()
    {
        var scene = SpatialWorkshopScene.CreateDefault();

        Assert.Contains(scene.Interactables, x => x.ExperienceId == "aec-remote-office");
        Assert.Contains(SpatialPlaceCatalog.CityPlaces, x => x.SceneId == "singularity-taxi");
    }
}
