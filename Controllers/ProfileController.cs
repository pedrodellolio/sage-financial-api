using Microsoft.AspNetCore.Mvc;
using SageFinancialAPI.Entities;
using SageFinancialAPI.Models;
using SageFinancialAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace SageFinancialAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController(IProfileService profileService) : BaseController
    {
        [HttpGet]
        public async Task<ActionResult<Profile?>> Get()
        {
            try
            {
                var result = await profileService.GetAsync(ProfileId);
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return BadRequest("Ocorreu um erro inesperado.");
            }
        }

         [HttpGet("default")]
        public async Task<ActionResult<Profile?>> GetDefault()
        {
            try
            {
                var result = await profileService.GetDefaultAsync(UserId);
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return BadRequest("Ocorreu um erro inesperado.");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<ICollection<Profile>>> GetAll()
        {
            try
            {
                var result = await profileService.GetAllAsync(UserId);
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return BadRequest("Ocorreu um erro inesperado.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Profile>> Post(ProfileDto request)
        {
            try
            {
                var profileDb = await profileService.GetByTitleAsync(request.Title);
                if (profileDb is not null)
                    return Conflict("Já existe uma Profile com esse título.");

                Profile profile = await profileService.PostAsync(request, UserId);
                return Ok(profile);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest("Ocorreu um erro inesperado.");
            }
        }

        [HttpPut]
        public async Task<ActionResult<Profile>> Put(ProfileUpdateDto request)
        {
            try
            {
                var profileDb = await profileService.GetAsync(request.Id);
                if (profileDb is null)
                    return NotFound("Profile não encontrada.");

                var profile = await profileService.PutAsync(profileDb);
                return Ok(profile);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return BadRequest("Ocorreu um erro inesperado.");
            }
        }

        [HttpDelete("{profileId}")]
        public async Task<ActionResult<bool>> Delete()
        {
            try
            {
                var profileDb = await profileService.GetAsync(ProfileId);
                if (profileDb is null)
                    return NotFound("Profile não encontrada.");

                var deleted = await profileService.DeleteAsync(profileDb);
                return Ok(deleted);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return BadRequest("Ocorreu um erro inesperado.");
            }
        }
    }
}