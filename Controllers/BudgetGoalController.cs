using Microsoft.AspNetCore.Mvc;
using SageFinancialAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using SageFinancialAPI.Models;
using SageFinancialAPI.Services;
using Newtonsoft.Json;

namespace SageFinancialAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BudgetGoalController(IBudgetGoalService budgetGoalService) : BaseController
    {
        [HttpGet("{budgetGoalId}")]
        public async Task<ActionResult<BudgetGoal?>> Get(Guid budgetGoalId)
        {
            try
            {
                var result = await budgetGoalService.GetAsync(budgetGoalId);
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

        [HttpGet("get-by-month-year")]
        public async Task<ActionResult<BudgetGoal?>> GetByBudgetMonthAndYear(int month, int year)
        {
            try
            {
                var result = await budgetGoalService.GetByBudgetMonthAndYearAsync(month, year, ProfileId);
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
        public async Task<ActionResult<ICollection<BudgetGoal>>> GetAll()
        {
            try
            {
                var result = await budgetGoalService.GetAllAsync(ProfileId);
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
        public async Task<ActionResult<BudgetGoal>> Post(BudgetGoalDto request)
        {
            try
            {
                BudgetGoal budgetGoal = await budgetGoalService.PostAsync(request, ProfileId);
                return Ok(budgetGoal);
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
        public async Task<ActionResult<BudgetGoal>> Put(BudgetGoalDto request)
        {
            try
            {
                var budgetGoalDb = await budgetGoalService.GetAsync(request.Id);
                if (budgetGoalDb is null)
                    return NotFound("BudgetGoal não encontrada.");

                budgetGoalDb.Value = request.Value;
                budgetGoalDb.Type = request.Type;
                budgetGoalDb.LabelId = request.LabelId;

                var budgetGoal = await budgetGoalService.PutAsync(budgetGoalDb);
                return Ok(budgetGoal);
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

        [HttpDelete("{budgetGoalId}")]
        public async Task<ActionResult<bool>> Delete(Guid budgetGoalId)
        {
            try
            {
                var budgetGoalDb = await budgetGoalService.GetAsync(budgetGoalId);
                if (budgetGoalDb is null)
                    return NotFound("BudgetGoal não encontrada.");

                var deleted = await budgetGoalService.DeleteAsync(budgetGoalDb, ProfileId);
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