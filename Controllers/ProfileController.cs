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
    public class ProfileController(IProfileService profileService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<Profile?>> Get(Guid profileId)
        {
            try
            {
                var result = await profileService.GetAsync(profileId);
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
                var result = await profileService.GetAllAsync();
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
                    return Conflict("Já existe um perfil com esse título.");

                Profile profile = await profileService.PostAsync(request);
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

        [HttpPut]
        public async Task<ActionResult<Profile>> Put(ProfileUpdateDto request)
        {
            try
            {
                var profileDb = await profileService.GetAsync(request.Id);
                if (profileDb is null)
                    return NotFound("Perfil não encontrado.");

                profileDb.Title = request.Title;
                profileDb.IsActive = request.IsActive;

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

        [HttpDelete]
        public async Task<ActionResult<bool>> Delete(Guid profileId)
        {
            try
            {
                var deleted = await profileService.DeleteAsync(profileId);
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