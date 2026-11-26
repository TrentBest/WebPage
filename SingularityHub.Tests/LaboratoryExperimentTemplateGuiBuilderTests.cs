#if WORKSHOP_USE_REPOSITORY_SOURCES
using System.Text.Json;
using TheSingularityWorkshop.Gui;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;
using Xunit;

namespace SingularityHub.Tests;

public sealed class LaboratoryExperimentTemplateGuiBuilderTests
{
    [Fact]
    public void WebPageProjectsVersionedAuthoringContractWithoutClaimingAdmissionOrExecution()
    {
        var evidence = new ExperimentValidationEvidence(
            "review-1", "Reviewed the documented educational model.", DateTimeOffset.Parse("2026-01-01T00:00:00Z"));
        var assessment = new ExperimentValidationAssessment(
            "thermal-expansion", "2.1.0", "linear-expansion", "1.4.0",
            ExperimentValidationState.Accepted, DateTimeOffset.Parse("2026-01-02T00:00:00Z"),
            [evidence], reviewer: "Laboratory Review Board");
        var safety = new ExperimentSafetyMetadata(
            ExperimentRiskLevel.Low,
            ["Hot surfaces may cause burns."],
            ["Use the supplied insulated handling tools."],
            ["Do not use as an industrial process controller."]);
        var definition = new ExperimentDefinition(
            "thermal-expansion", "2.1.0", "Thermal Expansion",
            "linear-expansion", "1.4.0", ScientificValidationLevel.EducationalModel,
            ["The sample is homogeneous."], ["The model omits phase changes."],
            safety, assessment,
            [new ExperimentPhysicsDependency(
                "thermal-expansion-capability", "thermodynamics", "linear-expansion", "1.4.0",
                "Resolve the sample's temperature-dependent expansion.")],
            [new ExperimentMicroBundleRequirement(
                42, "1.0.0", "physics-provider", "Supply the thermal material constants.")]);
        var schema = new ExperimentParameterSchema(
            definition.Id, definition.Version, 3,
            [
                new ExperimentParameterDefinition(
                    "initial-temperature", "Initial temperature", "Starting sample temperature.",
                    ExperimentParameterType.Number, unitSymbol: "K", minimum: 0, maximum: 2000),
                new ExperimentParameterDefinition(
                    "material", "Material", "Material identity.", ExperimentParameterType.Enumeration,
                    allowedValues: ["copper", "aluminum"])
            ]);
        var configuration = new ExperimentConfiguration(
            definition.Id, definition.Version, 3, "application/json",
            """{"initial-temperature":293.15,"material":"copper"}""");
        var template = new ExperimentAuthoringTemplate(
            "thermal-expansion-intro", "1.0.0", "Thermal Expansion — Introduction",
            definition, schema, configuration);

        var node = LaboratoryExperimentTemplateGuiBuilder.Build(template);

        Assert.Equal("thermal-expansion", node.Properties["data-experiment-id"]);
        Assert.Equal("2.1.0", node.Properties["data-experiment-version"]);
        Assert.Equal("EducationalModel", node.Properties["data-validation-level"]);
        Assert.Equal(configuration.Payload, node.Properties["data-default-configuration"]);
        Assert.Equal("Accepted", node.Find("experiment-validation-assessment").Properties["data-assessment-state"]);
        Assert.Equal("Low", node.Find("experiment-safety-disclosures").Properties["data-risk-level"]);
        Assert.Equal("293.15", node.Find("initial-temperature").Properties["data-default-value"]);
        Assert.Equal("K", node.Find("initial-temperature").Properties["data-unit"]);
        Assert.Equal("0", node.Find("initial-temperature").Properties["data-minimum"]);
        Assert.Equal("2000", node.Find("initial-temperature").Properties["data-maximum"]);
        Assert.Equal(JsonSerializer.Serialize(new[] { "copper", "aluminum" }),
            node.Find("material").Properties["data-allowed-values"]);
        Assert.Equal("thermal-expansion-capability",
            node.Find("physics-dependency-thermal-expansion-capability").Properties["data-physics-capability-id"]);
        Assert.Equal("42",
            node.Find("microbundle-requirement-42").Properties["data-microbundle-id"]);

        Assert.DoesNotContain("data-execution-admitted", node.Properties.Keys);
        Assert.DoesNotContain("data-execution-started", node.Properties.Keys);
        Assert.DoesNotContain("data-access-granted", node.Properties.Keys);
    }
}
#endif
