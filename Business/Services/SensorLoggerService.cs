using AutoMapper;
using Business.Interfaces;
using Business.Models;
using Business.Validation;
using Data.Entities;
using Data.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Business.Services
{
    public class SensorLoggerService : ISensorLogger
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _automapperProfile;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public SensorLoggerService(IUnitOfWork unitOfWork,
            IMapper automapperProfile,
            IWebHostEnvironment env,
            IHttpContextAccessor httpContextAccessor,
            UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _automapperProfile = automapperProfile;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public Task AddAsync(IEnumerable<ISensorLogger> model)
        {
            throw new NotImplementedException();
        }

        public async Task AddLogAsync(SensorLogModel model)
        {
            ValidateModel(model);

            var log = _automapperProfile.Map<SensorLog>(model);
            if (_env.IsStaging())
            {
                var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
                var userId = currentUser?.Id;
                log.UserId = userId;
            }

            await _unitOfWork.SensorLogRepository.AddAsync(log);

            await _unitOfWork.SaveAsync();
        }

        private static void ValidateModel(SensorLogModel model)
        {
            if (model == null)
            {
                throw new SensorException("Log does not exist.");
            }
        }
    }
}
