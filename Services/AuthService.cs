using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SageFinancialAPI.Data;
using SageFinancialAPI.Entities;
using SageFinancialAPI.Models;

namespace SageFinancialAPI.Services
{
    public class AuthService(AppDbContext context) : IAuthService
    {
        // public async Task<TokenResponseDto?> LoginAsync(UserDto request)
        // {
        //     var user = await context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
        //     if (user is null)
        //         return null;
        //     if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
        //         return null;

        //     return await CreateTokenResponse(user);
        // }

        // private async Task<TokenResponseDto?> CreateTokenResponse(User? user)
        // {
        //     if (user is null)
        //         return null;

        //     return new TokenResponseDto
        //     {
        //         AccessToken = CreateToken(user),
        //         RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
        //     };
        // }

        public async Task<User?> RegisterAsync(UserDto request)
        {
            if (await context.Users.AnyAsync(u => u.Email == request.Email))
                return null;

            var user = new User
            {
                Id = request.UserId,
                Email = request.Email
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> GetAsync(Guid userId)
        {
            return await context.Users.FindAsync(userId);
        }

        public async Task<User?> GetByProfileIdAsync(Guid profileId)
        {
            return await context.Users
                .Include(u => u.Profiles)
                .FirstOrDefaultAsync(u => u.Profiles.Any(p => p.Id == profileId));
        }

        public async Task<User> SaveNotificationTokenAsync(string token, Guid userId)
        {
            var user = await GetAsync(userId);
            if (user is null)
                throw new ApplicationException("Usuário não encontrado");

            user.PushNotificationsToken = token;
            context.Users.Update(user);
            await context.SaveChangesAsync();
            return user;
        }

        // private string CreateToken(User user)
        // {
        //     var claims = new List<Claim>{
        //         new(ClaimTypes.Name, user.Username),
        //         new(ClaimTypes.NameIdentifier, user.Id.ToString())
        //     };

        //     var key = new SymmetricSecurityKey(
        //         Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

        //     var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        //     var tokenDescriptor = new JwtSecurityToken(
        //         issuer: configuration.GetValue<string>("AppSettings:Issuer"),
        //         audience: configuration.GetValue<string>("AppSettings:Audience"),
        //         claims: claims,
        //         expires: DateTime.UtcNow.AddSeconds(30),
        //         signingCredentials: credentials
        //     );

        //     return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        // }

        // public async Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request)
        // {
        //     var user = await ValidateRefreshTokenAsync(request.RefreshToken);
        //     if (user is null)
        //         return null;

        //     return await CreateTokenResponse(user);
        // }

        // private async Task<User?> ValidateRefreshTokenAsync(string refreshToken)
        // {
        //     var user = await context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        //     if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        //         return null;

        //     return user;
        // }

        // private static string GenerateRefreshToken()
        // {
        //     var randomNumber = new byte[32];
        //     using var rng = RandomNumberGenerator.Create();
        //     rng.GetBytes(randomNumber);
        //     return Convert.ToBase64String(randomNumber);
        // }

        // private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
        // {
        //     var refreshToken = GenerateRefreshToken();
        //     user.RefreshToken = refreshToken;
        //     user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        //     await context.SaveChangesAsync();
        //     return refreshToken;
        // }
    }
}