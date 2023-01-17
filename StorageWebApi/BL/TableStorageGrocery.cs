using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
//using Microsoft.WindowsAzure.Storage;
using Microsoft.Extensions.Configuration;
using StorageWebApi.Services;
//using Microsoft.WindowsAzure.Storage.Table;

namespace StorageWebApi.BL
{
    public class TableStorageGrocery :ITableStorage
    {
        private const string TableName = "Item";
        private readonly IConfiguration _configuration;
        public TableStorageGrocery(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private async Task<TableClient> GetTableClient()
        {
            var serviceClient = new TableServiceClient(_configuration.GetValue<string>("MyConfig:StorageConnection"));
            
            var tableClient = serviceClient.GetTableClient(TableName);
            await tableClient.CreateIfNotExistsAsync();
            return tableClient;
        }
        public async Task<GroceryItemEntity> GetEntityAsync(string name,string id)
        {
            var tableClient = await GetTableClient();
            return await tableClient.GetEntityAsync<GroceryItemEntity>(name, id);

        }
        public async Task<GroceryItemEntity> UpsertEntityAsync(GroceryItemEntity entity)
        {
            var tableClient = await GetTableClient();
            await tableClient.UpsertEntityAsync(entity);
            return entity;
        }
        public async Task DeleteEntityAsync(string name, string id)
        {
            var tableClient = await GetTableClient();
            await tableClient.DeleteEntityAsync(name, id);
        }

    }
    public class GroceryItemEntity : ITableEntity
    {

        public string Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get ; set; }
        public ETag ETag { get ; set; }

       
        //public GroceryItemEntity(string id, string name)
        //{
        //    PartitionKey = name;
        //    RowKey = id;
        //}
        //public string PartitionKey { get; set; }
        //public string RowKey { get; set; }
        //public DateTimeOffset? Timestamp { get; set; }
        //public ETag ETag { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
