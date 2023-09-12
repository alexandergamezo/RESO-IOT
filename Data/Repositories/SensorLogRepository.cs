using Data.Data;
using Data.Entities;
using Data.Interfaces;

namespace Data.Repositories
{
    public class SensorLogRepository : ISensorLogRepository
    {
        private readonly SensorSystemDbContext _db;
        public SensorLogRepository(SensorSystemDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(SensorLog entity)
        {
            await _db.SensorLogRepository.AddAsync(entity);
        }

        public Task<IEnumerable<SensorLog>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}
