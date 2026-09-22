using System.Net;
using System.Text.Json;
using Azure;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace TheSingularityWorkshop.Api;

public sealed class WorldFunction
{
    private const string TableName = "WorkshopWorld";
    private const string StatePartition = "GLOBAL";
    private const string StateRow = "CURRENT";
    private const string EventPartition = "EVENTS";
    private readonly ILogger _logger;

    public WorldFunction(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<WorldFunction>();
    }

    [Function("GetWorldState")]
    public async Task<HttpResponseData> GetWorldState(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "world/state")] HttpRequestData req)
    {
        var table = await GetTableAsync();
        try
        {
            var entity = await table.GetEntityAsync<TableEntity>(StatePartition, StateRow);
            return await Json(req, WorkshopWorldDto.From(entity.Value));
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return await Json(req, WorkshopWorldDto.Initial());
        }
    }

    [Function("ActivateWorldSwitch")]
    public async Task<HttpResponseData> ActivateWorldSwitch(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "world/switch")] HttpRequestData req)
    {
        var submission = await JsonSerializer.DeserializeAsync<WorldSwitchSubmission>(
            req.Body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (submission is null ||
            string.IsNullOrWhiteSpace(submission.SwitchId) ||
            string.IsNullOrWhiteSpace(submission.LocationId))
            return req.CreateResponse(HttpStatusCode.BadRequest);

        var table = await GetTableAsync();
        var now = DateTimeOffset.UtcNow;
        var visitor = Normalize(submission.DiscovererId, "anonymous");

        var state = new TableEntity(StatePartition, StateRow)
        {
            ["PresentationMode"] = "ThreeDimensional",
            ["Epoch"] = 1L,
            ["LastEvent"] = "world.presentation.3d.unlock",
            ["SwitchId"] = submission.SwitchId,
            ["LocationId"] = submission.LocationId,
            ["DiscovererId"] = visitor,
            ["EventTimestampUtc"] = now
        };

        try
        {
            // Insert-only is intentional: the first successful discovery owns the transition.
            // Concurrent visitors therefore converge on one world event rather than racing
            // through read/modify/write logic.
            await table.AddEntityAsync(state);
            await RecordDiscoveryAsync(table, submission.SwitchId, submission.LocationId, visitor, now);

            return await Json(req, WorkshopWorldDto.From(state), HttpStatusCode.OK);
        }
        catch (RequestFailedException ex) when (ex.Status == 409)
        {
            _logger.LogInformation(
                "World switch {SwitchId} was already discovered; returning the existing world.",
                submission.SwitchId);

            var existing = await table.GetEntityAsync<TableEntity>(StatePartition, StateRow);
            return await Json(req, WorkshopWorldDto.From(existing.Value), HttpStatusCode.OK);
        }
    }

    private async Task<TableClient> GetTableAsync()
    {
        var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("AzureWebJobsStorage connection string is missing.");

        var table = new TableClient(connectionString, TableName);
        await table.CreateIfNotExistsAsync();
        return table;
    }

    private static async Task RecordDiscoveryAsync(
        TableClient table,
        string switchId,
        string locationId,
        string discovererId,
        DateTimeOffset occurredAtUtc)
    {
        var entity = new TableEntity(EventPartition, Guid.NewGuid().ToString("N"))
        {
            ["EventType"] = "world.presentation.3d.unlock",
            ["SwitchId"] = switchId,
            ["LocationId"] = locationId,
            ["DiscovererId"] = discovererId,
            ["OccurredAtUtc"] = occurredAtUtc
        };

        await table.AddEntityAsync(entity);
    }

    private static async Task<HttpResponseData> Json(HttpRequestData req, WorkshopWorldDto state, HttpStatusCode status = HttpStatusCode.OK)
    {
        var response = req.CreateResponse(status);
        await response.WriteAsJsonAsync(state);
        return response;
    }

    private static string Normalize(string? value, string fallback)
    {
        if (string.IsNullOrWhiteSpace(value)) return fallback;
        return value.Trim()[..Math.Min(value.Trim().Length, 96)];
    }

    private sealed record WorldSwitchSubmission(string SwitchId, string LocationId, string? DiscovererId);

    private sealed record WorkshopWorldDto(
        string PresentationMode,
        long Epoch,
        string? LastEvent,
        string? SwitchId,
        string? LocationId,
        string? DiscovererId,
        DateTimeOffset? EventTimestampUtc)
    {
        public static WorkshopWorldDto Initial()
            => new("TwoDimensional", 0, null, null, null, null, null);

        public static WorkshopWorldDto From(TableEntity entity)
            => new(
                entity.GetString("PresentationMode") ?? "TwoDimensional",
                entity.GetInt64("Epoch") ?? 0,
                entity.GetString("LastEvent"),
                entity.GetString("SwitchId"),
                entity.GetString("LocationId"),
                entity.GetString("DiscovererId"),
                entity.GetDateTimeOffset("EventTimestampUtc"));
    }
}
