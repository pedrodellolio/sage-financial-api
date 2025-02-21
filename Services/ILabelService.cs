using SageFinancialAPI.Entities;
using SageFinancialAPI.Models;

namespace SageFinancialAPI.Services
{
    public interface ILabelService
    {
        Task<Label?> GetAsync(Guid labelId);
        Task<ICollection<Label>> GetAllAsync(Guid profileId);
        Task<Label> PostAsync(LabelDto request, Guid profileId);
        Task<Label> PutAsync(Label label);
        Task<bool> DeleteAsync(Label Label);
    }
}