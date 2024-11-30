using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ListLogs
{
    public class GetBlob
    {
        private readonly ILogger<GetBlob> _logger;

        public GetBlob(ILogger<GetBlob> logger)
        {
            _logger = logger;
        }

        [Function("GetBlob")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "payload/{rowKey}")] HttpRequest req,string rowKey)
        {
            string tableConnectionString = "UseDevelopmentStorage=true";
            string blobConnectionString = "UseDevelopmentStorage=true";

            var tableClient = new TableServiceClient(tableConnectionString).GetTableClient("atea");
            var logEntry = await tableClient.GetEntityAsync<TableEntity>(rowKey, rowKey);
            string blobName = logEntry.Value.BlobName;
            
            var blobClient = new BlobServiceClient(blobConnectionString)
                    .GetBlobContainerClient("atea")
                    .GetBlobClient(blobName);

             var bolbResponse = await blobClient.DownloadContentAsync();

             return new OkObjectResult(bolbResponse.Value.Content.ToString());
        }
    }
}
