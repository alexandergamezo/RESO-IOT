using Business.Models;
using Business.Services;
using System.Security.Claims;

namespace Business.Interfaces
{
    public interface IIdentityService
    {
        Task<RegistrationResult> RegisterUserAsync(RegisterModel registerModel);
        Task<AuthResultModel> LoginUserAsync(LoginModel loginModel);
        Task<AuthResultModel> VerifyAndGenerateTokenAsync(TokenRequestModel tokenRequestModel);
        Task SignOutAsync(TokenRequestModel tokenRequestModel);
    }
}
