using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialGamesAndDigitensTests
{
    [Fact(DisplayName = "Incremental Unit Test 66 — game systems are generalized and composable")]
    public void GameSystemsComposeWithoutCommercialTitleDependencies()
    {
        var manifest = SpatialGameSystemsManifest.CreateDefault();
        var strategy = manifest.FindExperience("grand-strategy")!;

        Assert.Contains("strategy-4x", strategy.SystemIds);
        Assert.Contains("economy", strategy.SystemIds);
        Assert.Contains("diplomacy", strategy.SystemIds);
        Assert.Contains("production", strategy.SystemIds);
        Assert.Equal(strategy.SystemIds.Count, manifest.SystemsFor(strategy).Count);
    }

    [Fact(DisplayName = "Incremental Unit Test 67 — game shops distribute reusable Experience families")]
    public void GameShopsExposeFamilies()
    {
        var manifest = SpatialGameSystemsManifest.CreateDefault();

        Assert.Contains(manifest.Shops, x => x.Id == "strategy-shop" && x.PrimarySystemId == "strategy-4x");
        Assert.Contains(manifest.Shops, x => x.Id == "tactics-shop" && x.PrimarySystemId == "tactical-command");
        Assert.Contains(manifest.Shops, x => x.Id == "simulation-shop" && x.PrimarySystemId == "economy");
    }

    [Fact(DisplayName = "Incremental Unit Test 68 — Digitens have civic and ordinary-life roles")]
    public void DigitensHaveCivicAndEverydayRoles()
    {
        var manifest = SpatialDigitenManifest.CreateDefault();

        Assert.Equal("MAYOR", manifest.FindRole("mayor")!.Name);
        Assert.Equal("CITY COUNCIL", manifest.FindRole("council-member")!.Name);
        Assert.Equal("HVAC TECHNICIAN", manifest.FindRole("hvac-technician")!.Name);
        Assert.Equal("POLICE OFFICER", manifest.FindRole("police-officer")!.Name);
        Assert.Contains(manifest.Activities, x => x.Id == "work");
        Assert.Contains(manifest.Activities, x => x.Id == "socialize");
        Assert.Contains(manifest.Activities, x => x.Id == "maintain");
    }

    [Fact(DisplayName = "Incremental Unit Test 69 — civic life is modeled as services and routines")]
    public void CivicLifeIsServiceDriven()
    {
        var manifest = SpatialDigitenManifest.CreateDefault();

        Assert.Contains(manifest.Services, x => x.Id == "civic-government");
        Assert.Contains(manifest.Services, x => x.Id == "public-safety");
        Assert.Contains(manifest.Services, x => x.Id == "building-maintenance");
        Assert.Contains(manifest.Services, x => x.Id == "transit");

        var onCall = manifest.FindRoutine("on-call")!;
        Assert.Contains("respond", onCall.ActivityIds);
        Assert.Contains("maintain", onCall.ActivityIds);
    }
}
