using Data.Data;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class LightSensorRepository : ILightSensorRepository
    {
        private readonly SensorSystemDbContext _db;

        public LightSensorRepository(SensorSystemDbContext db)
        {
            _db = db;            
        }

        public async Task AddAsync(LightSensor entity)
        {
            await _db.LightSensors.AddAsync(entity);
        }

        public async Task<IEnumerable<LightSensor>> GetAllAsync()
        {
            return await _db.LightSensors.ToListAsync();
        }

        public async Task<LightSensor?> GetByIdWithDetailsAsync(int deviceId)
        {
            return await _db.LightSensors
                .Include(sensor => sensor.lightSensorTelemetries)
                .SingleOrDefaultAsync(sensor => sensor.DeviceId == deviceId);
        }
    }
}
