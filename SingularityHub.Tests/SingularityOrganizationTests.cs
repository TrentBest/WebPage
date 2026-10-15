using System;
using TheSingularityWorkshop.Workshop.City;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SingularityOrganizationTests
{
    [Fact]
    public void ArchitectureFirmUsesGeneralOrganizationAndDigitenIdentity()
    {
        var firm = new SingularityOrganization(
            "firm.northline", "Northline Studio", "Architecture", "building.studio");
        firm.AddMember(new OrganizationMember(42, "Architect"));
        firm.AddMember(new OrganizationMember(43, "Model Author"));
        firm.AddProject(new OrganizationProject(
            "project.courtyard-house", "Courtyard House", "BuildingPlan", "plan.courtyard-house"));

        Assert.Equal("Architecture", firm.Industry);
        Assert.Equal(2, firm.Members.Count);
        Assert.True(firm.TryGetMember(42, out var architect));
        Assert.Equal("Architect", architect!.Role);
        Assert.True(firm.TryGetProject("project.courtyard-house", out var project));
        Assert.Equal("BuildingPlan", project!.ArtifactKind);
        Assert.Equal("building.studio", firm.BuildingId);
    }

    [Fact]
    public void OrganizationRejectsDuplicateMemberIdentity()
    {
        var firm = new SingularityOrganization("firm", "Studio", "Architecture");
        firm.AddMember(new OrganizationMember(42, "Architect"));

        Assert.Throws<ArgumentException>(() => firm.AddMember(new OrganizationMember(42, "Director")));
    }

    [Fact]
    public void OrganizationCanChangeLocationWithoutChangingIdentity()
    {
        var organization = new SingularityOrganization("org", "Workshop School", "Education", "building.old");
        organization.LocateAt("building.new");

        Assert.Equal("org", organization.Id);
        Assert.Equal("building.new", organization.BuildingId);
    }
}
