using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ListLogs
{
    public class GetBlobPayloadFunction
    {
        private readonly ILogger<GetBlobPayloadFunction> _logger;

        public GetBlobPayloadFunction(ILogger<GetBlobPayloadFunction> logger)
        {
            _logger = logger;
        }

        [Function("GetBlobPayload")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "payload/{rowKey}")] HttpRequest req,
            string rowKey)
        {

            string tableConnectionString = "UseDevelopmentStorage=true";
            string blobConnectionString = "UseDevelopmentStorage=true";

            try
            {
                
                var tableClient = new TableServiceClient(tableConnectionString)
                    .GetTableClient("atea");

                var logEntry = await tableClient.GetEntityAsync<TableEntity>(rowKey, rowKey);

                if (logEntry == null)
                {
                    _logger.LogWarning($"Log entry with RowKey '{rowKey}' not found.");
                    return new NotFoundObjectResult("Log entry not found.");
                }

                string blobName = logEntry.Value.BlobName;

                if (string.IsNullOrEmpty(blobName))
                {
                    _logger.LogWarning($"Log entry with RowKey '{rowKey}' has an invalid or empty BlobName.");
                    return new BadRequestObjectResult("BlobName is empty or invalid.");
                }

                var blobClient = new BlobServiceClient(blobConnectionString)
                    .GetBlobContainerClient("atea")
                    .GetBlobClient(blobName);

                if (!await blobClient.ExistsAsync())
                {
                    _logger.LogWarning($"Blob '{blobName}' not found.");
                    return new NotFoundObjectResult("Blob not found.");
                }

                var downloadResponse = await blobClient.DownloadContentAsync();

                
                return new OkObjectResult(downloadResponse.Value.Content.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching blob payload: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
