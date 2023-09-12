using AutoMapper;
using Business.Interfaces;
using Business.Models;
using Business.Validation;
using Data.Entities;
using Data.Interfaces;

namespace Business.Services
{
    public class LightSensorTelemetryService : ILightSensorTelemetry
    {
        private readonly IUnitOfWork _unitOfwork;
        private readonly IMapper _automapperProfile;

        public LightSensorTelemetryService(IUnitOfWork unitOfWork, IMapper automapperProfile)
        {
            _unitOfwork = unitOfWork;
            _automapperProfile = automapperProfile;            
        }

        public async Task AddAsync(IEnumerable<LightSensorTelemetryModel> listModel)
        {
            foreach(var model in listModel)
            {
                ValidateModel(model);

                var telemetry = _automapperProfile.Map<LightSensorTelemetry>(model);
                await _unitOfwork.LightSensorTelemetryRepository.AddAsync(telemetry);

                await _unitOfwork.SaveAsync();
            }
        }

        public async Task<IEnumerable<LightSensorTelemetryModel>> GetAllAsync()
        {
            var lightSensorTelemetries = await _unitOfwork.LightSensorTelemetryRepository.GetAllAsync();
            var lightSensorTelemetryModels = _automapperProfile.Map<IEnumerable<LightSensorTelemetryModel>>(lightSensorTelemetries);

            return lightSensorTelemetryModels;
        }

        private static void ValidateModel(LightSensorTelemetryModel model)
        {
            if (model == null)
            {
                throw new SensorException("Telemetry does not exist.");
            }
        }
    }
}
