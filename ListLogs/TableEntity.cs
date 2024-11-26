using Azure;
using Azure.Data.Tables;

namespace ListLogs
{
    public class TableEntity : ITableEntity
    {
        public TableEntity()
        {
        }

        public TableEntity(bool success)
        {
            Success = success;
        }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } 
        public string BlobName { get; set; }
    }
}
