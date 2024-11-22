using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atea.Scraper_01
{
    public class TableEntity : ITableEntity
    {
        public TableEntity(bool success) 
        {
            Success = success;
        }
        public string PartitionKey {  get; set; }
        public string RowKey {  get; set; }
        public DateTimeOffset? Timestamp {  get; set; }
        public ETag ETag {  get; set; }
        public bool Success { get; set; }
    }
}
