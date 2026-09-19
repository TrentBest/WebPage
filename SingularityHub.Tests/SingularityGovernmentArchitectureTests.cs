using System;
using System.Linq;
using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SingularityGovernmentArchitectureTests
{
    [Fact]
    public void SingularityMayorIsTheInitialElectedExecutiveOffice()
    {
        var mayor = SingularityGovernmentCatalog.Resolve("government.mayor");

        Assert.Equal("Singularity Mayor", mayor.Title);
        Assert.Equal(SingularityGovernmentBranch.Executive, mayor.Branch);
        Assert.True(mayor.Elected);
        Assert.Equal("civic.government", mayor.BuildingSpecificationId);
    }

    [Fact]
    public void GovernmentContainsElectedAndAdministrativePowers()
    {
        Assert.Contains(
            SingularityGovernmentCatalog.ElectedOffices,
            office => office.Type == SingularityOfficeType.Mayor);

        Assert.Contains(
            SingularityGovernmentCatalog.Offices,
            office => office.Type == SingularityOfficeType.Council);

        Assert.Contains(
            SingularityGovernmentCatalog.Offices,
            office => office.Type == SingularityOfficeType.Planning);

        Assert.Contains(
            SingularityGovernmentCatalog.Offices,
            office => office.Type == SingularityOfficeType.Treasury);
    }

    [Fact]
    public void ElectionsAllowPlayersToRunAndVoteAndRecur()
    {
        var rules = SingularityElectionCatalog.DefaultRules;
        var opening = DateTimeOffset.Parse("2030-01-01T00:00:00+00:00");

        Assert.True(rules.CitizensMayRun);
        Assert.True(rules.CitizensMayVote);
        Assert.Equal(TimeSpan.FromDays(14), rules.ElectionInterval);

        var election = SingularityElectionCatalog.Create(
            "mayor-001",
            "government.mayor",
            opening,
            ["citizen-a", "citizen-b"]);

        Assert.Equal(opening.AddDays(14), election.ClosesAt);
        Assert.Equal(2, election.CandidateIds.Count);
    }

    [Fact]
    public void BuildingPurposeDrivesResidentialAndOfficeSpecifications()
    {
        var lowEnd = SpatialBuildingSpecificationCatalog.Resolve("residential.multifamily.low");
        var luxury = SpatialBuildingSpecificationCatalog.Resolve("residential.multifamily.luxury");
        var office = SpatialBuildingSpecificationCatalog.Resolve("office.corporate");

        Assert.Equal(10, lowEnd.MinimumWidth);
        Assert.Equal(10, lowEnd.MinimumDepth);
        Assert.True(luxury.MaximumFootprint > lowEnd.MaximumFootprint);
        Assert.Equal(SpatialBuildingPurpose.Office, office.Purpose);
        Assert.True(office.MaximumFootprint > office.MinimumFootprint);
    }

    [Fact]
    public void GovernmentBuildingsUseTheCivicBuildingSpecification()
    {
        var specification = SpatialBuildingSpecificationCatalog.Resolve("civic.government");

        Assert.Equal(SpatialBuildingPurpose.CivicGovernment, specification.Purpose);
        Assert.True(specification.AcceptsFootprint(30, 20));
        Assert.True(specification.MaximumFootprint > specification.MinimumFootprint);
    }
}
