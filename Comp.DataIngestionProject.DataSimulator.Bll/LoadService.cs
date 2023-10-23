using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using LibreHardwareMonitor.Hardware;
using System;
using System.Text;
using Comp.DataIngestionProject.DataSimulator.DTO;
using System.Text.Json;
using Azure.Messaging.EventHubs.Producer;
using Azure.Messaging.EventHubs;

namespace Comp.DataIngestionProject.DataSimulator.Bll
{
    public class LoadService
    {
        public string GetDeviceConnectionString()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.json");
            var config = configuration.Build();
            var connectionString = config.GetConnectionString("IoTHubDeviceConnectionString");
            return connectionString;
        }
        public (string,string) GetEventHubCredentials()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.json");
            var config = configuration.Build();
            var connectionString = config.GetConnectionString("EventHubConnectionString");
            var eventHubName = config.GetConnectionString("EventHubName");
            return (connectionString, eventHubName);
        }
        public string GetLoadData()
        {
            LoadDatas loadDatas = new LoadDatas();
            List<LoadData> listLoadData = new List<LoadData>();
            Computer computer = new Computer
            {
                IsCpuEnabled = true,
                IsMemoryEnabled = true,
            };

            computer.Open();

            foreach (var hardwareItem in computer.Hardware)
            {
                hardwareItem.Update();
                foreach (var sensor in hardwareItem.Sensors)
                {
                    if (sensor.SensorType == SensorType.Load && sensor.Value.HasValue)
                    {
                        var loadData = new LoadData
                        {
                            HardwareType = hardwareItem.HardwareType.ToString(),
                            SensorName = sensor.Name,
                            LoadPercentage = $"{sensor.Value} %",
                        };
                        listLoadData.Add(loadData);
                    }
                }
            }
            loadDatas = new LoadDatas
            {
                Id = Guid.NewGuid(),
                LoadData = listLoadData
            };
            var stringedLoadDatas = JsonSerializer.Serialize(loadDatas);
            computer.Close();
            return stringedLoadDatas;
        }
        public async Task SendLoadDataToIoTHub(string loadDatas)
        {
            string iotConnectionString = GetDeviceConnectionString();
            DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(iotConnectionString, TransportType.Mqtt);
            byte[] loadDataBytes = Encoding.UTF8.GetBytes(loadDatas);
            string loadDataBase64String = Convert.ToBase64String(loadDataBytes);
            Message message = new Message(Encoding.UTF8.GetBytes(loadDataBase64String));
            await deviceClient.SendEventAsync(message);
        }
        public async Task SendLoadDataToEventHub(string loadDatas)
        {
            (string connectionString, string eventHubName) = GetEventHubCredentials();
            byte[] loadDataBytes = Encoding.UTF8.GetBytes(loadDatas);
            string loadDataBase64String = Convert.ToBase64String(loadDataBytes);
            await using (var producerClient = new EventHubProducerClient(connectionString, eventHubName))
            {
                var eventBatch = await producerClient.CreateBatchAsync();
                eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(loadDataBase64String)));
                await producerClient.SendAsync(eventBatch);
            }
        }
    }
}