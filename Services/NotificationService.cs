using Expo.Server.Client;
using Expo.Server.Models;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using SageFinancialAPI.Data;
using SageFinancialAPI.Entities;
using SageFinancialAPI.Models;
using SageFinancialAPI.Validators;

namespace SageFinancialAPI.Services
{
    public class NotificationService(AppDbContext context, IAuthService authService) : INotificationService
    {
        private readonly PushApiClient _expoClient = new();

        public async Task<Notification?> GetAsync(Guid notificationId)
        {
            return await context.Notifications.FindAsync(notificationId);
        }

        public async Task<Notification?> GetAsync(Guid transactionId, Guid profileId)
        {
            return await context.Notifications.Include(x => x.Transaction).FirstOrDefaultAsync(p => p.ProfileId == profileId && p.TransactionId == transactionId);
        }

        public async Task<Notification?> GetByBudgetGoalAsync(Guid budgetGoalId, Guid profileId)
        {
            return await context.Notifications.Include(x => x.BudgetGoal).FirstOrDefaultAsync(p => p.ProfileId == profileId && p.BudgetGoalId == budgetGoalId);
        }

        public async Task<ICollection<Notification>> GetAllAsync(Guid profileId)
        {
            return await context.Notifications.Include(n => n.Transaction).Include(n => n.BudgetGoal).ThenInclude(n => n.Label).Where(p => p.ProfileId == profileId).ToListAsync();
        }

        public async Task<Notification> PutAsync(Notification notification)
        {
            context.Notifications.Update(notification);
            await context.SaveChangesAsync();
            return notification;
        }

        public async Task<Notification> PostAsync(NotificationDto request, Guid profileId)
        {
            var newNotification = new Notification
            {
                TransactionId = request.TransactionId,
                BudgetGoalId = request.BudgetGoalId,
                ProfileId = profileId
            };

            context.Notifications.Add(newNotification);
            await context.SaveChangesAsync();
            return newNotification;
        }

        public async Task<bool> DeleteAsync(Notification notification)
        {
            RemoveScheduledJobs(notification);
            context.Notifications.Remove(notification);
            var result = await context.SaveChangesAsync();
            return result > 0;
        }

        public async Task SendNotificationAsync(string expoPushToken, string title, string message)
        {
            if (!ExpoPushTokenValidator.IsValidExpoPushToken(expoPushToken))
                throw new ArgumentException("Expo push token inv�lido", nameof(expoPushToken));

            var pushTicketRequest = new PushTicketRequest
            {
                PushTo = [expoPushToken],
                PushTitle = title,
                PushBody = message,
                PushData = new { notificationId = Guid.NewGuid().ToString() }
            };

            await _expoClient.PushSendAsync(pushTicketRequest);
        }

        public async Task SendNotificationAsync(Guid userId, string title, string message)
        {
            var user = await authService.GetAsync(userId);
            if (user is null)
                throw new ApplicationException("Usu�rio n�o encontrado");

            if (!ExpoPushTokenValidator.IsValidExpoPushToken(user.PushNotificationsToken))
                throw new ArgumentException("Expo push token inv�lido", nameof(user.PushNotificationsToken));

            var pushTicketRequest = new PushTicketRequest
            {
                PushTo = [user.PushNotificationsToken],
                PushTitle = title,
                PushBody = message
            };

            await _expoClient.PushSendAsync(pushTicketRequest);
        }

        public async Task SendNotificationByProfileAsync(Guid profileId, string title, string message)
        {
            var user = await authService.GetByProfileIdAsync(profileId);
            if (user is null)
                throw new ApplicationException("Usu�rio n�o encontrado");

            var pushTicketRequest = new PushTicketRequest
            {
                PushTo = [user.PushNotificationsToken],
                PushTitle = title,
                PushBody = message,
            };

            await _expoClient.PushSendAsync(pushTicketRequest);
        }

        private static void RemoveScheduledJobs(Notification notification)
        {
            if (notification.TransactionId.HasValue)
            {
                string transactionJobId = $"RecurringTransaction-{notification.TransactionId.Value}";
                string notificationJobId = $"Notification-{notification.TransactionId.Value}";
                RecurringJob.RemoveIfExists(transactionJobId);
                RecurringJob.RemoveIfExists(notificationJobId);
            }

            if (notification.BudgetGoalId.HasValue)
            {
                string notificationLimitJobId = $"Notification-{notification.BudgetGoalId.Value}";
                RecurringJob.RemoveIfExists(notificationLimitJobId);
            }
        }
    }
}