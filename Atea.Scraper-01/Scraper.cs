using System;
using System.Net.Http.Json;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Grpc.Core;
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

        //https://restcountries.com/v3.1/lang/spanish
        [Function("Function1")]
        public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://restcountries.com");
            var response = await client.GetAsync("/v3.1/lang/spanish");
            var result = await response.Content.ReadFromJsonAsync<ICollection<Country>>();
            var tableServiceClient = new TableServiceClient("UserDevelopmentStorage=true");
            await tableServiceClient.CreateTableIfNotExistsAsync("atea");
            var tableClient = tableServiceClient.GetTableClient("atea");
            var key = Guid.NewGuid();
            await tableClient.AddEntityAsync(new TableEntity(response.IsSuccessStatusCode)
            {
                PartitionKey = key.ToString(),
                RowKey = key.ToString(),
            });

            var blobServiceClient = new BlobServiceClient("UserDevelopmentStorage=true");
            var blobContainerClient = await blobServiceClient.CreateBlobContainerAsync("atea");
            await blobContainerClient.Value.CreateIfNotExistsAsync();
            await blobContainerClient.Value.UploadBlobAsync($"{key.ToString()}.json", BinaryData.FromObjectAsJson(result));

            var blob = blobContainerClient.Value.GetBlobClient("97f268db-e166-4ffe-b26e-834245d2c23b.json");
            var content = await blob.DownloadContentAsync();

            //content.Value.Content.ToObjectFromJson<>();

            _logger.LogInformation("action performed");
        }
    }
}
