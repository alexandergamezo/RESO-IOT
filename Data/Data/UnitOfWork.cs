using Data.Interfaces;
using Data.Repositories;

namespace Data.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SensorSystemDbContext _db;
        public UnitOfWork(SensorSystemDbContext db)
        {
            _db = db;
        }

        public ILightSensorRepository LightSensorRepository => new LightSensorRepository(_db);
        public ILightSensorTelemetryRepository LightSensorTelemetryRepository => new LightSensorTelemetryRepository(_db);
        public ISensorLogRepository SensorLogRepository => new SensorLogRepository(_db);
        public IRefreshTokensRepository RefreshTokensRepository => new RefreshTokensRepository(_db);

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
