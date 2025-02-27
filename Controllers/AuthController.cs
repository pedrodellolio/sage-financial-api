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
        // [HttpPost("login")]
        // public async Task<ActionResult<TokenResponseDto>> Login(UserDto request)
        // {
        //     var result = await authService.LoginAsync(request);
        //     if (result is null)
        //         return BadRequest("Invalid username or password");
        //     return Ok(result);
        // }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            var user = await authService.RegisterAsync(request);
            if (user is null)
                return BadRequest("Username already exists");
            return Ok(user);
        }
    }
}