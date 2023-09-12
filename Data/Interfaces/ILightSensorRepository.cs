using Data.Entities;

namespace Data.Interfaces
{
    public interface ILightSensorRepository : IRepository<LightSensor>
    {
        public Task<LightSensor?> GetByIdWithDetailsAsync(int deviceId);
    }
}
