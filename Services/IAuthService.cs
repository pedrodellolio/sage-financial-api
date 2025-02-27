using SageFinancialAPI.Entities;
using SageFinancialAPI.Models;

namespace SageFinancialAPI.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDto request);
        // Task<TokenResponseDto?> LoginAsync(UserDto request);
        // Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request);
    }
}