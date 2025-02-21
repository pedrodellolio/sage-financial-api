using Microsoft.AspNetCore.Mvc;
using SageFinancialAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using SageFinancialAPI.Models;
using SageFinancialAPI.Services;
using System.Security.Claims;

namespace SageFinancialAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionController(ITransactionService transactionService) : BaseController
    {
        [HttpGet("{transactionId}")]
        public async Task<ActionResult<Transaction?>> Get(Guid transactionId)
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

        [HttpPost]
        public async Task<ActionResult<Transaction>> Post(TransactionDto request)
        {
            try
            {
                Transaction transaction = await transactionService.PostAsync(request, ProfileId);
                return Ok(transaction);
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
        public async Task<ActionResult<Transaction>> Put(TransactionUpdateDto request)
        {
            try
            {
                var transactionDb = await transactionService.GetAsync(request.Id);
                if (transactionDb is null)
                    return NotFound("Transaction não encontrada.");

                transactionDb.Title = request.Title;
                transactionDb.Type = request.Type;
                transactionDb.ValueBrl = request.ValueBrl;

                var transaction = await transactionService.PutAsync(transactionDb);
                return Ok(transaction);
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
                return BadRequest(ex.Message);
            }
            catch
            {
                return BadRequest("Ocorreu um erro inesperado.");
            }
        }
    }
}