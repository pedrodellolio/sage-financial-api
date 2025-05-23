using SageFinancialAPI.Entities;
using SageFinancialAPI.Models;

namespace SageFinancialAPI.Services
{
    public interface IWalletService
    {
        Task<Wallet?> GetAsync(Guid WalletId);
        Task<ICollection<Wallet>> GetAllAsync(Guid profileId);
        Task<ICollection<Wallet>> GetByPeriodAsync(DateTime start, DateTime end, Guid profileId);
        Task<Wallet?> GetByMonthAndYearAsync(int month, int year, Guid profileId);
        Task<Wallet> PostAsync(int month, int year, Guid profileId);
        Task<Wallet> CreateOrUpdateAsync(TransactionDto request, Guid profileId);
        Task<Wallet> PutAsync(Wallet wallet);
        Task<Wallet?> IncrementAsync(Transaction transaction, decimal oldValue);
        Task<bool> DeleteAsync(Wallet Wallet);
        Task<Wallet> PatchAsync(Wallet wallet);
    }
}