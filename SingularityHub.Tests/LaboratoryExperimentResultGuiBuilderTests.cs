#if WORKSHOP_USE_REPOSITORY_SOURCES
using TheSingularityWorkshop.Gui;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;
using Xunit;

namespace SingularityHub.Tests;

public sealed class LaboratoryExperimentResultGuiBuilderTests
{
    [Fact]
    public void WebPageProjectsImmutableRunProvenanceAndResultPayloadWithoutExecutionAuthority()
    {
        var runId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var replaySourceId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var started = DateTimeOffset.Parse("2026-11-20T10:00:00Z");
        var completed = DateTimeOffset.Parse("2026-11-20T10:00:02.500Z");
        var run = new ExperimentRunRecord(
            runId,
            "thermal-lab", "3.2.0",
            "thermal-expansion", "2.1.0",
            "linear-expansion", "1.4.0",
            3, "application/json",
            """{"initial-temperature":293.15}""",
            ScientificValidationLevel.EducationalModel,
            started, completed,
            "application/json",
            """{"final-length":1.0024,"unit":"m"}""",
            ["The sample is homogeneous."],
            ["The model omits phase changes."],
            replaySourceId);
        var view = new ExperimentResultView(run);

        var node = LaboratoryExperimentResultGuiBuilder.Build([view]);
        var result = node.Find("experiment-run-" + runId.ToString("N"));

        Assert.Equal("1", node.Properties["data-result-count"]);
        Assert.Equal(runId.ToString("D"), result.Properties["data-run-id"]);
        Assert.Equal("thermal-lab", result.Properties["data-laboratory-id"]);
        Assert.Equal("3.2.0", result.Properties["data-laboratory-version"]);
        Assert.Equal("thermal-expansion", result.Properties["data-experiment-id"]);
        Assert.Equal("2.1.0", result.Properties["data-experiment-version"]);
        Assert.Equal("linear-expansion", result.Properties["data-model-id"]);
        Assert.Equal("1.4.0", result.Properties["data-model-version"]);
        Assert.Equal("EducationalModel", result.Properties["data-validation-level"]);
        Assert.Equal(started.ToString("O"), result.Properties["data-started-at-utc"]);
        Assert.Equal(completed.ToString("O"), result.Properties["data-completed-at-utc"]);
        Assert.Equal("00:00:02.5000000", result.Properties["data-elapsed"]);
        Assert.Equal("3", result.Properties["data-configuration-schema-version"]);
        Assert.Equal(run.ConfigurationPayload, result.Properties["data-configuration-payload"]);
        Assert.Equal(run.ResultPayload, result.Properties["data-result-payload"]);
        Assert.Equal(replaySourceId.ToString("D"), result.Properties["data-replayed-from-run-id"]);
        Assert.Contains("homogeneous", result.Find("experiment-run-" + runId.ToString("N") + "-assumptions").TextContent);
        Assert.Contains("phase changes", result.Find("experiment-run-" + runId.ToString("N") + "-limitations").TextContent);

        Assert.DoesNotContain("data-execution-admitted", result.Properties.Keys);
        Assert.DoesNotContain("data-execution-started", result.Properties.Keys);
        Assert.DoesNotContain("data-access-granted", result.Properties.Keys);
    }

    [Fact]
    public void ResultCatalogueRejectsDuplicateRunIdentities()
    {
        var run = new ExperimentRunRecord(
            Guid.NewGuid(), "lab", "1.0.0", "experiment", "1.0.0",
            "model", "1.0.0", 1, "application/json", "{}",
            ScientificValidationLevel.EducationalModel,
            DateTimeOffset.Parse("2026-11-20T10:00:00Z"),
            DateTimeOffset.Parse("2026-11-20T10:00:01Z"),
            "application/json", "{}", ["assumption"], ["limitation"]);
        var view = new ExperimentResultView(run);

        Assert.Throws<ArgumentException>(() =>
            LaboratoryExperimentResultGuiBuilder.Build([view, view]));
    }
}
#endif
