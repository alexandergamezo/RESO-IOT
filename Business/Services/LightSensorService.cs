using AutoMapper;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;

namespace Business.Services
{
    public class LightSensorService : ILightSensor
    {
        private readonly IUnitOfWork _unitOfwork;

        public LightSensorService(IUnitOfWork unitOfWork)
        {
            _unitOfwork = unitOfWork;
        }

        public Task AddAsync(IEnumerable<LightSensorModel> model)
        {
            throw new NotImplementedException();
        }

        public async Task<int> CreateOrGetSensorAsync(int deviceId)
        {
            var existingSensors = await _unitOfwork.LightSensorRepository.GetAllAsync();
            var existingSensor = existingSensors.FirstOrDefault(s => s.DeviceId == deviceId);

            if (existingSensor != null)
            {
                return existingSensor.LightSensorId;
            }
            else
            {
                var newSensor = new LightSensor
                {
                    DeviceId = deviceId
                };

                await _unitOfwork.LightSensorRepository.AddAsync(newSensor);

                await _unitOfwork.SaveAsync();

                return newSensor.LightSensorId;
            }
        }

        public Task<IEnumerable<LightSensorModel>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<IlluminanceStatisticsModel>> GetFilteredMaximum(int deviceId)
        {
            var lightSensor = await _unitOfwork.LightSensorRepository.GetByIdWithDetailsAsync(deviceId);

            if (lightSensor == null || lightSensor.lightSensorTelemetries == null)
            {
                return Enumerable.Empty<IlluminanceStatisticsModel>();
            }

            var currentDate = DateTime.UtcNow.Date;
            var thirtyDaysAgo = currentDate.AddDays(-30);

            var statistics = lightSensor.lightSensorTelemetries
                .Where(telemetry => telemetry != null && DateTimeOffset.FromUnixTimeSeconds(telemetry.Timestamp).Date >= thirtyDaysAgo)
                .GroupBy(telemetry => telemetry != null ? DateTimeOffset.FromUnixTimeSeconds(telemetry.Timestamp).Date : default)
                .Select(group => new IlluminanceStatisticsModel
                {
                    Date = group.Key.ToString("yyyy-MM-dd"),
                    MaxIlluminance = group.Max(telemetry => telemetry != null ? telemetry.Illuminance : 0.0)
                })
                .OrderBy(stat => stat.Date);

            return statistics;
        }
    }
}
