using Microsoft.AspNetCore.Mvc;
using SageFinancialAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using SageFinancialAPI.Models;
using SageFinancialAPI.Services;
using System.Security.Claims;

namespace SageFinancialAPI.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected Guid UserId => GetUserId();
        protected Guid ProfileId => GetProfileId();

        private Guid GetUserId()
        {
            var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userIdClaim != null ? Guid.Parse(userIdClaim) : Guid.Empty;
        }

        private Guid GetProfileId()
        {
            var profileIdHeader = Request.Headers["X-Profile-Id"].FirstOrDefault();
            return profileIdHeader != null ? Guid.Parse(profileIdHeader) : Guid.Empty;
        }
    }
}