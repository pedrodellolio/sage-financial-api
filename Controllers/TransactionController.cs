using Microsoft.AspNetCore.Mvc;
using SageFinancialAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using SageFinancialAPI.Models;
using SageFinancialAPI.Services;
using System.Security.Claims;
using Newtonsoft.Json;
using SageFinancialAPI.Extensions;

namespace SageFinancialAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionController(ITransactionService transactionService) : BaseController
    {
        [HttpGet("{transactionId}")]
        public async Task<ActionResult<Transaction>> Get(Guid transactionId)
        {
            try
            {
                var result = await transactionService.GetAsync(transactionId);
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
        public async Task<ActionResult<ICollection<Transaction>>> GetByMonthAndYear(int month, int year, string? input = null, [FromQuery] TransactionFiltersDto? filters = null)
        {
            try
            {
                var result = await transactionService.GetAllByMonthAndYearAsync(month, year, ProfileId, input, filters);
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

        [HttpGet("get-by-period")]
        public async Task<ActionResult<ICollection<Transaction>>> GetByPeriod(DateTime start, DateTime end, TransactionType? type)
        {
            try
            {
                var result = await transactionService.GetByPeriodAsync(start, end, ProfileId, type);
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
        public async Task<ActionResult<ICollection<Transaction>>> GetAll()
        {
            try
            {
                var result = await transactionService.GetAllAsync(ProfileId);
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

        [HttpGet("total")]
        public async Task<ActionResult<decimal>> GetTotalExpenses(int month, int year, Guid labelId)
        {
            try
            {
                var result = await transactionService.GetAllByMonthAndYearLabelAsync(month, year, labelId, ProfileId, TransactionType.EXPENSE);
                var total = result.Sum(t => t.ValueBrl);
                return Ok(total);
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
        public async Task<ActionResult<bool>> Post(TransactionDto request)
        {
            try
            {
                var created = await transactionService.PostAsync(request, ProfileId);
                return Ok(created);
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
        public async Task<ActionResult<bool>> Put(TransactionUpdateDto request)
        {
            try
            {
                var transactionDb = await transactionService.GetAsync(request.Id);
                if (transactionDb is null)
                    return NotFound("Transaction não encontrada.");

                var updated = await transactionService.PutAsync(request, transactionDb);
                return Ok(updated);
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

        [HttpDelete("{transactionId}")]
        public async Task<ActionResult<bool>> Delete(Guid transactionId)
        {
            try
            {
                var transactionDb = await transactionService.GetAsync(transactionId);
                if (transactionDb is null)
                    return NotFound("Transaction não encontrada.");

                var deleted = await transactionService.DeleteAsync(transactionDb);
                return Ok(deleted);
            }
            catch (ApplicationException ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest("Ocorreu um erro inesperado.");
            }
        }
    }
}