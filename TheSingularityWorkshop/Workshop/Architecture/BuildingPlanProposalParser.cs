using System.Text.Json;
using System.Text.Json.Serialization;
using TheSingularityWorkshop.Workshop.MicroBundles;

namespace TheSingularityWorkshop.Workshop.Architecture;

/// <summary>
/// The result of interpreting an untrusted, model-facing building-plan proposal.
/// A successful parse is only schema validation; it is not building-code approval.
/// </summary>
public sealed record BuildingPlanProposalResult(
    bool IsValid,
    BuildingPlan? Plan,
    string Message);

/// <summary>
/// Parses clipboard or future optional-agent output into a candidate plan.
/// This class performs no network calls, persistence, or automatic application.
/// </summary>
public static class BuildingPlanProposalParser
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public static BuildingPlanProposalResult Parse(string response)
    {
        if (string.IsNullOrWhiteSpace(response))
            return Rejected("The response is empty.");

        try
        {
            var envelope = JsonSerializer.Deserialize<ProposalEnvelope>(response, JsonOptions);
            if (!string.Equals(envelope?.Kind, "building-plan-proposal", StringComparison.Ordinal)
                || envelope.Plan is null
                || envelope.Plan.Rooms is null
                || envelope.Plan.Walls is null
                || envelope.Plan.Openings is null)
            {
                return Rejected(
                    "Expected a building-plan-proposal JSON object containing a plan with rooms, walls, and openings.");
            }

            var data = envelope.Plan;
            var plan = new BuildingPlan(
                data.Id ?? string.Empty,
                data.Name ?? string.Empty,
                data.Width,
                data.Height,
                data.Rooms,
                data.Walls,
                data.Openings,
                data.Fixtures ?? Array.Empty<PlanFixture>());

            return new BuildingPlanProposalResult(
                true,
                plan,
                $"Validated proposal “{plan.Name}”: {plan.Rooms.Count} rooms, {plan.Walls.Count} walls, {plan.Openings.Count} openings. Review it before applying; validation covers only the plan schema's current geometric constraints.");
        }
        catch (JsonException ex)
        {
            return Rejected($"Response is not valid proposal JSON ({ex.Message}).");
        }
        catch (ArgumentException ex)
        {
            return Rejected($"BuildingPlan validation rejected the proposal: {ex.Message}");
        }
    }

    private static BuildingPlanProposalResult Rejected(string message) =>
        new(false, null, $"{message} The working plan was not changed.");

    private sealed class ProposalEnvelope
    {
        public string? Kind { get; set; }
        public ProposalPlan? Plan { get; set; }
    }

    private sealed class ProposalPlan
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public PlanRoom[]? Rooms { get; set; }
        public PlanWall[]? Walls { get; set; }
        public PlanOpening[]? Openings { get; set; }
        public PlanFixture[]? Fixtures { get; set; }
    }
}
