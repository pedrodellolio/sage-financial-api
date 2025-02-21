using SageFinancialAPI.Entities;
using SageFinancialAPI.Models;

namespace SageFinancialAPI.Services
{
    public interface IBudgetService
    {
        Task<Budget?> GetAsync(Guid budgetId);
        Task<Budget?> GetByMonthAndYearAsync(int month, int year, Guid profileId);
        Task<ICollection<Budget>> GetAllAsync(Guid profileId);
        Task<Budget> PostAsync(BudgetDto request, Guid profileId);
        Task<Budget> PutAsync(Budget budget);
        Task<bool> DeleteAsync(Budget Budget);
    }
}