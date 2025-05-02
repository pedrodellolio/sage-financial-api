using SageFinancialAPI.Entities;
using SageFinancialAPI.Models;

namespace SageFinancialAPI.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDto request);
        Task<User?> GetAsync(Guid userId);
        Task<User?> GetByProfileIdAsync(Guid profileId);
        Task<User> SaveNotificationTokenAsync(string token, Guid userId);
        //Task<TokenResponseDto?> LoginAsync(UserDto request);
        // Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request);
    }
}