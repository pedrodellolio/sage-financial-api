using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SageFinancialAPI.Entities;
using SageFinancialAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using SageFinancialAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace SageFinancialAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(UserDto request)
        {
            var result = await authService.LoginAsync(request);
            if (result is null)
                return BadRequest("Invalid username or password");
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            var user = await authService.RegisterAsync(request);
            if (user is null)
                return BadRequest("Username already exists");
            return Ok(user);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await authService.RefreshTokensAsync(request);
            if (result is null || result.RefreshToken is null || result.AccessToken is null)
                return Unauthorized("Invalid refresh token");
            return Ok(result);
        }

        [Authorize]
        [HttpGet("auth-only")]
        public IActionResult AuthenticatedOnly()
        {
            return Ok("You're authenticated");
        }
    }
}