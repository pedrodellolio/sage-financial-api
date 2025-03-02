using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SageFinancialAPI.Data;
using SageFinancialAPI.Entities;
using SageFinancialAPI.Models;

namespace SageFinancialAPI.Services
{
    public class BudgetGoalService(AppDbContext context, IBudgetService budgetService) : IBudgetGoalService
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
                LabelId = request.Label.Id,
                BudgetId = budget.Id
            };

            context.BudgetGoals.Add(newBudgetGoal);
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
    }
}