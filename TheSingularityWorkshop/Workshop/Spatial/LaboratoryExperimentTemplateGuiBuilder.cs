#if WORKSHOP_USE_REPOSITORY_SOURCES
using System.Globalization;
using System.Text.Json;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;
using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Projects a Laboratory-owned authoring template into WebPage's platform-neutral GUI tree.
/// The template remains authoritative for experiment identity, parameter schema and defaults.
/// This adapter does not evaluate host admission policy or invoke a scientific model.
/// </summary>
public static class LaboratoryExperimentTemplateGuiBuilder
{
    /// <summary>Builds a read-only authoring surface from one version-bound template.</summary>
    public static GuiNode Build(ExperimentAuthoringTemplate template)
    {
        ArgumentNullException.ThrowIfNull(template);

        var experiment = template.Experiment;
        var schema = template.ParameterSchema;
        var configuration = template.DefaultConfiguration;
        using var defaults = JsonDocument.Parse(configuration.Payload);

        var root = GuiBuilder.Create("panel", "laboratory-experiment-template")
            .Text(template.Name)
            .Property("data-template-id", template.Id)
            .Property("data-template-version", template.Version)
            .Property("data-experiment-id", experiment.Id)
            .Property("data-experiment-version", experiment.Version)
            .Property("data-model-id", experiment.ModelId)
            .Property("data-model-version", experiment.ModelVersion)
            .Property("data-validation-level", experiment.ValidationLevel.ToString())
            .Property("data-configuration-schema-version", configuration.SchemaVersion.ToString(CultureInfo.InvariantCulture))
            .Property("data-configuration-content-type", configuration.ContentType)
            .Property("data-default-configuration", configuration.Payload);

        root.Child("text", "experiment-assumptions", node =>
            node.Text("Assumptions: " + string.Join(" | ", experiment.Assumptions)));
        root.Child("text", "experiment-limitations", node =>
            node.Text("Limitations: " + string.Join(" | ", experiment.Limitations)));

        if (experiment.ValidationAssessment is { } assessment)
        {
            root.Child("text", "experiment-validation-assessment", node =>
                node.Text("Assessment: " + assessment.State)
                    .Property("data-assessment-state", assessment.State.ToString())
                    .Property("data-assessment-reviewer", assessment.Reviewer ?? string.Empty)
                    .Property("data-assessment-at-utc", assessment.AssessedAtUtc.ToString("O", CultureInfo.InvariantCulture))
                    .Property("data-assessment-evidence-count", assessment.Evidence.Count.ToString(CultureInfo.InvariantCulture)));
        }

        if (experiment.SafetyMetadata is { } safety)
        {
            root.Child("panel", "experiment-safety-disclosures", node =>
            {
                node.Text("Safety disclosures")
                    .Property("data-risk-level", safety.RiskLevel.ToString());
                node.Child("text", "experiment-hazards", text =>
                    text.Text("Hazards: " + string.Join(" | ", safety.Hazards)));
                node.Child("text", "experiment-precautions", text =>
                    text.Text("Precautions: " + string.Join(" | ", safety.Precautions)));
                node.Child("text", "experiment-prohibited-uses", text =>
                    text.Text("Prohibited uses: " + string.Join(" | ", safety.ProhibitedUses)));
            });
        }

        root.Child("panel", "experiment-parameters", parameters =>
        {
            parameters.Text("Configuration parameters")
                .Property("data-schema-experiment-id", schema.ExperimentId)
                .Property("data-schema-experiment-version", schema.ExperimentVersion)
                .Property("data-schema-version", schema.SchemaVersion.ToString(CultureInfo.InvariantCulture));

            foreach (var parameter in schema.Parameters)
            {
                parameters.Child("input", parameter.Id, input =>
                {
                    input.Text(parameter.Name)
                        .Property("data-parameter-id", parameter.Id)
                        .Property("data-parameter-type", parameter.Type.ToString())
                        .Property("data-required", parameter.Required.ToString().ToLowerInvariant())
                        .Property("data-description", parameter.Description);

                    if (parameter.UnitSymbol is not null)
                        input.Property("data-unit", parameter.UnitSymbol);
                    if (parameter.Minimum is { } minimum)
                        input.Property("data-minimum", minimum.ToString("R", CultureInfo.InvariantCulture));
                    if (parameter.Maximum is { } maximum)
                        input.Property("data-maximum", maximum.ToString("R", CultureInfo.InvariantCulture));
                    if (parameter.AllowedValues.Count > 0)
                        input.Property("data-allowed-values", JsonSerializer.Serialize(parameter.AllowedValues));

                    if (defaults.RootElement.TryGetProperty(parameter.Id, out var value))
                        input.Property("data-default-value", value.GetRawText());
                });
            }
        });

        foreach (var dependency in experiment.PhysicsDependencies)
        {
            root.Child("text", "physics-dependency-" + dependency.CapabilityId, node =>
                node.Text(dependency.Purpose)
                    .Property("data-physics-capability-id", dependency.CapabilityId)
                    .Property("data-physics-domain", dependency.Domain)
                    .Property("data-physics-model-id", dependency.ModelId)
                    .Property("data-physics-model-version", dependency.ModelVersion));
        }

        foreach (var requirement in experiment.MicroBundleRequirements)
        {
            root.Child("text", "microbundle-requirement-" + requirement.BundleId.ToString(CultureInfo.InvariantCulture), node =>
                node.Text(requirement.Purpose)
                    .Property("data-microbundle-id", requirement.BundleId.ToString(CultureInfo.InvariantCulture))
                    .Property("data-microbundle-version", requirement.Version)
                    .Property("data-microbundle-provider", requirement.ProviderId));
        }

        return root.Build();
    }
}
#endif
