namespace Data.Interfaces
{
    public interface IUnitOfWork
    {
        ILightSensorRepository LightSensorRepository { get; }
        ILightSensorTelemetryRepository LightSensorTelemetryRepository { get; }
        ISensorLogRepository SensorLogRepository { get; }
        IRefreshTokensRepository RefreshTokensRepository { get; }

        Task SaveAsync();
    }
}
