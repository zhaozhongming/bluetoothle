﻿using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Samples.Infrastructure
{    public class StorageHelper
    {
        public static async Task<TableResult> WriteData(Reading data)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=apflowmeter;AccountKey=LHkCqdje+RGjCjJII80MGcpg5XhqQDLeze3FQtXOgukg6rDjBlA1h3dHzos9iygYvqagrMzqUppUZmY6EoMC8g==;EndpointSuffix=core.windows.net");

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference("readings");
            // Create the TableOperation object that inserts the customer entity.
            data.PartitionKey = "Test Phase";
            data.RowKey = Guid.NewGuid().ToString();
            data.ReadingTime = DateTime.Now;

            TableOperation insertOperation = TableOperation.Insert(data);

            // Execute the insert operation.
            return await table.ExecuteAsync(insertOperation);
        }

        public static async Task<TableResult> WriteLog(string data)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=apflowmeter;AccountKey=LHkCqdje+RGjCjJII80MGcpg5XhqQDLeze3FQtXOgukg6rDjBlA1h3dHzos9iygYvqagrMzqUppUZmY6EoMC8g==;EndpointSuffix=core.windows.net");

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference("logs");
            // Create the TableOperation object that inserts the customer entity.
            Log log = new Log
            {
                PartitionKey = "Test Phase",
                RowKey = Guid.NewGuid().ToString(),
                Message = data
            };

            TableOperation insertOperation = TableOperation.Insert(log);

            // Execute the insert operation.
            return await table.ExecuteAsync(insertOperation);
        }
    }
}
