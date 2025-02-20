using SageFinancialAPI.Entities;
using SageFinancialAPI.Models;

namespace SageFinancialAPI.Services
{
    public interface IProfileService
    {
        Task<Profile?> GetAsync(Guid profileId);
        Task<ICollection<Profile>> GetAllAsync();
        Task<Profile?> GetByTitleAsync(string title);
        Task<Profile> PostAsync(ProfileDto request);
        Task<Profile?> PutAsync(Profile request);
        Task<bool> DeleteAsync(Guid profileId);
    }
}