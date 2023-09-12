using AutoMapper;
using Business.Models;
using Data.Entities;

namespace Business
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<LightSensorTelemetry, LightSensorTelemetryModel>().ReverseMap();

            CreateMap<LightSensorTelemetry, IlluminanceStatisticsModel>()
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeSeconds(src.Timestamp).Date.ToString("yyyy-MM-dd")))
            .ForMember(dest => dest.MaxIlluminance, opt => opt.MapFrom(src => src.Illuminance));

            CreateMap<LightSensor, LightSensorModel>();
            
            CreateMap<SensorLogModel, SensorLog>()
            .ForMember(dest => dest.SensorLogId, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => (string)null))
            .ReverseMap();

            CreateMap<RegisterModel, AppUser>()
                .ForMember(au => au.UserName, rm => rm.MapFrom(src => src.UserName))
                .ForMember(au => au.Email, rm => rm.MapFrom(src => src.EmailAddress))
                .ForMember(au => au.FirstName, rm => rm.MapFrom(src => src.FirstName))
                .ForMember(au => au.LastName, rm => rm.MapFrom(src => src.LastName));
        }
    }
}
