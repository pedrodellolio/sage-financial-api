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
    public class BudgetService(AppDbContext context) : IBudgetService
    {
        public async Task<Budget?> GetAsync(Guid budgetId)
        {
            return await context.Budgets.FindAsync(budgetId);
        }

        public async Task<Budget?> GetByMonthAndYearAsync(int month, int year, Guid profileId)
        {
            return await context.Budgets.FirstOrDefaultAsync(b => b.Month == month && b.Year == year && b.ProfileId == profileId);
        }

        public async Task<ICollection<Budget>> GetAllAsync(Guid profileId)
        {
            return await context.Budgets.Where(l => l.ProfileId == profileId).ToListAsync();
        }

        public async Task<Budget> PostAsync(BudgetDto request, Guid profileId)
        {
            var newBudget = new Budget
            {
                Month = request.Month,
                Year = request.Year,
                ProfileId = profileId
            };

            context.Budgets.Add(newBudget);
            await context.SaveChangesAsync();
            return newBudget;
        }

        public async Task<Budget> PutAsync(Budget budget)
        {
            context.Budgets.Update(budget);
            await context.SaveChangesAsync();
            return budget;
        }

        public async Task<bool> DeleteAsync(Budget budget)
        {
            context.Budgets.Remove(budget);
            var result = await context.SaveChangesAsync();
            return result > 0;
        }
    }
}