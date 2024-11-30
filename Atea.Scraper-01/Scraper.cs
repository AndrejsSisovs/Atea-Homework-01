using System.Net.Http.Json;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Atea.Scraper_01
{
    public class Scraper
    {
        private readonly ILogger _logger;

        public Scraper(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Scraper>();
        }

        [Function("Function1")]
        public async Task Run([TimerTrigger("*/1 * * * *")] TimerInfo myTimer)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://restcountries.com");
            string logMessage;

            try
            {
                var response = await client.GetAsync("/v3.1/lang/spanish");

                if (!response.IsSuccessStatusCode)
                {
                    logMessage = $"error status code: {response.StatusCode}";
                    await LogToTableStorage(false, logMessage);
                    return;
                }

                var result = await response.Content.ReadFromJsonAsync<ICollection<Country>>();

                var blobServiceClient = new BlobServiceClient("UseDevelopmentStorage=true");
                var blobContainerClient = blobServiceClient.GetBlobContainerClient("atea");

                await blobContainerClient.CreateIfNotExistsAsync();

                var blobName = $"{Guid.NewGuid()}.json";
                var blobClient = blobContainerClient.GetBlobClient(blobName);
                await blobClient.UploadAsync(BinaryData.FromObjectAsJson(result));

                logMessage = "Data successfully fetched and stored.";
                await LogToTableStorage(true, logMessage, blobName);
            }
            catch (Exception ex)
            {
                logMessage = $"error: {ex.Message}";
                await LogToTableStorage(false, logMessage);
            }

            _logger.LogInformation(logMessage);
        }

        private async Task LogToTableStorage(bool success, string message, string blobName = null)
        {
            var tableServiceClient = new TableServiceClient("UseDevelopmentStorage=true");
            var tableClient = tableServiceClient.GetTableClient("atea");

            await tableServiceClient.CreateTableIfNotExistsAsync("atea");

            var key = Guid.NewGuid();
            await tableClient.AddEntityAsync(new TableEntity
            {
                PartitionKey = key.ToString(),
                RowKey = key.ToString(),
                Timestamp = DateTimeOffset.UtcNow,
                Success = success,
                Message = message,
                BlobName = blobName
            });
        }
    }
}

