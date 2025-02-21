using SageFinancialAPI.Entities;
using SageFinancialAPI.Models;

namespace SageFinancialAPI.Services
{
    public interface IWalletService
    {
        Task<Wallet?> GetAsync(Guid WalletId);
        Task<ICollection<Wallet>> GetAllAsync(Guid profileId);
        Task<Wallet?> GetByMonthAndYearAsync(int month, int year, Guid profileId);
        Task<Wallet> PostAsync(int month, int year, Guid profileId);
        Task<Wallet> CreateOrUpdateAsync(TransactionDto request, Guid profileId);
        Task<Wallet> PutAsync(Wallet wallet);
        Task<bool> DeleteAsync(Wallet Wallet);
    }
}