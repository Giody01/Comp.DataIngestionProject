using Comp.DataIngestionProject.DataSimulator.Bll;
using Comp.DataIngestionProject.DTO;
using Comp.DataIngestionProject.EHToBlobContainer.Bll;
using Comp.DataIngestionProject.EHToCosmosDB.Bll;
using Comp.DataIngestionProject.EHToSQL.Bll;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace Comp.ConsoleApp.DataIngestionProject.TestWriting
{
    public class Program
    {
        private static IServiceProvider _serviceProvider;

        public static     async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
            var loadService = _serviceProvider.GetService<LoadService>();
            var blobWriterService = _serviceProvider.GetService<BlobWriterService>();
            var cosmosWriterService = _serviceProvider.GetService<CosmosWriterService>();
            var sqlWriterService = _serviceProvider.GetService<SQLWriterService>();
            var loadDatasStringed = loadService.GetLoadData();
            var loadDataForServices = JsonSerializer.Deserialize<LoadDataForServices>(loadDatasStringed);
            await blobWriterService.BlobWriter(loadDataForServices);
            await cosmosWriterService.CosmosWriter(loadDataForServices);
            sqlWriterService.SQLWriter(loadDataForServices);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // register your services, e.g.:
            services.AddSingleton<LoadService>();
            services.AddSingleton<BlobWriterService>();
            services.AddSingleton<CosmosWriterService>();
            services.AddSingleton<SQLWriterService>();
        }
    }

}