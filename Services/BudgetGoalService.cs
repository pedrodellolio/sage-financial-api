using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SageFinancialAPI.Data;
using SageFinancialAPI.Entities;
using SageFinancialAPI.Models;

namespace SageFinancialAPI.Services
{
    public class BudgetGoalService(AppDbContext context) : IBudgetGoalService
    {
        public async Task<BudgetGoal?> GetAsync(Guid budgetGoalId)
        {
            return await context.BudgetGoals.FindAsync(budgetGoalId);
        }

        public async Task<ICollection<BudgetGoal>> GetAllAsync(Guid budgetId)
        {
            return await context.BudgetGoals.Where(l => l.Budget.ProfileId == budgetId).ToListAsync();
        }

        public async Task<ICollection<BudgetGoal>> GetByBudgetMonthAndYearAsync(int month, int year, Guid profileId)
        {
            return await context.BudgetGoals.Where(l => l.Budget.Month == month && l.Budget.Year == year && l.Budget.ProfileId == profileId).ToListAsync();
        }

        public async Task<BudgetGoal> PostAsync(BudgetGoalDto request, Guid budgetId)
        {
            var newBudgetGoal = new BudgetGoal
            {
                Value = request.Value,
                Type = request.Type,
                BudgetId = budgetId
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