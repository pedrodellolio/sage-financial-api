using SageFinancialAPI.Entities;
using SageFinancialAPI.Models;

namespace SageFinancialAPI.Services
{
    public interface ILabelService
    {
        Task<Label?> GetAsync(Guid labelId);
        Task<Label?> GetByTitleAsync(string title);
        Task<ICollection<LabelDto>> GetAllAsync(Guid profileId);
        Task<ICollection<LabelDto>> GetAllNotInBudgetGoalAsync(int month, int year, Guid profileId);
        Task<Label> PostAsync(LabelDto request, Guid profileId);
        Task<Label> PutAsync(Label label);
        Task<bool> DeleteAsync(Label Label);
    }
}