using Business.Models;

namespace Business.Interfaces
{
    public interface ISensorLogger : ICrud<ISensorLogger>
    {
        Task AddLogAsync(SensorLogModel model);
    }
}
