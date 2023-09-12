using Data.Entities;

namespace Data.Interfaces
{
    public interface IRefreshTokensRepository : IRepository<RefreshToken>
    {
        void Delete(RefreshToken refreshToken);
    }
}
