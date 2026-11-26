#if WORKSHOP_USE_REPOSITORY_SOURCES
using System.Globalization;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;
using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Projects Laboratory-owned completed-run views into WebPage's platform-neutral GUI tree.
/// The immutable run record remains the scientific provenance source; WebPage only presents it.
/// </summary>
public static class LaboratoryExperimentResultGuiBuilder
{
    /// <summary>Builds a read-only result catalogue from Laboratory result views.</summary>
    public static GuiNode Build(IReadOnlyList<ExperimentResultView> results)
    {
        ArgumentNullException.ThrowIfNull(results);
        if (results.Any(result => result is null))
            throw new ArgumentException("Result views cannot contain null entries.", nameof(results));
        if (results.Select(result => result.RunId).Distinct().Count() != results.Count)
            throw new ArgumentException("Result run identities must be unique.", nameof(results));

        var root = GuiBuilder.Create("panel", "laboratory-experiment-results")
            .Text("Experiment results")
            .Property("data-result-count", results.Count.ToString(CultureInfo.InvariantCulture));

        foreach (var result in results.OrderBy(item => item.CompletedAtUtc).ThenBy(item => item.RunId))
        {
            var run = result.Run;
            var key = "experiment-run-" + result.RunId.ToString("N", CultureInfo.InvariantCulture);
            root.Child("panel", key, node =>
            {
                node.Text(result.ExperimentId)
                    .Property("data-run-id", result.RunId.ToString("D"))
                    .Property("data-laboratory-id", result.LaboratoryId)
                    .Property("data-laboratory-version", run.LaboratoryVersion)
                    .Property("data-experiment-id", result.ExperimentId)
                    .Property("data-experiment-version", result.ExperimentVersion)
                    .Property("data-model-id", result.ModelId)
                    .Property("data-model-version", result.ModelVersion)
                    .Property("data-validation-level", result.ValidationLevel.ToString())
                    .Property("data-started-at-utc", result.StartedAtUtc.ToString("O", CultureInfo.InvariantCulture))
                    .Property("data-completed-at-utc", result.CompletedAtUtc.ToString("O", CultureInfo.InvariantCulture))
                    .Property("data-elapsed", result.Elapsed.ToString("c", CultureInfo.InvariantCulture))
                    .Property("data-configuration-schema-version", run.ConfigurationSchemaVersion.ToString(CultureInfo.InvariantCulture))
                    .Property("data-configuration-content-type", run.ConfigurationContentType)
                    .Property("data-configuration-payload", run.ConfigurationPayload)
                    .Property("data-result-content-type", result.ContentType)
                    .Property("data-result-payload", result.Payload);

                if (result.ReplayedFromRunId is { } sourceRunId)
                    node.Property("data-replayed-from-run-id", sourceRunId.ToString("D"));

                node.Child("text", key + "-assumptions", text =>
                    text.Text("Assumptions: " + string.Join(" | ", result.Assumptions)));
                node.Child("text", key + "-limitations", text =>
                    text.Text("Limitations: " + string.Join(" | ", result.Limitations)));
            });
        }

        return root.Build();
    }
}
#endif
