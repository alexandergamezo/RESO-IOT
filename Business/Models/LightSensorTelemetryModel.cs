using System.Text.Json.Serialization;

namespace Business.Models
{
    public class LightSensorTelemetryModel
    {
        [JsonPropertyName("illum")]
        public double Illuminance { get; set; }

        [JsonPropertyName("time")]
        public long Timestamp { get; set; }

        [JsonIgnore]
        public int LightSensorId { get; set; }
    }
}
