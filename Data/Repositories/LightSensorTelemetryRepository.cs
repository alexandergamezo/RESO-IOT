using Data.Data;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class LightSensorTelemetryRepository : ILightSensorTelemetryRepository
    {
        private readonly SensorSystemDbContext _db;
        public LightSensorTelemetryRepository(SensorSystemDbContext db)
        {
            _db = db;            
        }

        public async Task AddAsync(LightSensorTelemetry entity)
        {
            await _db.LightSensorTelemetries.AddAsync(entity);
        }

        public async Task<IEnumerable<LightSensorTelemetry>> GetAllAsync()
        {
            return await _db.LightSensorTelemetries.ToListAsync();
        }
    }
}
