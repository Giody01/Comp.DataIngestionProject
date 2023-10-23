namespace Comp.DataIngestionProject.DTO
{
    public class LoadData
    {
        public string HardwareType { get; set; }
        public string SensorName { get; set; }
        public string? LoadPercentage { get; set; }

    }
    public class LoadDataForServices
    {
        public Guid Id { get; set; }
        public List<LoadData> LoadData { get; set; }
        public string Src { get; set; }
    }
}