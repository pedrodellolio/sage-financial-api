using Microsoft.AspNetCore.Mvc;
using SageFinancialAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using SageFinancialAPI.Models;
using SageFinancialAPI.Services;

namespace SageFinancialAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LabelController(ILabelService labelService) : BaseController
    {
        [HttpGet("{labelId}")]
        public async Task<ActionResult<Label?>> Get(Guid labelId)
        {
            try
            {
                var result = await labelService.GetAsync(labelId);
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
        public async Task<ActionResult<ICollection<LabelDto>>> GetAll(int? month, int? year, bool fromBudgetGoal)
        {
            try
            {
                ICollection<LabelDto> result;

                if (month is not null && year is not null && fromBudgetGoal)
                    result = await labelService.GetAllNotInBudgetGoalAsync(month.Value, year.Value, ProfileId);
                else
                    result = await labelService.GetAllAsync(ProfileId);

                return Ok(result);
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

        [HttpPost]
        public async Task<ActionResult<Label>> Post(LabelDto request)
        {
            try
            {
                Label label = await labelService.PostAsync(request, ProfileId);
                return Ok(label);
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
        public async Task<ActionResult<Label>> Put(LabelUpdateDto request)
        {
            try
            {
                var labelDb = await labelService.GetAsync(request.Id);
                if (labelDb is null)
                    return NotFound("Label não encontrada.");

                labelDb.Title = request.Title;
                labelDb.ColorHex = request.ColorHex;
                labelDb.IsActive = request.IsActive;

                var label = await labelService.PutAsync(labelDb);
                return Ok(label);
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

        [HttpDelete("{labelId}")]
        public async Task<ActionResult<bool>> Delete(Guid labelId)
        {
            try
            {
                var labelDb = await labelService.GetAsync(labelId);
                if (labelDb is null)
                    return NotFound("Label não encontrada.");

                var deleted = await labelService.DeleteAsync(labelDb);
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