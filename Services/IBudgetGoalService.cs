using SageFinancialAPI.Entities;
using SageFinancialAPI.Models;

namespace SageFinancialAPI.Services
{
    public interface IBudgetGoalService
    {
        Task<BudgetGoal?> GetAsync(Guid budgetGoalId);
        Task<ICollection<BudgetGoal>> GetByBudgetMonthAndYearAsync(int month, int year, Guid profileId);
        Task<ICollection<BudgetGoal>> GetAllAsync(Guid budgetId);
        Task<BudgetGoal> PostAsync(BudgetGoalDto request, Guid profileId);
        Task<BudgetGoal> PutAsync(BudgetGoal budgetGoal);
        Task<bool> DeleteAsync(BudgetGoal BudgetGoal);
    }
}