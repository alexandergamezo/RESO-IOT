using NLog;
using Sensor.Settings;
using System.Text;
using System.Text.Json;

namespace Sensor
{
    public class LightSensor
    {
        private readonly AppSettings _appSettings;
        private readonly SensorSettings _sensorSettings;
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public LightSensor(AppSettings appSettings, SensorSettings sensorSettings, ILogger logger)
        {
            _appSettings = appSettings;
            _sensorSettings = sensorSettings;
            _logger = logger;
            _httpClient = new HttpClient();
        }

        public async Task CollectAndSendDataAsync()
        {
            List<object> telemetryDataList = new List<object>();
            int dataSentCount = 0;

            while (dataSentCount < _appSettings.MaxDataSentCount)
            {
                try
                {
                    double illuminance = Math.Round(SimulateIlluminance(), 1);

                    var telemetryData = new
                    {
                        illum = illuminance,
                        time = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                    };

                    _logger.Info($"time: {telemetryData.time} illum: {telemetryData.illum}");

                    telemetryDataList.Add(telemetryData);

                    if (telemetryDataList.Count % 4 == 0)
                    {
                        string jsonTelemetry = JsonSerializer.Serialize(telemetryDataList);
                        await SendTelemetryDataAsync(jsonTelemetry);
                        telemetryDataList.Clear();
                        dataSentCount++;
                    }

                    await Task.Delay(TimeSpan.FromMinutes(_appSettings.IntervalMinutes));
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"Error: {ex.Message}");
                    await Task.Delay(TimeSpan.FromMinutes(_appSettings.IntervalMinutes));
                }
            }
        }

        private double SimulateIlluminance()
        {
            TimeSpan currentTime = DateTime.UtcNow.TimeOfDay;
            TimeSpan morningStart = new TimeSpan(6, 0, 0);
            TimeSpan eveningStart = new TimeSpan(18, 0, 0);

            if (currentTime >= morningStart && currentTime < eveningStart)
            {
                double minutesSinceMorningStart = currentTime.TotalMinutes - morningStart.TotalMinutes;
                double illuminance = _sensorSettings.MinIlluminance + 0.5 * (minutesSinceMorningStart / 15);
                return Math.Floor(illuminance * 2) / 2;
            }
            else
            {
                double minutesSinceEveningStart = currentTime.TotalMinutes - eveningStart.TotalMinutes;
                double illuminance = _sensorSettings.MaxIlluminance - 0.5 * (minutesSinceEveningStart / 15);
                return Math.Floor(illuminance * 2) / 2;
            }
        }

        private async Task SendTelemetryDataAsync(string jsonData)
        {
            string endpoint = _sensorSettings?.Endpoint?.Replace("{deviceId}", _appSettings?.DeviceId) ?? string.Empty;
            string serverUrl = _appSettings?.ServerUrl ?? string.Empty;

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync($"{serverUrl}{endpoint}", content);

            if (response.IsSuccessStatusCode)
            {
                _logger.Info("Telemetry data sent successfully.");
            }
            else
            {
                _logger.Info($"Failed to send telemetry data. Status Code: {response.StatusCode}");
            }
        }
    }
}