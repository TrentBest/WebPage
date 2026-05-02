using System.Net;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace TheSingularityWorkshop.Api
{
    public class VoteFunction
    {
        private readonly ILogger _logger;
        private const string TableName = "NamingPollVotes";
        private const string PartitionKey = "OneGUI_V1";

        public VoteFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<VoteFunction>();
        }

        [Function("SubmitVote")]
        public async Task<HttpResponseData> SubmitVote(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "vote")] HttpRequestData req)
        {
            _logger.LogInformation("Processing vote...");

            string requestBody;
            try
            {
                requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error reading body: {ex.Message}");
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            VoteSubmission? voteData;
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                voteData = JsonSerializer.Deserialize<VoteSubmission>(requestBody, options);
            }
            catch
            {
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (voteData == null || string.IsNullOrWhiteSpace(voteData.SelectedOption))
            {
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            // Connect to Table Storage
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

            if (string.IsNullOrEmpty(connectionString))
            {
                _logger.LogError("AzureWebJobsStorage connection string is missing.");
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }

            var tableClient = new TableClient(connectionString, TableName);
            await tableClient.CreateIfNotExistsAsync();

            // Create Entity
            var entity = new TableEntity(PartitionKey, Guid.NewGuid().ToString())
            {
                { "Option", voteData.SelectedOption },
                { "WriteIn", voteData.WriteInValue ?? "" },
                { "Timestamp", DateTime.UtcNow }
            };

            await tableClient.AddEntityAsync(entity);

            return req.CreateResponse(HttpStatusCode.OK);
        }

        [Function("GetResults")]
        public async Task<HttpResponseData> GetResults(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "results")] HttpRequestData req)
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            var tableClient = new TableClient(connectionString, TableName);

            try
            {
                await tableClient.CreateIfNotExistsAsync();
            }
            catch
            {
                // Table might not exist yet if no votes cast; that's fine.
            }

            var stats = new Dictionary<string, int>();

            try
            {
                var query = tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{PartitionKey}'");

                await foreach (var vote in query)
                {
                    if (vote.TryGetValue("Option", out var val) && val is string option)
                    {
                        if (!stats.ContainsKey(option)) stats[option] = 0;
                        stats[option]++;
                    }
                }
            }
            catch
            {
                // Ignore query errors
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(stats);
            return response;
        }
    }

    // FIX: Added default values to satisfy Non-Nullable rules
    public class VoteSubmission
    {
        public string SelectedOption { get; set; } = string.Empty;
        public string WriteInValue { get; set; } = string.Empty;
    }
}