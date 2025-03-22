using SageFinancialAPI.Entities;
using SageFinancialAPI.Models;

namespace SageFinancialAPI.Services
{
    public interface IProfileService
    {
        Task<Profile?> GetAsync(Guid profileId);
        Task<Profile?> GetDefaultAsync(Guid userId);
        Task<Profile?> GetByTitleAsync(string title);
        Task<ICollection<Profile>> GetAllAsync(Guid userId);
        Task<ICollection<ProfileBalanceDto>> GetAllProfileBalanceAsync(int month, int year, Guid userId);
        Task<Profile> PostAsync(ProfileDto request, Guid userId);
        Task<Profile> PutAsync(Profile profile);
        Task<bool> DeleteAsync(Profile profile);
    }
}