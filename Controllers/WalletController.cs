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
    public class WalletController(IWalletService walletService) : BaseController
    {
        [HttpGet("{walletId}")]
        public async Task<ActionResult<Wallet?>> Get(Guid walletId)
        {
            try
            {
                var result = await walletService.GetAsync(walletId);
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
        public async Task<ActionResult<ICollection<Wallet>>> GetByPeriod(DateTime start, DateTime end)
        {
            try
            {
                var result = await walletService.GetByPeriodAsync(start, end, ProfileId);
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
        public async Task<ActionResult<ICollection<Wallet>>> GetByMonthAndYear(int month, int year)
        {
            try
            {
                var result = await walletService.GetByMonthAndYearAsync(month, year, ProfileId);
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
        public async Task<ActionResult<ICollection<Wallet>>> GetAll()
        {
            try
            {
                var result = await walletService.GetAllAsync(ProfileId);
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
        public async Task<ActionResult<Wallet>> Post(WalletDto request)
        {
            try
            {
                var walletDb = await walletService.GetByMonthAndYearAsync(request.Month, request.Year, ProfileId);
                if (walletDb is not null)
                    return Conflict("Já existe uma Wallet para esse período.");

                Wallet wallet = await walletService.PostAsync(request.Month, request.Year, ProfileId);
                return Ok(wallet);
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
        public async Task<ActionResult<Wallet>> Put(WalletUpdateDto request)
        {
            try
            {
                var walletDb = await walletService.GetAsync(request.Id);
                if (walletDb is null)
                    return NotFound("Wallet não encontrada.");

                walletDb.ExpensesBrl = request.ExpensesBrl;
                walletDb.IncomesBrl = request.IncomesBrl;

                var wallet = await walletService.PutAsync(walletDb);
                return Ok(wallet);
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

        [HttpDelete("{walletId}")]
        public async Task<ActionResult<bool>> Delete(Guid walletId)
        {
            try
            {
                var walletDb = await walletService.GetAsync(walletId);
                if (walletDb is null)
                    return NotFound("Wallet não encontrada.");

                var deleted = await walletService.DeleteAsync(walletDb);
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

        [HttpPatch("sync")]
        public async Task<ActionResult<Wallet>> Sync([FromBody] PeriodDto request)
        {
            try
            {
                var walletDb = await walletService.GetByMonthAndYearAsync(request.Month, request.Year, ProfileId);
                if (walletDb is null)
                    return NotFound("Wallet não encontrada.");

                var patched = await walletService.PatchAsync(walletDb);
                return Ok(patched);
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