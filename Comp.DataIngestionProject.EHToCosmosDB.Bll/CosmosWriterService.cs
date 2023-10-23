using Comp.DataIngestionProject.DTO;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Comp.DataIngestionProject.EHToCosmosDB.Bll
{
    public class CosmosWriterService
    {
        public (string, string, string, string) GetCosmosCredentials()
        {
            var configuration = new ConfigurationBuilder().SetBasePath("C:\\Progetti\\Comp.DataIngestionProject").AddJsonFile("appsettings.json");
            var config = configuration.Build();
            string cosmosEndpointUri = config.GetSection("CosmosEndpointUri").Value;
            string cosmosPrimaryKey = config.GetSection("CosmosPrimaryKey").Value;
            string cosmosDatabaseId = config.GetSection("CosmosDatabaseId").Value;
            string cosmosContainerId = config.GetSection("CosmosContainerId").Value;
            return (cosmosEndpointUri, cosmosPrimaryKey, cosmosDatabaseId, cosmosContainerId);
        }

        public async Task CosmosWriter(LoadDataForServices loadDataForServices)
        {
            loadDataForServices.Src = "Cosmos Writer";
            (string cosmosEndpointUri, string cosmosPrimaryKey, string cosmosDatabaseId, string cosmosContainerId) = GetCosmosCredentials();
            CosmosClient cosmosClient = new CosmosClient(cosmosEndpointUri, cosmosPrimaryKey);
            Container container = cosmosClient.GetContainer(cosmosDatabaseId, cosmosContainerId);
            ItemResponse<LoadDataForServices> createResponse = await container.CreateItemAsync(loadDataForServices, new PartitionKey(loadDataForServices.Id.ToString()));

        }
    }
}