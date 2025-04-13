using Microsoft.EntityFrameworkCore.Storage;
using SageFinancialAPI.Entities;
using SageFinancialAPI.Models;

namespace SageFinancialAPI.Services
{
    public interface ITransactionService
    {
        Task<Transaction?> GetAsync(Guid transactionId);
        Task<ICollection<Transaction>> GetAllAsync(Guid profileId);
        Task<ICollection<Transaction>> GetAllByWalletAsync(Guid walletId);
        Task<ICollection<Entities.Transaction>> GetAllInstallmentsAsync(Guid transactionId);
        Task<ICollection<Transaction>> GetAllByMonthAndYearAsync(int month, int year, Guid profileId, string? input = null, TransactionFiltersDto? filters = null);
        Task<ICollection<Transaction>> GetAllByMonthAndYearLabelAsync(int month, int year, Guid labelId, Guid profileId, TransactionType? type);
        Task<ICollection<Transaction>> GetByPeriodAsync(DateTime start, DateTime end, Guid profileId, TransactionType? type = null);
        Task<bool> PostAsync(TransactionDto request, Guid profileId, bool scheduleRecurrence = true);
        Task<bool> PostManyAsync(TransactionDto request, Guid profileId);
        Task<bool> PutAsync(TransactionDto newTransaction, Transaction oldTransaction);
        Task<bool> DeleteAsync(Transaction Transaction);
    }
}