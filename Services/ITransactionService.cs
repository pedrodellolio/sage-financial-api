using SageFinancialAPI.Entities;
using SageFinancialAPI.Models;

namespace SageFinancialAPI.Services
{
    public interface ITransactionService
    {
        Task<Transaction?> GetAsync(Guid transactionId);
        Task<ICollection<Transaction>> GetAllAsync(Guid profileId);
        Task<ICollection<Transaction>> GetByMonthAndYearAsync(int month, int year, Guid profileId);
        Task<ICollection<Transaction>> GetByPeriodAsync(DateTime start, DateTime end, Guid profileId, TransactionType? type = null);
        Task<Transaction> PostAsync(TransactionDto request, Guid profileId);
        Task<Transaction> PutAsync(Transaction wallet);
        Task<bool> DeleteAsync(Transaction Transaction);
    }
}