namespace Data.Entities
{
    public class LightSensorTelemetry
    {
        public int LightSensorTelemetryId { get; set; }
        public int LightSensorId { get; set; }
        public double Illuminance { get; set; }
        public long Timestamp { get; set; }

        public LightSensor? lightSensor { get; set; }
    }
}
