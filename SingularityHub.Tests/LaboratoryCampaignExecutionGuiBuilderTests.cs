#if WORKSHOP_USE_REPOSITORY_SOURCES
using TheSingularityWorkshop.Gui;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;
using Xunit;

namespace SingularityHub.Tests;

public sealed class LaboratoryCampaignExecutionGuiBuilderTests
{
    [Fact]
    public void WebPageProjectsEveryCampaignOutcomeAndSuccessfulRunLineageWithoutAuthorityClaims()
    {
        var started = DateTimeOffset.Parse("2026-11-20T10:00:00Z");
        var completed = DateTimeOffset.Parse("2026-11-20T10:00:05Z");
        var runId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var definition = new ExperimentDefinition(
            "electrical-thermal", "1.2.0", "Electrical thermal response",
            "electrical-thermal-coupling", "2.1.0",
            ScientificValidationLevel.EducationalModel,
            ["Ideal fixed-voltage DC source."], ["No spatial temperature gradients."]);
        var run = new ExperimentRunRecord(
            runId, "thermal-lab", "3.2.0", "electrical-thermal", "1.2.0",
            "electrical-thermal-coupling", "2.1.0", 1, "application/json", """{"voltageV":12}""",
            ScientificValidationLevel.EducationalModel, started, completed,
            "application/json", """{"temperatureC":42}""",
            ["Ideal source."], ["No spatial gradients."]);
        var baseline = new ExperimentCampaignScenario("baseline", "Baseline", ExperimentScenarioKind.Baseline,
            "Reference operating point.", new ExperimentConfiguration("electrical-thermal", "1.2.0", 1, "application/json", """{"voltageV":12}"""));
        var boundary = new ExperimentCampaignScenario("boundary", "Boundary", ExperimentScenarioKind.Boundary,
            "Probe upper operating limit.", new ExperimentConfiguration("electrical-thermal", "1.2.0", 1, "application/json", """{"voltageV":99}"""));
        var campaign = new ExperimentCampaignDefinition(
            "thermal-campaign", "1.0.0", "Explore declared voltage response.",
            "electrical-thermal", "1.2.0", "electrical-thermal-coupling", "2.1.0",
            ["Temperature remains within the declared operating envelope."], [baseline, boundary]);
        var execution = new ExperimentCampaignExecutionRecord(
            Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            "thermal-campaign", "1.0.0", campaign,
            "thermal-lab", "3.2.0", "electrical-thermal", "1.2.0",
            "electrical-thermal-coupling", "2.1.0",
            started, completed, ExperimentCampaignExecutionStatus.PartiallyCompleted,
            [
                new ExperimentCampaignScenarioResult("baseline", ExperimentCampaignScenarioStatus.Completed, run),
                new ExperimentCampaignScenarioResult("boundary", ExperimentCampaignScenarioStatus.Failed, null,
                    "System.InvalidOperationException", "The model does not support this condition.")
            ]);

        var root = LaboratoryCampaignExecutionGuiBuilder.Build([execution]);
        var campaignNode = root.Find("campaign-execution-" + execution.ExecutionId.ToString("N"));
        var baselineNode = campaignNode.Find("campaign-execution-" + execution.ExecutionId.ToString("N") + "-scenario-baseline");
        var boundaryNode = campaignNode.Find("campaign-execution-" + execution.ExecutionId.ToString("N") + "-scenario-boundary");

        Assert.Equal("1", root.Properties["data-campaign-execution-count"]);
        Assert.Equal("PartiallyCompleted", campaignNode.Properties["data-execution-status"]);
        Assert.Equal("thermal-campaign", campaignNode.Properties["data-campaign-id"]);
        Assert.Equal("1.0.0", campaignNode.Properties["data-campaign-version"]);
        Assert.Equal("2", campaignNode.Properties["data-scenario-count"]);
        Assert.Equal("Completed", baselineNode.Properties["data-scenario-status"]);
        Assert.Equal(runId.ToString("D"), baselineNode.Properties["data-run-id"]);
        Assert.Equal(run.ResultPayload, baselineNode.Properties["data-run-result-payload"]);
        Assert.Equal("Failed", boundaryNode.Properties["data-scenario-status"]);
        Assert.Equal("System.InvalidOperationException", boundaryNode.Properties["data-failure-type"]);
        Assert.Contains("does not support", boundaryNode.Properties["data-failure-message"]);
        Assert.DoesNotContain("data-execution-admitted", campaignNode.Properties.Keys);
        Assert.DoesNotContain("data-execution-started", campaignNode.Properties.Keys);
        Assert.DoesNotContain("data-access-granted", campaignNode.Properties.Keys);
    }

    [Fact]
    public void CampaignCatalogueRejectsDuplicateExecutionIdentities()
    {
        var campaign = new ExperimentCampaignDefinition(
            "c", "1", "Objective", "e", "1", "m", "1", ["criterion"],
            [
                new ExperimentCampaignScenario("base", "Baseline", ExperimentScenarioKind.Baseline, "Reference.",
                    new ExperimentConfiguration("e", "1", 1, "application/json", "{}")),
                new ExperimentCampaignScenario("nominal", "Nominal", ExperimentScenarioKind.Nominal, "Normal.",
                    new ExperimentConfiguration("e", "1", 1, "application/json", "{}"))
            ]);
        var now = DateTimeOffset.Parse("2026-11-20T10:00:00Z");
        var execution = new ExperimentCampaignExecutionRecord(
            Guid.NewGuid(), "c", "1", campaign, "lab", "1", "e", "1", "m", "1",
            now, now, ExperimentCampaignExecutionStatus.Failed,
            [
                new ExperimentCampaignScenarioResult("base", ExperimentCampaignScenarioStatus.Failed, null, "Failure", "Failed."),
                new ExperimentCampaignScenarioResult("nominal", ExperimentCampaignScenarioStatus.NotRun, null)
            ]);

        Assert.Throws<ArgumentException>(() => LaboratoryCampaignExecutionGuiBuilder.Build([execution, execution]));
    }
}
#endif
