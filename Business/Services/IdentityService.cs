using AutoMapper;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Business.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _automapperProfile;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly SignInManager<AppUser> _signInManager;

        public IdentityService(UserManager<AppUser> userManager, IMapper automapperProfile, IConfiguration configuration, IUnitOfWork unitOfWork, TokenValidationParameters tokenValidationParameters, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _automapperProfile = automapperProfile;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _tokenValidationParameters = tokenValidationParameters;
            _signInManager = signInManager;
        }

        public async Task<RegistrationResult> RegisterUserAsync(RegisterModel registerModel)
        {
            var userExists = await _userManager.FindByEmailAsync(registerModel.EmailAddress);
            if (userExists != null)
            {
                return new RegistrationResult
                {
                    Success = false,
                    ErrorMessage = $"User {registerModel.EmailAddress} already exists."
                };
            }

            var newUser = _automapperProfile.Map<AppUser>(registerModel);

            var result = await _userManager.CreateAsync(newUser, registerModel.Password);

            return new RegistrationResult
            {
                Success = result.Succeeded,
                ErrorMessage = result.Succeeded ? null : string.Join(", ", result.Errors.Select(e => e.Description))
            };
        }

        public async Task<AuthResultModel> LoginUserAsync(LoginModel loginModel)
        {
            var userExists = await _userManager.FindByEmailAsync(loginModel.EmailAddress);
            if (userExists != null && await _userManager.CheckPasswordAsync(userExists, loginModel.Password))
            {
                var tokenValue = await GenerateJWTTokenAsync(userExists, null, loginModel.RememberMe);

                return tokenValue;
            }

            return null;
        }

        private async Task<AuthResultModel> GenerateJWTTokenAsync(AppUser user, RefreshToken rToken, bool rememberMe)
        {
            var authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName ?? "DefaultUserName"),
                new Claim(ClaimTypes.NameIdentifier, user.Id ?? "DefaultUserId"),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? "DefaultEmail"),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email ?? "DefaultEmail"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            string? secretValue = _configuration["JWT:Secret"];
            SymmetricSecurityKey authSigninKey;

            if (secretValue != null)
            {
                authSigninKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretValue));
            }
            else
            {
                throw new InvalidOperationException("JWT:Secret is null");
            }


            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: DateTime.UtcNow.AddMinutes(10),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256));

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            if (rToken != null)
            {
                var rTokenResponse = new AuthResultModel()
                {
                    Token = jwtToken,
                    RefreshToken = rToken.Token,
                    ExpiresAt = token.ValidTo
                };

                return rTokenResponse;
            }

            RefreshToken? refreshToken = null;
            if (rememberMe)
            {
                refreshToken = new RefreshToken()
                {
                    JwtId = token.Id,
                    IsRevoked = false,
                    UserId = user.Id,
                    DateAdded = DateTime.UtcNow,
                    DateExpire = DateTime.UtcNow.AddMonths(6),
                    Token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString()
                };

                await _unitOfWork.RefreshTokensRepository.AddAsync(refreshToken);
                await _unitOfWork.SaveAsync();
            }

            var response = new AuthResultModel()
            {
                Token = jwtToken,
                RefreshToken = refreshToken?.Token,
                ExpiresAt = token.ValidTo
            };

            return response;
        }

        public async Task<AuthResultModel> VerifyAndGenerateTokenAsync(TokenRequestModel tokenRequestModel)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var storedToken = (await _unitOfWork.RefreshTokensRepository.GetAllAsync()).FirstOrDefault(rt => rt.Token == tokenRequestModel.RefreshToken);
            var dbUser = storedToken?.UserId != null ? await _userManager.FindByIdAsync(storedToken.UserId) : null;

            try
            {
                jwtTokenHandler.ValidateToken(tokenRequestModel.Token, _tokenValidationParameters, out var validatedToken);
                return await GenerateJWTTokenAsync(dbUser, storedToken, true);
            }
            catch (SecurityTokenExpiredException)
            {
                if (storedToken.DateExpire >= DateTime.UtcNow)
                {
                    return await GenerateJWTTokenAsync(dbUser, storedToken, true);
                }
                else
                {
                    return await GenerateJWTTokenAsync(dbUser, null, true);
                }
            }
        }

        public async Task SignOutAsync(TokenRequestModel tokenRequestModel)
        {
            var refreshToken = (await _unitOfWork.RefreshTokensRepository.GetAllAsync()).FirstOrDefault(rt => rt.Token == tokenRequestModel.Token);

            if (refreshToken != null)
            {
                _unitOfWork.RefreshTokensRepository.Delete(refreshToken);
                await _unitOfWork.SaveAsync();
            }

            await _signInManager.SignOutAsync();
        }
    }
}
