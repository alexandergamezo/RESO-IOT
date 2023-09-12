namespace Data.Entities
{
    public class LightSensor
    {
        public int LightSensorId { get; set; }
        public int DeviceId { get; set; }
        public string? AppUserId { get; set; }
        public string? LightSensorName { get; set; }
        public string? Description { get; set; }

        public List<LightSensorTelemetry>? lightSensorTelemetries { get; set; }
    }
}
