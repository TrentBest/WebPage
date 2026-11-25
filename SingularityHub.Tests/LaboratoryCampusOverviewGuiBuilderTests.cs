#if WORKSHOP_USE_REPOSITORY_SOURCES
using TheSingularityWorkshop.Gui;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;
using DomainLaboratoryInstrument = TheSingularityWorkshop.SingularityLaboratories.Contracts.LaboratoryInstrument;
using Xunit;

namespace SingularityHub.Tests;

public sealed class LaboratoryCampusOverviewGuiBuilderTests
{
    [Fact]
    public void WebPageProjectsOwnerCampusSemanticsWithoutCreatingSpatialOrAuthorizationClaims()
    {
        var laboratory = new LaboratoryDefinition(
            "teaching-physics", "1.0.0", "Teaching Physics",
            "A fictional supervised measurement workspace.");
        var facility = new LaboratoryFacility(
            "teaching-hall", "1.0.0", "Teaching Hall",
            [new LaboratoryRoom("bench-room", "Measurement Bench", "Supervised measurement.")],
            [new LaboratoryApparatus("calorimeter", "Calorimetry Station",
                "thermal-measurement-apparatus", "bench-room", "Illustrative apparatus.")],
            [new DomainLaboratoryInstrument("temperature-probe", "Temperature Probe",
                "temperature-sensor", "calorimeter", "1.0.0", "SAMPLE-CAL-001")],
            [new MeasurementChannel("temperature", "Sample Temperature",
                "temperature-probe", "thermodynamic-temperature", "K",
                MeasurementSamplingMode.Periodic, 1d)]);
        var campus = new LaboratoryCampus(
            "teaching-campus", "1.0.0", "Teaching Campus",
            "A fictional test campus.", [laboratory], [facility],
            [new LaboratoryCampusWorkspace(
                "physics-workspace", laboratory.Id, facility.Id, "bench-room")]);

        var overview = LaboratoryCampusOverview.Create(campus);
        var node = LaboratoryCampusOverviewGuiBuilder.Build(overview);

        Assert.Equal("teaching-campus", node.Properties["data-campus-id"]);
        Assert.Equal("Teaching Campus", node.Text);
        Assert.Equal("Teaching Physics", node.Find("physics-workspace").Text);
        Assert.Equal("Teaching Hall", node.Find("physics-workspace-facility").Text);
        Assert.Equal("Measurement Bench — Supervised measurement.", node.Find("physics-workspace-room").Text);
        Assert.Equal("Calorimetry Station", node.Find("calorimeter").Text);
        Assert.Equal("Temperature Probe", node.Find("temperature-probe").Text);
        Assert.Equal("Sample Temperature — thermodynamic-temperature (K)", node.Find("temperature").Text);
        Assert.Equal("K", node.Find("temperature").Properties["data-unit"]);
        Assert.Equal("Periodic", node.Find("temperature").Properties["data-sampling-mode"]);
        Assert.Equal("1", node.Find("temperature").Properties["data-samples-per-second"]);
        Assert.DoesNotContain("data-access-granted", node.Properties.Keys);
        Assert.DoesNotContain("data-world-coordinate", node.Properties.Keys);
    }
}
#endif
