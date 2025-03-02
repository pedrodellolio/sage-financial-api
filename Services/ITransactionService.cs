using SageFinancialAPI.Entities;
using SageFinancialAPI.Models;

namespace SageFinancialAPI.Services
{
    public interface ITransactionService
    {
        Task<Transaction?> GetAsync(Guid transactionId);
        Task<ICollection<Transaction>> GetAllAsync(Guid profileId);
        Task<ICollection<Transaction>> GetAllByWalletAsync(Guid walletId);
        Task<ICollection<Transaction>> GetAllByMonthAndYearAsync(int month, int year, Guid profileId);
        Task<ICollection<Transaction>> GetAllByMonthAndYearLabelAsync(int month, int year, Guid labelId, Guid profileId, TransactionType? type);
        Task<ICollection<Transaction>> GetByPeriodAsync(DateTime start, DateTime end, Guid profileId, TransactionType? type = null);
        Task<Transaction> PostAsync(TransactionDto request, Guid profileId);
        Task<Transaction> PutAsync(Transaction wallet, decimal oldValue);
        Task<bool> DeleteAsync(Transaction Transaction);
    }
}