namespace Sensor.Settings
{
    public class AppSettings
    {
        public string? ServerUrl { get; set; }
        public string? DeviceId { get; set; }
        public double IntervalMinutes { get; set; }
        public int MaxDataSentCount { get; set; }
    }
}
