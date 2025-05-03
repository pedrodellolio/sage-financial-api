using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Validations;
using Newtonsoft.Json;
using SageFinancialAPI.Data;
using SageFinancialAPI.Entities;
using SageFinancialAPI.Models;

namespace SageFinancialAPI.Services
{
    public class BudgetGoalService(AppDbContext context, IBudgetService budgetService, INotificationService notificationService) : IBudgetGoalService
    {
        public async Task<BudgetGoal?> GetAsync(Guid budgetGoalId)
        {
            return await context.BudgetGoals
                .Include(t => t.Label)
                .FirstOrDefaultAsync(t => t.Id == budgetGoalId);
        }

        public async Task<ICollection<BudgetGoal>> GetAllAsync(Guid budgetId)
        {
            return await context.BudgetGoals
                .Include(bg => bg.Budget)
                .Include(bg => bg.Label)
                .Where(bg => bg.Budget.ProfileId == budgetId)
                .ToListAsync();
        }

        public async Task<ICollection<BudgetGoal>> GetByBudgetMonthAndYearAsync(int month, int year, Guid profileId)
        {
            var result = await context.BudgetGoals
                .Include(bg => bg.Budget)
                .Include(bg => bg.Label)
                .Where(bg => bg.Budget.Month == month
                            && bg.Budget.Year == year
                            && bg.Budget.ProfileId == profileId).ToListAsync();
            return result;
        }

        public async Task<BudgetGoal> PostAsync(BudgetGoalDto request, Guid profileId)
        {
            var budget = await budgetService.GetByMonthAndYearAsync(request.Month, request.Year, profileId);
            if (budget is null)
                budget = await budgetService.PostAsync(new BudgetDto { Month = request.Month, Year = request.Year }, profileId);

            var newBudgetGoal = new BudgetGoal
            {
                Value = request.Value,
                Type = request.Type,
                LabelId = request.LabelId,
                BudgetId = budget.Id
            };

            context.BudgetGoals.Add(newBudgetGoal);
            context.Notifications.Add(new Notification
            {
                BudgetGoal = newBudgetGoal,
                ProfileId = profileId
            });

            var label = await context.Labels.FirstOrDefaultAsync(l => l.Id == request.LabelId);
            if (label is null)
                throw new ApplicationException("Categoria não encontrada");

            ScheduleGoalLimitNotification(newBudgetGoal.Id, label.Title, profileId);

            await context.SaveChangesAsync();
            return newBudgetGoal;
        }

        public async Task<BudgetGoal> PutAsync(BudgetGoal budgetGoal)
        {
            context.BudgetGoals.Update(budgetGoal);
            await context.SaveChangesAsync();
            return budgetGoal;
        }

        public async Task<bool> DeleteAsync(BudgetGoal budgetGoal)
        {
            context.BudgetGoals.Remove(budgetGoal);
            var result = await context.SaveChangesAsync();
            return result > 0;
        }

        [AutomaticRetry(Attempts = 3)]
        public async Task ProcessGoalLimitNotification(Guid budgetGoalId, string labelTitle, Guid profileId)
        {
            var notification = await notificationService.GetByBudgetGoalAsync(budgetGoalId, profileId);
            if (notification is not null && notification.IsEnabled)
            {
                // Schedule notifications to alert the user when a goal's limit is reached
                await notificationService.SendNotificationByProfileAsync(
                    profileId,
                    $"Atenção!",
                    $"Fique atento! Você está quase no limite de gastos para {labelTitle}.");
            }
        }

        private void ScheduleGoalLimitNotification(Guid budgetGoalId, string labelTitle, Guid profileId)
        {
            try
            {
                string jobId = $"Notification-{budgetGoalId}";
                RecurringJob.AddOrUpdate(
                     jobId,
                     () => ProcessGoalLimitNotification(budgetGoalId, labelTitle, profileId),
                     Cron.Never);
            }
            catch
            {
                throw;
            }
        }
    }
}