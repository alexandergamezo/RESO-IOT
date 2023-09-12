using Business.Models;

namespace Business.Interfaces
{
    public interface ILightSensor : ICrud<LightSensorModel>
    {
        Task<int> CreateOrGetSensorAsync(int deviceId);
        Task<IEnumerable<IlluminanceStatisticsModel>> GetFilteredMaximum(int deviceId);
    }
}
