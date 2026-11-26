#if WORKSHOP_USE_REPOSITORY_SOURCES
using System.Globalization;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;
using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Projects Laboratory-owned immutable campaign execution envelopes into WebPage's read-only GUI.
/// Laboratory remains authoritative for campaign definitions, scenario outcomes, and run provenance.
/// </summary>
public static class LaboratoryCampaignExecutionGuiBuilder
{
    public static GuiNode Build(IReadOnlyList<ExperimentCampaignExecutionRecord> executions)
    {
        ArgumentNullException.ThrowIfNull(executions);
        if (executions.Any(execution => execution is null))
            throw new ArgumentException("Campaign executions cannot contain null entries.", nameof(executions));
        if (executions.Select(execution => execution.ExecutionId).Distinct().Count() != executions.Count)
            throw new ArgumentException("Campaign execution identities must be unique.", nameof(executions));

        var root = GuiBuilder.Create("panel", "laboratory-campaign-executions")
            .Text("Experiment campaigns")
            .Property("data-campaign-execution-count", executions.Count.ToString(CultureInfo.InvariantCulture));

        foreach (var execution in executions
            .OrderBy(item => item.CompletedAtUtc)
            .ThenBy(item => item.ExecutionId))
        {
            var key = "campaign-execution-" + execution.ExecutionId.ToString("N", CultureInfo.InvariantCulture);
            root.Child("panel", key, node =>
            {
                node.Text(execution.CampaignDefinition.Objective)
                    .Property("data-execution-id", execution.ExecutionId.ToString("D"))
                    .Property("data-campaign-id", execution.CampaignId)
                    .Property("data-campaign-version", execution.CampaignVersion)
                    .Property("data-experiment-id", execution.ExperimentId)
                    .Property("data-experiment-version", execution.ExperimentVersion)
                    .Property("data-model-id", execution.ModelId)
                    .Property("data-model-version", execution.ModelVersion)
                    .Property("data-laboratory-id", execution.LaboratoryId)
                    .Property("data-laboratory-version", execution.LaboratoryVersion)
                    .Property("data-execution-status", execution.Status.ToString())
                    .Property("data-started-at-utc", execution.StartedAtUtc.ToString("O", CultureInfo.InvariantCulture))
                    .Property("data-completed-at-utc", execution.CompletedAtUtc.ToString("O", CultureInfo.InvariantCulture))
                    .Property("data-acceptance-criteria", string.Join(" | ", execution.CampaignDefinition.AcceptanceCriteria))
                    .Property("data-scenario-count", execution.Scenarios.Count.ToString(CultureInfo.InvariantCulture));

                foreach (var scenario in execution.Scenarios)
                {
                    var scenarioKey = key + "-scenario-" + scenario.ScenarioId;
                    var definition = execution.CampaignDefinition.Scenarios.Single(item => item.Id == scenario.ScenarioId);
                    node.Child("panel", scenarioKey, scenarioNode =>
                    {
                        scenarioNode.Text(definition.Name)
                            .Property("data-scenario-id", scenario.ScenarioId)
                            .Property("data-scenario-kind", definition.Kind.ToString())
                            .Property("data-scenario-rationale", definition.Rationale)
                            .Property("data-scenario-status", scenario.Status.ToString());

                        if (scenario.FailureType is not null)
                            scenarioNode.Property("data-failure-type", scenario.FailureType);
                        if (scenario.FailureMessage is not null)
                            scenarioNode.Property("data-failure-message", scenario.FailureMessage);

                        if (scenario.Run is { } run)
                        {
                            scenarioNode.Property("data-run-id", run.RunId.ToString("D"))
                                .Property("data-run-result-content-type", run.ResultContentType)
                                .Property("data-run-result-payload", run.ResultPayload)
                                .Property("data-run-validation-level", run.ValidationLevel.ToString())
                                .Property("data-run-started-at-utc", run.StartedAtUtc.ToString("O", CultureInfo.InvariantCulture))
                                .Property("data-run-completed-at-utc", run.CompletedAtUtc.ToString("O", CultureInfo.InvariantCulture));
                        }
                    });
                }
            });
        }

        return root.Build();
    }
}
#endif
