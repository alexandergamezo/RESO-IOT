namespace Data.Entities
{
    public class SensorLog
    {
        public int SensorLogId { get; set; }
        public string? UserId { get; set; }
        public int DeviceId { get; set; }
        public string? Message { get; set; }
    }
}
