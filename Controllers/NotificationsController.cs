using Microsoft.AspNetCore.Mvc;
using SageFinancialAPI.Entities;
using SageFinancialAPI.Models;
using SageFinancialAPI.Services;

namespace SageFinancialAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController(INotificationService notificationService, IAuthService authService) : BaseController
    {
        [HttpGet("all")]
        public async Task<ActionResult<ICollection<Notification>>> GetAll()
        {
            try
            {
                var notifications = await notificationService.GetAllAsync(ProfileId);
                return Ok(notifications);
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

        [HttpPut("toggle/{id}")]
        public async Task<ActionResult<Notification>> Toggle(Guid id)
        {
            try
            {
                var notificationDb = await notificationService.GetAsync(id, ProfileId);
                if (notificationDb is null)
                    return NotFound("Notificação não encontrada.");

                notificationDb.IsEnabled = !notificationDb.IsEnabled;
                var label = await notificationService.PutAsync(notificationDb);
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

        [HttpPost]
        public async Task<ActionResult<Notification>> Post(NotificationDto request)
        {
            try
            {
                var notification = await notificationService.PostAsync(request, ProfileId);
                return Ok(notification);
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

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            try
            {
                var notificationDb = await notificationService.GetAsync(id, ProfileId);
                if (notificationDb is null)
                    return NotFound("Notificação não encontrada.");

                var deleted = await notificationService.DeleteAsync(notificationDb);
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

        [HttpPost("save-token")]
        public async Task<ActionResult> SaveToken([FromBody] string token)
        {
            try
            {
                await authService.SaveNotificationTokenAsync(token, UserId);
                return Ok();
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