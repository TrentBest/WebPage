using System.Linq;
using TheSingularityWorkshop.Gui;
using TheSingularityWorkshop.SingularityHub;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialAECInterrogationTests
{
    [Fact]
    public void Viewpoint_ChangesPerceptionWithoutChangingAvatarAnchor()
    {
        var anchor = new SpatialPoint(50, 50);

        var plan = SpatialViewpoint.Plan(anchor);
        var elevation = SpatialViewpoint.Elevation(anchor, "trophy-case");

        Assert.True(plan.IsAvatarAnchored);
        Assert.True(elevation.IsAvatarAnchored);
        Assert.Equal(anchor, elevation.Anchor);
        Assert.Equal("trophy-case", elevation.FocusId);
    }

    [Fact]
    public void BuildingProgram_CollectsQuantitiesAndSupportsGlobalAndIndividualSizing()
    {
        var program = SpatialBuildingProgramCatalog.ResidentialBaseline;

        Assert.Equal(3, program.ResolveRoom("bedroom").Quantity);
        Assert.Equal(11, program.ResolveRoom("bedroom").WidthFeet);
        Assert.Equal(9, program.ResolveRoom("bedroom").HeightFeet);

        var larger = program.ScaleAll(1.1);
        Assert.Equal(12.1, larger.ResolveRoom("bedroom").WidthFeet, 3);

        var custom = larger.ResizeRoom("bedroom", 13, 12);
        Assert.Equal(13, custom.ResolveRoom("bedroom").WidthFeet);
        Assert.Equal(12, custom.ResolveRoom("bedroom").DepthFeet);
    }

    [Fact]
    public void OntologyInterrogation_PresentsAllNineLayersOnTheTable()
    {
        var ontology = new OntologySignature(1, 2, 3, 4, 5, 6, 7, 8, 9);
        var interrogation = new SpatialOntologyInterrogation(ontology);

        Assert.Equal(9, interrogation.TablePresentation.Count);
        Assert.Equal(1, interrogation.ResolveLayer(0).Value);
        Assert.Equal(9, interrogation.ResolveLayer(8).Value);
    }

    [Fact]
    public void ModelSourcing_CrewContainsDistinctSpecialistsAndViablePhysicalParts()
    {
        var manifest = SpatialModelSourcingManifest.CreateDefault();

        Assert.Equal(3, manifest.Scouts.Count);
        Assert.Equal(3, manifest.Scouts.Select(x => x.Specialty).Distinct().Count());
        Assert.Contains(manifest.Candidates, x => x.Name == "Reception Desk" && x.Viable);
        Assert.Contains(manifest.Candidates, x => x.Name == "Trophy Case" && x.Viable);
    }

    [Fact]
    public void LaboratoryAccess_RequiresThreePhysicalIdentitySteps()
    {
        var access = new SpatialLaboratoryAccessControl();

        Assert.False(access.RequestFloor("gravity").Allowed);
        Assert.Equal(SpatialLaboratoryAccessStage.BadgeRequired, access.Stage);

        Assert.True(access.ScanBadge());
        Assert.True(access.ScanFingerprint());
        Assert.True(access.ScanRetina());
        Assert.Equal(SpatialLaboratoryAccessStage.Authorized, access.Stage);
    }

    [Fact]
    public void LaboratoryAccess_DeniesInsufficientClearanceAndReturnsToElevator()
    {
        var access = new SpatialLaboratoryAccessControl();

        access.ScanBadge();
        access.ScanFingerprint();
        access.ScanRetina();

        var denied = access.RequestFloor("quantum");

        Assert.False(denied.Allowed);
        Assert.Equal(SpatialLaboratoryAccessStage.Challenged, access.Stage);
        Assert.Contains("not cleared", denied.Message);

        access.ReturnToElevator();
        Assert.Equal(SpatialLaboratoryAccessStage.Authorized, access.Stage);
    }
}
