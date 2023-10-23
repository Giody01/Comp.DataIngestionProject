using Azure.Storage;
using Azure.Storage.Blobs;
using Comp.DataIngestionProject.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.Text.Json;

namespace Comp.DataIngestionProject.EHToBlobContainer.Bll
{
    public class BlobWriterService
    {
        public (string, string, string) GetBlobStorageCredentials()
        {
            var configuration = new ConfigurationBuilder().SetBasePath("C:\\Progetti\\Comp.DataIngestionProject").AddJsonFile("appsettings.json");
            var config = configuration.Build();
            var blobServiceEndpoint = config.GetSection("BlobServiceEndpoint").Value;
            var storageAccountName = config.GetSection("StorageAccountName").Value;
            var storageAccountKey = config.GetSection("StorageAccountKey").Value;
            return (blobServiceEndpoint, storageAccountName, storageAccountKey);
        }

        public async Task BlobWriter(LoadDataForServices loadDataForServices)
        {
            loadDataForServices.Src = "Blob Writer";
            string loadDataForServicesStringed = JsonSerializer.Serialize(loadDataForServices);
            (var blobServiceEndpoint, var storageAccountName, var storageAccountKey) = GetBlobStorageCredentials();
            string blobName = $"{loadDataForServices.Id}.json";

            //string containerName = "conteinerdataingestionproject";
            //string blobUri = $"{blobServiceEndpoint}/{containerName}/{blobName}{storageSasToken}";
            //BlobClient blobClient = new BlobClient(new Uri(blobUri));

            BlobServiceClient blobServiceClient = new BlobServiceClient(new Uri(blobServiceEndpoint), new StorageSharedKeyCredential(storageAccountName, storageAccountKey));
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("conteinerdataingestionproject");
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(loadDataForServicesStringed)));

        }
    }
}