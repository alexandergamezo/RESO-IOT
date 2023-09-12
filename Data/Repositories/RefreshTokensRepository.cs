using Data.Data;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class RefreshTokensRepository : IRefreshTokensRepository
    {
        private readonly SensorSystemDbContext _db;

        public RefreshTokensRepository(SensorSystemDbContext context)
        {
            _db = context;            
        }

        public async Task AddAsync(RefreshToken entity)
        {
            await _db.RefreshTokens.AddAsync(entity);
        }

        public async void Delete(RefreshToken refreshToken)
        {
            if (await _db.RefreshTokens.FindAsync(refreshToken) == null) throw new ArgumentException("Invalid entity");
            await Task.Run(() => _db.RefreshTokens.Remove(refreshToken));
        }

        public async Task<IEnumerable<RefreshToken>> GetAllAsync()
        {
            return await _db.RefreshTokens.ToListAsync();
        }
    }
}
