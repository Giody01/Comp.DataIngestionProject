namespace Comp.DataIngestionProject.DataSimulator.DTO
{
    public class LoadData
    {
        public string HardwareType { get; set; }
        public string SensorName { get; set; }
        public string? LoadPercentage { get; set; }

    }
    public class LoadDatas
    {
        public Guid Id { get; set; }
        public List<LoadData> LoadData { get; set; }
    }
}