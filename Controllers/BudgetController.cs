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
    public class BudgetController(IBudgetService budgetService) : BaseController
    {
        [HttpGet("{budgetId}")]
        public async Task<ActionResult<Budget?>> Get(Guid budgetId)
        {
            try
            {
                var result = await budgetService.GetAsync(budgetId);
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

        [HttpGet("period")]
        public async Task<ActionResult<Budget?>> GetByMonthAndYear(int month, int year)
        {
            try
            {
                var result = await budgetService.GetByMonthAndYearAsync(month, year, ProfileId);
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
        public async Task<ActionResult<ICollection<Budget>>> GetAll()
        {
            try
            {
                var result = await budgetService.GetAllAsync(ProfileId);
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
        public async Task<ActionResult<Budget>> Post(BudgetDto request)
        {
            try
            {
                Budget budget = await budgetService.PostAsync(request, ProfileId);
                return Ok(budget);
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
        public async Task<ActionResult<Budget>> Put(BudgetUpdateDto request)
        {
            try
            {
                var budgetDb = await budgetService.GetAsync(request.Id);
                if (budgetDb is null)
                    return NotFound("Budget não encontrada.");

                budgetDb.Month = request.Month;
                budgetDb.Year = request.Year;

                var budget = await budgetService.PutAsync(budgetDb);
                return Ok(budget);
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

        [HttpDelete("{budgetId}")]
        public async Task<ActionResult<bool>> Delete(Guid budgetId)
        {
            try
            {
                var budgetDb = await budgetService.GetAsync(budgetId);
                if (budgetDb is null)
                    return NotFound("Budget não encontrada.");

                var deleted = await budgetService.DeleteAsync(budgetDb);
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