using SageFinancialAPI.Entities;
using SageFinancialAPI.Models;

namespace SageFinancialAPI.Services
{
    public interface IFileService
    {
        Task<Entities.File> PostAsync(FileDto request, Guid profileId);
    }
}